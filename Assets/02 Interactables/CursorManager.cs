using System;
using UnityEngine;

public class CursorManager : MonoBehaviour
{

    private CursorManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public static void ChangeCursorInteraction(Interactable interactable)
    {
        SetTransparency(!interactable.inRange);
        //TODO: get texture and apply to cursor
        if (interactable == null)
        {
            return;
        }
        Debug.Log($"Changed cursor! Primary: {interactable.Primary}, Secondary: {interactable.Secondary}");
    }

    public static void ChangeCursorItem(ItemData item, bool transparent = false)
    {
        SetTransparency(transparent);
        //TODO: change cursor to fit accordingly
        Debug.Log($"Your cursor now shows that {item} is selected.");
    }
    
    
    public static void ResetCursor()
    {
        SetTransparency(false);
        //TODO: reset the cursor to default texture
        Debug.Log("Cursor Reset!");
    }

    public static void SetTransparency(bool transparent)
    {
        //TODO: set transparency accordingly
    }
}
