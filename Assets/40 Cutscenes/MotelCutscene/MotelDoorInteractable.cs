using UnityEngine;
using UnityEngine.Timeline;

public class MotelDoorInteractable : InteractableThreeDimensional
{
    [Header("Cutscene")]
    [SerializeField] private TimelineAsset cutscene;

    [Header("Observtion Lines")] 
    [SerializeField] private string notNeeded = "Ich bin nicht lange hier.";
    [SerializeField] private string motelNeeded = "Ein Motel. Wie praktisch.";
    [SerializeField] private string avoidMotel = "Da geh ich sicher kein zweites Mal rein!";
    
    
    public override interactionType Primary
    {
        get
        {
            if ((int)StateTracker.IntroState == (int)StateTracker.IntroStates.NoTrainRunsCompleted)
            {
                return interactionType.ENTER_OPEN;
            } else if ((int)StateTracker.IntroState >= (int)StateTracker.IntroStates.MotelCompleted)
                return interactionType.NONE;
            return interactionType.ENTER_CLOSED;
        }
    }

    public override interactionType Secondary => interactionType.LOOK;
    public override bool PrimaryNeedsInRange => true;
    public override bool SecondaryNeedsInRange => false;
    public override void PrimaryInteraction()
    {
        if ((int)StateTracker.IntroState == (int)StateTracker.IntroStates.NoTrainRunsCompleted)
        {
            Unhighlight();
            CutsceneManager.Instance.PlayCutscene(cutscene, OnCutsceneEnd);
        }
    }

    public override void SecondaryInteraction()
    {
        if ((int)StateTracker.IntroState < (int)StateTracker.IntroStates.NoTrainRunsCompleted)
        {
            DialogueManager.Instance.EnterDialogueModeSimple(notNeeded);
        } else if ((int)StateTracker.IntroState == (int)StateTracker.IntroStates.NoTrainRunsCompleted)
        {
            DialogueManager.Instance.EnterDialogueModeSimple(motelNeeded);
        }
        else
        {
            DialogueManager.Instance.EnterDialogueModeSimple(avoidMotel);
        }
    }

    private void OnCutsceneEnd()
    {
        StateTracker.IntroState = StateTracker.IntroStates.MotelCompleted;
    }
}
