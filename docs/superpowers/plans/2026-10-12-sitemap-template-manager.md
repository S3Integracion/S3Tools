# Administrador de Plantillas de Sitemap — Plan de Implementación

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Que el usuario pueda crear, editar, duplicar, importar/exportar y eliminar plantillas de sitemap WebScraper desde la app (pestaña "Plantillas" y diálogo modal), con persistencia entre sesiones, y elegir la plantilla exacta en Sitemap y Formato.

**Architecture:** Un repositorio (`SitemapTemplateRepository`) persiste todas las plantillas en un único archivo JSON en `%LocalAppData%\S3Tools\` con escritura atómica y recuperación ante corrupción; se siembra desde las dos plantillas actuales, ahora embebidas en el ensamblado. Un validador puro protege las invariantes del contenido. Un único `UserControl` (`TemplateManagerControl`) se hospeda en la nueva pestaña y en el diálogo; el evento `Changed` del repositorio mantiene sincronizados los combos de Sitemap y Formato.

**Tech Stack:** .NET 10 (`net10.0-windows`), WinForms, `System.Text.Json` / `JsonNode` (BCL), xUnit para pruebas.

**Spec:** Solicitud del usuario (2026-10-12) + decisiones confirmadas:
1. Panel en **ambos** lugares: pestaña "Plantillas" en MainForm **y** diálogo modal "Administrar plantillas…" desde Sitemap y Formato.
2. Plantillas persistentes entre cierres/aperturas del programa.
3. Plantillas de fábrica (Tiendas / BBvs) editables y eliminables + botón "Restaurar predeterminadas".
4. Edición vía editor JSON con validación en vivo, importar/exportar `.json`, vista previa de selectores de solo lectura.
5. Asociación opcional plantilla ↔ tiendas: al elegir tienda en Sitemap se preselecciona su plantilla (el usuario puede cambiarla).

## Context

Hoy las plantillas viven en `src/S3Tools/Resources/PlantillaSitemaps{Tiendas,BBvs}.json`, se copian junto al .exe y se eligen con radios fijos:
- Sitemap: radios Normal/Nube → `SitemapJsonBuilder.SelectTemplate` (`Services/Sitemap/SitemapJsonBuilder.cs`).
- Formato: radios Auto/Tiendas/BBvs → `TemplateResolver.TemplateFiles` (`Services/Formato/TemplateResolver.cs`), que además **autodetecta** la plantilla comparando encabezados CSV/XLSX con los `id` de selectores. Por eso el cambio afecta a Formato, no solo a Sitemap.

Cualquier cambio de selectores exige recompilar. Además: la publicación es single-file self-contained (`Properties/PublishProfiles/FolderProfile.pubxml`), así que la carpeta del exe no es un lugar fiable para escribir; y `BuildSitemapPayload` hace sustituciones por regex que fallan en silencio si la plantilla no tiene `_id`/`startUrl` y no escapa caracteres de control — inaceptable con contenido escrito por el usuario.

## Global Constraints

- `net10.0-windows`, WinForms, `LangVersion latest`, `Nullable disable`, **`ImplicitUsings disable`** (usings explícitos en cada archivo).
- Sin dependencias NuGet nuevas en `S3Tools.csproj` (solo en el proyecto de pruebas). JSON con `System.Text.Json` del BCL.
- Nunca escribir junto al ejecutable. Persistencia en `%LocalAppData%\S3Tools\sitemap-templates.json` (misma raíz que `Common/AppState.cs`).
- Archivos escritos en UTF-8 **sin BOM** (`new UTF8Encoding(false)`), como el código existente.
- Identificadores de código en inglés; textos de UI, mensajes y comentarios XML en español (patrón actual).
- Advertencias/errores de I/O se registran con `FileLogger.Warn/Error` (`Common/Logging/FileLogger.cs`); nada de `catch {}` silencioso en código nuevo.
- IDs fijos de fábrica: `"tiendas"` y `"bbvs"` (mantienen compatibilidad con las claves que ya usa Formato).
- Nombres de fábrica: `"Tiendas (Normal)"` y `"BBvs (Nube)"` (conservan el vocabulario de los radios actuales).
- Tiendas: única fuente `Common/StoreCatalog.cs` (`StoresLeft` + `StoresRight`).
- Versión: `2.5.0.0` → `2.6.0.0`.

## Modelo de datos e invariantes

Archivo `%LocalAppData%\S3Tools\sitemap-templates.json`:

```json
{
  "schemaVersion": 1,
  "templates": [
    {
      "id": "tiendas",
      "name": "Tiendas (Normal)",
      "description": "",
      "builtInKey": "tiendas",
      "stores": ["ProductosTX", "Holaproducto", "Altinor", "HervazTrade"],
      "createdUtc": "2026-10-12T00:00:00Z",
      "updatedUtc": "2026-10-12T00:00:00Z",
      "sitemap": { "_id": "Tittle", "startUrl": [], "selectors": [ ... ] }
    }
  ]
}
```

Un solo archivo (no índice + archivos sueltos): una escritura atómica cubre todo, sin consistencia entre archivos que mantener. Tamaño esperado: ~5 KB por plantilla.

| Invariante | Dónde se hace cumplir |
|---|---|
| `id` inmutable, `^[a-z0-9]{1,64}$`; nuevas = `Guid.NewGuid().ToString("N")` | Repositorio (Create) / Load descarta entradas inválidas |
| `name` recortado, 1–60 caracteres, único sin distinguir mayúsculas | Repositorio (Create/Update); Load renombra duplicados con sufijo ` (2)` |
| `description` ≤ 200 caracteres | Repositorio |
| `stores` ⊆ StoreCatalog; cada tienda vinculada a **como máximo una** plantilla | Repositorio: al guardar, la tienda se mueve y se devuelve una advertencia |
| `builtInKey` ∈ {`null`, `"tiendas"`, `"bbvs"`} | Repositorio |
| `sitemap` pasa `SitemapTemplateValidator` sin errores | Repositorio (Create/Update rechaza) |
| Siempre existe ≥ 1 plantilla | Repositorio (Delete rechaza la última); Load resiembra si queda vacío |
| `schemaVersion` desconocido mayor que 1 | Load: no sobrescribe; abre solo lectura en memoria con `LoadWarning` |

Escritura atómica: serializar a `sitemap-templates.json.tmp` → `File.Replace(tmp, main, main + ".bak")` (o `File.Move` si no existe `main`).
Carga: `main` → si falla, `.bak` → si falla, renombrar `main` a `sitemap-templates.corrupt-yyyyMMddHHmmss.json`, resembrar y fijar `LoadWarning` (la UI lo muestra una vez).

## Review Focus

1. **Plantilla seleccionada en Sitemap/Formato es eliminada o renombrada desde el panel** → el combo se refresca, conserva la selección si el id existe, si no cae en la vinculada a la tienda o la primera, y lo indica; el motor con id inexistente devuelve "La plantilla seleccionada ya no existe." (pruebas en Tasks 5 y 6).
2. **Archivo de persistencia corrupto o escritura interrumpida** (cierre forzado, edición manual) → la app arranca, recupera `.bak` o resiembra, nunca lanza en el arranque (pruebas en Task 3).
3. **JSON pegado/importado con BOM, en una sola línea, con comentarios o comas finales** → se acepta (lectura tolerante: `JsonCommentHandling.Skip`, `AllowTrailingCommas = true`) y se normaliza al guardar; errores reales muestran línea y columna (pruebas en Task 2).
4. **Cambios sin guardar** al cambiar de plantilla en la lista, salir de la pestaña, cerrar el diálogo o cerrar la app → prompt Guardar / No guardar / Cancelar (checklist manual Task 7, lógica `IsDirty` probada en Task 6).
5. **URLs con comillas, `\`, `&`, Unicode o caracteres de control** en `startUrl` → salida JSON válida y semánticamente igual a la actual en los casos que hoy funcionan (prueba de paridad en Task 1/2).

---

## Estructura de archivos

Crear:
- `tests/S3Tools.Tests/S3Tools.Tests.csproj` — xUnit, referencia al proyecto principal.
- `src/S3Tools/Models/Templates/SitemapTemplate.cs` — snapshot inmutable de una plantilla + `SitemapTemplateDraft` (entrada de Create/Update).
- `src/S3Tools/Models/Templates/TemplateIssue.cs` — `TemplateIssue`, `TemplateIssueSeverity`, `TemplateValidationResult`, `SelectorSummary`, `TemplateSaveResult`.
- `src/S3Tools/Services/Templates/SitemapTemplateValidator.cs` — validación pura del JSON.
- `src/S3Tools/Services/Templates/BuiltInSitemapTemplates.cs` — lee los recursos embebidos.
- `src/S3Tools/Services/Templates/SitemapTemplateStore.cs` — lectura/escritura atómica y recuperación del archivo (sin reglas de negocio).
- `src/S3Tools/Services/Templates/SitemapTemplateRepository.cs` — invariantes, CRUD, tiendas, restauración, evento `Changed`.
- `src/S3Tools/Controls/Templates/TemplateManagerControl.cs` — panel (layout por código, como `Forms/FileNameConfigDialog.cs`).
- `src/S3Tools/Forms/TemplateManagerDialog.cs` — hospeda el control en modal.

Modificar:
- `src/S3Tools/S3Tools.csproj` — plantillas de sitemap pasan de `Content` a `EmbeddedResource`; `InternalsVisibleTo`; versión.
- `S3Tools.slnx` — agregar proyecto de pruebas.
- `src/S3Tools/Services/Sitemap/SitemapJsonBuilder.cs` — `JsonNode` en lugar de regex; eliminar `SelectTemplate`/`LoadTemplateText`.
- `src/S3Tools/Services/Sitemap/SitemapEngineClient.cs`, `Models/Sitemap/SitemapEngineRequest.cs` — `TemplateId` reemplaza `TemplateMode`.
- `src/S3Tools/Services/Formato/TemplateResolver.cs`, `CsvHeaderNormalizer.cs`, `XlsxHeaderNormalizer.cs`, `FormatoEngineClient.cs` — plantillas desde el repositorio.
- `src/S3Tools/Common/AppState.cs` — recordar última plantilla en Sitemap y Formato.
- `src/S3Tools/Controls/Sitemap/SitemapControl{.cs,.Designer.cs}`, `Controls/Formato/FormatoControl{.cs,.Designer.cs}` — combos + botón "Administrar plantillas…".
- `src/S3Tools/Forms/MainForm{.cs,.Designer.cs}` — pestaña "Plantillas" + guardas de cambios sin guardar.
- `README.md`, `docs/ManualUsuario.md`.

Fuera de alcance: `PlantillaCategoriasAmazon.json` (módulo Categorías, formato distinto) y los arreglos duplicados de tiendas en `AsinBatcherControl`.

---

### Task 1: Proyecto de pruebas + pruebas de caracterización

Red de seguridad antes de refactorizar: fija el comportamiento actual del builder y de la autodetección de Formato.

**Files:**
- Create: `tests/S3Tools.Tests/S3Tools.Tests.csproj`, `tests/S3Tools.Tests/SitemapJsonBuilderCharacterizationTests.cs`, `tests/S3Tools.Tests/TemplateResolverCharacterizationTests.cs`
- Modify: `S3Tools.slnx`, `src/S3Tools/S3Tools.csproj`
- Copy: este plan a `docs/superpowers/plans/2026-10-12-sitemap-template-manager.md`

**Interfaces:**
- Produces: proyecto `S3Tools.Tests` que ve los tipos `internal` de `S3Tools`.

- [ ] **Step 1:** Crear `S3Tools.Tests.csproj`: `TargetFramework net10.0-windows`, `UseWindowsForms true`, `IsPackable false`, `ProjectReference ..\..\src\S3Tools\S3Tools.csproj`; paquetes `Microsoft.NET.Test.Sdk`, `xunit`, `xunit.runner.visualstudio` (últimas estables). Agregar a `S3Tools.slnx` bajo carpeta `/tests/`. En `S3Tools.csproj`: `<InternalsVisibleTo Include="S3Tools.Tests" />`.
- [ ] **Step 2:** Escribir pruebas de caracterización (deben **pasar** contra el código actual):
  - `BuildSitemapPayload_ReplacesIdAndStartUrl_Tiendas`: carga `PlantillaSitemapsTiendas.json` (vía `SitemapJsonBuilder.LoadTemplateText`), construye con `title="ProductosTX_1"`, `urls=["https://www.amazon.com.mx/dp/B0001ZZ5SG?th=1&aod=1","https://www.amazon.com/dp/0805470859"]`; parsea el resultado con `JsonNode.Parse` y afirma `_id == "ProductosTX_1"`, `startUrl` igual a las URLs, y que `selectors` es `JsonNode.DeepEquals` al de la plantilla original.
  - Mismo caso para BBvs.
  - Generar una sola vez (con el código actual) la salida de ambos casos y commitearla como `tests/S3Tools.Tests/Fixtures/expected-tiendas.json` y `expected-bbvs.json` (`CopyToOutputDirectory=PreserveNewest`); son la referencia de paridad de Task 2, cuando el builder viejo ya no exista.
  - `DetectTemplate_HeadersFromBbvs_ReturnsBbvs` (encabezados `web_scraper_order, web_scraper_start_url, precio_bb, vendedor, shipping`) → `"bbvs"`; `DetectTemplate_HeadersFromTiendas_ReturnsTiendas` (`costo_detailpage, shipsfrom_detailpage`) → `"tiendas"`; `DetectTemplate_NoMatch_ReturnsTiendas` (`foo, bar`) → `"tiendas"`.
- [ ] **Step 3:** Run `dotnet test S3Tools.slnx` → Expected: todas PASS.
- [ ] **Step 4:** Commit `test: add test project and characterization tests for sitemap builder and template detection` (incluye la copia del plan).

### Task 2: Validador de plantillas + builder con JsonNode

**Files:**
- Create: `src/S3Tools/Models/Templates/TemplateIssue.cs`, `src/S3Tools/Services/Templates/SitemapTemplateValidator.cs`, `tests/S3Tools.Tests/SitemapTemplateValidatorTests.cs`
- Modify: `src/S3Tools/Services/Sitemap/SitemapJsonBuilder.cs`, `tests/S3Tools.Tests/SitemapJsonBuilderCharacterizationTests.cs` (renombrar a `SitemapJsonBuilderTests.cs`)

**Interfaces:**
- Produces:
  - `enum TemplateIssueSeverity { Error, Warning }`
  - `sealed class TemplateIssue { TemplateIssueSeverity Severity; string Message; long? Line; long? Column; }` (Line/Column 1-based)
  - `sealed class SelectorSummary { string Id; string Type; string Selector; bool Multiple; IReadOnlyList<string> ParentSelectors; }`
  - `sealed class TemplateValidationResult { bool IsValid /* sin Errors */; IReadOnlyList<TemplateIssue> Issues; IReadOnlyList<SelectorSummary> Selectors; JsonObject Sitemap /* null si no parsea */; }`
  - `static TemplateValidationResult SitemapTemplateValidator.Validate(string json)`
  - `static string SitemapTemplateValidator.Normalize(JsonObject sitemap)` → JSON indentado (para "Formatear" y para exportar).
  - `static string SitemapJsonBuilder.BuildSitemapPayload(string templateJson, string title, IList<string> urls)` (misma firma que hoy).
  - Constante compartida `SitemapTemplateValidator.MaxTemplateChars = 1_000_000`.

Reglas del validador (Errores bloquean guardar; Advertencias no):
- Error: vacío; > `MaxTemplateChars`; JSON inválido (mensaje `"JSON inválido: {detalle}"` con `JsonException.LineNumber+1` y `BytePositionInLine+1`); raíz no es objeto; `selectors` ausente, no arreglo o vacío; selector que no es objeto; `id` de selector vacío o no string; `type` vacío; `parentSelectors` no arreglo; `id` de selector duplicado (sensible a mayúsculas, como WebScraper); `_id` presente y no string; `startUrl` presente y no string/arreglo de strings.
- Advertencia: falta `_id` o `startUrl` (el builder los agrega); `parentSelectors` referencia un id que no es `_root` ni un selector existente; ningún selector distinto de `*ElementClick` (Formato no podrá autodetectarla).
- Lectura tolerante: `JsonDocumentOptions { CommentHandling = Skip, AllowTrailingCommas = true }`; quitar BOM (`﻿`) inicial antes de parsear.

- [ ] **Step 1: Pruebas que fallan** (`SitemapTemplateValidatorTests`): ambas plantillas de fábrica → `IsValid`, `Selectors.Count` igual al conteo real del archivo; `"{"` → Error con `Line == 1`; `"[]"` → Error raíz; `{"selectors":[]}` → Error; selectores con id duplicado → Error que menciona el id; texto con BOM + comentario `// x` + coma final → `IsValid`; sin `_id`/`startUrl` → `IsValid` con 2 Warnings; `parentSelectors:["noexiste"]` → Warning; texto de `MaxTemplateChars + 1` → Error.
- [ ] **Step 2:** Run `dotnet test --filter SitemapTemplateValidatorTests` → FAIL (tipo no existe).
- [ ] **Step 3:** Implementar `TemplateIssue.cs` y `SitemapTemplateValidator`.
- [ ] **Step 4: Pruebas del builder nuevas** en `SitemapJsonBuilderTests`: paridad → para cada fixture de Task 1, `JsonNode.DeepEquals(Parse(nuevo), Parse(fixture))`; plantilla sin `_id`/`startUrl` → resultado los contiene; `startUrl` como string en la plantilla → reemplazado por arreglo; URL con `"`, `\`, `\t` y `ñ` → resultado parsea y la URL se recupera idéntica; `&` aparece literal (no `&`).
- [ ] **Step 5:** Reescribir `BuildSitemapPayload` con `JsonNode.Parse` (opciones tolerantes) → asignar `["_id"]` y `["startUrl"] = new JsonArray(urls...)` → `ToJsonString(new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, WriteIndented = false })`. `JsonObject` conserva el orden de propiedades; si `_id`/`startUrl` no existían, insertarlos al inicio (`JsonObject.Insert(0, …)`) para mantener la forma WebScraper. Actualizar el comentario XML (ya no es sustitución textual). Eliminar `IdFieldRegex`, `StartUrlFieldRegex`, `EscapeJson` (las pruebas de caracterización que llamaban `LoadTemplateText` pasan a leer el fixture/archivo directamente; `SelectTemplate`/`LoadTemplateText` se eliminan en Task 5).
- [ ] **Step 6:** Run `dotnet test` → todo PASS.
- [ ] **Step 7:** Commit `feat: add sitemap template validator and JsonNode-based payload builder`.

### Task 3: Plantillas embebidas + almacenamiento atómico con recuperación

**Files:**
- Create: `src/S3Tools/Models/Templates/SitemapTemplate.cs`, `src/S3Tools/Services/Templates/BuiltInSitemapTemplates.cs`, `src/S3Tools/Services/Templates/SitemapTemplateStore.cs`, `tests/S3Tools.Tests/SitemapTemplateStoreTests.cs`
- Modify: `src/S3Tools/S3Tools.csproj`

**Interfaces:**
- Consumes: `SitemapTemplateValidator.Validate` (Task 2).
- Produces:
  - `sealed class SitemapTemplate` (inmutable, propiedades get-only, constructor completo): `string Id, Name, Description, BuiltInKey; IReadOnlyList<string> Stores; DateTime CreatedUtc, UpdatedUtc; string SitemapJson; bool IsBuiltIn => BuiltInKey != null;`
  - `sealed class SitemapTemplateDraft { string Name; string Description; IReadOnlyList<string> Stores; string SitemapJson; }`
  - `static IReadOnlyList<SitemapTemplate> BuiltInSitemapTemplates.CreateDefaults(DateTime utcNow)` — ids/nombres de Global Constraints; `stores` = `StoreCatalog.StoresLeft` para tiendas, `StoreCatalog.StoresRight` para bbvs.
  - `static string BuiltInSitemapTemplates.GetSitemapJson(string builtInKey)` — lee `Assembly.GetManifestResourceStream("S3Tools.Templates.{key}.json")`.
  - `sealed class SitemapTemplateStore(string filePath)`: `StoreLoadResult Load()` y `void Save(IReadOnlyList<SitemapTemplate> templates)`.
  - `sealed class StoreLoadResult { IReadOnlyList<SitemapTemplate> Templates; string Warning /* null si limpio */; bool IsReadOnly /* schemaVersion futuro */; }`

- [ ] **Step 1:** En `S3Tools.csproj`, cambiar las dos plantillas de sitemap a `<EmbeddedResource Include="Resources\PlantillaSitemapsTiendas.json" LogicalName="S3Tools.Templates.tiendas.json" />` (ídem `bbvs`). `PlantillaCategoriasAmazon.json` sigue como `Content`.
- [ ] **Step 2: Pruebas que fallan** (`SitemapTemplateStoreTests`, cada prueba en un directorio temporal propio, limpiado en `Dispose`):
  - `Load_NoFile_SeedsDefaultsAndPersists` → 2 plantillas `tiendas`/`bbvs`, archivo creado, `Warning == null`.
  - `SaveThenLoad_RoundTripsAllFields` (incluye tiendas, fechas UTC y contenido semánticamente igual).
  - `Load_CorruptMain_ValidBak_RecoversFromBak` → `Warning` no nulo.
  - `Load_CorruptMainAndBak_RenamesCorruptAndReseeds` → existe `sitemap-templates.corrupt-*.json`, 2 plantillas de fábrica.
  - `Load_EntryWithInvalidSitemap_IsDroppedWithWarning`; `Load_DuplicateNames_SecondGetsSuffix` (`"X"`, `"X (2)"`); `Load_StoreLinkedTwice_KeepsFirst`; `Load_AllEntriesInvalid_Reseeds`.
  - `Load_FutureSchemaVersion_IsReadOnlyAndFileUntouched` (bytes del archivo iguales antes/después).
  - `Save_WritesUtf8WithoutBom_AndLeavesNoTmp`.
  - `BuiltIns_ResourcesExistAndValidate` → ambos recursos embebidos pasan el validador.
- [ ] **Step 3:** Run `dotnet test --filter SitemapTemplateStoreTests` → FAIL.
- [ ] **Step 4:** Implementar modelos, `BuiltInSitemapTemplates` y `SitemapTemplateStore` según "Modelo de datos e invariantes" (DTOs privados `[JsonPropertyName]` camelCase; `sitemap` persistido como objeto JSON, no como string; cada recuperación se registra con `FileLogger.Warn(nameof(SitemapTemplateStore), …)`).
- [ ] **Step 5:** Run `dotnet test` → PASS.
- [ ] **Step 6:** Commit `feat: embed default sitemap templates and add atomic template store`.

### Task 4: Repositorio — CRUD, tiendas, restauración, evento

**Files:**
- Create: `src/S3Tools/Services/Templates/SitemapTemplateRepository.cs`, `tests/S3Tools.Tests/SitemapTemplateRepositoryTests.cs`

**Interfaces:**
- Consumes: `SitemapTemplateStore`, `BuiltInSitemapTemplates`, `SitemapTemplateValidator`.
- Produces:
  - `sealed class TemplateSaveResult { bool Ok; SitemapTemplate Template; IReadOnlyList<TemplateIssue> Issues; }`
  - `sealed class SitemapTemplateRepository`:
    - `static SitemapTemplateRepository Default { get; }` (lazy; ruta `%LocalAppData%\S3Tools\sitemap-templates.json`)
    - `SitemapTemplateRepository(string filePath)`; carga perezosa en el primer acceso.
    - `event EventHandler Changed;` (se dispara tras cada mutación exitosa)
    - `string LoadWarning { get; }`, `bool IsReadOnly { get; }`
    - `IReadOnlyList<SitemapTemplate> GetAll()` (orden persistido), `SitemapTemplate Find(string id)` (null si no existe), `SitemapTemplate FindByStore(string store)` (case-insensitive)
    - `TemplateSaveResult Create(SitemapTemplateDraft draft)`, `TemplateSaveResult Update(string id, SitemapTemplateDraft draft)`, `TemplateSaveResult Delete(string id)`
    - `TemplateSaveResult Duplicate(string id)` → nombre `"{Name} (copia)"`, luego `"(copia 2)"`, …; sin tiendas (evita conflicto); `BuiltInKey = null`.
    - `TemplateSaveResult Import(string json, string suggestedName)` → nombre sugerido = nombre del archivo o `_id` del JSON; desambigua igual que Duplicate.
    - `void RestoreDefaults()`
  - Thread-safety: lecturas y mutaciones bajo un `lock`; `GetAll` devuelve copia (el motor lee desde `Task.Run`). Si `IsReadOnly`, toda mutación devuelve `Ok=false` con Error "El archivo de plantillas fue creado por una versión más reciente de S3Tools."

- [ ] **Step 1: Pruebas que fallan:**
  - `Create_Valid_PersistsAndRaisesChanged` (nueva instancia sobre el mismo archivo la ve).
  - `Create_DuplicateNameIgnoringCase_Fails`; `Create_NameEmptyOrOver60_Fails`; `Create_InvalidJson_FailsWithValidatorIssues`; `Create_UnknownStore_Fails`.
  - `Update_MovesStoreFromOtherTemplate_ReturnsWarning` → la otra plantilla ya no tiene la tienda; Warning menciona tienda y nombre anterior.
  - `Update_KeepsIdAndCreatedUtc_BumpsUpdatedUtc`; `Update_MissingId_Fails`.
  - `Delete_LastTemplate_Fails`; `Delete_BuiltIn_Succeeds`.
  - `RestoreDefaults_RecreatesDeletedBuiltIn_ResetsEditedBuiltIn_KeepsCustom`; `RestoreDefaults_DoesNotStealStoreLinkedToCustom`.
  - `Duplicate_GeneratesUniqueCopyNames`; `Import_UsesFileNameAndDisambiguates`.
  - `Mutation_WhenSaveThrows_StateUnchanged` (archivo destino bloqueado con `FileShare.None`) → `Ok=false`, `GetAll()` igual que antes, `Changed` no se dispara.
- [ ] **Step 2:** Run → FAIL.
- [ ] **Step 3:** Implementar. Patrón de mutación: copiar lista → aplicar → validar invariantes → `store.Save(copia)` → solo entonces reemplazar estado en memoria y disparar `Changed`. Excepciones de I/O → `TemplateSaveResult` con Error "No se pudo guardar: {mensaje}" + `FileLogger.Error`.
- [ ] **Step 4:** Run `dotnet test` → PASS.
- [ ] **Step 5:** Commit `feat: add sitemap template repository with store links and restore defaults`.

### Task 5: Motores Sitemap y Formato usan el repositorio

**Files:**
- Modify: `Models/Sitemap/SitemapEngineRequest.cs`, `Services/Sitemap/SitemapEngineClient.cs`, `Services/Sitemap/SitemapJsonBuilder.cs`, `Services/Formato/TemplateResolver.cs`, `Services/Formato/CsvHeaderNormalizer.cs`, `Services/Formato/XlsxHeaderNormalizer.cs`, `Services/Formato/FormatoEngineClient.cs`, `Common/AppState.cs`
- Test: `tests/S3Tools.Tests/SitemapEngineTests.cs`, `tests/S3Tools.Tests/TemplateResolverTests.cs` (reemplaza la de caracterización), `tests/S3Tools.Tests/AppStateTests.cs` opcional si se parametriza la ruta (no requerido)

**Interfaces:**
- Consumes: `SitemapTemplateRepository` (Task 4).
- Produces:
  - `SitemapEngineRequest`: eliminar `TemplateMode`; agregar `[DataMember(Name = "template_id")] public string TemplateId { get; set; }`.
  - `SitemapEngine.Handle(SitemapEngineRequest request, SitemapTemplateRepository repository)`; `SitemapEngineClient` pasa `SitemapTemplateRepository.Default`. La plantilla se resuelve **una vez** al inicio (snapshot); id vacío o inexistente → `Error("La plantilla seleccionada ya no existe. Elige otra en la sección Plantilla.")`.
  - `TemplateResolver.ResolveTemplate(string choice, IList<string> headers, IReadOnlyList<SitemapTemplate> templates)` → devuelve el **id**; `choice == "auto"` → `DetectTemplate`; id inexistente → `InvalidOperationException("La plantilla seleccionada ya no existe.")`.
  - `TemplateResolver.DetectTemplate(IList<string> headers, IReadOnlyList<SitemapTemplate> templates)`; fallback (sin coincidencias o empate en 0) = plantilla con `BuiltInKey == "tiendas"`, si no la primera. En empate > 0 gana la primera en orden de la lista.
  - Caché de encabezados esperados con clave `id + "|" + UpdatedUtc.Ticks` (se invalida sola al editar); se elimina `TemplateFiles`, `LoadTemplateDefinition`, `ResolveTemplatePath` y `TemplateDefinition.cs` si queda sin uso (deserializar con `JsonNode` desde `SitemapJson`).
  - `UpdateHeaders(string path, string templateChoice, string headerFormat, IReadOnlyList<SitemapTemplate> templates)` en ambos normalizadores.
  - `FormatoEngine.Handle(FormatoEngineRequest, SitemapTemplateRepository)`: toma `GetAll()` una vez; `TemplateCounts` con clave = **nombre** de la plantilla (para el mensaje de la UI).
  - `AppState`: `SetLastTemplateId(string context, string id)` / `TryGetLastTemplateId(string context, out string id)` con `context` ∈ {`"sitemap"`, `"formato"`}, archivo `last_template_{context}.txt`, mismo manejo de errores que `SetLastAsinOutputDir`.
  - Eliminar `SitemapJsonBuilder.SelectTemplate` y `LoadTemplateText`.

- [ ] **Step 1: Pruebas que fallan:**
  - `SitemapEngine_WithCustomTemplate_WritesItsSelectors` (repo temporal + plantilla custom + archivo `.txt` con 2 URLs → JSON de salida contiene los selectores custom y el `_id` esperado).
  - `SitemapEngine_DeletedTemplateId_ReturnsError` (mensaje exacto de arriba).
  - `DetectTemplate_*` de Task 1 portadas a la nueva firma con `BuiltInSitemapTemplates.CreateDefaults`.
  - `DetectTemplate_CustomTemplateWithUniqueSelectors_IsDetected`.
  - `DetectTemplate_AfterTemplateEdited_UsesNewSelectors` (mismo id, `UpdatedUtc` distinto).
  - `DetectTemplate_TiendasDeleted_FallsBackToFirst`.
  - `ResolveTemplate_UnknownId_Throws`.
- [ ] **Step 2:** Run → FAIL.
- [ ] **Step 3:** Implementar los cambios de Interfaces. Compilar: `dotnet build S3Tools.slnx` sin errores (los controles aún usan `TemplateMode`/radios: ajustar `SitemapControl.GetSelectedTemplateMode` y `FormatoControl.GetSelectedTemplate` temporalmente para devolver `"tiendas"`/`"bbvs"`/`"auto"` como ids, de modo que la app siga funcionando hasta Task 7).
- [ ] **Step 4:** Run `dotnet test` → PASS.
- [ ] **Step 5:** Commit `refactor: sitemap and formato engines read templates from repository`.

### Task 6: Panel `TemplateManagerControl` + `TemplateManagerDialog`

**Files:**
- Create: `src/S3Tools/Controls/Templates/TemplateManagerControl.cs`, `src/S3Tools/Controls/Templates/TemplateEditorState.cs`, `src/S3Tools/Forms/TemplateManagerDialog.cs`, `tests/S3Tools.Tests/TemplateEditorStateTests.cs`

**Interfaces:**
- Consumes: `SitemapTemplateRepository` (Task 4), `SitemapTemplateValidator` (Task 2), `StoreCatalog`.
- Produces:
  - `sealed class TemplateEditorState` (lógica sin UI, testeable): `void Load(SitemapTemplate t)`, `void LoadNew()`, propiedades editables `Name/Description/Stores/SitemapJson`, `bool IsDirty`, `bool IsNew`, `string EditingId`, `SitemapTemplateDraft ToDraft()`.
  - `sealed class TemplateManagerControl : UserControl`: ctor `(SitemapTemplateRepository repository)`; `bool HasUnsavedChanges`; `bool ConfirmLeave()` (prompt Sí/No/Cancelar; `true` = puede salir); `void SelectTemplate(string id)`.
  - `sealed class TemplateManagerDialog : Form`: ctor `(SitemapTemplateRepository repository, string initialTemplateId)`; `string SelectedTemplateId` al cerrar (para que el llamador seleccione la plantilla recién creada/editada). `FormClosing` → `ConfirmLeave()`.

Diseño UI (layout por código con `TableLayoutPanel`/`SplitContainer`, fuente del sistema como el resto de la app; editor en `Consolas 10`):
- **Izquierda (≈ 280 px):** `ListView` modo Details: Nombre · Selectores · Tiendas; las de fábrica con sufijo "· Predeterminada". Debajo: `Nueva`, `Duplicar`, `Importar…`, `Eliminar`, y enlace `Restaurar predeterminadas`.
- **Derecha:** Nombre (`TextBox`, MaxLength 60), Descripción (MaxLength 200), Tiendas vinculadas (`CheckedListBox` con todas las de `StoreCatalog`; las vinculadas a otra plantilla muestran "(en: {nombre})"). Debajo un `TabControl` interno: **JSON** (`TextBox` multilínea, `WordWrap=false`, `ScrollBars=Both`, `MaxLength = MaxTemplateChars`) y **Selectores** (`DataGridView` solo lectura: Id · Tipo · Selector · Múltiple · Padres).
- **Barra inferior:** línea de estado de validación (verde "✓ Válida · N selectores", ámbar "N advertencias", rojo "Línea L, col C: mensaje"; clic en un error mueve el cursor a esa línea), y botones `Formatear JSON`, `Exportar…`, `Descartar cambios`, `Guardar` (default).
- Validación en vivo con `System.Windows.Forms.Timer` de 300 ms tras cada tecla. `Guardar` deshabilitado si hay Errores o no hay cambios.
- Atajos: `Ctrl+S` guardar, `Ctrl+N` nueva, `Supr` en la lista elimina. `AccessibleName` en todos los campos; orden de tabulación izquierda→derecha, arriba→abajo.
- Estado vacío/primera vez: si `repository.LoadWarning != null`, mostrarlo una vez en un banner amarillo arriba del panel. Si `IsReadOnly`, banner + todos los botones de mutación deshabilitados.
- Al recibir `repository.Changed`: recargar lista conservando la selección por id (si desapareció, seleccionar la primera).

Textos fijos:
- Eliminar: `¿Eliminar la plantilla «{0}»? Esta acción no se puede deshacer.`
- Restaurar: `Se restaurarán las plantillas predeterminadas (Tiendas y BBvs) a su contenido original. Tus plantillas personalizadas no se modifican. ¿Continuar?`
- Sin guardar: `La plantilla «{0}» tiene cambios sin guardar. ¿Deseas guardarlos?` (Sí / No / Cancelar)
- Mover tienda (antes de guardar, si `FindByStore` devuelve otra): `La tienda {0} está vinculada a «{1}». ¿Vincularla a esta plantilla?`

- [ ] **Step 1: Pruebas que fallan** (`TemplateEditorStateTests`): tras `Load`, `IsDirty == false`; cambiar y regresar al valor original → `false`; cambiar tiendas (orden distinto, mismo conjunto) → `false`; `LoadNew()` → `IsNew`, `IsDirty == false` hasta escribir; `ToDraft()` recorta el nombre.
- [ ] **Step 2:** Run → FAIL.
- [ ] **Step 3:** Implementar `TemplateEditorState`, luego `TemplateManagerControl` y `TemplateManagerDialog` (tamaño mínimo 900×600, `StartPosition = CenterParent`, `ShowInTaskbar = false`).
- [ ] **Step 4:** Run `dotnet test` → PASS; `dotnet build` sin advertencias nuevas.
- [ ] **Step 5:** Commit `feat: add template manager panel and dialog`.

### Task 7: Integración en MainForm, Sitemap y Formato

**Files:**
- Modify: `Forms/MainForm.cs`, `Forms/MainForm.Designer.cs`, `Controls/Sitemap/SitemapControl.cs`, `Controls/Sitemap/SitemapControl.Designer.cs`, `Controls/Formato/FormatoControl.cs`, `Controls/Formato/FormatoControl.Designer.cs`

**Interfaces:**
- Consumes: `TemplateManagerControl`, `TemplateManagerDialog`, `SitemapTemplateRepository.Default`, `AppState.Set/TryGetLastTemplateId`, `SitemapEngineRequest.TemplateId`.

Comportamiento:
- **MainForm:** sexta pestaña `tabPlantillas` ("Plantillas") con `TemplateManagerControl(SitemapTemplateRepository.Default)` en `Dock=Fill`. `tabControlPrograms.Deselecting` → si se sale de Plantillas y `!ConfirmLeave()`, `e.Cancel = true`. `FormClosing` → mismo chequeo.
- **Sitemap:** reemplazar `_templateNormalRadio`/`_templateNubeRadio` en `templateGroup` por `_templateCombo` (`DropDownStyle = DropDownList`, items = plantillas, muestra `Name`) + botón `Administrar plantillas…` + `Label` de pista. Selección inicial: última recordada (`AppState`, contexto `"sitemap"`) → vinculada a la tienda marcada → primera. Al marcar una tienda con plantilla vinculada → seleccionarla y mostrar pista `Plantilla sugerida para {tienda}`; nombre manual de tienda → no cambia la plantilla. `repository.Changed` → repoblar conservando id; si el id desapareció → fallback y pista `La plantilla anterior ya no existe; se seleccionó «{0}».`. `ProcessButton_Click` envía `TemplateId`; guarda la elección en `AppState`. El botón abre `TemplateManagerDialog(repo, idActual)` y al cerrar selecciona `SelectedTemplateId`. Agregar el combo y botón a `_inputControls` (se deshabilitan en `SetBusy`). Reemplazar los arreglos locales `StoresLeft/StoresRight` por `StoreCatalog`. Actualizar `ShowHelp` (paso 4: "Elige la plantilla (o adminístralas con «Administrar plantillas…»).").
- **Formato:** reemplazar `_templateAutoRadio`/`_templateTiendasRadio`/`_templateBbvsRadio` por combo con primer item `Detección automática` (valor `"auto"`) + plantillas; botón `Administrar plantillas…`; recordar elección (contexto `"formato"`); mismo refresco por `Changed`. Actualizar `ShowHelp`.
- Desuscribirse de `repository.Changed` en `Dispose` de cada control.

- [ ] **Step 1:** Implementar MainForm (pestaña + guardas).
- [ ] **Step 2:** Implementar Sitemap (Designer: eliminar radios, agregar combo/botón/label al `templatePanel`).
- [ ] **Step 3:** Implementar Formato.
- [ ] **Step 4:** Run `dotnet build S3Tools.slnx` y `dotnet test` → PASS.
- [ ] **Step 5: Prueba manual** (`dotnet run --project src/S3Tools`), con `%LocalAppData%\S3Tools\sitemap-templates.json` borrado antes:
  1. Primer arranque: pestaña Plantillas muestra "Tiendas (Normal)" y "BBvs (Nube)"; el archivo existe.
  2. Sitemap con ProductosTX → combo en Tiendas; marcar BBvsBB2 → combo en BBvs con pista.
  3. Importar un `.json` exportado de WebScraper, vincular Altinor, guardar → aparece en combos de Sitemap y Formato sin reiniciar.
  4. Generar sitemaps con la plantilla nueva e importar uno en WebScraper → carga sin errores.
  5. Editar JSON con error → estado rojo con línea/columna, Guardar deshabilitado.
  6. Editar sin guardar y cambiar de pestaña / cerrar diálogo / cerrar app → aparece el prompt; Cancelar conserva la edición.
  7. Con la plantilla seleccionada en Sitemap, eliminarla desde el diálogo → combo cae en otra con pista.
  8. Cerrar y reabrir la app → plantillas y última selección persisten.
  9. Corromper el archivo a mano → la app arranca, recupera de `.bak` y muestra el banner.
  10. Restaurar predeterminadas → Tiendas/BBvs vuelven a su contenido; la plantilla importada sigue intacta.
  11. Formato en Auto con un CSV exportado de la plantilla nueva → mensaje final muestra su nombre.
  12. `dotnet publish -p:PublishProfile=FolderProfile` → el exe single-file arranca y siembra las plantillas sin los `.json` junto al exe.
- [ ] **Step 6:** Commit `feat: integrate template manager in main form, sitemap and formato`.

### Task 8: Documentación y versión

**Files:**
- Modify: `README.md`, `docs/ManualUsuario.md`, `src/S3Tools/S3Tools.csproj`

- [ ] **Step 1:** Versión `2.6.0.0` (`Version`, `AssemblyVersion`, `FileVersion`).
- [ ] **Step 2:** README: sección "Plantillas" (pestaña, diálogo, ubicación del archivo, restaurar) y actualizar "Estado actual" (las plantillas de sitemap ya no van junto al exe). Manual: reemplazar "Plantilla: Normal/Nube" y "Auto/Tiendas/BBvs" por el combo; nueva sección "Administrar plantillas" con los pasos de crear/importar/vincular tiendas/restaurar; corregir la ruta de persistencia (`%LocalAppData%\S3Tools\`, hoy dice `S3Integracion`).
- [ ] **Step 3:** Commit `docs: document sitemap template manager and bump version to 2.6.0`.

---

## Verificación end-to-end

- `dotnet build S3Tools.slnx` sin errores.
- `dotnet test S3Tools.slnx` → todas las pruebas en verde (caracterización, validador, store, repositorio, motores, editor).
- Checklist manual de Task 7 Step 5 completo, incluido el publish single-file.
- Antes del PR: `superpowers:requesting-code-review` sobre la rama completa.

## Ejecución recomendada

**Subagent-driven** (`superpowers:subagent-driven-development`): 8 tareas encadenadas por interfaces (validador → store → repositorio → motores → UI); un revisor fresco por tarea atrapa desajustes de firmas antes de que se propaguen, y un error en la capa de persistencia costaría datos del usuario. Alternativa más barata: ejecución nativa (`superpowers:executing-plans`) con una revisión final de rama.
