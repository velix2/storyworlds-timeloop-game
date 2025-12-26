using System;
using UnityEngine;

public class ItemHolder : InteractableThreeDimensional
{
    [SerializeField] private ItemData item;

    public override interactionType Primary => interactionType.GRAB;
    public override interactionType Secondary => interactionType.LOOK;
    public override bool PrimaryNeedsInRange => true;
    public override bool SecondaryNeedsInRange => false;

    public override void PrimaryInteraction()
    {
        //TODO: Pickup and add to Inventory
    }

    public override void SecondaryInteraction()
    {
        //TODO: Display observationText from itemData, dialogue system needed
    }
    
    
}
