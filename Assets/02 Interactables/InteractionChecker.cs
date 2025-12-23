using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class InteractionChecker : MonoBehaviour
{
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
    
    [SerializeField] private GameObject debugBall;
    
    private void Awake()
    {
        InputManager.PrimaryInteraction += OnPrimaryInteraction;
        InputManager.SecondaryInteraction += OnSecondaryInteraction;
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
            default:
                interactable = null;
                break;
        }
        CursorManager.ChangeCursorInteraction(interactable);
        interactable.Highlight();
        
    }

    private void OnPrimaryInteraction(Vector3 mousePosition)
    {
        if (selectedItem != null)
        {
            //TODO: deselect item
        }
        
        CheckInteractionHit(mousePosition)?.PrimaryInteraction();
    }
    
    private void OnSecondaryInteraction(Vector3 mousePosition)
    {
        if (selectedItem != null)
        {
            //TODO: item interaction
        }
        CheckInteractionHit(mousePosition)?.SecondaryInteraction();
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

        print(hit.collider);
        if (hit.collider != null)
        {
            print("Hit Interactable!");
            debugBall.transform.position = hit.collider.transform.position;
        }
        else
        {
            debugBall.transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
        }
        
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
