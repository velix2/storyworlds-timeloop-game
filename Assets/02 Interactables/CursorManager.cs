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
        //TODO
        if (interactable == null)
        {
            return;
        }
        Debug.Log($"Changed cursor! Primary: {interactable.Primary}, Secondary: {interactable.Secondary}");
    }
    
    
    public static void ResetCursor()
    {
        //TODO
        Debug.Log("Cursor Reset!");
    }
}
