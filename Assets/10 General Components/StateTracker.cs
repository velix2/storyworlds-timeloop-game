public class StateTracker
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

    public IntroStates IntroState = IntroStates.Init;

    public bool IsInIntro = true;
}
