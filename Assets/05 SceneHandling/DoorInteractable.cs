using UnityEngine;

namespace SceneHandling
{
    public class DoorInteractable :InteractableThreeDimensional
    {
        public override interactionType Primary =>
            isUnlocked ? interactionType.ENTER_OPEN : interactionType.ENTER_CLOSED;

        public override interactionType Secondary => interactionType.NONE;
        public override bool PrimaryNeedsInRange => true;
        public override bool SecondaryNeedsInRange => true;

        [SerializeField] private bool isUnlocked = true;

        [SerializeField] private string targetSceneName;
        
        public override void PrimaryInteraction()
        {
            SceneSwitcher.Instance.GoToScene(targetSceneName);
        }

        public override void SecondaryInteraction()
        {
        }
    }
}