using UnityEngine;

[CreateAssetMenu(fileName = "New Speaker Data", menuName = "Scriptable Objects/SpeakerData")]
public class SpeakerData : ScriptableObject
{
    public string speakerId;  
    public string speakerName;
    public Color color;
    public Portrait[] portraits;

}

[System.Serializable]
public struct Portrait
{
    public Emotion emotion;
    public Sprite portrait;
}


public enum Emotion
{
    NORMAL, ANNOYED, SAD, HAPPY
}
