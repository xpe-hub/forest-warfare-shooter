/*
 * Forest Warfare Shooter
 * Copyright © 2025 xpe.nettt 👑
 * StarRankingSystem.cs - Sistema de estrellas y ranking competitivo
 * Gamificación completa con progresión y recompensas
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace TacticalShooter.Progression
{
    public class StarRankingSystem : MonoBehaviour
    {
        [Header("Star System Configuration")]
        public int maxStarsPerMatch = 5;
        public int baseStarReward = 1;
        public float starMultiplierPerDifficulty = 1.2f;
        
        [Header("Ranking Tiers")]
        public RankingTier[] rankingTiers;
        public int starsPerTier = 25; // Increased for better progression
        
        [Header("Achievements")]
        public Achievement[] achievements;
        public bool enableAchievements = true;
        
        [Header("Progression Rewards")]
        public ProgressionReward[] progressionRewards;
        public bool enableProgressionRewards = true;
        
        [Header("World Open Maps (Battle for Neighborville Style)")]
        public WorldMap[] worldOpenMaps;
        public bool enableWorldOpenMaps = true;
        
        [Header("Mission System")]
        public Mission[] dailyMissions;
        public Mission[] weeklyMissions;
        public Mission[] seasonalMissions;
        public bool enableMissionSystem = true;
        
        [Header("Enhanced Ranking with Subdivisions")]
        public RankingSubdivision[] rankingSubdivisions;
        public bool enableSubdivisions = true;
        
        [Header("Advanced AI System")]
        public AdvancedAI[] aiCharacters;
        public bool enableAdvancedAI = true;
        public int maxAIPerMatch = 12;
        
        [Header("Simple Authentication")]
        public SimpleAuthSystem authSystem;
        public bool enableSimpleAuth = true;
        
        [Header("Game Configuration")]
        public GameConfiguration gameConfig;
        public bool enableGameConfig = true;
        
        [Header("Seasonal System")]
        public bool enableSeasonalRanking = true;
        public string currentSeason = "Temporada_Inicial";
        public int seasonNumber = 1;
        
        // Current player data
        private PlayerProgressionData playerData;
        
        // Star earning tracking
        private int starsEarnedThisMatch = 0;
        private int totalMatches = 0;
        private int currentWinStreak = 0;
        private int longestWinStreak = 0;
        
        // Enhanced performance tracking
        private Dictionary<string, int> performanceMetrics = new Dictionary<string, int>();
        private Dictionary<string, float> performanceRatios = new Dictionary<string, float>();
        private List<MatchResult> recentMatches = new List<MatchResult>();
        
        // Achievement tracking
        private Dictionary<string, bool> unlockedAchievements = new Dictionary<string, bool>();
        private List<Achievement> pendingAchievements = new List<Achievement>();
        
        // Enhanced event system
        public System.Action<int> OnStarsEarned;
        public System.Action<RankingTier> OnTierUp;
        public System.Action<Achievement> OnAchievementUnlocked;
        public System.Action<ProgressionReward> OnRewardClaimed;
        public System.Action<SeasonalProgress> OnSeasonalUpdate;

        void Start()
        {
            InitializeStarRankingSystem();
        }

        void InitializeStarRankingSystem()
        {
            LoadPlayerData();
            InitializeEnhancedRankingTiers();
            InitializeAchievements();
            InitializeProgressionRewards();
            InitializeWorldOpenMaps();
            InitializeRankingSubdivisions();
            InitializeMissionSystem();
            InitializeAdvancedAI();
            InitializeSimpleAuth();
            InitializeGameConfiguration();
            InitializeSeasonalSystem();
            
            Debug.Log("Enhanced Star Ranking System initialized");
        }

        void LoadPlayerData()
        {
            // Load player progression data from PlayerPrefs
            playerData = new PlayerProgressionData
            {
                totalStars = PlayerPrefs.GetInt("TotalStars", 0),
                currentTier = PlayerPrefs.GetInt("CurrentTier", 1),
                totalMatches = PlayerPrefs.GetInt("TotalMatches", 0),
                totalWins = PlayerPrefs.GetInt("TotalWins", 0),
                currentWinStreak = PlayerPrefs.GetInt("CurrentWinStreak", 0),
                longestWinStreak = PlayerPrefs.GetInt("LongestWinStreak", 0),
                totalPlayTime = PlayerPrefs.GetFloat("TotalPlayTime", 0f),
                favoriteGameMode = PlayerPrefs.GetString("FavoriteGameMode", "Conquest"),
                seasonalStars = PlayerPrefs.GetInt($"SeasonalStars_{currentSeason}", 0),
                lifetimeStars = PlayerPrefs.GetInt("LifetimeStars", 0)
            };
            
            // Load unlocked achievements
            string unlockedAchievementsJson = PlayerPrefs.GetString("UnlockedAchievements", "");
            if (!string.IsNullOrEmpty(unlockedAchievementsJson))
            {
                unlockedAchievements = JsonUtility.FromJson<Dictionary<string, bool>>(unlockedAchievementsJson);
            }
            
            // Load recent matches
            LoadRecentMatches();
        }

        void InitializeEnhancedRankingTiers()
        {
            rankingTiers = new RankingTier[]
            {
                // Tier 1: Semilla Verde - Verde esmeralda vibrante
                new RankingTier { tierName = "Semilla Verde", minStars = 0, maxStars = 24, tierColor = new Color(0.0f, 0.8f, 0.4f), badgeIcon = "semilla_verde_badge", tierDescription = "Eres una promesa en crecimiento" },
                
                // Tier 2: Cazador de Espejismos - Azul zafiro
                new RankingTier { tierName = "Cazador de Espejismos", minStars = 25, maxStars = 74, tierColor = new Color(0.2f, 0.6f, 1.0f), badgeIcon = "espejismos_badge", tierDescription = "Persigues objetivos que otros no ven" },
                
                // Tier 3: Forjador de Almas - Rojo carmesí
                new RankingTier { tierName = "Forjador de Almas", minStars = 75, maxStars = 149, tierColor = new Color(1.0f, 0.3f, 0.3f), badgeIcon = "almas_badge", tierDescription = "Das forma al destino con cada disparo" },
                
                // Tier 4: Tejedora de Destinos - Púrpura real
                new RankingTier { tierName = "Tejedora de Destinos", minStars = 150, maxStars = 249, tierColor = new Color(0.6f, 0.2f, 0.8f), badgeIcon = "destinos_badge", tierDescription = "Controlas el hilo de la batalla" },
                
                // Tier 5: ESMERALDA - Dorado Radiante (Nuevo rango principal)
                new RankingTier { tierName = "ESMERALDA", minStars = 250, maxStars = 374, tierColor = new Color(0.0f, 1.0f, 0.5f), badgeIcon = "esmeralda_badge", tierDescription = "Eres una fuerza imparable de la naturaleza" },
                
                // Tier 6: PATRIARCA - Platino Puro (Nuevo rango principal)
                new RankingTier { tierName = "PATRIARCA", minStars = 375, maxStars = 524, tierColor = new Color(0.9f, 0.9f, 0.9f), badgeIcon = "patriarca_badge", tierDescription = "Los cuervos anuncian tu llegada" },
                
                // Tier 7: COMANDO - Rubí Sangriento (Nuevo rango principal)
                new RankingTier { tierName = "COMANDO", minStars = 525, maxStars = 699, tierColor = new Color(0.8f, 0.1f, 0.1f), badgeIcon = "comando_badge", tierDescription = "Tu eco resuena por el bosque" },
                
                // Tier 8: Guardiana del Portal - Esmeralda Cósmica
                new RankingTier { tierName = "Guardiana del Portal", minStars = 700, maxStars = 999, tierColor = new Color(0.0f, 1.0f, 0.5f), badgeIcon = "portal_badge", tierDescription = "Vigilas los umbrales entre mundos" },
                
                // Tier 9: Fragmento de Estrella - Diamante Estelar
                new RankingTier { tierName = "Fragmento de Estrella", minStars = 1000, maxStars = 1499, tierColor = new Color(0.7f, 0.9f, 1.0f), badgeIcon = "estrella_badge", tierDescription = "Un pedazo del cosmos camina entre nosotros" },
                
                // Tier 10: Ser de Luz Eternal - Obsidiana Divine
                new RankingTier { tierName = "Ser de Luz Eternal", minStars = 1500, maxStars = 9999, tierColor = new Color(0.1f, 0.1f, 0.1f), badgeIcon = "luz_eternal_badge", tierDescription = "Has transcendido la existencia mortal" }
            };
        }

        void InitializeAchievements()
        {
            achievements = new Achievement[]
            {
                // === MATCH ACHIEVEMENTS ===
                new Achievement { id = "first_match", name = "Despertar Verde", description = "Tu primera conexión con la energía del bosque", icon = "awakening", rewardStars = 5 },
                new Achievement { id = "ten_matches", name = "Eco Perdurable", description = "Sobrevive a 10 batallas, tu eco resuena", icon = "echo", rewardStars = 15 },
                new Achievement { id = "fifty_matches", name = "Forjador de Recuerdos", description = "Lucha en 50 enfrentamientos, forjas tu leyenda", icon = "memories", rewardStars = 30 },
                new Achievement { id = "hundred_matches", name = "Tejedor de Historias", description = "Participa en 100 misiones, tu historia se extiende", icon = "stories", rewardStars = 50 },
                
                // === VICTORY ACHIEVEMENTS ===
                new Achievement { id = "first_win", name = "Primera Llamada", description = "Responde tu primera llamada a la victoria", icon = "call", rewardStars = 10 },
                new Achievement { id = "five_wins", name = "Danzante de Espejismos", description = "Baila entre 5 victorias como un espejismo", icon = "dancer", rewardStars = 20 },
                new Achievement { id = "ten_wins", name = "Guardiana de Fragmentos", description = "Conserva 10 fragmentos de victoria", icon = "guardian", rewardStars = 35 },
                new Achievement { id = "twenty_five_wins", name = "Maestra de Umbrales", description = "Cruza 25 umbrales hacia la victoria", icon = "threshold", rewardStars = 60 },
                new Achievement { id = "fifty_wins", name = "Ser de Luz Naciente", description = "Tu luz comienza a brillar con 50 victorias", icon = "nascent", rewardStars = 100 },
                
                // === STREAK ACHIEVEMENTS ===
                new Achievement { id = "three_streak", name = "Racha Ardiente", description = "3 victorias consecutivas", icon = "hot_streak", rewardStars = 20 },
                new Achievement { id = "five_streak", name = "Inexorable", description = "5 victorias sin parar", icon = "relentless", rewardStars = 40 },
                new Achievement { id = "ten_streak", name = "Tormenta Imparable", description = "10 victorias seguidas", icon = "storm", rewardStars = 80 },
                new Achievement { id = "twenty_streak", name = "Leyenda Viviente", description = "20 victorias consecutivas", icon = "living_legend", rewardStars = 150 },
                
                // === PERFORMANCE ACHIEVEMENTS ===
                new Achievement { id = "perfect_accuracy", name = "Ojo de Halcón", description = "Precisión perfecta en un combate", icon = "hawk_eye", rewardStars = 25 },
                new Achievement { id = "headshot_master", name = "Maestro del Tiro", description = "50 disparos a la cabeza", icon = "headshot_master", rewardStars = 35 },
                new Achievement { id = "elimination_expert", name = "Experto en Aniquilación", description = "Elimina 100 enemigos", icon = "elimination_expert", rewardStars = 45 },
                new Achievement { id = "survival_expert", name = "Superviviente Nato", description = "Sobrevive 1000 minutos en combate", icon = "survival_expert", rewardStars = 40 },
                
                // === SPECIAL ACHIEVEMENTS ===
                new Achievement { id = "comeback_victory", name = "Contra Todo Pronóstico", description = "Gana después de estar perdiendo", icon = "comeback", rewardStars = 30 },
                new Achievement { id = "clutch_performance", name = "Momento de Gloria", description = "Gana con 1 HP restante", icon = "clutch", rewardStars = 35 },
                new Achievement { id = "pentakill", name = "Matanza Perfecta", description = "Elimina a 5 enemigos en una sola ronda", icon = "pentakill", rewardStars = 75 },
                new Achievement { id = "flawless_victory", name = "Perfección Absoluta", description = "Victoria sin recibir daño", icon = "flawless", rewardStars = 60 },
                
                // === TEAM ACHIEVEMENTS ===
                new Achievement { id = "team_player", name = "Espíritu de Equipo", description = "100 asistencias a compañeros", icon = "team_player", rewardStars = 25 },
                new Achievement { id = "flag_captain", name = "Capitán de la Bandera", description = "Captura 25 banderas", icon = "flag_captain", rewardStars = 40 },
                new Achievement { id = "tactical_leader", name = "Líder Táctico", description = "Coordina 50 victorias de equipo", icon = "tactical_leader", rewardStars = 50 },
                
                // === TIME ACHIEVEMENTS ===
                new Achievement { id = "early_bird", name = "Ave Madrugadora", description = "Juega antes de las 6 AM", icon = "early_bird", rewardStars = 15 },
                new Achievement { id = "night_owl", name = "Búho Nocturno", description = "Juega después de medianoche", icon = "night_owl", rewardStars = 15 },
                new Achievement { id = "marathon_player", name = "Maratonista", description = "Juega durante 2 horas seguidas", icon = "marathon", rewardStars = 30 },
                
                // === RARE ACHIEVEMENTS ===
                new Achievement { id = "speed_demon", name = "Demonio de la Velocidad", description = "Victoria en menos de 2 minutos", icon = "speed_demon", rewardStars = 50 },
                new Achievement { id = "last_standing", name = "Último en Pie", description = "Victoria siendo el último surviviente del equipo", icon = "last_standing", rewardStars = 45 },
                new Achievement { id = "no_scope", name = "Sin Mirar", description = "Victoria sin usar mira", icon = "no_scope", rewardStars = 40 },
                
                // === MAP-SPECIFIC ACHIEVEMENTS ===
                new Achievement { id = "gas_station_regular", name = "Cliente Frecuente", description = "Visita la Estación de Gas Cósmica 50 veces", icon = "gas_customer", rewardStars = 60 },
                new Achievement { id = "gnome_whisperer", name = "Susurrador de Gnomos", description = "Interactúa con todos los gnomos en Garden Gnomes Park", icon = "gnome_friend", rewardStars = 35 },
                new Achievement { id = "backyard_master", name = "Maestro del Patio", description = "Consigue 100 victorias en Backyard Battleground", icon = "backyard_king", rewardStars = 75 },
                new Achievement { id = "zombie_exterminator", name = "Exterminador de Zombies", description = "Elimina 1000 zombies en Chomp Town", icon = "zombie_hunter", rewardStars = 100 },
                
                // === ESMERALDA TIER ACHIEVEMENTS ===
                new Achievement { id = "first_esmeralda", name = "Nacimiento Esmeralda", description = "Alcanza el rango ESMERALDA por primera vez", icon = "esmeralda_birth", rewardStars = 100 },
                new Achievement { id = "esmeralda_master", name = "Maestro Esmeralda", description = "Completa todas las subdivisiones de ESMERALDA", icon = "esmeralda_master", rewardStars = 150 },
                
                // === PATRIARCA TIER ACHIEVEMENTS ===
                new Achievement { id = "patriarch_wisdom", name = "Sabiduría Patriarcal", description = "Alcanza el rango PATRIARCA III", icon = "patriarch_crown", rewardStars = 200 },
                new Achievement { id = "crow_whisperer", name = "Susurrador de Cuervos", description = "Sobrevive 10 partidas con el título 'Sombrero de Cuervos'", icon = "crow_friend", rewardStars = 80 },
                
                // === COMANDO TIER ACHIEVEMENTS ===
                new Achievement { id = "command_supreme", name = "Comando Supremo", description = "Alcanza el rango COMANDO III", icon = "command_badge", rewardStars = 250 },
                new Achievement { id = "thunder_voice", name = "Voz del Thunder", description = "Consigue 25 eliminaciones con el emote 'Eco de Thunder'", icon = "thunder_voice", rewardStars = 120 },
                
                // === COLLECTOR ACHIEVEMENTS ===
                new Achievement { id = "map_collector", name = "Coleccionista de Mundos", description = "Juega en todos los mapas disponibles", icon = "world_collector", rewardStars = 300 },
                new Achievement { id = "mission_master", name = "Maestro de Misiones", description = "Completa 100 misiones diarias", icon = "mission_ace", rewardStars = 180 },
                new Achievement { id = "skin_collector", name = "Coleccionista de Skins", description = "Desbloquea 50 skins diferentes", icon = "skin_hunter", rewardStars = 150 },
                
                // === SOCIAL ACHIEVEMENTS ===
                new Achievement { id = "team_captain", name = "Capitán de Equipo", description = "Lidera 25 victorias de equipo", icon = "team_leader", rewardStars = 100 },
                new Achievement { id = "mentor", name = "Mentor", description = "Ayuda a 10 jugadores nuevos a mejorar", icon = "mentor_badge", rewardStars = 75 },
                new Achievement { id = "social_butterfly", name = "Mariposa Social", description = "Juega con 50 jugadores diferentes", icon = "social_butterfly", rewardStars = 90 },
                
                // === SPECIAL EVENT ACHIEVEMENTS ===
                new Achievement { id = "holiday_warrior", name = "Guerrero Festivo", description = "Consigue 20 victorias durante eventos especiales", icon = "holiday_spirit", rewardStars = 110 },
                new Achievement { id = "midnight_raider", name = "Saqueador de Medianoche", description = "Juega entre 12 AM y 6 AM por 30 días consecutivos", icon = "midnight_raider", rewardStars = 130 },
                
                // === EXTREME ACHIEVEMENTS ===
                new Achievement { id = "impossible_shot", name = "Tiro Imposible", description = "Consigue una eliminación a más de 200 metros", icon = "impossible_shot", rewardStars = 200 },
                new Achievement { id = "flawless_month", name = "Mes Perfecto", description = "Mantén un K/D ratio superior a 5.0 durante un mes completo", icon = "flawless_streak", rewardStars = 500 },
                new Achievement { id = "legendary_performance", name = "Rendimiento Legendario", description = "Consigue 50 kills, 0 deaths en una sola partida", icon = "legendary_play", rewardStars = 400 },
                
                // === SECRET ACHIEVEMENTS ===
                new Achievement { id = "easter_egg_hunter", name = "Cazador de Huevos de Pascua", description = "Encuentra todos los secretos ocultos en los mapas", icon = "secret_finder", rewardStars = 250 },
                new Achievement { id = "developer_friend", name = "Amigo del Desarrollador", description = "Juega en todas las versiones beta del juego", icon = "beta_tester", rewardStars = 300 },
                new Achievement { id = "community_hero", name = "Héroe de la Comunidad", description = "Ayuda a resolver 100 bugs reportados por la comunidad", icon = "community_hero", rewardStars = 350 }
            };
            
            // Initialize achievement tracking
            foreach (Achievement achievement in achievements)
            {
                unlockedAchievements[achievement.id] = false;
            }
        }

        void InitializeProgressionRewards()
        {
            progressionRewards = new ProgressionReward[]
            {
                // Early game rewards
                new ProgressionReward { level = 5, name = "Hoja Verde Emergente", description = "Desbloquea la piel de hoja verde para tu arma", type = RewardType.WeaponSkin, value = "green_leaf" },
                new ProgressionReward { level = 10, name = "Danza de Espejismos", description = "Nuevo emote de danza entre mundos", type = RewardType.Emote, value = "mirage_dance" },
                new ProgressionReward { level = 15, name = "Manto de Cazador", description = "Manto azul zafiro del cazador de espejismos", type = RewardType.CharacterSkin, value = "hunter_cloak" },
                
                // Mid game rewards
                new ProgressionReward { level = 20, name = "Forja del Alma", description = "Armadura roja carmesí forjada en almas", type = RewardType.CharacterSkin, value = "soul_forge" },
                new ProgressionReward { level = 25, name = "Bosque de Susurros", description = "Mapa exclusivo del Bosque de Susurros", type = RewardType.Map, value = "whisper_forest" },
                new ProgressionReward { level = 30, name = "Hiladora de Destinos", description = "Arma teñida de púrpura real del destino", type = RewardType.WeaponSkin, value = "fate_spinner" },
                new ProgressionReward { level = 35, name = "Emote del Eco", description = "Emote que resuena como el eco de thunder", type = RewardType.Emote, value = "thunder_echo" },
                
                // Late game rewards
                new ProgressionReward { level = 40, name = "Título ESMERALDA", description = "Título 'Fuerza Imparable'", type = RewardType.Title, value = "Emerald_Force" },
                new ProgressionReward { level = 42, name = "ESMERALDA I Badge", description = "Primera chispa de poder esmeralda", type = RewardType.Accessory, value = "esmeralda_1_badge" },
                new ProgressionReward { level = 44, name = "ESMERALDA II Badge", description = "Poder esmeralda en crecimiento", type = RewardType.Accessory, value = "esmeralda_2_badge" },
                new ProgressionReward { level = 46, name = "ESMERALDA III Badge", description = "Maestría esmeralda completa", type = RewardType.Accessory, value = "esmeralda_3_badge" },
                new ProgressionReward { level = 48, name = "Corona de Cuervos", description = "Corona ornamentada con plumas de cuervo", type = RewardType.CharacterSkin, value = "crow_crown" },
                new ProgressionReward { level = 50, name = "Portal de Thunder", description = "Mapa exclusivo Portal de Thunder", type = RewardType.Map, value = "thunder_portal" },
                new ProgressionReward { level = 52, name = "Título PATRIARCA", description = "Título 'Líder Ancestral'", type = RewardType.Title, value = "Patriarch_Leader" },
                new ProgressionReward { level = 54, name = "PATRIARCA I Badge", description = "Primer grado de liderazgo ancestral", type = RewardType.Accessory, value = "patriarca_1_badge" },
                new ProgressionReward { level = 55, name = "Pluma del Eco", description = "Pluma que captura ecos de batalla", type = RewardType.Accessory, value = "echo_feather" },
                new ProgressionReward { level = 57, name = "PATRIARCA II Badge", description = "Sabiduría patriarcal en desarrollo", type = RewardType.Accessory, value = "patriarca_2_badge" },
                new ProgressionReward { level = 59, name = "PATRIARCA III Badge", description = "Patriarca supremo de los cuervos", type = RewardType.Accessory, value = "patriarca_3_badge" },
                
                // End game rewards
                new ProgressionReward { level = 60, name = "Título Guardiana del Portal", description = "Título 'Vigilante de Umbrales'", type = RewardType.Title, value = "Portal_Guardian" },
                new ProgressionReward { level = 62, name = "Título COMANDO", description = "Título 'Maestro del Thunder'", type = RewardType.Title, value = "Command_Master" },
                new ProgressionReward { level = 64, name = "COMANDO I Badge", description = "Primer nivel de comando militar", type = RewardType.Accessory, value = "comando_1_badge" },
                new ProgressionReward { level = 65, name = "Armadura Cósmica", description = "Armadura esmeralda cósmica de guardiana", type = RewardType.CharacterSkin, value = "cosmic_armor" },
                new ProgressionReward { level = 67, name = "COMANDO II Badge", description = "Maestría en tácticas de comando", type = RewardType.Accessory, value = "comando_2_badge" },
                new ProgressionReward { level = 69, name = "COMANDO III Badge", description = "Comando supremo del thunder", type = RewardType.Accessory, value = "comando_3_badge" },
                new ProgressionReward { level = 70, name = "Mapa Reino de Fragmentos", description = "Mapa exclusivo Reino de Fragmentos Estelares", type = RewardType.Map, value = "star_fragment_realm" },
                new ProgressionReward { level = 75, name = "Título Fragmento de Estrella", description = "Título 'Pedazo de Cosmos'", type = RewardType.Title, value = "Star_Fragment" },
                new ProgressionReward { level = 80, name = "Aura de Luz Eternal", description = "Aura que irradia luz eterna", type = RewardType.Emote, value = "eternal_light" },
                new ProgressionReward { level = 85, name = "Arma de Luz Naciente", description = "Arma forjada en luz estelar", type = RewardType.WeaponSkin, value = "nascent_light" },
                new ProgressionReward { level = 90, "Título Ser de Luz Eternal", description = "Título 'Luz que Trasciende'", type = RewardType.Title, value = "Eternal_Light_Being" },
                new ProgressionReward { level = 95, name = "Corona de Luz Suprema", description = "Corona que irradia la luz más pura", type = RewardType.CharacterSkin, value = "supreme_light_crown" },
                new ProgressionReward { level = 100, name = "Título Luz Transcendida", description = "Título 'Luz que ha Alcanzado la Eternidad'", type = RewardType.Title, value = "Transcended_Light" }
            };
        }

        void InitializeWorldOpenMaps()
        {
            worldOpenMaps = new WorldMap[]
            {
                // Neighborhood Central - Inspirado en Battle for Neighborville
                new WorldMap { 
                    mapId = "neighborhood_central", 
                    mapName = "Vecindario Central", 
                    description = "El corazón del vecindario donde comenzó todo",
                    isOpenWorld = true, 
                    minTierRequired = 1, 
                    maxPlayers = 32, 
                    gameModes = new string[] { "Conquest", "King of Hill", "Team Deathmatch", "Capture the Flag" },
                    specialFeatures = new string[] { "Zonas Dinámicas", "Eventos Aleatorios", "NPCs Ambientales" }
                },
                
                // Garden Gnomes Park - Clásico de BFN
                new WorldMap { 
                    mapId = "garden_gnomes_park", 
                    mapName = "Parque de Gnomos de Jardín", 
                    description = "Un parque colorido lleno de gnomos de jardín gigantes",
                    isOpenWorld = true, 
                    minTierRequired = 1, 
                    maxPlayers = 32, 
                    gameModes = new string[] { "Conquest", "King of Hill", "Team Deathmatch", "Garden Defense" },
                    specialFeatures = new string[] { "Gnomos Interactivos", "Efectos Florales", "Cañones de Agua" }
                },
                
                // Suburbia Gardens - Mundo abierto con jardines
                new WorldMap { 
                    mapId = "suburbia_gardens", 
                    mapName = "Jardines de Suburbia", 
                    description = "Vastas áreas verdes con casas decorativas",
                    isOpenWorld = true, 
                    minTierRequired = 3, 
                    maxPlayers = 32, 
                    gameModes = new string[] { "Conquest", "King of Hill", "Plant vs Zombie Mode" },
                    specialFeatures = new string[] { "Jardines Interactivos", "Zombies Ocultos", "Power-ups Naturales" }
                },
                
                // Chomp Town - Mapa icónico de BFN
                new WorldMap { 
                    mapId = "chomp_town", 
                    mapName = "Ciudad Chomp", 
                    description = "Una ciudad donde los zombies campan a sus anchas",
                    isOpenWorld = true, 
                    minTierRequired = 4, 
                    maxPlayers = 32, 
                    gameModes = new string[] { "Conquest", "King of Hill", "Zombie Survival", "Capture the Flag" },
                    specialFeatures = new string[] { "Zombies Inteligentes", "Mecánica de Comer Cerebros", "Spawns Dinámicos" }
                },
                
                // Downtown District - Centro urbano
                new WorldMap { 
                    mapId = "downtown_district", 
                    mapName = "Distrito Downtown", 
                    description = "Rascacielos y calles bulliciosas del centro",
                    isOpenWorld = true, 
                    minTierRequired = 5, 
                    maxPlayers = 32, 
                    gameModes = new string[] { "Conquest", "King of Hill", "Team Deathmatch", "Bomb Defusal" },
                    specialFeatures = new string[] { "Techos Accesibles", "Zonas Comerciales", "Transporte Público" }
                },
                
                // Industrial Park - Zona industrial
                new WorldMap { 
                    mapId = "industrial_park", 
                    mapName = "Parque Industrial", 
                    description = "Fábricas, almacenes y áreas de carga",
                    isOpenWorld = true, 
                    minTierRequired = 7, 
                    maxPlayers = 32, 
                    gameModes = new string[] { "Conquest", "King of Hill", "Elimination", "Payload" },
                    specialFeatures = new string[] { "Maquinaria Peligrosa", "Zonas de Riesgo", "Efectos Ambientales" }
                },
                
                // University Campus - Campus universitario
                new WorldMap { 
                    mapId = "university_campus", 
                    mapName = "Campus Universitario", 
                    description = "Aulas, laboratorios y campos deportivos",
                    isOpenWorld = true, 
                    minTierRequired = 9, 
                    maxPlayers = 32, 
                    gameModes = new string[] { "Conquest", "King of Hill", "Team Deathmatch", "Capture the Flag" },
                    specialFeatures = new string[] { "Laboratorios Especiales", "Biblioteca Secreta", "Estadio Deportivo" }
                },
                
                // Harbor Docks - Puerto y muelles
                new WorldMap { 
                    mapId = "harbor_docks", 
                    mapName = "Muelles del Puerto", 
                    description = "Buques, grúas y áreas portuarias",
                    isOpenWorld = true, 
                    minTierRequired = 11, 
                    maxPlayers = 32, 
                    gameModes = new string[] { "Conquest", "King of Hill", "Bomb Defusal", "Payload" },
                    specialFeatures = new string[] { "Barcos Movibles", "Grúas Interactivas", "Carga Peligrosa" }
                },
                
                // Mountain Retreat - Refugio montañoso
                new WorldMap { 
                    mapId = "mountain_retreat", 
                    mapName = "Retiro Montañoso", 
                    description = "Cabañas, senderos y vistas panorámicas",
                    isOpenWorld = true, 
                    minTierRequired = 13, 
                    maxPlayers = 32, 
                    gameModes = new string[] { "Conquest", "King of Hill", "Survival", "Capture the Flag" },
                    specialFeatures = new string[] { "Efectos Climáticos", "Fauna Salvaje", "Pistas de Esquí" }
                },
                
                // Crystal Caverns - Cavernas de cristal
                new WorldMap { 
                    mapId = "crystal_caverns", 
                    mapName = "Cavernas de Cristal", 
                    description = "Gemas brillantes y pasajes subterráneos",
                    isOpenWorld = true, 
                    minTierRequired = 15, 
                    maxPlayers = 32, 
                    gameModes = new string[] { "Conquest", "Elimination", "King of Hill", "Team Deathmatch" },
                    specialFeatures = new string[] { "Cristales Energéticos", "Pasajes Secretos", "Efectos Mágicos" }
                },
                
                // Sky Temple - Templo celestial
                new WorldMap { 
                    mapId = "sky_temple", 
                    mapName = "Templo Celestial", 
                    description = "Plataformas flotantes y templos antiguos",
                    isOpenWorld = true, 
                    minTierRequired = 17, 
                    maxPlayers = 32, 
                    gameModes = new string[] { "Conquest", "King of Hill", "Elimination", "Capture the Flag" },
                    specialFeatures = new string[] { "Gravedad Alterada", "Teleporteres", "Guardianes Divinos" }
                },
                
                // Mirror Dimension - Dimensión espejo
                new WorldMap { 
                    mapId = "mirror_dimension", 
                    mapName = "Dimensión Espejo", 
                    description = "Un mundo invertido donde todo es diferente",
                    isOpenWorld = true, 
                    minTierRequired = 19, 
                    maxPlayers = 32, 
                    gameModes = new string[] { "Conquest", "King of Hill", "Elimination", "Team Deathmatch" },
                    specialFeatures = new string[] { "Espejos Dimensionales", "Física Alterada", "Enemigos Espejo" }
                },
                
                // Gas Station Super Creative - ¡El mapa súper creativo!
                new WorldMap { 
                    mapId = "cosmic_gas_station", 
                    mapName = "Estación de Gas Cósmica", 
                    description = "Una gasolinera interdimensional donde el combustible es energía pura",
                    isOpenWorld = true, 
                    minTierRequired = 6, 
                    maxPlayers = 24, 
                    gameModes = new string[] { "Conquest", "King of Hill", "Fuel Rush", "Space Race" },
                    specialFeatures = new string[] { "Combustible Energético", "Teletransportadores", "Tienda Cósmica", "Efectos Gravitacionales", "NPCs Alienígenas" }
                },
                
                // Graveyardville - BFN Style
                new WorldMap { 
                    mapId = "graveyardville", 
                    mapName = "Cementerio Ville", 
                    description = "Un cementerio lleno de zombies renacidos",
                    isOpenWorld = true, 
                    minTierRequired = 8, 
                    maxPlayers = 32, 
                    gameModes = new string[] { "Conquest", "King of Hill", "Zombie Apocalypse", "Graveyard Defense" },
                    specialFeatures = new string[] { "Tumbas Interactivas", "Zombies Esqueleto", "Efectos Sobrenaturales", "Niebla Misteriosa" }
                },
                
                // Beachside Boardwalk - Playa estilo BFN
                new WorldMap { 
                    mapId = "beachside_boardwalk", 
                    mapName = "Paseo Marítimo", 
                    description = "Playa bulliciosa con muelle y juegos de carnaval",
                    isOpenWorld = true, 
                    minTierRequired = 10, 
                    maxPlayers = 32, 
                    gameModes = new string[] { "Conquest", "King of Hill", "Beach Battle", "Carnival Chaos" },
                    specialFeatures = new string[] { "Juegos de Carnaval", "Efectos de Arena", "Cabinas de Playa", "Molinos de Viento" }
                },
                
                // Backyard Battleground - El clásico
                new WorldMap { 
                    mapId = "backyard_battleground", 
                    mapName = "Patio Trasero", 
                    description = "El patio trasero donde todo comenzó",
                    isOpenWorld = true, 
                    minTierRequired = 2, 
                    maxPlayers = 32, 
                    gameModes = new string[] { "Conquest", "King of Hill", "Team Deathmatch", "Backyard Brawl" },
                    specialFeatures = new string[] { "Casa Principal", "Garage", "Piscina", "Jardín de Flores", "Barbacoa" }
                },
                
                // Space Station Sigma - Mapa futurista
                new WorldMap { 
                    mapId = "space_station_sigma", 
                    mapName = "Estación Espacial Sigma", 
                    description = "Una estación espacial en órbita terrestre",
                    isOpenWorld = true, 
                    minTierRequired = 16, 
                    maxPlayers = 32, 
                    gameModes = new string[] { "Conquest", "King of Hill", "Space Combat", "Zero Gravity" },
                    specialFeatures = new string[] { "Gravedad Cero", "Ventanas al Espacio", "Túneles de Servicio", "Hangar de Naves" }
                },
                
                // Toxic Swamp - Área tóxica
                new WorldMap { 
                    mapId = "toxic_swamp", 
                    mapName = "Pantano Tóxico", 
                    description = "Un pantano contaminado con criaturas mutadas",
                    isOpenWorld = true, 
                    minTierRequired = 12, 
                    maxPlayers = 32, 
                    gameModes = new string[] { "Conquest", "King of Hill", "Swamp Survival", "Contamination" },
                    specialFeatures = new string[] { "Aguas Tóxicas", "Criaturas Mutadas", "Nubes de Gas", "Charcos Radioactivos" }
                },
                
                // Enchanted Forest - Bosque encantado
                new WorldMap { 
                    mapId = "enchanted_forest", 
                    mapName = "Bosque Encantado", 
                    description = "Un bosque mágico lleno de criaturas fantásticas",
                    isOpenWorld = true, 
                    minTierRequired = 14, 
                    maxPlayers = 32, 
                    gameModes = new string[] { "Conquest", "King of Hill", "Forest Magic", "Creature Hunt" },
                    specialFeatures = new string[] { "Árboles Inteligentes", "Criaturas Fantásticas", "Magia Natural", "Portal Místico" }
                },
                
                // Mech Factory - Fábrica de mechs
                new WorldMap { 
                    mapId = "mech_factory", 
                    mapName = "Fábrica de Mechs", 
                    description = "Una fábrica donde se construyen mechs gigantes",
                    isOpenWorld = true, 
                    minTierRequired = 18, 
                    maxPlayers = 32, 
                    gameModes = new string[] { "Conquest", "King of Hill", "Mech Battle", "Factory Defense" },
                    specialFeatures = new string[] { "Mechs Pilotables", "Montacargas Gigantes", "Ensamblaje Automático", "Líneas de Producción" }
                }
            };
        }

        void InitializeRankingSubdivisions()
        {
            rankingSubdivisions = new RankingSubdivision[]
            {
                // ESMERALDA Subdivisions
                new RankingSubdivision { 
                    parentTier = "ESMERALDA", 
                    subdivisionName = "ESMERALDA I", 
                    minStars = 250, maxStars = 287, 
                    icon = "esmeralda_1", 
                    description = "Primera chispa de poder esmeralda" 
                },
                new RankingSubdivision { 
                    parentTier = "ESMERALDA", 
                    subdivisionName = "ESMERALDA II", 
                    minStars = 288, maxStars = 324, 
                    icon = "esmeralda_2", 
                    description = "Poder esmeralda en crecimiento" 
                },
                new RankingSubdivision { 
                    parentTier = "ESMERALDA", 
                    subdivisionName = "ESMERALDA III", 
                    minStars = 325, maxStars = 374, 
                    icon = "esmeralda_3", 
                    description = "Maestría esmeralda completa" 
                },
                
                // PATRIARCA Subdivisions
                new RankingSubdivision { 
                    parentTier = "PATRIARCA", 
                    subdivisionName = "PATRIARCA I", 
                    minStars = 375, maxStars = 424, 
                    icon = "patriarca_1", 
                    description = "Primer grado de liderazgo ancestral" 
                },
                new RankingSubdivision { 
                    parentTier = "PATRIARCA", 
                    subdivisionName = "PATRIARCA II", 
                    minStars = 425, maxStars = 474, 
                    icon = "patriarca_2", 
                    description = "Sabiduría patriarcal en desarrollo" 
                },
                new RankingSubdivision { 
                    parentTier = "PATRIARCA", 
                    subdivisionName = "PATRIARCA III", 
                    minStars = 475, maxStars = 524, 
                    icon = "patriarca_3", 
                    description = "Patriarca supremo de los cuervos" 
                },
                
                // COMANDO Subdivisions
                new RankingSubdivision { 
                    parentTier = "COMANDO", 
                    subdivisionName = "COMANDO I", 
                    minStars = 525, maxStars = 574, 
                    icon = "comando_1", 
                    description = "Primer nivel de comando militar" 
                },
                new RankingSubdivision { 
                    parentTier = "COMANDO", 
                    subdivisionName = "COMANDO II", 
                    minStars = 575, maxStars = 624, 
                    icon = "comando_2", 
                    description = "Maestría en tácticas de comando" 
                },
                new RankingSubdivision { 
                    parentTier = "COMANDO", 
                    subdivisionName = "COMANDO III", 
                    minStars = 625, maxStars = 699, 
                    icon = "comando_3", 
                    description = "Comando supremo del thunder" 
                }
            };
        }

        void InitializeMissionSystem()
        {
            // Daily Missions
            dailyMissions = new Mission[]
            {
                new Mission { 
                    missionId = "daily_kills", 
                    missionName = "Cazador Diario", 
                    description = "Elimina 15 enemigos", 
                    type = MissionType.Eliminations, 
                    targetValue = 15, 
                    reward = 25, 
                    difficulty = MissionDifficulty.Easy, 
                    timeLimit = 24 * 60 * 60 // 24 hours in seconds
                },
                new Mission { 
                    missionId = "daily_victories", 
                    missionName = "Victoria Diaria", 
                    description = "Consigue 3 victorias", 
                    type = MissionType.Victories, 
                    targetValue = 3, 
                    reward = 35, 
                    difficulty = MissionDifficulty.Easy, 
                    timeLimit = 24 * 60 * 60
                },
                new Mission { 
                    missionId = "daily_assists", 
                    missionName = "Compañero Fiel", 
                    description = "Proporciona 10 asistencias", 
                    type = MissionType.Assists, 
                    targetValue = 10, 
                    reward = 20, 
                    difficulty = MissionDifficulty.Easy, 
                    timeLimit = 24 * 60 * 60
                }
            };
            
            // Weekly Missions
            weeklyMissions = new Mission[]
            {
                new Mission { 
                    missionId = "weekly_survivor", 
                    missionName = "Superviviente Semanal", 
                    description = "Sobrevive 60 minutos en combate", 
                    type = MissionType.PlayTime, 
                    targetValue = 3600, // 60 minutes in seconds
                    reward = 150, 
                    difficulty = MissionDifficulty.Medium, 
                    timeLimit = 7 * 24 * 60 * 60 // 7 days
                },
                new Mission { 
                    missionId = "weekly_headshots", 
                    missionName = "Tiro a la Cabeza Semanal", 
                    description = "Consigue 100 disparos a la cabeza", 
                    type = MissionType.Headshots, 
                    targetValue = 100, 
                    reward = 200, 
                    difficulty = MissionDifficulty.Hard, 
                    timeLimit = 7 * 24 * 60 * 60
                },
                new Mission { 
                    missionId = "weekly_maps", 
                    missionName = "Explorador de Mundos", 
                    description = "Juega en 5 mapas diferentes", 
                    type = MissionType.MapVariety, 
                    targetValue = 5, 
                    reward = 120, 
                    difficulty = MissionDifficulty.Medium, 
                    timeLimit = 7 * 24 * 60 * 60
                }
            };
            
            // Seasonal Missions
            seasonalMissions = new Mission[]
            {
                new Mission { 
                    missionId = "seasonal_legend", 
                    missionName = "Leyenda de la Temporada", 
                    description = "Alcanza el rango de Fragmento de Estrella", 
                    type = MissionType.Rank, 
                    targetValue = 9, // Fragment of Star tier
                    reward = 500, 
                    difficulty = MissionDifficulty.Legendary, 
                    timeLimit = 30 * 24 * 60 * 60 // 30 days
                },
                new Mission { 
                    missionId = "seasonal_perfectionist", 
                    missionName = "Perfeccionista Eterno", 
                    description = "Consigue 10 victorias perfectas", 
                    type = MissionType.PerfectGames, 
                    targetValue = 10, 
                    reward = 750, 
                    difficulty = MissionDifficulty.Legendary, 
                    timeLimit = 30 * 24 * 60 * 60
                }
            };
        }

        void InitializeAdvancedAI()
        {
            aiCharacters = new AdvancedAI[]
            {
                // AI Level 1: Novato - Inspired by PvZ & Fall Guys
                new AdvancedAI { 
                    aiId = "pea_shooter_newbie", 
                    aiName = "Pea Shooter Novato", 
                    aiLevel = 1, 
                    difficulty = AIDifficulty.Novice,
                    personality = AIPersonality.Cautious,
                    aimAccuracy = 0.3f, 
                    reactionTime = 2.5f,
                    strategyType = AIStrategy.Defensive,
                    skillLevel = 0.2f,
                    favoriteWeapons = new string[] { "Pea Shooter", "Basic Pistol" },
                    combatStyle = AICombatStyle.Assault,
                    characterModel = "CutePeaPlant",
                    specialAbility = "Piantura Verde",
                    description = "Un Pea Shooter novato que está aprendiendo a usar su poder"
                },
                new AdvancedAI { 
                    aiId = "zombie_dancer", 
                    aiName = "Zombie Bailarín", 
                    aiLevel = 1, 
                    difficulty = AIDifficulty.Novice,
                    personality = AIPersonality.Aggressive,
                    aimAccuracy = 0.25f, 
                    reactionTime = 2.8f,
                    strategyType = AIStrategy.Rush,
                    skillLevel = 0.15f,
                    favoriteWeapons = new string[] { "Brain Blaster", "Clumsy Shotgun" },
                    combatStyle = AICombatStyle.Berserker,
                    characterModel = "CuteZombie",
                    specialAbility = "Baile Confuso",
                    description = "Un zombie torpe pero entusiasta que baila mientras camina"
                },
                new AdvancedAI { 
                    aiId = "sunflower_cheerleader", 
                    aiName = "Girasol Animadora", 
                    aiLevel = 1, 
                    difficulty = AIDifficulty.Novice,
                    personality = AIPersonality.Cheerful,
                    aimAccuracy = 0.35f, 
                    reactionTime = 2.2f,
                    strategyType = AIStrategy.Support,
                    skillLevel = 0.25f,
                    favoriteWeapons = new string[] { "Sun Beam", "Cheer Staff" },
                    combatStyle = AICombatStyle.Support,
                    characterModel = "SunnyFlower",
                    specialAbility = "Luz Solar Energizante",
                    description = "Una girasol optimista que energiza a sus aliados"
                },
                
                // AI Level 2: Intermedio - PvZ & Fall Guys Characters
                new AdvancedAI { 
                    aiId = "cherry_bomb_expert", 
                    aiName = "Cereza Bomba Experta", 
                    aiLevel = 2, 
                    difficulty = AIDifficulty.Experienced,
                    personality = AIPersonality.Tactical,
                    aimAccuracy = 0.65f, 
                    reactionTime = 1.8f,
                    strategyType = AIStrategy.Tactical,
                    skillLevel = 0.5f,
                    favoriteWeapons = new string[] { "Cherry Bomb", "Explosive Pistol" },
                    combatStyle = AICombatStyle.Marksman,
                    characterModel = "CherryBomb",
                    specialAbility = "Explosión Controlada",
                    description = "Una cereza experta en explosiones controladas"
                },
                new AdvancedAI { 
                    aiId = "wallnut_heavy", 
                    aiName = "Nuez Muro Pesada", 
                    aiLevel = 2, 
                    difficulty = AIDifficulty.Experienced,
                    personality = AIPersonality.Patient,
                    aimAccuracy = 0.85f, 
                    reactionTime = 2.0f,
                    strategyType = AIStrategy.Defensive,
                    skillLevel = 0.7f,
                    favoriteWeapons = new string[] { "Nut Cannon", "Shield Launcher" },
                    combatStyle = AICombatStyle.Tank,
                    characterModel = "HeavyWallnut",
                    specialAbility = "Muro Defensivo",
                    description = "Una nuez robusta que actúa como tanque defensivo"
                },
                new AdvancedAI { 
                    aiId = "potato_miner", 
                    aiName = "Patata Minera", 
                    aiLevel = 2, 
                    difficulty = AIDifficulty.Experienced,
                    personality = AIPersonality.Tactical,
                    aimAccuracy = 0.75f, 
                    reactionTime = 1.5f,
                    strategyType = AIStrategy.Stealth,
                    skillLevel = 0.6f,
                    favoriteWeapons = new string[] { "Mine Launcher", "Stealth Rifle" },
                    combatStyle = AICombatStyle.Stealth,
                    characterModel = "PotatoMiner",
                    specialAbility = "Minas Explosivas",
                    description = "Una patata minera que coloca trampas inteligentes"
                },
                new AdvancedAI { 
                    aiId = "conehead_charger", 
                    aiName = "Cono Cabeza Cargando", 
                    aiLevel = 2, 
                    difficulty = AIDifficulty.Experienced,
                    personality = AIPersonality.Aggressive,
                    aimAccuracy = 0.7f, 
                    reactionTime = 1.6f,
                    strategyType = AIStrategy.Rush,
                    skillLevel = 0.65f,
                    favoriteWeapons = new string[] { "Cone Cannon", "Charge Rifle" },
                    combatStyle = AICombatStyle.Berserker,
                    characterModel = "ConeheadZombie",
                    specialAbility = "Carga Imparable",
                    description = "Un zombie con casco que carga sin miedo"
                },
                
                // AI Level 3: Expert - Advanced PvZ & Fall Guys
                new AdvancedAI { 
                    aiId = "snow_pea_sniper", 
                    aiName = "Guisante Nieve Francotirador", 
                    aiLevel = 3, 
                    difficulty = AIDifficulty.Expert,
                    personality = AIPersonality.Patient,
                    aimAccuracy = 0.9f, 
                    reactionTime = 0.8f,
                    strategyType = AIStrategy.Sniper,
                    skillLevel = 0.85f,
                    favoriteWeapons = new string[] { "Snow Pea Sniper", "Ice Beam" },
                    combatStyle = AICombatStyle.Sniper,
                    characterModel = "SnowPea",
                    specialAbility = "Congelación Letal",
                    description = "Un guisante de nieve con puntería perfecta"
                },
                new AdvancedAI { 
                    aiId = "chomper_fury", 
                    aiName = "Devorador Furioso", 
                    aiLevel = 3, 
                    difficulty = AIDifficulty.Expert,
                    personality = AIPersonality.Intense,
                    aimAccuracy = 0.95f, 
                    reactionTime = 0.6f,
                    strategyType = AIStrategy.Aggressive,
                    skillLevel = 0.9f,
                    favoriteWeapons = new string[] { "Chomp Blaster", "Bite Cannon" },
                    combatStyle = AICombatStyle.Dominator,
                    characterModel = "Chomper",
                    specialAbility = "Devorar Enemigos",
                    description = "Una planta carnívora furiosa que devora todo"
                },
                new AdvancedAI { 
                    aiId = "scientist_zombie", 
                    aiName = "Zombie Científico Loco", 
                    aiLevel = 3, 
                    difficulty = AIDifficulty.Expert,
                    personality = AIPersonality.Calculating,
                    aimAccuracy = 0.92f, 
                    reactionTime = 0.7f,
                    strategyType = AIStrategy.Tactical,
                    skillLevel = 0.88f,
                    favoriteWeapons = new string[] { "Science Ray", "Mutation Gun" },
                    combatStyle = AICombatStyle.Tactical,
                    characterModel = "ScientistZombie",
                    specialAbility = "Mutaciones Aleatorias",
                    description = "Un zombie científico con experimentos peligrosos"
                },
                new AdvancedAI { 
                    aiId = "fall_guy_swimmer", 
                    aiName = "Fall Guy Nadador", 
                    aiLevel = 3, 
                    difficulty = AIDifficulty.Expert,
                    personality = AIPersonality.Cheerful,
                    aimAccuracy = 0.88f, 
                    reactionTime = 0.9f,
                    strategyType = AIStrategy.Adaptive,
                    skillLevel = 0.83f,
                    favoriteWeapons = new string[] { "Bubble Gun", "Water Rifle" },
                    combatStyle = AICombatStyle.Speedster,
                    characterModel = "FallGuyBlue",
                    specialAbility = "Navegación Acuática",
                    description = "Un Fall Guy especializado en combate acuático"
                },
                
                // AI Level 4: Elite - Legendary PvZ & Fall Guys
                new AdvancedAI { 
                    aiId = "jelly_bean_elite", 
                    aiName = "Jelly Bean Élite", 
                    aiLevel = 4, 
                    difficulty = AIDifficulty.Elite,
                    personality = AIPersonality.Stealthy,
                    aimAccuracy = 0.98f, 
                    reactionTime = 0.4f,
                    strategyType = AIStrategy.Stealth,
                    skillLevel = 0.95f,
                    favoriteWeapons = new string[] { "Jelly Cannon", "Sticky Shot" },
                    combatStyle = AICombatStyle.Stealth,
                    characterModel = "JellyBean",
                    specialAbility = "Invisibilidad Pegajosa",
                    description = "Un jelly bean élite que se vuelve invisible"
                },
                new AdvancedAI { 
                    aiId = "giga_gargantuar", 
                    aiName = "Giga Gargantuár", 
                    aiLevel = 4, 
                    difficulty = AIDifficulty.Elite,
                    personality = AIPersonality.Storm,
                    aimAccuracy = 0.96f, 
                    reactionTime = 0.5f,
                    strategyType = AIStrategy.Dynamic,
                    skillLevel = 0.93f,
                    favoriteWeapons = new string[] { "Hammer Smash", "Giant Cannon" },
                    combatStyle = AICombatStyle.Dynamic,
                    characterModel = "GigaGargantuar",
                    specialAbility = "Golpe Sísmico",
                    description = "Un gigante que causa terremotos con sus golpes"
                },
                new AdvancedAI { 
                    aiId = "primal_peashooter", 
                    aiName = "Pea Shooter Primordial", 
                    aiLevel = 4, 
                    difficulty = AIDifficulty.Elite,
                    personality = AIPersonality.Cosmic,
                    aimAccuracy = 0.97f, 
                    reactionTime = 0.3f,
                    strategyType = AIStrategy.Cosmic,
                    skillLevel = 0.97f,
                    favoriteWeapons = new string[] { "Primal Cannon", "Cosmic Pea" },
                    combatStyle = AICombatStyle.Cosmic,
                    characterModel = "PrimalPea",
                    specialAbility = "Disparos Cósmicos",
                    description = "Un guisante ancestral con poder cósmico"
                },
                new AdvancedAI { 
                    aiId = "fall_guy_invisi", 
                    aiName = "Fall Guy Invisible", 
                    aiLevel = 4, 
                    difficulty = AIDifficulty.Elite,
                    personality = AIPersonality.Stealthy,
                    aimAccuracy = 0.94f, 
                    reactionTime = 0.35f,
                    strategyType = AIStrategy.Stealth,
                    skillLevel = 0.94f,
                    favoriteWeapons = new string[] { "Invisible Blaster", "Ghost Rifle" },
                    combatStyle = AICombatStyle.Stealth,
                    characterModel = "FallGuyGhost",
                    specialAbility = "Invisibilidad Total",
                    description = "Un Fall Guy que se vuelve completamente invisible"
                },
                
                // AI Level 5: Impossible - Ultimate PvZ & Fall Guys
                new AdvancedAI { 
                    aiId = "dr_zomboss", 
                    aiName = "Dr. Zomboss", 
                    aiLevel = 5, 
                    difficulty = AIDifficulty.Impossible,
                    personality = AIPersonality.Void,
                    aimAccuracy = 0.99f, 
                    reactionTime = 0.2f,
                    strategyType = AIStrategy.Void,
                    skillLevel = 0.99f,
                    favoriteWeapons = new string[] { "Zomboss Cannon", "Ultimate Ray" },
                    combatStyle = AICombatStyle.Perfect,
                    characterModel = "DrZomboss",
                    specialAbility = "Dominio Total",
                    description = "El maestro supremo de los zombies, poder absoluto"
                },
                new AdvancedAI { 
                    aiId = "ancient_treant", 
                    aiName = "Ent Ancestral", 
                    aiLevel = 5, 
                    difficulty = AIDifficulty.Impossible,
                    personality = AIPersonality.Temporal,
                    aimAccuracy = 0.99f, 
                    reactionTime = 0.1f,
                    strategyType = AIStrategy.Temporal,
                    skillLevel = 0.99f,
                    favoriteWeapons = new string[] { "Nature's Wrath", "Tree Cannon" },
                    combatStyle = AICombatStyle.Temporal,
                    characterModel = "AncientTreant",
                    specialAbility = "Poder de la Naturaleza",
                    description = "Un ser ancestral que controla las fuerzas de la naturaleza"
                },
                new AdvancedAI { 
                    aiId = "fall_guy_rainbow", 
                    aiName = "Fall Guy Arcoíris", 
                    aiLevel = 5, 
                    difficulty = AIDifficulty.Impossible,
                    personality = AIPersonality.Cosmic,
                    aimAccuracy = 0.99f, 
                    reactionTime = 0.05f,
                    strategyType = AIStrategy.Cosmic,
                    skillLevel = 0.99f,
                    favoriteWeapons = new string[] { "Rainbow Cannon", "All Colors Gun" },
                    combatStyle = AICombatStyle.Cosmic,
                    characterModel = "FallGuyRainbow",
                    specialAbility = "Poder del Arcoíris",
                    description = "El Fall Guy definitivo con todos los poderes"
                }
            };
        }

        void InitializeSimpleAuth()
        {
            if (!enableSimpleAuth) return;
            
            authSystem = new SimpleAuthSystem
            {
                currentPlayerName = PlayerPrefs.GetString("PlayerName", ""),
                isLoggedIn = !string.IsNullOrEmpty(PlayerPrefs.GetString("PlayerName", "")),
                loginHistory = LoadLoginHistory(),
                totalPlayTime = PlayerPrefs.GetFloat("TotalPlayTime", 0f),
                firstLoginDate = PlayerPrefs.GetString("FirstLoginDate", System.DateTime.Now.ToString()),
                lastLoginDate = System.DateTime.Now.ToString()
            };
        }

        void InitializeGameConfiguration()
        {
            if (!enableGameConfig) return;
            
            gameConfig = new GameConfiguration
            {
                // Graphics Settings
                graphicsQuality = (GraphicsQuality)PlayerPrefs.GetInt("GraphicsQuality", 2),
                resolutionWidth = PlayerPrefs.GetInt("ResolutionWidth", 1920),
                resolutionHeight = PlayerPrefs.GetInt("ResolutionHeight", 1080),
                fullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1,
                vSync = PlayerPrefs.GetInt("VSync", 1) == 1,
                frameRateLimit = PlayerPrefs.GetInt("FrameRateLimit", 60),
                antiAliasing = PlayerPrefs.GetInt("AntiAliasing", 2),
                textureQuality = (TextureQuality)PlayerPrefs.GetInt("TextureQuality", 2),
                shadowQuality = (ShadowQuality)PlayerPrefs.GetInt("ShadowQuality", 2),
                postProcessing = PlayerPrefs.GetInt("PostProcessing", 1) == 1,
                
                // Audio Settings
                masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f),
                sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1.0f),
                musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.8f),
                voiceVolume = PlayerPrefs.GetFloat("VoiceVolume", 1.0f),
                audioQuality = (AudioQuality)PlayerPrefs.GetInt("AudioQuality", 2),
                
                // Gameplay Settings
                mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 1.0f),
                invertMouseY = PlayerPrefs.GetInt("InvertMouseY", 0) == 1,
                fieldOfView = PlayerPrefs.GetFloat("FieldOfView", 90f),
                enableCrosshair = PlayerPrefs.GetInt("EnableCrosshair", 1) == 1,
                crosshairSize = PlayerPrefs.GetInt("CrosshairSize", 50),
                crosshairColor = PlayerPrefs.GetString("CrosshairColor", "#FFFFFF"),
                enableDamageNumbers = PlayerPrefs.GetInt("EnableDamageNumbers", 1) == 1,
                enableHitMarkers = PlayerPrefs.GetInt("EnableHitMarkers", 1) == 1,
                
                // Control Settings - Customizable Keybindings
                keyBindings = new Dictionary<string, KeyCode>
                {
                    { "MoveForward", (KeyCode)PlayerPrefs.GetInt("Key_MoveForward", (int)KeyCode.W) },
                    { "MoveBackward", (KeyCode)PlayerPrefs.GetInt("Key_MoveBackward", (int)KeyCode.S) },
                    { "MoveLeft", (KeyCode)PlayerPrefs.GetInt("Key_MoveLeft", (int)KeyCode.A) },
                    { "MoveRight", (KeyCode)PlayerPrefs.GetInt("Key_MoveRight", (int)KeyCode.D) },
                    { "Jump", (KeyCode)PlayerPrefs.GetInt("Key_Jump", (int)KeyCode.Space) },
                    { "Crouch", (KeyCode)PlayerPrefs.GetInt("Key_Crouch", (int)KeyCode.LeftControl) },
                    { "Sprint", (KeyCode)PlayerPrefs.GetInt("Key_Sprint", (int)KeyCode.LeftShift) },
                    { "Fire", (KeyCode)PlayerPrefs.GetInt("Key_Fire", (int)KeyCode.Mouse0) },
                    { "Aim", (KeyCode)PlayerPrefs.GetInt("Key_Aim", (int)KeyCode.Mouse1) },
                    { "Reload", (KeyCode)PlayerPrefs.GetInt("Key_Reload", (int)KeyCode.R) },
                    { "Interact", (KeyCode)PlayerPrefs.GetInt("Key_Interact", (int)KeyCode.F) },
                    { "UseItem", (KeyCode)PlayerPrefs.GetInt("Key_UseItem", (int)KeyCode.Q) },
                    { "Emote1", (KeyCode)PlayerPrefs.GetInt("Key_Emote1", (int)KeyCode.Alpha1) },
                    { "Emote2", (KeyCode)PlayerPrefs.GetInt("Key_Emote2", (int)KeyCode.Alpha2) },
                    { "Emote3", (KeyCode)PlayerPrefs.GetInt("Key_Emote3", (int)KeyCode.Alpha3) },
                    { "VoiceChat", (KeyCode)PlayerPrefs.GetInt("Key_VoiceChat", (int)KeyCode.V) },
                    { "Pause", (KeyCode)PlayerPrefs.GetInt("Key_Pause", (int)KeyCode.Escape) },
                    { "Scoreboard", (KeyCode)PlayerPrefs.GetInt("Key_Scoreboard", (int)KeyCode.Tab) },
                    { "Weapon1", (KeyCode)PlayerPrefs.GetInt("Key_Weapon1", (int)KeyCode.Alpha1) },
                    { "Weapon2", (KeyCode)PlayerPrefs.GetInt("Key_Weapon2", (int)KeyCode.Alpha2) },
                    { "Weapon3", (KeyCode)PlayerPrefs.GetInt("Key_Weapon3", (int)KeyCode.Alpha3) },
                    { "Weapon4", (KeyCode)PlayerPrefs.GetInt("Key_Weapon4", (int)KeyCode.Alpha4) }
                },
                
                // UI Settings
                showFPS = PlayerPrefs.GetInt("ShowFPS", 1) == 1,
                showPing = PlayerPrefs.GetInt("ShowPing", 1) == 1,
                uiScale = PlayerPrefs.GetFloat("UIScale", 1.0f),
                enableColorBlindMode = PlayerPrefs.GetInt("EnableColorBlindMode", 0) == 1,
                colorBlindType = (ColorBlindType)PlayerPrefs.GetInt("ColorBlindType", 0),
                
                // Network Settings
                serverRegion = (ServerRegion)PlayerPrefs.GetInt("ServerRegion", 0),
                maxPing = PlayerPrefs.GetInt("MaxPing", 150),
                autoReconnect = PlayerPrefs.GetInt("AutoReconnect", 1) == 1,
                
                // Accessibility Settings
                enableSubtitles = PlayerPrefs.GetInt("EnableSubtitles", 0) == 1,
                subtitleSize = PlayerPrefs.GetInt("SubtitleSize", 16),
                enableHighContrast = PlayerPrefs.GetInt("EnableHighContrast", 0) == 1,
                enableScreenShake = PlayerPrefs.GetInt("EnableScreenShake", 1) == 1,
                reduceMotion = PlayerPrefs.GetInt("ReduceMotion", 0) == 1,
                
                // Platform-specific settings
                platformSettings = new PlatformSpecificSettings
                {
                    isMobile = Application.isMobilePlatform,
                    isConsole = Application.platform == RuntimePlatform.PS4 || Application.platform == RuntimePlatform.XboxOne,
                    isPC = Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.LinuxPlayer,
                    supportsTouch = Application.isMobilePlatform,
                    supportsController = true,
                    supportsMouse = !Application.isMobilePlatform
                }
            };
        }

        void InitializeSeasonalSystem()
        {
            if (!enableSeasonalRanking) return;
            
            // Initialize seasonal tracking
            playerData.seasonalStars = PlayerPrefs.GetInt($"SeasonalStars_{currentSeason}", 0);
            
            // Check if we need to start a new season
            CheckSeasonTransition();
        }

        #region Enhanced Star Earning System
        public int CalculateStarsEarned(MatchResult result)
        {
            int stars = baseStarReward;
            
            // Base star for participation
            stars += 1;
            
            // Victory bonuses
            if (result.isVictory)
            {
                stars += 3; // Increased victory bonus
            }
            
            // Enhanced performance-based stars
            if (result.kills >= 10) stars += 2;
            if (result.kills >= 20) stars += 1;
            if (result.accuracy >= 0.8f) stars += 2;
            if (result.accuracy >= 0.95f) stars += 1;
            if (result.flagsCaptured >= 3) stars += 2;
            if (result.flagsCaptured >= 5) stars += 1;
            if (result.objectsCompleted >= 5) stars += 1;
            if (result.teamAssists >= 5) stars += 1;
            
            // Enhanced streak bonuses
            if (currentWinStreak >= 3) stars += 1;
            if (currentWinStreak >= 5) stars += 2;
            if (currentWinStreak >= 10) stars += 3;
            
            // Difficulty multiplier
            stars = Mathf.RoundToInt(stars * GetDifficultyMultiplier(result.gameMode));
            
            // Special circumstances
            if (result.isClutchPerformance) stars += 2;
            if (result.isComebackVictory) stars += 2;
            if (result.isPerfectGame) stars += 3;
            if (result.isPentaKill) stars += 5; // New bonus
            
            // Time bonus (faster matches = more stars)
            if (result.matchDuration < 300f) // Less than 5 minutes
                stars += 1;
            
            return Mathf.Clamp(stars, 1, maxStarsPerMatch);
        }

        public void AwardStars(int starCount, MatchResult result)
        {
            starsEarnedThisMatch = starCount;
            playerData.totalStars += starCount;
            playerData.lifetimeStars += starCount;
            playerData.seasonalStars += starCount; // Add seasonal tracking
            
            // Update tier if necessary
            CheckTierProgress();
            
            // Check achievements
            CheckAchievements(result);
            
            // Check seasonal rewards
            if (enableSeasonalRanking)
            {
                CheckSeasonalRewards();
            }
            
            // Save progress
            SavePlayerData();
            
            // Trigger events
            OnStarsEarned?.Invoke(starCount);
            
            Debug.Log($"🎖️ Awarded {starCount} stars! Total: {playerData.totalStars} | Seasonal: {playerData.seasonalStars}");
        }

        float GetDifficultyMultiplier(string gameMode)
        {
            switch (gameMode.ToLower())
            {
                case "elimination":
                    return 1.4f; // Hardest mode = more stars
                case "king_of_hill":
                    return 1.3f;
                case "conquest":
                    return 1.1f;
                case "team_deathmatch":
                    return 1.0f; // Standard
                case "bot_practice":
                    return 0.6f; // Easiest mode = fewer stars
                default:
                    return 1.0f;
            }
        }
        #endregion

        #region Enhanced Tier System
        public void CheckTierProgress()
        {
            int newTier = CalculateCurrentTier(playerData.totalStars);
            
            if (newTier > playerData.currentTier)
            {
                RankingTier oldTier = GetCurrentTier();
                playerData.currentTier = newTier;
                RankingTier newTierData = GetCurrentTier();
                
                // Enhanced tier up notification
                OnTierUp?.Invoke(newTierData);
                
                Debug.Log($"🌟 ¡Ascenso de Rango! {oldTier.tierName} ➜ {newTierData.tierName}");
                Debug.Log($"📖 {newTierData.tierDescription}");
                
                // Check for tier-specific rewards
                CheckTierRewards(newTierData);
            }
        }

        void CheckTierRewards(RankingTier tier)
        {
            // Award special tier rewards
            switch (tier.tierName)
            {
                case "Tejedora de Destinos":
                    AwardSpecialReward("Destinos_Special", "Hilo del Destino", "Controla el hilo de la batalla");
                    break;
                case "ESMERALDA":
                    AwardSpecialReward("Esmeralda_Special", "Corona de Esmeralda", "Corona que irradia poder esmeralda");
                    break;
                case "PATRIARCA":
                    AwardSpecialReward("Patriarca_Special", "Bastón Patriarcal", "Bastón que otorga sabiduría ancestral");
                    break;
                case "COMANDO":
                    AwardSpecialReward("Comando_Special", "Insignia de Comando", "Insignia suprema de comando militar");
                    break;
                case "Guardiana del Portal":
                    AwardSpecialReward("Portal_Special", "Clave Dimensional", "Poder de abrir portales entre mundos");
                    break;
                case "Ser de Luz Eternal":
                    AwardSpecialReward("Luz_Special", "Corona de Luz", "Corona que irradia luz eterna");
                    break;
            }
        }

        void AwardSpecialReward(string id, string name, string description)
        {
            // Implementation for special tier rewards
            Debug.Log($"🏆 ¡Recompensa Especial Desbloqueada: {name}!");
        }
        #endregion

        #region Enhanced Achievement System
        public void CheckAchievements(MatchResult result)
        {
            foreach (Achievement achievement in achievements)
            {
                if (unlockedAchievements[achievement.id]) continue;
                
                if (CheckAchievementCondition(achievement, result))
                {
                    UnlockAchievement(achievement);
                }
            }
        }

        bool CheckAchievementCondition(Achievement achievement, MatchResult result)
        {
            switch (achievement.id)
            {
                // Match achievements
                case "first_match":
                    return totalMatches == 1;
                case "ten_matches":
                    return totalMatches >= 10;
                case "fifty_matches":
                    return totalMatches >= 50;
                case "hundred_matches":
                    return totalMatches >= 100;
                
                // Victory achievements
                case "first_win":
                    return playerData.totalWins == 1;
                case "five_wins":
                    return playerData.totalWins >= 5;
                case "ten_wins":
                    return playerData.totalWins >= 10;
                case "twenty_five_wins":
                    return playerData.totalWins >= 25;
                case "fifty_wins":
                    return playerData.totalWins >= 50;
                
                // Streak achievements
                case "three_streak":
                    return currentWinStreak >= 3;
                case "five_streak":
                    return currentWinStreak >= 5;
                case "ten_streak":
                    return currentWinStreak >= 10;
                case "twenty_streak":
                    return currentWinStreak >= 20;
                
                // Performance achievements
                case "perfect_accuracy":
                    return result.accuracy >= 1.0f;
                case "headshot_master":
                    return GetPerformanceMetric("Headshots") >= 50;
                case "elimination_expert":
                    return GetPerformanceMetric("TotalKills") >= 100;
                case "survival_expert":
                    return playerData.totalPlayTime >= 1000f * 60f; // 1000 minutes
                
                // Special achievements
                case "comeback_victory":
                    return result.isComebackVictory;
                case "clutch_performance":
                    return result.isClutchPerformance;
                case "pentakill":
                    return result.isPentaKill;
                case "flawless_victory":
                    return result.isFlawlessVictory;
                
                // Team achievements
                case "team_player":
                    return result.teamAssists >= 100;
                case "flag_captain":
                    return GetPerformanceMetric("TotalFlagsCaptured") >= 25;
                case "tactical_leader":
                    return GetPerformanceMetric("TeamWins") >= 50;
                
                // Time achievements
                case "early_bird":
                    return System.DateTime.Now.Hour < 6;
                case "night_owl":
                    return System.DateTime.Now.Hour >= 0 && System.DateTime.Now.Hour < 6;
                case "marathon_player":
                    return result.matchDuration >= 7200f; // 2 hours
                
                // Rare achievements
                case "speed_demon":
                    return result.matchDuration < 120f && result.isVictory; // Less than 2 minutes
                case "last_standing":
                    return result.isLastStanding && result.isVictory;
                case "no_scope":
                    return result.noScopeKills >= 10 && result.isVictory;
                
                default:
                    return false;
            }
        }

        int GetPerformanceMetric(string metricName)
        {
            return performanceMetrics.ContainsKey(metricName) ? performanceMetrics[metricName] : 0;
        }

        void UnlockAchievement(Achievement achievement)
        {
            unlockedAchievements[achievement.id] = true;
            playerData.totalStars += achievement.rewardStars;
            
            // Save unlocked achievements
            SaveUnlockedAchievements();
            
            OnAchievementUnlocked?.Invoke(achievement);
            
            // Enhanced achievement notification
            Debug.Log($"🏅 ¡Logro Desbloqueado: {achievement.name}! (+{achievement.rewardStars} ⭐)");
            Debug.Log($"📜 {achievement.description}");
        }
        #endregion

        #region Seasonal System
        void CheckSeasonTransition()
        {
            // Simple season transition logic - can be enhanced
            string lastSeason = PlayerPrefs.GetString("LastSeason", "");
            
            if (lastSeason != currentSeason && !string.IsNullOrEmpty(lastSeason))
            {
                // New season started
                Debug.Log($"🌟 ¡Nueva Temporada Comenzada: {currentSeason}!");
                
                // Reset seasonal stats but keep lifetime stats
                playerData.seasonalStars = 0;
                PlayerPrefs.SetInt($"SeasonalStars_{currentSeason}", 0);
                
                // Award season transition bonus
                int seasonBonus = 50;
                playerData.totalStars += seasonBonus;
                playerData.seasonalStars += seasonBonus;
                
                Debug.Log($"🎁 ¡Bonus de Nueva Temporada: +{seasonBonus} ⭐!");
            }
            
            PlayerPrefs.SetString("LastSeason", currentSeason);
        }

        void CheckSeasonalRewards()
        {
            if (!enableSeasonalRanking) return;
            
            // Check for seasonal milestones
            int[] milestones = { 100, 250, 500, 750, 1000 };
            
            foreach (int milestone in milestones)
            {
                string milestoneKey = $"SeasonalMilestone_{milestone}";
                if (playerData.seasonalStars >= milestone && 
                    PlayerPrefs.GetInt(milestoneKey, 0) == 0)
                {
                    PlayerPrefs.SetInt(milestoneKey, 1);
                    int bonus = milestone / 10; // Bonus stars
                    playerData.totalStars += bonus;
                    playerData.seasonalStars += bonus;
                    
                    Debug.Log($"🎊 ¡Hito Estacional Alcanzado: {milestone} ⭐! Bonus: +{bonus} ⭐");
                }
            }
        }
        #endregion

        #region Enhanced Match Processing
        public void ProcessMatchResult(MatchResult result)
        {
            // Update match tracking
            totalMatches++;
            recentMatches.Add(result);
            
            // Keep only last 30 matches (increased from 20)
            if (recentMatches.Count > 30)
            {
                recentMatches.RemoveAt(0);
            }
            
            // Update win streak
            if (result.isVictory)
            {
                currentWinStreak++;
                if (currentWinStreak > longestWinStreak)
                {
                    longestWinStreak = currentWinStreak;
                }
            }
            else
            {
                currentWinStreak = 0;
            }
            
            // Update player data
            playerData.totalMatches++;
            if (result.isVictory)
            {
                playerData.totalWins++;
            }
            playerData.totalPlayTime += result.matchDuration;
            
            // Update enhanced performance metrics
            UpdateEnhancedPerformanceMetrics(result);
            
            // Calculate and award stars
            int earnedStars = CalculateStarsEarned(result);
            AwardStars(earnedStars, result);
            
            // Update favorite game mode with enhanced logic
            UpdateFavoriteGameMode(result.gameMode);
            
            // Save all data
            SavePlayerData();
            SaveRecentMatches();
            
            // Enhanced match summary
            Debug.Log($"⚔️ Combate Finalizado - ⭐: {earnedStars} | 🏆: {GetCurrentTier().tierName} | 🎯: {result.kills}/{result.deaths} | 📊: {(result.accuracy * 100):F1}%");
        }

        void UpdateEnhancedPerformanceMetrics(MatchResult result)
        {
            // Enhanced metric tracking
            string[] metrics = { 
                "TotalKills", "TotalDeaths", "TotalFlagsCaptured", "MostKills",
                "Headshots", "TeamWins", "ObjectiveTime", "DamageDealt", "HealingDone"
            };
            
            foreach (string metric in metrics)
            {
                if (!performanceMetrics.ContainsKey(metric))
                    performanceMetrics[metric] = 0;
            }
            
            // Update basic metrics
            performanceMetrics["TotalKills"] += result.kills;
            performanceMetrics["TotalDeaths"] += result.deaths;
            performanceMetrics["TotalFlagsCaptured"] += result.flagsCaptured;
            
            // Update enhanced metrics
            performanceMetrics["Headshots"] += result.headshots;
            performanceMetrics["TeamWins"] += result.isVictory ? 1 : 0;
            performanceMetrics["ObjectiveTime"] += result.objectiveTime;
            performanceMetrics["DamageDealt"] += result.damageDealt;
            performanceMetrics["HealingDone"] += result.healingDone;
            
            // Update ratios
            performanceRatios["KillDeathRatio"] = (float)performanceMetrics["TotalKills"] / Mathf.Max(1, performanceMetrics["TotalDeaths"]);
            performanceRatios["WinRate"] = (float)performanceMetrics["TeamWins"] / Mathf.Max(1, totalMatches);
            
            // Update records
            if (result.kills > performanceMetrics["MostKills"])
            {
                performanceMetrics["MostKills"] = result.kills;
                Debug.Log($"🔥 ¡Nuevo Record de Kills: {result.kills}!");
            }
        }
        #endregion

        #region Data Persistence (Enhanced)
        void SavePlayerData()
        {
            PlayerPrefs.SetInt("TotalStars", playerData.totalStars);
            PlayerPrefs.SetInt("CurrentTier", playerData.currentTier);
            PlayerPrefs.SetInt("TotalMatches", playerData.totalMatches);
            PlayerPrefs.SetInt("TotalWins", playerData.totalWins);
            PlayerPrefs.SetInt("CurrentWinStreak", currentWinStreak);
            PlayerPrefs.SetInt("LongestWinStreak", longestWinStreak);
            PlayerPrefs.SetFloat("TotalPlayTime", playerData.totalPlayTime);
            PlayerPrefs.SetString("FavoriteGameMode", playerData.favoriteGameMode);
            PlayerPrefs.SetInt("LifetimeStars", playerData.lifetimeStars);
            
            if (enableSeasonalRanking)
            {
                PlayerPrefs.SetInt($"SeasonalStars_{currentSeason}", playerData.seasonalStars);
            }
            
            PlayerPrefs.Save();
        }

        void SaveUnlockedAchievements()
        {
            string json = JsonUtility.ToJson(unlockedAchievements);
            PlayerPrefs.SetString("UnlockedAchievements", json);
            PlayerPrefs.Save();
        }

        void SaveRecentMatches()
        {
            string json = JsonUtility.ToJson(recentMatches.ToArray());
            PlayerPrefs.SetString("RecentMatches", json);
            PlayerPrefs.Save();
        }

        void LoadRecentMatches()
        {
            string json = PlayerPrefs.GetString("RecentMatches", "");
            if (!string.IsNullOrEmpty(json))
            {
                MatchResult[] matches = JsonUtility.FromJson<MatchResult[]>(json);
                recentMatches = matches.ToList();
            }
        }
        #endregion

        #region Helper Functions
        List<LoginRecord> LoadLoginHistory()
        {
            string json = PlayerPrefs.GetString("LoginHistory", "");
            if (string.IsNullOrEmpty(json))
                return new List<LoginRecord>();
                
            try
            {
                LoginRecord[] records = JsonUtility.FromJson<LoginRecord[]>(json);
                return records != null ? records.ToList() : new List<LoginRecord>();
            }
            catch
            {
                return new List<LoginRecord>();
            }
        }

        void SaveLoginHistory()
        {
            if (authSystem != null && authSystem.loginHistory != null)
            {
                string json = JsonUtility.ToJson(authSystem.loginHistory.ToArray());
                PlayerPrefs.SetString("LoginHistory", json);
            }
        }

        // Advanced AI Logic Functions
        public AdvancedAI GetRandomAI(AIDifficulty difficulty)
        {
            var suitableAIs = aiCharacters.Where(ai => ai.difficulty == difficulty && ai.aiLevel <= GetCurrentTier()).ToArray();
            if (suitableAIs.Length == 0)
                return aiCharacters.First(); // fallback
            
            return suitableAIs[Random.Range(0, suitableAIs.Length)];
        }

        // Creative Character Helper Functions
        public AdvancedAI[] GetPvZCharacters()
        {
            return aiCharacters.Where(ai => 
                ai.characterModel.Contains("Pea") || 
                ai.characterModel.Contains("Zombie") || 
                ai.characterModel.Contains("Cherry") || 
                ai.characterModel.Contains("Sun") ||
                ai.characterModel.Contains("Chomper")
            ).ToArray();
        }

        public AdvancedAI[] GetFallGuysCharacters()
        {
            return aiCharacters.Where(ai => 
                ai.characterModel.Contains("FallGuy")
            ).ToArray();
        }

        public AdvancedAI GetCharacterByModel(string modelName)
        {
            return aiCharacters.FirstOrDefault(ai => ai.characterModel == modelName);
        }

        public void ActivateCharacterAbility(AdvancedAI character)
        {
            if (character == null) return;
            
            // Trigger special ability based on character type
            switch (character.specialAbility)
            {
                case "Piantura Verde":
                    // Heal nearby allies
                    break;
                case "Baile Confuso":
                    // Disorient enemies
                    break;
                case "Luz Solar Energizante":
                    // Boost ally performance
                    break;
                case "Explosión Controlada":
                    // Controlled area damage
                    break;
                case "Muro Defensivo":
                    // Mobile shield
                    break;
                // Add more ability implementations...
            }
            
            Debug.Log($"✨ {character.aiName} used ability: {character.specialAbility}!");
        }

        public AdvancedAI[] GetActiveAIMatch(int playerCount)
        {
            var activeAIs = new List<AdvancedAI>();
            int targetAICount = Mathf.Min(maxAIPerMatch, Mathf.Max(0, 12 - playerCount));
            
            // Mix different difficulty levels
            var difficulties = new[] { AIDifficulty.Novice, AIDifficulty.Experienced, AIDifficulty.Expert, AIDifficulty.Elite };
            var difficultyWeights = new[] { 0.4f, 0.3f, 0.2f, 0.1f };
            
            for (int i = 0; i < targetAICount; i++)
            {
                float randomValue = Random.value;
                AIDifficulty selectedDifficulty = AIDifficulty.Novice;
                
                float cumulativeWeight = 0;
                for (int j = 0; j < difficulties.Length; j++)
                {
                    cumulativeWeight += difficultyWeights[j];
                    if (randomValue <= cumulativeWeight)
                    {
                        selectedDifficulty = difficulties[j];
                        break;
                    }
                }
                
                var ai = GetRandomAI(selectedDifficulty);
                if (ai != null && !activeAIs.Any(existing => existing.aiId == ai.aiId))
                {
                    activeAIs.Add(ai);
                }
            }
            
            return activeAIs.ToArray();
        }

        // Authentication Helper Functions
        public bool IsPlayerLoggedIn()
        {
            return authSystem != null && authSystem.isLoggedIn && !string.IsNullOrEmpty(authSystem.currentPlayerName);
        }

        public string GetCurrentPlayerName()
        {
            return IsPlayerLoggedIn() ? authSystem.currentPlayerName : "Guest";
        }

        // Configuration Helper Functions
        public void ApplyGraphicsSettings()
        {
            if (gameConfig == null) return;
            
            QualitySettings.SetQualityLevel((int)gameConfig.graphicsQuality);
            QualitySettings.antiAliasing = gameConfig.antiAliasing;
            QualitySettings.vSyncCount = gameConfig.vSync ? 1 : 0;
            Application.targetFrameRate = gameConfig.frameRateLimit;
        }

        public void ApplyAudioSettings()
        {
            if (gameConfig == null) return;
            
            AudioListener.volume = gameConfig.masterVolume;
        }

        public KeyCode GetKeyBinding(string actionName)
        {
            if (gameConfig == null || gameConfig.keyBindings == null)
                return KeyCode.None;
                
            return gameConfig.keyBindings.ContainsKey(actionName) ? gameConfig.keyBindings[actionName] : KeyCode.None;
        }

        public void SetKeyBinding(string actionName, KeyCode newKey)
        {
            if (gameConfig == null || gameConfig.keyBindings == null)
                return;
                
            gameConfig.keyBindings[actionName] = newKey;
            gameConfig.SaveConfiguration();
        }
        #endregion

        #region Public Getters (Enhanced)
        public int GetCurrentStars() => playerData.totalStars;
        public int GetCurrentTier() => playerData.currentTier;
        public int GetTotalMatches() => playerData.totalMatches;
        public int GetTotalWins() => playerData.totalWins;
        public int GetCurrentWinStreak() => currentWinStreak;
        public int GetLongestWinStreak() => longestWinStreak;
        public float GetTotalPlayTime() => playerData.totalPlayTime;
        public string GetFavoriteGameMode() => playerData.favoriteGameMode;
        public float GetWinRate() => playerData.totalMatches > 0 ? (float)playerData.totalWins / playerData.totalMatches : 0f;
        public int GetStarsEarnedThisMatch() => starsEarnedThisMatch;
        public List<MatchResult> GetRecentMatches() => recentMatches;
        
        // Enhanced getters
        public int GetLifetimeStars() => playerData.lifetimeStars;
        public int GetSeasonalStars() => playerData.seasonalStars;
        public float GetKillDeathRatio() => performanceRatios.ContainsKey("KillDeathRatio") ? performanceRatios["KillDeathRatio"] : 0f;
        public int GetBestMatchKills() => performanceMetrics.ContainsKey("MostKills") ? performanceMetrics["MostKills"] : 0;
        public RankingTier GetCurrentTierData() => GetCurrentTier();
        public string GetCurrentSeason() => currentSeason;
        
        public Dictionary<string, int> GetAllPerformanceMetrics() => new Dictionary<string, int>(performanceMetrics);
        public Dictionary<string, float> GetAllPerformanceRatios() => new Dictionary<string, float>(performanceRatios);
        #endregion
    }

    // Enhanced data classes
    [System.Serializable]
    public class PlayerProgressionData
    {
        public int totalStars;
        public int currentTier;
        public int totalMatches;
        public int totalWins;
        public int currentWinStreak;
        public int longestWinStreak;
        public float totalPlayTime;
        public string favoriteGameMode;
        public int seasonalStars;
        public int lifetimeStars;
    }

    [System.Serializable]
    public class RankingTier
    {
        public string tierName;
        public int minStars;
        public int maxStars;
        public Color tierColor;
        public string badgeIcon;
        public string tierDescription;
    }

    [System.Serializable]
    public class Achievement
    {
        public string id;
        public string name;
        [TextArea(2, 4)]
        public string description;
        public string icon;
        public int rewardStars;
    }

    [System.Serializable]
    public class ProgressionReward
    {
        public int level;
        public string name;
        [TextArea(2, 4)]
        public string description;
        public RewardType type;
        public string value;
        public bool isClaimed = false;
    }

    [System.Serializable]
    public class SeasonalProgress
    {
        public string seasonName;
        public int seasonNumber;
        public int seasonalStars;
        public int bestTier;
        public float totalPlayTime;
    }

    [System.Serializable]
    public class MatchResult
    {
        public bool isVictory;
        public int kills;
        public int deaths;
        public int assists;
        public int flagsCaptured;
        public int objectsCompleted;
        public float accuracy;
        public float matchDuration;
        public string gameMode;
        public int teamScore;
        public int enemyScore;
        public bool isClutchPerformance;
        public bool isComebackVictory;
        public bool isPerfectGame;
        public bool isPentaKill;
        public bool isFlawlessVictory;
        public bool isLastStanding;
        public int teamAssists;
        public int headshots;
        public float objectiveTime;
        public int damageDealt;
        public int healingDone;
        public int noScopeKills;
    }

    public enum RewardType
    {
        WeaponSkin,
        CharacterSkin,
        Emote,
        Title,
        Map,
        Accessory
    }

    [System.Serializable]
    public class WorldMap
    {
        public string mapId;
        public string mapName;
        [TextArea(2, 4)]
        public string description;
        public bool isOpenWorld;
        public int minTierRequired;
        public int maxPlayers;
        public string[] gameModes;
        public string[] specialFeatures;
        public bool isUnlocked = false;
        public int unlockStarsRequired = 0;
    }

    [System.Serializable]
    public class RankingSubdivision
    {
        public string parentTier;
        public string subdivisionName;
        public int minStars;
        public int maxStars;
        public string icon;
        public string description;
        public bool isUnlocked = false;
    }

    [System.Serializable]
    public class Mission
    {
        public string missionId;
        public string missionName;
        [TextArea(2, 4)]
        public string description;
        public MissionType type;
        public int targetValue;
        public int currentProgress = 0;
        public int reward;
        public MissionDifficulty difficulty;
        public int timeLimit; // in seconds
        public bool isCompleted = false;
        public bool isClaimed = false;
        public System.DateTime startTime;
        public System.DateTime endTime;
    }

    public enum MissionType
    {
        Eliminations,
        Victories,
        Assists,
        PlayTime,
        Headshots,
        MapVariety,
        Rank,
        PerfectGames,
        FlagsCaptured,
        ObjectivesCompleted
    }

    public enum MissionDifficulty
    {
        Easy,
        Medium,
        Hard,
        Legendary
    }

    // ===== ADVANCED AI SYSTEM =====
    [System.Serializable]
    public class AdvancedAI
    {
        public string aiId;
        public string aiName;
        public int aiLevel;
        public AIDifficulty difficulty;
        public AIPersonality personality;
        public float aimAccuracy; // 0.0 to 1.0
        public float reactionTime; // in seconds
        public AIStrategy strategyType;
        public float skillLevel; // 0.0 to 1.0
        public string[] favoriteWeapons;
        public AICombatStyle combatStyle;
        public string characterModel; // 3D Model reference
        public string specialAbility; // Unique ability name
        [TextArea(2, 4)]
        public string description;
        public bool isActive = false;
        public int matchesPlayed = 0;
        public float winRate = 0.5f;
        public int totalKills = 0;
        public int totalDeaths = 0;
        public int headshotPercentage = 0;
        public List<string> battleQuotes = new List<string>();
        public Color characterColor = Color.white;
        public float characterScale = 1.0f;
    }

    public enum AIDifficulty
    {
        Novice,      // Like new players
        Experienced, // Like veteran players  
        Expert,      // Like skilled players
        Elite,       // Like top 1% players
        Impossible   // Near perfect AI
    }

    public enum AIPersonality
    {
        Cautious,    // Plays safe, defensive
        Aggressive,  // Rushes enemies
        Tactical,    // Uses strategy
        Patient,     // Waits for perfect shots
        Cheerful,    // Optimistic, fast-paced
        Strategic,   // Long-term planning
        Intense,     // High-pressure situations
        Calculating, // Analyzes everything
        Stealthy,    // Sneaky, unseen
        Storm,       // Fast and destructive
        Cosmic,      // Mysterious, powerful
        Void,        // Dark, perfect
        Temporal     // Time-manipulating
    }

    public enum AIStrategy
    {
        Defensive,   // Protect objectives
        Rush,        // Fast aggressive pushes
        Tactical,    // Coordinated attacks
        Sniper,      // Long-range precision
        FastPaced,   // Quick skirmishes
        Adaptive,    // Changes strategy
        Aggressive,  // Always attacking
        Stealth,     // Undetected movements
        Dynamic,     // Mix of styles
        Cosmic,      // Otherworldly tactics
        Void,        // Reality-bending
        Temporal     // Time-based tactics
    }

    public enum AICombatStyle
    {
        Assault,     // Close range fighter
        Berserker,   // Reckless attacker
        Marksman,    // Medium range expert
        Sniper,      // Long range killer
        Speedster,   // Fast movement
        Adaptable,   // Any range
        Dominator,   // Controls battlefield
        Tactical,    // Position-based
        Stealth,     // Unseen killer
        Dynamic,     // Changes style
        Cosmic,      // Supernatural
        Perfect,     // Flawless execution
        Temporal,    // Time manipulation
        Tank,        // Heavy defense
        Support,     // Heals and buffs
        Engineer     // Builds and repairs
    }

    // ===== SIMPLE AUTHENTICATION SYSTEM =====
    [System.Serializable]
    public class SimpleAuthSystem
    {
        public string currentPlayerName;
        public bool isLoggedIn;
        public List<LoginRecord> loginHistory;
        public float totalPlayTime;
        public string firstLoginDate;
        public string lastLoginDate;
        public bool isFirstTimePlayer = false;
        
        public bool Login(string playerName)
        {
            if (string.IsNullOrEmpty(playerName) || playerName.Length < 2)
                return false;
                
            currentPlayerName = playerName.Trim();
            isLoggedIn = true;
            isFirstTimePlayer = string.IsNullOrEmpty(PlayerPrefs.GetString("PlayerName", ""));
            
            // Update login history
            var loginRecord = new LoginRecord
            {
                playerName = currentPlayerName,
                loginTime = System.DateTime.Now,
                sessionDuration = 0f
            };
            
            loginHistory.Add(loginRecord);
            SaveLoginHistory();
            
            // Save player data
            PlayerPrefs.SetString("PlayerName", currentPlayerName);
            PlayerPrefs.SetString("LastLoginDate", System.DateTime.Now.ToString());
            
            if (isFirstTimePlayer)
            {
                PlayerPrefs.SetString("FirstLoginDate", System.DateTime.Now.ToString());
            }
            
            PlayerPrefs.Save();
            
            Debug.Log($"✅ Player '{currentPlayerName}' logged in successfully!");
            return true;
        }
        
        public void Logout()
        {
            if (isLoggedIn && loginHistory.Count > 0)
            {
                // Update session duration
                var lastLogin = loginHistory[loginHistory.Count - 1];
                lastLogin.sessionDuration = (float)(System.DateTime.Now - lastLogin.loginTime).TotalSeconds;
                totalPlayTime += lastLogin.sessionDuration;
                PlayerPrefs.SetFloat("TotalPlayTime", totalPlayTime);
            }
            
            currentPlayerName = "";
            isLoggedIn = false;
            SaveLoginHistory();
            PlayerPrefs.Save();
            
            Debug.Log("👋 Player logged out");
        }
        
        public string GetPlayerDisplayName()
        {
            return isLoggedIn ? currentPlayerName : "Guest";
        }
        
        public float GetTotalPlayTimeHours()
        {
            return totalPlayTime / 3600f; // Convert seconds to hours
        }
    }

    [System.Serializable]
    public class LoginRecord
    {
        public string playerName;
        public System.DateTime loginTime;
        public float sessionDuration;
    }

    // ===== GAME CONFIGURATION SYSTEM =====
    [System.Serializable]
    public class GameConfiguration
    {
        // Graphics Settings
        public GraphicsQuality graphicsQuality;
        public int resolutionWidth;
        public int resolutionHeight;
        public bool fullscreen;
        public bool vSync;
        public int frameRateLimit;
        public int antiAliasing;
        public TextureQuality textureQuality;
        public ShadowQuality shadowQuality;
        public bool postProcessing;
        
        // Audio Settings
        public float masterVolume;
        public float sfxVolume;
        public float musicVolume;
        public float voiceVolume;
        public AudioQuality audioQuality;
        
        // Gameplay Settings
        public float mouseSensitivity;
        public bool invertMouseY;
        public float fieldOfView;
        public bool enableCrosshair;
        public int crosshairSize;
        public string crosshairColor;
        public bool enableDamageNumbers;
        public bool enableHitMarkers;
        
        // Control Settings
        public Dictionary<string, KeyCode> keyBindings;
        
        // UI Settings
        public bool showFPS;
        public bool showPing;
        public float uiScale;
        public bool enableColorBlindMode;
        public ColorBlindType colorBlindType;
        
        // Network Settings
        public ServerRegion serverRegion;
        public int maxPing;
        public bool autoReconnect;
        
        // Accessibility Settings
        public bool enableSubtitles;
        public int subtitleSize;
        public bool enableHighContrast;
        public bool enableScreenShake;
        public bool reduceMotion;
        
        // Platform Settings
        public PlatformSpecificSettings platformSettings;
        
        public void SaveConfiguration()
        {
            // Graphics
            PlayerPrefs.SetInt("GraphicsQuality", (int)graphicsQuality);
            PlayerPrefs.SetInt("ResolutionWidth", resolutionWidth);
            PlayerPrefs.SetInt("ResolutionHeight", resolutionHeight);
            PlayerPrefs.SetInt("Fullscreen", fullscreen ? 1 : 0);
            PlayerPrefs.SetInt("VSync", vSync ? 1 : 0);
            PlayerPrefs.SetInt("FrameRateLimit", frameRateLimit);
            PlayerPrefs.SetInt("AntiAliasing", antiAliasing);
            PlayerPrefs.SetInt("TextureQuality", (int)textureQuality);
            PlayerPrefs.SetInt("ShadowQuality", (int)shadowQuality);
            PlayerPrefs.SetInt("PostProcessing", postProcessing ? 1 : 0);
            
            // Audio
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
            PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
            PlayerPrefs.SetFloat("VoiceVolume", voiceVolume);
            PlayerPrefs.SetInt("AudioQuality", (int)audioQuality);
            
            // Gameplay
            PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity);
            PlayerPrefs.SetInt("InvertMouseY", invertMouseY ? 1 : 0);
            PlayerPrefs.SetFloat("FieldOfView", fieldOfView);
            PlayerPrefs.SetInt("EnableCrosshair", enableCrosshair ? 1 : 0);
            PlayerPrefs.SetInt("CrosshairSize", crosshairSize);
            PlayerPrefs.SetString("CrosshairColor", crosshairColor);
            PlayerPrefs.SetInt("EnableDamageNumbers", enableDamageNumbers ? 1 : 0);
            PlayerPrefs.SetInt("EnableHitMarkers", enableHitMarkers ? 1 : 0);
            
            // Controls
            foreach (var kvp in keyBindings)
            {
                PlayerPrefs.SetInt($"Key_{kvp.Key}", (int)kvp.Value);
            }
            
            // UI
            PlayerPrefs.SetInt("ShowFPS", showFPS ? 1 : 0);
            PlayerPrefs.SetInt("ShowPing", showPing ? 1 : 0);
            PlayerPrefs.SetFloat("UIScale", uiScale);
            PlayerPrefs.SetInt("EnableColorBlindMode", enableColorBlindMode ? 1 : 0);
            PlayerPrefs.SetInt("ColorBlindType", (int)colorBlindType);
            
            // Network
            PlayerPrefs.SetInt("ServerRegion", (int)serverRegion);
            PlayerPrefs.SetInt("MaxPing", maxPing);
            PlayerPrefs.SetInt("AutoReconnect", autoReconnect ? 1 : 0);
            
            // Accessibility
            PlayerPrefs.SetInt("EnableSubtitles", enableSubtitles ? 1 : 0);
            PlayerPrefs.SetInt("SubtitleSize", subtitleSize);
            PlayerPrefs.SetInt("EnableHighContrast", enableHighContrast ? 1 : 0);
            PlayerPrefs.SetInt("EnableScreenShake", enableScreenShake ? 1 : 0);
            PlayerPrefs.SetInt("ReduceMotion", reduceMotion ? 1 : 0);
            
            PlayerPrefs.Save();
            Debug.Log("💾 Game configuration saved!");
        }
        
        public void ResetToDefaults()
        {
            graphicsQuality = GraphicsQuality.High;
            resolutionWidth = 1920;
            resolutionHeight = 1080;
            fullscreen = true;
            vSync = true;
            frameRateLimit = 60;
            antiAliasing = 2;
            textureQuality = TextureQuality.High;
            shadowQuality = ShadowQuality.High;
            postProcessing = true;
            
            masterVolume = 1.0f;
            sfxVolume = 1.0f;
            musicVolume = 0.8f;
            voiceVolume = 1.0f;
            audioQuality = AudioQuality.High;
            
            mouseSensitivity = 1.0f;
            invertMouseY = false;
            fieldOfView = 90f;
            enableCrosshair = true;
            crosshairSize = 50;
            crosshairColor = "#FFFFFF";
            enableDamageNumbers = true;
            enableHitMarkers = true;
            
            showFPS = true;
            showPing = true;
            uiScale = 1.0f;
            enableColorBlindMode = false;
            colorBlindType = ColorBlindType.None;
            
            serverRegion = ServerRegion.Auto;
            maxPing = 150;
            autoReconnect = true;
            
            enableSubtitles = false;
            subtitleSize = 16;
            enableHighContrast = false;
            enableScreenShake = true;
            reduceMotion = false;
            
            // Reset keybindings to defaults
            ResetKeyBindingsToDefaults();
            
            Debug.Log("🔄 Configuration reset to defaults");
        }
        
        public void ResetKeyBindingsToDefaults()
        {
            keyBindings = new Dictionary<string, KeyCode>
            {
                { "MoveForward", KeyCode.W },
                { "MoveBackward", KeyCode.S },
                { "MoveLeft", KeyCode.A },
                { "MoveRight", KeyCode.D },
                { "Jump", KeyCode.Space },
                { "Crouch", KeyCode.LeftControl },
                { "Sprint", KeyCode.LeftShift },
                { "Fire", KeyCode.Mouse0 },
                { "Aim", KeyCode.Mouse1 },
                { "Reload", KeyCode.R },
                { "Interact", KeyCode.F },
                { "UseItem", KeyCode.Q },
                { "Emote1", KeyCode.Alpha1 },
                { "Emote2", KeyCode.Alpha2 },
                { "Emote3", KeyCode.Alpha3 },
                { "VoiceChat", KeyCode.V },
                { "Pause", KeyCode.Escape },
                { "Scoreboard", KeyCode.Tab },
                { "Weapon1", KeyCode.Alpha1 },
                { "Weapon2", KeyCode.Alpha2 },
                { "Weapon3", KeyCode.Alpha3 },
                { "Weapon4", KeyCode.Alpha4 }
            };
        }
    }

    [System.Serializable]
    public class PlatformSpecificSettings
    {
        public bool isMobile;
        public bool isConsole;
        public bool isPC;
        public bool supportsTouch;
        public bool supportsController;
        public bool supportsMouse;
    }

    // ===== ENUMS FOR CONFIGURATION =====
    public enum GraphicsQuality
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Ultra = 3
    }

    public enum TextureQuality
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Ultra = 3
    }

    public enum ShadowQuality
    {
        Disabled = 0,
        Low = 1,
        Medium = 2,
        High = 3
    }

    public enum AudioQuality
    {
        Low = 0,
        Medium = 1,
        High = 2,
        Ultra = 3
    }

    public enum ColorBlindType
    {
        None = 0,
        Protanopia = 1,
        Deuteranopia = 2,
        Tritanopia = 3
    }

    public enum ServerRegion
    {
        Auto = 0,
        NorthAmerica = 1,
        Europe = 2,
        Asia = 3,
        SouthAmerica = 4,
        Oceania = 5,
        Africa = 6
    }
}