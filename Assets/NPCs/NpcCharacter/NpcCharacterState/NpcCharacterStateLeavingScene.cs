using SceneHandling;

namespace NPCs.NpcCharacter.NpcCharacterState
{
    public class NpcCharacterStateLeavingScene : NpcCharacterState
    {
        /// <summary>
        /// Creates a new LeavingScene state and calculates which scene exit to take using BFS.
        /// </summary>
        /// <param name="targetScene">The desired target scene. Does not need to be an immediate neighboring scene.</param>
        public NpcCharacterStateLeavingScene(SceneMetaData targetScene)
        {
            // TODO implement
        }
        
        public override void OnUpdate()
        {
            // Move to scene exit
        }

        public override void OnStateBecameActive()
        {
            // Todo find where to move    
        }
    }
}