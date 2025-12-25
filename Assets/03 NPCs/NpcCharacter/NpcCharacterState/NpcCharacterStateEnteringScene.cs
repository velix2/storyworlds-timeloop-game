using System;
using SceneHandling;
using UnityEngine;

namespace NPCs.NpcCharacter.NpcCharacterState
{
    public class NpcCharacterStateEnteringScene : NpcCharacterState
    {
        private Vector3 _entrancePos, _targetPos;

        /// <summary>
        /// Creates a new EnteringScene state and calculates which scene entrance to take using BFS.
        /// </summary>
        /// <param name="previousScene">A previous scene where the NPC comes from. Does not need to be an immediate neighboring scene.</param>
        /// <param name="currentScene">The scene the NPC is in now</param>
        /// <param name="worldPosition">The world position of where the NPC will navigate to</param>
        public NpcCharacterStateEnteringScene(SceneMetaData previousScene, SceneMetaData currentScene, Vector3 worldPosition)
        {
            var path = previousScene.FindPathToScene(currentScene);
            if (path.Count < 2)
                throw new ArgumentException("Provided scene is not neighboring scene", nameof(previousScene));

            var prevNeighbor = path[^2];
            _entrancePos = currentScene.GetTransitionPositionOfNeighboringScene(prevNeighbor);
            _targetPos = worldPosition;
        }
        
        public override void OnUpdate()
        {
            if (Character.IsAtDestination)
            {
                Character.UpdateState(new NpcCharacterStateIdle());
            }
        }

        public override void OnStateBecameActive()
        {
            Character.transform.position = _entrancePos;
            Character.MoveTo(_targetPos);
        }
    }
}