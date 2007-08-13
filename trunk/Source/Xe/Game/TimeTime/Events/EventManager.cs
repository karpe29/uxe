
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace TsunamiGL.Xna.Components
{
	public delegate void CallbackMethod( EventArgs args );

	/// <summary>
	/// Handles various type of events.
	/// </summary>
	public partial class EventManager : GameComponent, IEventManager
	{
		/// <summary>
		/// Helper struct to hold information about a triggering event.
		/// </summary>
		private struct TriggerInfo
		{
			public string eventName;
			public EventArgs Args;
			public TriggerInfo( string eventName, EventArgs args )
			{
				this.eventName = eventName;
				this.Args = args;
			}
		}

		float _currentRealTime;
		float _currentGameTime;

		float _scheduledEventsUpdateDelay;
		float _scheduledEventsUpdateTimer;
		int _nextAvailableEventID;

		Dictionary<string, ScheduledEventInfo> _scheduledEvents = new Dictionary<string, ScheduledEventInfo>();
		Queue<ScheduledEventInfo> _scheduledEventsToUpdate = new Queue<ScheduledEventInfo>();

		Dictionary<string, Event> _events = new Dictionary<string, Event>();
		List<EventFilter> _filters = new List<EventFilter>();
		int _filterTraverser = 0;

		Queue<TriggerInfo> _triggeredEvents = new Queue<TriggerInfo>();

		/// <summary>
		/// Update frequency for Scheduled events specified in times per second. Default 10, min 1; max 100.
		/// </summary>
		public float ScheduledEventsUpdateFrequency
		{
			get { return 1 / _scheduledEventsUpdateDelay; }
			set { _scheduledEventsUpdateDelay = 1 / MathHelper.Clamp( value, 1, 100 ); }
		}

		/// <summary>
		/// Constructor
		/// </summary>
		public EventManager( Game game )
			: base( game )
		{
			game.Services.AddService( typeof( IEventManager ), this );
			ScheduledEventsUpdateFrequency = 10;
		}

		/// <summary>
		/// Register a class as event listener so it can receive events.
		/// </summary>
		/// <param name="listener">object that implements a IEventListener interface.</param>
		/// <returns>An IEventFilter that can be used to register events for the listener class.</returns>
		public IEventFilter RegisterEventListener( IEventListener listener )
		{
			if( listener == null )
				throw new ArgumentException( "IEventListener can not be null." );

			if( typeof( IEventListener ).IsAssignableFrom( listener.GetType() ) == false )
				throw new ArgumentException( "IEventListener must be assignable to listener object" );

			EventFilter filter = new EventFilter( _events, listener );
			_filters.Add( filter );
			return filter;
		}

        /// <summary>
        /// UnRegister a class from an event listener so it no longer receives events.
        /// </summary>
        /// <param name="listener">object that implements a IEventListener interface.</param>
        public void UnRegisterEventListener(IEventListener listener)
        {
            if (listener == null)
                throw new ArgumentException("IEventListener can not be null.");

            if (typeof(IEventListener).IsAssignableFrom(listener.GetType()) == false)
                throw new ArgumentException("IEventListener must be assignable to listener object");


            int i = _filters.Count;
            while (--i >= 0)
            {
                EventFilter f = _filters[i];
                if (f.Listener == listener)
                {
                    _filters.RemoveAt(i);
                }
            }
            
        }

		/// <summary>
		/// Creates a scheduled event
		/// </summary>
		/// <param name="eventEnum">Enum name to be used as event name.</param>
		/// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
		/// <returns>Unique identifier for the created event.</returns>
		/// <remarks>Time is measured in game time.</remarks>
		public string ScheduleEvent( Enum eventEnum, float delayInSeconds )
		{
			return CreateScheduleEvent( null, null, delayInSeconds, false, false, eventEnum.ToString(), EventArgs.Empty );
		}

		/// <summary>
		/// Creates a scheduled event
		/// </summary>
		/// <param name="eventName">Name of the event to schedule.</param>
		/// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
		/// <returns>Unique identifier for the created event.</returns>
		/// <remarks>Time is measured in game time.</remarks>
		public string ScheduleEvent( string eventName, float delayInSeconds )
		{
			return CreateScheduleEvent( null, null, delayInSeconds, false, false, eventName, EventArgs.Empty );
		}

		/// <summary>
		/// Creates a scheduled repeating event
		/// </summary>
		/// <param name="eventEnum">Enum name to be used as event name.</param>
		/// <param name="delayInSeconds">Number of seconds before each repeating trigger of the event</param>
		/// <returns>Unique identifier for the created event.</returns>
		/// <remarks>Time is measured in game time.</remarks>
		public string ScheduleRepeatingEvent( Enum eventEnum, float delayInSeconds )
		{
			return CreateScheduleEvent( null, null, delayInSeconds, true, false, eventEnum.ToString(), EventArgs.Empty );
		}

		/// <summary>
		/// Creates a scheduled repeating event
		/// </summary>
		/// <param name="eventName">Name of the event to schedule.</param>
		/// <param name="delayInSeconds">Number of seconds before each repeating trigger of the event</param>
		/// <returns>Unique identifier for the created event.</returns>
		/// <remarks>Time is measured in game time.</remarks>
		public string ScheduleRepeatingEvent( string eventName, float delayInSeconds )
		{
			return CreateScheduleEvent( null, null, delayInSeconds, true, false, eventName, EventArgs.Empty );
		}

		/// <summary>
		/// Creates a scheduled event based on real time (wall-clock)
		/// </summary>
		/// <param name="eventEnum">Enum name to be used as event name.</param>
		/// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
		/// <returns>Unique identifier for the created event.</returns>
		/// <remarks>Time is measured in real time (wall-clock).</remarks>
		public string ScheduleRealTimeEvent( Enum eventEnum, float delayInSeconds )
		{
			return CreateScheduleEvent( null, null, delayInSeconds, false, true, eventEnum.ToString(), EventArgs.Empty );
		}

		/// <summary>
		/// Creates a scheduled event based on real time (wall-clock)
		/// </summary>
		/// <param name="eventName">Name of the event to schedule.</param>
		/// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
		/// <returns>Unique identifier for the created event.</returns>
		/// <remarks>Time is measured in real time (wall-clock).</remarks>
		public string ScheduleRealTimeEvent( string eventName, float delayInSeconds )
		{
			return CreateScheduleEvent( null, null, delayInSeconds, false, true, eventName, EventArgs.Empty );
		}

		/// <summary>
		/// Creates a scheduled repeating event based on real time (wall-clock)
		/// </summary>
		/// <param name="eventEnum">Enum name to be used as event name.</param>
		/// <param name="delayInSeconds">Number of seconds before each repeating trigger of the event</param>
		/// <returns>Unique identifier for the created event.</returns>
		/// <remarks>Time is measured in real time (wall-clock).</remarks>
		public string ScheduleRealTimeRepeatingEvent( Enum eventEnum, float delayInSeconds )
		{
			return CreateScheduleEvent( null, null, delayInSeconds, true, true, eventEnum.ToString(), EventArgs.Empty );
		}

		/// <summary>
		/// Creates a scheduled repeating event based on real time (wall-clock)
		/// </summary>
		/// <param name="eventName">Name of the event to schedule.</param>
		/// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
		/// <returns>Unique identifier for the created event.</returns>
		/// <remarks>Time is measured in real time (wall-clock).</remarks>
		public string ScheduleRealTimeRepeatingEvent( string eventName, float delayInSeconds )
		{
			return CreateScheduleEvent( null, null, delayInSeconds, true, true, eventName, EventArgs.Empty );
		}

		/// <summary>
		/// Creates a scheduled event
		/// </summary>
		/// <param name="eventEnum">Enum name to be used as event name.</param>
		/// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
		/// <param name="args">An EventArgs that contains the event data.</param>
		/// <returns>Unique identifier for the created event.</returns>
		/// <remarks>Time is measured in game time.</remarks>
		public string ScheduleEvent( Enum eventEnum, float delayInSeconds, EventArgs args )
		{
			return CreateScheduleEvent( null, null, delayInSeconds, false, false, eventEnum.ToString(), args );
		}

		/// <summary>
		/// Creates a scheduled event
		/// </summary>
		/// <param name="eventName">Name of the event to schedule.</param>
		/// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
		/// <param name="args">An EventArgs that contains the event data.</param>
		/// <returns>Unique identifier for the created event.</returns>
		/// <remarks>Time is measured in game time.</remarks>
		public string ScheduleEvent( string eventName, float delayInSeconds, EventArgs args )
		{
			return CreateScheduleEvent( null, null, delayInSeconds, false, false, eventName, args );
		}

		/// <summary>
		/// Creates a scheduled repeating event
		/// </summary>
		/// <param name="eventEnum">Enum name to be used as event name.</param>
		/// <param name="delayInSeconds">Number of seconds before each repeating trigger of the event</param>
		/// <param name="args">An EventArgs that contains the event data.</param>
		/// <returns>Unique identifier for the created event.</returns>
		/// <remarks>Time is measured in game time.</remarks>
		public string ScheduleRepeatingEvent( Enum eventEnum, float delayInSeconds, EventArgs args )
		{
			return CreateScheduleEvent( null, null, delayInSeconds, true, false, eventEnum.ToString(), args );
		}

		/// <summary>
		/// Creates a scheduled repeating event
		/// </summary>
		/// <param name="eventName">Name of the event to schedule.</param>
		/// <param name="delayInSeconds">Number of seconds before each repeating trigger of the event</param>
		/// <param name="args">An EventArgs that contains the event data.</param>
		/// <returns>Unique identifier for the created event.</returns>
		/// <remarks>Time is measured in game time.</remarks>
		public string ScheduleRepeatingEvent( string eventName, float delayInSeconds, EventArgs args )
		{
			return CreateScheduleEvent( null, null, delayInSeconds, true, false, eventName, args );
		}

		/// <summary>
		/// Creates a scheduled event based on real time (wall-clock)
		/// </summary>
		/// <param name="eventEnum">Enum name to be used as event name.</param>
		/// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
		/// <param name="args">An EventArgs that contains the event data.</param>
		/// <returns>Unique identifier for the created event.</returns>
		/// <remarks>Time is measured in real time (wall-clock).</remarks>
		public string ScheduleRealTimeEvent( Enum eventEnum, float delayInSeconds, EventArgs args )
		{
			return CreateScheduleEvent( null, null, delayInSeconds, false, true, eventEnum.ToString(), args );
		}

		/// <summary>
		/// Creates a scheduled event based on real time (wall-clock)
		/// </summary>
		/// <param name="eventName">Name of the event to schedule.</param>
		/// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
		/// <param name="args">An EventArgs that contains the event data.</param>
		/// <returns>Unique identifier for the created event.</returns>
		/// <remarks>Time is measured in real time (wall-clock).</remarks>
		public string ScheduleRealTimeEvent( string eventName, float delayInSeconds, EventArgs args )
		{
			return CreateScheduleEvent( null, null, delayInSeconds, false, true, eventName, args );
		}

		/// <summary>
		/// Creates a scheduled repeating event based on real time (wall-clock)
		/// </summary>
		/// <param name="eventEnum">Enum name to be used as event name.</param>
		/// <param name="delayInSeconds">Number of seconds before each repeating trigger of the event</param>
		/// <param name="args">An EventArgs that contains the event data.</param>
		/// <returns>Unique identifier for the created event.</returns>
		/// <remarks>Time is measured in real time (wall-clock).</remarks>
		public string ScheduleRealTimeRepeatingEvent( Enum eventEnum, float delayInSeconds, EventArgs args )
		{
			return CreateScheduleEvent( null, null, delayInSeconds, true, true, eventEnum.ToString(), args );
		}

		/// <summary>
		/// Creates a scheduled repeating event based on real time (wall-clock)
		/// </summary>
		/// <param name="eventName">Name of the event to schedule.</param>
		/// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
		/// <param name="args">An EventArgs that contains the event data.</param>
		/// <returns>Unique identifier for the created event.</returns>
		/// <remarks>Time is measured in real time (wall-clock).</remarks>
		public string ScheduleRealTimeRepeatingEvent( string eventName, float delayInSeconds, EventArgs args )
		{
			return CreateScheduleEvent( null, null, delayInSeconds, true, true, eventName, args );
		}

		/// <summary>
		/// Creates a scheduled event
		/// </summary>
		/// <param name="method">Method to call when the event is triggered</param>
		/// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
		/// <param name="args">An EventArgs that contains the event data.</param>
		/// <returns>Unique identifier for the created event.</returns>
		/// <remarks>Time is measured in game time.</remarks>
		public string ScheduleEvent( CallbackMethod method, float delayInSeconds, EventArgs args )
		{
			return CreateScheduleEvent( null, method, delayInSeconds, false, false, null, args );
		}

		/// <summary>
		/// Creates a scheduled repeating event
		/// </summary>
		/// <param name="method">Method to call when the event is triggered</param>
		/// <param name="delayInSeconds">Number of seconds before each repeating trigger of the event</param>
		/// <param name="args">An EventArgs that contains the event data.</param>
		/// <returns>Unique identifier for the created event.</returns>
		/// <remarks>Time is measured in game time.</remarks>
		public string ScheduleRepeatingEvent( CallbackMethod method, float delayInSeconds, EventArgs args )
		{
			return CreateScheduleEvent( null, method, delayInSeconds, true, false, null, args );
		}

		/// <summary>
		/// Creates a scheduled event based on real time (wall-clock)
		/// </summary>
		/// <param name="method">Method to call when the event is triggered</param>
		/// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
		/// <param name="args">An EventArgs that contains the event data.</param>
		/// <returns>Unique identifier for the created event.</returns>
		/// <remarks>Time is measured in real time (wall-clock).</remarks>
		public string ScheduleRealTimeEvent( CallbackMethod method, float delayInSeconds, EventArgs args )
		{
			return CreateScheduleEvent( null, method, delayInSeconds, false, true, null, args );
		}

		/// <summary>
		/// Creates a scheduled repeating event based on real time (wall-clock)
		/// </summary>
		/// <param name="method">Method to call when the event is triggered</param>
		/// <param name="delayInSeconds">Number of seconds before each repeating trigger of the event</param>
		/// <param name="args">An EventArgs that contains the event data.</param>
		/// <returns>Unique identifier for the created event.</returns>
		/// <remarks>Time is measured in real time (wall-clock).</remarks>
		public string ScheduleRealTimeRepeatingEvent( CallbackMethod method, float delayInSeconds, EventArgs args )
		{
			return CreateScheduleEvent( null, method, delayInSeconds, true, true, null, args );
		}

		/// <summary>
		/// Creates a scheduled event
		/// </summary>
		/// <param name="id">Unique identifier for this event.</param>
		/// <param name="method">Method to call when the event is triggered</param>
		/// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
		/// <param name="args">An EventArgs that contains the event data.</param>
		/// <remarks>Time is measured in game time.</remarks>
		public void ScheduleEvent( string id, CallbackMethod method, float delayInSeconds, EventArgs args )
		{
			CreateScheduleEvent( id, method, delayInSeconds, false, false, null, args );
		}

		/// <summary>
		/// Creates a scheduled repeating event
		/// </summary>
		/// <param name="id">Unique identifier for this event.</param>
		/// <param name="method">Method to call when the event is triggered</param>
		/// <param name="delayInSeconds">Number of seconds before each repeating trigger of the event</param>
		/// <param name="args">An EventArgs that contains the event data.</param>
		/// <remarks>Time is measured in game time.</remarks>
		public void ScheduleRepeatingEvent( string id, CallbackMethod method, float delayInSeconds, EventArgs args )
		{
			CreateScheduleEvent( id, method, delayInSeconds, true, false, null, args );
		}

		/// <summary>
		/// Creates a scheduled event based on real time (wall-clock)
		/// </summary>
		/// <param name="id">Unique identifier for this event.</param>
		/// <param name="method">Method to call when the event is triggered</param>
		/// <param name="delayInSeconds">Number of seconds before the event is triggered</param>
		/// <param name="args">An EventArgs that contains the event data.</param>
		/// <remarks>Time is measured in real time (wall-clock).</remarks>
		public void ScheduleRealTimeEvent( string id, CallbackMethod method, float delayInSeconds, EventArgs args )
		{
			CreateScheduleEvent( id, method, delayInSeconds, false, true, null, args );
		}

		/// <summary>
		/// Creates a scheduled repeating event based on real time (wall-clock)
		/// </summary>
		/// <param name="id">Unique identifier for this event.</param>
		/// <param name="method">Method to call when the event is triggered</param>
		/// <param name="delayInSeconds">Number of seconds before each repeating trigger of the event</param>
		/// <param name="args">An EventArgs that contains the event data.</param>
		/// <remarks>Time is measured in real time (wall-clock).</remarks>
		public void ScheduleRealTimeRepeatingEvent( string id, CallbackMethod method, float delayInSeconds, EventArgs args )
		{
			CreateScheduleEvent( id, method, delayInSeconds, true, true, null, args );
		}

		/// <summary>
		/// Removes a scheduled event
		/// </summary>
		/// <param name="id">Unique identifier of the event.</param>
		public void RemoveScheduledEvent( string id )
		{
			if( _scheduledEvents.ContainsKey( id ) )
			{
				ScheduledEventInfo info = _scheduledEvents[id];
				info.Target = null;
				info.Args = null;
				info.EventName = null;
				_scheduledEvents.Remove( id );
			}
		}

		/// <summary>
		/// Checks if an event is scheduled
		/// </summary>
		/// <param name="id">Unique identifier of the event.</param>
		/// <returns>True if event is active; otherwise false</returns>
		public bool IsEventScheduled( string id )
		{
			if( _scheduledEvents.ContainsKey( id ) )
				return true;

			return false;
		}

		/// <summary>
		/// Returns the time left untill the scheduled event triggers.
		/// </summary>
		/// <param name="id">Unique identifier of the event.</param>
		/// <returns>Number of seconds until event triggers. -1 if the event wasn't found.</returns>
		public float TimeLeftOnScheduledEvent( string id )
		{
			if( _scheduledEvents.ContainsKey( id ) )
			{
				ScheduledEventInfo info = _scheduledEvents[id];
				return info.TriggerTime - info.LastUpdateTime;
			}
			return -1;
		}

		/// <summary>
		/// Changes the delay time before the event is triggered.
		/// </summary>
		/// <param name="id">Unique identifier of the event.</param>
		/// <param name="newDelayInSeconds">New number of seconds until the event is triggered.</param>
		public void ChangeEventDelay( string id, float newDelayInSeconds )
		{
			if( _scheduledEvents.ContainsKey( id ) )
			{
				ScheduledEventInfo info = _scheduledEvents[id];

				info.TriggerTime -= info.DelayInSeconds - newDelayInSeconds;
				info.DelayInSeconds = newDelayInSeconds;
			}
		}

		/// <summary>
		/// Triggers an event
		/// </summary>
		/// <param name="eventEnum">Enum to be used as event name.</param>
		public void TriggerEvent( Enum eventEnum )
		{
			_triggeredEvents.Enqueue( new TriggerInfo( eventEnum.ToString(), EventArgs.Empty ) );
		}

		/// <summary>
		/// Triggers an event
		/// </summary>
		/// <param name="eventName">Name of the event to trigger.</param>
		public void TriggerEvent( string eventName )
		{
			_triggeredEvents.Enqueue( new TriggerInfo( eventName, EventArgs.Empty ) );
		}

		/// <summary>
		/// Triggers an event
		/// </summary>
		/// <param name="eventEnum">Enum to be used as event name.</param>
		/// <param name="args">An EventArgs that contains the event data.</param>
		public void TriggerEvent( Enum eventEnum, EventArgs args )
		{
			_triggeredEvents.Enqueue( new TriggerInfo( eventEnum.ToString(), args ) );
		}

		/// <summary>
		/// Triggers an event
		/// </summary>
		/// <param name="eventName">Name of the event to trigger.</param>
		/// <param name="args">An EventArgs that contains the event data.</param>
		public void TriggerEvent( string eventName, EventArgs args )
		{
			_triggeredEvents.Enqueue( new TriggerInfo( eventName, args ) );
		}

		/// <summary>
		/// Override a scheduled events timer and trigger it on the next update sweep.
		/// </summary>
		/// <param name="id">Unique identifier of the event.</param>
		public void TriggerScheduledEvent( string id )
		{
			if( _scheduledEvents.ContainsKey( id ) )
			{
				ScheduledEventInfo info = _scheduledEvents[id];
				info.TriggerTime = info.LastUpdateTime;
			}
		}

		/// <summary>
		/// Called from Game
		/// </summary>
		public override void Update( GameTime gameTime )
		{
			_currentRealTime = (float) gameTime.TotalRealTime.TotalSeconds;
			_currentGameTime = (float) gameTime.TotalGameTime.TotalSeconds;

			// Update all scheduled events.
			if( _currentRealTime >= _scheduledEventsUpdateTimer )
			{
				UpdateScheduledEvents();
				_scheduledEventsUpdateTimer = _currentRealTime + _scheduledEventsUpdateDelay;
			}

			// Invoke all enqueued triggers.
			while( _triggeredEvents.Count > 0 )
			{
				TriggerInfo info = _triggeredEvents.Dequeue();
				if( _events.ContainsKey( info.eventName ) )
					_events[info.eventName].Invoke( _currentRealTime, info.Args );
			}

			// Traverse event filters, one each frame to see if
			// there are any dead objects to clean up.
			if( _filters.Count > 0 )
			{
				if( _filterTraverser >= _filters.Count )
					_filterTraverser = 0;

				EventFilter filter = _filters[_filterTraverser++];
				if( filter.IsAlive == false )
				{
					filter.UnsubscribeAll();
					_filters.Remove( filter );
				}
			}
			base.Update( gameTime );
		}

		/// <summary>
		/// Updates all scheduled events
		/// </summary>
		private void UpdateScheduledEvents()
		{
			// update from a copy of the event list incase
			// event methods creates or removes events.
			foreach( ScheduledEventInfo info in _scheduledEvents.Values )
				_scheduledEventsToUpdate.Enqueue( info );

			// Update each event.
			while( _scheduledEventsToUpdate.Count > 0 )
			{
				ScheduledEventInfo info = _scheduledEventsToUpdate.Dequeue();

				info.LastUpdateTime = info.RealTime ? _currentRealTime : _currentGameTime;

				if( info.LastUpdateTime >= info.TriggerTime )
				{
					if( info.EventName != null )
						TriggerEvent( info.EventName, info.Args );
					else
						info.Invoke();

					if( info.Repeating == false )
						RemoveScheduledEvent( info.ID );
					else
						info.TriggerTime = info.LastUpdateTime + info.DelayInSeconds;
				}
			}
		}

		/// <summary>
		/// Creates a scheduled event
		/// </summary>
		private string CreateScheduleEvent(
			string id,
			CallbackMethod method,
			float delayInSeconds,
			bool repeating,
			bool realTime,
			string eventName,
			EventArgs args )
		{
			ScheduledEventInfo info = new ScheduledEventInfo();
			info.ID = GetScheduledEventID( id );
			info.Target = method;
			info.DelayInSeconds = delayInSeconds;
			info.TriggerTime = ( realTime ? _currentRealTime : _currentGameTime ) + delayInSeconds;
			info.LastUpdateTime = realTime ? _currentRealTime : _currentGameTime;
			info.RealTime = realTime;
			info.Repeating = repeating;
			info.EventName = eventName;
			info.Args = args;

			if( info.ID == null )
				System.Diagnostics.Debug.WriteLine( "Warning: Unable to create scheduled event." );
			else
				_scheduledEvents.Add( info.ID, info );

			return info.ID;
		}

		/// <summary>
		/// Returns an available ID. null if the given argument id already is active.
		/// </summary>
		private string GetScheduledEventID( string id )
		{
			if( id != null )
			{
				// See if the requested id is available.
				if( _scheduledEvents.ContainsKey( id ) == false )
					return id;

                System.Diagnostics.Debug.WriteLine("Warning: A scheduled event with that ID is already active.");
				return null;
			}

			// This is needed incase someone created a manual numerical id.
			while( true )
			{
				_nextAvailableEventID++;
				string key = _nextAvailableEventID.ToString();
				if( _scheduledEvents.ContainsKey( key ) == false )
					break;
			}
			return _nextAvailableEventID.ToString();
		}
	}
}