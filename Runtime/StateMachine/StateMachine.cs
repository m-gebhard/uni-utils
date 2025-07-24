using System;
using System.Linq;

namespace UniUtils.StateMachine
{
    /// <summary>
    /// Represents a state machine that manages state transitions and updates.
    /// </summary>
    /// <example>
    /// Example usage:
    /// <code>
    /// public class IdleState : IState
    /// {
    ///     public void Enter() => Debug.Log("Entered Idle");
    ///     public void Update() => Debug.Log("Updating Idle");
    ///     public void FixedUpdate() { }
    ///     public void Exit() => Debug.Log("Exited Idle");
    /// }
    ///
    /// public class MoveState : IState
    /// {
    ///     public void Enter() => Debug.Log("Entered Move");
    ///     public void Update() => Debug.Log("Updating Move");
    ///     public void FixedUpdate() { }
    ///     public void Exit() => Debug.Log("Exited Move");
    /// }
    ///
    /// void SetupStateMachine()
    /// {
    ///     IdleState idle = new IdleState();
    ///     MoveState move = new MoveState();
    ///     StateMachine stateMachine = new StateMachine();
    ///
    ///     // Add transitions
    ///     stateMachine.AddTransition(idle, move, new FunctionPredicate(() =&gt; Input.GetKey(KeyCode.Space)));
    ///     stateMachine.AddTransition(move, idle, new FunctionPredicate(() =&gt; !Input.GetKey(KeyCode.Space)));
    ///
    ///     // Start in Idle
    ///     stateMachine.SetState(idle);
    ///
    ///     // In your MonoBehaviour.Update():
    ///     stateMachine.Update();
    /// }
    /// </code>
    /// </example>
    public class StateMachine
    {
        public StateNode CurrentState { get; private set; }
        public event Action <IState> OnStateChanged;

        private readonly StateMachineNodeManager nodeManager = new();

        /// <summary>
        /// Updates the state machine, checking for transitions and updating the current state.
        /// </summary>
        public void Update()
        {
            ITransition transition = GetNextStateTransition();

            if (transition != null)
            {
                ChangeState(transition.TargetState);
            }

            CurrentState?.State?.Update();
        }

        /// <summary>
        /// Calls the FixedUpdate method on the current state.
        /// </summary>
        public void FixedUpdate()
        {
            CurrentState?.State?.FixedUpdate();
        }

        /// <summary>
        /// Sets the current state of the state machine.
        /// </summary>
        /// <param name="state">The new state to set.</param>
        public void SetState(IState state)
        {
            CurrentState = nodeManager.Nodes[state.GetType()];
            CurrentState.State?.Enter();
            OnStateChanged?.Invoke(CurrentState.State);
        }

        /// <summary>
        /// Changes the current state to a new state.
        /// </summary>
        /// <param name="state">The new state to change to.</param>
        private void ChangeState(IState state)
        {
            if (state == CurrentState?.State)
            {
                return;
            }

            IState previousState = CurrentState?.State;
            StateNode newStateNode = nodeManager.Nodes[state.GetType()];
            IState newState = newStateNode.State;

            previousState?.Exit();
            newState?.Enter();

            CurrentState = newStateNode;
            OnStateChanged?.Invoke(newState);
        }

        /// <summary>
        /// Adds a transition from one state to another with a specified predicate.
        /// </summary>
        /// <param name="from">The state to transition from.</param>
        /// <param name="to">The state to transition to.</param>
        /// <param name="predicate">The predicate that determines if the transition should occur.</param>
        public void AddTransition(IState from, IState to, IPredicate predicate) =>
            nodeManager.AddTransition(from, to, predicate);

        /// <summary>
        /// Adds a transition from any state to a specified state with a specified predicate.
        /// </summary>
        /// <param name="to">The state to transition to.</param>
        /// <param name="predicate">The predicate that determines if the transition should occur.</param>
        public void AddAnyTransition(IState to, IPredicate predicate) =>
            nodeManager.AddAnyTransition(to, predicate);

        /// <summary>
        /// Gets the next state transition based on the current state and any transitions.
        /// </summary>
        /// <returns>The next state transition if one is found; otherwise, null.</returns>
        private ITransition GetNextStateTransition()
        {
            ITransition possibleAnyTransition =
                nodeManager.AnyTransitions.FirstOrDefault(t => t.Predicate.Evaluate());

            return possibleAnyTransition ?? CurrentState?.Transitions?.FirstOrDefault(t => t.Predicate.Evaluate());
        }
    }
}