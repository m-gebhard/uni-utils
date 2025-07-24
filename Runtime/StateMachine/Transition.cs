namespace UniUtils.StateMachine
{
    /// <summary>
    /// Implements a transition between states in a state machine.
    /// </summary>
    /// <example>
    /// Example usage:
    /// <code>
    /// void CreateTransition()
    /// {
    ///     IState idle = new IdleState();
    ///     IState move = new MoveState();
    ///
    ///     IPredicate canMove = new FunctionPredicate(() =&gt; Input.GetKey(KeyCode.Space));
    ///
    ///     ITransition transition = new Transition(canMove, move);
    ///
    ///     // Example: use in a state machine
    ///     StateMachine stateMachine = new StateMachine();
    ///     stateMachine.AddTransition(idle, move, canMove);
    /// }
    /// </code>
    /// </example>
    public class Transition : ITransition
    {
        /// <summary>
        /// Gets the predicate that determines if the transition should occur.
        /// </summary>
        public IPredicate Predicate { get; }

        /// <summary>
        /// Gets the target state of the transition.
        /// </summary>
        public IState TargetState { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Transition"/> class.
        /// </summary>
        /// <param name="predicate">The predicate that determines if the transition should occur.</param>
        /// <param name="targetState">The target state of the transition.</param>
        public Transition(IPredicate predicate, IState targetState)
        {
            Predicate = predicate;
            TargetState = targetState;
        }
    }
}