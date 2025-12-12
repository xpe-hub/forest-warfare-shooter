# 🔍 GUÍA RÁPIDA: Encontrar Licencia Unity

## 🚀 OPCIÓN 1: Ejecutar Script Automático (MÁS FÁCIL)

### **Para Windows:**
1. **Descarga** estos archivos a tu computadora:
   - `find_unity_license.bat`
2. **Clic derecho** en el archivo .bat
3. **Selecciona** "Ejecutar como administrador"
4. **El script** busca automáticamente tu licencia

### **Para Mac/Linux:**
1. **Descarga**: `find_unity_license.sh`
2. **Terminal**: `chmod +x find_unity_license.sh`
3. **Ejecuta**: `./find_unity_license.sh`

## 🔍 OPCIÓN 2: Búsqueda Manual

### **Busca en estas carpetas (Windows):**
```
C:\Users\[tu_usuario]\Downloads\
C:\Users\[tu_usuario]\Desktop\
C:\Users\[tu_usuario]\AppData\Roaming\Unity\License\
C:\Program Files\Unity\Hub\Editor\*\Editor\Data\Resources\License\
```

### **Busca archivos que terminen en:**
- `*.ulf`
- `Unity_v2022.x.ulf.unity.lic`
- `Unity_*.ulf`
- `license_*.ulf`

## 📁 UBICACIONES ESPECÍFICAS

### **Unity Hub License Directory:**
```
C:\Users\[tu_usuario]\AppData\Roaming\Unity\License\
├── Unity_*.ulf
└── license.ulf
```

### **Unity Editor License Directory:**
```
C:\Program Files\Unity\Hub\Editor\[versión]\Editor\Data\Resources\License\
└── Unity_v2022.x.ulf.unity.lic
```

### **Downloads Directory:**
```
C:\Users\[tu_usuario]\Downloads\
├── Unity_v2022.x.ulf.unity.lic
└── [otros archivos .ulf]
```

## 🔄 SI NO ENCUENTRAS NADA

### **Generar Nueva Licencia:**
1. **Unity Hub** > **Licenses**
2. **+ Add license**
3. **Activate with license request**
4. **Create license request** → Guardar `.alf`
5. **Unity3d.com/license** → Subir `.alf` → Descargar `.ulf`

## 💻 SCRIPTS INCLUIDOS

### **`find_unity_license.py`**
- Búsqueda completa en Python
- Busca en todas las ubicaciones posibles
- Muestra detalles de archivos encontrados

### **`find_unity_license.bat`**
- Script para Windows (línea de comandos)
- Búsqueda rápida en ubicaciones comunes
- Fácil de ejecutar

### **`find_unity_license.sh`**
- Script para Mac/Linux
- Menú interactivo
- Opciones para buscar o generar

## 🎯 RESULTADO ESPERADO

**Al ejecutar cualquier script, verás algo como:**
```
✅ ENCONTRADO: C:\Users\Usuario\Downloads\Unity_v2022.3.34f1.ulf
   Tamaño: 2048 bytes
   Fecha: 12/12/2025 12:30 PM
```

## 🚀 PRÓXIMO PASO

**Una vez que encuentres el archivo .ulf:**
1. **Ábrelo** con Notepad
2. **Copia** todo el contenido (Ctrl+A, Ctrl+C)
3. **GitHub** > Settings > Secrets > Actions
4. **Crea** secret `UNITY_LICENSE`
5. **Pega** el contenido completo

**¡GitHub Actions podrá compilar automáticamente!** 🎮