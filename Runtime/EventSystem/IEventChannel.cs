namespace UniUtils.EventSystem
{
    /// <summary>
    /// Interface representing an event channel.
    /// </summary>
    public interface IEventChannel
    {
        public void UnsubscribeAll();
    }
}