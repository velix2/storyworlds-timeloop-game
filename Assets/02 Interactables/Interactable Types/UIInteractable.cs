using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Interactable subclass for Interactables that use Image.
/// !! Will overwrite Material.
/// </summary>
public abstract class InteractableUI : Interactable
    {

        protected Material outlineMaterial;

        [Header("Outline")] 
        [SerializeField] private Image imageToOutline;
        [SerializeField] private Color color = Color.white;
        [SerializeField, Range(0f, 2f)] private float width = 1.0f;
        
        protected new void Awake()
        {
            base.Awake();
            
            imageToOutline.material = SetupMaterial(OutlineMaterial.material2d);
            inRange = true;
        }

        /// <summary>
        /// Creates a copy of the material, initiates it and sets outlineMaterial to the copy.
        /// <br/> !!You will have to manually reassign the copied material to the element.
        /// </summary>
        /// <param name="material">Outline material</param>
        private Material SetupMaterial(Material material)
        {
            outlineMaterial = Instantiate(material);
            outlineMaterial.SetColor("_Color", color);
            outlineMaterial.SetFloat("_enabled", 0);
            outlineMaterial.SetFloat("_width", width);
            return outlineMaterial;
        }

        public override bool PrimaryNeedsInRange => false;
        public override bool SecondaryNeedsInRange => false;

        public override void Highlight()
        {
            if (!HighlightOverwrite) outlineMaterial.SetFloat("_enabled", 1);
        }

        public override void Unhighlight()
        {
            
            if (!HighlightOverwrite) outlineMaterial.SetFloat("_enabled", 0);
        }
    }