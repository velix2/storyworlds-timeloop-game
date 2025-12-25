using NPCs.NpcData;
using UnityEngine;
using UnityEngine.AI;

namespace NPCs.NpcCharacter
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NpcCharacter : MonoBehaviour
    {
        public NpcModel Model { get; set; }

        private NpcCharacterState.NpcCharacterState _state;

        public NavMeshAgent Agent { get; private set; }

        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
        }

        public void UpdateState(NpcCharacterState.NpcCharacterState newState)
        {
            _state = newState;
            newState.Character = this;
            newState.OnStateBecameActive();
        }

        private void Update()
        {
            _state.OnUpdate();
        }

        public void MoveTo(Vector3 pos)
        {
            Agent.SetDestination(pos);
        }
    }
}