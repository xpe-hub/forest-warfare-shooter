using UnityEngine;

public class FlagController : MonoBehaviour
{
    [Header("Configuración de Bandera")]
    public TeamController owningTeam;
    public bool isCaptured = false;
    public float captureRadius = 2f;
    public float captureTime = 5f; // Tiempo en segundos para capturar
    public float autoReturnTime = 30f; // Tiempo para retorno automático
    
    [Header("Visual")]
    public Transform flagMesh;
    public Color teamColor = Color.red;
    
    [Header("Sonido")]
    public AudioClip captureSound;
    public AudioClip returnSound;
    
    private float captureProgress = 0f;
    private TeamController capturingTeam = null;
    private Vector3 originalPosition;
    private float lastCaptureTime;
    
    void Start()
    {
        originalPosition = transform.position;
        if (flagMesh == null)
            flagMesh = transform;
            
        UpdateFlagColor();
    }
    
    void Update()
    {
        HandleCaptureLogic();
        HandleAutoReturn();
    }
    
    void HandleCaptureLogic()
    {
        // Encontrar jugadores en el radio de captura
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        TeamController nearbyTeam = null;
        int playersCount = 0;
        
        foreach (GameObject player in players)
        {
            if (Vector3.Distance(player.transform.position, transform.position) <= captureRadius)
            {
                TeamController playerTeam = player.GetComponent<PlayerController>()?.currentTeam;
                if (playerTeam != null)
                {
                    if (nearbyTeam == null || nearbyTeam == playerTeam)
                    {
                        nearbyTeam = playerTeam;
                        playersCount++;
                    }
                    else
                    {
                        // Equipos diferentes - cancelar captura
                        captureProgress = 0f;
                        capturingTeam = null;
                        return;
                    }
                }
            }
        }
        
        // Procesar captura si hay suficientes jugadores del mismo equipo
        if (nearbyTeam != null && playersCount >= 1 && nearbyTeam != owningTeam)
        {
            if (capturingTeam == null || capturingTeam != nearbyTeam)
            {
                capturingTeam = nearbyTeam;
                captureProgress = 0f;
            }
            
            captureProgress += Time.deltaTime / captureTime;
            
            // Actualizar visualización de progreso
            UpdateCaptureProgress();
            
            if (captureProgress >= 1f)
            {
                CaptureFlag(nearbyTeam);
            }
        }
        else
        {
            // Cancelar captura si no hay jugadores suficientes o de equipos diferentes
            captureProgress = 0f;
            capturingTeam = null;
        }
    }
    
    void HandleAutoReturn()
    {
        if (isCaptured && Time.time - lastCaptureTime > autoReturnTime)
        {
            ReturnToBase();
        }
    }
    
    void CaptureFlag(TeamController newTeam)
    {
        owningTeam = newTeam;
        isCaptured = true;
        lastCaptureTime = Time.time;
        captureProgress = 0f;
        capturingTeam = null;
        
        UpdateFlagColor();
        
        if (captureSound != null)
        {
            AudioSource.PlayClipAtPoint(captureSound, transform.position);
        }
        
        // Notificar al GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnFlagCaptured(this, newTeam);
        }
    }
    
    void ReturnToBase()
    {
        transform.position = originalPosition;
        isCaptured = false;
        owningTeam = null;
        captureProgress = 0f;
        capturingTeam = null;
        
        UpdateFlagColor();
        
        if (returnSound != null)
        {
            AudioSource.PlayClipAtPoint(returnSound, transform.position);
        }
    }
    
    void UpdateFlagColor()
    {
        if (flagMesh != null)
        {
            Renderer renderer = flagMesh.GetComponent<Renderer>();
            if (renderer != null)
            {
                Color finalColor = isCaptured ? teamColor : Color.gray;
                if (owningTeam != null)
                {
                    finalColor = owningTeam.teamColor;
                }
                renderer.material.color = finalColor;
            }
        }
    }
    
    void UpdateCaptureProgress()
    {
        // Efecto visual para mostrar progreso de captura
        if (flagMesh != null)
        {
            Vector3 originalScale = flagMesh.localScale;
            flagMesh.localScale = originalScale + Vector3.up * 0.1f * Mathf.Sin(Time.time * 10f);
        }
    }
    
    // Método para obtener información de la bandera
    public string GetFlagInfo()
    {
        string status = isCaptured ? $"Capturada por {owningTeam.teamName}" : "Neutral";
        return $"{name}: {status}";
    }
}