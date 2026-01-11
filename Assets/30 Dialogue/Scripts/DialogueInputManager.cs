using UnityEngine;
using UnityEngine.InputSystem;
using static InputManager;

public class DialogueInputManager : MonoBehaviour
{
    public delegate void DialogueInteraction();

    public static event DialogueInteraction ContinueDialogue;
    public static event DialogueInteraction ContinueDialogueSimple;

    private static DialogueInputManager _instance;
    private DialogueControls dialogueControls;
    public static DialogueControls DialogueControls => _instance.dialogueControls;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.Log("Double Dialogue Input Manager has been destroyed");
            Destroy(gameObject);
            return;
        }
        else
        {
            _instance = this;
        }
        dialogueControls = new DialogueControls(); 
        DontDestroyOnLoad(gameObject);

    }

    private void SignalDialogueContinue(InputAction.CallbackContext context)
    {
        if (!DialogueManager.Instance.DialogueIsPlaying || DialogueManager.Instance.SimpleDialogueIsPlaying) return;
        ContinueDialogue?.Invoke();
    }
    private void SignalDialogueContinueSimple(InputAction.CallbackContext context)
    {
        if (!DialogueManager.Instance.DialogueIsPlaying || !DialogueManager.Instance.SimpleDialogueIsPlaying) return;
        ContinueDialogueSimple?.Invoke();
    }
    private void OnEnable()
    {
        if (dialogueControls != null)
        {
            dialogueControls.Standard.ContinueDialogue.performed += SignalDialogueContinue;
            dialogueControls.Standard.ContinueDialogueSimple.performed += SignalDialogueContinueSimple;
            dialogueControls.Enable();
        }
    }

    private void OnDisable()
    {
        if (dialogueControls != null)
        {
            dialogueControls.Standard.ContinueDialogue.performed -= SignalDialogueContinue;
            dialogueControls.Standard.ContinueDialogueSimple.performed -= SignalDialogueContinueSimple;
            dialogueControls.Disable();
        }
    }

}
