using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Xe.GameScreen;
using Microsoft.Xna.Framework.Input;
using Xe.Graphics3D;
using Xe.Physics3D;
using Xe.SpaceRace;
using Xe.Graphics3D.Particles;
using Xe.Tools;

namespace Xe.SpaceRace
{
	public class ShipType : PhysicalType
	{
		string m_model = "";

		List<Vector3> m_reactors = new List<Vector3>();

		#region properties

		public string ModelAsset { get { return m_model; } }
		public List<Vector3> Reactors { get { return m_reactors; } }


		#endregion

		public ShipType(string model, float handling, float acceleration, float maxSpeed, float resistance, float gFactor, Vector3 [] reactors)
			:base (handling,acceleration,maxSpeed,resistance,gFactor)
		{
			m_model = model;

			foreach (Vector3 v in reactors)
				m_reactors.Add(v);
		}



		static public ShipType[] Types = {	new ShipType(@"Content\Models\StarChaser1", 1.3f, 1.2f, 1.0f, 0.8f, 1.1f,  new Vector3[] { new Vector3(8,0,30), new Vector3(-8,0,30) } ), 
											new ShipType(@"Content\Models\StarChaser2", 0.8f, 1.3f, 1.1f, 1.2f, 1.0f,  new Vector3[] { new Vector3(3.25f,0.3f,42f), new Vector3(-3.25f,0.3f,42f), new Vector3(2f,-4.75f,42f),new Vector3(-2f,-4.75f,42f), new Vector3(6.5f,-3.75f,42f),new Vector3(-6.5f,-3.75f,42f) } ), 
											new ShipType(@"Content\Models\StarChaser3", 1.2f, 1.1f, 1.0f, 0.8f, 1.3f,  new Vector3[] { new Vector3(0,7.5f,38f),new Vector3(3f,9.5f,38f),new Vector3(-3f,9.5f,38f),new Vector3(2f,4.5f,38f), new Vector3(-2f,4.5f,38f) } ), 
											new ShipType(@"Content\Models\StarChaser4", 1.0f, 0.8f, 1.2f, 1.3f, 1.1f,  new Vector3[] { new Vector3(-3.75f,-3.25f,58f),new Vector3(3.75f,-3.25f,58f),new Vector3(-9.75f,-4.75f,57f), new Vector3(9.75f,-4.75f,57f) } ) };
	}
	
	/// <summary>
	/// Take care of rendering and managing the ship
	/// </summary>
	public class Ship : IShipPhysical
	{
		IGameScreen m_gameScreen;

		ShipType m_shipType;

		BasicModel3D m_model;

		public float count = 0,
			ratioParticles=12;

		public BasicModel3D Model { get { return m_model; } }

		private Vector3 m_reactorLength = new Vector3(0, 0, 50),
			particlePos,particleSpeed,particleGravity;
		private Stats m_stats;

		private Matrix m_ParticlesView=Matrix.Identity;

		public void setParticlesView(Matrix matrix)
		{
			m_ParticlesView = matrix;
		}

		ParticleSystem smokePlumeParticles;
		ParticleSystem fireParticles;

		public Ship(GameScreenManager gameScreenManager, ShipType type)
			: base(gameScreenManager.Game, (PhysicalType)type)
		{
			m_stats = (Stats)gameScreenManager.Game.Services.GetService(typeof(Stats));

			m_gameScreen = gameScreenManager.CurrentGameScreen ;
			m_shipType = type;

			fireParticles = new ShipFireParticleSystem(gameScreenManager.Game, XeGame.ContentManager);
			fireParticles.Initialize();


			smokePlumeParticles = new ShipSmokePlumeParticleSystem(gameScreenManager.Game, XeGame.ContentManager);
			smokePlumeParticles.Initialize();
			
			this.Initialize();
		}

		protected override void LoadGraphicsContent(bool loadAllContent)
		{
			base.LoadGraphicsContent(loadAllContent);
			
			if (loadAllContent)
			{
				m_model = new BasicModel3D(m_gameScreen.GameScreenManager, m_shipType.ModelAsset);
			}
		}


		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
			float seconds = ((float)(gameTime.ElapsedGameTime).Ticks) / 10000000f;

			m_model.World = DrawOrientation * Matrix.CreateTranslation(Position);

			float inc= -ratioParticles * Speed.Z / m_maxSpeed;
			count += 1+inc;
			if (count > ratioParticles)
			{
				float orientY = -RotationSpeed.Y / m_handling /2;
				float orientX =  -RotationSpeed.X / m_handling / 2;
				Matrix orientParticles = Matrix.CreateFromYawPitchRoll(orientY, orientX, 0);
				particleSpeed = new Vector3(0, 0, 8) * (2 + inc);
				foreach (Vector3 reactor in m_shipType.Reactors)
				{
					particlePos = reactor;
					fireParticles.AddParticle(particlePos, particleSpeed);
					smokePlumeParticles.AddParticle(particlePos, particleSpeed);
				}
				particleGravity = Vector3.Transform(new Vector3(0, 0, 4), orientParticles) * (2 + inc);

				fireParticles.Gravity = particleGravity;
				smokePlumeParticles.Gravity = particleGravity;


				count -= ratioParticles;
			}

			fireParticles.Update(gameTime);
			smokePlumeParticles.Update(gameTime);

			/*
			KeyboardState ks = Keyboard.GetState();

			if (ks.IsKeyDown(Keys.A))
				XeGame.PostProcessManager.EnableGaussianBlur = true;
			else
				XeGame.PostProcessManager.EnableGaussianBlur = false;

			if (ks.IsKeyDown(Keys.B))
				XeGame.PostProcessManager.EnableBloom = true;
			else
				XeGame.PostProcessManager.EnableBloom = false;

			if (ks.IsKeyDown(Keys.C))
				XeGame.PostProcessManager.EnableColorInverse = true;
			else
				XeGame.PostProcessManager.EnableColorInverse = false;

			if (ks.IsKeyDown(Keys.D))
				XeGame.PostProcessManager.EnableRadialBlur = true;
			else
				XeGame.PostProcessManager.EnableRadialBlur = false;

			if (ks.IsKeyDown(Keys.E))
				XeGame.PostProcessManager.EnableToneMapping = true;
			else
				XeGame.PostProcessManager.EnableToneMapping = false;
			*/

		}
			

		public override void Draw(GameTime gameTime)
		{
			// update world matrix here fonction of IPhysical object data
			if (m_model != null)
				m_model.Draw(gameTime);

			smokePlumeParticles.SetCamera(m_ParticlesView, m_model.Projection);
			smokePlumeParticles.Draw(gameTime);

			fireParticles.SetCamera( m_ParticlesView,m_model.Projection);
			fireParticles.Draw(gameTime);
			
			base.Draw(gameTime);
		}

	}
}
