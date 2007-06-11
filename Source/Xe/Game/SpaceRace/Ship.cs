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
using Xe.Physics;
using Xe.SpaceRace;


namespace Xe.SpaceRace
{
	class ShipType : PhysicalType
	{
		string m_model;

		#region properties

		public string ModelAsset { get { return m_model; } }

		#endregion

		public ShipType(string model, float handling, float acceleration, float maxSpeed, float resistance, float gFactor)
			:base (handling,acceleration,maxSpeed,resistance,gFactor)
		{
			m_model = model;
		}
		
		static public ShipType[] Types = {	new ShipType(@"Content\Models\StarChaser1", 1.3f, 1.2f, 1.0f, 0.8f, 1.1f), 
											new ShipType(@"Content\Models\StarChaser2", 0.8f, 1.3f, 1.1f, 1.2f, 1.0f), 
											new ShipType(@"Content\Models\StarChaser3", 1.2f, 1.1f, 1.0f, 0.8f, 1.3f), 
											new ShipType(@"Content\Models\StarChaser4", 1.0f, 0.8f, 1.2f, 1.3f, 1.1f) };
	}


	
	/// <summary>
	/// Take care of rendering and managing the ship
	/// </summary>
	class Ship : IPhysical3D
	{
		SpaceRaceScreen m_gameScreen;

		ShipType m_type;

		BasicModel3D m_model;

		public BasicModel3D Model { get { return m_model; } }
		
		

		public Ship(GameScreenManager gameScreenManager, ShipType type)
			: base(gameScreenManager.Game,new PhysicalType(type.Handling,type.Acceleration,type.MaxSpeed, type.Resistance, type.GravityFactor))
		{
			m_gameScreen = gameScreenManager.CurrentGameScreen as SpaceRaceScreen;
			m_type = type;
			//rotationAcceleration = new Vector3(1, 0, 0);	
			rotationSpeed = new Vector3(0, 0, 0);
			this.Initialize();
		}

		protected override void LoadGraphicsContent(bool loadAllContent)
		{
			base.LoadGraphicsContent(loadAllContent);
			
			if (loadAllContent)
			{
				m_model = new BasicModel3D(m_gameScreen.GameScreenManager, m_type.ModelAsset);
			}
		}


		void UpdateCameraThirdPerson()
		{
			/// NO MORE USED
			/*
			Vector3 thirdPersonReference = new Vector3(0, 2000, 2000);
			thirdPersonReference = Vector3.Transform(thirdPersonReference, Matrix.CreateFromQuaternion(m_rotation));
			thirdPersonReference = Vector3.Transform(thirdPersonReference, Matrix.CreateTranslation(m_position));

			Vector3 cameraUp = new Vector3(0, 1, 0);
			cameraUp = Vector3.Transform(cameraUp, Matrix.CreateFromQuaternion(m_rotation));
			//cameraUp = Vector3.Transform(cameraUp, Matrix.CreateTranslation(m_position));

			ViewMatrix = Matrix.CreateLookAt(thirdPersonReference, m_position , cameraUp);
			 * */
		}

		public override void Update(GameTime gameTime)
		{

			/*if (rotationSpeed.Y > 10) rotationAcceleration = new Vector3(0f, -2f, 0f);
			if (rotationSpeed.Y < 0) { 
				rotationAcceleration = new Vector3(0f, 2f, 0f);
				rotationSpeed = new Vector3(0f, 0f, 0f);
			}*/
			
			base.Update(gameTime);



			m_model.World = DrawOrientation*Matrix.CreateTranslation(linearPosition);
			
			/*	

			m_yRotation = 0;
			if (g1.DPad.Left == ButtonState.Pressed||g1.ThumbSticks.Left.X < 0)
				m_yRotation = m_baseRotation;

			if (g1.DPad.Right == ButtonState.Pressed ||	g1.ThumbSticks.Left.X > 0)
				m_yRotation = -m_baseRotation;

			m_xRotation = 0;
			if (g1.DPad.Up == ButtonState.Pressed || g1.ThumbSticks.Left.Y > 0)
				m_xRotation = m_baseRotation;

			if (g1.DPad.Down == ButtonState.Pressed || g1.ThumbSticks.Left.Y < 0)
				m_xRotation = -m_baseRotation;


			Quaternion additionalRot = Quaternion.CreateFromAxisAngle(new Vector3(0, 1, 0), m_yRotation) * Quaternion.CreateFromAxisAngle(new Vector3(1, 0, 0), m_xRotation);
			m_rotation *= additionalRot;

			float speed = 0;
			if (g1.Triggers.Right > 0)
				speed = m_baseSpeed;

			if (g1.Triggers.Left > 0)
				speed = -m_baseSpeed;


			Vector3 addvector = new Vector3(0, 0, 1);
			addvector = Vector3.Transform(addvector, Matrix.CreateFromQuaternion(m_rotation));
			addvector.Normalize();

			m_position += addvector * speed;


			UpdateCameraThirdPerson();
				*/
		}
			

		public override void Draw(GameTime gameTime)
		{
			

			// update world matrix here fonction of IPhysical object data
			if (m_model != null)
				m_model.Draw(gameTime);
				

			base.Draw(gameTime);
		}

	}
}
