using UnityEngine;


/// <summary>
/// Interactable which can be looked at with LMB. Then, single line of dialogue is played as SimpleDialogue.
/// </summary>
public class ReadableInteractable : InteractableThreeDimensional
{
    [SerializeField] private string dialogueLine;
    
    
    public override interactionType Primary => interactionType.LOOK;
    public override interactionType Secondary => interactionType.NONE;
    public override bool PrimaryNeedsInRange => true;
    public override bool SecondaryNeedsInRange => true;
    public override void PrimaryInteraction()
    {
        DialogueManager.Instance.EnterDialogueModeSimple(dialogueLine);
    }

    public override void SecondaryInteraction()
    {
    }
}
