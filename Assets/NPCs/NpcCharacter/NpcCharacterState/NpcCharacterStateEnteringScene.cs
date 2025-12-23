using SceneHandling;
using UnityEngine;

namespace NPCs.NpcCharacter.NpcCharacterState
{
    public class NpcCharacterStateEnteringScene : NpcCharacterState
    {
        /// <summary>
        /// Creates a new EnteringScene state and calculates which scene entrance to take using BFS.
        /// </summary>
        /// <param name="previousScene">A previous scene where the NPC comes from. Does not need to be an immediate neighboring scene.</param>
        public NpcCharacterStateEnteringScene(SceneMetaData previousScene, Vector3 worldPosition)
        {
            // TODO implement
        }
        
        public override void OnUpdate()
        {
        }

        public override void OnStateBecameActive()
        {
            // Todo find where to move    
        }
    }
}