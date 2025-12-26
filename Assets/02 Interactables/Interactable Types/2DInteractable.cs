using UnityEngine;

/// <summary>
/// Interactable Subclass for Interactables that use a SpriteRenderer.
/// !! Will overwrite Material.
/// </summary>
public abstract class InteractableTwoDimensional : OutlineViaShaderInteractable
{
    
    [Header("Outline")] 
    [SerializeField] private SpriteRenderer spriteRenderer;
    
    protected new void Awake()
    {
        base.Awake();
        spriteRenderer.material = SetupMaterial(OutlineMaterial.material2d);

    }
    
}
