using TimeManagement;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ReactiveInteriorWindow : DaytimePhaseChangeSubscriber<Material>
{
    [SerializeField] private string materialNameToReplace = "Window";
    private int _materialIndexToReplace = -1;

    private Renderer _renderer;
    
    protected override void ApplyElement(Material elementToApply, DaytimePhase _)
    {
        OverrideMaterial(elementToApply);
    }

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        materialNameToReplace += " (Instance)"; // Add this Suffix for what reason whatsoever

        // Find the index of the material that matches the specified name
        for (var i = 0; i < _renderer.materials.Length; i++)
        {
            if (_renderer.materials[i].name != materialNameToReplace) continue;
            _materialIndexToReplace = i;
            break;
        }

        if (_materialIndexToReplace != -1) return;
        Debug.LogError($"Material {materialNameToReplace} not found");
    }


    private void OverrideMaterial(Material newMat)
    {
        // Get a copy of the current materials array
        Material[] tempMaterials = _renderer.materials;

        // Replace the specific element in your copy
        tempMaterials[_materialIndexToReplace] = newMat;

        // Reassign the entire array back to the renderer
        _renderer.materials = tempMaterials;
    }
}