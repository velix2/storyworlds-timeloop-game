using UnityEngine;

/// <summary>
/// Interactable Subclass for Interactables that use a SpriteRenderer.
/// !! Will overwrite Material.
/// </summary>
public abstract class InteractableTwoDimensional : Interactable
{
    private Material outlineMaterial;
    
    [Header("Outline")] 
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color color = Color.white;
    [SerializeField, Range(0f, 2f)] private float width = 1.0f;
    
    protected new void Awake()
    {
        base.Awake();
        spriteRenderer.material = SetupMaterial(OutlineMaterial.material2d);

    }
    
    /// <summary>
    /// Creates a copy of the material, initiates it and sets outlineMaterial to the copy.
    /// <br/> !!You will have to manually reassign the copied material to the element.
    /// </summary>
    /// <param name="material">Outline Material.</param>
    private Material SetupMaterial(Material material)
    {
        outlineMaterial = Instantiate(material);
        outlineMaterial.SetColor("_Color", color);
        outlineMaterial.SetFloat("_enabled", 0);
        outlineMaterial.SetFloat("_width", width);
        return outlineMaterial;
    }
    
    public override void Highlight()
    {
        if (!HighlightOverwrite) outlineMaterial.SetFloat("_enabled", 1);
    }

    public override void Unhighlight()
    {
        if (!HighlightOverwrite) outlineMaterial.SetFloat("_enabled", 0);
    }
}
