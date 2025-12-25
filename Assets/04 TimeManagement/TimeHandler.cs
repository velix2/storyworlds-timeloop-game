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

            CurrentTime = dayStartTimeInMinutes;
        }

        #endregion

        private void OnEnable()
        {
            // Subscribe to own event for cleanup
            onDayEnded.AddListener(ResetForNextCycle);
        }

        #region Events

        /// <summary>
        /// Fired when time passes. Should e.g. be used to let the world react to progressing time.
        /// </summary>
        public UnityEvent<TimePassedEventPayload> onTimePassed = new();

        /// <summary>
        /// Invoked when day ends. Should be used for proper cleanup/reset of objects that persist over loops.
        /// </summary>
        public UnityEvent onDayEnded = new();

        /// <summary>
        /// Reports a passing of time, which will result in onTimePassed being invoked.
        /// </summary>
        /// <param name="minutes"></param>
        public void PassTime(int minutes)
        {
            CurrentTime = CurrentTime + minutes;

            var dayHasEnded = CurrentTime >= dayStartTimeInMinutes + dayLengthInMinutes;
            var payload = new TimePassedEventPayload(minutes, CurrentTime, dayHasEnded, CurrentDaytimePhase);

            onTimePassed?.Invoke(payload);
            
            if (dayHasEnded) onDayEnded?.Invoke();
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

            // TODO all the other stuff we will need to do
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