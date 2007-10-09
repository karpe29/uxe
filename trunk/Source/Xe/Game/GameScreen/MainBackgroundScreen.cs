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

		private static int nb_cotes = 72;
		private double ratio;
		private List<tube> les_tubes = new List<tube>();
		private List<cercle> les_cercles = new List<cercle>();
		private Random aleatoire = new Random();
		private Vector3 pos;
		private Matrix orient;
		private int count = 0;
		private double courbe = 0, inc_courbe = 0.0001;
		private Color[] coul = new Color[] { Color.DarkBlue, Color.Black };


		//Position of the model in world space
		Vector3 modelPosition = new Vector3(0, 0, 0);
		float modelRotation = 0.0f;

		//Position of the Camera in world space, for our view matrix
		Vector3 cameraPosition = new Vector3(-200, 0, 0);
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
			double rayon;
			orient = Matrix.Identity;
			for (int i = 0; i <= 200; i++)
			{
				pos = new Vector3(i * 2, 0, 0);
				rayon = 50 - 10 * Math.Log((double)i / 5 + 1);
				les_cercles.Add(new cercle(XeGame.Device, nb_cotes, rayon, pos, orient, coul[i % 2]));
			}
			for (int i = 0; i < 200; i++)
			{
				les_tubes.Add(new tube(XeGame.Device, nb_cotes, les_cercles[i], les_cercles[i + 1]));
			}
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
					for (int k = 0; k < les_tubes.Count; k++)
					{
						les_tubes[k].draw();
					}

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
			long time = (long)(gameTime.ElapsedGameTime.Milliseconds);
			
			/*if (Keyboard.GetState()[Keys.Up] == KeyState.Down)
			{
				Matrix viewMatrix = Matrix.CreateLookAt(new Vector3(100f, 500f, 0f), new Vector3(100f, 0f, 0f), new Vector3(1f, 0, 0));
				myEffect.Parameters["xView"].SetValue(viewMatrix);


			}
			if (Keyboard.GetState()[Keys.Down] == KeyState.Down)
			{
				Matrix viewMatrix = Matrix.CreateLookAt(new Vector3(-75f, 10f, -10f), new Vector3(100f, 0f, 0f), new Vector3(0, 1f, 0));
				myEffect.Parameters["xView"].SetValue(viewMatrix);
			}*/

			
			//courbe += inc_courbe;
			//if (Math.Abs(courbe) >= 0.005) inc_courbe = -inc_courbe;
			Matrix rot;
			courbe += time * 0.003;
			Vector3 pos = new Vector3(0, 0, 0),
				rot_axis = Vector3.Transform(Vector3.Up, Matrix.CreateRotationX((float)courbe))				;

			for (int i = 0; i < les_cercles.Count; i++)
			{
				//les_cercles[i].vertices[(count + i) % nb_cotes].Color = Color.Black;
				//les_cercles[i].vertices[(count + 3 + i) % nb_cotes].Color = Color.LightGreen;

				//rot = Matrix.CreateRotationY((float)( i * courbe));
				rot = Matrix.CreateFromAxisAngle(rot_axis, (float)(i * 0.004));
				les_cercles[i].deplace(pos, rot);
				pos = pos + Vector3.Transform(new Vector3(2, 0, 0), rot);

			}
			count++;
			count %= nb_cotes;


	


			//la transformatio en elle même

			base.Update(gameTime);
		}

		public class cercle
		{
			private GraphicsDevice graph;
			public int nb_cotes;
			public double rayon;
			public VertexPositionColor[] vertices;
			public Vector3 pos;
			public Matrix orient;
			public Color couleur;

			public cercle(GraphicsDevice l_graph, int l_nb_cotes, double l_rayon, Vector3 l_pos, Matrix l_orient, Color l_couleur)
			{
				graph = l_graph;
				nb_cotes = l_nb_cotes;
				rayon = l_rayon;
				pos = l_pos;
				orient = l_orient;
				couleur = l_couleur;


				vertices = new VertexPositionColor[nb_cotes];
				deplace();
				for (int j = 0; j < nb_cotes; j++)
				{
					vertices[j].Color = couleur;
				}
			}

			public void deplace(Vector3 l_pos, Matrix l_orient)
			{
				pos = l_pos;
				orient = l_orient;
				deplace();
			}

			public void deplace()
			{
				Vector3 point_cercle;
				double angle;

				for (int j = 0; j < nb_cotes; j++)
				{
					angle = Math.PI * 2 * j / nb_cotes;
					point_cercle.X = (float)0;
					point_cercle.Y = (float)(rayon * Math.Sin(angle));
					point_cercle.Z = (float)(rayon * Math.Cos(angle));
					vertices[j].Position = pos + Vector3.Transform(point_cercle, orient);
				}
			}



		}


		public class tube
		{
			private GraphicsDevice graph;
			private IndexBuffer indexBuffer;
			private VertexBuffer vertexBuffer;
			private int nb_cotes;
			public cercle cercle_debut, cercle_fin;
			public VertexPositionColor[] vertices;



			public tube(GraphicsDevice l_graph, int l_nb_cotes, cercle l_cercle_debut, cercle l_cercle_fin)
			{
				graph = l_graph;
				nb_cotes = l_nb_cotes;
				cercle_debut = l_cercle_debut;
				cercle_fin = l_cercle_fin;
				short[] indices = new short[nb_cotes * 6];
				for (int i = 0; i < nb_cotes; i++)
				{
					indices[i * 6] = indices[i * 6 + 3] = (short)(i);
					indices[i * 6 + 1] = (short)((i + 1) % nb_cotes);
					indices[i * 6 + 2] = indices[i * 6 + 5] = (short)(nb_cotes + (i + 1) % nb_cotes);
					indices[i * 6 + 4] = (short)(nb_cotes + i);
				}

				this.indexBuffer = new IndexBuffer(graph, typeof(short), indices.Length, ResourceUsage.WriteOnly, ResourceManagementMode.Automatic);
				this.indexBuffer.SetData(indices);

				vertices = new VertexPositionColor[nb_cotes * 2];
				this.vertexBuffer = new VertexBuffer(graph, typeof(VertexPositionColor), vertices.Length, ResourceUsage.WriteOnly, ResourceManagementMode.Automatic);



			}


			public void draw()
			{
				//current_couleur=(current_couleur+1)%100;
				/*for (int j = 0; j < nb_cotes; j++)
				{
					vertices[j].Color = coul;
					vertices[j + nb_cotes].Color = coul;
				}*/

				cercle_debut.vertices.CopyTo(vertices, 0);
				cercle_fin.vertices.CopyTo(vertices, nb_cotes);
				this.vertexBuffer.SetData(vertices);

				graph.Vertices[0].SetSource(vertexBuffer, 0, VertexPositionColor.SizeInBytes);
				graph.Indices = this.indexBuffer;
				graph.VertexDeclaration = new VertexDeclaration(graph, VertexPositionColor.VertexElements);
				graph.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, nb_cotes * 2, 0, nb_cotes * 2);
			}

		}

	}
}
