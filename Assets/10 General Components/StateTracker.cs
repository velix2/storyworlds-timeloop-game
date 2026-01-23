using UnityEngine.Events;

public static class StateTracker
{
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

    private static IntroStates _introState = IntroStates.LeftTrainCompleted;

    public static IntroStates IntroState
    {
        get => _introState;
        set
        {
            _introState = value;
            OnIntroStateChanged.Invoke(_introState);
        }
    }

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
            newSonStateAvailable = value != _sonCallState;
            _sonCallState = value;
        }
    }

}