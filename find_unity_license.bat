@echo off
echo 🌲 FOREST WARFARE SHOOTER - Buscador de Licencias Unity
echo ================================================================
echo Este script busca tu archivo de licencia Unity (.ulf)
echo para configurar GitHub Actions automaticamente.
echo ================================================================

echo.
echo 🔍 Buscando archivos .ulf en Downloads...
dir "%USERPROFILE%\Downloads\*.ulf" /s 2>nul
if %ERRORLEVEL% EQU 0 (
    echo ✅ Encontrados en Downloads!
) else (
    echo ❌ No encontrados en Downloads
)

echo.
echo 🔍 Buscando archivos .ulf en Desktop...
dir "%USERPROFILE%\Desktop\*.ulf" /s 2>nul
if %ERRORLEVEL% EQU 0 (
    echo ✅ Encontrados en Desktop!
) else (
    echo ❌ No encontrados en Desktop
)

echo.
echo 🔍 Buscando archivos .ulf en AppData...
dir "%USERPROFILE%\AppData\Roaming\Unity\License\*.ulf" /s 2>nul
if %ERRORLEVEL% EQU 0 (
    echo ✅ Encontrados en AppData!
) else (
    echo ❌ No encontrados en AppData
)

echo.
echo 🔍 Buscando en Unity Hub Editor...
dir "C:\Program Files\Unity\Hub\Editor\*\Editor\Data\Resources\License\*.ulf" /s 2>nul
if %ERRORLEVEL% EQU 0 (
    echo ✅ Encontrados en Unity Hub!
) else (
    echo ❌ No encontrados en Unity Hub
)

echo.
echo 🔍 Busqueda recursiva rapida en todo el usuario...
for /r "%USERPROFILE%" %%f in (*.ulf) do (
    echo ✅ ENCONTRADO: %%f
    echo    Tamaño: %%~zf bytes
    echo    Fecha: %%~tf
    echo.
)

echo.
echo ================================================================
echo 🎯 RESULTADO:
echo Si encontraste archivos .ulf:
echo 1. Abre el archivo con Notepad
echo 2. Copia TODO el contenido (Ctrl+A, Ctrl+C)
echo 3. Ve a GitHub Secrets y crea UNITY_LICENSE
echo 4. Pega el contenido completo
echo.
echo Si NO encontraste archivos:
echo 1. Ve a Unity Hub > Licenses > + Add license
echo 2. Selecciona "Activate with license request"
echo 3. Create license request (guarda el .alf)
echo 4. Sube el .alf en unity3d.com/license
echo 5. Descarga el .ulf resultante
echo ================================================================

pause