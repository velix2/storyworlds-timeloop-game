using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InteractionChecker : MonoBehaviour
{
    public UnityEvent<ItemData> ItemExhausted = new();
    enum interactionModes
    {
        PHYSICS,
        UI,
        DISABLED
    }

    private interactionModes mode;
    public void SetToPhysicsMode() { mode = interactionModes.PHYSICS; }
    public void SetToUIMode() { mode = interactionModes.UI; }
    public void SetToDisabledMode() { mode = interactionModes.DISABLED; }
    
    private int layerMask;
    
    private ItemData selectedItem;
    private Interactable highlightedInteractable;
    
    [SerializeField] private GameObject debugBall;
    
    private void Awake()
    {
        InputManager.PrimaryInteraction += OnPrimaryInteractionInput;
        InputManager.SecondaryInteraction += OnSecondaryInteractionInput;
    }

    private void Start()
    {
        layerMask = LayerMask.GetMask(Interactable.LayerName);
    }

    private void Update()
    {
        Interactable interactable;
        switch (mode)
        {
            case interactionModes.PHYSICS:
                interactable = PhysicsRaycast(InputManager.GetMousePosition());
                break;
            case interactionModes.UI:
                interactable = UIRaycast(InputManager.GetMousePosition());
                break;
            case interactionModes.DISABLED:    
                return;
            default:
                Debug.Log($"Mode {mode} is not implemented.");
                goto case interactionModes.DISABLED;
            
        }

        //entered area of interactable, changing highlightedInteractable
        if (interactable != null && interactable != highlightedInteractable)
        {
            CursorManager.ChangeCursorInteraction(interactable);
            highlightedInteractable?.Unhighlight();
            interactable.Highlight();
            highlightedInteractable = interactable;
        } 
        //exited area of interactable
        else if (interactable == null && highlightedInteractable != null)
        {
            CursorManager.ResetCursor();
            highlightedInteractable.Unhighlight();
            highlightedInteractable = null;
        }
        
        
    }

    private void OnPrimaryInteractionInput(Vector3 mousePosition)
    {
        Interactable interactable = CheckInteractionHit(mousePosition);
        
        if (selectedItem != null)
        {
            var usedUp = interactable?.ItemInteraction(selectedItem);
            if (usedUp.HasValue && usedUp.Value)
            {
                ItemExhausted.Invoke(selectedItem);
            }
            DeselectItem();
        }
        interactable?.PrimaryInteraction();
    }
    
    private void OnSecondaryInteractionInput(Vector3 mousePosition)
    {
        if (selectedItem != null)
        {
            DeselectItem();
        }
        CheckInteractionHit(mousePosition)?.SecondaryInteraction();
    }

    public void SelectItem(ItemData itemData)
    {
        selectedItem = itemData;
        CursorManager.ChangeCursorItem(itemData);
    }

    public void DeselectItem()
    {
        if (selectedItem != null)
        {
            selectedItem = null;
            CursorManager.ResetCursor();
        }
    }
    
    private Interactable CheckInteractionHit(Vector3 mousePosition)
    {
        switch (mode)
        {
            case interactionModes.PHYSICS:
                return PhysicsRaycast(mousePosition);
            case interactionModes.UI:
                return UIRaycast(mousePosition);
            default:
                return null;
        }
    }

    #region Raycast

    private Interactable PhysicsRaycast(Vector3 mousePosition)
    {
        mousePosition.z = 1;
        
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        Physics.Raycast(ray, out var hit, Mathf.Infinity, layerMask);
        
        return hit.collider?.gameObject.GetComponent<Interactable>();
    }

    private Interactable UIRaycast(Vector3 mousePosition)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        return results.Select(r => r.gameObject.GetComponentInParent<Interactable>())
            .FirstOrDefault(i => i != null);;

    }

    #endregion
    
    
}
