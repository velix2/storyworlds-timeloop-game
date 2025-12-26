using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Interactable subclass for Interactables that use Image.
/// !! Will overwrite Material.
/// </summary>
public abstract class InteractableUI : OutlineViaShaderInteractable
    {
        
        [Header("Outline")] 
        [SerializeField] private Image imageToOutline;
        
        protected new void Awake()
        {
            base.Awake();
            
            imageToOutline.material = SetupMaterial(OutlineMaterial.material2d);
            inRange = true;
        }
        
        public override bool PrimaryNeedsInRange => false;
        public override bool SecondaryNeedsInRange => false;
    }