# -*- coding: utf-8 -*-
"""Sitemap engine.

Reads URL lists from txt/csv/xlsx/json files and writes WebScraper
sitemap JSON files using the configured templates.
"""
import copy
import csv
import io
import json
import os
import re
import sys
import traceback
import zipfile
from datetime import datetime
from pathlib import Path

TEMPLATE_TIENDAS = "PlantillaSitemapsTiendas.json"
TEMPLATE_BBVS = "PlantillaSitemapsBBvs.json"

STORES_TIENDAS = {"productostx", "holaproducto", "altinor", "hervaztrade", "hervaz trade"}
STORES_BBVS = {"bbvs_template", "bbvsbb2_2da", "bbvsbb2"}

SITEMAP_ID_ALLOWED_RE = re.compile(r"[^a-zA-Z0-9_()+-]")
URL_RE = re.compile(r'https?://[^\s"\']+', re.IGNORECASE)


def _app_dir():
    return Path(getattr(sys, "_MEIPASS", Path(__file__).resolve().parent))


def _reset_cwd():
    try:
        app_dir = _app_dir()
        try:
            os.chdir(os.path.relpath(str(app_dir)))
        except Exception:
            os.chdir(app_dir)
    except Exception:
        pass


def sanitize_name(text, default_value):
    """Normalize output names to allowed characters."""
    repl = (text or "").strip()
    repl = repl.replace(" ", "_").replace("-", "_")
    repl = SITEMAP_ID_ALLOWED_RE.sub("_", repl)
    repl = re.sub(r"_+", "_", repl)
    repl = repl.strip("_").strip(".")
    return repl or default_value


def sanitize_sitemap_id(text):
    return sanitize_name(text, "sitemap")


def sanitize_folder_name(text):
    return sanitize_name(text, "sitemap")


def normalize_store(value):
    return re.sub(r"\s+", "", (value or "").strip().lower())


def select_template(store):
    normalized = normalize_store(store)
    if normalized in STORES_BBVS:
        return TEMPLATE_BBVS
    return TEMPLATE_TIENDAS


def load_template(template_name):
    template_path = Path(template_name)
    if not template_path.exists():
        template_path = _app_dir() / template_name
    if not template_path.exists():
        raise FileNotFoundError(f"Sitemap template not found: {template_path}")
    return json.loads(template_path.read_text(encoding="utf-8"))


def read_text_fallback(path):
    for enc in ("utf-8-sig", "utf-8", "latin-1"):
        try:
            return Path(path).read_text(encoding=enc)
        except Exception:
            continue
    return Path(path).read_text(encoding="utf-8", errors="ignore")


def extract_urls_from_text(text):
    urls = []
    for line in (text or "").splitlines():
        cleaned = (line or "").strip()
        if not cleaned:
            continue
        if cleaned.lower() in ("start_url", "starturl"):
            continue
        matches = URL_RE.findall(cleaned)
        if matches:
            urls.extend(matches)
    return urls


def read_urls_from_text(path):
    return extract_urls_from_text(read_text_fallback(path))


def read_urls_from_csv(path):
    text = read_text_fallback(path)
    if not text.strip():
        return []

    sample = text[:4096]
    try:
        dialect = csv.Sniffer().sniff(sample, delimiters=";\t,|")
        delimiter = dialect.delimiter
    except Exception:
        if ";" in sample and "," not in sample:
            delimiter = ";"
        elif "\t" in sample:
            delimiter = "\t"
        elif "|" in sample:
            delimiter = "|"
        else:
            delimiter = ","

    urls = []
    reader = csv.reader(io.StringIO(text), delimiter=delimiter)
    for row in reader:
        for cell in row:
            cleaned = str(cell or "").strip()
            if not cleaned:
                continue
            if cleaned.lower() in ("start_url", "starturl"):
                continue
            matches = URL_RE.findall(cleaned)
            if matches:
                urls.extend(matches)
    return urls


def _extract_urls_from_json_value(value, urls):
    if isinstance(value, str):
        matches = URL_RE.findall(value)
        if matches:
            urls.extend(matches)
        return

    if isinstance(value, list):
        for item in value:
            _extract_urls_from_json_value(item, urls)
        return

    if isinstance(value, dict):
        if "startUrl" in value:
            _extract_urls_from_json_value(value["startUrl"], urls)
        if "start_url" in value:
            _extract_urls_from_json_value(value["start_url"], urls)
        if "url" in value:
            _extract_urls_from_json_value(value["url"], urls)
        for key, val in value.items():
            if key in ("startUrl", "start_url", "url"):
                continue
            _extract_urls_from_json_value(val, urls)


def read_urls_from_json(path):
    text = read_text_fallback(path)
    if not text.strip():
        return []
    payload = json.loads(text)
    urls = []
    _extract_urls_from_json_value(payload, urls)
    return urls


def read_urls_from_excel(path):
    try:
        from openpyxl import load_workbook
    except Exception as exc:
        raise RuntimeError("openpyxl is required to read .xlsx files") from exc

    urls = []
    wb = load_workbook(path, read_only=True, data_only=True)
    try:
        for ws in wb.worksheets:
            for row in ws.iter_rows(values_only=True):
                for cell in row:
                    if cell is None:
                        continue
                    cleaned = str(cell).strip()
                    if not cleaned:
                        continue
                    if cleaned.lower() in ("start_url", "starturl"):
                        continue
                    matches = URL_RE.findall(cleaned)
                    if matches:
                        urls.extend(matches)
    finally:
        wb.close()
    return urls


def read_urls_from_file(path):
    """Read URLs from txt/csv/xlsx/json based on file extension."""
    ext = Path(path).suffix.lower()
    if ext == ".csv":
        return read_urls_from_csv(path)
    if ext == ".json":
        return read_urls_from_json(path)
    if ext == ".xls":
        raise RuntimeError("Unsupported Excel format .xls. Convert to .xlsx.")
    if ext == ".xlsx":
        return read_urls_from_excel(path)
    return read_urls_from_text(path)


def build_sitemap_payload(title, urls, template):
    payload = copy.deepcopy(template)
    payload["_id"] = title
    payload["startUrl"] = urls
    return payload


def ensure_folder(path):
    Path(path).mkdir(parents=True, exist_ok=True)


def zip_outputs(files, target_zip):
    with zipfile.ZipFile(target_zip, "w", compression=zipfile.ZIP_DEFLATED) as z:
        for fp in files:
            z.write(fp, arcname=Path(fp).name)
    return target_zip


def _error(message, tb=None):
    return {"ok": False, "error": message, "traceback": tb or ""}


def handle_process(data):
    """Generate sitemap JSON files from input URL batches."""
    _reset_cwd()

    input_files = data.get("input_files") or []
    input_files = [f for f in input_files if f]
    if not input_files:
        return _error("Missing input_files")

    for fp in input_files:
        if not Path(fp).exists():
            return _error(f"Input file not found: {fp}")

    output_dir = (data.get("output_dir") or "").strip() or str(Path.home() / "Downloads")
    base_name = (data.get("base_name") or "").strip()
    if not base_name:
        return _error("Missing base_name")

    store_label = (data.get("store") or "").strip()
    base_label = f"{store_label}_{base_name}" if store_label else base_name

    template_name = select_template(data.get("store"))
    template = load_template(template_name)

    ddmmaa = datetime.now().strftime("%d%m%y")
    hhmm = datetime.now().strftime("%H%M")
    folder_name = f"{sanitize_folder_name(base_label)}_{ddmmaa}_{hhmm}"
    work_dir = Path(output_dir) / folder_name
    ensure_folder(str(work_dir))

    base_id = sanitize_sitemap_id(base_label)
    total = len(input_files)
    output_files = []

    for idx, fp in enumerate(input_files, start=1):
        try:
            urls = read_urls_from_file(fp)
        except Exception as exc:
            return _error(f"Failed to read {fp}: {exc}")
        if not urls:
            return _error(f"No URLs found in: {fp}")

        if total > 1:
            title = f"{base_id}_{idx}"
        else:
            title = base_id

        out_path = work_dir / f"{title}.json"
        payload = build_sitemap_payload(title, urls, template)
        with out_path.open("w", encoding="utf-8") as f:
            json.dump(payload, f, ensure_ascii=True, separators=(",", ":"))
        output_files.append(str(out_path))

    zip_path = ""
    if bool(data.get("zip_output")):
        zip_path = str(Path(output_dir) / f"{sanitize_folder_name(base_label)}.zip")
        zip_outputs(output_files, zip_path)

    return {
        "ok": True,
        "output_folder": str(work_dir),
        "zip_path": zip_path,
        "output_files": output_files,
    }


def handle_request(data):
    action = (data.get("action") or "").strip().lower()
    if action == "process":
        return handle_process(data)
    return _error("Unknown action")


def main():
    raw = sys.stdin.read()
    if not raw.strip():
        resp = _error("No input received")
    else:
        try:
            data = json.loads(raw)
            resp = handle_request(data)
        except Exception as exc:
            resp = _error(str(exc), traceback.format_exc())
    sys.stdout.write(json.dumps(resp, ensure_ascii=True))


if __name__ == "__main__":
    main()
