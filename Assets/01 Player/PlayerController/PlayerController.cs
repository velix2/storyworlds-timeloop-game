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

    public static bool movementBlocked;
    private bool inventoryOpen;
    

    private Collider[] colliders;

    private void Awake()
    {
        colliders = GetComponentsInChildren<Collider>(true);
    }

    private void Start()
    {
        inventoryManager = FindFirstObjectByType<InventoryManager>();
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
        
        Vector3 move;
        if (!movementBlocked)
        {
            bool sprinting = InputManager.GetSprinting();
            Vector2 input = InputManager.GetPlayerMovement();
            move = new Vector3(input.x, 0, input.y);
            move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
            move.y = 0;
            move = Vector3.ClampMagnitude(move, 1f) * (sprinting? 2.0f * speed : speed);
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
        DialogueManager.DialogueModeChanged += OnDialogueModeChanged;
    }

    private void OnDisable()
    {
        CutsceneManager.CutsceneStarted -= OnCutsceneStart;
        CutsceneManager.CutsceneEnded -= OnCutsceneEnd;
        CutsceneManager.CutsceneContinue -= OnCutsceneContinue;
        CutsceneManager.CutscenePaused -= OnCutscenePaused;
        DialogueManager.DialogueModeChanged -= OnDialogueModeChanged;
    }

    private void OnCutsceneStart()
    {
        animator.applyRootMotion = true;
        InputManager.PlayerControls.Standard.Disable();
        foreach (var col in colliders)
            col.enabled = false;
        OnInventoryCloseInput();
        interactionChecker.SetToDisabledMode();
    }

    private void OnCutsceneEnd()
    {
        animator.applyRootMotion = false;
        InputManager.PlayerControls.Standard.Enable();
        UnfreezeAnimation();
        foreach (var col in colliders)
            col.enabled = true;
        interactionChecker.SetToPhysicsMode();
    }

    private void OnCutsceneContinue()
    {
        UnfreezeAnimation();
    }

    private void OnCutscenePaused()
    {
        FreezeAnimation();
    }

    private void OnDialogueModeChanged()
    {
        if(DialogueManager.Instance.DialogueIsPlaying)
        {
            InputManager.PlayerControls.Standard.Disable();
            FreezeAnimation();
        }
        else
        {
            InputManager.PlayerControls.Standard.Enable();
            UnfreezeAnimation();
        }
    }
    private void OnInventoryOpenInput()
    {
        if (inventoryOpen) return;
        if (!inventoryManager.IsReady) return;
        if (DialogueManager.Instance.DialogueIsPlaying) return;
        inventoryOpen = true;
        FreezeAnimation();
        movementBlocked = true;
        interactionChecker.SetToUIMode();
        inventoryManager.Open();
    }

    private void OnInventoryCloseInput()
    {
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
        if (movementBlocked) return;
        
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

    private int animatorLookX = Animator.StringToHash("lookX");
    private int animatorLookY = Animator.StringToHash("lookY");
    private int animatorMoveX = Animator.StringToHash("moveX");
    private int animatorMoveY = Animator.StringToHash("moveY");
    private int animatorSprinting = Animator.StringToHash("sprinting");
    private int magnitude = Animator.StringToHash("moveMagnitude");
    
    private void LateUpdate()
    {
        if (DialogueManager.Instance.DialogueIsPlaying) return;
        if (movementBlocked)
        {
            animator.SetFloat(magnitude, 0);
            return;
        }
        Vector2 input = InputManager.GetPlayerMovement();
        AnimatorSprinting(InputManager.GetSprinting());
        AnimatorWalkDirection(input);
    }
    private void AnimatorWalkDirection(Vector2 value)
    {
        animator.SetFloat(animatorMoveY, value.y);
        animator.SetFloat(animatorMoveX, value.x);
        float mag = value.sqrMagnitude;
        animator.SetFloat(magnitude, mag);
        if (mag > 0.1) AnimatorLookDirection(value);
        
    }

    private void AnimatorLookDirection(Vector2 value)
    {
        animator.SetFloat(animatorLookX, value.x);
        animator.SetFloat(animatorLookY, value.y);
    }

    private void AnimatorSprinting(bool value)
    {
        animator.SetBool(animatorSprinting, value);
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
