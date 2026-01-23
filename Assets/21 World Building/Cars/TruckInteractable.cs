using UnityEngine;

public class TruckInteractable : InteractableThreeDimensional
{
    private string observationText
    {
        get
        {
            return StateTracker.EvelynQuestState < StateTracker.EvelynQuestStates.TalkedTo
                ? "Wem gehört der Truck?"
                : "Evelyn's Truck.";

        }
    }
    
    public override interactionType Primary => interactionType.NONE;
    public override interactionType Secondary => interactionType.LOOK;
    public override bool PrimaryNeedsInRange => true;
    public override bool SecondaryNeedsInRange => false;
    public override void PrimaryInteraction()
    {
    }

    public override void SecondaryInteraction()
    {
        DialogueManager.Instance.EnterDialogueModeSimple(observationText);
    }
}
