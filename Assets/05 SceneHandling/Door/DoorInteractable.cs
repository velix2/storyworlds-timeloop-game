using UnityEngine;

namespace Doors
{
    /// <summary>
    /// A door that always is either locked or unlocked.
    /// </summary>
    public class DoorInteractable : InteractableThreeDimensional
    {
        public override interactionType Primary =>
            isUnlocked ? interactionType.ENTER_OPEN : interactionType.ENTER_CLOSED;

        public override interactionType Secondary => interactionType.NONE;
        public override bool PrimaryNeedsInRange => true;
        public override bool SecondaryNeedsInRange => true;

        [SerializeField] protected bool isAlwaysLocked;

        [SerializeField] private string textToSayWhenDoorIsLocked = "Zugesperrt...";
        
        protected virtual bool isUnlocked => !isAlwaysLocked;

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