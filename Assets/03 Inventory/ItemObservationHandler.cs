using System;
using UnityEngine;

public class ItemObservationHandler : MonoBehaviour
{
    private void Start()
    {
        InventoryManager.Instance.ItemObserved.AddListener(OnItemObserved);
    }

    private void OnItemObserved(ItemData item)
    {
        DialogueManager.Instance.EnterDialogueModeSimple(item.ObservationText);
    }
}

