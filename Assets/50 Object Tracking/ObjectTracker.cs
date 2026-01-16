using System.Collections.Generic;
using UnityEngine;

namespace Object_Tracking
{
    public class ObjectTracker : MonoBehaviour
    {
        #region Singleton

        // Singleton stuff

        /// <summary>
        /// The singleton instance.
        /// </summary>
        public static ObjectTracker Instance;


        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        #endregion

        /// <summary>
        /// Stores a string that represents a freezable object's string identifier and its corresponding state index.
        /// </summary>
        private readonly Dictionary<string, int> _frozenObjectStates = new();

        /// <summary>
        /// Checks if for a certain <see cref="IFreezable"/> a state is stored.
        /// </summary>
        /// <param name="freezable">The freezable to check.</param>
        /// <param name="state">The state index if a state is present, otherwise -1</param>
        /// <returns>True iff a state is stored</returns>
        public bool CheckForStoredState(IFreezable freezable, out int state)
        {
            state = -1;
            
            var identifier = freezable.GetFreezableIdentifier();

            return _frozenObjectStates.TryGetValue(identifier, out state);
        }

        public void StoreState(IFreezable freezable, int stateIndex)
        {
            _frozenObjectStates[freezable.GetFreezableIdentifier()] = stateIndex;
        }

        public void RemoveState(IFreezable freezable)
        {
            _frozenObjectStates.Remove(freezable.GetFreezableIdentifier());
        }
    }
}