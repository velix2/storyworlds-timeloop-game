using UnityEngine;
using UnityEngine.Timeline;

public class LarryDinnerChairInteractable : InteractableThreeDimensional
{
    [Header("Cutscene")]
    [SerializeField] private TimelineAsset cutscene;
    
    public override interactionType Primary => interactionType.GRAB;
    public override interactionType Secondary => interactionType.NONE;
    public override bool PrimaryNeedsInRange => true;
    public override bool SecondaryNeedsInRange => false;
    
    public override void PrimaryInteraction()
    {
        if ((int)StateTracker.IntroState == (int)StateTracker.IntroStates.LarrySweetHomeCompleted)
        {
            CutsceneManager.Instance.PlayCutscene(cutscene, OnCutsceneEnd);
            CursorManager.ResetCursor();
            Unhighlight();
        }
        else
        {
            //TODO: Sit down
        }
        
    }
    

    public override void SecondaryInteraction(){}

    private void OnCutsceneEnd()
    {
        StateTracker.IntroState++;
    }
}
