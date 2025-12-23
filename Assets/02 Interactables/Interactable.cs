using System;
using UnityEditor.PackageManager;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class Interactable : MonoBehaviour
{

    private static string layerName = "Interactable";
    public static string LayerName => layerName;
    private static int layer;
    public static int Layer => layer;
    
    private void Awake()
    {
        layer = LayerMask.NameToLayer(layerName);
        if (layer == -1)
        {
            Debug.LogError($"Layer \"{layerName}\" was not found.");
        }
        gameObject.layer = layer;
    }

    
    public abstract void PrimaryInteraction();
    public abstract void SecondaryInteraction();
    
}
