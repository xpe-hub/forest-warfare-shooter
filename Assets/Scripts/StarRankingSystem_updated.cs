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
        public int starsPerTier = 20;
        
        [Header("Achievements")]
        public Achievement[] achievements;
        public bool enableAchievements = true;
        
        [Header("Progression Rewards")]
        public ProgressionReward[] progressionRewards;
        public bool enableProgressionRewards = true;
        
        // Current player data
        private PlayerProgressionData playerData;
        
        // Star earning tracking
        private int starsEarnedThisMatch = 0;
        private int totalMatches = 0;
        private int currentWinStreak = 0;
        private int longestWinStreak = 0;
        
        // Performance tracking
        private Dictionary<string, int> performanceMetrics = new Dictionary<string, int>();
        private List<MatchResult> recentMatches = new List<MatchResult>();
        
        // Achievement tracking
        private Dictionary<string, bool> unlockedAchievements = new Dictionary<string, bool>();
        private List<Achievement> pendingAchievements = new List<Achievement>();
        
        // Event system
        public System.Action<int> OnStarsEarned;
        public System.Action<RankingTier> OnTierUp;
        public System.Action<Achievement> OnAchievementUnlocked;
        public System.Action<ProgressionReward> OnRewardClaimed;

        void Start()
        {
            InitializeStarRankingSystem();
        }

        void InitializeStarRankingSystem()
        {
            LoadPlayerData();
            InitializeRankingTiers();
            InitializeAchievements();
            InitializeProgressionRewards();
            
            Debug.Log("Star Ranking System initialized");
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
                favoriteGameMode = PlayerPrefs.GetString("FavoriteGameMode", "Conquest")
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

        void InitializeRankingTiers()
        {
            rankingTiers = new RankingTier[]
            {
                new RankingTier { tierName = "Rookie", minStars = 0, maxStars = 19, tierColor = Color.gray, badgeIcon = "rookie_badge" },
                new RankingTier { tierName = "Bronze", minStars = 20, maxStars = 39, tierColor = new Color(0.8f, 0.5f, 0.2f), badgeIcon = "bronze_badge" },
                new RankingTier { tierName = "Silver", minStars = 40, maxStars = 59, tierColor = Color.gray, badgeIcon = "silver_badge" },
                new RankingTier { tierName = "Gold", minStars = 60, maxStars = 79, tierColor = Color.yellow, badgeIcon = "gold_badge" },
                new RankingTier { tierName = "Platinum", minStars = 80, maxStars = 99, tierColor = Color.cyan, badgeIcon = "platinum_badge" },
                new RankingTier { tierName = "Diamond", minStars = 100, maxStars = 119, tierColor = Color.blue, badgeIcon = "diamond_badge" },
                new RankingTier { tierName = "Master", minStars = 120, maxStars = 139, tierColor = new Color(0.6f, 0.2f, 1f), badgeIcon = "master_badge" },
                new RankingTier { tierName = "Grandmaster", minStars = 140, maxStars = 159, tierColor = new Color(1f, 0.4f, 0f), badgeIcon = "grandmaster_badge" },
                new RankingTier { tierName = "Legend", minStars = 160, maxStars = 179, tierColor = new Color(1f, 0.8f, 0f), badgeIcon = "legend_badge" },
                new RankingTier { tierName = "Immortal", minStars = 180, maxStars = 999, tierColor = new Color(1f, 0.2f, 0.2f), badgeIcon = "immortal_badge" }
            };
        }

        void InitializeAchievements()
        {
            achievements = new Achievement[]
            {
                // Match achievements
                new Achievement { id = "first_match", name = "First Steps", description = "Play your first match", icon = "first_steps", rewardStars = 5 },
                new Achievement { id = "ten_matches", name = "Getting Started", description = "Play 10 matches", icon = "ten_matches", rewardStars = 10 },
                new Achievement { id = "fifty_matches", name = "Dedicated Player", description = "Play 50 matches", icon = "fifty_matches", rewardStars = 25 },
                new Achievement { id = "hundred_matches", name = "Veteran", description = "Play 100 matches", icon = "hundred_matches", rewardStars = 50 },
                
                // Win achievements
                new Achievement { id = "first_win", name = "Victory!", description = "Win your first match", icon = "first_win", rewardStars = 10 },
                new Achievement { id = "five_wins", name = "Winning Streak", description = "Win 5 matches", icon = "five_wins", rewardStars = 15 },
                new Achievement { id = "ten_wins", name = "Champion", description = "Win 10 matches", icon = "ten_wins", rewardStars = 25 },
                new Achievement { id = "twenty_five_wins", name = "Conqueror", description = "Win 25 matches", icon = "twenty_five_wins", rewardStars = 50 },
                new Achievement { id = "fifty_wins", name = "Legendary", description = "Win 50 matches", icon = "fifty_wins", rewardStars = 100 },
                
                // Streak achievements
                new Achievement { id = "three_streak", name = "Hot Streak", description = "Win 3 matches in a row", icon = "three_streak", rewardStars = 20 },
                new Achievement { id = "five_streak", name = "Unstoppable", description = "Win 5 matches in a row", icon = "five_streak", rewardStars = 35 },
                new Achievement { id = "ten_streak", name = "Godlike", description = "Win 10 matches in a row", icon = "ten_streak", rewardStars = 75 },
                
                // Performance achievements
                new Achievement { id = "perfect_accuracy", name = "Sharp Shooter", description = "Get 100% accuracy in a match", icon = "perfect_accuracy", rewardStars = 30 },
                new Achievement { id = "most_kills", name = "Elimination Master", description = "Get most kills in 10 matches", icon = "most_kills", rewardStars = 40 },
                new Achievement { id = "flag_capturer", name = "Flag Master", description = "Capture 50 flags", icon = "flag_capturer", rewardStars = 35 },
                
                // Special achievements
                new Achievement { id = "comeback_victory", name = "Against All Odds", description = "Win after being behind", icon = "comeback_victory", rewardStars = 25 },
                new Achievement { id = "clutch_performance", name = "Clutch Master", description = "Win with 1 HP remaining", icon = "clutch_performance", rewardStars = 30 },
                new Achievement { id = "team_player", name = "Team Player", description = "Get 100 team assists", icon = "team_player", rewardStars = 20 }
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
                new ProgressionReward { level = 5, name = "Bronze Weapon Skin", description = "Unlock bronze weapon skin", type = RewardType.WeaponSkin, value = "bronze_rifle" },
                new ProgressionReward { level = 10, name = "Silver Character Skin", description = "Unlock silver character skin", type = RewardType.CharacterSkin, value = "silver_soldier" },
                new ProgressionReward { level = 15, name = "Emote Pack", description = "Unlock victory emote", type = RewardType.Emote, value = "victory_dance" },
                new ProgressionReward { level = 20, name = "Gold Weapon Skin", description = "Unlock gold weapon skin", type = RewardType.WeaponSkin, value = "golden_rifle" },
                new ProgressionReward { level = 25, name = "Exclusive Map", description = "Unlock exclusive battleground", type = RewardType.Map, value = "snowy_forest" },
                new ProgressionReward { level = 30, name = "Platinum Character", description = "Unlock platinum character skin", type = RewardType.CharacterSkin, value = "platinum_warrior" },
                new ProgressionReward { level = 35, name = "Legendary Emote", description = "Unlock legendary celebration", type = RewardType.Emote, value = "legendary_celebration" },
                new ProgressionReward { level = 40, name = "Diamond Weapon", description = "Unlock diamond weapon skin", type = RewardType.WeaponSkin, value = "diamond_rifle" },
                new ProgressionReward { level = 50, name = "Master Title", description = "Unlock Master rank title", type = RewardType.Title, value = "Master_of_Warfare" }
            };
        }

        #region Star Earning System
        public int CalculateStarsEarned(MatchResult result)
        {
            int stars = baseStarReward;
            
            // Base star for participation
            stars += 1;
            
            // Victory bonuses
            if (result.isVictory)
            {
                stars += 2;
            }
            
            // Performance-based stars
            if (result.kills >= 10) stars += 1;
            if (result.accuracy >= 0.8f) stars += 1;
            if (result.flagsCaptured >= 3) stars += 1;
            if (result.objectsCompleted >= 5) stars += 1;
            if (result.teamAssists >= 5) stars += 1;
            
            // Streak bonuses
            if (currentWinStreak >= 3) stars += 1;
            if (currentWinStreak >= 5) stars += 1;
            
            // Difficulty multiplier
            stars = Mathf.RoundToInt(stars * GetDifficultyMultiplier(result.gameMode));
            
            // Special circumstances
            if (result.isClutchPerformance) stars += 1;
            if (result.isComebackVictory) stars += 1;
            if (result.isPerfectGame) stars += 2;
            
            return Mathf.Clamp(stars, 1, maxStarsPerMatch);
        }

        public void AwardStars(int starCount, MatchResult result)
        {
            starsEarnedThisMatch = starCount;
            playerData.totalStars += starCount;
            
            // Update tier if necessary
            CheckTierProgress();
            
            // Check achievements
            CheckAchievements(result);
            
            // Save progress
            SavePlayerData();
            
            // Trigger events
            OnStarsEarned?.Invoke(starCount);
            
            Debug.Log($"Awarded {starCount} stars! Total: {playerData.totalStars}");
        }

        float GetDifficultyMultiplier(string gameMode)
        {
            switch (gameMode.ToLower())
            {
                case "elimination":
                    return 1.3f; // Harder mode = more stars
                case "king_of_hill":
                    return 1.2f;
                case "conquest":
                    return 1.0f; // Standard
                case "bot_practice":
                    return 0.7f; // Easier mode = fewer stars
                default:
                    return 1.0f;
            }
        }
        #endregion

        #region Tier System
        public void CheckTierProgress()
        {
            int newTier = CalculateCurrentTier(playerData.totalStars);
            
            if (newTier > playerData.currentTier)
            {
                RankingTier oldTier = GetCurrentTier();
                playerData.currentTier = newTier;
                RankingTier newTierData = GetCurrentTier();
                
                OnTierUp?.Invoke(newTierData);
                
                Debug.Log($"Tier up! {oldTier.tierName} ➜ {newTierData.tierName}");
            }
        }

        int CalculateCurrentTier(int totalStars)
        {
            for (int i = rankingTiers.Length - 1; i >= 0; i--)
            {
                if (totalStars >= rankingTiers[i].minStars)
                {
                    return i + 1; // Tiers are 1-indexed
                }
            }
            return 1; // Default to first tier
        }

        public RankingTier GetCurrentTier()
        {
            int tierIndex = Mathf.Clamp(playerData.currentTier - 1, 0, rankingTiers.Length - 1);
            return rankingTiers[tierIndex];
        }

        public RankingTier GetNextTier()
        {
            int nextTierIndex = Mathf.Clamp(playerData.currentTier, 0, rankingTiers.Length - 1);
            if (nextTierIndex >= rankingTiers.Length - 1)
            {
                return null; // Already at max tier
            }
            return rankingTiers[nextTierIndex + 1];
        }

        public int GetStarsToNextTier()
        {
            RankingTier nextTier = GetNextTier();
            if (nextTier == null) return 0; // Max tier reached
            
            int currentTierMinStars = GetCurrentTier().minStars;
            return nextTier.minStars - playerData.totalStars + currentTierMinStars;
        }
        #endregion

        #region Achievement System
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
                case "first_match":
                    return totalMatches == 1;
                case "ten_matches":
                    return totalMatches >= 10;
                case "fifty_matches":
                    return totalMatches >= 50;
                case "hundred_matches":
                    return totalMatches >= 100;
                
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
                
                case "three_streak":
                    return currentWinStreak >= 3;
                case "five_streak":
                    return currentWinStreak >= 5;
                case "ten_streak":
                    return currentWinStreak >= 10;
                
                case "perfect_accuracy":
                    return result.accuracy >= 1.0f;
                case "most_kills":
                    return performanceMetrics.ContainsKey("MostKills") && performanceMetrics["MostKills"] >= 10;
                case "flag_capturer":
                    return performanceMetrics.ContainsKey("TotalFlagsCaptured") && performanceMetrics["TotalFlagsCaptured"] >= 50;
                
                case "comeback_victory":
                    return result.isComebackVictory;
                case "clutch_performance":
                    return result.isClutchPerformance;
                case "team_player":
                    return result.teamAssists >= 100;
                
                default:
                    return false;
            }
        }

        void UnlockAchievement(Achievement achievement)
        {
            unlockedAchievements[achievement.id] = true;
            playerData.totalStars += achievement.rewardStars;
            
            // Save unlocked achievements
            SaveUnlockedAchievements();
            
            OnAchievementUnlocked?.Invoke(achievement);
            
            Debug.Log($"Achievement unlocked: {achievement.name} (+{achievement.rewardStars} stars)");
        }

        public List<Achievement> GetUnlockedAchievements()
        {
            return achievements.Where(a => unlockedAchievements[a.id]).ToList();
        }

        public List<Achievement> GetLockedAchievements()
        {
            return achievements.Where(a => !unlockedAchievements[a.id]).ToList();
        }
        #endregion

        #region Progression Rewards
        public void CheckProgressionRewards()
        {
            int playerLevel = CalculatePlayerLevel();
            
            foreach (ProgressionReward reward in progressionRewards)
            {
                if (reward.level <= playerLevel && !reward.isClaimed)
                {
                    reward.isClaimed = true;
                    OnRewardClaimed?.Invoke(reward);
                    
                    Debug.Log($"Progression reward unlocked: {reward.name}");
                }
            }
        }

        int CalculatePlayerLevel()
        {
            // Level calculation based on total stars
            return Mathf.FloorToInt(playerData.totalStars / 10f) + 1;
        }

        public void ClaimProgressionReward(ProgressionReward reward)
        {
            if (reward.isClaimed) return;
            
            reward.isClaimed = true;
            
            // Apply reward
            ApplyProgressionReward(reward);
            
            OnRewardClaimed?.Invoke(reward);
        }

        void ApplyProgressionReward(ProgressionReward reward)
        {
            switch (reward.type)
            {
                case RewardType.WeaponSkin:
                    // Unlock weapon skin
                    PlayerPrefs.SetInt($"WeaponSkin_{reward.value}", 1);
                    break;
                case RewardType.CharacterSkin:
                    // Unlock character skin
                    PlayerPrefs.SetInt($"CharacterSkin_{reward.value}", 1);
                    break;
                case RewardType.Emote:
                    // Unlock emote
                    PlayerPrefs.SetInt($"Emote_{reward.value}", 1);
                    break;
                case RewardType.Title:
                    // Unlock title
                    PlayerPrefs.SetString("UnlockedTitle", reward.value);
                    break;
                case RewardType.Map:
                    // Unlock map
                    PlayerPrefs.SetInt($"Map_{reward.value}", 1);
                    break;
            }
            
            PlayerPrefs.Save();
        }
        #endregion

        #region Match Result Processing
        public void ProcessMatchResult(MatchResult result)
        {
            // Update match tracking
            totalMatches++;
            recentMatches.Add(result);
            
            // Keep only last 20 matches
            if (recentMatches.Count > 20)
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
            
            // Update performance metrics
            UpdatePerformanceMetrics(result);
            
            // Calculate and award stars
            int earnedStars = CalculateStarsEarned(result);
            AwardStars(earnedStars, result);
            
            // Update favorite game mode
            UpdateFavoriteGameMode(result.gameMode);
            
            // Save all data
            SavePlayerData();
            SaveRecentMatches();
            
            Debug.Log($"Match processed - Stars: {earnedStars}, Tier: {GetCurrentTier().tierName}");
        }

        void UpdatePerformanceMetrics(MatchResult result)
        {
            // Update various performance metrics
            if (!performanceMetrics.ContainsKey("TotalKills"))
                performanceMetrics["TotalKills"] = 0;
            if (!performanceMetrics.ContainsKey("TotalDeaths"))
                performanceMetrics["TotalDeaths"] = 0;
            if (!performanceMetrics.ContainsKey("TotalFlagsCaptured"))
                performanceMetrics["TotalFlagsCaptured"] = 0;
            if (!performanceMetrics.ContainsKey("MostKills"))
                performanceMetrics["MostKills"] = 0;
            
            performanceMetrics["TotalKills"] += result.kills;
            performanceMetrics["TotalDeaths"] += result.deaths;
            performanceMetrics["TotalFlagsCaptured"] += result.flagsCaptured;
            
            if (result.kills > performanceMetrics["MostKills"])
            {
                performanceMetrics["MostKills"] = result.kills;
            }
        }

        void UpdateFavoriteGameMode(string gameMode)
        {
            // Simple favorite game mode calculation
            // In a real implementation, you'd track frequency of each mode
            if (playerData.favoriteGameMode == "Conquest")
            {
                // Keep as conquest if player plays it frequently
            }
        }
        #endregion

        #region Data Persistence
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

        #region Public Getters
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
        #endregion
    }

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
    }

    [System.Serializable]
    public class RankingTier
    {
        public string tierName;
        public int minStars;
        public int maxStars;
        public Color tierColor;
        public string badgeIcon;
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
        public int teamAssists;
    }

    public enum RewardType
    {
        WeaponSkin,
        CharacterSkin,
        Emote,
        Title,
        Map
    }
}