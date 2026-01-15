using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Doors
{
    [RequireComponent(typeof(DoorInteractable))]
    public class DoorOverrideOnCutsceneExtension : MonoBehaviour
    {
        [Serializable]
        private class IntroStateOverrides
        {
            public StateTracker.IntroStates introState; 
            public bool isDoorUnlocked;
        }

        [SerializeField] private List<IntroStateOverrides> whenToOverride;
        
        private DoorInteractable _door;

        private void Awake()
        {
            _door = GetComponent<DoorInteractable>();
        }

        private void Start()
        {
            OnIntroStateChanged(StateTracker.IntroState);
        }

        private void OnEnable()
        {
            StateTracker.OnIntroStateChanged.AddListener(OnIntroStateChanged);
        }
        
        private void OnDisable()
        {
            StateTracker.OnIntroStateChanged.RemoveListener(OnIntroStateChanged);
        }

        private void OnIntroStateChanged(StateTracker.IntroStates state)
        {
            var stateOverride = whenToOverride.FirstOrDefault(o => o.introState == state);

            if (stateOverride != null)
            {
                _door.isLockOverriden = true;

                _door.lockedOnOverride = !stateOverride.isDoorUnlocked;
            }
            else
            {
                _door.isLockOverriden = false;
            }
        }
        
    }
}