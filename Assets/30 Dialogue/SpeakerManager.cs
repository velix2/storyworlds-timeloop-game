using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script manages all the speaker data
/// Every speaker is stored in speakerDirectionary and can be accessed through every other script
/// The listof  speakers has to be initialized in the inspector
/// </summary>
public class SpeakerManager : MonoBehaviour
{
    public static SpeakerManager Instance { get; private set; }

    [SerializeField] private List<SpeakerData> speakers;  
    public Dictionary<string, SpeakerData> speakerDictionary;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Found more than one SpeakerManager in the scene!");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Initialize the dictionary
        speakerDictionary = new Dictionary<string, SpeakerData>();
        foreach (var speaker in speakers)
        {
            speakerDictionary[speaker.speakerId] = speaker;
        }
    }

    public SpeakerData GetSpeakerDataByID(string speakerID)
    {
        if (speakerDictionary.TryGetValue(speakerID, out SpeakerData speakerData))
        {
            return speakerData;
        }
        Debug.LogError($"Speaker with ID '{speakerID}' not found!");
        return null;
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

        SpeakerData speaker = GetSpeakerDataByID(speakerID);
        if(speaker == null)
        {
            return null;
        }

        foreach (Portrait p in speaker.portraits)
        {
            if(p.emotion == emotion)
            {
                speakerPortrait = p.sprite;
                break;
            }
        }

        if (speakerPortrait == null) 
        {
            Debug.LogWarning("Portrait of " + speaker.name 
                + " with emotion " + emotion + " could not be found!");
            return speaker.portraits[0].sprite;
        }
        
        return speakerPortrait;
    }


   
}
