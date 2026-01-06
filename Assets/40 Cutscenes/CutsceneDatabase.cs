using UnityEngine;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "CutsceneDatabase", menuName = "Scriptable Objects/CutsceneDatabase")]
public class CutsceneDatabase : ScriptableObject
{
    public PlayableAsset[] cutscenes;
}

