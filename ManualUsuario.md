# Manual de Usuario - S3Tools

Herramienta de escritorio para preparar lotes de URLs, generar sitemaps y normalizar
archivos de WebScraper desde una sola interfaz.

## Requisitos
- Windows con .NET Framework 4.7.2.
- Para motores Python (`.py`): Python 3.12 recomendado.
- Dependencias Python:
  - Asin Batcher: `pandas`, `openpyxl`.
  - Sitemap: `openpyxl`.
  - Formato: `openpyxl`.

Si usas motores empaquetados (`.exe`) no necesitas Python instalado.

## Inicio rapido
1) Abre la app `S3Tools`.
2) Ve a la pestaña de la herramienta que necesitas.
3) Carga tus archivos, completa los campos y presiona **Procesar**.
4) La app abre automaticamente la carpeta resultante o el ZIP.

## Interfaz general
- La ventana principal muestra pestañas para cada herramienta.
- Cada pestaña incluye:
  - Zona de entrada de archivos.
  - Opciones de configuracion.
  - Carpeta destino.
  - Boton **Procesar**.
- El cursor cambia a espera mientras se procesa.

## Asin Batcher
Convierte ASINs en lotes de URLs listos para generar sitemaps.

### Entradas
- Archivos `.txt` o `.xlsx`.
- El contenido se depura para eliminar duplicados.

### Pasos
1) **Archivo de entrada**: selecciona un `.txt` o `.xlsx`.
2) **Previsualizacion**: revisa total, unicos y duplicados.
3) **Tienda**: elige una de las opciones disponibles.
4) **Nombre del archivo**: define el nombre base para la salida.
5) **Mercado**: MX o US.
6) **Lotes**: cantidad de lotes a generar.
7) **Orden**: Ordenado, Inverso o Aleatorio.
8) **Carpeta destino**: Descargas, Escritorio u otra.
9) (Opcional) **Exportar como ZIP**.
10) Presiona **Procesar**.

### Salidas
- Archivos `.txt` con encabezado `start_url`.
- Nombres: `Tienda_Nombre.txt` o `Tienda_Nombre_1.txt`.
- Solo caracteres permitidos: `a-zA-Z0-9_()+-`.
- Espacios y `-` se convierten a `_`.
- Si marcas ZIP, obtendras un archivo comprimido.

### Exportar duplicados
Si hay duplicados detectados, puedes exportarlos como CSV desde el boton
**Exportar duplicados**.

## Sitemap
Convierte lotes de URLs a sitemaps JSON con plantillas WebScraper.

### Entradas
- Archivos `.txt`, `.csv`, `.xlsx` o `.json`.
- Puedes importar multiples archivos.

### Pasos
1) **Importar archivos**: agrega los lotes de URLs.
2) **Modo**:
   - *Convertir todos*: toma todo lo de la lista.
   - *Seleccionar lotes*: procesa solo los seleccionados.
3) **Cargar ultimo lote**: trae los archivos generados por Asin Batcher.
4) **Tienda**: elige la tienda para aplicar la plantilla correcta.
5) **Nombre para sitemaps**: base del nombre de salida.
6) **Carpeta destino**: Descargas, Escritorio u otra.
7) (Opcional) **Exportar como ZIP**.
8) Presiona **Procesar**.

### Salidas
- Archivos `.json` en formato WebScraper.
- Nombres: `Tienda_Nombre.json` o `Tienda_Nombre_1.json`.
- Mismo saneado de caracteres que en Asin Batcher.

## Formato
Normaliza las primeras dos columnas de archivos WebScraper.

### Entradas
- Archivos `.csv` o `.xlsx`.

### Pasos
1) **Importar archivos**.
2) **Modo**:
   - *Procesar todos*: toma todo lo de la lista.
   - *Seleccionar archivos*: procesa solo los seleccionados.
3) **Plantilla**:
   - Auto: detecta segun contenido.
   - Tiendas: fuerza plantilla de tiendas.
   - BBvs: fuerza plantilla de BBvs.
4) Presiona **Procesar**.

### Salidas
- Se actualizan las primeras dos columnas en la misma carpeta de origen.
- Al finalizar se muestra el total de archivos actualizados.

## Pestañas sin logica activa
- **S3 Scraper** y **Control Remoto** aparecen en la UI pero no tienen logica de
  negocio implementada en esta version.

## Plantillas de sitemap
- `PlantillaSitemapsTiendas.json`: ProductosTX, Holaproducto, Altinor, Hervaz Trade.
- `PlantillaSitemapsBBvs.json`: BBvs_Template, BBvsBB2_2da, BBvsBB2.

## Persistencia de carpetas
- La carpeta de salida del Asin Batcher se guarda en:
  `%LocalAppData%\S3Integracion\last_asin_output_dir.txt`.
- Se usa para precargar archivos en la pestaña Sitemap.

## Solucion de problemas
- **No se encuentra el motor**: verifica rutas en `Engines/` o variables de entorno.
- **Errores con `.py`**: revisa Python y dependencias (`openpyxl`, `pandas`).
- **Archivos no cargan**: confirma extensiones y que no esten abiertos por otra app.
- **Salida vacia**: valida que el nombre base no este en blanco y que existan URLs.

## Soporte
Si el problema persiste, registra el mensaje de error y la entrada utilizada para
reproducirlo internamente.
