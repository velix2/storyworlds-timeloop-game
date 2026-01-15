using System;
using System.Collections;
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
    [SerializeField] private AudioClip openAudio;
    [SerializeField] private AudioClip closeAudio;
    [SerializeField] private AudioClip pickupAudio;
    [SerializeField] private GameObject itemBoxContainer;
    private Animator animator;
    private CanvasGroup canvasGroup;

    private ItemBox[] itemBoxes;
    private byte nextEmptyIndex = 0;
    public int Capacity => itemBoxes.Length;

    private AnimationClip openClip;
    private bool animating;
    public bool Animating => animating;

    private void Awake()
    {
        itemBoxes = itemBoxContainer.GetComponentsInChildren<ItemBox>();
        for (byte i = 0; i <  Capacity; i++)
        {
            itemBoxes[i].RemoveDisplayedItem();
            itemBoxes[i].BoxClickedPrimary.AddListener(OnItemBoxClickedPrimary);
            itemBoxes[i].BoxClickedSecondary.AddListener(OnItemBoxClickedSecondary);
        }
        
        canvasGroup = GetComponent<CanvasGroup>();
        animator = GetComponent<Animator>();
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "Inventory_Open")
            {
                openClip = clip;
                break;
            }
        }
        HideDisplayInstant();
    }

    /// <summary>
    /// Shows inventory display. Use method from PlayerController to stop player movement.
    /// </summary>
    public void ShowDisplay()
    {
        canvasGroup.alpha = 1;
        AudioManager.PlaySFX(openAudio);
        StartCoroutine(PlayAnimation(true));
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

    }

    /// <summary>
    /// Hide inventory display. Use method from PlayerController to stop player movement.
    /// </summary>
    public void HideDisplay()
    {
        print("hide");
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        AudioManager.PlaySFX(closeAudio);
        StartCoroutine(PlayAnimation(false));
        
    }

    private void HideDisplayInstant()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0;
    }

    private IEnumerator PlayAnimation(bool forward)
    {
        animating = true;
        animator.Play(forward ? "Open" : "Close");
        yield return new WaitForSeconds(openClip.length);
        if (!forward) canvasGroup.alpha = 0;
        animating = false;
    }
    
    public void AddItemToDisplay(ItemData item, bool playSound = false)
    {
        if (nextEmptyIndex >= itemBoxes.Length)
        {
            Debug.LogError("Tried adding another item to an already full item display.");
            return;
        }
        
        if (playSound) AudioManager.PlaySFX(pickupAudio);
        itemBoxes[nextEmptyIndex++].DisplayItem(item);
    }

    public ItemData RemoveItemToDisplay(ItemData item)
    {
        int index = -1;
        for (byte i = 0; i < Capacity; i++)
        {
            if (itemBoxes[i].heldItem.Equals(item))
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
        
        print(index);
        ItemData result = itemBoxes[index].heldItem;
        
        while (index + 1 < Capacity && itemBoxes[index + 1].heldItem != null )
        {
            itemBoxes[index].DisplayItem(itemBoxes[index + 1].heldItem);
            index++;
        }
        
        itemBoxes[index].RemoveDisplayedItem();
        nextEmptyIndex = (byte) index;
        
        return result;
    }
    
    #region SignalHandlers
    private void OnItemBoxClickedPrimary(ItemData item)
    {
        if (item == null)
        {
            Debug.Log($"Nothing inside the clicked item box.");
            return;
        }
        
        ItemBoxPrimaryInteract.Invoke(item);
    }

    private void OnItemBoxClickedSecondary(ItemData item)
    {
        if (item == null)
        {
            Debug.Log($"Nothing inside the clicked item box.");
            return;
        }
        
        ItemBoxSecondaryInteract.Invoke(item);
    }
    #endregion
}
