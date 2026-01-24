using UnityEngine.Events;

public static class StateTracker
{

    #region Intro
    
    public enum IntroStates
    {
        Init,

        LeftTrainCompleted, // first free roam
        RarityShopCompleted,
        DinerCompleted,
        NoTrainRunsCompleted,
        MotelCompleted,
        LarryWaitingCompleted,
        LarrySweetHomeCompleted,
        LarryDinnerCompleted,

        // Original day
        SonCallCompleted,
        TimothyFirstEncounterCompleted,
        DinerWithRadioCompleted,

        // First repeating day
        SonCallRepeatCompleted,
        NoTrainRuns2Completed,
        DinerWithRadioRepeatCompleted,
        EverythingRepeatsCompleted,

        IntroCompleted,
    }

    public static bool IsInIntro => IntroState != IntroStates.IntroCompleted;

    public static UnityEvent<IntroStates> OnIntroStateChanged = new UnityEvent<IntroStates>();

    private static IntroStates _introState = IntroStates.IntroCompleted;

    public static IntroStates IntroState
    {
        get => _introState;
        set
        {
            _introState = value;
            OnIntroStateChanged.Invoke(_introState);
        }
    }
    
    #endregion

    #region SonCall
    
    public enum SonCallStates
    {
        //replace names relating to the story progress
        firstCall,
        secondCall
    }

    public static bool newSonStateAvailable = true;

    private static SonCallStates _sonCallState;
    public static SonCallStates SonCallState
    {
        get => _sonCallState;
        set
        {
            //newSonStateAvailable = value != _sonCallState;
            _sonCallState = value;
        }
    }
    
    #endregion

    #region CoffeeMachineQuest
    public struct EvelynState
    {
        public enum QuestStates
        {
            Init,
            IntroCutsceneWatched,
            TalkedTo,
            TriedLeavingTown,
            Completed
        }
        
        /// <summary>
        /// Var used in dialogue with Evelyn. Value determines her answers in regular dialogue.
        /// </summary>
        public bool coffeeGiven //coffee given is a separate var to allow further quest expanding and puzzles 
        {
            get => (bool)DialogueManager.Instance.variableObserver.GetVariable("evelynCoffeeGot");
            set => DialogueManager.Instance.variableObserver.SetVariable("evelynCoffeeGot", value);
        }
        
        /// <summary>
        /// The player has asked Evelyn for a ride, and she agreed to it. She will be waiting outside.
        /// </summary>
        public bool readyForRide
        {
            set => DialogueManager.Instance.variableObserver.SetVariable("evelynReadyForRide", value);
            get => (bool)DialogueManager.Instance.variableObserver.GetVariable("evelynReadyForRide");
        }

        /// <summary>
        /// The player confirms his decision to ride with Evelyn, when she is waiting outside<b/>
        /// Used to trigger ride cutscene.
        /// </summary>
        public bool rideConfirmation
        {
            set => DialogueManager.Instance.variableObserver.SetVariable("evelynRideConfirmation", value);
            get => (bool)DialogueManager.Instance.variableObserver.GetVariable("evelynRideConfirmation");
        }
        
        /// <summary>
        /// The player already tried leaving town via Evelyn.
        /// </summary>
        public bool triedLeaving;
        
        private QuestStates _questState;

        public QuestStates QuestState
        {
            get => _questState;
            set => _questState = value;
        }
        
    }

    public static EvelynState Evelyn = new EvelynState();

    #endregion


}