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
	class MainBackgroundScreen2 : IGameScreen
	{
		Model myModel;
		Effect myEffect;
		Texture2D myTexture;

		//Position of the model in world space
		Vector3 modelPosition = new Vector3(0, 0, 0);
		float modelRotation = 0.0f;

		//Position of the Camera in world space, for our view matrix
		Vector3 cameraPosition = new Vector3(0, 0, 8690);
		Vector3 cameraTagetPosition = new Vector3(0, 0, 0);

		private Matrix projectionMatrix;
		private float aspectRatio;

		Random r = new Random();

		SpriteBatch sb;

		RenderTarget2D t;


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

		//private float TimeScale = 0.006f, Horizontal = 0.75f, Vertical = 0.75f;

		public MainBackgroundScreen2(GameScreenManager gameScreenManager)
			: base(gameScreenManager, true)
		{
		}

		protected override void LoadGraphicsContent(bool loadAllContent)
		{
			base.LoadGraphicsContent(loadAllContent);

			myModel = GameScreenManager.ContentManager.Load<Model>(@"Content\Models\MenuTunnel");

			myTexture = GameScreenManager.ContentManager.Load<Texture2D>(@"Content\Textures\FireGrade");

			myEffect = GameScreenManager.ContentManager.Load<Effect>(@"Content\Effects\vbomb");
			myEffect.Parameters["GradeTex"].SetValue(myTexture);
			myEffect.Parameters["Speed"].SetValue(0.3f);
			foreach (EffectParameter p in myEffect.Parameters)
				Console.WriteLine(p.Name);

			sb = new SpriteBatch(this.GraphicsDevice);

			t = new RenderTarget2D(this.GraphicsDevice,
				this.GraphicsDevice.PresentationParameters.BackBufferWidth,
				this.GraphicsDevice.PresentationParameters.BackBufferHeight, 1,
				this.GraphicsDevice.PresentationParameters.BackBufferFormat,
				this.GraphicsDevice.DepthStencilBuffer.MultiSampleType,
				this.GraphicsDevice.DepthStencilBuffer.MultiSampleQuality);

			//Aspect ratio to use for the projection matrix
			aspectRatio = aspectRatio = (float)this.GraphicsDevice.PresentationParameters.BackBufferWidth / (float)this.GraphicsDevice.PresentationParameters.BackBufferHeight;

			ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 500000.0f);
		}

		#region IGameScreen Members

		public override bool IsBlockingUpdate { get { return false; } }

		public override bool IsBlockingDraw { get { return false; } }

		#endregion

		public override void Draw(GameTime gameTime)
		{
			GameScreenManager.Stats.AddModelPolygonsCount(myModel);

			
			

			this.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
			this.GraphicsDevice.RenderState.TwoSidedStencilMode = true;

			this.GraphicsDevice.RenderState.DepthBufferEnable = true;
			this.GraphicsDevice.RenderState.AlphaBlendEnable = false;
			this.GraphicsDevice.RenderState.AlphaTestEnable = false;

			this.GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
			this.GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;

			this.GraphicsDevice.SetRenderTarget(0, t);

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


					myEffect.Parameters["Timer"].SetValue((float)(gameTime.TotalGameTime.TotalSeconds));
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
			
			this.GraphicsDevice.ResolveRenderTarget(0);

			this.GraphicsDevice.SetRenderTarget(0, null);


			// Don't use or write to the z buffer
			//this.GraphicsDevice.RenderState.DepthBufferEnable = false;
			//this.GraphicsDevice.RenderState.DepthBufferWriteEnable = false;
			// Disable alpha for the first pass
			//this.GraphicsDevice.RenderState.AlphaBlendEnable = false;

			//t.GetTexture().Save("C:\\test.jpg", ImageFileFormat.Bmp);
			
			sb.Begin(SpriteBlendMode.AlphaBlend,
							  SpriteSortMode.Immediate,
							  SaveStateMode.SaveState);

			sb.Draw(t.GetTexture(), new Rectangle(0,0,t.Width,t.Height), Color.White);

			//sb.Draw(t.GetTexture(), new Rectangle(300, 300, 200, 200), Color.White);

			sb.End();
			
			base.Draw(gameTime);
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

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

