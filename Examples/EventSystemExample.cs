using System;
using UnityEngine;
using UnityEngine.Scripting;
using UniUtils.EventSystem;
using UniUtils.Extensions;

namespace UniUtils.Examples
{
    /// <summary>
    /// Example demonstrating how to publish and subscribe to a custom input event
    /// using a simple event system. It listens for any key press and publishes
    /// an event containing the pressed key's string representation.
    /// </summary>
    /// <remarks>Make sure to add the EventManager MonoBehaviour in your scene for it to work.</remarks>
    public class EventSystemExample : MonoBehaviour
    {
        // This handle ID is used to unsubscribe from the event when this component is destroyed.
        private int receiveInputEventHandleId;

        private void Awake()
        {
            // Subscribe to the ReceivedAnyInputEvent event of the input event channel and log the received input string.
            receiveInputEventHandleId = EventManager.Subscribe<InputEventChannel, ReceivedAnyInputEvent>(e =>
            {
                this.Log($"Received input event: {e.input}");
            });
        }

        private void OnDestroy()
        {
            // Unsubscribe from the event to prevent memory leaks.
            EventManager.Unsubscribe<InputEventChannel, ReceivedAnyInputEvent>(receiveInputEventHandleId);
        }

        private void Update()
        {
            // On any key down, publish a ReceivedAnyInputEvent with the input string on the InputEventChannel.
            if (Input.anyKeyDown)
            {
                EventManager.Publish<InputEventChannel, ReceivedAnyInputEvent>(
                    new ReceivedAnyInputEvent { input = Input.inputString }
                );
            }
        }
    }

    /// <summary>
    /// Event data struct containing the input string detected on any key press.
    /// </summary>
    public struct ReceivedAnyInputEvent : IEvent<InputEventChannel>
    {
        /// <summary>
        /// The string representing the input key(s) pressed.
        /// </summary>
        public string input;
    }

    /// <summary>
    /// Event channel used to route ReceivedAnyInputEvent events.
    /// </summary>
    /// <remarks>[Preserve] has to be added to all Event channels in order to prevent
    /// Unity's IL2CPP from stripping them out during build time.</remarks>
    [Preserve]
    public class InputEventChannel : EventChannel<InputEventChannel> {}
}