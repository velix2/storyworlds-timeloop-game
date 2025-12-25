using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemBox : InteractableUI
{
    [SerializeField] private Image image;
    
    [Header("Debug")]
    public byte id;

    public UnityEvent<int> BoxClickedPrimary;
    public UnityEvent<int> BoxClickedSecondary;
    
    public void DisplayItem(ItemData item)
    {
        if (item == null)
        {
            RemoveDisplayedItem();
        }
        else
        {
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
        BoxClickedPrimary.Invoke(id);
    }

    public override void SecondaryInteraction()
    {
        BoxClickedSecondary.Invoke(id);
    }
    
    public override bool ItemInteraction(ItemData otherItem)
    {
        //TODO: Item Combination
        return false;
    }
}
