using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using XeFramework.GameScreen;
using Microsoft.Xna.Framework.Input;
using XeFramework.Graphics3D;
using XeFramework.Input;
using XeFramework.Objects3D;
using Xe._3D.Physics;

namespace XeFramework.XeGame.SpaceRace
{
	class ShipType
	{
		string m_model;
		float m_handling;
		float m_acceleration;
		float m_maxSpeed;
		float m_resistance;
		float m_gravityFactor;

		#region properties

		public string ModelAsset { get { return m_model; } }
		public float Handling { get { return m_handling; } }
		public float Acceleration { get { return m_acceleration; } }
		public float MaxSpeed { get { return m_maxSpeed; } }
		public float Resistance { get { return m_resistance; } }
		public float GravityFactor { get { return m_gravityFactor; } }

		#endregion

		public ShipType(string model, float handling, float acceleration, float maxSpeed, float resistance, float gFactor)
		{
			m_model = model;
			m_handling = handling;
			m_acceleration = acceleration;
			m_maxSpeed = maxSpeed;
			m_resistance = resistance;
			m_gravityFactor = gFactor;
		}
		
		static public ShipType[] Types = {	new ShipType(@"Content\Models\StarChaser1", 1.3f, 1.2f, 1.0f, 0.8f, 1.1f), 
											new ShipType(@"Content\Models\StarChaser2", 0.8f, 1.3f, 1.1f, 1.2f, 1.0f), 
											new ShipType(@"Content\Models\StarChaser3", 1.2f, 1.1f, 1.0f, 0.8f, 1.3f), 
											new ShipType(@"Content\Models\StarChaser4", 1.0f, 0.8f, 1.2f, 1.3f, 1.1f) };
	}

	
	/// <summary>
	/// Take care of rendering and managing the ship
	/// </summary>
	class Ship : IPhysicableObject
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



			m_model.World = Matrix.CreateRotationZ(rotationPosition.Z) * Matrix.CreateRotationX(rotationPosition.X) * Matrix.CreateRotationY(rotationPosition.Y)*Matrix.CreateTranslation(linearPosition);
			m_model.View = Matrix.CreateLookAt(new Vector3(200, 100, 200), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
			m_model.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)(1), 1, 10000);

			/*m = Matrix.Identity;

			base.Update(gameTime);

			m_gameScreen.GameScreenManager.Stats.AddModelPolygonsCount(m_model);
			
			GamePadState g1 = GamePad.GetState(PlayerIndex.One);
			GamePadState g2 = GamePad.GetState(PlayerIndex.Two);
			GamePadState g3 = GamePad.GetState(PlayerIndex.Three);
			GamePadState g4 = GamePad.GetState(PlayerIndex.Four);

		

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
			/*this.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
			this.GraphicsDevice.RenderState.TwoSidedStencilMode = true;

			this.GraphicsDevice.RenderState.DepthBufferEnable = true;
			this.GraphicsDevice.RenderState.AlphaBlendEnable = false;
			this.GraphicsDevice.RenderState.AlphaTestEnable = false;
			this.GraphicsDevice.RenderState.DepthBufferWriteEnable = true;

			this.GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
			this.GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;
			*/

			// update world matrix here fonction of IPhysical object data
			if (m_model != null)
				m_model.Draw(gameTime);
				

			base.Draw(gameTime);
		}

	}
}
