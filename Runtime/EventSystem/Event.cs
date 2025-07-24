namespace UniUtils.EventSystem
{
    /// <summary>
    /// Interface for events with a specific event channel.
    /// </summary>
    /// <typeparam name="TChannel">The type of the event channel.</typeparam>
    /// <example>
    /// <code>
    /// Define an event 'PlayerJumpEvent' on the 'PlayerEventChannel' channel
    ///
    /// public class PlayerJumpEvent : IEvent&lt;PlayerEventChannel&gt;
    /// {
    ///     public float jumpStrength;
    /// }
    ///</code>
    /// </example>
    public interface IEvent<TChannel> where TChannel : EventChannel<TChannel>
    {
    }
}