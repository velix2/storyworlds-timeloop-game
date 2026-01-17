using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Sound", menuName = "Resource/Audio/Sound")]
public class Sound : ScriptableObject
{
    [SerializeField] private AudioClip[] audios;
    private AudioClip[] clips;
    private AudioClip lastClip;
    
    /*private void Inititialize()
    {
        if (audios.Length == 0)
        {
            Debug.LogError($"{name} has no audio clips!");
            return;
        }

        if (clips != null) return;

        if (audios.Length > 1)
        {
            clips = new AudioClip[audios.Length - 1];
            for (int i = 0; i < audios.Length - 1; i++)
            {
                clips[i] = audios[i];
            }

            lastClip = audios[audios.Length - 1];
        }
        else
        {
            lastClip = audios[0];
        }
    }*/
    

    public AudioClip GetAudioClip()
    {
        /*Inititialize();
        if (audios.Length == 0) return null;
        int index = Random.Range(0, clips.Length - 1);
        AudioClip clip = clips[index];
        clips[index] = lastClip;
        lastClip = clip;
        return clip;*/

        if (audios == null || audios.Length == 0)
        {
            Debug.LogError($"{name} has no audio clips!");
            return null;
        }

        if (audios.Length == 1)
            return audios[0];

        AudioClip clip;
        do
        {
            clip = audios[Random.Range(0, audios.Length)];
        }
        while (clip == lastClip);

        lastClip = clip;
        return clip;
    }
}
