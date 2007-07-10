using System;
using System.Collections.Generic;
using System.Text;
using Xe.Physics3D;
using Microsoft.Xna.Framework;
using Xe.Graphics3D;
using Xe.GameScreen;
using Xe.Tools;

namespace Xe.SpaceRace
{
	public class PlanetType : PhysicalType
	{
		public enum Names
		{
			Sun,
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


	public class Planet : IPlanetPhysical
	{
		PlanetType m_planetType;


		protected BumpModel3D m_model;
		private Vector3 m_startPosition,m_absolutePosition;

		private SolarSystem m_solarSystem;

		public BumpModel3D Model { get { return m_model; } }

		// usefull for the sun override
		/*protected Planet(GameScreenManager gameScreenManager, SolarSystem solarSystem, PhysicalType type)
			: base(gameScreenManager.Game, type)
		{
			m_type = type;
			m_solarSystem = solarSystem;
		}*/

		public Planet(GameScreenManager gameScreenManager, SolarSystem solarSystem, PlanetType type,Vector3 startPosition,Vector3 rotationSpeed)
			: base (gameScreenManager.Game, (PhysicalType)type)
		{
			m_type = (PhysicalType)type;
			m_planetType = type;
			m_startPosition = startPosition;
			RotationSpeed = rotationSpeed;
			m_solarSystem = solarSystem;

			m_model = new BumpModel3D(gameScreenManager, type.AssetName);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			Position = Vector3.Transform(m_startPosition, Orientation);
			if (m_solarSystem != null)
			{
				m_absolutePosition = m_solarSystem.Sun.Position + Position;
			} 
				else
				{
				m_absolutePosition=Position;
				}
			
			m_model.World = DrawOrientation * Matrix.CreateTranslation(m_absolutePosition);


			m_model.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			if (m_solarSystem != null)
			{
				m_model.LightPosition = m_solarSystem.Sun.Position;
			}
			m_model.Draw(gameTime);

			base.Draw(gameTime);
		}

		public void SetCamera(Matrix view, Matrix projection)
		{
			m_model.View = view;
			m_model.Projection = projection;

			
		}

	}

	/*class Sun : Planet
	{
		public Sun(GameScreenManager gameScreenManager, SolarSystem solarSystem, PhysicalType type)
			: base(gameScreenManager,solarSystem, type)
		{
			m_model = new BumpModel3D(gameScreenManager, @"Planets\Sun");
		}

	}*/
}
