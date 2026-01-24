using System;
using UnityEngine;

public class EvelynnQuestAfterLeaveTrigger : MonoBehaviour
{
    [SerializeField] private TextAsset dialogueJson;

    private void Start()
    {
        if (StateTracker.Evelyn.QuestState != StateTracker.EvelynState.QuestStates.TriedLeavingTown)
        {
            Destroy(gameObject);
            return;
        }

        StateTracker.Evelyn.QuestState = StateTracker.EvelynState.QuestStates.Completed;
        DialogueManager.Instance.EnterDialogueMode(dialogueJson);
    }
}
