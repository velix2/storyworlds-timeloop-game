using NPCs.NpcCharacter.NpcCharacterState;
using NPCs.NpcData;
using UnityEngine;

namespace NPCs.NpcCharacter
{
    public class NpcCharacter : MonoBehaviour
    {
        public NpcModel Model { get; set; }

        private NpcCharacterState.NpcCharacterState _state;

        public void UpdateState(NpcCharacterState.NpcCharacterState newState)
        {
            // TODO implement
            throw new System.NotImplementedException();
        }
    }
}