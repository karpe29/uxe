using System;

namespace TsunamiGL.Xna.Components
{
    /// <summary>
    /// Interface all event listener classes must implement.
    /// </summary>
    public interface IEventListener
    {
        /// <summary>
        /// Called from the EventManager when a registred event occurs.
        /// </summary>
        /// <param name="eventName">Name of the event. (the name is always in upper case).</param>
        /// <param name="args">EventArgs for the event.</param>
        void OnEvent(string eventName, EventArgs args);
    }
}