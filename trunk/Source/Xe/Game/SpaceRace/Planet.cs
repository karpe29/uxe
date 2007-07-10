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


	public class Planet : IPhysical3D
	{
		PlanetType m_planetType;


		protected BumpModel3D m_model;

		private SolarSystem m_solarSystem;


		public float m_distanceToSun, m_rotationStart, m_aroundRotationSpeed,m_aroundRotation, m_selfRotationSpeed;
		private Vector3 m_aroundRotationAxe, m_selfRotationAxe;

		public SolarSystem SolarSystem { set { m_solarSystem = value; } }

		public BumpModel3D Model { get { return m_model; } }



		public Planet(GameScreenManager gameScreenManager, PlanetType type, float distanceToSun, float rotationStart, float rotationSpeed, Vector3 rotationAxe, float selfRotationSpeed, Vector3 selfRotationAxe)
			: base(gameScreenManager.Game, (PhysicalType)type)
		{
			m_type = (PhysicalType)type;
			m_planetType = type;
			m_distanceToSun = distanceToSun;
			m_rotationStart = rotationStart;
			m_aroundRotationSpeed = rotationSpeed;
			m_aroundRotationAxe = rotationAxe;
			m_selfRotationSpeed = selfRotationSpeed;
			m_selfRotationAxe = selfRotationAxe;

			m_model = new BumpModel3D(gameScreenManager, type.AssetName);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

						float seconds = ((float)(gameTime.ElapsedGameTime).Milliseconds) / 1000f;

			if (m_solarSystem != null) // top level sun
			{
				m_aroundRotation= (m_aroundRotation+m_aroundRotationSpeed*seconds)%MathHelper.TwoPi;
				Position = Vector3.Transform(m_distanceToSun * Vector3.Forward, Matrix.CreateFromAxisAngle(m_aroundRotationAxe, m_rotationStart + m_aroundRotation));
			}

			Orientation = Matrix.CreateFromYawPitchRoll(m_rotationPosition.Y, m_rotationPosition.X, m_rotationPosition.Z) * Orientation;

			m_model.World = DrawOrientation * Matrix.CreateTranslation(Position);
			if (m_solarSystem != null) // top level sun
			{
				m_model.World*= m_solarSystem.Orientation* Matrix.CreateTranslation(m_solarSystem.Sun.Position);
			}

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
			if (this.m_solarSystem == null) // ici on est le soleil de niveau 0
			{
				return this.Position;
			}
			else
			{
				return this.m_solarSystem.Sun.GetTopSunPosition();
			}
		}

		public void SetCamera(Matrix view, Matrix projection)
		{
			m_model.View = view;
			m_model.Projection = projection;


		}

	}
}
