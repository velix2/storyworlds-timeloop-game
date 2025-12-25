using UnityEngine;

public class TestInteractable : Interactable
{

    public override void PrimaryInteraction()
    {
        print("Primary on TestInteractable");
        DialogueManager.GetInstance().EnterDialogueMode(inkJson);
    }

    public override void SecondaryInteraction()
    {
        print("Secondary on TestInteractable");
    }
}
