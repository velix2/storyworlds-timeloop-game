using UnityEngine;

public class Test : InteractableThreeDimensional
{
    public override interactionType Primary => interactionType.SPEAK;
    public override interactionType Secondary => interactionType.LOOK;

    public override void PrimaryInteraction()
    {
        Highlight();
        print("Primary on TestInteractable");
    }
    
    public override void SecondaryInteraction()
    {
        Unhighlight();
        print("Secondary on TestInteractable");
    }
}
