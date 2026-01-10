using UnityEngine;

/// <summary>
/// Interactable subclass for Interactables that use a MeshRenderer.
/// </summary>
[RequireComponent(typeof(Outline))]
public abstract class InteractableThreeDimensional : Interactable
{
    protected Outline outline;

    [Header("Outline")] 
    public override Color OutlineColor => outline.OutlineColor;
    [SerializeField] private Outline.Mode mode = Outline.Mode.OutlineAll;
    public override float OutlineWidth => outline.OutlineWidth;


    protected new void Awake()
    {
        base.Awake();
        outline = GetComponent<Outline>();
        outline.enabled = false;
        outline.OutlineMode = mode;
        outline.OutlineWidth = 6.0f;
    }
    

    public override void Highlight()
    {
        if (!HighlightOverwrite) outline.enabled = true;
    }

    public override void Unhighlight()
    {
        if (!HighlightOverwrite) outline.enabled = false;
    }
}