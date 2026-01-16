using System;
using Object_Tracking;
using UnityEngine;

public class CoffeeMachineInteractable : InteractableThreeDimensional, IFreezable
{
    private enum RepairState
    {
        Broken,
        Repaired
    }

    private RepairState _repairState = RepairState.Broken;

    [Space] [Header("Dialogues")] [SerializeField]
    private string dialogueFreezeWhileBroken =
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
            _repairState = (RepairState)stateIndex;
        }
    }

    public override void PrimaryInteraction()
    {
        Say(_repairState is RepairState.Broken ? dialogueInspectWhileBroken : dialogueInspectWhileRepaired);
    }

    public override void SecondaryInteraction()
    {
        Say(_repairState is RepairState.Broken ? dialogueLookWhileBroken : dialogueLookWhileRepaired);
    }

    public override bool ItemInteraction(ItemData otherItem)
    {
        if (otherItem.Equals(repairToolsItemData))
        {
            if (_repairState is RepairState.Broken)
            {
                // Player repairs
                Say(dialogueRepairWhileBroken);

                _repairState = RepairState.Repaired;

                return false;
            }
            else
            {
                Say(dialogueRepairWhileRepaired);

                return false;
            }
        }
        else if (otherItem.Equals(freezeArtifact))
        {
            // Player wants to freeze

            // Check if machine is still broken
            if (_repairState == RepairState.Broken)
            {
                Say(dialogueFreezeWhileBroken);
                return false;
            }
            else
            {
                // check if frozen -> unfreeze, else freeze
                if (thisFreezable.CheckForStoredState(out _))
                {
                    Say(dialogueUnfreeze);
                    thisFreezable.UnfreezeState();

                    return false;
                }
                else
                {
                    Say(dialogueFreezeWhileRepaired);
                    thisFreezable.StoreState((int)_repairState);

                    return false;
                }
            }
        }
        else
        {
            // Interacted with any other item -> default response
            return base.ItemInteraction(otherItem);
        }
    }


    public string GetFreezableIdentifier()
    {
        // try to make this as unique as possible, use name + scene
        return nameof(CoffeeMachineInteractable) + "_" + gameObject.name + "_" + gameObject.scene.name;
    }

    private void Say(string s)
    {
        // TODO replace with call to simple dialogue
        Debug.Log("Marcus: " + s);
    }
}