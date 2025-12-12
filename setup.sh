#!/bin/bash

# Script de configuración para Forest Warfare Shooter
# Este script ayuda a configurar el proyecto Unity localmente

echo "🌲 Forest Warfare Shooter - Configuración del Proyecto"
echo "=================================================="

# Verificar si Unity Hub está instalado
if command -v unity &> /dev/null; then
    echo "✅ Unity Editor encontrado"
else
    echo "❌ Unity Editor no encontrado"
    echo "📥 Descarga Unity Hub desde: https://unity.com/download"
    exit 1
fi

# Verificar versión de Unity
UNITY_VERSION=$(unity --version 2>/dev/null || echo "No detectado")
echo "🔧 Versión de Unity: $UNITY_VERSION"

# Crear directorios necesarios
echo "📁 Creando estructura de directorios..."
mkdir -p Assets/Scenes
mkdir -p Assets/Prefabs
mkdir -p Assets/Materials
mkdir -p Assets/Textures
mkdir -p Assets/Audio

# Verificar si las escenas existen
if [ ! -f "Assets/Scenes/MainMenu.unity" ]; then
    echo "⚠️  Las escenas no están creadas. El proyecto necesita escenas Unity."
    echo "📝 Por favor crea las siguientes escenas en Unity Editor:"
    echo "   - Assets/Scenes/MainMenu.unity"
    echo "   - Assets/Scenes/GameScene.unity" 
    echo "   - Assets/Scenes/RankingScene.unity"
    echo "   - Assets/Scenes/MapSelection.unity"
fi

# Verificar configuración de GitHub
if [ ! -f ".github/workflows/build.yml" ]; then
    echo "⚠️  GitHub Actions no configurado"
    echo "📖 Lee GITHUB_ACTIONS_SETUP.md para configurar compilación automática"
fi

echo ""
echo "🎮 Configuración completada!"
echo "📖 Lee README.md para instrucciones detalladas"
echo "🚀 Para compilar: Unity Editor > File > Build Settings"
echo ""
echo "🎯 Para GitHub Actions:"
echo "   1. Configura secrets en GitHub (UNITY_LICENSE, UNITY_EMAIL, UNITY_PASSWORD)"
echo "   2. Haz git push para compilar automáticamente"
echo "   3. Crea releases para builds automáticos"
