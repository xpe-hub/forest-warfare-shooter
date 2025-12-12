using UnityEngine;
using System.Collections.Generic;

public class TeamController : MonoBehaviour
{
    [Header("Información del Equipo")]
    public string teamName = "Equipo";
    public Color teamColor = Color.red;
    public int teamScore = 0;
    public int playerLimit = 5;
    
    [Header("Configuración de Banderas")]
    public FlagController homeFlag;
    public List<FlagController> capturedFlags = new List<FlagController>();
    
    [Header("Spawn Points")]
    public Transform[] spawnPoints;
    
    [Header("Configuración")]
    public bool isPlayerTeam = true;
    public int teamIndex = 0;
    
    private List<GameObject> teamPlayers = new List<GameObject>();
    private int currentSpawnIndex = 0;
    
    void Start()
    {
        // Registrar en el GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterTeam(this);
        }
    }
    
    public bool CanAddPlayer()
    {
        return teamPlayers.Count < playerLimit;
    }
    
    public void AddPlayer(GameObject player)
    {
        if (CanAddPlayer() && !teamPlayers.Contains(player))
        {
            teamPlayers.Add(player);
            
            // Asignar color del equipo al jugador
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.SetTeam(this);
            }
        }
    }
    
    public void RemovePlayer(GameObject player)
    {
        if (teamPlayers.Contains(player))
        {
            teamPlayers.Remove(player);
        }
    }
    
    public Transform GetSpawnPoint()
    {
        if (spawnPoints.Length == 0)
            return transform;
            
        Transform spawnPoint = spawnPoints[currentSpawnIndex];
        currentSpawnIndex = (currentSpawnIndex + 1) % spawnPoints.Length;
        
        return spawnPoint;
    }
    
    public void AddScore(int points)
    {
        teamScore += points;
        Debug.Log($"{teamName} ganó {points} puntos. Total: {teamScore}");
        
        // Verificar condición de victoria
        if (GameManager.Instance != null)
        {
            GameManager.Instance.CheckWinCondition();
        }
    }
    
    public void CaptureFlag(FlagController flag)
    {
        if (!capturedFlags.Contains(flag))
        {
            capturedFlags.Add(flag);
            AddScore(100); // Puntos por capturar bandera
            
            Debug.Log($"{teamName} capturó la bandera {flag.name}");
        }
    }
    
    public void LoseFlag(FlagController flag)
    {
        if (capturedFlags.Contains(flag))
        {
            capturedFlags.Remove(flag);
            Debug.Log($"{teamName} perdió la bandera {flag.name}");
        }
    }
    
    public int GetTotalFlags()
    {
        return 1 + capturedFlags.Count; // +1 por la bandera base
    }
    
    public List<GameObject> GetAlivePlayers()
    {
        List<GameObject> alivePlayers = new List<GameObject>();
        
        foreach (GameObject player in teamPlayers)
        {
            if (player != null)
            {
                PlayerController playerController = player.GetComponent<PlayerController>();
                if (playerController != null && playerController.IsAlive())
                {
                    alivePlayers.Add(player);
                }
            }
        }
        
        return alivePlayers;
    }
    
    public int GetPlayerCount()
    {
        return teamPlayers.Count;
    }
    
    public int GetAlivePlayerCount()
    {
        return GetAlivePlayers().Count;
    }
    
    // Método para obtener información del equipo
    public string GetTeamInfo()
    {
        return $"{teamName}: {GetAlivePlayerCount()}/{GetPlayerCount()} jugadores vivos, {teamScore} puntos, {capturedFlags.Count} banderas capturadas";
    }
    
    void OnDestroy()
    {
        // Desregistrar del GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.UnregisterTeam(this);
        }
    }
}