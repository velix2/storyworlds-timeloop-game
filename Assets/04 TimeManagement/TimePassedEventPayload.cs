namespace TimeManagement
{
    /// <summary>
    /// Contains additional info about how much time has passed, what time it is now, etc.
    /// </summary>
    public readonly struct TimePassedEventPayload
    {
        /// <summary>
        /// Minutes that have passed in this event
        /// </summary>
        public readonly int MinutesDelta;

        /// <summary>
        /// The new daytime in minutes
        /// </summary>
        public readonly int NewDaytimeInMinutes;

        /// <summary>
        /// True when the day has ended and the cycle resets
        /// </summary>
        public readonly bool DayHasEnded;

        /// <summary>
        /// The daytime phase of the current time
        /// </summary>
        public readonly DaytimePhase DaytimePhase;

        public TimePassedEventPayload(int minutesDelta, int newDaytimeInMinutes, bool dayHasEnded, DaytimePhase daytimePhase)
        {
            MinutesDelta = minutesDelta;
            NewDaytimeInMinutes = newDaytimeInMinutes;
            DayHasEnded = dayHasEnded;
            DaytimePhase = daytimePhase;
        }
    }
}