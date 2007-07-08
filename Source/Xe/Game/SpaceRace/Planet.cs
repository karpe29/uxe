using System;
using System.Collections.Generic;
using System.Text;
using Xe.Physics3D;
using Microsoft.Xna.Framework;
using Xe.Graphics3D;
using Xe.GameScreen;

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
			Uranus,
			Venus,
			Phobos,
			Deimos,
			LASTUNUSED
		};

		string m_assetName;

		public string AssetName { get { return m_assetName; } }

		public PlanetType(Names name, float acceleration, float maxSpeed, float resistance, float gFactor)
			: base(0,acceleration,maxSpeed, resistance, gFactor)
		{
			m_assetName = @"Planets\" + name.ToString();
		}
	}


	class Planet : IPhysical3D
	{
		PlanetType m_planetType;

		public float RotationSpeed = 1; // rpm around sun
		public float distanceToSun = 0;

		protected BumpModel3D m_model;

		public BumpModel3D Model { get { return m_model; } }

		// usefull for the sun override
		protected Planet(GameScreenManager gameScreenManager, PhysicalType type)
			: base(gameScreenManager.Game, type)
		{
			m_type = type;
		}

		public Planet(GameScreenManager gameScreenManager, PlanetType type)
			: base (gameScreenManager.Game, (PhysicalType)type)
		{
			m_type = (PhysicalType)type;
			m_planetType = type;

			m_model = new BumpModel3D(gameScreenManager, type.AssetName);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			m_model.World = Matrix.CreateTranslation(this.Position);

			m_model.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			m_model.Draw(gameTime);

			base.Draw(gameTime);
		}

		public void SetCamera(Matrix view, Matrix projection)
		{
			m_model.View = view;
			m_model.Projection = projection;

			
		}

	}

	class Sun : Planet
	{
		public Sun(GameScreenManager gameScreenManager, PhysicalType type)
			: base(gameScreenManager, type)
		{
			m_model = new BumpModel3D(gameScreenManager, @"Planets\Sun");
		}

	}
}
