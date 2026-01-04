using UnityEngine;

public class TestInteractable2D : InteractableTwoDimensional
{
    public override interactionType Primary => primary;
    public override interactionType Secondary => secondary;
    public override bool PrimaryNeedsInRange => primaryNeedsInRange;
    public override bool SecondaryNeedsInRange => secondaryNeedsInRange;

    [SerializeField] private interactionType primary;
    [SerializeField] private interactionType secondary;
    [SerializeField] private bool primaryNeedsInRange = true;
    [SerializeField] private bool secondaryNeedsInRange = false;
    
    public override void PrimaryInteraction()
    {
        print("prim");
    }

    public override void SecondaryInteraction()
    {
        print("sec");
    }
    
}