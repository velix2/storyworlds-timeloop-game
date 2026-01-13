using System.Collections.Generic;
using System.Linq;
using TimeManagement;
using UnityEngine;

namespace Doors
{
    /// <summary>
    /// A door that is only unlocked during the specified phases. If none are specified, door is always unlocked.
    /// </summary>
    public class TimedLockableDoorInteractable : DoorInteractable
    {
        [SerializeField] private List<TimePhase> timePhasesUnlocked;
        
        protected override bool isUnlocked
        {
            get
            {
                if (isAlwaysLocked) return false;
                
                if (timePhasesUnlocked.Count == 0) return true;
                var currentTime = TimeHandler.Instance.CurrentTime;
                return timePhasesUnlocked.Any(phase => phase.IsBetween(currentTime));
            }
        }
    }
}