using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer mixer;
    public AudioMixer Mixer => mixer;

    [SerializeField] private AudioMixerGroup stepGroup;
    public AudioMixerGroup StepGroup => stepGroup;
    
    [SerializeField] private AudioMixerGroup sfxGroup;
    public AudioMixerGroup SFXGroup => sfxGroup;
    
    [SerializeField] private AudioMixerGroup musicGroup;
    public AudioMixerGroup MusicGroup => MusicGroup;

    private static AudioManager instance;
    public static AudioManager Instance => instance;

    [Header("References")] 
    [SerializeField] private AudioSource musicSourcePrefab;
    [SerializeField] private AudioSource sfxSourcePrefab;
    
    
    #region Music
    private static AudioSource musicSource1;
    private static AudioSource musicSource2;
    private static bool alternatingMusicSource;

    private static AudioSource activeMusicSource => alternatingMusicSource ? musicSource1 : musicSource2;
    private static AudioSource inactiveMusicSource => alternatingMusicSource ? musicSource2 : musicSource1;
    
    private static AudioClip currentDefaultMusic;
    private static bool playingDefaultMusic = true;


    public static void BackToDefaultMusic()
    {
        playingDefaultMusic = true;
        BlendMusic(currentDefaultMusic);
        
    }

    public static void ChangeDefaultMusic(AudioClip audio)
    {
        currentDefaultMusic = audio;
        if (playingDefaultMusic) BlendMusic(currentDefaultMusic);
    }

    public static void ChangeMusic(AudioClip audio)
    {   
        BlendMusic(audio);
    }

    public static void MuteMusic()
    {
        instance.StartCoroutine(FadeOut(activeMusicSource));
    }

    public static void UnmuteMusic()
    {
        instance.StartCoroutine(FadeIn(activeMusicSource));
    }

    private static void BlendMusic(AudioClip newSong)
    {
        inactiveMusicSource.clip = newSong;

        instance.StartCoroutine(FadeOut(activeMusicSource));
        instance.StartCoroutine(FadeIn(inactiveMusicSource));
        
        alternatingMusicSource = !alternatingMusicSource;
    }

    private static readonly float fadeDuration = 5.0f;
    private static IEnumerator FadeIn(AudioSource source)
    {
        source.Play();
        for (float i = 0.0f; i < fadeDuration; i += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(0.0f, 1.0f, i / fadeDuration);
            yield return null;
        }

        source.volume = 1.0f;
        source.UnPause();
    }

    private static IEnumerator FadeOut(AudioSource source)
    {
        for (float i = 0.0f; i < fadeDuration; i += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(1.0f, 0.0f, i / fadeDuration);
            yield return null;
        }

        source.volume = 0.0f;
        source.Pause();
    }
    
    #endregion

    #region Sound

    private static List<AudioSource> availableSFX = new List<AudioSource>();
    
    public static void PlaySFXat(AudioClip clip, Vector3 worldPosition)
    {
        AudioSource source = PopAvailableSFX();
        source.clip = clip;
        source.transform.position = worldPosition;
        source.Play();
        instance.StartCoroutine(BlockSFXDuringPlay(source));
    }

    public static void PlaySFX(AudioClip clip)
    {
        AudioSource source = PopAvailableSFX();
        source.clip = clip;
        source.transform.position = Camera.main.transform.position;
        source.transform.parent = Camera.main?.transform;
        
        source.Play();
        instance.StartCoroutine(BlockSFXDuringPlay(source));
    }

    private static AudioSource PopAvailableSFX()
    {
        AudioSource result;
        if (availableSFX.Count == 0) result = Instantiate(instance.sfxSourcePrefab);
        else
        {
            result = availableSFX[0];
            availableSFX.RemoveAt(0);
        }
        return result;
    }

    private static IEnumerator BlockSFXDuringPlay(AudioSource source)
    {
        availableSFX.Remove(source);
        if (source.clip != null) yield return new WaitForSeconds(source.clip.length);
        availableSFX.Add(source);
        source.transform.parent = null;
    }
    #endregion

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        ApplyMusicSource();
    }

    private void Start()
    { 
        SceneSwitcher.Instance.SceneSwitched.AddListener(ApplyMusicSource);
    }

    private static void ApplyMusicSource()
    {
        if (musicSource1 == null)
        {
            musicSource1 = Instantiate(instance.musicSourcePrefab);
            DontDestroyOnLoad(musicSource1.gameObject);
        }
        musicSource1.transform.parent = Camera.main?.transform;
        
        if (musicSource2 == null)
        {
            musicSource2 = Instantiate(instance.musicSourcePrefab);
            DontDestroyOnLoad(musicSource2.gameObject);
        }
        musicSource2.transform.parent = Camera.main?.transform;
    } 

}