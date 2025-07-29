using System.Collections.Generic;

namespace UniUtils.FSM
{
    /// <summary>
    /// Represents a node in the state machine, holding a state and its transitions.
    /// </summary>
    public sealed class StateNode
    {
        /// <summary>
        /// Gets the state associated with this node.
        /// </summary>
        public IState State { get; }

        /// <summary>
        /// Gets the set of transitions from this state.
        /// </summary>
        public HashSet<ITransition> Transitions { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateNode"/> class with the specified state.
        /// </summary>
        /// <param name="state">The state to associate with this node.</param>
        public StateNode(IState state)
        {
            State = state;
            Transitions = new HashSet<ITransition>();
        }

        /// <summary>
        /// Adds a transition from this state to another state with a specified predicate.
        /// </summary>
        /// <param name="to">The state to transition to.</param>
        /// <param name="predicate">The predicate that determines if the transition should occur.</param>
        public void AddTransition(IState to, IPredicate predicate) => Transitions.Add(new Transition(predicate, to));
    }
}