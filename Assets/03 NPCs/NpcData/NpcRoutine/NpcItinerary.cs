using System.Collections.Generic;
using SceneHandling;
using UnityEngine;

namespace NPCs.NpcData.NpcRoutine
{
    public class NpcItinerary
    {
        public List<SceneMetaData> ScenesToTraverse = new();
        public List<Vector3> StartPositions = new();
        public List<Vector3> EndPositions = new();
        public List<float> TimeToSpendInEachScene = new();
        public float ItineraryStartTime;
        public float TimeSpentInItinerary;

        /// <summary>
        /// Returns the index of the scene the npc should be in right now 
        /// </summary>
        /// <param name="progressInScenePercentage">Percentage of progress the NPC should have made in this scene.</param>
        /// <returns></returns>
        public int GetIndexOfCurrentSceneOfItinerary(out float progressInScenePercentage)
        {
            var ctr = TimeSpentInItinerary;

            for (int i = 0; i < TimeToSpendInEachScene.Count; i++)
            {
                var thisTime = TimeToSpendInEachScene[i];

                if (thisTime >= ctr)
                {
                    progressInScenePercentage = ctr / thisTime;
                    return i;
                }

                ctr -= thisTime;
            }

            progressInScenePercentage = 1f;
            return TimeToSpendInEachScene.Count - 1;
        }
    }
}