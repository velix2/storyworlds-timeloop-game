using System;
using UnityEngine;
using UnityEngine.Events;

namespace TimeManagement
{
    /// <summary>
    /// This singleton object is responsible for managing Time updates. It offers an event callback that is invoked when time passes.
    /// Objects (mainly the player) can report to this singleton a passing of time.
    /// </summary>
    public class TimeHandler : MonoBehaviour
    {
        /// <summary>
        /// Defines at what time the game day starts in minutes, i.e. 8*60 = 8:00
        /// </summary>
        [Tooltip("Defines at what time the game day starts in minutes, i.e. 8*60 = 8:00")] [SerializeField]
        private int dayStartTimeInMinutes = 8 * 60;

        /// <summary>
        /// Defines how long a game day is, i.e. after how many in game minutes the day resets
        /// </summary>
        [Tooltip("Defines how long a game day is, i.e. after how many in game minutes the day resets")] [SerializeField]
        private int dayLengthInMinutes = 16 * 60;

        public int CurrentTime { get; private set; }

        #region Singleton

        // Singleton stuff

        /// <summary>
        /// The singleton instance.
        /// </summary>
        public static TimeHandler Instance;


        private void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            CurrentTime = dayStartTimeInMinutes;
        }

        #endregion

        private void OnEnable()
        {
            // Subscribe to own event for cleanup
            onDayStarts.AddListener(ResetForNextCycle);
        }

        #region Events
        
        /// <summary>
        /// Fired when time passes. Should e.g. be used to let the world react to progressing time.
        /// </summary>
        public UnityEvent<TimePassedEventPayload> onTimePassed = new();

        /// <summary>
        /// Invoked when new day starts. Should be used for proper cleanup/reset of objects that persist over loops.
        /// </summary>
        public UnityEvent onDayStarts = new();

        /// <summary>
        /// Reports a passing of time, which will result in onTimePassed being invoked.
        /// </summary>
        /// <param name="minutes"></param>
        public void PassTime(int minutes)
        {
            CurrentTime = CurrentTime + minutes;

            var dayHasEnded = CurrentTime >= dayLengthInMinutes;
            var payload = new TimePassedEventPayload(minutes, CurrentTime, dayHasEnded);

            onTimePassed?.Invoke(payload);
        }

        /// <summary>
        /// Callback for when a cycle ends
        /// </summary>
        private void ResetForNextCycle()
        {
            // TODO inform Scene Management to load starting scene

            // Reset time to day start
            CurrentTime = dayStartTimeInMinutes;

            // TODO all the other stuff we will need to do
        }
        #endregion
    }
}