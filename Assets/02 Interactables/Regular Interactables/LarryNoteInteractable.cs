using UnityEngine;

public class LarryNoteInteractable : InteractableThreeDimensional
{
    [SerializeField] private string noteText;

    private string observationText
    {
        get
        {
            if (StateTracker.IntroState >= StateTracker.IntroStates.SonCallRepeatCompleted)
                return "Der Zettel liegt ja immer noch da.";
            return "Larry hat mir eine Notiz hinterlassen.";
        }
    }
    
    public override Interactable.interactionType Primary => Interactable.interactionType.INSPECT;
    public override Interactable.interactionType Secondary => Interactable.interactionType.LOOK;
    public override bool PrimaryNeedsInRange => true;
    public override bool SecondaryNeedsInRange => true;
    public override void PrimaryInteraction()
    {
        DialogueManager.Instance.EnterDialogueModeSimple(noteText);
    }

    public override void SecondaryInteraction()
    {
        DialogueManager.Instance.EnterDialogueModeSimple(observationText);
    }
}
