# 🌲 Forest Warfare Shooter

Un juego FPS shooter en el bosque con IA avanzada, sistema de rangos creativos, y mapas de mundo abierto inspirados en Battle for Neighborville.

## 🎮 Características Principales

### 🏆 Sistema de Rangos Creativos
- **Esmeralda**: 4 subdivisiones (Esmeralda I, II, III, IV)
- **Patriarca**: 6 subdivisiones (Patriarca I-VI)  
- **Comando**: 4 subdivisiones (Comando I-IV)

### 🤖 IA Avanzada
- **18 personajes únicos** inspirados en Plants vs Zombies y Fall Guys
- **Sistema de combate inteligente** con 4 niveles de dificultad
- **Comportamientos únicos** por personaje y dificultad

### 🗺️ Mapas de Mundo Abierto
- **19+ mapas** inspirados en Battle for Neighborville
- **Variedad de biomas**: bosque, desierto, urbano, montaña
- **Diseños únicos** para diferentes estilos de juego

### 🎯 Sistema de Misiones y Logros
- **Misiones dinámicas** basadas en rangos
- **Sistema de recompensas** progresivo
- **Logros creativos** desbloqueables

## 🎯 Cómo Jugar

### Requisitos del Sistema
- **Unity Editor 2022.3 LTS** o superior
- **Sistema Operativo**: Windows 10/11, macOS 10.15+, o Linux
- **RAM**: Mínimo 4GB, Recomendado 8GB
- **Espacio**: 2GB libres

### Instalación y Configuración
1. **Clonar el repositorio**:
   ```bash
   git clone https://github.com/xpe-hub/forest-warfare-shooter.git
   cd forest-warfare-shooter
   ```

2. **Abrir en Unity**:
   - Abrir Unity Hub
   - "Add project from disk"
   - Seleccionar la carpeta `ForestWarfareShooter`
   - Usar Unity Editor 2022.3 LTS o superior

3. **Configurar Build Settings**:
   - File > Build Settings
   - Platform: Windows/Mac/Linux
   - Architecture: x86_64
   - Target Platform: Desktop

## 🎮 Controles del Juego

### Movimiento
- **WASD**: Movimiento del jugador
- **Espacio**: Saltar
- **Shift**: Correr
- **Ctrl**: Agacharse

### Combate
- **Click izquierdo**: Disparar
- **Click derecho**: Apuntar
- **R**: Recargar arma
- **Q**: Cambiar arma

### Sistema de Rangos
- **Tab**: Abrir panel de rangos
- **M**: Abrir mapa
- **I**: Inventario
- **C**: Estadísticas del jugador

## 🤖 Compilación Automática con GitHub Actions

### ⚡ Compilación Automática
El proyecto incluye **GitHub Actions** que compila automáticamente el juego:

- **Windows** → `.exe`
- **macOS** → `.app`  
- **Linux** → ejecutable

### 🚀 Cómo Activar Compilación Automática

#### Opción 1: Push Automático (Recomendado)
```bash
git add .
git commit -m "Update game features"
git push origin main
```
**Resultado**: GitHub Actions compila automáticamente y genera builds.

#### Opción 2: Manual Trigger
1. Ve a **Actions** tab en GitHub
2. Selecciona **"Build Unity Project"**
3. Clic **"Run workflow"**

#### Opción 3: Release Build
1. Ve a **Releases** en GitHub
2. **Draft a new release**
3. Tag: `v1.0.0`
4. **Publish release**
**Resultado**: Builds automáticos adjuntos al release

### 🔐 Configuración Requerida

Para que funcione, configura estos **Secrets en GitHub**:

1. **Repository Settings** → **Secrets and variables** → **Actions**
2. Agregar secrets:
   - `UNITY_LICENSE`: Tu licencia de Unity (.ulf completo)
   - `UNITY_EMAIL`: Email de tu cuenta Unity
   - `UNITY_PASSWORD`: Contraseña de tu cuenta Unity

### 📖 Instrucciones Detalladas
Ver: **`GITHUB_ACTIONS_SETUP.md`** para guía completa paso a paso.

## 🏗️ Compilación Manual a EXE

### Para Compilar a Ejecutable:
1. **En Unity Editor**:
   - File > Build Settings
   - Add Open Scenes
   - Seleccionar plataforma destino
   - Player Settings:
     - Company Name: `xpe-hub`
     - Product Name: `Forest Warfare Shooter`
     - Version: `1.0.0`
   - Build > Seleccionar carpeta de destino

2. **El ejecutable generado incluirá**:
   - `Forest Warfare Shooter.exe` (archivo principal)
   - Carpeta `Forest Warfare Shooter_Data` (recursos del juego)

### Archivos Importantes
- **`StarRankingSystem_Enhanced.cs`**: Sistema principal con IA, rangos, mapas y personajes
- **`GameManager.cs`**: Controlador principal del juego
- **`OptimizedPlayerController_Updated.cs`**: Control del jugador optimizado
- **`AudioManager_updated.cs`**: Gestión de audio
- **`TeamController.cs`**: Sistema de equipos

## 🎨 Personajes IA Únicos

### Plantas vs Zombies Series
1. **Peashooter**: Especialista en armas de fuego
2. **Cherry Bomb**: Experto en explosivos
3. **Wall-nut**: Tanque defensivo
4. **Chomper**: Especialista en combate cuerpo a cuerpo

### Fall Guys Series
5. **Red Confetti**: Velocista agresivo
6. **Purple Party Animal**: Estratega táctico
7. **Yellow Star**: Especialista en precisión
8. **Blue Boomerang**: Experto en armas especiales

### Híbridos Únicos
9. **Forest Guardian**: Guardián del bosque con poderes naturales
10. **Zombie Commander**: Líder zombie con tácticas militares
11. **Berry Blaster**: Especialista en armas de energía
12. **Cactus Sharpshooter**: Francotirador del desierto

## 🗺️ Mapas Disponibles

### Bosques
- **Emerald Forest**: Bosque esmeralda con cascadas
- **Twisted Woods**: Bosque retorcido con niebla
- **Ancient Grove**: Bosque antiguo con árboles milenarios
- **Crystal Caverns**: Cuevas cristalinas subterráneas

### Desiertos
- **Sandy Barrens**: Desierto árido con dunas
- **Cactus Canyon**: Cañón de cactus con precipicios
- **Mirage Oasis**: Oasis con efectos de espejismo

### Urbano
- **Neon District**: Distrito neón futurista
- **Abandoned City**: Ciudad abandonada post-apocalíptica
- **Metro Station**: Estación de metro subterránea

## 🏆 Sistema de Rangos Detallado

### Rangos Esmeralda (Niveles 1-4)
- **Esmeralda I**: Rango inicial
- **Esmeralda II**: Veteran bosque
- **Esmeralda III**: Guardián esmeralda
- **Esmeralda IV**: Maestro esmeralda

### Rangos Patriarca (Niveles 5-10)
- **Patriarca I-VI**: Progresión avanzada con habilidades únicas

### Rangos Comando (Niveles 11-14)
- **Comando I-IV**: Elite máximo con recompensas especiales

## 🎮 Modos de Juego

### Modo Campaña
- **Historia lineal** con misiones progresivas
- **Introducción a rangos** y personajes IA
- **Tutorial interactivo** para nuevos jugadores

### Modo Arena
- **Combates 1v1** contra IA avanzada
- **Torneos eliminatorios** con premios especiales
- **Clasificaciones** por rango

### Modo Supervivencia
- **Hordas de enemigos** crecientes
- **Cooperative multiplayer** local
- **Boss battles** únicos

## 🔧 Desarrollo y Modificaciones

### Estructura del Proyecto
```
ForestWarfareShooter/
├── Assets/
│   ├── Scripts/          # Scripts C# principales
│   ├── Scenes/           # Escenas del juego
│   ├── Prefabs/          # Objetos predefinidos
│   ├── Materials/        # Materiales 3D
│   └── Textures/         # Texturas
├── ProjectSettings/      # Configuración Unity
└── Documentation/        # Guías y documentación
```

### Scripts Principales
- **`StarRankingSystem_Enhanced.cs`**: Sistema completo (2,422 líneas)
- **`GameManager.cs`**: Controlador principal
- **`OptimizedPlayerController_Updated.cs`**: Control del jugador
- **`AudioManager_updated.cs`**: Gestión de audio
- **`TeamController.cs`**: Sistema de equipos
- **`WeaponController.cs`**: Sistema de armas
- **`OptimizedForestGenerator.cs`**: Generación de bosques

## 🚀 Contribuciones

Las contribuciones son bienvenidas. Por favor:
1. Fork el proyecto
2. Crear una rama para tu feature
3. Commit tus cambios
4. Push a la rama
5. Abrir un Pull Request

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Ver `LICENSE` para más detalles.

## 👨‍💻 Desarrollador

**xpe-hub** - Desarrollo completo del sistema de IA, rangos, mapas y gameplay

---

**¡Disfruta jugando Forest Warfare Shooter! 🌲⚔️**