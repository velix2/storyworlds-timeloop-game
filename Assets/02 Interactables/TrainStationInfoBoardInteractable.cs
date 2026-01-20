using UnityEngine;

public class TrainStationInfoBoardInteractable : InteractableThreeDimensional
{
    [SerializeField] private string beforeNoTrainRuns2Dialogue;
    [SerializeField] private string afterNoTrainRuns2Dialogue;
    
    
    public override interactionType Primary => interactionType.LOOK;
    public override interactionType Secondary => interactionType.NONE;
    public override bool PrimaryNeedsInRange => true;
    public override bool SecondaryNeedsInRange => true;
    public override void PrimaryInteraction()
    {
        DialogueManager.Instance.EnterDialogueModeSimple(StateTracker.IntroState >= StateTracker.IntroStates.SonCallRepeatCompleted ? afterNoTrainRuns2Dialogue : beforeNoTrainRuns2Dialogue);
        
        // increment to the next intro state
        if (StateTracker.IntroState is StateTracker.IntroStates.SonCallRepeatCompleted) StateTracker.IntroState = StateTracker.IntroStates.NoTrainRuns2Completed;
    }

    public override void SecondaryInteraction()
    {
    }
}
