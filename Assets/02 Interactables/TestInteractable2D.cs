public class TestInteractable2D : InteractableTwoDimensional
{
    public override interactionType Primary => interactionType.GRAB;
    public override interactionType Secondary => interactionType.LOOK;
    public override bool PrimaryNeedsInRange => true;
    public override bool SecondaryNeedsInRange => false;

    public override void PrimaryInteraction()
    {
        print("prim");
    }

    public override void SecondaryInteraction()
    {
        print("sec");
    }
    
}