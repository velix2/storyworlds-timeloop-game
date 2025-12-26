using System.Collections.Generic;
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

    /// <summary>
    /// Function to set the speaker's name on the name box
    /// </summary>
    /// <param name="speakerId"></param>
    /// <param name="speakerNameText"></param>
    public void SetSpeaker(string speakerId, TextMeshProUGUI speakerNameText)
    {
        if (speakerDictionary.ContainsKey(speakerId))
        {
            SpeakerData speaker = speakerDictionary[speakerId];
            speakerNameText.text = speaker.speakerName;
            speakerNameText.color = speaker.color;
        }
        else
        {
            Debug.LogError("Speaker with ID " + speakerId + " could not be found!");
        }
    }

    /// <summary>
    /// Function to set the portrait of the speaker according to the emotion
    /// </summary>
    /// <param name="speakerId"></param>
    /// <param name="emotion"></param>
    public void SetSpeakerEmotion(string speakerId, Emotion emotion, Image portraitImage)
    {
        if (speakerDictionary.ContainsKey(speakerId))
        {
            SpeakerData speaker = speakerDictionary[speakerId];
            Portrait portrait = GetEmotionPortrait(speaker, emotion);
            portraitImage.sprite = portrait.portrait;
        }
        else
        {
            Debug.LogError("Speaker with ID " + speakerId + " could not be found!");
        }
    }

 /// <summary>
 /// Get the right portrait according to the amotion.
 /// </summary>
 /// <param name="speakerData"></param>
 /// <param name="emotion"></param>
 /// <returns>Returns first element of the portraits list, if emotion could not be found.</returns>
    private Portrait GetEmotionPortrait(SpeakerData speakerData, Emotion emotion)
    {
        if(speakerData.portraits == null || speakerData.portraits.Length == 0)
        {
            Debug.LogError("Speaker portraits is null or empty!");
            return default;
        }
        foreach (Portrait portrait in speakerData.portraits)
        {
            if (portrait.emotion == emotion)
            {
                return portrait;
            }
        }
        Debug.LogWarning("Portrait of " + speakerData.name + " with emotion " + emotion + "could not be found!");
        return speakerData.portraits[0];
    }
}
