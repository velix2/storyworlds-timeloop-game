using UnityEngine;

[RequireComponent(typeof(Outline))]
public abstract class InteractableThreeDimensional : Interactable
{
    protected Outline outline;

    [Header("Outline")] 
    [SerializeField] private Color color = Color.white;
    [SerializeField] private Outline.Mode mode = Outline.Mode.OutlineAll; 
    [SerializeField, Range(0f, 10f)] private float width = 6.0f;
    
    
    protected void Awake()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
        outline.OutlineColor = color;
        outline.OutlineWidth = width;
        outline.OutlineMode = mode;
    }
    
    public override void Highlight()
    {
        outline.enabled = true;
    }

    public override void Unhighlight()
    {
        outline.enabled = false;
    }
}