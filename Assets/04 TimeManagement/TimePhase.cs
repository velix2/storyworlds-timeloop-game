using System;
using UnityEngine;

namespace TimeManagement
{
    [Serializable]
    public class TimePhase
    {
        [SerializeField] private int startTime, endTime;

        /// <summary>
        /// Returns true iff t is in this time phase.
        /// Wraps around midnight.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool IsBetween(int t)
        {
            if (startTime <= endTime)
            {
                return startTime <= t && t <= endTime;
            }
            else
            {
                return startTime <= t || t <= endTime;
            }
        }
    }
}