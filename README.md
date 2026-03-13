# S3Tools

Aplicación de escritorio en `WinForms` sobre `.NET 10` que integra tres herramientas operativas en una sola interfaz:

- `Asin Batcher`
- `Sitemap`
- `Formato`

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
- Plantillas sitemap: en raíz del proyecto:
  - `PlantillaSitemapsTiendas.json`
  - `PlantillaSitemapsBBvs.json`

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

## Estructura principal del código

- `Form1.cs`: contenedor principal con pestañas
- `AsinBatcherControl.cs`: UI de Asin Batcher
- `SitemapControl.cs`: UI de Sitemap
- `FormatoControl.cs`: UI de Formato
- `ControlRemotoControl.cs`: pestaña visible, sin lógica de negocio activa
- `AsinBatcherEngineClient.cs`: lógica de procesamiento Asin Batcher en C#
- `SitemapEngineClient.cs`: lógica de generación de sitemap en C#
- `FormatoEngineClient.cs`: lógica de normalización en C#
- `AppState.cs`: persistencia local de estado
- `FileNameConfigDialog.cs`: configuración de prefijos para nombres de salida

Recursos:
- `PlantillaSitemapsTiendas.json`
- `PlantillaSitemapsBBvs.json`

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
dotnet build
```

Ejecución en desarrollo:

```powershell
dotnet run --project .\S3Integración_programs.csproj
```

---

## Uso rápido

1. Abrir la aplicación.
2. Elegir una pestaña (`Asin Batcher`, `Sitemap`, `Formato`).
3. Cargar archivos de entrada.
4. Configurar parámetros de salida.
5. Pulsar **Procesar**.
6. Revisar carpeta/ZIP generado.

Para guía detallada de operación, revisar `ManualUsuario.md`.

---

## Persistencia local

La aplicación guarda la última carpeta de salida de Asin Batcher en:

- `%LocalAppData%\S3Integracion\last_asin_output_dir.txt`

Esto permite a `Sitemap` precargar automáticamente archivos recientes.

---

## Troubleshooting

- Si un archivo no se procesa, validar extensión y formato.
- Si no se detectan URLs en `Sitemap`, revisar contenido de entrada.
- Si falla lectura de Excel, confirmar que el archivo no esté abierto/bloqueado.
- Si el build falla, ejecutar nuevamente `dotnet restore` y luego `dotnet build`.

---

## Alcance actual

- `Asin Batcher`, `Sitemap` y `Formato`: funcionales en C#.
- `Control Remoto`: pestaña disponible en UI, sin lógica operativa incluida en esta fase.
