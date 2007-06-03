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
		string m_modelFilename;
		Quaternion m_initialRotation;
		float m_initialSize;

	}

	class Ship : IPhysicableObject
	{
		SpaceRaceScreen m_gameScreen;

		Matrix m = Matrix.Identity;

		Model m_model;

		//Camera m_camera;
		//CameraInput m_input;

		private Vector3 m_position; // only z and x are used.

		public Vector3 transformedReference;

		public Vector3 Position
		{
			get { return m_position; }
			set { m_position = value; }
		}

		Vector3 m_direction;

		public Quaternion m_rotation = Quaternion.Identity;

		float m_xRotation = 0, m_yRotation = 0, m_zRotation = 0;

		float m_baseSpeed = 10;
		float m_baseRotation = 0.001f;

		public Matrix ViewMatrix;
		public Matrix ProjectionMatrix;

		/// <summary>
		/// Return interpolated value based on two TimeSpan
		/// </summary>
		/// <param name="f">The max value</param>
		/// <param name="duration">The current duration</param>
		/// <param name="totalDuration">The total desired effect duration</param>
		/// <returns>A value between 0 and f</returns>
		public float GetInterpolatedValue(float f, TimeSpan duration, TimeSpan totalDuration)
		{
			if (duration <= TimeSpan.Zero)
				return 0;

			if (duration >= totalDuration)
				return f;

			return MathHelper.SmoothStep(0, f, duration.Ticks / totalDuration.Ticks);
		}

		public Ship(GameScreenManager gameScreenManager, Model model)
			: base(gameScreenManager.Game)
		{
			m_gameScreen = gameScreenManager.CurrentGameScreen as SpaceRaceScreen;
			m_model = model;

			/*
			m_camera = new Camera(this.Game);
			m_camera.CreateCamera(new Vector3(5000, 5000, 5000), MathHelper.PiOver4, 1.0f, 1.0f, 1000000.0f);
			this.Game.Components.Add(m_camera);

			m_input = new CameraInput(this.Game, m_camera);
			this.Game.Components.Add(m_input);
			*/

			this.Initialize();
		}

		protected override void LoadGraphicsContent(bool loadAllContent)
		{
			base.LoadGraphicsContent(loadAllContent);

			if (loadAllContent)
			{
				//m_model = m_gameScreen.GameScreenManager.ContentManager.Load<Model>(@"Content\Models\StarChaser1");
			}

			float AspectRatio = (float)Game.Window.ClientBounds.Width / (float)Game.Window.ClientBounds.Height;
			ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), AspectRatio, 1.0f, 200000.0f);
		}

		void UpdateCameraFirstPerson()
		{
			/*
			Matrix rotationMatrix = Matrix.CreateRotationY(avatarYaw);

			// Transform the head offset so the camera is positioned properly relative to the avatar.
			Vector3 headOffset = Vector3.Transform(avatarHeadOffset, rotationMatrix);

			// Calculate the camera's current position.
			Vector3 cameraPosition = avatarPosition + headOffset;

			// Create a vector pointing the direction the camera is facing.
			Vector3 transformedReference = Vector3.Transform(cameraReference, rotationMatrix);

			// Calculate the position the camera is looking at.
			Vector3 cameraLookat = transformedReference + cameraPosition;

			// Set up the view matrix and projection matrix.

			view = Matrix.CreateLookAt(cameraPosition, cameraLookat, new Vector3(0.0f, 1.0f, 0.0f));

			Viewport viewport = graphics.GraphicsDevice.Viewport;
			float aspectRatio = (float)viewport.Width / (float)viewport.Height;

			proj = Matrix.CreatePerspectiveFieldOfView(viewAngle, aspectRatio, nearClip, farClip);
			*/
		}

		void UpdateCameraThirdPerson()
		{
			Vector3 thirdPersonReference = new Vector3(0, 2000, 2000);
			thirdPersonReference = Vector3.Transform(thirdPersonReference, Matrix.CreateFromQuaternion(m_rotation));
			thirdPersonReference = Vector3.Transform(thirdPersonReference, Matrix.CreateTranslation(m_position));

			Vector3 cameraUp = new Vector3(0, 1, 0);
			cameraUp = Vector3.Transform(cameraUp, Matrix.CreateFromQuaternion(m_rotation));
			//cameraUp = Vector3.Transform(cameraUp, Matrix.CreateTranslation(m_position));

			ViewMatrix = Matrix.CreateLookAt(thirdPersonReference, m_position , cameraUp);
		}

		public override void Update(GameTime gameTime)
		{
			m = Matrix.Identity;

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


			//Copy any parent transforms
			Matrix[] transforms = new Matrix[m_model.Bones.Count];
			m_model.CopyAbsoluteBoneTransformsTo(transforms);

			//Draw the model, a model can have multiple meshes, so loop
			for (int i = 0; i < m_model.Meshes.Count; i++)
			{
				//This is where the mesh orientation is set, as well as our camera and projection
				for (int j = 0; j < m_model.Meshes[i].Effects.Count; j++)
				{
					(m_model.Meshes[i].Effects[j] as BasicEffect).EnableDefaultLighting();
					(m_model.Meshes[i].Effects[j] as BasicEffect).World = transforms[m_model.Meshes[i].ParentBone.Index] * Matrix.CreateScale(1) *Matrix.CreateFromQuaternion(m_rotation) * Matrix.CreateTranslation(m_position);

					(m_model.Meshes[i].Effects[j] as BasicEffect).View = this.ViewMatrix;
					(m_model.Meshes[i].Effects[j] as BasicEffect).Projection = this.ProjectionMatrix;
				}

				m_model.Meshes[i].Draw();
			}

			base.Draw(gameTime);
		}

	}
}
