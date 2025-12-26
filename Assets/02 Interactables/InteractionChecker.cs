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
    /// <summary>
    /// InteractionChecker will use physics based raycast to look for Interactables.<br/>
    /// Will not find canvas based Interactables.
    /// </summary>
    public void SetToPhysicsMode() { mode = interactionModes.PHYSICS; }
    /// <summary>
    /// InteractionChecker will use canvas based raycast to look for Interactables.<br/>
    /// Will not find collider based Interactables.
    /// </summary>
    public void SetToUIMode() { mode = interactionModes.UI; }
    /// <summary>
    /// Disables the raycast all together.
    /// </summary>
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
    
    /// <summary>
    /// Highlights all Interactables inside HighlightZone.<br/>
    /// Interactables which enter/leave the HighlightZone will automatically be highlighted/unhighlighted.<br/>
    /// Can be undone by UnhighlightAll().
    /// </summary>
    public void HighlightAll()
    {
        highlightingAll = true;
        foreach (Interactable interactable in inHighlightRange)
        {
            interactable.Highlight();
        }
    }
    
    /// <summary>
    /// Undo the effect of HighlightAll().
    /// </summary>
    public void UnhighlightAll()
    {
        highlightingAll = false;
        foreach (Interactable interactable in inHighlightRange)
        {
            interactable.Unhighlight();
        }
        HighlightSingle(GetInteractableAtMousePosition());
    }
    /// <summary>
    /// Sets highlightedInteractable, if difference to current is detected. <br/>
    /// Highlights the Interactables and sets cursor accordingly.
    /// </summary>
    /// <param name="interactable"></param>
    private void HighlightSingle(Interactable interactable)
    {
        //different interactable than the one highlighted
        if (interactable != null && interactable != highlightedInteractable)
        {
            if (selectedItem) CursorManager.SetTransparency(interactable.inRange);
            else CursorManager.ChangeCursorInteraction(interactable);
            
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
            if (selectedItem) CursorManager.SetTransparency(false);
            else CursorManager.ResetCursor();
            
            if (!highlightingAll)
            {
                highlightedInteractable.Unhighlight();
                
            }
            highlightedInteractable = null;
        }
    }
    /// <summary>
    /// Sets Item to use for Item Interactions and sets cursor accordingly.<br/>
    /// Will be set to null after Interaction or DeselectItem() call.
    /// </summary>
    /// <param name="itemData">Item to use for Item Interaction</param>
    public void SelectItem(ItemData itemData)
    {
        if (itemData)
        {
            selectedItem = itemData;
            CursorManager.ChangeCursorItem(itemData);
        }
        
    }
    /// <summary>
    /// Removes Item to use for Item Interactions.
    /// </summary>
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
        //Setup event handlers
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
        if (DialogueManager.GetInstance().dialogueIsPlaying) return;

        HighlightSingle(GetInteractableAtMousePosition());
    }
    
    /// <summary>
    /// Event Handler for PrimaryInteraction Inputs.<br/>
    /// Used to trigger PrimaryInteraction() or ItemInteraction() of highlightedInteractable.
    /// </summary>
    /// <param name="mousePosition"></param>
    private void OnPrimaryInteractionInput(Vector3 mousePosition)
    {
        if (highlightedInteractable != null)
        {
            if (selectedItem != null)
            {
                if (highlightedInteractable.inRange)
                {
                    if (highlightedInteractable.ItemInteraction(selectedItem))
                    {
                        ItemExhausted.Invoke(selectedItem);
                    
                    }
                    DeselectItem();
                }
            }
            else if (!highlightedInteractable.PrimaryNeedsInRange || highlightedInteractable.inRange)
                highlightedInteractable.PrimaryInteraction();
            //after usage the state of the interactable might change, and therefore the type of interactions possible.
            //if the player still has the mouse hovered over it the old interactions would be displayed
            //here is the fix:
            highlightedInteractable = null;
            HighlightSingle(GetInteractableAtMousePosition());
        }
    }
    
    /// <summary>
    /// Event Handler for PrimaryInteraction Inputs.<br/>
    /// Used to trigger PrimaryInteraction() or ItemInteraction() of highlightedInteractable.
    /// </summary>
    /// <param name="mousePosition"></param>
    private void OnSecondaryInteractionInput(Vector3 mousePosition)
    {
        if (highlightedInteractable != null && (!highlightedInteractable.SecondaryNeedsInRange || highlightedInteractable.inRange))
        {
            if (selectedItem != null)
            {
                DeselectItem();
                return;
            }
            highlightedInteractable.SecondaryInteraction();
        }
    }

    #region Hightlight Zone
    /// <summary>
    /// Event Handler for TriggerEnter of HighlightZone.<br/>
    /// Adds Interactable to list of Interactables to highlight with HighlightAll().
    /// </summary>
    /// <param name="collider"></param>
    private void OnHightlightZoneTriggerEnter(Collider collider)
    {
        Interactable interactable = collider.gameObject.GetComponent<Interactable>();
        if (interactable != null)
        {
            inHighlightRange.Add(interactable);
            if (highlightingAll) interactable.Highlight();
        }
    }
    
    /// <summary>
    /// Event Handler for TriggerEnter of HighlightZone.<br/>
    /// Removes Interactable from list of Interactables to highlight with HighlightAll().
    /// </summary>
    /// <param name="collider"></param>
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
    /// <summary>
    /// Event handler for TriggerEnter of InteractionZone.<br/>
    /// Sets inRange value in Interactable accordingly.
    /// </summary>
    /// <param name="collider"></param>
    private void OnInteractionZoneTriggerEnter(Collider collider)
    {
        Interactable interactable = collider.gameObject.GetComponent<Interactable>();
        if (interactable != null)
        {
            interactable.inRange = true;
            if (interactable == highlightedInteractable) CursorManager.SetTransparency(false);
        }
    }
    /// <summary>
    /// Event handler for TriggerExit of InteractionZone.<br/>
    /// Sets inRange value in Interactable accordingly.
    /// </summary>
    /// <param name="collider"></param>
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
    
    /// <summary>
    /// Uses
    /// </summary>
    /// <returns></returns>
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
