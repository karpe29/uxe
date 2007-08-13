
using System;
using System.Collections.Generic;

namespace TsunamiGL.Xna.Components
{
	public partial class EventManager
	{
		/// <summary>
		/// Helper class for event filters
		/// </summary>
		private class EventFilter : IEventFilter
		{
			/// <summary>
			/// Helper class for event subscriptions
			/// </summary>
			private class EventSubscriptionInfo
			{
				public float ThrottleDelay;
				public float ThrottleTriggerTime;

				public EventSubscriptionInfo( float throttleDelay )
				{
					ThrottleDelay = throttleDelay;
				}
			}

			// Thread-safe non-compiler optimized static field used for ID counting.
			private static volatile int _idCounter = 0;

			public readonly int ID = _idCounter++;
			Dictionary<string, Event> _events;
			WeakReference _listener = new WeakReference( null );
			Dictionary<string, EventSubscriptionInfo> _eventSubscriptions = new Dictionary<string, EventSubscriptionInfo>();

            public WeakReference Listener
            {
                get { return _listener; }
            }

			/// <summary>
			/// Returns true if the instance object for this filter is still alive; otherwise false.
			/// </summary>
			public bool IsAlive
			{
				get { return _listener.IsAlive; }
			}

			/// <summary>
			/// Constructor
			/// </summary>
			public EventFilter( Dictionary<string, Event> eventsList, IEventListener listener )
			{
				_events = eventsList;
				_listener.Target = listener;
			}

			/// <summary>
			/// Registers an event that this filter will subscribe to
			/// </summary>
			/// <param name="eventName">Enum that will be converted into a string as event name.</param>
			public void Subscribe( Enum eventName )
			{
				Subscribe( eventName, 0.0f );
			}

			/// <summary>
			/// Registers an event that this filter will subscribe to
			/// </summary>
			/// <param name="eventName">Name of the event to subscribe to.</param>
			public void Subscribe( string eventName )
			{
				Subscribe( eventName, 0.0f );
			}

			/// <summary>
			/// Registers an event that this filter will subscribe to
			/// </summary>
			/// <param name="eventName">Enum that will be converted into a string as event name.</param>
			/// <param name="throttle">Maximum number of seconds that must pass between event triggers.</param>
			public void Subscribe( Enum eventName, float throttle )
			{
				string name = eventName.ToString();
				Subscribe( name, throttle );
			}

			/// <summary>
			/// Registers an event that this filter will subscribe to
			/// </summary>
			/// <param name="eventName">Name of the event to subscribe to.</param>
			/// <param name="throttle">Maximum number of seconds that must pass between event triggers.</param>
			public void Subscribe( string eventName, float throttle )
			{
				eventName = eventName.ToUpper();

				if( _events.ContainsKey( eventName ) == false )
					_events.Add( eventName, new Event( eventName ) );

				_events[eventName].AddFilter( this );


				if( _eventSubscriptions.ContainsKey( eventName ) == false )
					_eventSubscriptions.Add( eventName, new EventSubscriptionInfo( throttle ) );
			}

			/// <summary>
			/// Unregisters an event this filter is subscribing to
			/// </summary>
			/// <param name="eventName">Enum that will be converted into a string as event name.</param>
			public void Unsubscribe( Enum eventName )
			{
				string name = eventName.ToString();
				Unsubscribe( name );
			}

			/// <summary>
			/// Unregisters an event this filter is subscribing to
			/// </summary>
			/// <param name="eventName">Name of the event to unsubscribe.</param>
			public void Unsubscribe( string eventName )
			{
				if( _events.ContainsKey( eventName ) )
					_events[eventName].RemoveFilter( this );

				_eventSubscriptions.Remove( eventName );
			}

			/// <summary>
			/// Unregisters all events this filter is subscribing to
			/// </summary>
			/// <param name="eventName">Name of the event to unsubscribe.</param>
			public void UnsubscribeAll()
			{
				foreach( string name in _eventSubscriptions.Keys )
					if( _events.ContainsKey( name ) )
						_events[name].RemoveFilter( this );

				_eventSubscriptions.Clear();
			}

			/// <summary>
			/// Invokes the OnEvent method on the IEventListener this filter
			/// belongs to for all subscribed events.
			/// </summary>
			public void Invoke( string eventName, float currentTime, EventArgs args )
			{
				// Make sure the listener object is alive.
				IEventListener listener = _listener.Target as IEventListener;
				if( listener == null )
				{
					UnsubscribeAll();
					_events = null;
					return;
				}

				// Look! It's moving. It's alive. It's alive... It's alive, it's moving, 
				// it's alive, it's alive, it's alive, it's alive, IT'S ALIVE!
				if( _eventSubscriptions.ContainsKey( eventName ) )
				{
					// Get all events we're subscribing to and measure against the throttle time.
					EventSubscriptionInfo info = _eventSubscriptions[eventName];
					if( currentTime >= info.ThrottleTriggerTime )
					{
						info.ThrottleTriggerTime = currentTime + info.ThrottleDelay;
						listener.OnEvent( eventName, args );
					}
				}
			}
		}
	}
}