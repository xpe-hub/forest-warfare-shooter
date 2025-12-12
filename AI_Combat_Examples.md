# 🤖 Sistema de IA Avanzada - Forest Warfare Shooter

## Ejemplos de Combate con IA Diferentes Niveles

### 🎯 **AI Nivel 1 - Novato (Rookie)**
```
Soldado Rookie vs Jugador Humano:
- Precisión: 30% (tiros errados frecuentes)
- Tiempo de reacción: 2.5 segundos
- Estrategia: Defensiva, se esconde mucho
- Headshots: 15% de probabilidad
- Comportamiento: "¡Me dispararon! *se esconde*"
```

### ⚔️ **AI Nivel 2 - Veterano (Veteran)**
```
Veterano de Guerra vs Jugador Humano:
- Precisión: 65% (buena puntería)
- Tiempo de reacción: 1.8 segundos
- Estrategia: Táctica, usa coberturas
- Headshots: 35% de probabilidad
- Comportamiento: "Posición... disparar... ¡impacto!"
```

### 🎯 **AI Nivel 3 - Experto (Expert)**
```
Maestro del Battlefield vs Jugador Humano:
- Precisión: 90% (casi perfecta)
- Tiempo de reacción: 0.8 segundos
- Estrategia: Adaptativa, cambia tácticas
- Headshots: 60% de probabilidad
- Comportamiento: "Análisis completo... ¡ejecución!"
```

### 👑 **AI Nivel 4 - Élite (Elite)**
```
Shadow Assassin vs Jugador Humano:
- Precisión: 98% (casi imposible fallar)
- Tiempo de reacción: 0.4 segundos
- Estrategia: Sigilosa, aparece de la nada
- Headshots: 80% de probabilidad
- Comportamiento: *aparece detrás* "Shhh... ¡bang!"
```

### 💀 **AI Nivel 5 - Imposible (Impossible)**
```
The Void Walker vs Jugador Humano:
- Precisión: 99% (perfecta)
- Tiempo de reacción: 0.2 segundos
- Estrategia: Dimensional, movimientos imposibles
- Headshots: 95% de probabilidad
- Comportamiento: "El vacío me llama... ¡victoria!"
```

## 🎮 **Personalidades de IA Únicas**

### 😨 **Cautioso (Cautious)**
- Se esconde cuando tiene poca vida
- Usa coberturas siempre
- No hace pushes agresivos
- "Mejor esperar una oportunidad..."

### ⚡ **Agresivo (Aggressive)**
- Rush directo al enemigo
- Baja precisión pero mucho volumen
- "¡Vamos por ellos! ¡Sin miedo!"

### 🧠 **Táctico (Tactical)**
- Planifica cada movimiento
- Usa grenades inteligentes
- "Analizando campo de batalla..."

### 🕰️ **Paciente (Patient)**
- Espera el momento perfecto
- Sniper experto
- "El momento perfecto llegará..."

### 🌪️ **Tormenta (Storm)**
- Movimiento rápido y errático
- Dispara mientras se mueve
- "¡Soy la tormenta!"

### 🌌 **Cósmico (Cosmic)**
- Movimientos imposibles de predecir
- Efectos visuales únicos
- "Los secretos del cosmos..."

## 🎯 **Sistema de Headshots Inteligente**

### Probabilidades por Nivel:
- **Novato**: 15% - 25% headshot rate
- **Veterano**: 30% - 45% headshot rate  
- **Experto**: 50% - 70% headshot rate
- **Élite**: 75% - 85% headshot rate
- **Imposible**: 90% - 99% headshot rate

### Factores que Afectan Precisión:
```csharp
float finalAccuracy = baseAccuracy * 
    (distance < 10f ? 1.2f : 1.0f) *        // Distancia
    (isMoving ? 0.8f : 1.0f) *               // Movimiento
    (enemyMoving ? 0.9f : 1.0f) *            // Enemigo en movimiento
    (lowHealth ? 0.7f : 1.0f) *              // Salud baja
    (hasAdvantage ? 1.1f : 1.0f);            // Posición ventajosa
```

## 🧠 **Lógica de Decisión Avanzada**

### Evaluación de Amenazas:
```csharp
public float EvaluateThreat(Transform enemy)
{
    float distance = Vector3.Distance(transform.position, enemy.position);
    float enemyAccuracy = GetEnemyAccuracy();
    float ourHealth = currentHealth / maxHealth;
    
    return (enemyAccuracy * 0.4f) + 
           ((100 - distance) * 0.003f * 0.3f) + 
           (ourHealth * 0.3f);
}
```

### Selección de Estrategia:
```csharp
public AIStrategy SelectStrategy()
{
    var threats = EvaluateAllThreats();
    var advantages = EvaluateAllAdvantages();
    
    if (threats.Count > advantages.Count * 2)
        return AIStrategy.Defensive;
    else if (advantages.Count > threats.Count * 2)
        return AIStrategy.Aggressive;
    else
        return AIStrategy.Tactical;
}
```

## 🎭 **Sistema de Comunicación de IA**

### Frases por Personalidad:

**Cautioso:**
- "Veo movimiento... mejor esperar"
- "Demasiados enemigos, me retiro"
- "Esperando reinforcements"

**Agresivo:**
- "¡Vamos a por ellos!"
- "¡Sin piedad!"
- "¡El ataque es la mejor defensa!"

**Táctico:**
- "Planificando próximo movimiento"
- "Ventaja táctica obtenida"
- "Repositioning para optimal fire"

**Paciente:**
- "Esperando el momento perfecto"
- "Controlando la respiración..."
- "El arte del tiro preciso"

## 🎪 **Comportamientos Únicos por Mapa**

### Gasolinera Cósmica:
- AIs se teletransportan entre bombas
- "¡Combustible cósmico activado!"
- Efectos gravitacionales afectan movimiento

### Cementerio Ville:
- Zombies AIs se regeneran
- "¡Muerte es solo el comienzo!"
- Niebla afecta visibilidad

### Estación Espacial Sigma:
- AIs flotan en gravedad cero
- "¡El espacio es mi hogar!"
- Ventanas al espacio bloquean disparos

## 🏆 **Logros de IA Específicos**

- **"Domador de IA"** - Derrota 10 AIs Élite seguidas
- **"Sobreviviente Imposible"** - Sobrevive contra The Void Walker
- **"IA Whisperer"** - Entiende patrones de 20 AIs diferentes
- **"Speedrunner"** - Completa partida vs solo AIs Imposible en <5 minutos

## 💰 **Sistema de IA para Monetización**

### IA Pagos (Premium AI):
- **IA Legendaria** - $2.99/mes
- **IA Imposible** - $4.99/mes  
- **IA Personalizada** - $9.99/mes (crea tu propia IA)

### Características Premium:
- IA aprende de tu estilo de juego
- Personalidades únicas desbloqueables
- Estadísticas detalladas vs cada IA
- Modo entrenamiento personalizado

---

**🎮 ¡La IA de Forest Warfare Shooter llevará tu experiencia de combate a un nivel completamente nuevo!**