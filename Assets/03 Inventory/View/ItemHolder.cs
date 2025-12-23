using UnityEngine;

public class ItemHolder : Interactable
{
    [SerializeField] private ItemData item;
    public override void PrimaryInteraction()
    {
        //TODO: Pickup and add to Inventory
    }

    public override void SecondaryInteraction()
    {
        //TODO: Display observationText from itemData
    }
}
