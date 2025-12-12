# 🌲 FOREST WARFARE - TACTICAL SHOOTER 3D

## 🎯 **DESCRIPCIÓN DEL PROYECTO**

**"Forest Warfare"** es un shooter táctico 3D ambientado en un bosque místico, donde dos escuadras (Roja y Azul) compiten por la conquista de banderas en combate estratégico.

### ⚔️ **MECÁNICAS PRINCIPALES**
- **Modo de Juego:** Conquista por Banderas (primer equipo en capturar 3 banderas gana)
- **Combate:** Shooter táctico en primera persona con 6 tipos de armas
- **Ambiente:** Bosque generado proceduralmente con efectos dinámicos
- **Personajes:** 4 clases únicas por equipo con equipamiento distintivo
- **Táctica:** Formaciones de escuadra, comandos de voz, comunicación táctica

---

## 🏗️ **CONFIGURACIÓN DEL PROYECTO UNITY**

### 📋 **REQUISITOS**
- **Unity 2022.3 LTS** o superior
- **Universal Render Pipeline (URP)** para gráficos modernos
- **Unity Netcode for GameObjects** para multiplayer
- **NavMesh** para AI de enemigos

### 🔧 **CONFIGURACIÓN INICIAL**

#### 1. **Crear Nuevo Proyecto**
```bash
# En Unity Hub
1. Crear nuevo proyecto 3D (URP)
2. Nombre: "ForestWarfare"
3. Ubicación: /tu/ruta/proyectos
```

#### 2. **Importar Scripts**
Copia todos los archivos de `Assets/Scripts/` a tu proyecto Unity:

```
Assets/
├── Scripts/
│   ├── Player/TacticalPlayerController.cs
│   ├── Teams/TeamController.cs
│   ├── Flags/FlagController.cs
│   ├── Combat/WeaponController.cs
│   ├── Environment/ForestGenerator.cs
│   └── GameManager/GameManager.cs
```

#### 3. **Configurar Multiplayer**
- Instalar **Unity Netcode for GameObjects**
- Configurar NetworkManager
- Setup de NetworkPrefabs

#### 4. **Crear Escenas**
- `MainMenu.unity` - Menú principal
- `ForestBattlefield.unity` - Mapa principal
- `GameSetup.unity` - Configuración de partida

---

## 🎮 **SISTEMAS IMPLEMENTADOS**

### ⚔️ **1. SISTEMA DE JUGADOR**
**Archivo:** `TacticalPlayerController.cs`

**Características:**
- ✅ Movimiento táctico (caminar, correr, agacharse, saltar)
- ✅ Sistema de puntería con mouse
- ✅ Salud y regeneración
- ✅ Muerte y respawn automático
- ✅ Transporte de banderas

**Controles:**
- **WASD:** Movimiento
- **Mouse:** Mirar alrededor
- **Shift:** Correr
- **Ctrl:** Agacharse
- **Espacio:** Saltar
- **Click Izq:** Disparar
- **Click Der:** Apuntar
- **R:** Recargar
- **Esc:** Pausar

### 🚩 **2. SISTEMA DE BANDERAS**
**Archivo:** `FlagController.cs`

**Mecánicas:**
- ✅ 5 banderas estratégicamente ubicadas
- ✅ Sistema de captura con tiempo
- ✅ Auto-retorno después de 10 segundos
- ✅ Efectos visuales y sonoros
- ✅ Contador de progreso de captura

**Lógica de Captura:**
- Un equipo debe estar solo en el radio de captura
- Tiempo de captura: 3 segundos
- Auto-retorno: 10 segundos

### ⚔️ **3. SISTEMA DE EQUIPOS**
**Archivo:** `TeamController.cs`

**Características:**
- ✅ Dos equipos: Rojo y Azul
- ✅ Spawn points automáticos
- ✅ Sistema de estadísticas
- ✅ Balanceo automático de equipos
- ✅ Comandos de voz entre compañeros

**Estadísticas por Equipo:**
- Banderas capturadas
- Jugadores eliminados
- Muertes del equipo

### 🔫 **4. SISTEMA DE ARMAS**
**Archivo:** `WeaponController.cs`

**Armas Implementadas:**
1. **Rifle de Asalto** - Combat estándar
2. **Rifle de Francotirador** - Largo alcance
3. **Subfusil** - Combate cercano
4. **Escopeta** - Daño masivo
5. **Pistola** - Arma secundaria
6. **Granadas** - Explosivas y humo

**Mecánicas:**
- ✅ Sistema de retroceso
- ✅ Apuntar con click derecho
- ✅ Recarga con tiempo
- ✅ Efectos de disparo
- ✅ Trail de balas

### 🌲 **5. GENERADOR DE BOSQUE**
**Archivo:** `ForestGenerator.cs`

**Características:**
- ✅ Generación procedural de árboles
- ✅ Rocas y arbustos aleatorios
- ✅ Cuerpos de agua animados
- ✅ Sistema de viento y efectos ambientales
- ✅ Iluminación dinámica con niebla

**Configuración:**
- Densidad de árboles: 10%
- Densidad de rocas: 5%
- Densidad de agua: 2%
- Altura de árboles: 5-15 unidades
- Escala de objetos: 0.8-1.5x

### 🎯 **6. MANAGER PRINCIPAL**
**Archivo:** `GameManager.cs`

**Estados del Juego:**
- **WaitingForPlayers:** Esperando mínimo 4 jugadores
- **StartingRound:** Cuenta regresiva de 5 segundos
- **InProgress:** Partida activa
- **Paused:** Juego pausado
- **Ended:** Fin de partida

**Configuración:**
- Máximo 16 jugadores
- 3 banderas para ganar
- Tiempo de ronda: 10 minutos
- Balanceo automático de equipos

---

## 🎨 **PERSONAJES LLAMATIVOS**

### 🔴 **EQUIPO ROJO**
1. **Soldado Élite** - Armadura completa, rifle de asalto
2. **Francotirador** - Rifle de precisión, ghillie suit
3. **Médico de Campo** - Equipo de curación, subfusil
4. **Comandante** - Armor especial, liderazgo táctico

### 🔵 **EQUIPO AZUL**
- **Mismas clases** con colores y equipamiento distintivos

### 🎭 **CARACTERÍSTICAS VISUALES**
- **Animaciones fluidas** para todos los movimientos
- **Efectos de partículas** en disparos y explosiones
- **Iluminación dinámica** con efectos de luz solar
- **UI futurista** con HUD táctico
- **Sonido espacial** con efectos ambientales

---

## 🔧 **CONFIGURACIÓN PASO A PASO**

### **PASO 1: Configurar Unity**
```csharp
// En Project Settings > Player
- Scripting Backend: IL2CPP
- API Compatibility Level: .NET 4.x

// En Quality Settings
- Set to "High" for best visuals
- Enable MSAA for anti-aliasing
```

### **PASO 2: Configurar NetworkManager**
```csharp
// Crear NetworkManager en la escena
- Add NetworkManager component
- Set Network Prefabs
- Configure Connection Approval
```

### **PASO 3: Crear Prefabs**
```csharp
// Player Prefab
- CharacterController
- NetworkIdentity
- TacticalPlayerController
- TeamController
- WeaponController
- Camera

// Flag Prefab
- NetworkIdentity
- FlagController
- Particle Effects
- Audio Source

// Weapon Prefabs
- Model 3D
- WeaponController
- Particle Systems
- Audio Sources
```

### **PASO 4: Configurar Escenas**
```csharp
// Main Menu Scene
- Canvas UI
- Button handlers
- NetworkManager setup

// Game Scene
- ForestGenerator
- GameManager
- Spawn points
- Flag positions
- Lighting setup
```

---

## 🎵 **SISTEMA DE AUDIO**

### **MÚSICA**
- **Tema Principal** - Ambientación de bosque
- **Música de Combate** - Tensión durante peleas
- **Música de Victoria** - Celebración de victoria
- **Música de Derrota** - Reflectiva para derrota

### **EFECTOS DE SONIDO**
- **Armas:** Disparos, recargas, impactos
- **Ambiente:** Viento, pájaros, agua
- **UI:** Clicks, notificaciones
- **Voz:** Comandos de equipo

---

## 🎮 **CONTROLES DEL JUEGO**

### **MOVIMIENTO**
- **WASD:** Mover personaje
- **Mouse:** Mirar alrededor
- **Shift:** Correr (más rápido)
- **Ctrl:** Agacharse (más sigiloso)
- **Espacio:** Saltar

### **COMBATE**
- **Click Izquierdo:** Disparar
- **Click Derecho:** Apuntar (reducir recoil)
- **R:** Recargar arma
- **1-5:** Cambiar armas

### **COMUNICACIÓN**
- **F1:** Comandar "Follow Me"
- **F2:** Comandar "Hold Position"
- **F3:** Comandar "Attack"
- **F4:** Comandar "Defend"

### **INTERFAZ**
- **Tab:** Mostrar marcador
- **M:** Minimapa
- **Esc:** Menú de pausa

---

## 🏆 **MODO DE CONQUISTA**

### **OBJETIVO**
- Capturar y mantener 3 banderas
- Cada bandera capturada vale 1 punto
- Primera escuadra en llegar a 3 puntos gana

### **ESTRATEGIA**
- **Defensa:** Mantener banderas capturadas
- **Ataque:** Capturar banderas enemigas
- **Táctica:** Coordinación de escuadra
- **Comunicación:** Comandos de voz

### **MAPA**
```
    [NW Flag]    [N Flag]    [NE Flag]
        |           |           |
        |           |           |
[SW Flag]-----[CENTER]-----[SE Flag]
        |           |           |
        |           |           |
    [S Flag]    [Flag]    [Flag]
```

---

## 🔧 **SOLUCIÓN DE PROBLEMAS**

### **ERRORES COMUNES**

#### **1. Scripts no compilan**
```csharp
// Verificar namespaces
using TacticalShooter.Player;
using TacticalShooter.Teams;
using TacticalShooter.Flags;
```

#### **2. Network no funciona**
```csharp
// Verificar NetworkManager setup
// Comprobar Network Prefabs
// Revisar Connection Approval
```

#### **3. Performance issues**
```csharp
// Reducir densidad en ForestGenerator
// Optimizar texturas
// Usar Level of Detail (LOD)
```

---

## 📱 **FUTURAS MEJORAS**

### **PLANIFICADO**
- 🎮 **Más mapas:** Río, Montaña, Ruinas
- 🤖 **AI Enemigos:** Bots cuando faltan jugadores
- 🏆 **Rankings:** Sistema de progreso del jugador
- 🎨 **Customización:** Unlock skins y accesorios
- 📱 **Mobile:** Versión para dispositivos móviles

### **CARACTERÍSTICAS AVANZADAS**
- 🌦️ **Weather System:** Lluvia, niebla dinámica
- 🔊 **Voice Chat:** Comunicación de voz real
- 📊 **Analytics:** Estadísticas detalladas
- 🎪 **Events:** Eventos especiales y torneos

---

## 💡 **TIPS PARA DESARROLLO**

### **OPTIMIZACIÓN**
- Usar **Object Pooling** para bullets
- Implementar **Occlusion Culling** para bosques
- Usar **Texture Atlasing** para UI
- Configurar **Quality Settings** apropiadamente

### **TESTING**
- Probar con **4+ jugadores** siempre
- Verificar **Network Synchronization**
- Testear **Performance** en diferentes dispositivos
- Validar **Balance** de armas y equipos

---

## 🎯 **CONCLUSIÓN**

Este proyecto de **Forest Warfare** es un shooter táctico completo y moderno que combina:

✅ **Gameplay estratégico** con conquista de banderas  
✅ **Gráficos impresionantes** con generación procedural  
✅ **Multiplayer robusto** con UNet  
✅ **Audio inmersivo** con efectos ambientales  
✅ **Código modular** y bien documentado  

**¡Perfecto para crear un juego competitivo y divertido!** 🎮🔥

---

## 📞 **SOPORTE**

Para preguntas técnicas o sugerencias de mejora, el código está completamente documentado y estructurado para fácil modificación y expansión.

**¡Que disfrutes creando tu shooter épico en el bosque!** 🌲⚔️🎮