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
		};

		string m_assetName;

		public string AssetName { get { return m_assetName; } }

		public PlanetType(Names name, float acceleration, float maxSpeed, float resistance, float gFactor)
			: base(0, acceleration, maxSpeed, resistance, gFactor)
		{
			m_assetName = @"Planets\" + name.ToString();
		}
	}


	public class Planet : IPlanetPhysical
	{
		PlanetType m_planetType;


		protected BumpModel3D m_model;
		private Vector3 m_startPosition, m_absolutePosition;

		private SolarSystem m_solarSystem;

		public SolarSystem SolarSystem { set { m_solarSystem = value; } }

		public BumpModel3D Model { get { return m_model; } }



		public Planet(GameScreenManager gameScreenManager, PlanetType type, Vector3 startPosition, Vector3 rotationSpeed)
			: base(gameScreenManager.Game, (PhysicalType)type)
		{
			m_type = (PhysicalType)type;
			m_planetType = type;
			m_startPosition = startPosition;
			RotationSpeed = rotationSpeed;

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
				m_absolutePosition = Position;
			}

			m_model.World = DrawOrientation * Matrix.CreateTranslation(m_absolutePosition);


			m_model.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			m_model.LightPosition = GetTopSunPosition();
			
			m_model.Draw(gameTime);

			base.Draw(gameTime);
		}

		public Vector3 GetTopSunPosition()
		{
			Vector3 v = Vector3.Zero;

			//while (m_solarSystem != null && m_solarSystem.Sun != null)
			//	v = m_solarSystem.Sun.GetTopSunPosition();

			return v;
		}

		public void SetCamera(Matrix view, Matrix projection)
		{
			m_model.View = view;
			m_model.Projection = projection;


		}

	}
}
