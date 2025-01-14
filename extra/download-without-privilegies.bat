@echo off
setlocal enabledelayedexpansion

set targetFile=target.bat

if "%1"=="" (
    set param=default.bat
) else (
    set param=%*
)

(
    for /f "tokens=*" %%a in ('findstr /v /b "set __COMPAT_LAYER" "%targetFile%"') do (
        @echo %%a
    )
) > temp.bat

echo set __COMPAT_LAYER=RunAsInvoker >> temp.bat
echo start "%param%" >> neakysouborbezmezer.exe

move /y temp.bat %targetFile% >nul
