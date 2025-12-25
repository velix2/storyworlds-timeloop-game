using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InteractionChecker : MonoBehaviour
{
    public UnityEvent<ItemData> ItemExhausted = new();
    private enum interactionModes
    {
        PHYSICS,
        UI,
        DISABLED
    }

    private interactionModes mode;
    public void SetToPhysicsMode() { mode = interactionModes.PHYSICS; }
    public void SetToUIMode() { mode = interactionModes.UI; }
    public void SetToDisabledMode() { mode = interactionModes.DISABLED; }

    [Header("References")] 
    [SerializeField] private CollisionReporter interactionZone;
    [SerializeField] private CollisionReporter highlightZone;

    [Header("Debug")] 
    [SerializeField] private bool drawInteractionZone;
    [SerializeField] private bool drawHighlightZone;
    
    private int layerMask;
    
    private ItemData selectedItem;
    
    private Interactable highlightedInteractable;
    private List<Interactable> inHighlightRange = new();
    private bool highlightingAll;
    
    [SerializeField] private GameObject debugBall;
    
    public void HighlightAll()
    {
        highlightingAll = true;
        foreach (Interactable interactable in inHighlightRange)
        {
            interactable.Highlight();
        }
    }
    public void UnhighlightAll()
    {
        highlightingAll = false;
        foreach (Interactable interactable in inHighlightRange)
        {
            interactable.Unhighlight();
        }
        HighlightAtMousePosition();
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
    
    private void Awake()
    {
        InputManager.PrimaryInteraction += OnPrimaryInteractionInput;
        InputManager.SecondaryInteraction += OnSecondaryInteractionInput;
        
        highlightZone.TriggerEnter.AddListener(OnHightlightZoneTriggerEnter);
        highlightZone.TriggerExit.AddListener(OnHightlightZoneTriggerExit);
        
        interactionZone.TriggerEnter.AddListener(OnInteractionZoneTriggerEnter);
        interactionZone.TriggerExit.AddListener(OnInteractionZoneTriggerExit);
        
    }

    private void Start()
    {
        layerMask = LayerMask.GetMask(Interactable.LayerName);
    }

    private void Update()
    {
        HighlightAtMousePosition();
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

    #region Hightlight Zone
    private void OnHightlightZoneTriggerEnter(Collider collider)
    {
        
        Interactable interactable = collider.gameObject.GetComponent<Interactable>();
        if (interactable != null)
        {
            inHighlightRange.Add(interactable);
            if (highlightingAll) interactable.Highlight();
        }
    }
    private void OnHightlightZoneTriggerExit(Collider collider)
    {
        Interactable interactable = collider.gameObject.GetComponent<Interactable>();
        if (interactable != null)
        {
            inHighlightRange.Remove(interactable);
            if (highlightingAll) interactable.Unhighlight();
        }
    }
    #endregion
    

    #region Interaction Zone
    private void OnInteractionZoneTriggerEnter(Collider collider)
    {
        Interactable interactable = collider.gameObject.GetComponent<Interactable>();
        if (interactable != null)
        {
            interactable.inRange = true;
            if (interactable == highlightedInteractable) CursorManager.SetTransparency(false);
        }
    }
    private void OnInteractionZoneTriggerExit(Collider collider)
    {
        Interactable interactable = collider.gameObject.GetComponent<Interactable>();
        if (interactable != null)
        {
            interactable.inRange = false;
            if (interactable == highlightedInteractable) CursorManager.SetTransparency(true);
        }
    }
    #endregion
    
    
    
    private void HighlightAtMousePosition()
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
                return;
                
        }

        //entered area of interactable, changing highlightedInteractable
        if (interactable != null && interactable != highlightedInteractable)
        {
            CursorManager.ChangeCursorInteraction(interactable);
            if (!highlightingAll)
            {
                highlightedInteractable?.Unhighlight();
                interactable.Highlight();
            }
            highlightedInteractable = interactable;
        } 
        //exited area of interactable
        else if (interactable == null && highlightedInteractable != null)
        {
            CursorManager.ResetCursor();
            if (!highlightingAll)
            {
                highlightedInteractable.Unhighlight();
                
            }
            highlightedInteractable = null;
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
    
    private void OnDrawGizmos()
    {
        if (drawInteractionZone)
        {
            if (interactionZone.TryGetComponent(out SphereCollider collider))
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(collider.transform.position, collider.radius);
            }
        }
        
        if (drawHighlightZone)
        {
            if (highlightZone.TryGetComponent(out SphereCollider collider))
            {
                Gizmos.color = Color.crimson;
                Gizmos.DrawWireSphere(collider.transform.position, collider.radius);
            }
        }
    }
    
}
