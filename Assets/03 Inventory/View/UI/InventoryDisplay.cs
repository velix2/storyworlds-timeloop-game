using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Object = UnityEngine.Object;

[RequireComponent(typeof(CanvasGroup))]
public class InventoryDisplay : MonoBehaviour
{
    [HideInInspector] public UnityEvent<ItemData> ItemBoxPrimaryInteract;
    [HideInInspector] public UnityEvent<ItemData> ItemBoxSecondaryInteract;
    
    [Header("References")]
    [SerializeField] private GameObject itemBoxContainer;
    private CanvasGroup canvasGroup;

    private ItemBox[] itemBoxes;
    private ItemData[] itemMap;
    private byte nextEmptyIndex = 0;
    
    private void Awake()
    {
        itemBoxes = itemBoxContainer.GetComponentsInChildren<ItemBox>();
        for (byte i = 0; i <  itemBoxes.Length; i++)
        {
            itemBoxes[i].id = i;
            itemBoxes[i].RemoveDisplay();
            itemBoxes[i].BoxClickedPrimary.AddListener(OnItemBoxClickedPrimary);
            itemBoxes[i].BoxClickedSecondary.AddListener(OnItemBoxClickedSecondary);
        }

        itemMap = new ItemData[itemBoxes.Length];

        canvasGroup = GetComponent<CanvasGroup>();

    }

    /// <summary>
    /// Use Open method in PlayerController
    /// </summary>
    public void ShowDisplay()
    {
        //TODO: do the following with animations
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

    }

    /// <summary>
    /// Use Close method in PlayerController
    /// </summary>
    public void HideDisplay()
    {
        //TODO: do the following with animations
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    public void AddItemToDisplay(ItemData item)
    {
        if (nextEmptyIndex >= itemBoxes.Length)
        {
            Debug.LogError("Tried adding another item to an already full item display.");
            return;
        }
        
        itemBoxes[nextEmptyIndex].DisplayItem(item);
        itemMap[nextEmptyIndex++] = item;
    }

    public ItemData RemoveItemToDisplay(ItemData item)
    {
        int index = -1;
        for (byte i = 0; i <  itemMap.Length; i++)
        {
            if (itemMap[i] == item)
            {
                index = i;
                break;
            }
        }

        if (index == -1)
        {
            Debug.Log($"Tried deleting {item}, but couldn't be found within item array of display.");
            return null;
        }
        
        
        ItemData result = itemMap[index];
        
        while (index + 1 < itemBoxes.Length && itemMap[index + 1] != null )
        {
            itemMap[index] = itemMap[index + 1];
            itemBoxes[index].DisplayItem(itemMap[index]);

            index++;
        }
        
        itemMap[index] = null;
        itemBoxes[index].RemoveDisplay();

        nextEmptyIndex = (byte) index;
        
        return result;
    }
    
    #region SignalHandlers
    private void OnItemBoxClickedPrimary(int id)
    {
        if (itemMap[id] == null)
        {
            Debug.Log($"Nothing inside the clicked item box with id: {id}.");
            return;
        }
        
        ItemBoxPrimaryInteract.Invoke(itemMap[id]);
    }

    private void OnItemBoxClickedSecondary(int id)
    {
        if (itemMap[id] == null)
        {
            Debug.Log($"Nothing inside the clicked item box with id: {id}.");
            return;
        }
        
        ItemBoxSecondaryInteract.Invoke(itemMap[id]);
    }
    #endregion
}
