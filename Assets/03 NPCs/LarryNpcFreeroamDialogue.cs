using System.Collections.Generic;
using System.Linq;
using NPCs.NpcCharacter;
using TimeManagement;
using UnityEngine;

[RequireComponent(typeof(NpcCharacter))]
public class LarryNpcFreeroamDialogue : InteractableTwoDimensional
{
    [System.Serializable]
    private class TimePhaseToDialogueMapping
    {
        public TimePhase timePhase;
        public TextAsset dialogueJson;
    }

    [SerializeField] private List<TimePhaseToDialogueMapping> _timePhaseToDialogueMappings = new();


    private NpcCharacter _npcCharacter;

    [SerializeField]
    private string lookDialogue = "Larry, mein Gastgeber. Schusseliger Kerl, aber immerhin sehr hilfsbereit.";

    public override interactionType Primary =>
        CanBeSpokenTo
            ? interactionType.SPEAK
            : interactionType.NONE;

    public override interactionType Secondary => interactionType.LOOK;
    public override bool PrimaryNeedsInRange => true;
    public override bool SecondaryNeedsInRange => false;

    public override void PrimaryInteraction()
    {
        if (!CanBeSpokenTo) return;
        
        var mapping = _timePhaseToDialogueMappings.FirstOrDefault(mapping =>
            mapping.timePhase.IsBetween(TimeHandler.Instance.CurrentTime));
        DialogueManager.Instance.EnterDialogueMode(mapping?.dialogueJson);
    }

    public override void SecondaryInteraction()
    {
        DialogueManager.Instance.EnterDialogueModeSimple(lookDialogue);
    }

    private bool CanBeSpokenTo => _npcCharacter.CanBeSpokenTo &&
                                  _timePhaseToDialogueMappings.Any(mapping =>
                                      mapping.timePhase.IsBetween(TimeHandler.Instance.CurrentTime));
}