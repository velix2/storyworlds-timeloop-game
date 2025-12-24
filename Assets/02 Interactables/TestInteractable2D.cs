public class TestInteractable2D : InteractableTwoDimensional
{
    public override interactionType Primary => interactionType.GRAB;
    public override interactionType Secondary => interactionType.LOOK;
    public override void PrimaryInteraction()
    {
        print("prim");
    }

    public override void SecondaryInteraction()
    {
        print("sec");
    }
}