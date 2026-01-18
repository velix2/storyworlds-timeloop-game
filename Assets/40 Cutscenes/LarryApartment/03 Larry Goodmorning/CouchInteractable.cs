using TimeManagement;
using UnityEngine;
using UnityEngine.Timeline;

public class CouchInteractable : InteractableThreeDimensional
{
    [Header("Cutscene")] [SerializeField] private TimelineAsset firstSleepCutscene;

    [Header("Observation Texts")] 
    [SerializeField] private string beforeDinner = "Hoffentlich wird das nicht mein Schlafplatz sein.";
    [SerializeField] private string afterDinner = "Mein Schlafplatz. Leider.";
    [SerializeField] private string duringTimeloop = "Mein Schlafplatz.";
    
    
    public override interactionType Primary
    {
        get
        {
            if (!StateTracker.IsInIntro || StateTracker.IntroState == StateTracker.IntroStates.LarrySweetHomeCompleted)
            {
                return interactionType.GRAB;
            }
            return interactionType.NONE;
        }
    }
    public override interactionType Secondary => interactionType.LOOK;
    public override bool PrimaryNeedsInRange => true;
    public override bool SecondaryNeedsInRange => false;
    public override void PrimaryInteraction()
    {
        if (StateTracker.IsInIntro)
        {
            if (StateTracker.IntroState == StateTracker.IntroStates.LarryDinnerCompleted)
            {
                CutsceneManager.Instance.PlayCutscene(firstSleepCutscene);
                CursorManager.ResetCursor();
                Unhighlight();
            }
        }
        else
        {
            //TODO: Sleepy-time
        }
    }

    public override void SecondaryInteraction()
    {
        if (StateTracker.IsInIntro)
        {
            switch (StateTracker.IntroState)
            {
                case StateTracker.IntroStates.LarrySweetHomeCompleted:
                    DialogueManager.Instance.EnterDialogueModeSimple(beforeDinner);
                    break;
                default:
                    DialogueManager.Instance.EnterDialogueModeSimple(afterDinner);
                    break;
            }
        }
        else
        { 
            DialogueManager.Instance.EnterDialogueModeSimple(duringTimeloop);
        }
    }
}
