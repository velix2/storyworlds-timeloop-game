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
        if (InventoryManager.Instance.AddItem(item, true))
        {
            Destroy(gameObject);
            Destroy(this);
        }
    }

    public override void SecondaryInteraction()
    {
        DialogueManager.Instance.EnterDialogueModeSimple(item.ObservationText);
    }
    
    
}
