using UnityEngine;
using UnityEngine.UI;

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
            SetupMaterial(imageToOutline.material);
            imageToOutline.material = outlineMaterial;

            inRange = true;
        }

        /// <summary>
        /// Creates a copy of the material, initiates it and sets outlineMaterial to the copy.
        /// <br> !!You will have to manually reassign the copied material to the element.
        /// </summary>
        /// <param name="material">The material of the element you want to have outlined.</param>
        protected void SetupMaterial(Material material)
        {
            outlineMaterial = Instantiate(material);
            outlineMaterial.SetColor("_Color", color);
            outlineMaterial.SetFloat("_enabled", 0);
            outlineMaterial.SetFloat("_width", width);
        }
        
        public override void Highlight()
        {
            outlineMaterial.SetFloat("_enabled", 1);
        }

        public override void Unhighlight()
        {
            
            outlineMaterial.SetFloat("_enabled", 0);
        }
    }