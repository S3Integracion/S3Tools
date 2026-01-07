# S3Tools
Herramientas integradas en una sola app de escritorio (WinForms + motores Python) para preparar lotes,
generar sitemaps y normalizar salidas de WebScraper.

## Objetivo
Unificar multiples herramientas internas en un solo ejecutable Windows, manteniendo la logica Python como
motor interno y usando C# para la interfaz. El flujo principal es: ASIN -> lotes de URLs -> sitemaps
JSON -> normalizacion de columnas.

## Funcionalidades
- Asin Batcher: carga ASINs desde TXT/XLSX, elimina duplicados, genera lotes de URLs por tienda/mercado.
- Sitemap: convierte lotes (TXT/CSV/XLSX/JSON) a sitemaps JSON con plantillas WebScraper.
- Formato: normaliza las primeras dos columnas de archivos CSV/XLSX exportados por WebScraper.
- Integracion de motores Python con opcion de empaquetarlos en `.exe`.
- Persistencia de la ultima carpeta de salida para mejorar el flujo entre herramientas.

## Arquitectura general
- UI principal: WinForms (.NET Framework 4.7.2).
- Motores: scripts Python ejecutados como subproceso (stdin/stdout JSON).
- Opcional: empaquetar cada motor con PyInstaller para distribuir sin Python.

El cliente C# resuelve el motor en este orden:
1) Variable de entorno (ruta manual).
2) Motor embebido como recurso (si se agrega en el ensamblado).
3) Ejecutable local en `Engines/<Motor>/<script>.exe` (mismo nombre que el `.py`).
4) Script local en `Engines/<Motor>/<script>.py` usando `python`.

## Estructura principal
- `Form1.cs`: shell con pestañas para cada herramienta.
- `AsinBatcherControl.cs`: UI de Asin Batcher.
- `SitemapControl.cs`: UI de Sitemap.
- `FormatoControl.cs`: UI de Formato.
- `AsinBatcherEngineClient.cs`: cliente del motor Asin Batcher.
- `SitemapEngineClient.cs`: cliente del motor Sitemap.
- `FormatoEngineClient.cs`: cliente del motor Formato.
- `AppState.cs`: estado local (ultima carpeta de salida del Asin Batcher).
- `Engines/AsinBatcherEngine/engine.py`: motor de ASIN -> URLs.
- `Engines/Sitemap/form_site.py`: motor de URLs -> sitemaps JSON.
- `Engines/Formato/format.py`: motor de normalizacion de las primeras dos columnas.
- `Engines/Sitemap/PlantillaSitemaps*.json`: plantillas para los sitemaps.

## Requisitos
### Si se usa el motor Python (.py)
- Python 3.12 recomendado.
- Asin Batcher: `pandas` + `openpyxl` para leer Excel.
- Sitemap: `openpyxl` para leer `.xlsx`.
- Formato: `openpyxl` para editar `.xlsx`.

### Si se usa el motor empaquetado (.exe)
- No requiere Python instalado.
- Asegurar que los `.exe` estan en `Engines/<Motor>/`.

## Uso rapido (usuario final)
1) Abre la app y elige la pestaña de la herramienta (Asin Batcher / Sitemap / Formato).
2) Carga archivos de entrada y completa los campos solicitados.
3) Define la carpeta destino y presiona **Procesar**.
4) La app abre automaticamente la carpeta resultante o el ZIP.

Para un paso a paso detallado, revisa `ManualUsuario.md`.

## Compilar motores (.exe)
1) Instala dependencias Python (segun el motor) y PyInstaller: `python -m pip install pyinstaller`.
2) Ejecuta desde la raiz: `powershell -ExecutionPolicy Bypass -File .\\build_engines.ps1`.

Salida (mismo nombre que el `.py`):
- `Engines/AsinBatcherEngine/engine.exe`
- `Engines/Formato/format.exe`
- `Engines/Sitemap/form_site.exe`

## Variables de entorno (opcional)
- `ASIN_BATCHER_ENGINE_PATH`: ruta del motor Asin Batcher (`.exe` o `.py`).
- `SITEMAP_ENGINE_PATH`: ruta del motor Sitemap (`.exe` o `.py`).
- `FORMATO_ENGINE_PATH`: ruta del motor Formato (`.exe` o `.py`).

## Detalles de uso por herramienta
### Asin Batcher
1) Selecciona archivo de entrada (`.txt` o `.xlsx`).
2) Define tienda, mercado, orden y cantidad de lotes.
3) Define nombre de salida.
4) Procesa y genera lotes de URLs.

Salida:
- Archivos `.txt` con encabezado `start_url`.
- Nombres: `Prefijo1Prefijo2NombreTienda.txt` o `Prefijo1Prefijo2NombreTienda_1.txt`, etc.
- Solo caracteres permitidos: `a-zA-Z0-9_()+-`.
- Espacios y `-` se convierten a `_`.
- Opcional: exportar como ZIP y/o generar CSV de duplicados.

### Sitemap
1) Importa multiples archivos (`.txt`, `.csv`, `.xlsx`, `.json`).
2) Selecciona tienda y nombre base.
3) Genera sitemaps con la plantilla adecuada.

Salida:
- Archivos `.json` en formato WebScraper.
- Nombres: `Prefijo1Prefijo2NombreTienda.json` o `Prefijo1Prefijo2NombreTienda_1.json`, etc.
- Mismo saneado de caracteres que en Asin Batcher.
- Opcional: exportar como ZIP.

### Formato
1) Importa archivos `.csv` o `.xlsx` generados por WebScraper.
2) Elige plantilla (Auto/Tiendas/BBvs).
3) Procesa y actualiza solo las primeras dos columnas en la misma carpeta.

### Control Remoto
Tab disponible en la UI pero sin implementacion de logica de negocio en el codigo actual.

## Plantillas de sitemap
- `PlantillaSitemapsTiendas.json`: usada para ProductosTX, Holaproducto, Altinor, Hervaz Trade.
- `PlantillaSitemapsBBvs.json`: usada para BBvs_Template, BBvsBB2_2da, BBvsBB2.

## Persistencia y rutas
- La carpeta de salida del Asin Batcher se guarda en `%LocalAppData%\S3Integracion\last_asin_output_dir.txt`
  para precargar archivos en la pestana Sitemap.
- Carpeta por defecto: Descargas del usuario.

## Troubleshooting rapido
- Si el motor no responde, verifica la ruta del `.exe` o `.py` y las variables de entorno.
- Si usas `.py`, confirma dependencias (`openpyxl`, `pandas`) y version de Python.
- Si un archivo no carga, revisa la extension y que no este bloqueado por otra app.
