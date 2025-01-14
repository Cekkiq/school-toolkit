@echo off
setlocal enabledelayedexpansion

if "%1"=="" (
    echo Nebyl zadaný parametr. Použijte například: c.bat -f neco.bat
    exit /b
)

if "%1"=="-f" (
    if "%2"=="" (
        echo Nebyl zadaný soubor pro -f.
        exit /b
    )
    set param=%2
    set __COMPAT_LAYER=RunAsInvoker
    start "" "%param%"
) else (
    echo Neplatný parametr: %1
    echo Použijte například: c.bat -f neco.bat
    exit /b
)
