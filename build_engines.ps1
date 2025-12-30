param(
    [string]$EnginesPath = ""
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$enginesDir = $EnginesPath
if ([string]::IsNullOrWhiteSpace($enginesDir)) {
    $enginesDir = Join-Path $root "Engines"
}

if (-not (Test-Path $enginesDir)) {
    Write-Error ("Engines folder not found: {0}" -f $enginesDir)
    exit 1
}

$pythonCmd = Get-Command python -ErrorAction SilentlyContinue
if (-not $pythonCmd) {
    Write-Error "Python not found in PATH."
    exit 1
}
$python = $pythonCmd.Source

& $python -m PyInstaller --version *> $null
if ($LASTEXITCODE -ne 0) {
    Write-Error "PyInstaller not available. Run: python -m pip install pyinstaller"
    exit 1
}

$globalTemplates = @()
$globalTemplateDir = Join-Path $enginesDir "Sitemap"
if (Test-Path $globalTemplateDir) {
    $globalTemplates = Get-ChildItem -Path $globalTemplateDir -Filter *.json -File -ErrorAction SilentlyContinue
}

$scriptFiles = Get-ChildItem -Path $enginesDir -Recurse -Filter *.py -File |
    Where-Object { $_.FullName -notmatch '\\(build|dist|__pycache__)\\' }

if (-not $scriptFiles) {
    Write-Error ("No .py engines found under {0}" -f $enginesDir)
    exit 1
}

foreach ($script in $scriptFiles) {
    $scriptDir = $script.DirectoryName
    $workPath = Join-Path $scriptDir "build"
    $specPath = $workPath
    $dataSeparator = [System.IO.Path]::PathSeparator

    Write-Host ("Building {0}" -f $script.FullName)

    $dataEntries = New-Object System.Collections.Generic.HashSet[string]

    $localData = Get-ChildItem -Path $scriptDir -Recurse -File |
        Where-Object {
            $_.FullName -notmatch '\\(build|dist|__pycache__)\\' -and
            $_.Extension -notin ".py", ".pyc", ".spec", ".exe", ".dll", ".pdb"
        }

    foreach ($item in $localData) {
        $rel = $item.FullName.Substring($scriptDir.Length).TrimStart('\')
        $dest = Split-Path -Parent $rel
        if ([string]::IsNullOrWhiteSpace($dest)) {
            $dest = "."
        }
        $null = $dataEntries.Add(("{0}|{1}" -f $item.FullName, $dest))
    }

    foreach ($template in $globalTemplates) {
        $null = $dataEntries.Add(("{0}|." -f $template.FullName))
    }

    $args = @(
        "-m", "PyInstaller",
        "--onefile",
        "--clean",
        "--noconfirm",
        "--distpath", $scriptDir,
        "--workpath", $workPath,
        "--specpath", $specPath
    )

    foreach ($entry in $dataEntries) {
        $parts = $entry -split '\|', 2
        $src = $parts[0]
        $dest = $parts[1]
        $args += ("--add-data={0}{1}{2}" -f $src, $dataSeparator, $dest)
    }

    $args += $script.FullName

    & $python @args
    if ($LASTEXITCODE -ne 0) {
        Write-Error ("Build failed for {0}" -f $script.FullName)
        exit 1
    }
}

Write-Host "All engines built."
