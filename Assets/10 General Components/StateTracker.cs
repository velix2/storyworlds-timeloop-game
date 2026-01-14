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

    public static IntroStates IntroState = IntroStates.LeftTrainCompleted;

    public static bool IsInIntro = true;
}
