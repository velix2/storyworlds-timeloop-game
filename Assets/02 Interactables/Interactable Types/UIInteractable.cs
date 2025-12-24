using UnityEngine;
using UnityEngine.UI;

public abstract class UIInteractable : Interactable
{

    protected Material outlineMaterial;

        [Header("Outline")] 
        [SerializeField] private Color color = Color.white;
        [SerializeField, Range(0, 10f)] private float width = 6f;
        
        protected void Awake()
        {
            SetupMaterial(GetComponent<Image>().material);
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
            outlineMaterial.SetFloat("_width", width);
            outlineMaterial.SetFloat("_enabled", 0);
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