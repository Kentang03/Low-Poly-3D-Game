using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource uiSource;
    
    [Header("Main Menu Music")]
    public AudioClip mainMenuMusic;
    public AudioClip gameplayMusic;
    
    [Header("UI Sounds")]
    public AudioClip buttonClickSound;
    public AudioClip buttonHoverSound;
    public AudioClip transitionSound;
    
    [Header("Audio Settings")]
    public AudioMixer audioMixer;
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    [Range(0f, 1f)]
    public float musicVolume = 0.7f;
    [Range(0f, 1f)]
    public float sfxVolume = 0.8f;
    
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<AudioManager>();
                if (instance == null)
                {
                    GameObject audioManagerGO = new GameObject("AudioManager");
                    instance = audioManagerGO.AddComponent<AudioManager>();
                }
            }
            return instance;
        }
    }
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioSources();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        LoadAudioSettings();
        PlayMainMenuMusic();
    }
    
    void InitializeAudioSources()
    {
        // Jika audio sources belum di-assign, buat secara otomatis
        if (musicSource == null)
        {
            GameObject musicGO = new GameObject("MusicSource");
            musicGO.transform.SetParent(transform);
            musicSource = musicGO.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
        }
        
        if (sfxSource == null)
        {
            GameObject sfxGO = new GameObject("SFXSource");
            sfxGO.transform.SetParent(transform);
            sfxSource = sfxGO.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
        
        if (uiSource == null)
        {
            GameObject uiGO = new GameObject("UISource");
            uiGO.transform.SetParent(transform);
            uiSource = uiGO.AddComponent<AudioSource>();
            uiSource.loop = false;
            uiSource.playOnAwake = false;
        }
    }
    
    void LoadAudioSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
        
        UpdateAudioMixer();
    }
    
    void UpdateAudioMixer()
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
        }
        else
        {
            // Jika tidak ada audio mixer, set volume langsung
            if (musicSource != null) musicSource.volume = masterVolume * musicVolume;
            if (sfxSource != null) sfxSource.volume = masterVolume * sfxVolume;
            if (uiSource != null) uiSource.volume = masterVolume * sfxVolume;
        }
    }
    
    // Music Methods
    public void PlayMainMenuMusic()
    {
        if (mainMenuMusic != null && musicSource != null)
        {
            PlayMusic(mainMenuMusic);
        }
    }
    
    public void PlayGameplayMusic()
    {
        if (gameplayMusic != null && musicSource != null)
        {
            PlayMusic(gameplayMusic);
        }
    }
    
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource != null && clip != null)
        {
            if (musicSource.clip == clip && musicSource.isPlaying) return;
            
            musicSource.clip = clip;
            musicSource.Play();
        }
    }
    
    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }
    
    public void FadeOutMusic(float duration)
    {
        StartCoroutine(FadeAudioSource(musicSource, musicSource.volume, 0f, duration, true));
    }
    
    public void FadeInMusic(float duration)
    {
        if (musicSource != null)
        {
            float targetVolume = masterVolume * musicVolume;
            StartCoroutine(FadeAudioSource(musicSource, 0f, targetVolume, duration, false));
        }
    }
    
    // UI Sound Methods
    public void PlayButtonClick()
    {
        PlayUISound(buttonClickSound);
    }
    
    public void PlayButtonHover()
    {
        PlayUISound(buttonHoverSound);
    }
    
    public void PlayTransition()
    {
        PlayUISound(transitionSound);
    }
    
    public void PlayUISound(AudioClip clip)
    {
        if (uiSource != null && clip != null)
        {
            uiSource.PlayOneShot(clip);
        }
    }
    
    // SFX Methods
    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
    
    public void PlaySFX(AudioClip clip, float volume)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }
    
    // Volume Control
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateAudioMixer();
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        UpdateAudioMixer();
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        UpdateAudioMixer();
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }
    
    // Coroutine untuk fade effect
    System.Collections.IEnumerator FadeAudioSource(AudioSource audioSource, float startVolume, float targetVolume, float duration, bool stopAfterFade)
    {
        if (audioSource == null) yield break;
        
        float elapsedTime = 0f;
        
        while (elapsedTime < duration)
        {
            float progress = elapsedTime / duration;
            audioSource.volume = Mathf.Lerp(startVolume, targetVolume, progress);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        audioSource.volume = targetVolume;
        
        if (stopAfterFade && audioSource.volume <= 0f)
        {
            audioSource.Stop();
        }
    }
}