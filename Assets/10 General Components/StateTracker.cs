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
        LarryDinnerCompleted,

        // Original day
        SonCallCompleted,
        TimothyFirstEncounterCompleted,
        NoTrainRunsDayTwoCompleted,
        DinerWithRadioCompleted,
        DayReflectionCompleted,

        // First repeating day
        SonCallRepeatCompleted,
        NoTrainRunsDayTwoRepeatCompleted,
        DinerWithRadioRepeatCompleted,

        IntroCompleted,
    }

    private static IntroStates _introState = IntroStates.Init;

    public static bool IsInIntro = true;

    public static UnityEvent<IntroStates> OnIntroStateChanged = new UnityEvent<IntroStates>();

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