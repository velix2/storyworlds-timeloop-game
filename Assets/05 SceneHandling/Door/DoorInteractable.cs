using UnityEngine;

namespace Doors
{
    /// <summary>
    /// A door that always is unlocked, unless overriden.
    /// </summary>
    public class DoorInteractable : InteractableThreeDimensional
    {
        public override interactionType Primary =>
            isUnlocked ? interactionType.ENTER_OPEN : interactionType.ENTER_CLOSED;

        public override interactionType Secondary => interactionType.NONE;
        public override bool PrimaryNeedsInRange => true;
        public override bool SecondaryNeedsInRange => true;

        public bool isLockOverriden = false;
        public bool lockedOnOverride = false;

        [SerializeField] private string textToSayWhenDoorIsLocked = "Zugesperrt...";
        
        private bool isUnlocked => isLockOverriden ? lockedOnOverride : UnlockState;
        protected virtual bool UnlockState => true;

        [SerializeField] private string targetSceneName;
        
        public override void PrimaryInteraction()
        {
            if (!isUnlocked)
            {
                // TODO play dialogue
                Debug.Log("Marcus: " + textToSayWhenDoorIsLocked);
                //DialogueManager.Instance.EnterDialogueModeSimple(textToSayWhenDoorIsLocked);
                return;
            }
            SceneSwitcher.Instance.GoToScene(targetSceneName);
        }

        public override void SecondaryInteraction()
        {
        }
    }
}