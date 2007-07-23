using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Xe.GameScreen;
using Microsoft.Xna.Framework.Input;
using Xe.Graphics3D;
using Xe.Input;
using Xe.Objects3D;
using Xe.Physics3D;
using Xe.SpaceRace;
using Xe.Particles3D;
using Xe.Tools;

namespace Xe.SpaceRace
{
	class ShipType : PhysicalType
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
											new ShipType(@"Content\Models\StarChaser2", 0.8f, 1.3f, 1.1f, 1.2f, 1.0f,  new Vector3[] { new Vector3(0,0,0), new Vector3(100,100,100) } ), 
											new ShipType(@"Content\Models\StarChaser3", 1.2f, 1.1f, 1.0f, 0.8f, 1.3f,  new Vector3[] { new Vector3(0,0,0), new Vector3(100,100,100) } ), 
											new ShipType(@"Content\Models\StarChaser4", 1.0f, 0.8f, 1.2f, 1.3f, 1.1f,  new Vector3[] { new Vector3(0,0,0), new Vector3(100,100,100) } ) };
	}
	
	/// <summary>
	/// Take care of rendering and managing the ship
	/// </summary>
	class Ship : IShipPhysical
	{
		SpaceRaceScreen m_gameScreen;

		ShipType m_shipType;

		BasicModel3D m_model;

		public BasicModel3D Model { get { return m_model; } }

		private Vector3 m_reactorLength = new Vector3(0, 0, 50),
			particlePos,particleSpeed,particleGravity;
		private Stats m_stats;

		ParticleSystem smokePlumeParticles;
		ParticleSystem fireParticles;

		public Ship(GameScreenManager gameScreenManager, ShipType type)
			: base(gameScreenManager.Game, (PhysicalType)type)
		{
			m_stats = (Stats)gameScreenManager.Game.Services.GetService(typeof(Stats));

			m_gameScreen = gameScreenManager.CurrentGameScreen as SpaceRaceScreen;
			m_shipType = type;

			fireParticles = new FireParticleSystem(gameScreenManager.Game, gameScreenManager.ContentManager);
			fireParticles.Initialize();

			smokePlumeParticles = new SmokePlumeParticleSystem(gameScreenManager.Game, gameScreenManager.ContentManager);
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


			foreach (Vector3 reactor in m_shipType.Reactors)
			{
				particlePos=Vector3.Transform(reactor, Orientation) ;
				particleSpeed = Vector3.Transform(Vector3.Zero, Orientation);
				particleGravity=Vector3.Transform(Vector3.Zero, Orientation);
				fireParticles.AddParticle(particlePos + Position, particleSpeed);
				fireParticles.Gravity = particleGravity;
				m_stats.AddDebugString(Helper.Vector3ToString3f(particlePos));
				m_stats.AddDebugString(Helper.Vector3ToString3f(particleSpeed));
				m_stats.AddDebugString(Helper.Vector3ToString3f(particleGravity));

			}


			foreach (Vector3 reactor in m_shipType.Reactors)
			{
				//smokePlumeParticles.AddParticle(Vector3.Transform(reactor, Orientation) + Position, Vector3.Transform(Speed, Orientation));
			}

			fireParticles.Update(gameTime);
			smokePlumeParticles.Update(gameTime);
		}
			

		public override void Draw(GameTime gameTime)
		{
			// update world matrix here fonction of IPhysical object data
			if (m_model != null)
				m_model.Draw(gameTime);

			//smokePlumeParticles.SetCamera(m_model.View, m_model.Projection);
			//smokePlumeParticles.Gravity = Vector3.Transform(new Vector3(0, 0, 0), Orientation);
			//smokePlumeParticles.Draw(gameTime);

			fireParticles.SetCamera(m_model.View, m_model.Projection);
			fireParticles.Draw(gameTime);
				

			base.Draw(gameTime);
		}

	}
}
