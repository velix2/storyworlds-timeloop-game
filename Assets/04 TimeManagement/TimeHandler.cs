using System;
using System.Collections;
using DataClasses;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
        
        [Tooltip("Defines at what time the first intro day starts in minutes, i.e. 12*60 = 12:00")] [SerializeField]
        private int firstIntroDayStartTimeInMinutes = 12 * 60;

        /// <summary>
        /// Defines how long a game day is, i.e. after how many in game minutes the day resets
        /// </summary>
        [Tooltip("Defines how long a game day is, i.e. after how many in game minutes the day resets")] [SerializeField]
        private int dayLengthInMinutes = 16 * 60;

        [Space] [Tooltip("Defines when morning begins in day minutes, i.e. 8*60 = 8:00")] [SerializeField]
        private int morningBeginInMinutes = 8 * 60;

        [Tooltip("Defines when afternoon begins in day minutes, i.e. 13*60 = 14:00")] [SerializeField]
        private int afternoonBeginInMinutes = 13 * 60;
        
        [Tooltip("Defines when evening begins in day minutes, i.e. 15*60 + 30 = 15:30")] [SerializeField]
        private int eveningBeginInMinutes = 15 * 60 + 30;
        
        [Tooltip("Defines when night begins in day minutes, i.e. 17*60 = 17:00")] [SerializeField]
        private int nightBeginInMinutes = 17 * 60;
        
        public int CurrentTime { get; private set; }

        /// <summary>
        /// Stores prev phase to detect when daytime phase changes for event invocation
        /// </summary>
        private DaytimePhase _prevPhase = DaytimePhase.Night;
        
        /// <summary>
        /// Returns the current daytime phase.
        /// </summary>
        public DaytimePhase CurrentDaytimePhase
        {
            get
            {
                var t = CurrentTime;
                if (t < morningBeginInMinutes) return DaytimePhase.Night;
                if (t < afternoonBeginInMinutes) return DaytimePhase.Morning;
                if (t < eveningBeginInMinutes) return DaytimePhase.Afternoon;
                if (t < nightBeginInMinutes) return DaytimePhase.Evening;
                return DaytimePhase.Night;
            }
        }

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

            CurrentTime = firstIntroDayStartTimeInMinutes;
        }

        #endregion
        
        #region Events

        /// <summary>
        /// Fired when time passes. Should e.g. be used to let the world react to progressing time.
        /// </summary>
        public UnityEvent<TimePassedEventPayload> onTimePassed = new();
        

        /// <summary>
        /// Invoked when day phase changes. Sends the new phase as payload.
        /// Can be used for visual changes.
        /// </summary>
        public UnityEvent<DaytimePhase> onDayPhaseChanged = new();
        
        private void OnEnable()
        {
            // Subscribe to own event for cleanup
            onTimePassed.AddListener(ResetForNextCycleWrapper);
        }


        private void OnDisable()
        {
            onTimePassed.RemoveListener(ResetForNextCycleWrapper);
        }

        private void ResetForNextCycleWrapper(TimePassedEventPayload payload)
        {
            if (payload.DayHasEnded) ResetForNextCycle();
        }
        
        private void Start()
        {
            // After everything finished setting up, call one initial Time event
            StartCoroutine(PostStart());
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private IEnumerator PostStart()
        {
            yield return new WaitForEndOfFrame();
            // Send one initial PassTime for Setup
            PassTime(0);
        }

        /// <summary>
        /// Reports a passing of time, which will result in onTimePassed being invoked, as well as onDayEnded, if the day ended.
        /// Also invokes onDaytimePhaseChanged, if daytime phase changed.
        /// </summary>
        /// <param name="minutes"></param>
        public static void PassTime(int minutes)
        {
            if (Instance == null)
            {
                Debug.Log("There is no TimeHandler in the scene.");
                return;
            }

            if (minutes < 0)
            {
                Debug.LogError("Tried passing negative minutes: " + minutes);
                return;
            }
            Instance.CurrentTime += minutes;

            var dayHasEnded = Instance.CurrentTime >= Instance.dayStartTimeInMinutes + Instance.dayLengthInMinutes;
            var currentDaytimePhase = Instance.CurrentDaytimePhase;
            var payload = new TimePassedEventPayload(minutes, Instance.CurrentTime, dayHasEnded, currentDaytimePhase);
            
            
            if (currentDaytimePhase != Instance._prevPhase)
            {
                Instance._prevPhase = currentDaytimePhase;
                Instance.onDayPhaseChanged?.Invoke(currentDaytimePhase);
            }
            
            Instance.onTimePassed?.Invoke(payload);
        }

        public static void ResetDay()
        {
            if (Instance == null)
            {
                Debug.Log("There is no TimeHandler in the scene.");
                return;
            }

            // Set this to night, so that we trigger a daytime phase change for sure after reset
            Instance._prevPhase = DaytimePhase.Night;
            Instance.ResetForNextCycle();
        }

        /// <summary>
        /// Sets the time of day to a certain day phase. Does not trigger a day reset e.g. if in the evening, set to morning is called.
        /// </summary>
        /// <param name="phase">The phase to set the time to</param>
        public void SetTimeToPhase(DaytimePhase phase)
        {
            var targetTime = phase switch
            {
                DaytimePhase.Night => nightBeginInMinutes,
                DaytimePhase.Morning => morningBeginInMinutes,
                DaytimePhase.Afternoon => afternoonBeginInMinutes,
                DaytimePhase.Evening => eveningBeginInMinutes,
                _ => throw new ArgumentOutOfRangeException(nameof(phase), phase, null)
            };

            CurrentTime = targetTime;
            
            // Trigger an empty pass time event so everything updates correctly
            PassTime(0);
        } 

        public static void SkipToNextDaytimePhase()
        {
            if (Instance == null)
            {
                Debug.Log("There is no TimeHandler in the scene.");
                return;
            }
            
            int timeUntilNextPhase = Instance.CurrentTime;
            switch (Instance.CurrentDaytimePhase)
            {
                case DaytimePhase.Morning:
                    timeUntilNextPhase = -timeUntilNextPhase + Instance.afternoonBeginInMinutes;
                    break;
                case DaytimePhase.Afternoon:
                    timeUntilNextPhase = -timeUntilNextPhase + Instance.eveningBeginInMinutes;
                    break;
                case DaytimePhase.Evening:
                    timeUntilNextPhase = -timeUntilNextPhase + Instance.nightBeginInMinutes;
                    break;
                case DaytimePhase.Night:
                    Instance.ResetForNextCycle();
                    goto default;
                default:
                    Debug.Log("Tried skipping to an unknown DaytimePhase.");
                    return;
            }
            
            PassTime(timeUntilNextPhase);
        }

        /// <summary>
        /// Callback for when a cycle ends
        /// </summary>
        private void ResetForNextCycle()
        {
            // Inform Scene Management to load starting scene
            SceneManager.LoadScene(GameData.StartingSceneName);
            
            // Reset time to day start
            CurrentTime = dayStartTimeInMinutes;
            
            // Send a Pass Time signal with 0 mins just for proper set up of all events etc.
            PassTime(0);

            ResetQuestProgression();
        }

        /// <summary>
        /// Called to reset all quest progressions which are not frozen
        /// </summary>
        private void ResetQuestProgression()
        {
            //Evelyn Coffee Quest
            if (StateTracker.EvelynQuestState == StateTracker.EvelynQuestStates.CoffeeGiven)
            {
                StateTracker.EvelynQuestState = StateTracker.EvelynQuestStates.TalkedTo;
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Formats the given day time in minutes to a 24h formatted string
        /// </summary>
        /// <param name="gameMinutes">The Day time in minutes</param>
        /// <returns>The formatted 24h string notation</returns>
        public static string GameMinutesToFormatted(int gameMinutes)
        {
            var hours = (gameMinutes / 60) % 24;
            var minutes = gameMinutes % 60;
            return $"{hours:D2}:{minutes:D2}";
        }

        #endregion
    }
}