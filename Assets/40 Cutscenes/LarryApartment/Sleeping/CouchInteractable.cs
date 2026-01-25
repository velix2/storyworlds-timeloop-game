using System;
using System.Collections;
using System.Collections.Generic;
using TimeManagement;
using UnityEngine;
using UnityEngine.Timeline;

public class CouchInteractable : InteractableThreeDimensional
{
    [Header("Cutscene")] 
    [SerializeField] private TimelineAsset sleepCutscene;
    [SerializeField] private TimelineAsset sleepCutsceneWithGlow;

    [SerializeField] private AudioClip telephoneRing;
    [SerializeField] private List<TextAsset> sonCalls = new List<TextAsset>();

    [Header("Observation Texts")] 
    [SerializeField] private string beforeDinner = "Hoffentlich wird das nicht mein Schlafplatz sein.";
    [SerializeField] private string afterDinner = "Mein Schlafplatz. Leider.";
    [SerializeField] private string duringTimeloop = "Mein Schlafplatz.";

    [Space]
    [SerializeField] private string goToSleepText = "Gute Nacht...";

    public static bool newDayBegins = false;
    
    public override interactionType Primary
    {
        get
        {
            if (!StateTracker.IsInIntro || StateTracker.IntroState == StateTracker.IntroStates.LarryDinnerCompleted || 
                StateTracker.IntroState == StateTracker.IntroStates.DinerWithRadioCompleted)
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
        if (!StateTracker.IsInIntro || (int)StateTracker.IntroState == (int)StateTracker.IntroStates.LarryDinnerCompleted 
            || StateTracker.IntroState == StateTracker.IntroStates.DinerWithRadioCompleted)
        {
            StartCoroutine(SleepSequence());
            CursorManager.ResetCursor();
            Unhighlight();
        }
    }
    private IEnumerator SleepSequence()
    {
        newDayBegins = true;
        DialogueManager.Instance.EnterDialogueModeSimple(goToSleepText);
        yield return new WaitWhile(() => DialogueManager.Instance.SimpleDialogueIsPlaying);
        if(StateTracker.IntroState == StateTracker.IntroStates.DinerWithRadioCompleted)
        {
            CutsceneManager.Instance.PlayCutscene(sleepCutsceneWithGlow, OnCutsceneEnd);
        }
        else
        {
            CutsceneManager.Instance.PlayCutscene(sleepCutscene, OnCutsceneEnd);
        }
        
    }
    
    private void OnCutsceneEnd()
    {
        if (StateTracker.IsInIntro) StateTracker.IntroState++;
        
        TimeHandler.ResetDay();
    }
    
    private void Start()
    {
        if (newDayBegins)
        {
            StartCoroutine(SonCall());
            newDayBegins = false;
        }
        
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

    

    private IEnumerator SonCall()
    {
        if (StateTracker.newSonStateAvailable)
        {
            TextAsset call = GetSonCall();
            if (call != null)
            {
                yield return new WaitForSeconds(1.0f);
                AudioManager.PlaySFX(telephoneRing);
                yield return new WaitForSeconds(telephoneRing.length);
                DialogueManager.Instance.EnterDialogueMode(call);

                StateTracker.SonCallState = StateTracker.SonCallStates.secondCall;
            }
        }
        
    }

    private TextAsset GetSonCall()
    {
        int index = (int) StateTracker.SonCallState;
        return (index < sonCalls.Count) ? sonCalls[index] : null;
    }
}
