namespace UniUtils.FSM
{
    /// <summary>
    /// Represents a state in the state machine.
    /// </summary>
    /// <example>
    /// Example implementation of a state:
    /// <code>
    /// public class IdleState : IState
    /// {
    ///     public void Enter()
    ///     {
    ///         Debug.Log("Entering Idle State");
    ///     }
    ///
    ///     public void Update()
    ///     {
    ///         // Idle behavior
    ///     }
    ///
    ///     public void FixedUpdate()
    ///     {
    ///         // Physics-related idle behavior
    ///     }
    ///
    ///     public void Exit()
    ///     {
    ///         Debug.Log("Exiting Idle State");
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface IState
    {
        /// <summary>
        /// Called when the state is entered.
        /// </summary>
        public void Enter();

        /// <summary>
        /// Called every frame to update the state.
        /// </summary>
        public void Update();

        /// <summary>
        /// Called at fixed intervals to update the state.
        /// </summary>
        public void FixedUpdate();

        /// <summary>
        /// Called when the state is exited.
        /// </summary>
        public void Exit();
    }
}