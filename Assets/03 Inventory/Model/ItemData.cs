using System;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Resource/ItemData", fileName = "New ItemData")][Serializable]
public class ItemData : ScriptableObject
{

    /// <summary>
    /// Event to signal, that the player is trying to combine two Items.<br/>
    /// Will be caught by InventoryManager, which removes/adds Items accordingly.
    /// </summary>
    /// <param name="item1, item2">The items to be combined</param>
    public static UnityEvent<ItemData, ItemData> AttemptItemCombination = new();
    
    [SerializeField] private string itemName;
    public string ItemName => itemName;

    [SerializeField] private TextAsset observationText;
    public TextAsset ObservationText => observationText;

    [SerializeField] private bool multiplePossible;
    public bool MultiplePossible => multiplePossible;
    
    [SerializeField] private Sprite sprite;
    public Sprite Sprite => sprite;

    public ItemData MakeCopy()
    {
        ItemData item = Instantiate(this);
        return item;
    }

    public override string ToString()
    {
        return itemName;
    }

    public override bool Equals(object other)
    {
        return other is ItemData && itemName.Equals(((ItemData)other).itemName);
    }
}
