using System.Collections;
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

    [Space]
    [SerializeField] private string goToSleepText = "Gute Nacht...";
    
    
    
    public override interactionType Primary
    {
        get
        {
            if (!StateTracker.IsInIntro || StateTracker.IntroState == StateTracker.IntroStates.LarryDinnerCompleted)
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
            if ((int)StateTracker.IntroState == (int)StateTracker.IntroStates.LarryDinnerCompleted)
            {
                CutsceneManager.Instance.PlayCutscene(firstSleepCutscene, OnCutsceneEnd);
                CursorManager.ResetCursor();
                Unhighlight();
            }
        }
        else
        {
            DialogueManager.Instance.EnterDialogueModeSimple(goToSleepText);
            StartCoroutine(SleepAfterDialogueClosed());
        }
    }
    
    private void OnCutsceneEnd()
    {
        StateTracker.IntroState = (StateTracker.IntroStates)((int)StateTracker.IntroState + 1);
        TimeHandler.PassTime(9999);
        
    }

    public override void SecondaryInteraction()
    {
        if (StateTracker.IsInIntro)
        {
            if ((int)StateTracker.IntroState < (int)StateTracker.IntroStates.LarryDinnerCompleted)
            {
                DialogueManager.Instance.EnterDialogueModeSimple(beforeDinner);
            } else DialogueManager.Instance.EnterDialogueModeSimple(afterDinner);
        }
        else
        { 
            DialogueManager.Instance.EnterDialogueModeSimple(duringTimeloop);
        }
    }

    private IEnumerator SleepAfterDialogueClosed()
    {
        yield return new WaitWhile(() => DialogueManager.Instance.SimpleDialogueIsPlaying);
        
        // Reset day
        TimeHandler.PassTime(9999);
    }
}
