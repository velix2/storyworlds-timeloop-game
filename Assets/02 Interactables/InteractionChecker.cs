using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
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
        HighlightSingle(GetInteractableAtMousePosition());
    }
    private void HighlightSingle(Interactable interactable)
    {
        //changing highlightedInteractable, if not already highlighted
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
        //no interactable to highlight
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
        HighlightSingle(GetInteractableAtMousePosition());
    }
    
    private void OnPrimaryInteractionInput(Vector3 mousePosition)
    {

        if (highlightedInteractable != null && highlightedInteractable.inRange)
        {
            if (selectedItem != null)
            {
                if (highlightedInteractable.ItemInteraction(selectedItem))
                {
                    ItemExhausted.Invoke(selectedItem);
                }
                DeselectItem();
            }
            else highlightedInteractable.PrimaryInteraction();
            UpdateHighlightedInteractableStatusAtMousePosition();
        }
    }
    private void OnSecondaryInteractionInput(Vector3 mousePosition)
    {
        if (selectedItem != null)
        {
            DeselectItem();
            return;
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

    private void UpdateHighlightedInteractableStatusAtMousePosition()
    {
        highlightedInteractable = null;
        HighlightSingle(GetInteractableAtMousePosition());
    }
    
    private Interactable GetInteractableAtMousePosition()
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
                interactable = null;
                break;
            default:
                Debug.Log($"Mode {mode} is not implemented.");
                goto case interactionModes.DISABLED;
                
        }

        return interactable;
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

    #region Raycasts

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
