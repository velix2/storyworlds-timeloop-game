using UnityEngine;

[RequireComponent(typeof(CharacterController)), RequireComponent(typeof(InteractionChecker))]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private CharacterController controller;
    [SerializeField] private InteractionChecker interactionChecker;
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private Animator animator;

    [Header("Parameters")] 
    [SerializeField] private float speed = 10f;

    private bool movementBlocked;
    private bool frozen;
    private bool inventoryOpen;

    private Collider[] colliders;

    private void Awake()
    {
        colliders = GetComponentsInChildren<Collider>(true);
    }

    private void Start()
    {
        //IDs for animator variables
        lookX = Animator.StringToHash("lookX");
        lookY = Animator.StringToHash("lookY");
        moveX = Animator.StringToHash("moveX");
        moveY = Animator.StringToHash("moveY");
        magnitude = Animator.StringToHash("moveMagnitude");

        inventoryManager = InventoryManager.Instance;
        Debug.Log(inventoryManager.ItemSelected);
        inventoryManager.ItemSelected.AddListener(interactionChecker.SelectItem);
        interactionChecker.ItemExhausted.AddListener(inventoryManager.RemoveItem);
        
        InputManager.PlayerControls.Standard.InventoryOpen.performed += _ => OnInventoryOpenInput();
        InputManager.PlayerControls.Standard.InventoryClose.performed += _ => OnInventoryCloseInput();

        InputManager.PlayerControls.Standard.HighlightAllInteractables.started += _ => OnHighlightAllInput(true);
        InputManager.PlayerControls.Standard.HighlightAllInteractables.canceled += _ => OnHighlightAllInput(false);
    }


    void Update()
    {
        if(DialogueManager.Instance != null && CutsceneManager.Instance != null)
        {
            if (DialogueManager.Instance.DialogueIsPlaying) return;
            if (CutsceneManager.Instance.CutsceneIsPlaying) return;
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

    private void OnEnable()
    {
        CutsceneManager.CutsceneStarted += OnCutsceneStart;
        CutsceneManager.CutsceneEnded += OnCutsceneEnd;
        CutsceneManager.CutsceneContinue += OnCutsceneContinue;
        CutsceneManager.CutscenePaused += OnCutscenePaused;
    }

    private void OnDisable()
    {
        CutsceneManager.CutsceneStarted -= OnCutsceneStart;
        CutsceneManager.CutsceneEnded -= OnCutsceneEnd;
        CutsceneManager.CutsceneContinue -= OnCutsceneContinue;
        CutsceneManager.CutscenePaused -= OnCutscenePaused;
    }

    private void OnCutsceneStart()
    {
        animator.applyRootMotion = true;
        InputManager.PlayerControls.Disable();
        foreach (var col in colliders)
            col.enabled = false;
    }

    private void OnCutsceneEnd()
    {
        animator.applyRootMotion = false;
        InputManager.PlayerControls.Enable();
        UnfreezeAnimation();
        foreach (var col in colliders)
            col.enabled = true;
    }

    private void OnCutsceneContinue()
    {
        UnfreezeAnimation();
    }

    private void OnCutscenePaused()
    {
        FreezeAnimation();
    }


    private void OnInventoryOpenInput()
    {
        if (frozen) return;
        if (inventoryOpen) return;
        if (!inventoryManager.IsReady) return;
        inventoryOpen = true;
        FreezeAnimation();
        movementBlocked = true;
        interactionChecker.SetToUIMode();
        inventoryManager.Open();
    }

    private void OnInventoryCloseInput()
    {
        if (frozen) return;
        if (!inventoryOpen) return;
        if (!inventoryManager.IsReady) return;
        inventoryOpen = false;
        UnfreezeAnimation();
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

    #region AnimationRelated

    private int lookX;
    private int lookY;
    private int moveX;
    private int moveY;
    private int magnitude;
    
    private void LateUpdate()
    {
        if (DialogueManager.Instance.DialogueIsPlaying) return;
        if (frozen) return;
        Vector2 input = InputManager.GetPlayerMovement();
        AnimatorWalkDirection(input);
    }
    private void AnimatorWalkDirection(Vector2 value)
    {
        animator.SetFloat(moveY, value.y);
        animator.SetFloat(moveX, value.x);
        float mag = value.sqrMagnitude;
        animator.SetFloat(magnitude, mag);
        if (mag > 0.1) AnimatorLookDirection(value);
        
    }

    private void AnimatorLookDirection(Vector2 value)
    {
        animator.SetFloat(lookX, value.x);
        animator.SetFloat(lookY, value.y);
    }

    public void FreezeAnimation()
    {
        animator.enabled = false;
    }

    public void UnfreezeAnimation()
    {
        animator.enabled = true;
    }
    #endregion
}
