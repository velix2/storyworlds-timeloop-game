using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SceneHandling;
using UnityEngine;

namespace NPCs.NpcData.NpcRoutine
{
    /// <summary>
    /// This Scriptable Object describes an NPC's daily routine. Also allows storing 
    /// </summary>
    [CreateAssetMenu(fileName = "NPC Day Routine", menuName = "Scriptable Objects/NpcDayRoutine")]
    public class DayRoutine : ScriptableObject
    {
        [Serializable]
        public class RoutineElement
        {

            [Tooltip(
                "Daytime from when the NPC will be moving to the specified location, or be there. Must be in the format of HH:MM (e.g. 13:00; 24h format)")]
            [SerializeField]
            private string daytime = "08:00";

            [Tooltip("Scene meta data where the NPC will go to at the specified time.")] [SerializeField]
            private SceneMetaData targetScene;


            [Tooltip("World position in the target scene where the NPC will be.")] [SerializeField]
            private Vector3 targetPositionInTargetScene;

            public SceneMetaData TargetScene => targetScene;

            public Vector3 TargetPositionInTargetScene => targetPositionInTargetScene;

            /// <summary>
            /// Returns the specified Daytime as minutes
            /// </summary>
            public int DayTimeInMinutes
            {
                get
                {
                    // check cached first and ensure cached value has not changed
                    if (_cachedDayTimeString == daytime) return _cachedDayTimeMinutes;

                    // Use Regex to validate format
                    if (!Regex.IsMatch(daytime, @"^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$"))
                    {
                        throw new FormatException(
                            $"Invalid daytime format on NPC Routine {ToString()}: '{daytime}'. Must be HH:MM.");
                    }

                    var split = daytime.Split(':');

                    // Parse string
                    int.TryParse(split[0], out int hours);
                    int.TryParse(split[1], out int minutes);

                    var dayTimeInMinutes = hours * 60 + minutes;

                    // Cache value
                    _cachedDayTimeString = daytime;
                    _cachedDayTimeMinutes = dayTimeInMinutes;

                    return dayTimeInMinutes;
                }
            }

            private int _cachedDayTimeMinutes;
            private string _cachedDayTimeString = "";
        }

        [SerializeField] private List<RoutineElement> routineElements;

        /// <summary>
        /// Internal data structure that keeps track of routine elements.
        /// </summary>
        private SortedDictionary<int, RoutineElement> _routineElementsSortedByDayMinutes;

        private void Awake()
        {
            _routineElementsSortedByDayMinutes =
                new SortedDictionary<int, RoutineElement>(
                    routineElements.ToDictionary(element => element.DayTimeInMinutes, e => e));
        }

        public RoutineElement GetCurrentRoutineElement(int daytimeMinutes)
        {
            var dayTimeOfCurrentActivity = _routineElementsSortedByDayMinutes.Keys
                .Where(i => i <= daytimeMinutes)
                .OrderByDescending(i => i)
                .FirstOrDefault();

            return _routineElementsSortedByDayMinutes[dayTimeOfCurrentActivity];
        }

        public RoutineElement GetPreviousRoutineElement(int daytimeMinutes)
        {
            var dayTimeOfPrevActivity = _routineElementsSortedByDayMinutes.Keys
                .Where(i => i <= daytimeMinutes)
                .OrderByDescending(i => i)
                .ElementAtOrDefault(1);

            return _routineElementsSortedByDayMinutes[dayTimeOfPrevActivity];
        }


        /// <summary>
        /// Allows right click to visually sort the elements. No impact on implementation.
        /// </summary>
        [ContextMenu("Sort Routine Elements")]
        public void SortRoutineElements()
        {
            routineElements.Sort((e0, e1) => e0.DayTimeInMinutes - e1.DayTimeInMinutes);
        }
    }
}