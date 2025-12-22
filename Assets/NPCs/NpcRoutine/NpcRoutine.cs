using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace NPCs.NpcRoutine
{
    /// <summary>
    /// This Scriptable Object describes an NPC's daily routine. Also allows storing 
    /// </summary>
    [CreateAssetMenu(fileName = "NPC Routine", menuName = "Scriptable Objects/NpcRoutine")]
    public class NpcRoutine : ScriptableObject
    {
        [Serializable]
        public class RoutineElement
        {
            /// <summary>
            /// Daytime from when the NPC will be moving to the specified location, or be there. Must be in the format of HH:MM (e.g. 13:00; 24h format)
            /// </summary>
            [Tooltip(
                "Daytime from when the NPC will be moving to the specified location, or be there. Must be in the format of HH:MM (e.g. 13:00; 24h format)")]
            [SerializeField]
            private string daytime = "08:00";

            /// <summary>
            /// Name of the scene where the NPC will go to at the specified time.
            /// </summary>
            [SerializeField] private string targetSceneName;


            /// <summary>
            /// World position in the target scene where the NPC will be.
            /// </summary>
            [SerializeField] private Vector3 targetPositionInTargetScene;

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

        [Serializable]
        public class DayRoutine
        {
            [Tooltip("An Id that identifies this routine. Can e.g. be used to swap out routines after a certain event in the story.")]
            [SerializeField] private string dayRoutineId;
            
            [SerializeField] private List<RoutineElement> routineElements;
        }
        
        [Tooltip("An identifier that is used to connect the in-world NPC to its routine handler.")]
        [SerializeField] private string npcId;

        [Tooltip("List of all possible routines for this NPC")]
        [SerializeField] private List<DayRoutine> dayRoutines;
        
    }
}