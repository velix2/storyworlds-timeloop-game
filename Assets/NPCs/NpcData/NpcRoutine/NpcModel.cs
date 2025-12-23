using System;
using System.Collections.Generic;
using System.Linq;
using SceneHandling;
using TimeManagement;
using UnityEngine;

namespace NPCs.NpcData.NpcRoutine
{
    /// <summary>
    /// This class keeps track of an NPC's assumed internal model state. E.g. the current routine, and the corresponding current location.
    /// </summary>
    public class NpcModel
    {
        private DayRoutine _currentRoutine;
        private DayRoutine.RoutineElement _currentActivity;
        
        private NpcItinerary _itinerary;
        private SceneMetaData _currentScene;

        private NpcHandler _handler;

        public NpcModel(DayRoutine startRoutine, NpcHandler handler)
        {
            _currentRoutine = startRoutine;
            _handler = handler;

            // Subscribe to new time updates
            TimeHandler.Instance.onTimePassed.AddListener(OnTimePassed);
        }

        private void OnTimePassed(TimePassedEventPayload payload)
        {
            // Check if current activity is still the same
            var newActivity = _currentRoutine.GetCurrentRoutineElement(payload.NewDaytimeInMinutes);
            
            if (newActivity == _currentActivity) return;
            
            // Otherwise do the path planning
            var pathWithTime = CalculatePathOfScenesAndMoveTime(_currentActivity, newActivity);
            
            _currentActivity = newActivity;
        }

        /// <summary>
        /// The move speed for the heuristic representing the distance progress of an NPC in meters/second. Should be lower than the actual move speed.
        /// </summary>
        private float _moveSpeedHeuristic = 5.0f;

        /// <summary>
        /// Calculates which scenes need to be traversed to reach next activity. Also provides the time required to traverse each scene based on the heuristic value.
        /// </summary>
        /// <returns>List of scenes in order of which they need to be traversed with their corresponding time.</returns>
        private List<Tuple<SceneMetaData, float>> CalculatePathOfScenesAndMoveTime(DayRoutine.RoutineElement prevActivity, DayRoutine.RoutineElement nextActivity)
        {
            var path = prevActivity.TargetScene.FindPathToScene(nextActivity.TargetScene);

            var it = new NpcItinerary();
            var distanceToTraversePerScene = new List<float>();
            
            // Case next activity is in same scene
            if (path.Count == 1)
            {
                distanceToTraversePerScene.Add(Vector3.Distance(prevActivity.TargetPositionInTargetScene,
                    nextActivity.TargetPositionInTargetScene));
            }
            else
            {
                Vector3 startPosInScene, targetPosInScene;
                // In first scene to second scene
                targetPosInScene = path[0].GetTransitionPositionOfNeighboringScene(path[1]);
                distanceToTraversePerScene.Add(Vector3.Distance(prevActivity.TargetPositionInTargetScene, targetPosInScene));

                for (int i = 1; i < path.Count - 1; i++)
                {
                    // Pos. of link to next scene on path
                    startPosInScene = path[i].GetTransitionPositionOfNeighboringScene(path[i - 1]);
                    targetPosInScene = path[i].GetTransitionPositionOfNeighboringScene(path[i + 1]);
                    distanceToTraversePerScene.Add(Vector3.Distance(startPosInScene, targetPosInScene));
                }
                
                // in last scene to target pos
                startPosInScene = path[^1].GetTransitionPositionOfNeighboringScene(path[^2]);
                distanceToTraversePerScene.Add(Vector3.Distance(startPosInScene, nextActivity.TargetPositionInTargetScene));

            }

            return path.Zip(distanceToTraversePerScene,
                (scene, distanceToTraverse) =>
                    new Tuple<SceneMetaData, float>(scene, distanceToTraverse / _moveSpeedHeuristic)).ToList();
        }

        public void Update(float deltaTime)
        {
            _itinerary.TimeSpentInItinerary += deltaTime;
            var calculatedSceneIdx = _itinerary.GetIndexOfCurrentSceneOfItinerary(out var progress);
            var calculatedScene = _itinerary.ScenesToTraverse[calculatedSceneIdx];
            
            if (calculatedScene == _currentScene) return;
            _currentScene = calculatedScene;
            _handler.onNpcSwitchedScene?.Invoke(this, calculatedSceneIdx, _itinerary);
        }
    }
}