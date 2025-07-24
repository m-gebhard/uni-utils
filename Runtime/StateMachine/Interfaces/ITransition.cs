namespace UniUtils.StateMachine
{
    /// <summary>
    /// Represents a transition between states in a state machine.
    /// </summary>
    public interface ITransition
    {
        /// <summary>
        /// Gets the predicate that determines if the transition should occur.
        /// </summary>
        public IPredicate Predicate { get; }

        /// <summary>
        /// Gets the target state of the transition.
        /// </summary>
        public IState TargetState { get; }
    }
}