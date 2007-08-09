using System;
using System.Collections.Generic;
using System.Text;
using Xe.Physics3D;
using Microsoft.Xna.Framework;
using Xe.Graphics3D;
using Xe.GameScreen;
using Xe.Tools;
using Xe.Graphics3D.Particles;

namespace Xe.SpaceRace
{
	public class PlanetType : PhysicalType
	{
		public enum Names
		{
			Deimos	= 600,
			Phobos	= 960,
			Pluto	= 2306,
			Mercury = 2439,
			Mars	= 3402,
			Moon	= 3474,
			Venus	= 6051,
			Earth	= 6378,
			Neptune = 24961,
			Uranus	= 25656,
			Saturn	= 60268,
			Jupiter = 71491,
			Jupiter2 = 71492,
			Sun		= 353640
		};

		string m_assetName;

		public Names Name;

		public string AssetName { get { return m_assetName; } }

		public PlanetType(Names name, float acceleration, float maxSpeed, float resistance, float gFactor)
			: base(0, acceleration, maxSpeed, resistance, gFactor)
		{
			m_assetName = @"Planets\" + name.ToString();
			Name = name;
		}
	}


	public class Planet : IPhysical3D
	{
		public PlanetType m_planetType;


		protected BumpModel3D m_model;

		private SolarSystem m_solarSystem;
		private ParticleSystem fireParticles;

		public float m_distanceToSun, m_rotationStart, m_aroundRotationSpeed,m_aroundRotation, m_selfRotationSpeed,m_selfRotation;
		private Vector3 m_aroundRotationAxe, m_selfRotationAxe, particlePos, particleSpeed, particleGravity;
		private Matrix particleOrient;

		public SolarSystem SolarSystem { set { m_solarSystem = value; } }

		public BumpModel3D Model { get { return m_model; } }




		public Planet(GameScreenManager gameScreenManager, PlanetType type,SolarSystem solarSystem, float distanceToSun, float rotationStart, float rotationSpeed, Vector3 rotationAxe, float selfRotationSpeed, Vector3 selfRotationAxe)
			: base(gameScreenManager.Game, (PhysicalType)type)
		{
			m_type = (PhysicalType)type;
			m_solarSystem = solarSystem;
			m_planetType = type;
			m_distanceToSun = distanceToSun;
			m_rotationStart = rotationStart;
			m_aroundRotationSpeed = rotationSpeed;
			m_aroundRotationAxe = rotationAxe;
			m_selfRotationSpeed = selfRotationSpeed;
			m_selfRotationAxe = selfRotationAxe;

			m_model = new BumpModel3D(gameScreenManager, type.AssetName);

			if (this.m_solarSystem == null) // ici on est le soleil de niveau 0
			{
				fireParticles = new SunFireParticleSystem(gameScreenManager.Game, XeGame.ContentManager);
				fireParticles.Initialize();
			}

		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			float seconds = ((float)(gameTime.ElapsedGameTime).Ticks) / 10000000f;

			if (m_solarSystem != null) // top level sun
			{
				m_aroundRotation= (m_aroundRotation+m_aroundRotationSpeed*seconds)%MathHelper.TwoPi;
				Position = Vector3.Transform(m_distanceToSun * Vector3.Forward, Matrix.CreateFromAxisAngle(m_aroundRotationAxe, m_rotationStart + m_aroundRotation));
			}
			m_selfRotation = (m_selfRotation + m_selfRotationSpeed * seconds) % MathHelper.TwoPi;

			Orientation = Matrix.CreateFromAxisAngle(m_selfRotationAxe,  m_selfRotation);

			m_model.World = DrawOrientation * Matrix.CreateTranslation(Position);
			if (m_solarSystem != null) // top level sun
			{
				m_model.World*= m_solarSystem.Orientation* Matrix.CreateTranslation(m_solarSystem.Sun.Position);
			}
			else
			{
				for (int i = 0; i < 100;i++ ) // nombre de particules générées a chaque Update
				{
					// on calcule la position d'apparition de la particule sur le soleil via 2 angles
					// angleH ==> longitude
					// angleV ==> latitude
					float angleH = Helper.RandomFloat(0, MathHelper.TwoPi);
					float angleV = Helper.RandomFloat(0, MathHelper.Pi) - MathHelper.PiOver2;

					//System.Console.WriteLine(angleH + " " + angleV);
					// on crée la matrice de rotation associée
					particleOrient=Matrix.CreateFromYawPitchRoll(angleH,angleV,0);
					// qui nous permet de générer un vecteur correspondant a la position de la particule par rapport au centre du soleil
					particlePos = Vector3.Transform(Vector3.Forward, particleOrient) * (float)m_planetType.Name;
					// on définit une vitesse colinéaire au vecteur Position comme ca la particule va "s'éloigner" du soleil
					particleSpeed = Vector3.Zero;//particlePos/20;
					// on met la gravité a zero car la gravité affecte toutes les particules donc on peut pas s'en servir pour faire retomber les particules vers le soleil
					particleGravity = Vector3.Zero;
					fireParticles.AddParticle(Position + particlePos, particleSpeed);
					fireParticles.Gravity = particleGravity;

				}


				fireParticles.Update(gameTime);

			}

			m_model.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			if (m_solarSystem == null) // top level sun
			{
				// On affecte la meme camera que pour le soleil
				fireParticles.SetCamera(m_model.View, m_model.Projection);
				fireParticles.Draw(gameTime);
			}

			m_model.LightPosition = GetTopSunPosition();

			//m_model.Draw(gameTime);

			
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
