using System;
using System.Collections.Generic;
using System.Text;

namespace XeFramework.Events
{
	public enum Modifier
	{
		SpeedRatio, // in percent %
		Position, // Vector3
		Direction, // Vector3
		Rotation, // TODO: To be defined : Vector + Angle, or Matrix, Or Quaternion.
	}

	public abstract class IEvent
	{
		string Name;

		// list of IEvent required to this IEvent to occurs
		List<IEvent> m_required;

		// List of IEvent this participate to occurences. (aka list of the IEvent who this is in m_required)
		List<IEvent> m_occurs;

		List<Modifier> m_availableModifiers;

		public void ApplyModifier(Modifier thisModifier, object value)
		{
			// Check if modifier is available, then apply it.
		}
	}

	class EventManager
	{
		// First Event to occurs
		IEvent m_bigBang;

		// Last event than can occurs -> End of the game (or Error :p)
		IEvent m_apocalypse;



		public void BuildEventListFromFile(string filepath)
		{

		}
	}
}
