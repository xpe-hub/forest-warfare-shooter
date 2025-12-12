using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Configuración del Juego")]
    public int winScore = 1000;
    public float gameTimeLimit = 600f; // 10 minutos
    public bool gameStarted = false;
    public bool gameEnded = false;
    
    [Header("Estado del Juego")]
    public float currentGameTime = 0f;
    public TeamController winningTeam = null;
    public GameState currentState = GameState.Waiting;
    
    [Header("Referencias")]
    public List<TeamController> teams = new List<TeamController>();
    public List<FlagController> flags = new List<FlagController>();
    public OptimizedForestGenerator forestGenerator;
    
    [Header("UI")]
    public GameObject gameUI;
    public GameObject startScreen;
    public GameObject endScreen;
    
    public enum GameState
    {
        Waiting,
        Playing,
        Paused,
        Ended
    }
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        InitializeGame();
    }
    
    void Update()
    {
        if (gameStarted && !gameEnded)
        {
            UpdateGameTime();
            CheckWinCondition();
            HandlePauseInput();
        }
    }
    
    void InitializeGame()
    {
        // Configurar referencias
        if (forestGenerator == null)
        {
            forestGenerator = FindObjectOfType<OptimizedForestGenerator>();
        }
        
        // Generar bosque si existe el generador
        if (forestGenerator != null)
        {
            forestGenerator.GenerateForest();
        }
        
        // Encontrar banderas en la escena
        flags = FindObjectsOfType<FlagController>().ToList();
        
        // Configurar pantallas UI
        if (startScreen != null) startScreen.SetActive(true);
        if (gameUI != null) gameUI.SetActive(false);
        if (endScreen != null) endScreen.SetActive(false);
        
        Debug.Log("GameManager inicializado");
    }
    
    public void StartGame()
    {
        if (currentState != GameState.Waiting) return;
        
        currentState = GameState.Playing;
        gameStarted = true;
        currentGameTime = 0f;
        
        // Spawnear jugadores en sus spawn points
        SpawnAllPlayers();
        
        // Configurar UI
        if (startScreen != null) startScreen.SetActive(false);
        if (gameUI != null) gameUI.SetActive(true);
        
        Debug.Log("¡Juego iniciado!");
    }
    
    void SpawnAllPlayers()
    {
        // Encontrar todos los jugadores en la escena
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        
        foreach (GameObject player in players)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null && playerController.currentTeam != null)
            {
                Transform spawnPoint = playerController.currentTeam.GetSpawnPoint();
                if (spawnPoint != null)
                {
                    player.transform.position = spawnPoint.position;
                    player.transform.rotation = spawnPoint.rotation;
                }
            }
        }
    }
    
    void UpdateGameTime()
    {
        currentGameTime += Time.deltaTime;
        
        // Verificar límite de tiempo
        if (currentGameTime >= gameTimeLimit)
        {
            EndGameByTimeLimit();
        }
    }
    
    void CheckWinCondition()
    {
        if (teams.Count == 0) return;
        
        // Verificar puntuación
        foreach (TeamController team in teams)
        {
            if (team.teamScore >= winScore)
            {
                EndGame(team);
                return;
            }
        }
        
        // Verificar si solo queda un equipo con jugadores
        List<TeamController> teamsWithPlayers = teams.Where(t => t.GetAlivePlayerCount() > 0).ToList();
        
        if (teamsWithPlayers.Count == 1)
        {
            EndGame(teamsWithPlayers[0]);
        }
    }
    
    public void OnFlagCaptured(FlagController flag, TeamController capturingTeam)
    {
        // Notificar captura de bandera
        capturingTeam.CaptureFlag(flag);
        
        Debug.Log($"Bandera {flag.name} capturada por {capturingTeam.teamName}");
        
        // Verificar condición de victoria
        CheckWinCondition();
    }
    
    void EndGame(TeamController winner)
    {
        if (gameEnded) return;
        
        gameEnded = true;
        winningTeam = winner;
        currentState = GameState.Ended;
        gameStarted = false;
        
        // Mostrar pantalla final
        if (endScreen != null) endScreen.SetActive(true);
        if (gameUI != null) gameUI.SetActive(false);
        
        Debug.Log($"¡Juego terminado! Ganador: {winner.teamName}");
    }
    
    void EndGameByTimeLimit()
    {
        // Determinar ganador por puntuación
        TeamController winner = teams.OrderByDescending(t => t.teamScore).FirstOrDefault();
        EndGame(winner);
    }
    
    void HandlePauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    public void TogglePause()
    {
        if (currentState == GameState.Playing)
        {
            currentState = GameState.Paused;
            Time.timeScale = 0f;
            Debug.Log("Juego pausado");
        }
        else if (currentState == GameState.Paused)
        {
            currentState = GameState.Playing;
            Time.timeScale = 1f;
            Debug.Log("Juego reanudado");
        }
    }
    
    public void RegisterTeam(TeamController team)
    {
        if (!teams.Contains(team))
        {
            teams.Add(team);
        }
    }
    
    public void UnregisterTeam(TeamController team)
    {
        if (teams.Contains(team))
        {
            teams.Remove(team);
        }
    }
    
    public void RestartGame()
    {
        // Reiniciar estado del juego
        currentState = GameState.Waiting;
        gameStarted = false;
        gameEnded = false;
        winningTeam = null;
        currentGameTime = 0f;
        Time.timeScale = 1f;
        
        // Reiniciar equipos
        foreach (TeamController team in teams)
        {
            team.teamScore = 0;
            team.capturedFlags.Clear();
        }
        
        // Reiniciar banderas
        foreach (FlagController flag in flags)
        {
            flag.ReturnToBase();
        }
        
        // Configurar UI
        if (startScreen != null) startScreen.SetActive(true);
        if (gameUI != null) gameUI.SetActive(false);
        if (endScreen != null) endScreen.SetActive(false);
        
        Debug.Log("Juego reiniciado");
    }
    
    // Métodos de utilidad para UI
    public string GetFormattedTime()
    {
        float remainingTime = Mathf.Max(0, gameTimeLimit - currentGameTime);
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        return $"{minutes:00}:{seconds:00}";
    }
    
    public List<TeamController> GetTeamsOrderedByScore()
    {
        return teams.OrderByDescending(t => t.teamScore).ToList();
    }
    
    public TeamController GetPlayerTeam(GameObject player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        return playerController?.currentTeam;
    }
}