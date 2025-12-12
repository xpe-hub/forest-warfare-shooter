/*
 * Forest Warfare Shooter
 * Copyright © 2025 xpe.nettt 👑
 * Todos los derechos reservados
 * 
 * OptimizedPlayerController.cs - Controlador del jugador optimizado para hardware bajo
 * Diseñado específicamente para AMD A4 5400 + 4GB RAM
 * Configuración PC-first, luego adaptación para móviles
 */

using UnityEngine;
using System.Collections.Generic;

namespace TacticalShooter.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class OptimizedPlayerController : MonoBehaviour
    {
        // CONFIGURACIÓN PARA AMD A4 5400 + 4GB RAM
        [Header("Movement Settings")]
        public float walkSpeed = 3f; // Reducido para mejor rendimiento
        public float runSpeed = 5f; // Reducido
        public float crouchSpeed = 2f;
        public float jumpHeight = 2f;
        public float gravity = -15f; // Reducido
        public float crouchHeight = 1f;
        public float standingHeight = 1.8f; // Reducido altura del player
        
        [Header("Mouse Settings")]
        public float mouseSensitivity = 1.5f; // Reducido para fluidez
        public float maxLookAngle = 75f; // Reducido ángulo
        
        [Header("Combat Settings")]
        public int maxHealth = 100;
        public float healthRegeneration = 1f; // Reducido regeneración
        public float timeBetweenShots = 0.15f; // Reducido velocidad disparo
        
        [Header("Performance Settings")]
        public bool enableFrameRateCap = true;
        public int targetFPS = 30; // Cap FPS a 30
        public bool disableShadowCasting = true;
        public bool disableRealtimeLighting = true;
        
        [Header("References")]
        public Transform cameraTransform;
        public CharacterController controller;
        public OptimizedWeaponController weaponController;
        
        // Private variables
        private Vector3 velocity;
        private float cameraPitch = 0f;
        private int currentHealth;
        private bool isDead = false;
        private bool isCrouching = false;
        private float currentSpeed;
        private float lastFrameTime;
        
        // Performance optimization
        private int frameCount;
        private float timeSinceLastFrame = 0f;
        private float frameRateUpdateInterval = 0.5f;
        private float currentFPS;
        
        // Components
        private Camera playerCamera;
        private PlayerStats playerStats;
        private OptimizedTeamController teamController;
        
        void Start()
        {
            OptimizeForLowEndHardware();
            InitializePlayer();
        }
        
        void OptimizeForLowEndHardware()
        {
            // Configurar calidad para hardware bajo
            QualitySettings.SetQualityLevel(0); // Calidad mínima
            
            // Deshabilitar características pesadas
            QualitySettings.shadows = ShadowQuality.Disable;
            QualitySettings.shadowDistance = 0f;
            QualitySettings.antiAliasing = 0;
            
            // Cap FPS para estabilidad
            if (enableFrameRateCap)
            {
                Application.targetFrameRate = targetFPS;
            }
            
            // Configurar memoria
            System.GC.Collect();
        }
        
        void InitializePlayer()
        {
            // Setup camera
            if (cameraTransform == null)
            {
                playerCamera = Camera.main;
                if (playerCamera != null)
                    cameraTransform = playerCamera.transform;
            }
            
            // Setup controller
            if (controller == null)
                controller = GetComponent<CharacterController>();
                
            // Reducir tamaño del collider para mejor rendimiento
            controller.radius = 0.4f; // Reducido de 0.5
            controller.height = standingHeight;
            controller.center = new Vector3(0, standingHeight / 2, 0);
            
            // Get components
            playerStats = GetComponent<PlayerStats>();
            teamController = GetComponent<OptimizedTeamController>();
            
            // Initialize health
            currentHealth = maxHealth;
            
            // Lock cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            // Set spawn position
            if (teamController != null)
            {
                Vector3 spawnPos = teamController.GetTeamSpawnPoint();
                transform.position = spawnPos;
            }
        }
        
        void Update()
        {
            // Performance monitoring
            MonitorPerformance();
            
            if (isDead) return;
            
            HandleMovement();
            HandleMouseLook();
            HandleCrouching(); // TECLA C PARA AGACHARSE
            HandleJumping();
            HandleCombat();
            HandleHealth();
            HandleCarryFlag();
            HandleDeath();
        }
        
        void MonitorPerformance()
        {
            frameCount++;
            timeSinceLastFrame += Time.deltaTime;
            
            if (timeSinceLastFrame >= frameRateUpdateInterval)
            {
                currentFPS = frameCount / timeSinceLastFrame;
                frameCount = 0;
                timeSinceLastFrame = 0f;
                
                // Auto-adjust settings if FPS too low
                if (currentFPS < 20f && !isDead)
                {
                    AutoReduceQuality();
                }
            }
        }
        
        void AutoReduceQuality()
        {
            // Reducir dinámicamente la calidad si el rendimiento baja
            QualitySettings.antiAliasing = 0;
            QualitySettings.shadows = ShadowQuality.Disable;
            
            // Notificar al usuario
            if (GameManager.Instance != null && GameManager.Instance.uiManager != null)
            {
                GameManager.Instance.uiManager.ShowPerformanceWarning();
            }
        }
        
        void HandleMovement()
        {
            // Calculate speed based on state
            currentSpeed = walkSpeed;
            if (Input.GetKey(KeyCode.LeftShift) && !isCrouching)
                currentSpeed = runSpeed;
            if (isCrouching)
                currentSpeed = crouchSpeed;
            
            // Get input
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            
            // Calculate movement direction
            Vector3 direction = transform.right * horizontal + transform.forward * vertical;
            direction = direction.normalized * currentSpeed;
            
            // Apply gravity (simplified)
            if (controller.isGrounded && velocity.y < 0)
                velocity.y = -1f;
            velocity.y += gravity * Time.deltaTime;
            
            // Move player
            Vector3 move = direction + velocity * Time.deltaTime;
            controller.Move(move * Time.deltaTime);
        }
        
        void HandleMouseLook()
        {
            if (cameraTransform == null) return;
            
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;
            
            // Rotate player body (yaw)
            transform.Rotate(Vector3.up * mouseX);
            
            // Rotate camera (pitch)
            cameraPitch -= mouseY;
            cameraPitch = Mathf.Clamp(cameraPitch, -maxLookAngle, maxLookAngle);
            cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        }
        
        void HandleCrouching()
        {
            // CAMBIADO: Ahora usa tecla C en lugar de Ctrl
            if (Input.GetKeyDown(KeyCode.C))
            {
                ToggleCrouch();
            }
        }
        
        void ToggleCrouch()
        {
            isCrouching = !isCrouching;
            controller.height = isCrouching ? crouchHeight : standingHeight;
            controller.center = new Vector3(0, controller.height / 2, 0);
        }
        
        void HandleJumping()
        {
            if (Input.GetButtonDown("Jump") && controller.isGrounded && !isCrouching)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }
        
        void HandleCombat()
        {
            if (weaponController != null)
            {
                bool isFiring = Input.GetButton("Fire1");
                bool isAiming = Input.GetMouseButton(1);
                bool isReloading = Input.GetKeyDown(KeyCode.R);
                weaponController.UpdateWeapon(isFiring, isAiming, isReloading);
            }
        }
        
        void HandleHealth()
        {
            // Regenerate health slowly
            if (currentHealth < maxHealth && !isDead)
            {
                currentHealth += Mathf.RoundToInt(healthRegeneration * Time.deltaTime);
                currentHealth = Mathf.Min(currentHealth, maxHealth);
            }
            UpdateHealthUI();
        }
        
        void HandleCarryFlag()
        {
            // Simplified flag carrying (reduce computations)
            if (weaponController != null && weaponController.carriedFlag != null)
            {
                weaponController.carriedFlag.transform.position = transform.position + Vector3.up * 1.5f;
            }
        }
        
        void HandleDeath()
        {
            if (currentHealth <= 0 && !isDead)
            {
                Die();
            }
        }
        
        void UpdateHealthUI()
        {
            if (GameManager.Instance != null && GameManager.Instance.uiManager != null)
            {
                GameManager.Instance.uiManager.UpdateHealthUI(currentHealth, maxHealth);
            }
        }
        
        void Die()
        {
            isDead = true;
            
            // Drop carried flag
            if (weaponController != null && weaponController.carriedFlag != null)
            {
                weaponController.carriedFlag.DropFlag(transform.position + Vector3.up);
                weaponController.carriedFlag = null;
            }
            
            // Show death screen
            if (GameManager.Instance != null)
            {
                GameManager.Instance.uiManager.ShowDeathScreen();
            }
            
            // Respawn after delay
            Invoke("RespawnPlayer", 5f);
        }
        
        void RespawnPlayer()
        {
            if (teamController != null)
            {
                Vector3 spawnPos = teamController.GetTeamSpawnPoint();
                transform.position = spawnPos;
                currentHealth = maxHealth;
                isDead = false;
                velocity = Vector3.zero;
            }
        }
        
        // Public getters
        public int GetCurrentHealth() => currentHealth;
        public bool IsDead() => isDead;
        public bool IsCrouching() => isCrouching;
        public PlayerStats GetPlayerStats() => playerStats;
        public OptimizedTeamController GetTeamController() => teamController;
        public float GetCurrentFPS() => currentFPS;
        
        // Performance info for UI
        public string GetPerformanceInfo()
        {
            return $"FPS: {currentFPS:F1} | RAM: {System.GC.GetTotalMemory(false) / 1024 / 1024}MB";
        }
    }
}