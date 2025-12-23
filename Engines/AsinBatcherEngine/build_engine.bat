@echo off
setlocal

pushd "%~dp0" || exit /b 1
set "SCRIPT_DIR=%CD%"

where python >nul 2>nul
if errorlevel 1 (
  echo Python no encontrado en PATH.
  popd
  exit /b 1
)

python -m PyInstaller --onefile --clean ^
  --name AsinBatcherEngine ^
  --add-data "Formato de Sitemap.json;." ^
  --distpath "%SCRIPT_DIR%" ^
  --workpath "%SCRIPT_DIR%\\build" ^
  --specpath "%SCRIPT_DIR%" ^
  "%SCRIPT_DIR%\\engine.py"

if errorlevel 1 (
  echo Error al generar el engine.
  popd
  exit /b 1
)

echo Engine generado: %SCRIPT_DIR%AsinBatcherEngine.exe
popd
endlocal
