using UnityEngine;

public class DialogueWithNPCInteractable : InteractableTwoDimensional
{
    public override interactionType Primary => interactionType.SPEAK;
    public override interactionType Secondary => interactionType.LOOK;
    public override bool PrimaryNeedsInRange => true;
    public override bool SecondaryNeedsInRange => false;

    [SerializeField] private TextAsset[] inkJson;

    public override void PrimaryInteraction()
    {
        if(DialogueManager.GetInstance() != null)
        {
            DialogueManager.GetInstance().EnterDialogueMode(inkJson[0]);
        }
        else
        {
            Debug.LogError("There is no Dialogue Manager in the scene!");
        }
        
    }

    public override void SecondaryInteraction()
    {
        if (DialogueManager.GetInstance() != null)
        {
            DialogueManager.GetInstance().EnterDialogueModePlayer(inkJson[1]);
        }
        else
        {
            Debug.LogError("There is no Dialogue Manager in the scene!");
        }
    }
}
