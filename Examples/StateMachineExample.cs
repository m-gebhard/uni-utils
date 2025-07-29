using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniUtils.Extensions;
using UniUtils.FSM;

namespace UniUtils.Examples
{
    /// <summary>
    /// Demonstrates the use of a state machine for controlling a NavMeshAgent's behavior.
    /// Includes patrol and idle states with transitions based on a predicate.
    /// </summary>
    public class StateMachineExample : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private List<Transform> waypoints;

        private bool shouldPatrol;
        private StateMachine stateMachine;

        /// <summary>
        /// Initializes the state machine and sets up transitions between states.
        /// </summary>
        private void Awake()
        {
            stateMachine = new StateMachine();

            // Initialize the patrol state with the NavMeshAgent and waypoints
            PatrolState patrolState = new(agent, waypoints);
            // Initialize the idle state
            IdleState idleState = new();

            // Create predicate to determine if the agent should patrol
            FunctionPredicate shouldPatrolPredicate = new(() => shouldPatrol);

            // Add state transitions to the state machine
            // Idle → Patrol when shouldPatrol is true
            stateMachine.AddTransition(idleState, patrolState, shouldPatrolPredicate);
            // Any state → Idle when shouldPatrol is false
            stateMachine.AddAnyTransition(idleState, !shouldPatrolPredicate);

            // Set the initial state of the state machine
            stateMachine.SetState(idleState);

            // Invoke the TogglePatrol method every 5 seconds to switch between patrol and idle states
            InvokeRepeating(nameof(TogglePatrol), 5f, 5f);
        }

        /// <summary>
        /// Updates the state machine every frame.
        /// </summary>
        private void Update()
        {
            stateMachine?.Update();
        }

        private void TogglePatrol()
        {
            shouldPatrol = !shouldPatrol;
        }
    }

    /// <summary>
    /// Represents the idle state of the agent.
    /// </summary>
    public class IdleState : IState
    {
        public void Enter()
        {
            this.Log("Entering Idle State");
        }

        public void Update()
        {
        }

        public void FixedUpdate()
        {
        }

        public void Exit()
        {
        }
    }

    /// <summary>
    /// Represents the patrol state of the agent.
    /// </summary>
    public class PatrolState : IState
    {
        /// <summary>
        /// The NavMeshAgent used for movement.
        /// </summary>
        private NavMeshAgent agent;

        /// <summary>
        /// The list of waypoints for the agent to patrol.
        /// </summary>
        private List<Transform> waypoints;

        /// <summary>
        /// Initializes a new instance of the <see cref="PatrolState"/> class.
        /// </summary>
        /// <param name="agent">The NavMeshAgent used for movement.</param>
        /// <param name="waypoints">The list of waypoints for the agent to patrol.</param>
        public PatrolState(NavMeshAgent agent, List<Transform> waypoints)
        {
            this.agent = agent;
            this.waypoints = waypoints;
        }

        /// <summary>
        /// Called when entering the patrol state. Sets the agent's destination to a random waypoint.
        /// </summary>
        public void Enter()
        {
            agent.SetDestination(waypoints.Random().position);
            this.Log("Entering Patrol State");
        }

        /// <summary>
        /// Called every frame while in the patrol state. Moves the agent to the next waypoint if it reaches the current one.
        /// </summary>
        public void Update()
        {
            if (agent.remainingDistance < agent.stoppingDistance)
            {
                // Move to the next waypoint
                agent.SetDestination(waypoints.Random().position);
            }
        }

        /// <summary>
        /// Called every fixed frame while in the patrol state.
        /// </summary>
        public void FixedUpdate()
        {
        }

        /// <summary>
        /// Called when exiting the patrol state. Resets the agent's path.
        /// </summary>
        public void Exit()
        {
            agent.ResetPath();
        }
    }
}