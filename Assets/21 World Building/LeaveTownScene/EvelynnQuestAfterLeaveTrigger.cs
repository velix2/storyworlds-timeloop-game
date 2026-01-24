using System;
using UnityEngine;

public class EvelynnQuestAfterLeaveTrigger : MonoBehaviour
{
    [SerializeField] private TextAsset dialogueJson;

    private void Start()
    {
        if (StateTracker.EvelynQuestState != StateTracker.EvelynQuestStates.TriedLeavingTown)
        {
            Destroy(gameObject);
            return;
        }

        StateTracker.EvelynQuestState = StateTracker.EvelynQuestStates.QuestCompleted;
        DialogueManager.Instance.EnterDialogueMode(dialogueJson);
    }
}
