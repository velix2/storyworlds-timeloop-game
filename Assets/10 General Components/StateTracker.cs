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

    private static IntroStates _introState = IntroStates.TimothyFirstEncounterCompleted;

    public static IntroStates IntroState
    {
        get => _introState;
        set
        {
            _introState = value;
            OnIntroStateChanged.Invoke(_introState);
        }
    }
}