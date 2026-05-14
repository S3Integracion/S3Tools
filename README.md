# S3Tools

Aplicación de escritorio en `WinForms` sobre `.NET 10` que integra cinco herramientas operativas en una sola interfaz:

- `Asin Batcher`
- `Sitemap`
- `Formato`
- `Asin no Report`
- `Categorías`

El objetivo es centralizar el flujo de trabajo de preparación de datos para WebScraper en un único ejecutable, con lógica 100% en C#.

---

## Resumen del flujo funcional

Flujo principal del programa:

1. **ASINs** (`Asin Batcher`)  
2. **Lotes de URLs** (`Asin Batcher`)  
3. **Sitemaps JSON** (`Sitemap`)  
4. **Normalización de columnas** (`Formato`)

---

## Estado actual del proyecto

- Framework objetivo: `.NET 10 (net10.0-windows)`
- UI: `Windows Forms`
- Motores Python: **eliminados**
- Lógica de negocio: **C# nativo**
- Plantillas en raíz del proyecto:
  - `PlantillaSitemapsTiendas.json`
  - `PlantillaSitemapsBBvs.json`
  - `PlantillaCategoriasAmazon.json`

---

## Funcionalidades por módulo

## 1) Asin Batcher

Permite cargar ASINs, limpiarlos y generar lotes de URLs listos para scraping.

### Entradas
- `.txt`
- `.xlsx`

### Capacidades
- Limpieza y normalización de ASIN
- Eliminación de duplicados
- Ordenado de resultados (`Ordenado`, `Inverso`, `Aleatorio`)
- División en lotes configurables
- Selección de mercado (`US` / `MX`)
- Opción de incluir parámetro de vendedor (`aod=1`)
- Exportación en `.txt` y opción de `.zip`
- Exportación de duplicados a `.csv`

### Salidas
- Archivos `.txt` con encabezado `start_url`
- Nombres saneados para evitar caracteres inválidos

---

## 2) Sitemap

Convierte archivos de entrada con URLs en archivos sitemap JSON compatibles con WebScraper.

### Entradas
- `.txt`
- `.csv`
- `.xlsx`
- `.json`

### Capacidades
- Extracción de URLs por regex
- Selección de plantilla (`Normal` / `Nube`)
- Construcción de nombres por tienda y prefijos
- Generación de múltiples sitemaps
- Exportación directa o comprimida en `.zip`

### Salidas
- Archivos `.json` con estructura de sitemap WebScraper

---

## 3) Formato

Normaliza encabezados en archivos exportados por WebScraper.

### Entradas
- `.csv`
- `.xlsx`

### Capacidades
- Detección de plantilla (`Auto`, `Tiendas`, `BBvs`)
- Normalización de primeras dos columnas:
  - `web_scraper_order` / `web-scraper-order`
  - `web_scraper_start_url` / `web-scraper-start-url`
- Conservación del resto de columnas

### Salidas
- Modificación del archivo de entrada en la misma ubicación

---

## 4) Asin no Report

Compara un archivo base (`.csv`/`.xlsx`) contra uno o más reportes tabulados de Amazon (`.txt`) y devuelve los ASINs que están en la base pero no aparecen en ningún reporte.

### Entradas
- Base: `.csv` o `.xlsx` (con columna `asin`, `asins` o `asin1`)
- Reportes: múltiples `.txt` (tabulados con encabezado)

### Capacidades
- Selección de hoja para archivos base `.xlsx`
- Importación múltiple de reportes `.txt`
- Detección de ASIN en encabezados `asin`, `asins` y `asin1`
- Conserva orden original del archivo base
- Elimina duplicados del base en la salida final
- Excluye valores puramente numéricos (ASIN inválido)
- Resultado en texto para copiar/pegar y exportar a `.txt`

### Salidas
- Lista de ASINs faltantes en reportes (uno por línea)
- Resumen de conteos del análisis en la interfaz

---

## 5) Categorías

Genera URLs de categorías de Amazon México a partir de una URL de tienda. Extrae el identificador de tienda (`p_6`) y lo combina con plantillas predefinidas por categoría y un rango de páginas.

### Entradas
- URL absoluta de `amazon.com.mx` que contenga el filtro `p_6` (codificado o decodificado)
- Catálogo de categorías desde `PlantillaCategoriasAmazon.json`

### Capacidades
- Extracción robusta del identificador de tienda (`p_6`) en URLs codificadas o decodificadas
- Validación del dominio (`amazon.com.mx`) y del formato del identificador
- 18 categorías predefinidas (configurables sin recompilar)
- Selección por casillas con filtro por texto, **Seleccionar todas** y **Ninguna**
- Tabla de **Verificación** con una URL `page=2` por categoría seleccionada (clickeable)
- Generación de **una sola URL por categoría** con el placeholder de rango `[1-N]` reemplazando el valor de `page` y `ref=sr_pg_` (tope de N: `1000`)
- Conserva la estructura específica de cada plantilla; reemplaza únicamente `{store}` y `{page}` (incluido `ref=sr_pg_{page}`)
- Apertura directa de URLs en el navegador (doble clic o `Ctrl+clic`)
- Copiar todas / copiar selección al portapapeles
- Exportación a `.txt` (una URL por línea) y `.csv` (Categoría, Página, Tienda, URL)

### Salidas
- Tabla de URLs generadas en pantalla
- `.txt` o `.csv` exportado a la ruta elegida

### Configuración de categorías
El archivo `PlantillaCategoriasAmazon.json` (raíz del proyecto, copiado al output en cada build) contiene un arreglo de objetos con:

- `Nombre`: nombre visible de la categoría
- `Departamento`: código `i=` de Amazon (ej. `electronics`, `kitchen`, `pets`)
- `Nodo`: identificador `n:` interno de la categoría
- `Plantilla`: URL plantilla con marcadores `{store}` y `{page}`
- `Activo`: `true`/`false`
- `Orden`: entero para orden de visualización

Las plantillas que no contengan ambos marcadores (`{store}` y `{page}`) se omiten al cargar y se reportan como advertencia. Para añadir o ajustar categorías solo edita el JSON y presiona **Recargar categorías** en la pestaña.

---

## Estructura del proyecto

```
S3Tools/
├── .editorconfig              # Reglas de estilo C# y naming rules
├── .gitignore                 # Patrones .NET estándar (excluye obj/, bin/, .vs/, *.user, *.pfx…)
├── Directory.Build.props      # Propiedades MSBuild compartidas
├── S3Tools.slnx               # Solución
├── README.md                  # Este archivo
├── LICENSE.TXT
├── docs/
│   ├── ManualUsuario.md       # Guía de operación detallada
│   └── S3Tools.pdf
└── src/S3Tools/
    ├── S3Tools.csproj         # Proyecto principal (.NET 10, WinForms)
    ├── Program.cs             # Punto de entrada
    ├── Forms/
    │   ├── MainForm.*         # Ventana principal con pestañas
    │   └── FileNameConfigDialog.*
    ├── Controls/              # UserControls por módulo
    │   ├── AsinBatcher/
    │   ├── Sitemap/
    │   ├── Formato/
    │   ├── AsinNoReport/
    │   └── Categorias/
    ├── Services/              # Lógica de negocio por módulo
    │   ├── AsinBatcher/       # AsinBatcherEngineClient + Extractor + Splitter + UrlBuilder + OutputWriter
    │   ├── Sitemap/           # SitemapEngineClient + UrlExtractor + SitemapJsonBuilder
    │   ├── Formato/           # FormatoEngineClient + XlsxHeaderNormalizer + CsvHeaderNormalizer + TemplateResolver
    │   ├── AsinNoReport/
    │   └── Categorias/
    ├── Models/                # DTOs por módulo
    │   ├── AsinBatcher/       # EngineRequest, EngineResponse, ExtractionResult
    │   ├── Sitemap/
    │   └── Formato/
    ├── Common/                # Utilidades compartidas
    │   ├── AppState.cs        # Persistencia de UI (última carpeta)
    │   ├── RegexPatterns.cs   # Patrones Asin / Url centralizados
    │   ├── StoreCatalog.cs    # Tiendas y mercados soportados
    │   └── Logging/
    │       └── FileLogger.cs  # Logger best-effort en %LocalAppData%\S3Tools\logs
    ├── Resources/             # Plantillas JSON e ícono (copiados al output)
    │   ├── PlantillaSitemapsTiendas.json
    │   ├── PlantillaSitemapsBBvs.json
    │   ├── PlantillaCategoriasAmazon.json
    │   └── S3Tools.ico
    └── Properties/
        └── Resources.resx
```

---

## Requisitos de ejecución

- Windows
- `.NET 10 SDK` (para compilar)

No se requiere Python ni PyInstaller.

---

## Compilación y ejecución

Desde la raíz del repositorio:

```powershell
dotnet restore
dotnet build .\S3Tools.slnx
```

Ejecución en desarrollo:

```powershell
dotnet run --project .\src\S3Tools\S3Tools.csproj
```

Compilación de Release:

```powershell
dotnet build .\S3Tools.slnx -c Release
```

---

## Uso rápido

1. Abrir la aplicación.
2. Elegir una pestaña (`Asin Batcher`, `Sitemap`, `Formato`, `Asin no Report`, `Categorías`).
3. Cargar archivos de entrada o pegar la URL (en `Categorías`).
4. Configurar parámetros de salida.
5. Pulsar **Procesar** o **Generar**.
6. Revisar carpeta/ZIP/archivo generado.

Para guía detallada de operación, revisar `docs/ManualUsuario.md`.

---

## Persistencia local

La aplicación guarda la última carpeta de salida de Asin Batcher en:

- `%LocalAppData%\S3Tools\last_asin_output_dir.txt`

Esto permite a `Sitemap` precargar automáticamente archivos recientes.

Los logs de la aplicación (inicio, errores no fatales, advertencias) se escriben en:

- `%LocalAppData%\S3Tools\logs\app-YYYYMMDD.log`

---

## Troubleshooting

- Si un archivo no se procesa, validar extensión y formato.
- Si no se detectan URLs en `Sitemap`, revisar contenido de entrada.
- Si falla lectura de Excel, confirmar que el archivo no esté abierto/bloqueado.
- Si el build falla, ejecutar nuevamente `dotnet restore` y luego `dotnet build`.

---

## Alcance actual

- `Asin Batcher`, `Sitemap`, `Formato`, `Asin no Report` y `Categorías`: funcionales en C#.
