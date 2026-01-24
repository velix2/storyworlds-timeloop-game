using System;
using NPCs.NpcCharacter.NpcCharacterState;
using NPCs.NpcData;
using UnityEngine;
using UnityEngine.AI;

namespace NPCs.NpcCharacter
{
    /// <summary>
    /// The component of the NPC in world. It essentially functions as a puppet for the model of the NPC, i.e. its day routine.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
    public class NpcCharacter : MonoBehaviour
    {
        /// <summary>
        /// The <see cref="NpcModel"/> this NpcCharacter represents. Needs to be set after instantiating.
         /// </summary>
        public NpcModel Model { get; set; }

        public bool CanBeSpokenTo => _state is NpcCharacterStateIdle;

        /// <summary>
        /// The current <see cref="NpcCharacterState"/> of the Npc
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

        private Animator animator;
        
        private void Awake()
        {
            Agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            //IDs for animator variables
            lookX = Animator.StringToHash("lookX");
            lookY = Animator.StringToHash("lookY");
            moveX = Animator.StringToHash("moveX");
            moveY = Animator.StringToHash("moveY");
            magnitude = Animator.StringToHash("moveMagnitude");
            
        }

        private void Start()
        {
            animator.runtimeAnimatorController = Model.WalkAnimator;
            
            // Place Agent on next best location on mesh
            if (!Agent.isOnNavMesh)
            {
                var searchRadius = 2f;
                // 1. Try to find the nearest valid point on the NavMesh
                if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, searchRadius, NavMesh.AllAreas))
                {
                    // 2. Teleport the agent to that specific point
                    Agent.Warp(hit.position);
                }
                else
                {
                    Debug.LogWarning("Could not find a NavMesh location within radius: " + searchRadius);
                }
            }
        }

        private void OnDestroy()
        {
            // Forward cleanup to state
            _state.OnStateExit();
        }

        /// <summary>
        /// Updates the NPC's state and invokes <see cref="NpcCharacterState.OnStateBecameActive"/> on the new state.
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

        
        private int lookX;
        private int lookY;
        private int moveX;
        private int moveY;
        private int magnitude;
        private void LateUpdate()
        {
            if (IsAtDestination)
            {
                animator.SetFloat(lookX, 0f);
                animator.SetFloat(lookY, -1f);
                
                animator.SetFloat(moveY, 0);
                animator.SetFloat(moveX, 0);
                
                animator.SetFloat(magnitude, 0);
                return;
            }
            
            Vector2 move = new Vector2(Agent.desiredVelocity.x, Agent.desiredVelocity.z);
            animator.SetFloat(moveY, move.y);
            animator.SetFloat(moveX, move.x);
            float mag = move.sqrMagnitude;
            animator.SetFloat(magnitude, mag);
            if (mag > 0.1)
            {
                animator.SetFloat(lookX, move.x);
                animator.SetFloat(lookY, move.y);
            }
            
            
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