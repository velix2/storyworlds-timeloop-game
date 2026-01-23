using System;
using System.Collections;
using TimeManagement;
using UnityEngine;

public class NPC_Evelyn_Interactable : InteractableTwoDimensional
{
    [SerializeField] private Animator animator;
    private int animatorSitting = Animator.StringToHash("sitting");
    private int animatorTalkedTo = Animator.StringToHash("talkedTo");
    [Header("Dialogue")]
    [SerializeField] private string observationText;
    [SerializeField] private TextAsset dialogueAsset;
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
        DialogueManager.Instance.EnterDialogueMode(dialogueAsset);
        yield return new WaitWhile(() => DialogueManager.Instance.DialogueIsPlaying);
        animator.SetBool(animatorTalkedTo, false);
    }

    public override void SecondaryInteraction()
    {
        DialogueManager.Instance.EnterDialogueModeSimple(observationText);
    }

    private void Start()
    {
        if (StateTracker.IntroState < StateTracker.IntroStates.SonCallCompleted || TimeHandler.Instance.CurrentTime > 60 * 22)
        {
            gameObject.SetActive(false);
        }
        else
        {
            animator.SetBool(animatorSitting, true);
        }
    }
}
