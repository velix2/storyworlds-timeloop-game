using System;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Repeating Sound", menuName = "Resource/Audio/Sound")]
public class SoundRepeating : ScriptableObject
{
    [SerializeField] private AudioClip[] possibleAudios; // user interface
    
    private AudioClip[] audioPool; // a copy of possible Audios, where the audio clips are actually pulled from
    private AudioClip lastClip; // needed for check so that no clip is played twice in a row


    private void OnValidate()
    {
        if (possibleAudios.Length > 1)
        {
            audioPool = new AudioClip[possibleAudios.Length - 1];
            Array.Copy(possibleAudios, audioPool, possibleAudios.Length - 1);

            lastClip = possibleAudios[possibleAudios.Length - 1];
        }
    }
    
    
    public AudioClip GetAudioClip()
    {
        if (possibleAudios.Length < 1) return null;
        if (possibleAudios.Length == 1) return possibleAudios[0];
        //(possibleAudios.Length > 1) => audioPooling;
        int index = Random.Range(0, audioPool.Length);

        Debug.LogWarning(index);
        AudioClip clip = audioPool[index];
        audioPool[index] = lastClip;
        lastClip = clip;
        return clip;
    }
}
