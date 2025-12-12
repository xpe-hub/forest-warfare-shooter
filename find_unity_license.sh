#!/bin/bash
# Script para encontrar o generar licencia Unity para GitHub Actions

echo "🌲 FOREST WARFARE SHOOTER - Licencia Unity para GitHub Actions"
echo "================================================================"

# Función para buscar archivos .ulf
find_ulf_files() {
    echo "🔍 Buscando archivos .ulf existentes..."
    
    # Buscar en Downloads
    if ls ~/Downloads/*.ulf 2>/dev/null; then
        echo "✅ Encontrados en Downloads:"
        ls -la ~/Downloads/*.ulf
    else
        echo "❌ No encontrados en Downloads"
    fi
    
    # Buscar en Desktop
    if ls ~/Desktop/*.ulf 2>/dev/null; then
        echo "✅ Encontrados en Desktop:"
        ls -la ~/Desktop/*.ulf
    else
        echo "❌ No encontrados en Desktop"
    fi
    
    # Buscar en Unity directories
    if ls ~/Library/Application\ Support/Unity/License/*.ulf 2>/dev/null; then
        echo "✅ Encontrados en Unity License:"
        ls -la ~/Library/Application\ Support/Unity/License/*.ulf
    else
        echo "❌ No encontrados en Unity License"
    fi
}

# Función para generar nueva licencia
generate_new_license() {
    echo ""
    echo "🎯 GENERAR NUEVA LICENCIA PARA GITHUB ACTIONS"
    echo "=============================================="
    echo ""
    echo "OPCIÓN 1: Crear solicitud de licencia"
    echo "1. Abre Unity Hub"
    echo "2. Ve a Licenses"
    echo "3. Clic + Add license"
    echo "4. Selecciona 'Activate with license request'"
    echo "5. Clic 'Create license request'"
    echo "6. Guarda el archivo .alf en Desktop"
    echo ""
    echo "OPCIÓN 2: Descargar directamente"
    echo "1. Ve a: https://license.unity3d.com/"
    echo "2. Selecciona 'Personal'"
    echo "3. Llena el formulario:"
    echo "   - Email: xpepaneles@gmail.com"
    echo "   - Use: Personal/non-commercial"
    echo "   - Revenue: Less than $100k"
    echo "4. Descarga el archivo .ulf"
    echo ""
    echo "📂 UBICACIONES COMUNES DEL .ulf:"
    echo "- ~/Downloads/"
    echo "- ~/Desktop/"
    echo "- ~/Documents/"
    echo ""
}

# Función para preparar GitHub Actions
prepare_github() {
    echo ""
    echo "🚀 CONFIGURAR GITHUB ACTIONS"
    echo "============================="
    echo ""
    echo "Una vez que tengas el archivo .ulf:"
    echo ""
    echo "1. Abre el archivo .ulf con Notepad/TextEdit"
    echo "2. Copia TODO el contenido (Ctrl+A, Ctrl+C)"
    echo "3. Ve a: https://github.com/xpe-hub/forest-warfare-shooter"
    echo "4. Settings > Secrets and variables > Actions"
    echo "5. Crea estos 3 secrets:"
    echo ""
    echo "   UNITY_EMAIL: xpepaneles@gmail.com"
    echo "   UNITY_PASSWORD: [tu contraseña]"
    echo "   UNITY_LICENSE: [pega TODO el contenido del .ulf]"
    echo ""
    echo "6. Haz git push y GitHub compilará automáticamente!"
    echo ""
}

# Función principal
main() {
    echo "🔧 ¿Qué quieres hacer?"
    echo "1. Buscar archivos .ulf existentes"
    echo "2. Generar nueva licencia"
    echo "3. Preparar GitHub Actions"
    echo "4. Todo lo anterior"
    echo ""
    read -p "Selecciona (1-4): " choice
    
    case $choice in
        1)
            find_ulf_files
            ;;
        2)
            generate_new_license
            ;;
        3)
            prepare_github
            ;;
        4)
            find_ulf_files
            echo ""
            read -p "¿Quieres generar nueva licencia? (y/n): " generate
            if [[ $generate =~ ^[Yy]$ ]]; then
                generate_new_license
            fi
            prepare_github
            ;;
        *)
            echo "❌ Opción inválida"
            ;;
    esac
}

# Ejecutar
main