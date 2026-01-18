using System;
using Object_Tracking;
using UnityEngine;

public class CoffeeMachineInteractable : InteractableThreeDimensional, IFreezable
{
    private enum CoffeeMachineRepairState
    {
        Broken,
        Repaired
    }

    private bool IsQuestAvailable => !StateTracker.IsInIntro;

    private CoffeeMachineRepairState _repairState = CoffeeMachineRepairState.Broken;
    
    private CoffeeMachineRepairState RepairState
    {
        get => _repairState;
        set
        {
            _repairState = value;
            smokeParticles.SetActive(_repairState == CoffeeMachineRepairState.Broken && IsQuestAvailable); // Update smoke particles
        }
    }

    [Space] [Header("Dialogues")] [SerializeField]
    private string dialogueAnyInteractWhileIntro =
        "Ohh, Kaffee... Wenn ich mehr Zeit hätte, würde ich mir einen genehmigen...";

    [SerializeField] private string dialogueFreezeWhileBroken =
        "Zwecklos, die Maschine jetzt schon einzufrieren. Ich muss sie erst reparieren.";

    [SerializeField] private string dialogueFreezeWhileRepaired =
        "Spannend. Wenn das wirklich so funktioniert, wie ich mir das vorstelle, sollte die Kaffeemaschine morgen immer noch funktionieren...";

    [SerializeField] private string dialogueUnfreeze =
        "Jetzt müsste der Effekt wieder aufgehoben sein. Morgen also kein Kaffee mehr.";

    [SerializeField] private string dialogueLookWhileBroken = "...Soll da Rauch raus kommen?";
    [SerializeField] private string dialogueLookWhileRepaired = "Endlich wieder Kaffee!";

    [SerializeField] private string dialogueInspectWhileBroken =
        "Ich glaube, das kann ich reparieren. Brauche nur passendes Werkzeug...";

    [SerializeField] private string dialogueInspectWhileRepaired = "Sieht gut aus. Hält hoffentlich fürs Erste.";

    [SerializeField] private string dialogueRepairWhileBroken =
        "So, einfach bisschen das hier nach da, und dann das festdrehen, das hier austauschen... Wunderbar!";

    [SerializeField] private string dialogueRepairWhileRepaired =
        "Die Kaffeemaschine ist wieder funktionstüchtig, ich schraube lieber nicht weiter dran herum...";
    
    [Space] [SerializeField] private GameObject smokeParticles;
    


    /// <summary>
    /// The item the player can repair this coffee machine with.
    /// </summary>
    [SerializeField] private ItemData repairToolsItemData;

    /// <summary>
    /// The artifact the player can (un)freeze the state with.
    /// </summary>
    [SerializeField] private ItemData freezeArtifact;

    private IFreezable thisFreezable;

    public override interactionType Primary => interactionType.INSPECT;
    public override interactionType Secondary => interactionType.LOOK;
    public override bool PrimaryNeedsInRange => true;
    public override bool SecondaryNeedsInRange => false;
    

    private void Start()
    {
        thisFreezable = this;

        // Check if this thing is frozen
        if (thisFreezable.CheckForStoredState(out var stateIndex))
        {
            RepairState = (CoffeeMachineRepairState)stateIndex;
        }
    }

    public override void PrimaryInteraction()
    {
        if (!IsQuestAvailable) Say(dialogueAnyInteractWhileIntro);
        else Say(RepairState is CoffeeMachineRepairState.Broken ? dialogueInspectWhileBroken : dialogueInspectWhileRepaired);
    }

    public override void SecondaryInteraction()
    {
        if (!IsQuestAvailable) Say(dialogueAnyInteractWhileIntro);
        else Say(RepairState is CoffeeMachineRepairState.Broken ? dialogueLookWhileBroken : dialogueLookWhileRepaired);
    }

    public override bool ItemInteraction(ItemData otherItem)
    {
        if (!IsQuestAvailable) return base.ItemInteraction(otherItem); // Default response during intro
        
        if (otherItem.Equals(repairToolsItemData))
        {
            if (RepairState is CoffeeMachineRepairState.Broken)
            {
                // Player repairs
                Say(dialogueRepairWhileBroken);

                RepairState = CoffeeMachineRepairState.Repaired;

                return false;
            }

            Say(dialogueRepairWhileRepaired);

            return false;
        }

        if (otherItem.Equals(freezeArtifact))
        {
            // Player wants to freeze

            // Check if machine is still broken
            if (RepairState == CoffeeMachineRepairState.Broken)
            {
                Say(dialogueFreezeWhileBroken);
                return false;
            }

            // check if frozen -> unfreeze, else freeze
            if (thisFreezable.CheckForStoredState(out _))
            {
                Say(dialogueUnfreeze);
                thisFreezable.UnfreezeState();

                return false;
            }

            Say(dialogueFreezeWhileRepaired);
            thisFreezable.StoreState((int)RepairState);

            return false;
        }

        // Interacted with any other item -> default response
        return base.ItemInteraction(otherItem);
    }


    public string GetFreezableIdentifier()
    {
        // try to make this as unique as possible, use name + scene
        return nameof(CoffeeMachineInteractable) + "_" + gameObject.name + "_" + gameObject.scene.name;
    }

    private void Say(string s)
    {
        DialogueManager.Instance.EnterDialogueModeSimple(s);
    }
}