using System;
using TimeManagement;
using UnityEngine;

public class LocationMusicHolder : MonoBehaviour
{
    [SerializeField] private AudioClip defaultMusic;
    [SerializeField] private bool useTimeDependantMusic;
    
    [Header("Time Dependant Music")] 
    [SerializeField] private AudioClip morningMusic;
    [SerializeField] private AudioClip afternoonMusic;
    [SerializeField] private AudioClip eveningMusic;
    [SerializeField] private AudioClip nightMusic;

    private AudioClip current;
    
    private void Start()
    {
        TimeHandler.Instance.onDayPhaseChanged.AddListener(OnDayPhaseChanged);
        if (!useTimeDependantMusic) AudioManager.ChangeDefaultMusic(defaultMusic);
    }

    private void OnDayPhaseChanged(DaytimePhase phase)
    {
        if (!useTimeDependantMusic) return;

        AudioClip musicToPlay = GetDayPhaseMusic(phase);
        if (musicToPlay != null)print(musicToPlay.name);
        if (musicToPlay == current) return;
        
        AudioManager.ChangeDefaultMusic(musicToPlay);
        current = musicToPlay;
    }

    private AudioClip GetDayPhaseMusic(DaytimePhase phase)
    {
        AudioClip musicToPlay = null;
        switch (phase)
        {
            case DaytimePhase.Afternoon: musicToPlay = afternoonMusic; break;
            case DaytimePhase.Evening: musicToPlay = eveningMusic; break;
            case DaytimePhase.Morning: musicToPlay = morningMusic; break;
            case DaytimePhase.Night: musicToPlay = nightMusic; break;
        }
        return musicToPlay;
    }
}


