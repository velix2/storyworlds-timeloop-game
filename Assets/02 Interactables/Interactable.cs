using System;
using UnityEngine;

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

    [Header("Outline")] 
    public virtual Color OutlineColor
    {
        get;
        set;
    } = Color.white;

    public virtual float OutlineWidth
    {
        get;
        set;
    } = 2.0f;
    
    /// <summary>
    /// Bool used to determine if Interactable is in range of the player.<br/>
    /// Don't use it, can be overwritten by InteractionChecker.
    /// </summary>
    public bool inRange;
    
    private bool highlightOverwrite;
    protected bool HighlightOverwrite => highlightOverwrite;

    public enum interactionType
    {
        NONE,
        LOOK,
        SPEAK,
        GRAB,
        ENTER_OPEN,
        ENTER_CLOSED,
        INSPECT,
        _enumlength
    }


    /// <summary>
    /// Defines the type of interaction, that is performed when calling PrimaryInteraction.<br/>
    /// Value is used for displaying e.g. changing mouse cursor. <br/>
    /// You can make the return type dependent on status of the Interactable.
    /// </summary>
    public abstract interactionType Primary
    {
        get;
    }

    /// <summary>
    /// Defines the type of interaction, that is performed when calling SecondaryInteraction().<br/>
    /// Value is used for displaying e.g. changing mouse cursor. <br/>
    /// You can make the return type dependent on status of the Interactable.
    /// </summary>
    public abstract interactionType Secondary
    {
        get;
    }

    /// <summary>
    /// Defines if the Interactable must be in range to call PrimaryInteraction().<br/>
    /// You can make the return type dependent on status of the Interactable.
    /// </summary>
    public abstract bool PrimaryNeedsInRange
    {
        get;
    }
    /// <summary>
    /// Defines if the Interactable must be in range to call SecondaryInteraction().<br/>
    /// You can make the return type dependent on status of the Interactable.
    /// </summary>
    public abstract bool SecondaryNeedsInRange
    {
        get;
    }

    /// <summary>
    /// The method that is called, when the player interacts with this Interactable primarily.<br/>
    /// </summary>
    public abstract void PrimaryInteraction();
    /// <summary>
    /// The method that is called, when the player interacts with this Interactable secondarily.<br/>
    /// </summary>
    public abstract void SecondaryInteraction();

    /// <summary>
    /// The method that is called, when the player uses an item with this Interactable.<br/>
    /// Can only be called by the player when inRange is true.
    /// </summary>
    /// <param name="otherItem">The used item</param>
    /// <returns>bool value informs if the applied item is used up and should be removed from the inventory</returns>
    public virtual bool ItemInteraction(ItemData otherItem)
    {
        DialogueManager.Instance.EnterDialogueModeSimple("Das wird so nicht funktionieren.");
        return false;
    }

    /// <summary>
    /// Used to highlight the Interactable. <br/>
    /// Will be undone by Unhighlight(), which might be called by InteractionChecker.
    /// </summary>
    public abstract void Highlight();
    /// <summary>
    /// Used to remove the highlighting of the Interactable. <br/>
    /// </summary>
    public abstract void Unhighlight();

    /// <summary>
    /// Used to highlight the Interactable. <br/>
    /// Will override all highlighting done by Highlight() and Unhighlight() <br/>
    /// !! Can only be undone by UndoHighlightPermanently().
    /// </summary>
    public void HighlightPermanently()
    {
        Highlight();
        highlightOverwrite = true;
    }

    /// <summary>
    /// Undoes highlighting overwrite by HighlightPermanently.
    /// </summary>
    public void UndoHighlightPermanently()
    {
        highlightOverwrite = false;
        Unhighlight();
    }

    public void Enable()
    {
        enabled = true;
    }

    public void Disable()
    {
        enabled = false;
        UndoHighlightPermanently();
    }
    
}
