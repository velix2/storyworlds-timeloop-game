using System;
using SceneHandling;
using UnityEngine;

namespace NPCs.NpcCharacter.NpcCharacterState
{
    public class NpcCharacterStateLeavingScene : NpcCharacterState
    {
        private Vector3 _targetPos;

        /// <summary>
        /// Creates a new LeavingScene state and calculates which scene exit to take using BFS.
        /// </summary>
        /// <param name="currentScene"></param>
        /// <param name="targetScene">The desired target scene. Does not need to be an immediate neighboring scene.</param>
        public NpcCharacterStateLeavingScene(SceneMetaData currentScene, SceneMetaData targetScene)
        {
            var path = currentScene.FindPathToScene(targetScene);
            if (path.Count < 2)
                throw new ArgumentException("Provided scene is not neighboring scene", nameof(targetScene));

            var nextNeighbor = path[1];
            _targetPos = currentScene.GetTransitionPositionOfNeighboringScene(nextNeighbor);
        }

        public override void OnUpdate()
        {
        }

        public override void OnStateBecameActive()
        {
            Character.MoveTo(_targetPos);
        }
    }
}