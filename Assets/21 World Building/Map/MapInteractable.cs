using Unity.Cinemachine;
using UnityEngine;

public class MapInteractable : InteractableThreeDimensional
{
    [SerializeField] private string observationText = "Ganz schön klein.";
    [SerializeField] private CinemachineCamera cam;
    private bool zoomedIn = false;
    public override interactionType Primary => interactionType.INSPECT;

    public override interactionType Secondary
    {
        get
        {
            if (zoomedIn) return interactionType.NONE;
            return interactionType.LOOK;
        }
    }

    public override bool PrimaryNeedsInRange => true;
    public override bool SecondaryNeedsInRange => false;
    public override void PrimaryInteraction()
    {
        if (zoomedIn)
        {
            CameraManager.BackToMain();
            PlayerController.movementBlocked = false;
            zoomedIn = false;
        }
        else
        {
            CameraManager.FocusCam(cam);
            PlayerController.movementBlocked = true;
            zoomedIn = true;
        }
    }

    public override void SecondaryInteraction()
    {
        if (zoomedIn) return;
        DialogueManager.Instance.EnterDialogueModeSimple(observationText);
    }
}
