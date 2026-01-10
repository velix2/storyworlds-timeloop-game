using System;
using System.Collections;
using Mono.Cecil;
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