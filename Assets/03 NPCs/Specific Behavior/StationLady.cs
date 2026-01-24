using TimeManagement;
using UnityEngine;

public class StationLady : InteractableTwoDimensional
{
    [SerializeField] private TimePhase timeAvailable;

    [SerializeField] private string defaultDialogueHello = "Willkommen im Bahnhof von Echo's Lake!";
    [SerializeField] private TextAsset dialogueNoTrain2;
    [SerializeField] private string defaultDialogueNoTrain = "Es tut mir Leid, es fahren leider gerade keine Züge.";

    public override interactionType Primary => interactionType.SPEAK;
    public override interactionType Secondary => interactionType.NONE;
    public override bool PrimaryNeedsInRange => true;
    public override bool SecondaryNeedsInRange => true;

    public override void PrimaryInteraction()
    {
        switch (StateTracker.IntroState)
        {
            case StateTracker.IntroStates.Init:
            case StateTracker.IntroStates.LeftTrainCompleted:
            case StateTracker.IntroStates.RarityShopCompleted:
                DialogueManager.Instance.EnterDialogueModeSimple(defaultDialogueHello);
                break;
            case StateTracker.IntroStates.SonCallRepeatCompleted:
                DialogueManager.Instance.EnterDialogueMode(dialogueNoTrain2);
                StateTracker.IntroState = StateTracker.IntroStates.NoTrainRuns2Completed;
                break;
            default:
                DialogueManager.Instance.EnterDialogueModeSimple(defaultDialogueNoTrain);
                break;
        }
    }

    public override void SecondaryInteraction()
    {
    }
}