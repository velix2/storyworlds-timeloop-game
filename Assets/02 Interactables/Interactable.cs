using System;
using UnityEditor.PackageManager;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class Interactable : MonoBehaviour
{
    #region Non Specific Static
    private static string layerName = "Interactable";
    public static string LayerName => layerName;
    private static int layer;
    
    protected void Awake()
    {
        layer = LayerMask.NameToLayer(layerName);
        if (layer == -1)
        {
            Debug.LogError($"Layer \"{layerName}\" was not found.");
        }
        gameObject.layer = layer;
    }
    #endregion

    public bool inRange;
    
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="otherItem"></param>
    /// <returns>bool value informs if the applied item is used up and should be removed from the inventory</returns>
    public virtual bool ItemInteraction(ItemData otherItem)
    {
        //TODO: Default dialogue if items are not applicable (failsafe if otherwise not defined)
        return false;
    }
    public abstract void Highlight();
    public abstract void Unhighlight();
    

}
