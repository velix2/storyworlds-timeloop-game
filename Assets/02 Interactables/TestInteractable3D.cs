using UnityEngine;

public class TestInteractable3D : InteractableThreeDimensional
{
    public override interactionType Primary => interactionType.SPEAK;
    public override interactionType Secondary => interactionType.LOOK;

    public override void PrimaryInteraction()
    {
        print("Primary on TestInteractable");
    }
    
    public override void SecondaryInteraction()
    {
        print("Secondary on TestInteractable");
    }
}
