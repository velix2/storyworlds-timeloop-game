using System;
using UnityEditor.PackageManager;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class Interactable : MonoBehaviour
{
    #region Non Specific
    private static string layerName = "Interactable";
    public static string LayerName => layerName;
    private static int layer;
    
    private void Awake()
    {
        layer = LayerMask.NameToLayer(layerName);
        if (layer == -1)
        {
            Debug.LogError($"Layer \"{layerName}\" was not found.");
        }
        gameObject.layer = layer;
    }
    #endregion

    public enum interactionType
    {
        NONE,
        LOOK,
        GRAB,
        SPEAK,
        ENTER
    }


    public abstract interactionType Primary
    {
        get;
    }

    public abstract interactionType Secondary
    {
        get;
    }

    public abstract void PrimaryInteraction();
    public abstract void SecondaryInteraction();
    public abstract void Highlight();
    public abstract void Unhighlight();


}
