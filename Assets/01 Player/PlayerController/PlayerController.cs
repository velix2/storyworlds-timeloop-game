using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController)), RequireComponent(typeof(InteractionChecker)), RequireComponent(typeof(InventoryManager))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private CharacterController controller;
    [SerializeField] private InteractionChecker interactionChecker;
    [SerializeField] private InventoryManager inventoryManager;

    [Header("Parameters")] 
    [SerializeField] private float speed = 10f;

    private bool movementBlocked;
    private bool frozen;

    private void Start()
    {
        inventoryManager.ItemSelected.AddListener(interactionChecker.SelectItem);
        //TODO: AddListener for ItemObservation event, DialogueSystem needed
        
        interactionChecker.ItemExhausted.AddListener(inventoryManager.RemoveItem);
        
        InputManager.PlayerControls.Standard.InventoryOpen.performed += _ => OnInventoryOpenInput();
        InputManager.PlayerControls.Standard.InventoryClose.performed += _ => OnInventoryCloseInput();

        InputManager.PlayerControls.Standard.HighlightAllInteractables.started += _ => OnHighlightAllInput(true);
        InputManager.PlayerControls.Standard.HighlightAllInteractables.canceled += _ => OnHighlightAllInput(false);
        
        OnInventoryCloseInput();
    }


    void Update()
    {
        if(DialogueManager.GetInstance() != null)
        {
            if (DialogueManager.GetInstance().dialogueIsPlaying) return;
        }

    
        if (frozen) return;
        Vector3 move;
        if (!movementBlocked)
        {
            Vector2 input = InputManager.GetPlayerMovement();
            move = new Vector3(input.x, 0, input.y);
            move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
            move.y = 0;
            move = Vector3.ClampMagnitude(move, 1f) * speed;
        }
        else move = Vector3.zero;
        
        Vector3 finalMove = move + Vector3.down * 9.81f;
        controller.Move(finalMove * Time.deltaTime);
        
    }

    private void OnInventoryOpenInput()
    {
        if (frozen) return;
        movementBlocked = true;
        interactionChecker.SetToUIMode();
        inventoryManager.Open();
    }

    private void OnInventoryCloseInput()
    {
        if (frozen) return;
        movementBlocked = false;
        interactionChecker.SetToPhysicsMode();
        inventoryManager.Close();
    }

    private void OnHighlightAllInput(bool pressed)
    {
        if (frozen) return;
        
        if (pressed)
        {
            interactionChecker.HighlightAll();
        }
        else
        {
            interactionChecker.UnhighlightAll();
        }
    }
    
}
