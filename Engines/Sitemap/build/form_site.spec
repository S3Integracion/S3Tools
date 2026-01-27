# -*- mode: python ; coding: utf-8 -*-


a = Analysis(
    ['C:\\Users\\Public\\GitHub\\S3Tools\\Engines\\Sitemap\\form_site.py'],
    pathex=[],
    binaries=[],
    datas=[('C:\\Users\\Public\\GitHub\\S3Tools\\Engines\\Sitemap\\PlantillaSitemapsBBvs.json', '.'), ('C:\\Users\\Public\\GitHub\\S3Tools\\Engines\\Sitemap\\PlantillaSitemapsTiendas.json', '.')],
    hiddenimports=[],
    hookspath=[],
    hooksconfig={},
    runtime_hooks=[],
    excludes=[],
    noarchive=False,
    optimize=0,
)
pyz = PYZ(a.pure)

exe = EXE(
    pyz,
    a.scripts,
    a.binaries,
    a.datas,
    [],
    name='form_site',
    debug=False,
    bootloader_ignore_signals=False,
    strip=False,
    upx=True,
    upx_exclude=[],
    runtime_tmpdir=None,
    console=True,
    disable_windowed_traceback=False,
    argv_emulation=False,
    target_arch=None,
    codesign_identity=None,
    entitlements_file=None,
)
