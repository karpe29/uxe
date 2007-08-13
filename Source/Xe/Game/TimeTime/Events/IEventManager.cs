
using System;
using System.Collections.Generic;
using System.Text;

namespace TsunamiGL.Xna.Components
{
    public interface IEventManager
    {
        /// <summary>
        /// Update frequency for Scheduled events specified in times per second. Default 10, min 1; max 100.
        /// </summary>
        float ScheduledEventsUpdateFrequency { get; set; }

        /// <summary>
        /// Register a class as event listener so it can receive events.
        /// </summary>
        /// <param name="listener">object that implements a IEventListener interface.</param>
        /// <returns>An IEventFilter that can be used to register events for the listener class.</returns>
        IEventFilter RegisterEventListener(IEventListener listener);

        /// <summary>
        /// Creates a scheduled event
        /// </summary>
        /// <param name="eventEnum">Enum name to be used as event name.</param>
        /// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
        /// <returns>Unique identifier for the created event.</returns>
        /// <remarks>Time is measured in game time.</remarks>
        string ScheduleEvent(Enum eventEnum, float delayInSeconds);

        /// <summary>
        /// Creates a scheduled event
        /// </summary>
        /// <param name="eventName">Name of the event to schedule.</param>
        /// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
        /// <returns>Unique identifier for the created event.</returns>
        /// <remarks>Time is measured in game time.</remarks>
        string ScheduleEvent(string eventName, float delayInSeconds);

        /// <summary>
        /// Creates a scheduled repeating event
        /// </summary>
        /// <param name="eventEnum">Enum name to be used as event name.</param>
        /// <param name="delayInSeconds">Number of seconds before each repeating trigger of the event</param>
        /// <returns>Unique identifier for the created event.</returns>
        /// <remarks>Time is measured in game time.</remarks>
        string ScheduleRepeatingEvent(Enum eventEnum, float delayInSeconds);

        /// <summary>
        /// Creates a scheduled repeating event
        /// </summary>
        /// <param name="eventName">Name of the event to schedule.</param>
        /// <param name="delayInSeconds">Number of seconds before each repeating trigger of the event</param>
        /// <returns>Unique identifier for the created event.</returns>
        /// <remarks>Time is measured in game time.</remarks>
        string ScheduleRepeatingEvent(string eventName, float delayInSeconds);

        /// <summary>
        /// Creates a scheduled event based on real time (wall-clock)
        /// </summary>
        /// <param name="eventEnum">Enum name to be used as event name.</param>
        /// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
        /// <returns>Unique identifier for the created event.</returns>
        /// <remarks>Time is measured in real time (wall-clock).</remarks>
        string ScheduleRealTimeEvent(Enum eventEnum, float delayInSeconds);

        /// <summary>
        /// Creates a scheduled event based on real time (wall-clock)
        /// </summary>
        /// <param name="eventName">Name of the event to schedule.</param>
        /// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
        /// <returns>Unique identifier for the created event.</returns>
        /// <remarks>Time is measured in real time (wall-clock).</remarks>
        string ScheduleRealTimeEvent(string eventName, float delayInSeconds);

        /// <summary>
        /// Creates a scheduled repeating event based on real time (wall-clock)
        /// </summary>
        /// <param name="eventEnum">Enum name to be used as event name.</param>
        /// <param name="delayInSeconds">Number of seconds before each repeating trigger of the event</param>
        /// <returns>Unique identifier for the created event.</returns>
        /// <remarks>Time is measured in real time (wall-clock).</remarks>
        string ScheduleRealTimeRepeatingEvent(Enum eventEnum, float delayInSeconds);

        /// <summary>
        /// Creates a scheduled repeating event based on real time (wall-clock)
        /// </summary>
        /// <param name="eventName">Name of the event to schedule.</param>
        /// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
        /// <returns>Unique identifier for the created event.</returns>
        /// <remarks>Time is measured in real time (wall-clock).</remarks>
        string ScheduleRealTimeRepeatingEvent(string eventName, float delayInSeconds);

        /// <summary>
        /// Creates a scheduled event
        /// </summary>
        /// <param name="eventEnum">Enum name to be used as event name.</param>
        /// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
        /// <param name="args">An EventArgs that contains the event data.</param>
        /// <returns>Unique identifier for the created event.</returns>
        /// <remarks>Time is measured in game time.</remarks>
        string ScheduleEvent(Enum eventEnum, float delayInSeconds, EventArgs args);

        /// <summary>
        /// Creates a scheduled event
        /// </summary>
        /// <param name="eventName">Name of the event to schedule.</param>
        /// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
        /// <param name="args">An EventArgs that contains the event data.</param>
        /// <returns>Unique identifier for the created event.</returns>
        /// <remarks>Time is measured in game time.</remarks>
        string ScheduleEvent(string eventName, float delayInSeconds, EventArgs args);

        /// <summary>
        /// Creates a scheduled repeating event
        /// </summary>
        /// <param name="eventEnum">Enum name to be used as event name.</param>
        /// <param name="delayInSeconds">Number of seconds before each repeating trigger of the event</param>
        /// <param name="args">An EventArgs that contains the event data.</param>
        /// <returns>Unique identifier for the created event.</returns>
        /// <remarks>Time is measured in game time.</remarks>
        string ScheduleRepeatingEvent(Enum eventEnum, float delayInSeconds, EventArgs args);

        /// <summary>
        /// Creates a scheduled repeating event
        /// </summary>
        /// <param name="eventName">Name of the event to schedule.</param>
        /// <param name="delayInSeconds">Number of seconds before each repeating trigger of the event</param>
        /// <param name="args">An EventArgs that contains the event data.</param>
        /// <returns>Unique identifier for the created event.</returns>
        /// <remarks>Time is measured in game time.</remarks>
        string ScheduleRepeatingEvent(string eventName, float delayInSeconds, EventArgs args);

        /// <summary>
        /// Creates a scheduled event based on real time (wall-clock)
        /// </summary>
        /// <param name="eventEnum">Enum name to be used as event name.</param>
        /// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
        /// <param name="args">An EventArgs that contains the event data.</param>
        /// <returns>Unique identifier for the created event.</returns>
        /// <remarks>Time is measured in real time (wall-clock).</remarks>
        string ScheduleRealTimeEvent(Enum eventEnum, float delayInSeconds, EventArgs args);

        /// <summary>
        /// Creates a scheduled event based on real time (wall-clock)
        /// </summary>
        /// <param name="eventName">Name of the event to schedule.</param>
        /// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
        /// <param name="args">An EventArgs that contains the event data.</param>
        /// <returns>Unique identifier for the created event.</returns>
        /// <remarks>Time is measured in real time (wall-clock).</remarks>
        string ScheduleRealTimeEvent(string eventName, float delayInSeconds, EventArgs args);

        /// <summary>
        /// Creates a scheduled repeating event based on real time (wall-clock)
        /// </summary>
        /// <param name="eventEnum">Enum name to be used as event name.</param>
        /// <param name="delayInSeconds">Number of seconds before each repeating trigger of the event</param>
        /// <param name="args">An EventArgs that contains the event data.</param>
        /// <returns>Unique identifier for the created event.</returns>
        /// <remarks>Time is measured in real time (wall-clock).</remarks>
        string ScheduleRealTimeRepeatingEvent(Enum eventEnum, float delayInSeconds, EventArgs args);

        /// <summary>
        /// Creates a scheduled repeating event based on real time (wall-clock)
        /// </summary>
        /// <param name="eventName">Name of the event to schedule.</param>
        /// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
        /// <param name="args">An EventArgs that contains the event data.</param>
        /// <returns>Unique identifier for the created event.</returns>
        /// <remarks>Time is measured in real time (wall-clock).</remarks>
        string ScheduleRealTimeRepeatingEvent(string eventName, float delayInSeconds, EventArgs args);

        /// <summary>
        /// Creates a scheduled event
        /// </summary>
        /// <param name="method">Method to call when the event is triggered</param>
        /// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
        /// <param name="args">The argument of type EventArgs that contains the event data.</param>
        /// <returns>Unique identifier for the created event.</returns>
        /// <remarks>Time is measured in game time.</remarks>
        string ScheduleEvent(CallbackMethod method, float delayInSeconds, EventArgs args);

        /// <summary>
        /// Creates a scheduled repeating event
        /// </summary>
        /// <param name="method">Method to call when the event is triggered</param>
        /// <param name="delayInSeconds">Number of seconds before each repeating trigger of the event</param>
        /// <param name="args">An EventArgs that contains the event data.</param>
        /// <returns>Unique identifier for the created event.</returns>
        /// <remarks>Time is measured in game time.</remarks>
        string ScheduleRepeatingEvent(CallbackMethod method, float delayInSeconds, EventArgs args);

        /// <summary>
        /// Creates a scheduled event based on real time (wall-clock)
        /// </summary>
        /// <param name="method">Method to call when the event is triggered</param>
        /// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
        /// <param name="args">An EventArgs that contains the event data.</param>
        /// <returns>Unique identifier for the created event.</returns>
        /// <remarks>Time is measured in real time (wall-clock).</remarks>
        string ScheduleRealTimeEvent(CallbackMethod method, float delayInSeconds, EventArgs args);

        /// <summary>
        /// Creates a scheduled repeating event based on real time (wall-clock)
        /// </summary>
        /// <param name="method">Method to call when the event is triggered</param>
        /// <param name="delayInSeconds">Number of seconds before each repeating trigger of the event</param>
        /// <param name="args">An EventArgs that contains the event data.</param>
        /// <returns>Unique identifier for the created event.</returns>
        /// <remarks>Time is measured in real time (wall-clock).</remarks>
        string ScheduleRealTimeRepeatingEvent(CallbackMethod method, float delayInSeconds, EventArgs args);

        /// <summary>
        /// Creates a scheduled event
        /// </summary>
        /// <param name="id">Unique identifier for this event.</param>
        /// <param name="method">Method to call when the event is triggered</param>
        /// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
        /// <param name="args">An EventArgs that contains the event data.</param>
        /// <remarks>Time is measured in game time.</remarks>
        void ScheduleEvent(string id, CallbackMethod method, float delayInSeconds, EventArgs args);

        /// <summary>
        /// Creates a scheduled repeating event
        /// </summary>
        /// <param name="id">Unique identifier for this event.</param>
        /// <param name="method">Method to call when the event is triggered</param>
        /// <param name="delayInSeconds">Number of seconds before each repeating trigger of the event</param>
        /// <param name="args">An EventArgs that contains the event data.</param>
        /// <remarks>Time is measured in game time.</remarks>
        void ScheduleRepeatingEvent(string id, CallbackMethod method, float delayInSeconds, EventArgs args);

        /// <summary>
        /// Creates a scheduled event based on real time (wall-clock)
        /// </summary>
        /// <param name="id">Unique identifier for this event.</param>
        /// <param name="method">Method to call when the event is triggered</param>
        /// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
        /// <param name="args">An EventArgs that contains the event data.</param>
        /// <remarks>Time is measured in real time (wall-clock).</remarks>
        void ScheduleRealTimeEvent(string id, CallbackMethod method, float delayInSeconds, EventArgs args);

        /// <summary>
        /// Creates a scheduled repeating event based on real time (wall-clock)
        /// </summary>
        /// <param name="id">Unique identifier for this event.</param>
        /// <param name="method">Method to call when the event is triggered</param>
        /// <param name="delayInSeconds">Number of seconds before each repeating trigger of the event</param>
        /// <param name="args">An EventArgs that contains the event data.</param>
        /// <remarks>Time is measured in real time (wall-clock).</remarks>
        void ScheduleRealTimeRepeatingEvent(string id, CallbackMethod method, float delayInSeconds, EventArgs args);

        /// <summary>
        /// Removes a scheduled event
        /// </summary>
        /// <param name="id">Unique identifier of the event.</param>
        void RemoveScheduledEvent(string id);

        /// <summary>
        /// Checks if an event is scheduled
        /// </summary>
        /// <param name="id">Unique identifier of the event.</param>
        /// <returns>True if event is active; otherwise false</returns>
        bool IsEventScheduled(string id);

        /// <summary>
        /// Returns the time left untill the scheduled event triggers.
        /// </summary>
        /// <param name="id">Unique identifier of the event.</param>
        /// <returns>Number of seconds until event triggers. -1 if the event wasn't found.</returns>
        float TimeLeftOnScheduledEvent(string id);

        /// <summary>
        /// Changes the delay time before the event is triggered.
        /// </summary>
        /// <param name="id">Unique identifier of the event.</param>
        /// <param name="newDelayInSeconds">New number of seconds until the event is triggered.</param>
        void ChangeEventDelay(string id, float newDelayInSeconds);

        /// <summary>
        /// Triggers an event
        /// </summary>
        /// <param name="eventEnum">Enum to be used as event name.</param>
        void TriggerEvent(Enum eventEnum);

        /// <summary>
        /// Triggers an event
        /// </summary>
        /// <param name="eventName">Name of the event to trigger.</param>
        void TriggerEvent(string eventName);

        /// <summary>
        /// Triggers an event
        /// </summary>
        /// <param name="eventEnum">Enum to be used as event name.</param>
        /// <param name="args">An EventArgs that contains the event data.</param>
        void TriggerEvent(Enum eventEnum, EventArgs args);

        /// <summary>
        /// Triggers an event
        /// </summary>
        /// <param name="eventName">Name of the event to trigger.</param>
        /// <param name="args">An EventArgs that contains the event data.</param>
        void TriggerEvent(string eventName, EventArgs args);

        /// <summary>
        /// Override a scheduled events timer and trigger it on the next update sweep.
        /// </summary>
        /// <param name="id">Unique identifier of the event.</param>
        void TriggerScheduledEvent(string id);
    }
}