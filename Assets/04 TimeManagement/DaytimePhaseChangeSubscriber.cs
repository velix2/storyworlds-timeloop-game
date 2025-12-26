using System;
using UnityEngine;

namespace TimeManagement
{
    public abstract class DaytimePhaseChangeSubscriber<T> : MonoBehaviour
    {
        [SerializeField] private T elementMorning, elementAfternoon, elementEvening, elementNight;

        private void OnEnable()
        {
            TimeHandler.Instance.onDayPhaseChanged.AddListener(OnDaytimePhaseChanged);
            AfterOnEnable();
        }

        private void OnDisable()
        {
            TimeHandler.Instance.onDayPhaseChanged.RemoveListener(OnDaytimePhaseChanged);
            AfterOnDisable();
        }

        /// <summary>
        /// Replacement for OnEnable(). Works exactly the same.
        /// </summary>
        protected virtual void AfterOnEnable()
        {
        }

        /// <summary>
        /// Replacement for OnDisable(). Works exactly the same.
        /// </summary>
        protected virtual void AfterOnDisable()
        {
        }

        /// <summary>
        /// Invoked when the daytime phase changes while passing the correct element to apply.
        /// </summary>
        /// <param name="elementToApply">The element which should be applied.</param>
        /// <param name="daytimePhase">The corresponding new daytime phase.</param>
        protected abstract void ApplyElement(T elementToApply, DaytimePhase daytimePhase);

        private void OnDaytimePhaseChanged(DaytimePhase phase)
        {
            switch (phase)
            {
                case DaytimePhase.Night:
                    ApplyElement(elementNight, DaytimePhase.Night);
                    break;
                case DaytimePhase.Morning:
                    ApplyElement(elementMorning, DaytimePhase.Morning);
                    break;
                case DaytimePhase.Afternoon:
                    ApplyElement(elementAfternoon, DaytimePhase.Afternoon);
                    break;
                case DaytimePhase.Evening:
                    ApplyElement(elementEvening, DaytimePhase.Evening);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }
    }
}