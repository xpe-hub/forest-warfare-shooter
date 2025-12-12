# 🚀 GitHub Actions - Automatización de Compilación

## ⚠️ Configuración Requerida

Para que la compilación automática funcione, necesitas configurar las siguientes **Secretas de GitHub** en tu repositorio:

### 🔐 Secrets de GitHub (OBLIGATORIOS)

Ve a tu repositorio: **Settings > Secrets and variables > Actions**

#### 1. **UNITY_LICENSE** 
- **Qué es**: La licencia de Unity que permite compilar automáticamente
- **Cómo obtenerla**:
  1. Abre Unity Editor localmente
  2. Ve a: **Help > Manage License**
  3. Clic en "Manual activation" 
  4. Selecciona "Save license request to file"
  5. Crea un archivo `.alf`
  6. Ve a: https://license.unity3d.com/
  7. Sube el archivo `.alf` y descarga la licencia
  8. La licencia será un archivo `.ulf`
  9. **Contenido del archivo `.ulf` completo** → Copia y pega en el secret `UNITY_LICENSE`

#### 2. **UNITY_EMAIL**
- **Qué es**: El email de tu cuenta de Unity
- **Ejemplo**: `tuemail@gmail.com`

#### 3. **UNITY_PASSWORD**
- **Qué es**: La contraseña de tu cuenta de Unity
- **Importante**: Asegúrate de usar la contraseña correcta

### 📝 Pasos Detallados para Unity License

```bash
# 1. En Unity Editor local:
Help > Manage License > Manual activation

# 2. Guardar como: Unity_v2022.x.ulf.unity.lic

# 3. Ir a: https://license.unity3d.com/
# 4. Subir el archivo .alf
# 5. Descargar el archivo .ulf

# 6. El contenido completo del archivo .ulf va en el secret UNITY_LICENSE
```

## 🏗️ ¿Qué Hace GitHub Actions?

### Automáticamente Compila para:
- **Windows** (.exe)
- **macOS** (.app)
- **Linux** (.x86_64)

### Cada vez que:
- Haces `git push` a la rama `main`
- Creas un nuevo **Release** en GitHub
- Ejecutas manualmente desde Actions

### Los Builds Quedan Disponibles:
- Como **Artifacts** en la página de Actions
- Como **Releases** cuando etiquetas una versión
- Descarga directa desde GitHub

## 🎯 Cómo Usar GitHub Actions

### Opción 1: Push Triggers (Automático)
```bash
git add .
git commit -m "Update game features"
git push origin main
```
**Resultado**: GitHub Actions automáticamente compila y genera builds.

### Opción 2: Manual Trigger
1. Ve a la pestaña **Actions** en tu repositorio
2. Selecciona **"Build Unity Project"**
3. Clic **"Run workflow"**
4. Selecciona rama y ejecuta

### Opción 3: Release Build
1. Ve a **Releases** en tu repositorio
2. **Draft a new release**
3. Tag: `v1.0.0` 
4. **Publish release**
**Resultado**: Se genera automáticamente builds y se adjuntan al release

## 📦 Archivos Generados

### Windows Build
```
Build/Windows/
├── Forest Warfare Shooter.exe
├── Forest Warfare Shooter_Data/
├── UnityCrashHandler64.exe
└── UnityPlayer.dll
```

### macOS Build
```
Build/Mac/
├── Forest Warfare Shooter.app
└── Forest Warfare Shooter.app.dSYM/
```

### Linux Build
```
Build/Linux/
├── Forest Warfare Shooter
├── Forest Warfare Shooter_Data/
└── *.so files
```

## 🔧 Configuración Avanzada

### Modificar Build Settings
Edita el archivo `.github/workflows/build.yml` para cambiar:
- **Nombres de archivos** de salida
- **Versiones** del juego
- **Rutas** de build
- **Plataformas** objetivo

### Variables de Configuración
```yaml
build-name: Forest Warfare Shooter
build-version: 1.0.0
build-path: Build/Windows
```

## ⚠️ Limitaciones Conocidas

### Tiempo de Compilación
- **Primera vez**: ~15-20 minutos
- **Compilaciones siguientes**: ~5-10 minutos (con cache)

### Límites de GitHub Actions
- **Minutes por mes**: 2000 (públicos) / 0 (privados)
- **Tiempo por job**: 60 minutos máximo
- **Tamaño del artifact**: 1GB máximo

### Requisitos de Licencia
- **Unity Personal**: ✅ Soportada
- **Unity Plus/Pro**: ✅ Soportada
- **Unity Student**: ✅ Soportada
- **Sin licencia**: ❌ No funciona

## 🎮 Descargar Builds

### Desde Actions
1. Ve a **Actions** tab
2. Selecciona el workflow run
3. Descarga el **artifact** correspondiente

### Desde Releases
1. Ve a **Releases** tab
2. Descarga la versión que quieras
3. Incluye todos los archivos de la plataforma

## 🚨 Solución de Problemas

### Error: "Unity License Not Found"
- Verifica que el secret `UNITY_LICENSE` esté configurado correctamente
- Asegúrate de que el contenido del archivo `.ulf` esté completo

### Error: "Build Failed"
- Revisa los logs en la página de Actions
- Verifica que no haya errores de compilación en el código
- Asegúrate de que las escenas estén configuradas correctamente

### Build Muy Lento
- Es normal la primera vez
- Las siguientes builds usan cache y son más rápidas
- El cache se reinicia si cambias archivos críticos de Unity

## 🎯 Resultado Final

Una vez configurado, tendrás:
- ✅ **Compilación automática** cada vez que hagas push
- ✅ **Ejecutables listos** para Windows, Mac y Linux
- ✅ **Releases automáticas** con builds incluidos
- ✅ **No necesitas Unity** para generar builds

**¡El juego se compila automáticamente desde GitHub!** 🎮