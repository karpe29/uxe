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

		public Vector3 AroundSunRotationVector = Vector3.Forward;
		public float AroundSunRotationSpeed = MathHelper.Pi; // rpm around sun ( radians /sec)
		public float AroundSunRotationOffset = 0;

		public Vector3 SelfRotationVector = Vector3.Up;
		public float SelfRotationSpeed = MathHelper.Pi; // (radians /sec)
		public float SelfRotationOffset = 0;

		public float distanceToSun = 0;

		protected BumpModel3D m_model;

		private SolarSystem m_solarSystem;

		public BumpModel3D Model { get { return m_model; } }

		// usefull for the sun override
		protected Planet(GameScreenManager gameScreenManager, SolarSystem solarSystem, PhysicalType type)
			: base(gameScreenManager.Game, type)
		{
			m_type = type;
			m_solarSystem = solarSystem;
		}

		public Planet(GameScreenManager gameScreenManager, SolarSystem solarSystem, PlanetType type)
			: base (gameScreenManager.Game, (PhysicalType)type)
		{
			m_type = (PhysicalType)type;
			m_planetType = type;

			m_solarSystem = solarSystem;

			m_model = new BumpModel3D(gameScreenManager, type.AssetName);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			if (this.GetType() != typeof(Sun))
			{
				// calculate new position
				//Matrix aroundSun = Matrix.CreateFromAxisAngle(AroundSunRotationVector, (AroundSunRotationOffset + this.AroundSunRotationSpeed * (float)gameTime.TotalGameTime.TotalSeconds) % MathHelper.TwoPi);
				//Vector3 newPosition = Vector3.Transform(AroundSunRotationVector, aroundSun);
				//this.Position = m_solarSystem.Sun.Position + newPosition * distanceToSun;

				// en fait, il faudrait ici determiner un vecteur orthogonal a AroundSunRotationVector
				Vector3 positionOffset = new Vector3(0,0, distanceToSun);
				
				// marche pareil avec une matrice de rotation
				Quaternion q =  Quaternion.CreateFromAxisAngle(AroundSunRotationVector, (AroundSunRotationOffset + this.AroundSunRotationSpeed * (float)gameTime.TotalGameTime.TotalSeconds) % MathHelper.TwoPi);

				positionOffset = Vector3.Transform(positionOffset, q);


				this.Position = m_solarSystem.Sun.Position + positionOffset;
			}

			Matrix selfRotation = Matrix.CreateFromAxisAngle(SelfRotationVector, (this.SelfRotationOffset + this.SelfRotationSpeed * (float)gameTime.TotalGameTime.TotalSeconds)%MathHelper.TwoPi);
			
			m_model.World = selfRotation * Matrix.CreateTranslation(this.Position);

			m_model.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			m_model.LightPosition = m_solarSystem.Sun.Position;
			
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
		public Sun(GameScreenManager gameScreenManager, SolarSystem solarSystem, PhysicalType type)
			: base(gameScreenManager,solarSystem, type)
		{
			m_model = new BumpModel3D(gameScreenManager, @"Planets\Sun");
		}

	}
}
