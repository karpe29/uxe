using System;
using System.Collections.Generic;
using System.Text;
using Xe.Physics3D;
using Microsoft.Xna.Framework;
using Xe.Graphics3D;

namespace Xe.SpaceRace
{
	class PlanetType : PhysicalType
	{
		public enum Names
		{
			Earth,
			Jupiter,
			Jupiter2,
			Mars,
			Mercury,
			Moon,
			Neptune,
			Pluto,
			Saturn,
			Sun,
			Uranus,
			Venus,
			Phobos,
			Deimos
		};

		string m_assetName;

		public string AssetName { get { return m_assetName; } }

		public PlanetType(string assetName, float acceleration, float maxSpeed, float resistance, float gFactor)
			: base(0,acceleration,maxSpeed, resistance, gFactor)
		{
			m_assetName = assetName;
		}
	}


	class Planet : IPhysical3D
	{
		PlanetManager m_manager;
		PlanetType m_type;

		ParallaxModel3D m_model;

		public ParallaxModel3D Model { get { return m_model; } }


		public Planet(PlanetManager manager, PlanetType type)
			: base (manager.GameScreenManager.Game, (PhysicalType)type)
		{
			m_manager = manager;
			m_type = type;

			m_model = new ParallaxModel3D(manager.GameScreenManager, type.AssetName);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			m_model.Update(gameTime);

			this.UpdatePositions(gameTime);
		}

		private void UpdatePositions(GameTime gameTime)
		{
			
		}

		public override void Draw(GameTime gameTime)
		{
			m_model.Draw(gameTime);

			base.Draw(gameTime);
		}


	}
}
