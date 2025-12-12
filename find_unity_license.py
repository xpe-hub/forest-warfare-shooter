#!/usr/bin/env python3
"""
Script para encontrar archivos de licencia de Unity (.ulf)
Busca en todas las ubicaciones comunes donde Unity guarda las licencias
"""

import os
import glob
from pathlib import Path

def find_unity_licenses():
    """
    Busca archivos .ulf de Unity en todas las ubicaciones posibles
    """
    print("🔍 Buscando archivos de licencia Unity (.ulf)...")
    print("=" * 60)
    
    # Ubicaciones comunes donde Unity guarda licencias
    search_paths = []
    
    # Usuario actual
    user_path = os.path.expanduser("~")
    search_paths.extend([
        os.path.join(user_path, "Downloads"),
        os.path.join(user_path, "Desktop"),
        os.path.join(user_path, "Documents"),
        os.path.join(user_path, "AppData", "Roaming", "Unity", "License"),
        os.path.join(user_path, "AppData", "Local", "Unity", "License"),
    ])
    
    # Directorio de instalación de Unity
    program_files = os.environ.get("ProgramFiles", "C:\\Program Files")
    search_paths.extend([
        os.path.join(program_files, "Unity", "Hub", "Editor"),
        os.path.join(program_files, "Unity", "Hub", "Editor", "*", "Editor", "Data", "Resources", "License"),
        os.path.join(program_files, "Unity", "Editor", "Data", "Resources", "License"),
    ])
    
    # Búsqueda adicional en todo el sistema (puede ser lenta)
    system_search = [
        os.path.join(user_path, "**", "*.ulf"),
        os.path.join(user_path, "**", "Unity*", "**", "*.ulf"),
    ]
    
    found_files = []
    
    # Buscar en rutas específicas
    for path in search_paths:
        if os.path.exists(path):
            print(f"📁 Buscando en: {path}")
            # Buscar archivos .ulf
            ulf_files = glob.glob(os.path.join(path, "*.ulf"))
            if ulf_files:
                found_files.extend(ulf_files)
                for file in ulf_files:
                    print(f"  ✅ ENCONTRADO: {file}")
                    print(f"     Tamaño: {os.path.getsize(file)} bytes")
                    print(f"     Modificado: {os.path.getmtime(file)}")
            else:
                print(f"  ❌ No encontrado en esta ruta")
    
    print("\n" + "=" * 60)
    
    # Búsqueda recursiva en todo el usuario (más lenta)
    print("🔄 Búsqueda adicional en todo el sistema...")
    for pattern in system_search:
        matches = glob.glob(pattern, recursive=True)
        for match in matches:
            if match not in found_files:
                found_files.append(match)
                print(f"  ✅ ENCONTRADO: {match}")
    
    # Mostrar resultados
    if found_files:
        print(f"\n🎉 ¡ENCONTRADOS {len(found_files)} ARCHIVOS DE LICENCIA!")
        print("=" * 60)
        for i, file_path in enumerate(found_files, 1):
            print(f"\n📄 LICENCIA #{i}:")
            print(f"   📂 Ruta: {file_path}")
            print(f"   📏 Tamaño: {os.path.getsize(file_path)} bytes")
            
            # Intentar leer el contenido (primeras líneas)
            try:
                with open(file_path, 'r', encoding='utf-8', errors='ignore') as f:
                    content = f.read(500)  # Primeros 500 caracteres
                    print(f"   📝 Contenido (primeros 500 chars):")
                    print(f"   {content[:200]}...")
            except:
                print(f"   ❌ No se pudo leer el contenido")
            
            print(f"   💾 Para GitHub Actions:")
            print(f"      - Abre este archivo con Notepad")
            print(f"      - Copia TODO el contenido")
            print(f"      - Pégalo en GitHub como UNITY_LICENSE secret")
    
    else:
        print("\n❌ NO SE ENCONTRARON ARCHIVOS DE LICENCIA (.ulf)")
        print("=" * 60)
        print("\n💡 SOLUCIONES:")
        print("1. Revisa tu carpeta Downloads/Descargas")
        print("2. Busca archivos con nombres como:")
        print("   - Unity_v2022.x.ulf.unity.lic")
        print("   - Unity_*.ulf")
        print("   - license*.ulf")
        print("3. Genera una nueva licencia:")
        print("   - Unity Hub > Licenses > + Add license")
        print("   - Activate with license request")
        print("   - Create license request")
    
    return found_files

def main():
    print("🌲 FOREST WARFARE SHOOTER - Buscador de Licencias Unity")
    print("=" * 60)
    print("Este script busca tu archivo de licencia Unity (.ulf)")
    print("para configurar GitHub Actions automáticamente.")
    print("=" * 60)
    
    try:
        licenses = find_unity_licenses()
        
        if licenses:
            print(f"\n🚀 PRÓXIMO PASO:")
            print("1. Abre el archivo .ulf encontrado con Notepad")
            print("2. Copia TODO el contenido")
            print("3. Ve a: https://github.com/xpe-hub/forest-warfare-shooter")
            print("4. Settings > Secrets and variables > Actions")
            print("5. Crea secret UNITY_LICENSE y pega el contenido")
            print("\n🎮 ¡GitHub Actions podrá compilar automáticamente!")
        
    except Exception as e:
        print(f"\n❌ ERROR: {e}")
        print("🔧 Revisa los permisos o ejecuta como administrador")

if __name__ == "__main__":
    main()