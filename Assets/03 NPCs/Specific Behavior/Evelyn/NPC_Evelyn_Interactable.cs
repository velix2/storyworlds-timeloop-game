using System;
using System.Collections;
using TimeManagement;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class NPC_Evelyn_Interactable : InteractableTwoDimensional
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private TruckInteractable truck;
    private int animatorSitting = Animator.StringToHash("sitting");
    private int animatorTalkedTo = Animator.StringToHash("talkedTo");
    [Header("Dialogue")] 
    [SerializeField] private ItemData coffeeReference;
    [SerializeField] private string observationText;
    [SerializeField] private TextAsset regularDialogue;
    [SerializeField] private TextAsset itemReject;
    [SerializeField] private TextAsset coffeeGet;
    [Header("Cutscenes")] 
    [SerializeField] private TimelineAsset driveAwayCutscene;
    [SerializeField] private TimelineAsset outOfCoffeeCutscene;


    private bool outside
    {
        get => (bool)DialogueManager.Instance.variableObserver.GetVariable("evelynOutside");
        set => DialogueManager.Instance.variableObserver.SetVariable("evelynOutside", value);
    }
    
    public override interactionType Primary => interactionType.SPEAK;
    public override interactionType Secondary => interactionType.LOOK;
    public override bool PrimaryNeedsInRange => true;
    public override bool SecondaryNeedsInRange => false;
    public override void PrimaryInteraction()
    {
        StartCoroutine(TalkSequence(regularDialogue));
    }

    private IEnumerator TalkSequence(TextAsset dialogue)
    {
        PlayerController.movementBlocked = true;
        animator.SetBool(animatorTalkedTo, true);
        if (!outside)yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length + 0.5f);
        DialogueManager.Instance.EnterDialogueMode(dialogue);
        yield return new WaitWhile(() => DialogueManager.Instance.DialogueIsPlaying);
        
        if (StateTracker.Evelyn.rideConfirmation)
        {
            CutsceneManager.Instance.PlayCutscene(driveAwayCutscene);
        } 
        
        animator.SetBool(animatorTalkedTo, false);
        PlayerController.movementBlocked = false;
    }
    
    public override void SecondaryInteraction()
    {
        DialogueManager.Instance.EnterDialogueModeSimple(observationText);
    }

    public override bool ItemInteraction(ItemData otherItem)
    {
        if (otherItem == coffeeReference)
        {
            StartCoroutine(TalkSequence(coffeeGet));
            StateTracker.Evelyn.coffeeGiven = true;
            return true;
        }
        else
        {
            StartCoroutine(TalkSequence(itemReject));
            return false;
        }
    }


    private void Start()
    {
        if (StateTracker.IntroState >= StateTracker.IntroStates.SonCallCompleted) //story progression
        {
            if (TimeHandler.Instance.CurrentTime > 60 * 22) goto not_here;    //drives off
            
            switch (SceneManager.GetActiveScene().name)
            {
                case "Outdoor":
                    if (TimeHandler.Instance.CurrentTime <= 60 * 20) goto not_here;
                    outside = true;
                    animator.SetBool(animatorSitting, false);
                    break;
                case "Diner":
                    if (TimeHandler.Instance.CurrentTime > 60 * 20) goto not_here;
                    animator.SetBool(animatorSitting, true);
                    if (!StateTracker.IsInIntro && StateTracker.Evelyn.QuestState == StateTracker.EvelynState.QuestStates.Init)
                    {
                        CutsceneManager.Instance.PlayCutscene(outOfCoffeeCutscene);
                        StateTracker.Evelyn.QuestState = StateTracker.EvelynState.QuestStates.IntroCutsceneWatched;
                    }

                    outside = false;
                    break;
                default:
                    Debug.LogError("Behaviour is undefined in this scene!");
                    return;
            }

            return;
        }
        not_here:
            gameObject.SetActive(false); 
        
    }
}
