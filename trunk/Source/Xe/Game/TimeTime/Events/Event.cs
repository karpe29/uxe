
using System;
using System.Collections.Generic;

namespace Xe.TimeTime.Events
{
	public partial class EventManager
	{
		/// <summary>
		/// Helper class for events. Stores all filters related to this event.
		/// </summary>
		private sealed class Event
		{
			private string _eventName;

			Dictionary<int, EventFilter> _filters = new Dictionary<int, EventFilter>();
			Queue<EventFilter> _invokeQueue = new Queue<EventFilter>();

			/// <summary>
			/// Constructor
			/// </summary>
			public Event( string eventName )
			{
				_eventName = eventName;
			}

			/// <summary>
			/// Adds a filter to the dictionary if it doesn't already exists.
			/// </summary>
			public void AddFilter( EventFilter filter )
			{
				if( _filters.ContainsKey( filter.ID ) == false )
					_filters.Add( filter.ID, filter );
			}

			/// <summary>
			/// Removes a filter from the dictionary.
			/// </summary>
			public void RemoveFilter( EventFilter filter )
			{
				if( _filters.ContainsKey( filter.ID ) )
					_filters.Remove( filter.ID );
			}

			/// <summary>
			/// Invokes this event on all stored event filters
			/// </summary>
			public void Invoke( float currentTime, EventArgs args )
			{
				// Call invoke from a duplicated list, events might
				// be modified during the loop.
				foreach( EventFilter filter in _filters.Values )
					_invokeQueue.Enqueue( filter );

				while( _invokeQueue.Count > 0 )
					_invokeQueue.Dequeue().Invoke( _eventName, currentTime, args );
			}
		}
	}
}