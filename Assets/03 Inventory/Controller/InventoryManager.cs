using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controller Class used to have a handle on what items are in the inventory.
/// </summary>
public class InventoryManager : MonoBehaviour
{
    private InventoryDisplay display;
    public List<ItemData> items = new List<ItemData>();

    public UnityEvent<ItemData> ItemSelected => display.ItemBoxPrimaryInteract;
    public UnityEvent<ItemData> ItemObserved => display.ItemBoxSecondaryInteract;
    

    private void Awake()
    {
        display = FindFirstObjectByType<InventoryDisplay>();
        if (display == null)
        {
            Debug.LogError("InventoryDisplay for Inventory Manager was not found.");
            return;
        }
        
    }

    private void Start()
    {
        foreach (var itemData in items)
        {
            display.AddItemToDisplay(itemData);
        }
    }

    public bool AddItem(ItemData item)
    {
        items.Add(item);
        display.AddItemToDisplay(item);
        
        //TODO: capacity check
        return true;
    }

    public void RemoveItem(ItemData item)
    {
        items.Remove(item);
        display.RemoveItemToDisplay(item);
    }

    /// <summary>
    /// Shows inventory display. Use method from PlayerController to stop player movement.
    /// </summary>
    public void Open()
    {
        display.ShowDisplay();
    }

    /// <summary>
    /// Hide inventory display. Use method from PlayerController to stop player movement.
    /// </summary>
    public void Close()
    {
        display.HideDisplay();
    }
    
}
