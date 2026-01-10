using System;
using System.Collections;
using UnityEngine;

namespace TimeManagement
{
    public abstract class DaytimePhaseChangeSubscriber<T> : MonoBehaviour
    {
        [Tooltip("The element that will be applied at morning")] [SerializeField]
        protected T elementMorning;

        [Tooltip("The element that will be applied at afternoon")] [SerializeField]
        protected T elementAfternoon;

        [Tooltip("The element that will be applied at evening")] [SerializeField]
        protected T elementEvening;

        [Tooltip("The element that will be applied at night")] [SerializeField]
        protected T elementNight;

        [SerializeField] private float invocationDelaySeconds = 0.25f;
        

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

        private void Start()
        {
            BeforeStart();
            
            switch (TimeHandler.Instance.CurrentDaytimePhase)
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
            }
        }

        /// <summary>
        /// Replacement for <see cref="OnEnable"/>. Works exactly the same.
        /// </summary>
        protected virtual void AfterOnEnable()
        {
        }

        /// <summary>
        /// Replacement for <see cref="OnDisable"/>. Works exactly the same.
        /// </summary>
        protected virtual void AfterOnDisable()
        {
        }
        
        /// <summary>
        /// Replacement for <see cref="Start"/>. Runs before the base class' Start.
        /// </summary>
        protected virtual void BeforeStart()
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
                    StartCoroutine(ApplyElementWrapperCoroutine(elementNight, DaytimePhase.Night));
                    break;
                case DaytimePhase.Morning:
                    StartCoroutine(ApplyElementWrapperCoroutine(elementMorning, DaytimePhase.Morning));
                    break;
                case DaytimePhase.Afternoon:
                    StartCoroutine(ApplyElementWrapperCoroutine(elementAfternoon, DaytimePhase.Afternoon));
                    break;
                case DaytimePhase.Evening:
                    StartCoroutine(ApplyElementWrapperCoroutine(elementEvening, DaytimePhase.Evening));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(phase), phase, null);
            }
        }

        private IEnumerator ApplyElementWrapperCoroutine(T elem, DaytimePhase phase)
        {
            yield return new WaitForSeconds(invocationDelaySeconds);
            ApplyElement(elem, phase);
        }
    }
}