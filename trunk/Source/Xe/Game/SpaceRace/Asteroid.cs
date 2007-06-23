using System;
using System.Collections.Generic;
using System.Text;
using Xe.Physics3D;

namespace Xe.SpaceRace
{
	class AsteroidType : PhysicalType
	{
		public enum Names
		{
			Asteroid1,
			Asteroid2,
			Asteroid3,
			Asteroid4
		};

		string m_assetName;

		public AsteroidType(string assetName, float acceleration, float maxSpeed, float resistance, float gFactor)
			: base(0,acceleration,maxSpeed, resistance, gFactor)
		{
			m_assetName = assetName;
		}
	}


	class Asteroid : IPhysical3D
	{
		AsteroidManager m_manager;
		AsteroidType m_type;

		public Asteroid(AsteroidManager manager, AsteroidType type)
			: base(manager.GameScreenManager.Game, (PhysicalType)type)
		{
			m_manager = manager;
			m_type = type;
		}


	}
}
