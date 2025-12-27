using Unity.Cinemachine;
using UnityEngine;

public class TestFocusInteractable : InteractableThreeDimensional
{
    public override interactionType Primary => interactionType.NONE;
    public override interactionType Secondary => interactionType.LOOK;
    public override bool PrimaryNeedsInRange => false;
    public override bool SecondaryNeedsInRange => false;

    [SerializeField] private CinemachineCamera cam;
    private bool active;

    protected void Awake()
    {
        base.Awake();
        cam.enabled = false;
    }
    public override void PrimaryInteraction()
    {
        return;
    }

    public override void SecondaryInteraction()
    {
        if (active)
        {
            CameraManager.BackToMain();
            active = false;
        }
        else
        {
            CameraManager.FocusCam(cam);
            active = true;
        }
    }
}
