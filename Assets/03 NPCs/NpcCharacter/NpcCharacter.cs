using System;
using NPCs.NpcData;
using UnityEngine;
using UnityEngine.AI;

namespace NPCs.NpcCharacter
{
    /// <summary>
    /// The component of the NPC in world. It essentially functions as a puppet for the model of the NPC, i.e. its day routine.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class NpcCharacter : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="NpcModel"/> this NpcCharacter represents. Needs to be set after instantiating.
         /// </summary>
        public NpcModel Model { get; set; }

        /// <summary>
        /// The current <see cref="NpcCharacterState.NpcCharacterState"/> of the Npc
        /// </summary>
        private NpcCharacterState.NpcCharacterState _state;
        
        public NavMeshAgent Agent { get; private set; }
        
        /// <summary>
        /// Returns true if the NavMeshAgent is at the goal position.
        /// </summary>
        public bool IsAtDestination => 
            !Agent.pathPending && 
            Agent.remainingDistance <= Agent.stoppingDistance &&
            (!Agent.hasPath || Agent.velocity.sqrMagnitude == 0f);
        
        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
        }

        private void OnDestroy()
        {
            // Forward cleanup to state
            _state.OnStateExit();
        }

        /// <summary>
        /// Updates the NPC's state and invokes <see cref="NpcCharacterState.NpcCharacterState.OnStateBecameActive"/> on the new state.
        /// </summary>
        /// <param name="newState">The state to be applied</param>
        public void UpdateState(NpcCharacterState.NpcCharacterState newState)
        {
            _state?.OnStateExit();
            _state = newState;
            newState.Character = this;
            newState.OnStateBecameActive();
        }

        private void Update()
        {
            _state.OnUpdate();
        }

        /// <summary>
        /// Sets a new position goal for the nav mesh agent.
        /// </summary>
        /// <param name="pos">The target position in world space.</param>
        public void MoveTo(Vector3 pos)
        {
            Agent.SetDestination(pos);
        }
    }
}