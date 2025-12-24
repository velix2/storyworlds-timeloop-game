using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.UI;

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
        
        display.ItemBoxPrimaryInteract.AddListener(OnDisplayItemPrimaryInteraction);
        display.ItemBoxSecondaryInteract.AddListener(OnDisplayItemSecondaryInteraction);
    }

    private void Start()
    {
        foreach (var itemData in items)
        {
            display.AddItemToDisplay(itemData);
        }
    }

    /// <summary>
    /// Use Open method in PlayerController
    /// </summary>
    public void Open()
    {
        display.ShowDisplay();
    }

    /// <summary>
    /// Use Close method in PlayerController
    /// </summary>
    public void Close()
    {
        display.HideDisplay();
    }


    private void OnDisplayItemPrimaryInteraction(ItemData item)
    {
        //TODO: Select item to interact with others
        Debug.Log($"Item {item} selected in inventory");
    }

    private void OnDisplayItemSecondaryInteraction(ItemData item)
    {
        //TODO: Display observation text
        Debug.Log($"Item {item} observed in inventory");
        
    }
}
