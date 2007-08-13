
using System;
using System.Collections.Generic;
using System.Reflection;

namespace TsunamiGL.Xna.Components
{
    public partial class EventManager
    {
        /// <summary>
        /// Helper class for scheduled events
        /// </summary>
        private sealed class ScheduledEventInfo
        {
            public string ID;
            public float DelayInSeconds;
            public float TriggerTime;
            public float LastUpdateTime;
            public bool RealTime;
            public WeakReference Instance = new WeakReference(null);
            public string EventName;
            public EventArgs Args;
            public bool Repeating;
            private MethodInfo _method;

            /// <summary>
            /// Set callback target
            /// </summary>
            public CallbackMethod Target
            {
                set
                {
                    if (value == null)
                    {
                        Instance = null;
                        _method = null;
                        return;
                    }

                    Instance.Target = value.Target;
                    _method = value.Method;
                }
            }

            /// <summary>
            /// Invokes the stored method if target is still alive.
            /// </summary>
            public void Invoke()
            {
                // Make strong reference before invoking.
                object o = Instance.Target;
                if (o != null)
                {
                    // Look! It's moving. It's alive. It's alive... It's alive, it's moving, 
                    // it's alive, it's alive, it's alive, it's alive, IT'S ALIVE!
                    _method.Invoke(o, new object[] { Args });
                }

                // Otherwise; target is dead and long gone, so we do nothing.
            }
        }
    }
}