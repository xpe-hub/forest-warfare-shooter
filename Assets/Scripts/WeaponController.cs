using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [Header("Configuración del Arma")]
    public string weaponName = "Rifle";
    public float damage = 25f;
    public float range = 100f;
    public float fireRate = 1f; // Disparos por segundo
    public float reloadTime = 2f;
    public int maxAmmo = 30;
    public int maxMagazineSize = 30;
    
    [Header("Efectos Visuales")]
    public Transform muzzleTransform;
    public GameObject muzzleFlash;
    public GameObject bulletHolePrefab;
    public ParticleSystem bulletTrail;
    
    [Header("Sonidos")]
    public AudioClip fireSound;
    public AudioClip reloadSound;
    public AudioClip emptyClipSound;
    
    [Header("Optimización")]
    public LayerMask hitMask = -1; // Todas las capas por defecto
    public bool enableBulletDrop = false;
    public float bulletDropForce = 0.1f;
    
    private int currentAmmo;
    private int magazineAmmo;
    private bool isReloading = false;
    private float nextFireTime = 0f;
    private Camera playerCamera;
    private AudioSource audioSource;
    
    void Start()
    {
        currentAmmo = maxAmmo;
        magazineAmmo = maxMagazineSize;
        playerCamera = Camera.main;
        audioSource = GetComponent<AudioSource>();
        
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    
    void Update()
    {
        if (isReloading) return;
        
        // Manejo de disparo automático/precisión
        if (Input.GetButton("Fire1"))
        {
            TryFire();
        }
        
        // Recarga manual
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartReload();
        }
    }
    
    void TryFire()
    {
        if (Time.time < nextFireTime) return;
        if (magazineAmmo <= 0)
        {
            // Sonido de tambor vacío
            if (emptyClipSound != null)
            {
                audioSource.PlayOneShot(emptyClipSound);
            }
            return;
        }
        
        Fire();
        nextFireTime = Time.time + 1f / fireRate;
    }
    
    void Fire()
    {
        // Reducir munición
        magazineAmmo--;
        
        // Sonido de disparo
        if (fireSound != null)
        {
            audioSource.PlayOneShot(fireSound);
        }
        
        // Efectos visuales
        if (muzzleFlash != null)
        {
            muzzleFlash.SetActive(true);
            Invoke("HideMuzzleFlash", 0.1f);
        }
        
        if (bulletTrail != null)
        {
            bulletTrail.Play();
        }
        
        // Raycast para impacto
        RaycastHit hit;
        Vector3 fireDirection = playerCamera.transform.forward;
        
        // Simular caída de bala si está habilitada
        if (enableBulletDrop)
        {
            fireDirection += Vector3.down * bulletDropForce * Vector3.Distance(transform.position, playerCamera.transform.position);
        }
        
        if (Physics.Raycast(playerCamera.transform.position, fireDirection, out hit, range, hitMask))
        {
            // Impacto detectado
            HandleHit(hit);
            
            // Crear agujero de bala
            if (bulletHolePrefab != null)
            {
                GameObject bulletHole = Instantiate(bulletHolePrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(bulletHole, 5f);
            }
        }
        
        // Auto-recarga si el cargador está vacío
        if (magazineAmmo <= 0 && currentAmmo > 0)
        {
            Invoke("StartReload", 0.5f);
        }
    }
    
    void HandleHit(RaycastHit hit)
    {
        // Verificar si hit es un jugador
        PlayerController playerController = hit.collider.GetComponent<PlayerController>();
        if (playerController != null)
        {
            // Verificar si es un enemigo
            TeamController hitTeam = playerController.currentTeam;
            TeamController shooterTeam = GetComponentInParent<PlayerController>()?.currentTeam;
            
            if (hitTeam != shooterTeam)
            {
                playerController.TakeDamage(damage);
                Debug.Log($"Jugador {playerController.name} recibió {damage} de daño");
            }
        }
        
        // Efectos adicionales según el material
        Renderer hitRenderer = hit.collider.GetComponent<Renderer>();
        if (hitRenderer != null)
        {
            // Crear partículas de impacto
            GameObject impactEffect = new GameObject("ImpactEffect");
            impactEffect.transform.position = hit.point;
            impactEffect.transform.rotation = Quaternion.LookRotation(hit.normal);
            
            ParticleSystem particles = impactEffect.AddComponent<ParticleSystem>();
            var main = particles.main;
            main.startColor = Color.yellow;
            main.startSize = 0.1f;
            main.startLifetime = 0.5f;
            
            particles.Play();
            Destroy(impactEffect, 1f);
        }
    }
    
    void HideMuzzleFlash()
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.SetActive(false);
        }
    }
    
    public void StartReload()
    {
        if (isReloading || currentAmmo <= 0 || magazineAmmo >= maxMagazineSize) return;
        
        isReloading = true;
        Debug.Log("Recargando...");
        
        if (reloadSound != null)
        {
            audioSource.PlayOneShot(reloadSound);
        }
        
        Invoke("CompleteReload", reloadTime);
    }
    
    void CompleteReload()
    {
        int ammoNeeded = maxMagazineSize - magazineAmmo;
        int ammoToReload = Mathf.Min(ammoNeeded, currentAmmo);
        
        currentAmmo -= ammoToReload;
        magazineAmmo += ammoToReload;
        
        isReloading = false;
        Debug.Log($"Recarga completa. Munición actual: {currentAmmo}, Cargador: {magazineAmmo}");
    }
    
    public int GetCurrentAmmo()
    {
        return currentAmmo;
    }
    
    public int GetMagazineAmmo()
    {
        return magazineAmmo;
    }
    
    public bool IsReloading()
    {
        return isReloading;
    }
    
    // Método para recargar sin tiempo (para desarrollo/testing)
    public void InstantReload()
    {
        currentAmmo = maxAmmo;
        magazineAmmo = maxMagazineSize;
        isReloading = false;
    }
}