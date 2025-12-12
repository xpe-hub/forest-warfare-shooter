/*
 * Forest Warfare Shooter
 * Copyright © 2025 xpe.nettt (XPE Games) 🌐
 * AnimationController.cs - Sistema de animaciones fluidas y expresivas
 * Inspirado en juegos como Fall Guys - animaciones divertidas y dinámicas
 */

// System namespaces
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Namespace declaration
namespace TacticalShooter.Animation
{
    public class AnimationController : MonoBehaviour
    {
        [Header("Animation Settings")]
        public Animator playerAnimator;
        public float animationSpeed = 1f;
        public bool enableFootstepAnimations = true;
        public bool enableReactiveAnimations = true;
        public bool enableGestureSystem = true;

        [Header("Animation Layers")]
        public int baseLayerIndex = 0;   // Walking, running, jumping
        public int combatLayerIndex = 1; // Shooting, aiming
        public int gestureLayerIndex = 2; // Victory, defeat, celebrations
        public int expressionLayerIndex = 3; // Emotes, reactions

        [Header("Special Animations")]
        public bool enableVictoryDances = true;
        public bool enableDefeatReactions = true;
        public bool enableDeathAnimations = true;
        public bool enableWeaponAnimations = true;

        [Header("Procedural Animations")]
        public bool enableProceduralIdle = true;
        public bool enableWeaponBob = true;
        public bool enableBreathing = true;
        public bool enableHeadTracking = true;

        // Animation state tracking
        private AnimatorStateInfo currentState;
        private float animationBlendValue = 0f;
        private bool isGrounded = true;
        private bool isMoving = false;
        private bool isRunning = false;
        private bool isCrouching = false;
        private bool isAiming = false;
        private bool isShooting = false;
        private bool isReloading = false;
        private bool isDead = false;

        // Procedural animation variables
        private Vector3 originalPosition;
        private float breathingTimer = 0f;
        private float bobTimer = 0f;
        private float idleTimer = 0f;
        private float gestureTimer = 0f;

        // Animation curves and variables
        private float walkBobAmount = 0.05f;
        private float runBobAmount = 0.1f;
        private float breathingAmount = 0.02f;
        private float headTrackingSpeed = 5f;

        // Special animation tracking
        private bool playingVictoryAnimation = false;
        private bool playingDefeatAnimation = false;
        private bool playingDeathAnimation = false;
        private Queue<GestureAnimation> gestureQueue = new Queue<GestureAnimation>();

        // Component references
        private PlayerController playerController;
        private WeaponController weaponController;
        private Transform headTransform;
        private Transform weaponTransform;

        // Animation parameter names
        private readonly string SPEED_PARAM = "Speed";
        private readonly string GROUNDED_PARAM = "IsGrounded";
        private readonly string MOVING_PARAM = "IsMoving";
        private readonly string RUNNING_PARAM = "IsRunning";
        private readonly string CROUCHING_PARAM = "IsCrouching";
        private readonly string AIMING_PARAM = "IsAiming";
        private readonly string SHOOTING_PARAM = "IsShooting";
        private readonly string RELOADING_PARAM = "IsReloading";
        private readonly string DEAD_PARAM = "IsDead";
        private readonly string VICTORY_PARAM = "Victory";
        private readonly string DEFEAT_PARAM = "Defeat";
        private readonly string GESTURE_PARAM = "Gesture";
        private readonly string EMOTE_PARAM = "Emote";

        void Start()
        {
            InitializeAnimationController();
        }

        void InitializeAnimationController()
        {
            // Get references
            playerController = GetComponent<PlayerController>();
            weaponController = GetComponentInChildren<WeaponController>();

            // Find animator
            if (playerAnimator == null)
            {
                playerAnimator = GetComponent<Animator>();
                if (playerAnimator == null)
                {
                    playerAnimator = GetComponentInChildren<Animator>();
                }
            }

            // Setup original position
            originalPosition = transform.position;

            // Setup layer weights
            SetupAnimationLayers();

            // Find head and weapon transforms
            SetupTransforms();

            Debug.Log("Animation Controller initialized");
        }

        void SetupAnimationLayers()
        {
            if (playerAnimator == null) return;

            // Set initial layer weights
            playerAnimator.SetLayerWeight(baseLayerIndex, 1f);
            playerAnimator.SetLayerWeight(combatLayerIndex, 0f);
            playerAnimator.SetLayerWeight(gestureLayerIndex, 0f);
            playerAnimator.SetLayerWeight(expressionLayerIndex, 0f);
        }

        void SetupTransforms()
        {
            // Find head transform
            Transform[] childTransforms = GetComponentsInChildren<Transform>();
            foreach (Transform t in childTransforms)
            {
                if (t.name.ToLower().Contains("head") || t.name.ToLower().Contains("camera"))
                {
                    headTransform = t;
                    break;
                }
            }

            // Find weapon transform
            if (weaponController != null)
            {
                weaponTransform = weaponController.transform;
            }
        }

        void Update()
        {
            if (playerAnimator == null) return;

            UpdateAnimationState();
            UpdateProceduralAnimations();
            ProcessGestureQueue();
            UpdateSpecialAnimations();
            UpdateCombatAnimations();
        }

        void UpdateAnimationState()
        {
            // Get player state from controller
            if (playerController != null)
            {
                isGrounded = playerController.IsGrounded();
                isMoving = playerController.IsMoving();
                isRunning = playerController.IsRunning();
                isCrouching = playerController.IsCrouching();
                isDead = playerController.IsDead();
            }

            // Get weapon state
            if (weaponController != null)
            {
                isAiming = weaponController.IsAiming();
                isShooting = weaponController.IsShooting();
                isReloading = weaponController.IsReloading();
            }

            // Calculate animation blend value
            float targetBlend = 0f;
            if (isMoving)
            {
                targetBlend = isRunning ? 1f : 0.5f;
            }

            animationBlendValue = Mathf.Lerp(animationBlendValue, targetBlend, Time.deltaTime * 5f);

            // Set animator parameters
            playerAnimator.SetFloat(SPEED_PARAM, animationBlendValue);
            playerAnimator.SetBool(GROUNDED_PARAM, isGrounded);
            playerAnimator.SetBool(MOVING_PARAM, isMoving);
            playerAnimator.SetBool(RUNNING_PARAM, isRunning);
            playerAnimator.SetBool(CROUCHING_PARAM, isCrouching);
            playerAnimator.SetBool(AIMING_PARAM, isAiming);
            playerAnimator.SetBool(SHOOTING_PARAM, isShooting);
            playerAnimator.SetBool(RELOADING_PARAM, isReloading);
            playerAnimator.SetBool(DEAD_PARAM, isDead);
        }

        void UpdateProceduralAnimations()
        {
            // Breathing animation
            if (enableBreathing && !isDead)
            {
                breathingTimer += Time.deltaTime;
                float breathingOffset = Mathf.Sin(breathingTimer * 0.8f) * breathingAmount;

                if (headTransform != null)
                {
                    Vector3 headPosition = headTransform.localPosition;
                    headPosition.y = 1.6f + breathingOffset;
                    headTransform.localPosition = headPosition;
                }
            }

            // Weapon bob animation
            if (enableWeaponBob && weaponTransform != null && isMoving && !isAiming)
            {
                bobTimer += Time.deltaTime * (isRunning ? 2f : 1f);
                float bobAmount = isRunning ? runBobAmount : walkBobAmount;
                float bobOffset = Mathf.Sin(bobTimer * 10f) * bobAmount;

                Vector3 weaponPosition = weaponTransform.localPosition;
                weaponPosition.y = bobOffset;
                weaponTransform.localPosition = weaponPosition;
            }

            // Idle animations
            if (enableProceduralIdle && !isMoving && !isAiming && !isDead)
            {
                idleTimer += Time.deltaTime;

                // Occasional idle gestures
                if (idleTimer > 30f) // Every 30 seconds
                {
                    PlayRandomIdleGesture();
                    idleTimer = 0f;
                }
            }

            // Head tracking
            if (enableHeadTracking && headTransform != null)
            {
                UpdateHeadTracking();
            }
        }

        void UpdateHeadTracking()
        {
            // Simple head tracking - look at nearest enemy or look forward
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            Transform nearestEnemy = null;
            float closestDistance = float.MaxValue;

            foreach (GameObject enemy in enemies)
            {
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance && distance < 10f)
                {
                    closestDistance = distance;
                    nearestEnemy = enemy.transform;
                }
            }

            if (nearestEnemy != null && !isDead)
            {
                // Look at enemy
                Vector3 direction = (nearestEnemy.position - headTransform.position).normalized;
                Quaternion targetRotation = Quaternion.LookRotation(direction);

                headTransform.rotation = Quaternion.Slerp(headTransform.rotation, targetRotation, Time.deltaTime * headTrackingSpeed);
            }
        }

        void ProcessGestureQueue()
        {
            if (gestureQueue.Count > 0)
            {
                GestureAnimation currentGesture = gestureQueue.Peek();

                // Check if current gesture is playing
                if (!currentGesture.isPlaying)
                {
                    // Start the gesture
                    StartCoroutine(PlayGestureAnimation(currentGesture));
                }
            }
        }

        void UpdateSpecialAnimations()
        {
            // Victory animations
            if (enableVictoryDances && playingVictoryAnimation)
            {
                // Victory dance logic is handled in coroutine
            }

            // Defeat animations
            if (enableDefeatReactions && playingDefeatAnimation)
            {
                // Defeat reaction logic is handled in coroutine
            }

            // Death animations
            if (enableDeathAnimations && playingDeathAnimation)
            {
                // Death animation logic is handled in coroutine
            }
        }

        void UpdateCombatAnimations()
        {
            // Combat layer weight management
            float combatWeight = 0f;

            if (isAiming) combatWeight = 0.5f;
            if (isShooting) combatWeight = 0.8f;
            if (isReloading) combatWeight = 1f;

            playerAnimator.SetLayerWeight(combatLayerIndex, combatWeight);

            // Weapon-specific animations
            if (enableWeaponAnimations && weaponController != null)
            {
                UpdateWeaponAnimations();
            }
        }

        void UpdateWeaponAnimations()
        {
            // Weapon reload animations
            if (isReloading)
            {
                // Play reload animation
                playerAnimator.SetTrigger("Reload");
            }

            // Weapon shoot animations
            if (isShooting)
            {
                // Play shoot animation
                playerAnimator.SetTrigger("Shoot");
            }
        }

        #region Gesture System
        public void PlayGesture(GestureType gestureType, float duration = 3f)
        {
            GestureAnimation gesture = new GestureAnimation
            {
                type = gestureType,
                duration = duration,
                isPlaying = false
            };

            gestureQueue.Enqueue(gesture);
        }

        public void PlayRandomIdleGesture()
        {
            GestureType[] idleGestures = {
                GestureType.LookAround,
                GestureType.Shrug,
                GestureType.Wave,
                GestureType.ThumbsUp,
                GestureType.Point
            };

            GestureType randomGesture = idleGestures[Random.Range(0, idleGestures.Length)];
            PlayGesture(randomGesture, 2f);
        }

        public void PlayVictoryAnimation()
        {
            if (!enableVictoryDances) return;

            playingVictoryAnimation = true;
            StartCoroutine(PlayVictorySequence());
        }

        public void PlayDefeatAnimation()
        {
            if (!enableDefeatReactions) return;

            playingDefeatAnimation = true;
            StartCoroutine(PlayDefeatSequence());
        }

        public void PlayDeathAnimation()
        {
            if (!enableDeathAnimations) return;

            playingDeathAnimation = true;
            StartCoroutine(PlayDeathSequence());
        }

        IEnumerator PlayGestureAnimation(GestureAnimation gesture)
        {
            gesture.isPlaying = true;

            // Play gesture animation based on type
            switch (gesture.type)
            {
                case GestureType.Wave:
                    playerAnimator.SetTrigger("Wave");
                    break;
                case GestureType.ThumbsUp:
                    playerAnimator.SetTrigger("ThumbsUp");
                    break;
                case GestureType.Point:
                    playerAnimator.SetTrigger("Point");
                    break;
                case GestureType.Shrug:
                    playerAnimator.SetTrigger("Shrug");
                    break;
                case GestureType.Dance:
                    playerAnimator.SetTrigger("Dance");
                    break;
                case GestureType.Victory:
                    playerAnimator.SetTrigger("Victory");
                    break;
                case GestureType.Celebrate:
                    playerAnimator.SetTrigger("Celebrate");
                    break;
                case GestureType.LookAround:
                    playerAnimator.SetTrigger("LookAround");
                    break;
            }

            // Wait for animation duration
            yield return new WaitForSeconds(gesture.duration);

            // Remove from queue
            gestureQueue.Dequeue();
        }

        IEnumerator PlayVictorySequence()
        {
            // Victory dance sequence
            playerAnimator.SetTrigger("Victory");
            yield return new WaitForSeconds(1f);

            // Random victory gestures
            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(0.5f);
                PlayGesture(GestureType.Shrug, 2f);
            }

            playingVictoryAnimation = false;
        }

        IEnumerator PlayDefeatSequence()
        {
            // Defeat reaction sequence
            playerAnimator.SetTrigger("Defeat");
            yield return new WaitForSeconds(1f);

            // Sad gesture
            yield return new WaitForSeconds(0.5f);
            PlayGesture(GestureType.Shrug, 2f);

            playingDefeatAnimation = false;
        }

        IEnumerator PlayDeathSequence()
        {
            // Death animation sequence
            playerAnimator.SetTrigger("Death");
            yield return new WaitForSeconds(0.5f);

            // Death pose
            playerAnimator.SetBool("Dead", true);

            playingDeathAnimation = false;
        }

        void PlayRandomGesture()
        {
            GestureType[] gestures = {
                GestureType.Wave,
                GestureType.ThumbsUp,
                GestureType.Point,
                GestureType.Celebrate
            };

            GestureType randomGesture = gestures[Random.Range(0, gestures.Length)];
            PlayGesture(randomGesture, 1.5f);
        }
        #endregion

        #region Animation Event Callbacks
        public void OnFootstep()
        {
            if (enableFootstepAnimations)
            {
                // Play footstep sound and dust effect
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayFootstepSound();
                }

                // Create dust effect
                CreateFootstepEffect();
            }
        }

        public void OnJump()
        {
            // Play jump animation event
            playerAnimator.SetTrigger("Jump");
        }

        public void OnLand()
        {
            // Play landing animation event
            playerAnimator.SetTrigger("Land");
        }

        public void OnWeaponReload()
        {
            // Weapon reload animation event
            playerAnimator.SetTrigger("WeaponReload");
        }

        public void OnWeaponShoot()
        {
            // Weapon shoot animation event
            playerAnimator.SetTrigger("WeaponShoot");
        }
        #endregion

        #region Utility Methods
        void CreateFootstepEffect()
        {
            // Create dust particle effect at feet
            GameObject dustEffect = Instantiate(Resources.Load<GameObject>("Effects/FootstepDust"), 
                                               transform.position + Vector3.down * 0.9f, 
                                               Quaternion.identity);

            // Auto destroy
            Destroy(dustEffect, 2f);
        }

        public bool IsPlayingAnimation(string animationName)
        {
            if (playerAnimator == null) return false;

            AnimatorStateInfo stateInfo = playerAnimator.GetCurrentAnimatorStateInfo(baseLayerIndex);
            return stateInfo.IsName(animationName);
        }

        public float GetAnimationLength(string animationName)
        {
            if (playerAnimator == null) return 0f;

            RuntimeAnimatorController controller = playerAnimator.runtimeAnimatorController;
            foreach (AnimationClip clip in controller.animationClips)
            {
                if (clip.name == animationName)
                {
                    return clip.length;
                }
            }
            return 0f;
        }

        public void SetAnimationSpeed(float speed)
        {
            animationSpeed = speed;
            if (playerAnimator != null)
            {
                playerAnimator.speed = speed;
            }
        }
        #endregion

        // Getters
        public bool IsAnimating() => playerAnimator != null && playerAnimator.GetCurrentAnimatorStateInfo(0).length > 0;
        public bool IsMoving() => isMoving;
        public bool IsAiming() => isAiming;
        public bool IsDead() => isDead;
        public float GetAnimationBlendValue() => animationBlendValue;
    }

    [System.Serializable]
    public class GestureAnimation
    {
        public GestureType type;
        public float duration;
        public bool isPlaying;
    }

    public enum GestureType
    {
        Wave,
        ThumbsUp,
        Point,
        Shrug,
        Dance,
        Victory,
        Celebrate,
        LookAround,
        Emote1,
        Emote2,
        Emote3
    }
}