using System;
using UnityEngine;

public class InteractionChecker : MonoBehaviour
{

    [SerializeField] private GameObject debugBall;
    private int layerMask;
    private void Awake()
    {
        InputManager.PrimaryInteraction += OnPrimaryInteraction;
        InputManager.SecondaryInteraction += OnSecondaryInteraction;
    }

    private void Start()
    {
        layerMask = LayerMask.GetMask(Interactable.LayerName);
    }

    private void OnPrimaryInteraction(Vector3 mousePosition)
    {
        CheckInteractionHit(mousePosition)?.PrimaryInteraction();
    }
    
    private void OnSecondaryInteraction(Vector3 mousePosition)
    {
        CheckInteractionHit(mousePosition)?.SecondaryInteraction();
    }
    
    private Interactable CheckInteractionHit(Vector3 mousePosition)
    {
        // Deactivate interaction while in dialogue
        if (DialogueManager.GetInstance().dialogueIsPlaying) return null;

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
    
}
