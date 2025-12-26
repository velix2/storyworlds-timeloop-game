using UnityEngine;

/// <summary>
/// Helper class to have a handle on OutlineMaterials.
/// </summary>
public static class OutlineMaterial
{
    public static readonly Material material2d;
    
    

    static OutlineMaterial()
    {
        material2d = Resources.Load<Material>("2D Highlighting Shaders/Outline_2D");
    }
}

public abstract class OutlineViaShaderInteractable : Interactable
{
    private Material outlineMaterial;
    
    
    public override Color OutlineColor { 
        get => OutlineColor;
        set
        {
            OutlineColor = value;
            outlineMaterial.SetColor("_Color", value);
        } 
    }


    public override float OutlineWidth
    {
        get => OutlineWidth;
        set
        {
            OutlineWidth = value;
            outlineMaterial.SetFloat("_width", value);
        }
    } //if you want to init value you have to do it manually in Awake or Start

    /// <summary>
    /// Creates a copy of the material, initiates it and sets outlineMaterial to the copy.
    /// <br/> !!You will have to manually reassign the copied material to the element.
    /// </summary>
    /// <param name="material">Outline Material.</param>
    protected Material SetupMaterial(Material material)
    {
        outlineMaterial = Instantiate(material);
        outlineMaterial.SetFloat("_enabled", 0);
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