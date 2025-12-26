using System;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    
    /// <summary>
    /// Changes Cursor depending on the given Interactable.<br/>
    /// Used by InteractionChecker.
    /// </summary>
    /// <param name="interactable">Interactable the cursor should be based on</param>
    public static void ChangeCursorInteraction(Interactable interactable)
    {
        //TODO: get texture and apply to cursor
        if (interactable == null) return;
        
        SetTransparency(interactable.PrimaryNeedsInRange && !interactable.inRange);
        Debug.Log($"Changed cursor! Primary: {interactable.Primary}, Secondary: {interactable.Secondary}");
        
    }

    /// <summary>
    /// Changes Cursor depending on given Item.<br/>
    /// Used by InteractionChecker.
    /// </summary>
    /// <param name="item">Item the cursor should be based on</param>
    /// <param name="transparent"></param>
    public static void ChangeCursorItem(ItemData item, bool transparent = false)
    {
        SetTransparency(transparent);
        //TODO: change cursor to fit accordingly
        Debug.Log($"Your cursor now shows that {item} is selected.");
    }
    
    /// <summary>
    /// Resets the Cursor to default.<br/>
    /// Used by InteractionChecker.
    /// </summary>
    public static void ResetCursor()
    {
        SetTransparency(false);
        //TODO: reset the cursor to default texture
        Debug.Log("Cursor Reset!");
    }

    /// <summary>
    /// Sets tranparency of cursor.<br/>
    /// Indirectly used by InteractionChecker.
    /// </summary>
    /// <param name="transparent"></param>
    public static void SetTransparency(bool transparent)
    {
        //TODO: set transparency accordingly
    }
}
