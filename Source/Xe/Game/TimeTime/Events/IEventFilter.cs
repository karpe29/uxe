using System;
using System.Collections.Generic;
using System.Text;

namespace Xe.TimeTime.Events
{
    /// <summary>
    /// Interface for event filters, used to manage event
    /// subscriptions for IEventListener classes.
    /// </summary>
    public interface IEventFilter
    {
        /// <summary>
        /// Registers an event that this filter will subscribe to
        /// </summary>
        /// <param name="eventName">Enum that will be converted into a string as event name.</param>
        void Subscribe(Enum eventName);

        /// <summary>
        /// Registers an event that this filter will subscribe to
        /// </summary>
        /// <param name="eventName">Name of the event to subscribe to.</param>
        void Subscribe(string eventName);

        /// <summary>
        /// Registers an event that this filter will subscribe to
        /// </summary>
        /// <param name="eventName">Enum that will be converted into a string as event name.</param>
        /// <param name="throttle">Maximum number of seconds that must pass between event triggers.</param>
        void Subscribe(Enum eventName, float throttle);

        /// <summary>
        /// Registers an event that this filter will subscribe to
        /// </summary>
        /// <param name="eventName">Name of the event to subscribe to.</param>
        /// <param name="throttle">Maximum number of seconds that must pass between event triggers.</param>
        void Subscribe(string eventName, float throttle);

        /// <summary>
        /// Unregisters an event this filter is subscribing to
        /// </summary>
        /// <param name="eventName">Enum that will be converted into a string as event name.</param>
        void Unsubscribe(Enum eventName);

        /// <summary>
        /// Unregisters an event this filter is subscribing to
        /// </summary>
        /// <param name="eventName">Name of the event to unsubscribe.</param>
        void Unsubscribe(string eventName);

        /// <summary>
        /// Unregisters all events this filter is subscribing to
        /// </summary>
        /// <param name="eventName">Name of the event to unsubscribe.</param>
        void UnsubscribeAll();
    }
}