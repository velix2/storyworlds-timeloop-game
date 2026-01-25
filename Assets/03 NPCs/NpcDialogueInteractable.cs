using UnityEngine;

public class NpcDialogueInteractable : InteractableTwoDimensional
{
    [SerializeField] private string lookDialogue;
    [SerializeField] private TextAsset dialogueJson;
    
    
    public override interactionType Primary => interactionType.SPEAK;
    public override interactionType Secondary => interactionType.LOOK;
    public override bool PrimaryNeedsInRange => true;
    public override bool SecondaryNeedsInRange => false;
    public override void PrimaryInteraction()
    {
        DialogueManager.Instance.EnterDialogueMode(dialogueJson);
    }

    public override void SecondaryInteraction()
    {
        DialogueManager.Instance.EnterDialogueModeSimple(lookDialogue);
    }
}
