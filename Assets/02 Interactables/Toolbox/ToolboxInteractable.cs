using UnityEngine;

public class ToolboxInteractable : InteractableThreeDimensional
{
    private static bool CanTakeTool => !StateTracker.IsInIntro && _containsTool;
    private static bool _containsTool = true;

    [SerializeField] private string lookDialogue = "Ein Werkzeugkasten... Spannend, er wirkt eigentlich motorisch eher unbegabt.";
    [SerializeField] private TextAsset toolTakenDialogueJson;
    [SerializeField] private ItemData toolItemData;

    public override interactionType Primary => CanTakeTool ? interactionType.INSPECT : interactionType.NONE;
    public override interactionType Secondary => interactionType.LOOK;
    public override bool PrimaryNeedsInRange => true;
    public override bool SecondaryNeedsInRange => false;
    public override void PrimaryInteraction()
    {
        if (!CanTakeTool) return;
        
        InventoryManager.Instance.AddItem(toolItemData, true);
        DialogueManager.Instance.EnterDialogueMode(toolTakenDialogueJson);
        _containsTool = false;
    }

    public override void SecondaryInteraction()
    {
        DialogueManager.Instance.EnterDialogueModeSimple(lookDialogue);
    }
}
