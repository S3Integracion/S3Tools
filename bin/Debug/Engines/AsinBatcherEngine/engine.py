# -*- coding: utf-8 -*-
"""Asin Batcher engine.

Reads ASIN input files, removes duplicates, builds Amazon URLs,
splits into batches, and returns JSON to the WinForms client.
"""
import json
import os
import re
import sys
import shutil
import zipfile
import traceback
from pathlib import Path
from datetime import datetime
import random

DEFAULT_BATCHES = 30
DEFAULT_MARKET = "US"
MARKETS = ["MX", "US"]
ORDER_CHOICES = ["Ordenado", "Inverso", "Aleatorio"]

STORES_LEFT = ["ProductosTX", "Holaproducto", "Altinor", "HervazTrade"]
STORES_RIGHT = ["BBvs_Template", "BBvsBB2_2da", "BBvsBB2"]
ALL_STORES = STORES_LEFT + STORES_RIGHT

NAME_ALLOWED_RE = re.compile(r"[^a-zA-Z0-9_()+-]")


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


def _purge_numpy_pandas_modules():
    to_del = [m for m in list(sys.modules) if m.startswith("numpy") or m.startswith("pandas")]
    for m in to_del:
        try:
            del sys.modules[m]
        except Exception:
            pass


def _strip_suspicious_paths(selected_dir):
    new_sp = []
    for p in list(sys.path):
        if not p:
            continue
        try:
            pp = Path(p)
        except Exception:
            continue
        if selected_dir and pp.resolve() == selected_dir.resolve():
            continue
        tail = pp.name.lower()
        if ("numpy" in tail) or ("pandas" in tail):
            continue
        new_sp.append(p)
    sys.path[:] = new_sp


def sanitize_for_read(selected_path):
    _reset_cwd()
    os.environ.setdefault("PANDAS_IGNORE_CLIPBOARD", "1")
    p = Path(selected_path)
    selected_dir = p.parent if p.exists() else None
    _purge_numpy_pandas_modules()
    _strip_suspicious_paths(selected_dir)


def clean_asin(s):
    s = (s or "").strip().upper()
    return re.sub(r"[^A-Z0-9]", "", s)


def is_inventory_report(filename):
    base = Path(filename).name
    if re.fullmatch(r"Reporte\+de\+inventario\+\d{2}-\d{2}-\d{4}\.(txt|xlsx|xls)", base, re.IGNORECASE):
        return True
    try:
        with open(filename, "r", encoding="utf-8", errors="ignore") as f:
            first = f.readline()
        if "\t" in first:
            headers = [h.strip().lower() for h in first.split("\t")]
            return "asin" in headers
    except Exception:
        pass
    return False


def read_asins_from_inventory_txt(path):
    import csv
    p = Path(path)
    if not p.is_file():
        return []

    rows = []
    for enc in ("utf-8-sig", "latin-1"):
        try:
            with p.open("r", encoding=enc, errors="ignore", newline="") as f:
                reader = csv.reader(f, delimiter="\t")
                rows = list(reader)
            break
        except Exception:
            rows = []
            continue
    if not rows:
        return []

    asin_re = re.compile(r"\b[A-Z0-9]{10}\b")
    header = [c.strip().lower() for c in rows[0]] if rows else []
    asins = []

    if "asin" in header:
        idx = header.index("asin")
        for row in rows[1:]:
            if idx < len(row):
                cell = (row[idx] or "").strip().upper()
                m = asin_re.search(cell)
                if m:
                    asins.append(clean_asin(m.group(0)))
    else:
        for row in rows:
            for cell in row:
                m = asin_re.search(str(cell).strip().upper())
                if m:
                    asins.append(clean_asin(m.group(0)))

    return [a for a in asins if a]


def read_asins_from_inventory_excel(path):
    import pandas as pd
    df = pd.read_excel(path, dtype=str, engine="openpyxl")
    cols = [c.strip().lower() for c in df.columns]
    if "asin" not in cols:
        vals = [clean_asin(x or "") for x in df.iloc[:, 0].fillna("").tolist()]
        return [v for v in vals if v]
    asin_col = df.columns[cols.index("asin")]
    vals = [clean_asin(x or "") for x in df[asin_col].fillna("").tolist()]
    return [v for v in vals if v]


def read_asins_from_plain_txt(path):
    lines = Path(path).read_text(encoding="utf-8", errors="ignore").splitlines()
    vals = [clean_asin(x) for x in lines]
    return [v for v in vals if v]


def extract_asins_any(path):
    """Read ASINs from txt/xlsx and return (unique, duplicates)."""
    sanitize_for_read(path)
    p = Path(path)
    ext = p.suffix.lower()
    asins = []

    if ext in [".xlsx", ".xls"]:
        if is_inventory_report(path):
            asins = read_asins_from_inventory_excel(path)
        else:
            try:
                import pandas as pd
                df = pd.read_excel(path, dtype=str, engine="openpyxl")
                vals = [clean_asin(x or "") for x in df.iloc[:, 0].fillna("").tolist()]
                asins = [v for v in vals if v]
            except Exception:
                asins = []
    elif ext == ".txt":
        if is_inventory_report(path):
            asins = read_asins_from_inventory_txt(path)
        else:
            asins = read_asins_from_plain_txt(path)
    else:
        try:
            asins = read_asins_from_plain_txt(path)
        except Exception:
            asins = []

    uniques, dups, seen = [], [], set()
    for a in asins:
        if a in seen:
            dups.append(a)
        else:
            seen.add(a)
            uniques.append(a)
    return uniques, dups


def to_url(asin, market):
    if market == "US":
        return f"https://www.amazon.com/dp/{asin}?th=1"
    return f"https://www.amazon.com.mx/dp/{asin}?th=1"


def split_in_batches(items, batches):
    if batches <= 1:
        return [items]
    n = len(items)
    if n == 0:
        return [[] for _ in range(batches)]
    base = n // batches
    remainder = n % batches
    out = []
    start = 0
    for i in range(batches):
        count = base + (1 if i < remainder else 0)
        end = start + count
        out.append(items[start:end])
        start = end
    return out


def ensure_folder(path):
    Path(path).mkdir(parents=True, exist_ok=True)


# Normalize user names to allowed output chars.
def sanitize_filename(text):
    repl = (text or "").strip()
    repl = repl.replace(" ", "_").replace("-", "_")
    repl = NAME_ALLOWED_RE.sub("_", repl)
    repl = re.sub(r"_+", "_", repl)
    repl = repl.strip("_").strip(".")
    return repl or "archivo"
def write_batches_as_txt(batches_list, folder, store, market, base_label):
    out_files = []
    safe_base = sanitize_filename(base_label)
    total = len(batches_list)
    for idx, batch in enumerate(batches_list, start=1):
        if total > 1:
            fname = f"{safe_base}_{idx}.txt"
        else:
            fname = f"{safe_base}.txt"
        fpath = Path(folder) / fname
        with fpath.open("w", encoding="utf-8") as f:
            f.write("start_url\n")
            for asin in batch:
                f.write(to_url(asin, market) + "\n")
        out_files.append(str(fpath))
    return out_files


def zip_outputs(files, target_zip):
    with zipfile.ZipFile(target_zip, "w", compression=zipfile.ZIP_DEFLATED) as z:
        for fp in files:
            z.write(fp, arcname=Path(fp).name)
    return target_zip


def reorder_asins(uniques, mode):
    mode = (mode or "").lower()
    if mode == "inverso":
        return sorted(uniques, reverse=True)
    if mode == "aleatorio":
        shuffled = list(uniques)
        random.shuffle(shuffled)
        return shuffled
    return sorted(uniques)


def compute_store_from_selection(selected_store):
    if selected_store in ALL_STORES:
        return selected_store
    return ALL_STORES[0]


def export_duplicates_csv(dups, outdir):
    if not dups:
        return ""
    Path(outdir).mkdir(parents=True, exist_ok=True)
    fpath = Path(outdir) / f"duplicados_{datetime.now().strftime('%Y%m%d_%H%M%S')}.csv"
    with fpath.open("w", encoding="utf-8") as f:
        f.write("asin\n")
        seen = set()
        for a in dups:
            if a not in seen:
                seen.add(a)
                f.write(a + "\n")
    return str(fpath)


def _error(message, tb=None):
    return {"ok": False, "error": message, "traceback": tb or ""}


def _preview_response(uniques, dups):
    return {
        "ok": True,
        "total": len(uniques) + len(dups),
        "unique": len(uniques),
        "duplicates": len(dups),
    }


def handle_preview(data):
    """Return counts for UI preview without writing outputs."""
    input_path = (data.get("input_path") or "").strip()
    if not input_path:
        return _error("Missing input_path")
    if not Path(input_path).exists():
        return _error("Input file not found")
    uniques, dups = extract_asins_any(input_path)
    return _preview_response(uniques, dups)


def handle_export_duplicates(data):
    """Export duplicate ASINs to CSV."""
    input_path = (data.get("input_path") or "").strip()
    if not input_path:
        return _error("Missing input_path")
    if not Path(input_path).exists():
        return _error("Input file not found")
    outdir = (data.get("output_dir") or "").strip() or str(Path.home() / "Downloads")
    uniques, dups = extract_asins_any(input_path)
    csv_path = export_duplicates_csv(dups, outdir)
    return {
        "ok": True,
        "duplicates": len(dups),
        "csv_path": csv_path,
    }


def handle_process(data):
    """Generate URL batches and return output metadata."""
    input_path = (data.get("input_path") or "").strip()
    if not input_path:
        return _error("Missing input_path")
    if not Path(input_path).exists():
        return _error("Input file not found")

    outdir = (data.get("output_dir") or "").strip() or str(Path.home() / "Downloads")
    ensure_folder(outdir)

    market = data.get("market") if data.get("market") in MARKETS else DEFAULT_MARKET
    order = data.get("order") if data.get("order") in ORDER_CHOICES else ORDER_CHOICES[0]

    prefix1 = (data.get("name_prefix_1") or "").strip()
    prefix2 = (data.get("name_prefix_2") or "").strip()
    store_name = (data.get("store_name") or "").strip()
    use_new_name = bool(store_name or prefix1 or prefix2)

    if use_new_name:
        if not store_name:
            store_name = (data.get("store") or "").strip()
        if not store_name:
            return _error("Missing store_name")
        base_label = f"{prefix1}{prefix2}{store_name}"
        name_store = store_name
    else:
        store = compute_store_from_selection(data.get("store"))
        file_label = (data.get("file_label") or "").strip()
        if not file_label:
            return _error("Missing file_label")
        base_label = f"{store}_{file_label}"
        name_store = store

    try:
        batches = int(data.get("batches") or DEFAULT_BATCHES)
    except Exception:
        batches = DEFAULT_BATCHES
    if batches < 1:
        batches = DEFAULT_BATCHES

    zip_out = bool(data.get("zip_output"))

    uniques, dups = extract_asins_any(input_path)
    if not uniques:
        return _error("No valid ASINs found")

    if batches > len(uniques):
        return _error(
            "La cantidad de lotes no puede ser mayor que la cantidad de URLs. "
            f"URLs: {len(uniques)} | Lotes: {batches}"
        )

    uniques = reorder_asins(uniques, order)

    ddmmaa = datetime.now().strftime("%d%m%y")
    hhmm = datetime.now().strftime("%H%M")
    folder_name = f"{sanitize_filename(base_label)}_{ddmmaa}_{hhmm}"

    work_dir = Path(outdir) / folder_name
    ensure_folder(str(work_dir))
    batches_list = split_in_batches(uniques, batches)
    out_files = write_batches_as_txt(batches_list, str(work_dir), name_store, market, base_label)

    zip_path = ""
    if zip_out:
        zip_path = str(Path(outdir) / f"{sanitize_filename(base_label)}.zip")
        zip_outputs(out_files, zip_path)
        try:
            shutil.rmtree(work_dir)
        except Exception:
            pass

    resp = _preview_response(uniques, dups)
    resp.update({
        "output_folder": "" if zip_out else str(work_dir),
        "zip_path": zip_path,
    })
    return resp


def handle_request(data):
    action = (data.get("action") or "").strip().lower()
    if action == "preview":
        return handle_preview(data)
    if action == "process":
        return handle_process(data)
    if action == "export_duplicates":
        return handle_export_duplicates(data)
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
