# S3Tools .NET Framework 4.8 → .NET 10.0 + Python Engine Migration Tasks

## Overview

This document tracks the execution of the S3Tools migration from .NET Framework 4.8 to .NET 10.0-windows, including the elimination of all Python engine dependencies and migration to native C# implementations. The migration follows an All-At-Once strategy with functional phasing for engine implementations.

**Progress**: 4/6 tasks complete (67%) ![0%](https://progress-bar.xyz/67)

---

## Tasks

### [✓] TASK-001: Atomic framework upgrade and initial compilation fixes *(Completed: 2026-03-13 00:06)*
**References**: Plan §Phase 0, Plan §Step 1

- [✓] (1) Convert S3Integración_programs.csproj to SDK-style format per Plan §Step 1.1 (replace entire file with SDK-style template)
- [✓] (2) Project file is SDK-style with <Project Sdk="Microsoft.NET.Sdk"> (**Verify**)
- [✓] (3) Update TargetFramework to net10.0-windows and add <UseWindowsForms>true</UseWindowsForms> per Plan §Step 1.2
- [✓] (4) TargetFramework is net10.0-windows (**Verify**)
- [✓] (5) Add PackageReference elements for ClosedXML, System.Drawing.Common, System.Configuration.ConfigurationManager per Plan §Step 1.3 (use latest compatible versions)
- [✓] (6) All 3 packages referenced in project file (**Verify**)
- [✓] (7) Run dotnet restore
- [✓] (8) Dependencies restored successfully (**Verify**)
- [✓] (9) Run dotnet build to identify compilation errors
- [✓] (10) Build completes (compilation errors expected and acceptable at this stage) (**Verify**)
- [✓] (11) Fix framework-related compilation errors that do not require engine classes per Plan §Breaking Changes Catalog (focus on assembly references, using statements)
- [✓] (12) Run dotnet build to verify framework-related fixes
- [✓] (13) Project builds (remaining errors for engine-dependent code acceptable) (**Verify**)
- [✓] (14) Commit changes with message: "TASK-001: Convert to SDK-style, target net10.0-windows, add packages"

---

### [✓] TASK-002: Implement Formato C# engine *(Completed: 2026-03-12 17:11)*
**References**: Plan §Phase 1, Plan §Formato Engine Migration Strategy, Plan §Step 2.1

- [✓] (1) Create FormatoDotNetEngine.cs implementing all functions per Plan §Formato Engine (template loading, header normalization, CSV/XLSX processing with ClosedXML)
- [✓] (2) FormatoDotNetEngine.cs file exists with Handle() entry point (**Verify**)
- [✓] (3) Implement request/response classes matching JSON contract per Plan §Formato Engine Migration Strategy
- [✓] (4) JSON contract classes defined (FormatoEngineRequest, FormatoEngineResponse) (**Verify**)
- [✓] (5) Update FormatoEngineClient.cs to call FormatoDotNetEngine.Handle() first with Python fallback per Plan §Fallback Pattern
- [✓] (6) FormatoEngineClient.cs uses C# engine with fallback pattern (**Verify**)
- [✓] (7) Test Formato engine with CSV files from Assets folder per Plan §Test File Inventory
- [✓] (8) CSV files processed correctly (headers match expected values) (**Verify**)
- [✓] (9) Test Formato engine with XLSX files from Assets folder per Plan §Test File Inventory
- [✓] (10) XLSX files processed correctly (headers match expected values) (**Verify**)
- [✓] (11) Verify template detection matches Python behavior per Plan §Validation Criteria
- [✓] (12) Template detection produces same results as Python (**Verify**)
- [✓] (13) Commit changes with message: "TASK-002: Implement Formato C# engine with Python fallback"

---

### [✓] TASK-003: Implement Sitemap C# engine *(Completed: 2026-03-12 17:15)*
**References**: Plan §Phase 2, Plan §Sitemap Engine Migration Strategy, Plan §Step 2.2

- [✓] (1) Create SitemapDotNetEngine.cs implementing all functions per Plan §Sitemap Engine (XML generation, URL processing, template integration)
- [✓] (2) SitemapDotNetEngine.cs file exists with entry point (**Verify**)
- [✓] (3) Implement request/response classes matching JSON contract
- [✓] (4) JSON contract classes defined for Sitemap (**Verify**)
- [✓] (5) Update SitemapEngineClient.cs to call C# engine first with Python fallback per Plan §Fallback Pattern
- [✓] (6) SitemapEngineClient.cs uses C# engine with fallback (**Verify**)
- [✓] (7) Test Sitemap engine with test files per Plan §Test File Inventory
- [✓] (8) Sitemap outputs match Python behavior (**Verify**)
- [✓] (9) Commit changes with message: "TASK-003: Implement Sitemap C# engine with Python fallback"

---

### [✓] TASK-004: Implement Asin Batcher C# engine *(Completed: 2026-03-12 17:16)*
**References**: Plan §Phase 2, Plan §Asin Batcher Engine Migration Strategy, Plan §Step 2.2

- [✓] (1) Create AsinBatcherDotNetEngine.cs implementing all functions per Plan §Asin Batcher Engine (ASIN processing, duplicate detection, batch generation, ZIP creation)
- [✓] (2) AsinBatcherDotNetEngine.cs file exists with entry point (**Verify**)
- [✓] (3) Implement request/response classes matching JSON contract
- [✓] (4) JSON contract classes defined for Asin Batcher (**Verify**)
- [✓] (5) Update AsinBatcherEngineClient.cs to call C# engine first with Python fallback per Plan §Fallback Pattern
- [✓] (6) AsinBatcherEngineClient.cs uses C# engine with fallback (**Verify**)
- [✓] (7) Test Asin Batcher engine with test files per Plan §Test File Inventory
- [✓] (8) Asin Batcher outputs match Python behavior (**Verify**)
- [✓] (9) Commit changes with message: "TASK-004: Implement Asin Batcher C# engine with Python fallback"

---

### [▶] TASK-005: Fix remaining Windows Forms API incompatibilities and final build verification
**References**: Plan §Phase 3, Plan §Breaking Changes Catalog, Plan §Step 3

- [▶] (1) Run dotnet build to identify remaining compilation errors
- [ ] (2) Build completes with error list (**Verify**)
- [ ] (3) Fix Windows Forms API incompatibilities per Plan §Breaking Changes Catalog (focus on 5,708 binary incompatible issues - verify assembly references, regenerate designer files if needed)
- [ ] (4) Fix System.Drawing issues if any (System.Drawing.Common package should resolve these)
- [ ] (5) Fix Configuration system issues if any (System.Configuration.ConfigurationManager package should resolve these)
- [ ] (6) Run dotnet build after fixes
- [ ] (7) Solution builds with 0 errors and 0 warnings (**Verify**)
- [ ] (8) Launch application to verify no startup exceptions
- [ ] (9) Application starts successfully (**Verify**)
- [ ] (10) Verify TabControl contains 3 tabs (Formato, Sitemap, Asin Batcher) without exceptions
- [ ] (11) All tabs present and accessible (**Verify**)
- [ ] (12) Commit changes with message: "TASK-005: Fix Windows Forms API incompatibilities, achieve zero errors"

---

### [ ] TASK-006: Comprehensive testing and Python engine removal
**References**: Plan §Phase 4, Plan §Testing & Validation Strategy, Plan §Test File Inventory

- [ ] (1) Test Formato engine with all CSV and XLSX files per Plan §Test File Inventory (Assets\CSV and Assets\XLSX folders)
- [ ] (2) All Formato test files processed correctly (**Verify**)
- [ ] (3) Test Sitemap engine with sitemap test files per Plan §Test File Inventory
- [ ] (4) All Sitemap test files processed correctly (**Verify**)
- [ ] (5) Test Asin Batcher engine with ASIN test files per Plan §Test File Inventory
- [ ] (6) All Asin Batcher test files processed correctly (**Verify**)
- [ ] (7) Run performance benchmark comparing C# vs Python engines per Plan §Performance Testing
- [ ] (8) C# engines meet or exceed Python performance (**Verify**)
- [ ] (9) Delete Engines/ folder containing Python engine files per Plan §Python Removal
- [ ] (10) Engines/ folder deleted from repository (**Verify**)
- [ ] (11) Remove Python fallback code from FormatoEngineClient.cs, SitemapEngineClient.cs, AsinBatcherEngineClient.cs
- [ ] (12) All client classes call C# engines directly without fallback (**Verify**)
- [ ] (13) Run dotnet build to verify no Python dependencies
- [ ] (14) Solution builds with 0 errors and no Python references (**Verify**)
- [ ] (15) Run final test suite to verify all functionality works without Python
- [ ] (16) All tests pass with 0 failures (**Verify**)
- [ ] (17) Commit changes with message: "TASK-006: Complete testing and Python engine removal"

---




