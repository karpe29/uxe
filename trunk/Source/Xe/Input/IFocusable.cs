using System;
using System.Collections.Generic;

namespace Xe.Input
{
    public interface IFocusable
    {
        /// <summary>
        /// Sets focus on the object.
        /// </summary>
        bool Focus();

        /// <summary>
        /// Removes focus from the object.
        /// </summary>
        void UnFocus();

        bool TabNext();
        bool TabPrev();

        /// <summary>
        /// Called when focus is recieved.
        /// </summary>
        event GotFocusHandler GotFocus;

        /// <summary>
        /// Called when focus is lost.
        /// </summary>
        event LostFocusHandler LostFocus;

        /// <summary>
        /// Whether or not the object can be tabbed to.
        /// </summary>
        bool IsTabable { get; set; }

        /// <summary>
        /// Gets or Sets the Tab Order of the object.
        /// </summary>
        int TabOrder { get; set; }
    }

    public delegate void GotFocusHandler(object sender);
    public delegate void LostFocusHandler(object sender);
}
