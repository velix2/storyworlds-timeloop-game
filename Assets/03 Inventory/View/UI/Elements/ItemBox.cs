using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemBox : InteractableUI
{
    [SerializeField] private Image image;

    [Header("Debug")] public ItemData heldItem;

    public UnityEvent<ItemData> BoxClickedPrimary;
    public UnityEvent<ItemData> BoxClickedSecondary;
    
    
    public void DisplayItem(ItemData item)
    {
        if (item == null)
        {
            RemoveDisplayedItem();
        }
        else
        {
            heldItem = item;
            image.sprite = item.Sprite;
            image.raycastTarget = true;
            Color c = image.color;
            c.a = 1f;
            image.color = c;
        }
        
    }

    public void RemoveDisplayedItem()
    {
        image.sprite = null;
        image.raycastTarget = false;
        Color c = image.color;
        c.a = 0;
        image.color = c;
    }

    public override interactionType Primary => interactionType.GRAB;
    public override interactionType Secondary => interactionType.LOOK;

    public override void PrimaryInteraction()
    {
        BoxClickedPrimary.Invoke(heldItem);
    }

    public override void SecondaryInteraction()
    {
        BoxClickedSecondary.Invoke(heldItem);
    }
    public override bool ItemInteraction(ItemData otherItem)
    {
        if (otherItem != heldItem)
        //InventoryManager will handle the compatibility for combination and the removal if needed
        ItemData.AttemptItemCombination.Invoke(heldItem, otherItem);
        
        //don't let InteractionChecker handle the removal
        return false;
    }
}
