using UnityEngine;

public class TrainStationInfoBoardInteractable : InteractableThreeDimensional
{
    [SerializeField] private string beforeNoTrainRuns2Dialogue;
    [SerializeField] private string onNoTrainRuns2Dialogue;
    [SerializeField] private string afterNoTrainRuns2Dialogue;
    
    
    public override interactionType Primary => interactionType.LOOK;
    public override interactionType Secondary => interactionType.NONE;
    public override bool PrimaryNeedsInRange => true;
    public override bool SecondaryNeedsInRange => true;
    public override void PrimaryInteraction()
    {
        switch (StateTracker.IntroState)
        {
            // Also allow state afterwards for repeating interact
            case StateTracker.IntroStates.SonCallRepeatCompleted or StateTracker.IntroStates.NoTrainRuns2Completed:
                DialogueManager.Instance.EnterDialogueModeSimple(onNoTrainRuns2Dialogue);
                break;
            case < StateTracker.IntroStates.SonCallRepeatCompleted:
                DialogueManager.Instance.EnterDialogueModeSimple(beforeNoTrainRuns2Dialogue);
                break;
            default:
                DialogueManager.Instance.EnterDialogueModeSimple(afterNoTrainRuns2Dialogue);
                break;
        }
    }

    public override void SecondaryInteraction()
    {
    }
}
