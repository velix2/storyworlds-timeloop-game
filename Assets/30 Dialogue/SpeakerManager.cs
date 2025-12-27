using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeakerManager : MonoBehaviour
{
    public List<SpeakerData> speakers;  
    private Dictionary<string, SpeakerData> speakerDictionary;

    void Awake()
    { 
        speakerDictionary = new Dictionary<string, SpeakerData>();
        foreach (var speaker in speakers)
        {
            speakerDictionary[speaker.speakerId] = speaker;
        }
    }

    public SpeakerData GetSpeakerDataByID(string speakerID)
    {
        return speakerDictionary[speakerID];
    }

    /// <summary>
    /// Function to search for a sprite by emotion
    /// </summary>
    /// <param name="speakerID">Current speaker</param>
    /// <param name="emotion">with emotion</param>
    /// <returns>Sprite with the given emotion. Returns the first element in the dictionary if it could not be found.</returns>
    public Sprite GetSpeakerPortraitByEmotion(string speakerID, Emotion emotion)
    {
        if (speakerDictionary[speakerID].portraits == null || speakerDictionary[speakerID].portraits.Length == 0)
        {
            Debug.LogError("Speaker portraits is null or empty!");
            return null;
        }

        Sprite speakerPortrait = null;

        foreach (Portrait p in speakerDictionary[speakerID].portraits)
        {
            if(p.emotion == emotion)
            {
                speakerPortrait = p.sprite;
                break;
            }
        }

        if (speakerPortrait == null) 
        {
            Debug.LogWarning("Portrait of " + speakerDictionary[speakerID].name 
                + " with emotion " + emotion + " could not be found!");
            return speakerDictionary[speakerID].portraits[0].sprite;
        }
        
        return speakerPortrait;
    }

    public Dictionary<string, SpeakerData> GetSpeakerDirectionary()
    {
        return speakerDictionary;
    }

   
}
