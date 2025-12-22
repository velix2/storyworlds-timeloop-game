using UnityEngine;

public class TestInteractable : Interactable
{
    
    
    public override void PrimaryInteraction()
    {
        print("Primary on TestInteractable");
    }

    public override void SecondaryInteraction()
    {
        print("Secondary on TestInteractable");
    }
}
