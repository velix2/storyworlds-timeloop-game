using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Resource/ItemData", fileName = "New ItemData")][Serializable]
public class ItemData : ScriptableObject
{

    public static UnityEvent<ItemData, ItemData> AttemptItemCombination = new();
    
    [SerializeField] private string itemName;
    public string ItemName => itemName;

    [SerializeField] private string observationText;
    public string ObservationText => observationText;

    [SerializeField] private Sprite sprite;
    public Sprite Sprite => sprite;

    public override string ToString()
    {
        return itemName;
    }
}
