/*
 * Forest Warfare Shooter
 * Copyright © 2025 xpe.nettt 👑
 * AudioManager.cs - Sistema de audio dinámico y creativo
 * Sonidos inspirados en juegos como Fall Guys - diversos y expresivos
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TacticalShooter.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        public AudioSource musicSource;
        public AudioSource sfxSource;
        public AudioSource voiceSource;
        public AudioSource ambientSource;

        [Header("Volume Controls")]
        [Range(0f, 1f)] public float masterVolume = 1f;
        [Range(0f, 1f)] public float musicVolume = 0.7f;
        [Range(0f, 1f)] public float sfxVolume = 1f;
        [Range(0f, 1f)] public float voiceVolume = 0.8f;
        [Range(0f, 1f)] public float ambientVolume = 0.5f;

        [Header("Dynamic Audio")]
        public bool enableDynamicMusic = true;
        public bool enableSpatialAudio = true;
        public bool enableFootstepVariations = true;
        public bool enableReactiveAudio = true;

        [Header("Audio Clips")]
        public AudioClip[] musicClips;
        public AudioClip[] sfxClips;
        public AudioClip[] voiceClips;
        public AudioClip[] ambientClips;
        public AudioClip[] footstepClips;
        public AudioClip[] weaponClips;
        public AudioClip[] uiClips;
        public AudioClip[] environmentalClips;

        // Singleton instance
        public static AudioManager Instance { get; private set; }

        // Audio state tracking
        private bool isMusicPlaying = false;
        private bool isPaused = false;
        private AudioClip currentMusicClip;
        private float musicFadeTime = 2f;

        // Audio pools for dynamic sound management
        private Queue<AudioSource> availableSfxSources = new Queue<AudioSource>();
        private List<AudioSource> activeSfxSources = new List<AudioSource>();

        // Spatial audio tracking
        private Dictionary<Transform, AudioSource> spatialAudioSources = new Dictionary<Transform, AudioSource>();

        // Dynamic audio events
        private System.Action<AudioEventType> onAudioEvent;

        // Audio categories for organized management
        private Dictionary<string, List<AudioClip>> audioCategories = new Dictionary<string, List<AudioClip>>();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioManager();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            LoadAudioSettings();
            StartCoroutine(InitializeAmbientAudio());
        }

        void InitializeAudioManager()
        {
            // Create audio sources if not assigned
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
            }

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
            }

            if (voiceSource == null)
            {
                voiceSource = gameObject.AddComponent<AudioSource>();
            }

            if (ambientSource == null)
            {
                ambientSource = gameObject.AddComponent<AudioSource>();
                ambientSource.loop = true;
            }

            // Setup audio source properties
            SetupAudioSource(musicSource, "Music");
            SetupAudioSource(sfxSource, "SFX");
            SetupAudioSource(voiceSource, "Voice");
            SetupAudioSource(ambientSource, "Ambient");

            // Initialize audio categories
            InitializeAudioCategories();
        }

        void SetupAudioSource(AudioSource source, string name)
        {
            source.playOnAwake = false;
            source.spatialBlend = enableSpatialAudio ? 1f : 0f;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.maxDistance = 50f;
            source.volume = 1f;
        }

        void InitializeAudioCategories()
        {
            // Music categories
            audioCategories["MenuMusic"] = new List<AudioClip>();
            audioCategories["GameMusic"] = new List<AudioClip>();
            audioCategories["VictoryMusic"] = new List<AudioClip>();
            audioCategories["DefeatMusic"] = new List<AudioClip>();

            // SFX categories
            audioCategories["Footsteps"] = new List<AudioClip>();
            audioCategories["WeaponSFX"] = new List<AudioClip>();
            audioCategories["UI"] = new List<AudioClip>();
            audioCategories["Environmental"] = new List<AudioClip>();
            audioCategories["PlayerActions"] = new List<AudioClip>();
            audioCategories["Combat"] = new List<AudioClip>();
            audioCategories["Interactive"] = new List<AudioClip>();

            // Voice categories
            audioCategories["Victory"] = new List<AudioClip>();
            audioCategories["Defeat"] = new List<AudioClip>();
            audioCategories["Pain"] = new List<AudioClip>();
            audioCategories["Commands"] = new List<AudioClip>();
            audioCategories["Emotes"] = new List<AudioClip>();
        }

        #region Music System
        public void PlayMusic(AudioClip clip, bool fadeIn = true)
        {
            if (clip == null) return;

            StartCoroutine(FadeMusic(clip, fadeIn));
        }

        public void PlayMenuMusic()
        {
            if (audioCategories["MenuMusic"].Count > 0)
            {
                AudioClip menuMusic = audioCategories["MenuMusic"][Random.Range(0, audioCategories["MenuMusic"].Count)];
                PlayMusic(menuMusic, true);
            }
        }

        public void PlayGameMusic()
        {
            if (enableDynamicMusic)
            {
                PlayDynamicMusic();
            }
            else if (audioCategories["GameMusic"].Count > 0)
            {
                AudioClip gameMusic = audioCategories["GameMusic"][Random.Range(0, audioCategories["GameMusic"].Count)];
                PlayMusic(gameMusic, false);
            }
        }

        public void PlayVictoryMusic()
        {
            if (audioCategories["VictoryMusic"].Count > 0)
            {
                AudioClip victoryMusic = audioCategories["VictoryMusic"][Random.Range(0, audioCategories["VictoryMusic"].Count)];
                PlayMusic(victoryMusic, true);
            }
        }

        public void PlayDefeatMusic()
        {
            if (audioCategories["DefeatMusic"].Count > 0)
            {
                AudioClip defeatMusic = audioCategories["DefeatMusic"][Random.Range(0, audioCategories["DefeatMusic"].Count)];
                PlayMusic(defeatMusic, true);
            }
        }

        public void StopMusic(bool fadeOut = true)
        {
            StartCoroutine(FadeMusic(null, fadeOut));
        }

        IEnumerator FadeMusic(AudioClip newClip, bool fadeIn)
        {
            if (isMusicPlaying && musicSource.isPlaying)
            {
                // Fade out current music
                float startVolume = musicSource.volume;
                for (float t = 0; t < musicFadeTime; t += Time.deltaTime)
                {
                    musicSource.volume = Mathf.Lerp(startVolume, 0f, t / musicFadeTime);
                    yield return null;
                }

                musicSource.Stop();
            }

            if (newClip != null)
            {
                // Start new music
                musicSource.clip = newClip;
                musicSource.volume = fadeIn ? 0f : masterVolume * musicVolume;
                musicSource.Play();
                isMusicPlaying = true;

                if (fadeIn)
                {
                    // Fade in new music
                    for (float t = 0; t < musicFadeTime; t += Time.deltaTime)
                    {
                        musicSource.volume = Mathf.Lerp(0f, masterVolume * musicVolume, t / musicFadeTime);
                        yield return null;
                    }
                }
            }
            else
            {
                isMusicPlaying = false;
            }
        }

        void PlayDynamicMusic()
        {
            // Dynamic music based on game state
            GameState gameState = GameManager.Instance?.GetGameState() ?? GameState.Waiting;

            switch (gameState)
            {
                case GameState.Waiting:
                    PlayMenuMusic();
                    break;
                case GameState.Playing:
                    PlayIntenseCombatMusic();
                    break;
                case GameState.Ended:
                    CheckForVictoryMusic();
                    break;
            }
        }

        void PlayIntenseCombatMusic()
        {
            // More intense music when in combat
            if (audioCategories["GameMusic"].Count > 0)
            {
                AudioClip combatMusic = audioCategories["GameMusic"][Random.Range(0, audioCategories["GameMusic"].Count)];
                PlayMusic(combatMusic, false);
            }
        }

        void CheckForVictoryMusic()
        {
            // Determine if it was a victory or defeat
            bool isVictory = GameManager.Instance?.IsPlayerVictory() ?? false;

            if (isVictory)
            {
                PlayVictoryMusic();
            }
            else
            {
                PlayDefeatMusic();
            }
        }
        #endregion

        #region SFX System
        public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            if (clip == null) return;

            sfxSource.pitch = pitch;
            sfxSource.volume = masterVolume * sfxVolume * volume;
            sfxSource.PlayOneShot(clip);

            // Trigger audio event
            onAudioEvent?.Invoke(AudioEventType.SFXPlayed);
        }

        public void PlaySFXAtPosition(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (clip == null) return;

            AudioSource tempSource = new GameObject("TempSFX").AddComponent<AudioSource>();
            tempSource.transform.position = position;
            tempSource.clip = clip;
            tempSource.volume = masterVolume * sfxVolume * volume;
            tempSource.spatialBlend = 1f;
            tempSource.Play();

            Destroy(tempSource.gameObject, clip.length);
        }

        public void PlayRandomSFX(string category, float volume = 1f)
        {
            if (audioCategories.ContainsKey(category) && audioCategories[category].Count > 0)
            {
                AudioClip randomClip = audioCategories[category][Random.Range(0, audioCategories[category].Count)];
                PlaySFX(randomClip, volume);
            }
        }
        #endregion

        #region Footstep System
        public void PlayFootstepSound()
        {
            if (!enableFootstepVariations)
            {
                PlaySFX(footstepClips[0]);
                return;
            }

            // Determine surface type and play appropriate footstep
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f))
            {
                string surfaceType = hit.collider.tag;
                PlaySurfaceSpecificFootstep(surfaceType);
            }
            else
            {
                PlayRandomFootstep();
            }
        }

        void PlaySurfaceSpecificFootstep(string surfaceType)
        {
            switch (surfaceType.ToLower())
            {
                case "grass":
                    PlaySFX(footstepClips[0]); // Grass footsteps
                    break;
                case "dirt":
                    PlaySFX(footstepClips[1]); // Dirt footsteps
                    break;
                case "wood":
                    PlaySFX(footstepClips[2]); // Wood footsteps
                    break;
                case "metal":
                    PlaySFX(footstepClips[3]); // Metal footsteps
                    break;
                default:
                    PlayRandomFootstep();
                    break;
            }
        }

        void PlayRandomFootstep()
        {
            if (footstepClips.Length > 0)
            {
                AudioClip randomFootstep = footstepClips[Random.Range(0, footstepClips.Length)];
                PlaySFX(randomFootstep, 0.7f);
            }
        }
        #endregion

        #region Weapon Audio
        public void PlayWeaponSound(WeaponSoundType soundType)
        {
            switch (soundType)
            {
                case WeaponSoundType.Fire:
                    PlayRandomSFX("WeaponSFX");
                    break;
                case WeaponSoundType.Reload:
                    PlayRandomSFX("WeaponSFX");
                    break;
                case WeaponSoundType.Empty:
                    PlaySFX(weaponClips[0]); // Empty click
                    break;
                case WeaponSoundType.Hit:
                    PlayRandomSFX("Combat");
                    break;
                case WeaponSoundType.Miss:
                    PlaySFX(weaponClips[1]); // Miss sound
                    break;
            }
        }
        #endregion

        #region UI Audio
        public void PlayButtonSound()
        {
            PlayRandomSFX("UI", 0.8f);
        }

        public void PlayHoverSound()
        {
            PlaySFX(uiClips[0], 0.5f); // Hover sound
        }

        public void PlayErrorSound()
        {
            PlaySFX(uiClips[1]); // Error sound
        }

        public void PlaySuccessSound()
        {
            PlaySFX(uiClips[2]); // Success sound
        }

        public void PlayNotificationSound()
        {
            PlaySFX(uiClips[3]); // Notification sound
        }
        #endregion

        #region Player Voice System
        public void PlayVoiceLine(VoiceLineType lineType)
        {
            switch (lineType)
            {
                case VoiceLineType.Victory:
                    PlayRandomVoice("Victory");
                    break;
                case VoiceLineType.Defeat:
                    PlayRandomVoice("Defeat");
                    break;
                case VoiceLineType.Pain:
                    PlayRandomVoice("Pain");
                    break;
                case VoiceLineType.Command:
                    PlayRandomVoice("Commands");
                    break;
                case VoiceLineType.Emote:
                    PlayRandomVoice("Emotes");
                    break;
            }
        }

        void PlayRandomVoice(string category)
        {
            if (audioCategories.ContainsKey(category) && audioCategories[category].Count > 0)
            {
                AudioClip randomVoice = audioCategories[category][Random.Range(0, audioCategories[category].Count)];
                voiceSource.volume = masterVolume * voiceVolume;
                voiceSource.PlayOneShot(randomVoice);
            }
        }
        #endregion

        #region Environmental Audio
        public void PlayEnvironmentalSound(EnvironmentalSoundType soundType)
        {
            switch (soundType)
            {
                case EnvironmentalSoundType.Wind:
                    PlayAmbientSound();
                    break;
                case EnvironmentalSoundType.Birds:
                    PlayBirdsSounds();
                    break;
                case EnvironmentalSoundType.Rustling:
                    PlayRustlingSounds();
                    break;
                case EnvironmentalSoundType.Cracking:
                    PlayCrackingSounds();
                    break;
            }
        }

        void PlayAmbientSound()
        {
            if (ambientClips.Length > 0)
            {
                AudioClip ambientClip = ambientClips[Random.Range(0, ambientClips.Length)];
                ambientSource.clip = ambientClip;
                ambientSource.volume = masterVolume * ambientVolume;
                ambientSource.Play();
            }
        }

        void PlayBirdsSounds()
        {
            PlaySFX(environmentalClips[0]); // Bird chirping
        }

        void PlayRustlingSounds()
        {
            PlaySFX(environmentalClips[1]); // Leaf rustling
        }

        void PlayCrackingSounds()
        {
            PlaySFX(environmentalClips[2]); // Branch cracking
        }

        IEnumerator InitializeAmbientAudio()
        {
            yield return new WaitForSeconds(2f);
            PlayAmbientSound();
        }
        #endregion

        #region Reactive Audio System
        public void OnPlayerAction(PlayerActionType action)
        {
            if (!enableReactiveAudio) return;

            switch (action)
            {
                case PlayerActionType.Jump:
                    PlayRandomSFX("PlayerActions", 0.8f);
                    break;
                case PlayerActionType.Land:
                    PlayRandomSFX("PlayerActions", 0.6f);
                    break;
                case PlayerActionType.Crouch:
                    PlayRandomSFX("PlayerActions", 0.4f);
                    break;
                case PlayerActionType.Sprint:
                    PlayRandomSFX("PlayerActions", 0.5f);
                    break;
            }
        }

        public void OnCombatEvent(CombatEventType eventType)
        {
            if (!enableReactiveAudio) return;

            switch (eventType)
            {
                case CombatEventType.Kill:
                    PlayRandomSFX("Combat", 1f);
                    PlayVoiceLine(VoiceLineType.Emote);
                    break;
                case CombatEventType.Death:
                    PlayRandomSFX("Combat", 0.8f);
                    PlayVoiceLine(VoiceLineType.Pain);
                    break;
                case CombatEventType.FlagCapture:
                    PlayRandomSFX("Interactive", 1f);
                    PlayVoiceLine(VoiceLineType.Command);
                    break;
                case CombatEventType.ObjectiveComplete:
                    PlayRandomSFX("Interactive", 0.9f);
                    break;
            }
        }
        #endregion

        #region Volume Control
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            ApplyVolumes();
            PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            ApplyVolumes();
            PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        }

        public void SetSfxVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            ApplyVolumes();
            PlayerPrefs.SetFloat("SfxVolume", sfxVolume);
        }

        public void SetVoiceVolume(float volume)
        {
            voiceVolume = Mathf.Clamp01(volume);
            ApplyVolumes();
            PlayerPrefs.SetFloat("VoiceVolume", voiceVolume);
        }

        public void SetAmbientVolume(float volume)
        {
            ambientVolume = Mathf.Clamp01(volume);
            ApplyVolumes();
            PlayerPrefs.SetFloat("AmbientVolume", ambientVolume);
        }

        void ApplyVolumes()
        {
            if (musicSource != null) musicSource.volume = masterVolume * musicVolume;
            if (ambientSource != null) ambientSource.volume = masterVolume * ambientVolume;
            // SFX and voice volumes are applied per-play
        }

        void LoadAudioSettings()
        {
            masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
            musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
            sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1f);
            voiceVolume = PlayerPrefs.GetFloat("VoiceVolume", 0.8f);
            ambientVolume = PlayerPrefs.GetFloat("AmbientVolume", 0.5f);

            ApplyVolumes();
        }
        #endregion

        #region Utility Methods
        public void PauseAllAudio()
        {
            isPaused = true;
            AudioListener.pause = true;
        }

        public void ResumeAllAudio()
        {
            isPaused = false;
            AudioListener.pause = false;
        }

        public void MuteAllAudio(bool mute)
        {
            AudioListener.volume = mute ? 0f : 1f;
        }

        public void AddAudioEventListener(System.Action<AudioEventType> listener)
        {
            onAudioEvent += listener;
        }

        public void RemoveAudioEventListener(System.Action<AudioEventType> listener)
        {
            onAudioEvent -= listener;
        }
        #endregion

        // Getters
        public bool IsMusicPlaying => isMusicPlaying;
        public bool IsPaused => isPaused;
        public float GetMasterVolume() => masterVolume;
        public float GetMusicVolume() => musicVolume;
        public float GetSfxVolume() => sfxVolume;
    }

    public enum WeaponSoundType
    {
        Fire,
        Reload,
        Empty,
        Hit,
        Miss
    }

    public enum VoiceLineType
    {
        Victory,
        Defeat,
        Pain,
        Command,
        Emote
    }

    public enum EnvironmentalSoundType
    {
        Wind,
        Birds,
        Rustling,
        Cracking
    }

    public enum PlayerActionType
    {
        Jump,
        Land,
        Crouch,
        Sprint
    }

    public enum CombatEventType
    {
        Kill,
        Death,
        FlagCapture,
        ObjectiveComplete
    }

    public enum AudioEventType
    {
        MusicStarted,
        MusicStopped,
        SFXPlayed,
        VoicePlayed,
        VolumeChanged
    }
}