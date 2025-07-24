using System;
using System.Collections.Generic;

namespace UniUtils.StateMachine
{
    /// <summary>
    /// Manages the nodes and transitions for a state machine.
    /// </summary>
    internal sealed class StateMachineNodeManager
    {
        /// <summary>
        /// Gets the dictionary of state nodes.
        /// </summary>
        public Dictionary<Type, StateNode> Nodes { get; } = new();

        /// <summary>
        /// Gets the set of transitions that can occur from any state.
        /// </summary>
        public HashSet<ITransition> AnyTransitions { get; } = new();

        /// <summary>
        /// Adds a transition from one state to another with a specified predicate.
        /// </summary>
        /// <param name="from">The state to transition from.</param>
        /// <param name="to">The state to transition to.</param>
        /// <param name="predicate">The predicate that determines if the transition should occur.</param>
        public void AddTransition(IState from, IState to, IPredicate predicate)
        {
            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, predicate);
        }

        /// <summary>
        /// Adds a transition from any state to a specified state with a specified predicate.
        /// </summary>
        /// <param name="to">The state to transition to.</param>
        /// <param name="predicate">The predicate that determines if the transition should occur.</param>
        public void AddAnyTransition(IState to, IPredicate predicate)
        {
            AnyTransitions.Add(new Transition(predicate, GetOrAddNode(to).State));
        }

        /// <summary>
        /// Gets or adds a state node for the specified state.
        /// </summary>
        /// <param name="state">The state to get or add a node for.</param>
        /// <returns>The state node for the specified state.</returns>
        private StateNode GetOrAddNode(IState state)
        {
            StateNode node = Nodes.GetValueOrDefault(state.GetType());

            if (node != null) return node;

            node = new StateNode(state);
            Nodes.Add(state.GetType(), node);

            return node;
        }
    }
}