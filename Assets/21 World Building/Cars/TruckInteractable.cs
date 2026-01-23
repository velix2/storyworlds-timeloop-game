using UnityEngine;

public class TruckInteractable : InteractableThreeDimensional
{
    // TODO implement with Quest Logic
    [SerializeField] private string dialogueLine = "[Ich will implementiert werden!!!]";
    
    public override interactionType Primary => interactionType.NONE;
    public override interactionType Secondary => interactionType.LOOK;
    public override bool PrimaryNeedsInRange => true;
    public override bool SecondaryNeedsInRange => true;
    public override void PrimaryInteraction()
    {
    }

    public override void SecondaryInteraction()
    {
        DialogueManager.Instance.EnterDialogueModeSimple(dialogueLine);
    }
}
