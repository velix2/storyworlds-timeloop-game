using System;
using UnityEngine;
using UnityEngine.Timeline;

public class LarryApartmentWait : InteractableTwoDimensional
{
    public override interactionType Primary => interactionType.SPEAK;
    public override interactionType Secondary => interactionType.LOOK;
    public override bool PrimaryNeedsInRange => true;
    public override bool SecondaryNeedsInRange => false;

    [SerializeField] private TimelineAsset larryWaitingCutscene;
    [SerializeField] private Transform waitingPoint;
    [SerializeField] private string observationText = "Er scheint ja wirklich zu warten.";
    [SerializeField] private string nextScene = "LarryHouse";
    


    protected void Start()
    {
        StateTracker.OnIntroStateChanged.AddListener(CheckIntroState);
        CheckIntroState(StateTracker.IntroState);
    }

    private void CheckIntroState(StateTracker.IntroStates state)
    {
        if ((int)state == (int)StateTracker.IntroStates.MotelCompleted)
        {
            transform.position = waitingPoint.position;
        }
        else
        {
            transform.position = Vector3.zero;
        }
    }

    private void OnCutsceneEnd()
    {
        CutsceneManager.CutsceneEnded -= OnCutsceneEnd;
        StateTracker.IntroState = StateTracker.IntroStates.LarryWaitingCompleted;
        SceneSwitcher.Instance.GoToScene(nextScene);
    }
    
    
    public override void PrimaryInteraction()
    {
        CutsceneManager.CutsceneEnded += OnCutsceneEnd;
        CutsceneManager.Instance.PlayCutscene(larryWaitingCutscene);
        Unhighlight();
        CursorManager.ResetCursor();
    }

    public override void SecondaryInteraction()
    {
        DialogueManager.Instance.EnterDialogueModeSimple(observationText);
    }
}
