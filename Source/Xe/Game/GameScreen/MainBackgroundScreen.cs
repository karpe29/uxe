using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using Xe.GUI;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Xe;

namespace Xe.GameScreen
{
	class MainBackgroundScreen : IGameScreen
	{
		Model myModel;
		Effect myEffect;
		Texture2D myTexture;

		//Position of the model in world space
		Vector3 modelPosition = new Vector3(0, 0, 0);
		float modelRotation = 0.0f;

		//Position of the Camera in world space, for our view matrix
		Vector3 cameraPosition = new Vector3(0, 0, 2000);
		Vector3 cameraTagetPosition = new Vector3(0, 0, 0);

		private Matrix projectionMatrix;
		private float aspectRatio;

		Random r = new Random();

		public Matrix ViewMatrix
		{
			get
			{
				// pas optimal du tout !
				return Matrix.CreateLookAt(cameraPosition, cameraTagetPosition, Vector3.Up) * Matrix.CreateRotationZ(modelRotation);
			}
			/*set
			{
				// Set view matrix, usually only done in ChaseCamera.Update!
				viewMatrix = value;
			}*/
		}

		public Matrix ProjectionMatrix
		{
			get
			{
				return projectionMatrix;
			}
			set
			{
				projectionMatrix = value;
			}
		}

		private float TimeScale = 4.0f, Horizontal = 0.75f, Vertical = 0.75f;

		public MainBackgroundScreen(GameScreenManager gameScreenManager)
			: base(gameScreenManager, true)
		{
		}

		public void AddEffectToModel(Model thisModel, Effect thisEffect)
		{
			for (int i = 0; i < thisModel.Meshes.Count; i++)
			{
				for (int j = 0; j < thisModel.Meshes[i].MeshParts.Count; j++)
				{
					thisModel.Meshes[i].MeshParts[j].Effect = thisEffect;
				}
			}
		}



		protected override void LoadGraphicsContent(bool loadAllContent)
		{
			base.LoadGraphicsContent(loadAllContent);

			myModel = XeGame.ContentManager.Load<Model>(@"Content\Models\MenuTunnel");

			myEffect = XeGame.ContentManager.Load<Effect>(@"Content\Effects\MrWiggle");
			myEffect.Parameters["TimeScale"].SetValue(TimeScale);
			myEffect.Parameters["Horizontal"].SetValue(Horizontal);
			myEffect.Parameters["Vertical"].SetValue(Vertical);
			myEffect.Parameters["colorTexture"].SetValue(myTexture);
			myEffect.CurrentTechnique = myEffect.Techniques["Textured"];
			foreach (EffectParameter p in myEffect.Parameters)
				Console.WriteLine(p.Name);

			myTexture = XeGame.ContentManager.Load<Texture2D>(@"Content\Textures\wedge_p1_diff_v1");

			//Aspect ratio to use for the projection matrix
			aspectRatio = aspectRatio = (float)this.GraphicsDevice.PresentationParameters.BackBufferWidth / (float)this.GraphicsDevice.PresentationParameters.BackBufferHeight;

			ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 500000.0f);
		}

		#region IGameScreen Members

		public override bool IsBlockingUpdate { get { return true; } }

		public override bool IsBlockingDraw { get { return true; } }

		#endregion

		public override void Draw(GameTime gameTime)
		{
			//base.Draw(gameTime);

			XeGame.Stats.AddModelPolygonsCount(myModel);

			this.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
			this.GraphicsDevice.RenderState.TwoSidedStencilMode = true;

			this.GraphicsDevice.RenderState.DepthBufferEnable = true;
			this.GraphicsDevice.RenderState.AlphaBlendEnable = false;
			this.GraphicsDevice.RenderState.AlphaTestEnable = false;

			this.GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
			this.GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;

			//Copy any parent transforms
			Matrix[] transforms = new Matrix[myModel.Bones.Count];
			myModel.CopyAbsoluteBoneTransformsTo(transforms);

			//myEffect.Parameters

			//myEffect.Parameters["World"].SetValue(Matrix.Identity);
			//myEffect.Parameters["WorldViewProj"].SetValue(ViewMatrix * ProjectionMatrix);



			//Draw the model, a model can have multiple meshes, so loop
			for (int i = 0; i < myModel.Meshes.Count; i++)
			{
				//This is where the mesh orientation is set, as well as our camera and projection
				for (int j = 0; j < myModel.Meshes[i].Effects.Count; j++)
				{
					(myModel.Meshes[i].Effects[j] as BasicEffect).EnableDefaultLighting();
					(myModel.Meshes[i].Effects[j] as BasicEffect).World =
						transforms[myModel.Meshes[i].ParentBone.Index]
						* Matrix.CreateRotationY(modelRotation)
						* Matrix.CreateTranslation(modelPosition);

					(myModel.Meshes[i].Effects[j] as BasicEffect).View = ViewMatrix;
					(myModel.Meshes[i].Effects[j] as BasicEffect).Projection = ProjectionMatrix;


					myEffect.Parameters["Timer"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);
					/*
							myEffect.Parameters["TimeScale"].SetValue(TimeScale);
							myEffect.Parameters["Horizontal"].SetValue(Horizontal);
							myEffect.Parameters["Vertical"].SetValue(Vertical);
					*/
					myEffect.Parameters["World"].SetValue(transforms[myModel.Meshes[i].ParentBone.Index]);
					myEffect.Parameters["WorldIT"].SetValue(Matrix.Invert(Matrix.Transpose(transforms[myModel.Meshes[i].ParentBone.Index])));
					myEffect.Parameters["WorldViewProj"].SetValue(transforms[myModel.Meshes[i].ParentBone.Index] * ViewMatrix * ProjectionMatrix);
				}

				myEffect.Begin();

				foreach (EffectPass pass in myEffect.CurrentTechnique.Passes)
				{
					pass.Begin();
					//Draw the mesh, will use the effects set above.
					foreach (ModelMeshPart part in myModel.Meshes[i].MeshParts)
					{
						this.GraphicsDevice.VertexDeclaration = part.VertexDeclaration;
						this.GraphicsDevice.Vertices[0].SetSource(myModel.Meshes[i].VertexBuffer, part.StreamOffset, part.VertexStride);
						this.GraphicsDevice.Indices = myModel.Meshes[i].IndexBuffer;
						this.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.BaseVertex, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount);
					}
					//myModel.Meshes[i].Draw();
					pass.End();
				}

				myEffect.End();

			}
		}

		public override void Update(GameTime gameTime)
		{
			//base.Update(gameTime);

			//modelRotation += (float)gameTime.ElapsedGameTime.TotalMilliseconds * (MathHelper.ToRadians(0.02f) * r.Next(1, 2));

			//float f = 0.002f;
			/*
			float f = 5f;

			KeyboardState k = Keyboard.GetState();
			
			if (k.IsKeyDown(Keys.Right))
				cameraPosition.X += f;
			if (k.IsKeyDown(Keys.Left))
				cameraPosition.X -= f;
			if (k.IsKeyDown(Keys.Up))
				cameraPosition.Z += f;
			if (k.IsKeyDown(Keys.Down))
				cameraPosition.Z -= f;
			if (k.IsKeyDown(Keys.PageUp))
				cameraPosition.Y += f;
			if (k.IsKeyDown(Keys.PageDown))
				cameraPosition.Y -= f;


			if (k.IsKeyDown(Keys.NumPad6))
				cameraTagetPosition.X += f;
			if (k.IsKeyDown(Keys.NumPad4))
				cameraTagetPosition.X -= f;
			if (k.IsKeyDown(Keys.NumPad8))
				cameraTagetPosition.Z += f;
			if (k.IsKeyDown(Keys.NumPad2))
				cameraTagetPosition.Z -= f;
			if (k.IsKeyDown(Keys.NumPad9))
				cameraTagetPosition.Y += f;
			if (k.IsKeyDown(Keys.NumPad3))
				cameraTagetPosition.Y -= f;
			
			/*
			if (k.IsKeyDown(Keys.Up))
				Horizontal += f;

			if (k.IsKeyDown(Keys.Down))
				Horizontal -= f;

			if (k.IsKeyDown(Keys.Right))
				Vertical -= f;

			if (k.IsKeyDown(Keys.Left))
				Vertical += f;

			if (k.IsKeyDown(Keys.Add))
				TimeScale += f;

			if (k.IsKeyDown(Keys.Subtract))
				TimeScale -= f;

			Console.WriteLine("TimeScale  : " + TimeScale);
			Console.WriteLine("Horizontal : " + Horizontal);
			Console.WriteLine("Vertical   : " + Vertical);
			*/
		}
	}
}
