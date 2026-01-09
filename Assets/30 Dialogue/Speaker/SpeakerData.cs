using UnityEngine;

/// <summary>
/// This ScriptableObject is for storing the speaker's data that is going to be accessed by the dialogue manager
/// This file also includes a Portrait class to store the character sprites and the emotion
/// SpeakerData and Emotion can be expanded if needed
/// </summary>

[CreateAssetMenu(fileName = "New Speaker Data", menuName = "Scriptable Objects/SpeakerData")]
public class SpeakerData : ScriptableObject
{
    public string speakerId;  
    public string speakerName;
    public Portrait[] portraits;

}

[System.Serializable]
public struct Portrait
{
    public Emotion emotion;
    public Sprite sprite;
}


public enum Emotion
{
    NORMAL, ANGRY, SAD, HAPPY, NERVOUS
}
