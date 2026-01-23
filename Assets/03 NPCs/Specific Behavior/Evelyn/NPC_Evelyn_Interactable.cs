using System;
using System.Collections;
using TimeManagement;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class NPC_Evelyn_Interactable : InteractableTwoDimensional
{
    [SerializeField] private Animator animator;
    private int animatorSitting = Animator.StringToHash("sitting");
    private int animatorTalkedTo = Animator.StringToHash("talkedTo");
    [Header("Dialogue")] 
    [SerializeField] private ItemData coffeeReference;
    [SerializeField] private string observationText;
    [SerializeField] private TextAsset noCoffeeDialogue;
    [SerializeField] private TextAsset regularDialogue;
    [SerializeField] private TextAsset readyForRideDialogue;
    [SerializeField] private TextAsset itemReject;
    [SerializeField] private TextAsset coffeeGet;
    [Header("Cutscenes")] 
    [SerializeField] private TimelineAsset driveAwayCutscene;
    [SerializeField] private TimelineAsset outOfCoffeeCutscene;
    

    private bool outside;
    
    public override interactionType Primary => interactionType.SPEAK;
    public override interactionType Secondary => interactionType.LOOK;
    public override bool PrimaryNeedsInRange => true;
    public override bool SecondaryNeedsInRange => false;
    public override void PrimaryInteraction()
    {
        StartCoroutine(TalkSequence());
    }

    private IEnumerator TalkSequence()
    {
        animator.SetBool(animatorTalkedTo, true);
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
        DialogueManager.Instance.EnterDialogueMode(GetCurrentDialogue());
        yield return new WaitWhile(() => DialogueManager.Instance.DialogueIsPlaying);
        animator.SetBool(animatorTalkedTo, false);
    }
    
    private TextAsset GetCurrentDialogue()
    {
        switch (StateTracker.EvelynQuestState)
        {
            case StateTracker.EvelynQuestStates.COFFEE_GIVEN:
                return outside ? readyForRideDialogue : regularDialogue;
            default:
                return noCoffeeDialogue;
        }
    }

    public override void SecondaryInteraction()
    {
        DialogueManager.Instance.EnterDialogueModeSimple(observationText);
    }

    public override bool ItemInteraction(ItemData otherItem)
    {
        if (otherItem == coffeeReference)
        {
            DialogueManager.Instance.EnterDialogueMode(coffeeGet);
            StateTracker.EvelynQuestState = StateTracker.EvelynQuestStates.COFFEE_GIVEN;
            return true;
        }
        else
        {
            DialogueManager.Instance.EnterDialogueMode(itemReject);
            return false;
        }
    }


    private void Start()
    {
        if (StateTracker.IntroState >= StateTracker.IntroStates.SonCallCompleted) //story progression
        {
            if (TimeHandler.Instance.CurrentTime > 60 * 22)     //drives off
            {
                gameObject.SetActive(false);
                return;
            }

            switch (SceneManager.GetActiveScene().name)
            {
                case "Outdoor":
                    outside = true;
                    animator.SetBool(animatorSitting, false);
                    break;
                case "Diner":
                    animator.SetBool(animatorSitting, true);
                    if (!StateTracker.IsInIntro && StateTracker.EvelynQuestState == StateTracker.EvelynQuestStates.INIT)
                    {
                        CutsceneManager.Instance.PlayCutscene(outOfCoffeeCutscene);
                        StateTracker.EvelynQuestState = StateTracker.EvelynQuestStates.CUTSCENE_WATCHED;
                    }
                    
                    break;
                default:
                    Debug.LogError("Behaviour is undefined in this scene!");
                    return;
            }
            
        }
        else
        {
            gameObject.SetActive(false);  
        }
        
    }
}
