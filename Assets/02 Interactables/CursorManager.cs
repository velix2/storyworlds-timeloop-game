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
        //TODO: get texture and apply to cursor
        if (interactable == null)
        {
            return;
        }
        Debug.Log($"Changed cursor! Primary: {interactable.Primary}, Secondary: {interactable.Secondary}");
    }
    
    
    public static void ResetCursor()
    {
        //TODO: reset the cursor to default texture
        Debug.Log("Cursor Reset!");
    }
}
