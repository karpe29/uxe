using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using Xe.Gui;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Xe;
using Xe.GameScreen;
using Xe.SpaceRace;

namespace Xe.GameScreen
{
	class MainBackgroundScreen : IGameScreen
	{
		//Model myModel;

		tunnel myTunnel=new tunnel();
	
		
		public Race Race;
		Player player;
		public SpaceRaceInitDatas InitDatas;

		//Position of the model in world space
		Vector3 modelPosition = new Vector3(0, 0, 0);

		//Position of the Camera in world space, for our view matrix
		Vector3 cameraPosition = new Vector3(0, 0, 0);
		float cameraTargetOffset = 100;
		Vector3 cameraTargetPosition = new Vector3(100,0, 0);

		private Matrix projection;
		private Matrix view;
		private Matrix world;
		private float aspectRatio;



		public Matrix Projection
		{
			get
			{
				return projection;
			}
			set
			{
				projection = value;
			}
		}


		public Matrix View
		{
			get
			{
				return view;
			}
			set
			{
				view = value;
				myTunnel.myEffect.Parameters["ViewI"].SetValue(Matrix.Invert(view));
				myTunnel.myEffect.Parameters["WorldViewProj"].SetValue(World * view * Projection);
			}
		}


		public Matrix World
		{
			get
			{
				return world;
			}
			set
			{
				world = value;
				myTunnel.myEffect.Parameters["WorldIT"].SetValue(Matrix.Invert(Matrix.Transpose(world)));
				myTunnel.myEffect.Parameters["World"].SetValue(world);
		}
		}



		public MainBackgroundScreen(GameScreenManager gameScreenManager)
			: base(gameScreenManager, true)
		{
			/*InitDatas = new SpaceRaceInitDatas(1, 0);
			InitDatas.ShipTypes.Add(ShipType.Types[0]);
			InitDatas.GamePadIndexes.Add(0);
			Race = new Race(gameScreenManager, InitDatas.DifficultyPercent);
			player = new Player(gameScreenManager, InitDatas.ShipTypes[0], (PlayerIndex)0, InitDatas.GamePadIndexes[0]);
		
			 */
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

			//myModel = XeGame.ContentManager.Load<Model>(@"Content\Models\MenuTunnel");
			
			//myEffect = new BasicEffect(XeGame.Device,null);
			//((BasicEffect)myEffect).VertexColorEnabled = true;
			/*((BasicEffect)myEffect).LightingEnabled = true;
			((BasicEffect)myEffect).DirectionalLight0.Enabled = true;
			((BasicEffect)myEffect).DirectionalLight0.DiffuseColor = Vector3.One;
			((BasicEffect)myEffect).DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1.0f, -1.0f, -1.0f));
			((BasicEffect)myEffect).DirectionalLight0.SpecularColor = Vector3.One;

			/*myEffect.Parameters["TimeScale"].SetValue(TimeScale);
			myEffect.Parameters["Horizontal"].SetValue(Horizontal);
			myEffect.Parameters["Vertical"].SetValue(Vertical);
			myEffect.Parameters["colorTexture"].SetValue(myTexture);
			myEffect.CurrentTechnique = myEffect.Techniques["Textured"];
			foreach (EffectParameter p in myEffect.Parameters)
				Console.WriteLine(p.Name);
			*/


			//Aspect ratio to use for the projection matrix
			aspectRatio = aspectRatio = (float)this.GraphicsDevice.PresentationParameters.BackBufferWidth / (float)this.GraphicsDevice.PresentationParameters.BackBufferHeight;


			Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 500000.0f);
			World = Matrix.Identity;
			View = Matrix.CreateLookAt(cameraPosition, cameraTargetPosition, Vector3.Up);


			
			/*((BasicEffect)myEffect).World = Matrix.Identity;
			((BasicEffect)myEffect).View = Matrix.CreateLookAt(cameraPosition, cameraTagetPosition, Vector3.Up);
			((BasicEffect)myEffect).Projection= Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f), aspectRatio, 1.0f, 500000.0f);
			 */
		}

		#region IGameScreen Members

		public override bool IsBlockingUpdate { get { return false; } }

		public override bool IsBlockingDraw { get { return false; } }

		#endregion

		public override void Draw(GameTime gameTime)
		{
			//base.Draw(gameTime);

			//XeGame.Stats.AddModelPolygonsCount(myModel);

			this.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
			//this.GraphicsDevice.RenderState.TwoSidedStencilMode = true;


			this.GraphicsDevice.RenderState.DepthBufferEnable = true;
			this.GraphicsDevice.RenderState.AlphaBlendEnable = false;
			this.GraphicsDevice.RenderState.AlphaTestEnable = false;

			this.GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Wrap;
			this.GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Wrap;

			//Copy any parent transforms
			//Matrix[] transforms = new Matrix[myModel.Bones.Count];
			//myModel.CopyAbsoluteBoneTransformsTo(transforms);



				





			//myEffect.Parameters

			//myEffect.Parameters["World"].SetValue(Matrix.Identity);
			//myEffect.Parameters["WorldViewProj"].SetValue(ViewMatrix * ProjectionMatrix);



			//Draw the model, a model can have multiple meshes, so loop
			/*for (int i = 0; i < myModel.Meshes.Count; i++)
			{
				//This is where the mesh orientation is set, as well as our camera and projection
				for (int j = 0; j < myModel.Meshes[i].Effects.Count; j++)
				{
					//(myModel.Meshes[i].Effects[j] as BasicEffect).EnableDefaultLighting();
					(myModel.Meshes[i].Effects[j] as BasicEffect).World =
						transforms[myModel.Meshes[i].ParentBone.Index]
						* Matrix.CreateRotationY(modelRotation)
						* Matrix.CreateTranslation(modelPosition);

					(myModel.Meshes[i].Effects[j] as BasicEffect).View = ViewMatrix;
					(myModel.Meshes[i].Effects[j] as BasicEffect).Projection = ProjectionMatrix;


					//myEffect.Parameters["Timer"].SetValue((float)gameTime.TotalGameTime.TotalSeconds);
					/*
							myEffect.Parameters["TimeScale"].SetValue(TimeScale);
							myEffect.Parameters["Horizontal"].SetValue(Horizontal);
							myEffect.Parameters["Vertical"].SetValue(Vertical);
					


				}

		
				((BasicEffect)myEffect).World = Matrix.Identity;
				((BasicEffect)myEffect).View = ViewMatrix;
				((BasicEffect)myEffect).Projection = projectionMatrix;
			  
			 

			*/



			//player.Draw(gameTime);

			
					//Draw the mesh, will use the effects set above.*/
			myTunnel.Draw(gameTime);
					/*foreach (ModelMeshPart part in myModel.Meshes[i].MeshParts)
					{
						this.GraphicsDevice.VertexDeclaration = part.VertexDeclaration;
						this.GraphicsDevice.Vertices[0].SetSource(myModel.Meshes[i].VertexBuffer, part.StreamOffset, part.VertexStride);
						this.GraphicsDevice.Indices = myModel.Meshes[i].IndexBuffer;
						this.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.BaseVertex, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount);
					}
					//myModel.Meshes[i].Draw();*/
			
		}

		public override void Update(GameTime gameTime)
		{
			long time = (long)(gameTime.ElapsedGameTime.Milliseconds);

			//player.Update(gameTime);
			
			int touche = 0;

			if (Keyboard.GetState()[Keys.Up] == KeyState.Down)
			{
				touche = 1;
			}
			if (Keyboard.GetState()[Keys.Down] == KeyState.Down)
			{
				touche = 2;
			}
			if (Keyboard.GetState()[Keys.Right] == KeyState.Down)
			{
				touche = 3;
			}
			if (Keyboard.GetState()[Keys.Left] == KeyState.Down)
			{
				touche = 4;
			}

			if (touche != 0)
			{
				Vector3 direction = Vector3.Normalize(cameraTargetPosition - cameraPosition);
				switch (touche)
				{
					case 1:
						cameraPosition += time * direction;
						break;
					case 2:
						cameraPosition -= time * direction;
						break;
					case 3:
						direction = Vector3.Transform(direction, Matrix.CreateFromAxisAngle(Vector3.Up, -0.01f));
						break;
					case 4:
						direction = Vector3.Transform(direction, Matrix.CreateFromAxisAngle(Vector3.Up, 0.01f));
						break;
				}

				cameraTargetPosition = cameraPosition + cameraTargetOffset * direction;
				View = Matrix.CreateLookAt(cameraPosition, cameraTargetPosition, Vector3.Up);


			}
			myTunnel.Update(gameTime);
		
			/*
			Projection = player.m_camera.Projection;
			View = player.m_camera.View;
			 */

			



			//myEffect.Parameters["rotationMatrix"].SetValue(Matrix.CreateFromAxisAngle(axe, MathHelper.Pi / (float)tunnel.nb_cercles));
	
			//courbe += inc_courbe;
			//if (Math.Abs(courbe) >= 0.005) inc_courbe = -inc_courbe;
			
/*
			Matrix rot;
			courbe += time * 0.003;
			courbe %= MathHelper.TwoPi;
			Vector3 pos = new Vector3(0, 0, 0),rot_axis;

			for (int i = 0; i < les_cercles.Count; i++)
			{
				//les_cercles[i].vertices[(count + i) % nb_cotes].Color = Color.Black;
				//les_cercles[i].vertices[(count + 3 + i) % nb_cotes].Color = Color.LightGreen;

				//rot = Matrix.CreateRotationY((float)( i * courbe));
				rot_axis = Vector3.Transform(Vector3.Up, Matrix.CreateRotationX((float) (courbe + i * 0.01))) ;
				rot = Matrix.CreateFromAxisAngle(rot_axis, (float)(i * 0.004));
				les_cercles[i].deplace(pos, rot);
				pos = pos + Vector3.Transform(new Vector3(2, 0, 0), rot);

			}
			count++;
			count %= nb_cotes;
			
			*/
	


			//la transformatio en elle même

			base.Update(gameTime);
		}



		public class tunnel
		{
			public Effect myEffect;
			Texture2D myTexture;

			public static int nb_cotes = 60, nb_cercles = 200;
			public static float longueur = 2000,rayon=50;
			private List<tube> les_tubes = new List<tube>();
			private List<cercle> les_cercles = new List<cercle>();
			private Vector3 pos;
			private Matrix orient=Matrix.Identity;
			Random r = new Random();
			float objectiveTime = 0, currentTime = 0,refTime=0, stepTime = 0,objectiveAngle=0,currentAngle=0,stepAngle=0;
			Vector3 objectiveAxe = Vector3.Zero, currentAxe = Vector3.UnitY, stepAxe = Vector3.Zero;

			public tunnel()
			{

				for (int i = 0; i <= nb_cercles; i++)
				{
					pos = new Vector3((float)i * longueur / (float)nb_cercles, 0, 0);
					les_cercles.Add(new cercle(XeGame.Device, nb_cotes, rayon, pos, orient));
				}
				for (int i = 0; i < nb_cercles; i++)
				{
					les_tubes.Add(new tube(XeGame.Device, nb_cotes, les_cercles[i], les_cercles[i + 1]));
				}

				myEffect = XeGame.ContentManager.Load<Effect>(@"Content\Effects\tunnel");

				myEffect.CurrentTechnique = myEffect.Techniques[1];

				myTexture = XeGame.ContentManager.Load<Texture2D>(@"Content\Textures\team");

				myEffect.Parameters["colorTexture"].SetValue(myTexture);

				myEffect.Parameters["LightPos"].SetValue(new Vector3(20, 49.9f, 0));
				myEffect.Parameters["LightColor"].SetValue(new Vector3(1, 0, 0));
				myEffect.Parameters["AmbiColor"].SetValue(new Vector3(.5f, .5f, .5f));
				myEffect.Parameters["SurfColor"].SetValue(new Vector3(1, 1, 1));

				myEffect.Parameters["longueur"].SetValue(longueur);
				myEffect.Parameters["nbCercles"].SetValue(nb_cercles);

				myEffect.Parameters["SpeedBourrelet"].SetValue(200);
				myEffect.Parameters["SpeedTexture"].SetValue(100);



			}

			public void Update(GameTime gameTime)
			{
				currentTime = (float)(gameTime.TotalGameTime.TotalSeconds)-refTime;
				float time=(float)(gameTime.ElapsedGameTime.TotalSeconds);

				if (currentTime >= objectiveTime)
				{
					refTime = (float)(gameTime.TotalGameTime.TotalSeconds);
					objectiveTime = (float)(r.NextDouble() * 7 + 3);
					stepTime = (float)(r.NextDouble() * (objectiveTime - 4) + 2);

					objectiveAngle = (float)r.NextDouble();
					objectiveAxe = Vector3.Transform(Vector3.UnitY, Matrix.CreateFromAxisAngle(Vector3.UnitX, (float)(r.NextDouble() * MathHelper.TwoPi)));

					stepAngle = (objectiveAngle-currentAngle) / stepTime;
					stepAxe = (objectiveAxe-currentAxe) / stepTime;
				}
				else
				{
					if (currentTime >= stepTime)
					{
					}
					else
					{
						currentAngle += stepAngle * time;
						currentAxe += stepAxe * time;
					}
				}

				
				// params a définir pour la flexion du tunnel
				//Vector3 axe = Vector3.Transform(Vector3.UnitY, Matrix.CreateFromAxisAngle(Vector3.UnitX, (float)(gameTime.TotalGameTime.TotalSeconds) / 5));
				//float angle = .7f;



				//currentAxe = Vector3.Normalize(currentAxe);
				Vector4[] PosCentres = new Vector4[nb_cercles + 1];
				Vector3 step = new Vector3(longueur / nb_cercles, 0, 0);
				Vector3 currentPos = Vector3.Zero;
				for (int i = 0; i <= nb_cercles; i++)
				{
					if (i > 0) currentPos += Vector3.Transform(step, Matrix.CreateFromAxisAngle(currentAxe, i*currentAngle / nb_cercles));
					PosCentres[i].X = currentPos.X;
					PosCentres[i].Y = currentPos.Y;
					PosCentres[i].Z = currentPos.Z;
					PosCentres[i].W = 0;
				}

				myEffect.Parameters["Timer"].SetValue(currentTime+refTime);
				myEffect.Parameters["PosCentres"].SetValue(PosCentres);
				myEffect.Parameters["rotationAngle"].SetValue(currentAngle);
				myEffect.Parameters["rotationAxis"].SetValue(currentAxe);
			}

			public void Draw(GameTime gameTime)
			{


				myEffect.Begin();

				foreach (EffectPass pass in myEffect.CurrentTechnique.Passes)
				{
					pass.Begin();

					for (int k = 0; k < les_tubes.Count; k++)
					{
						les_tubes[k].draw();
					}
					pass.End();
				}

				myEffect.End();


			}


			public class cercle
			{
				private GraphicsDevice graph;
				public int nb_cotes;
				public double rayon;
				public VertexPositionTexture[] vertices;
				public Vector3 pos;
				public Matrix orient;

				public cercle(GraphicsDevice l_graph, int l_nb_cotes, double l_rayon, Vector3 l_pos, Matrix l_orient)
				{
					graph = l_graph;
					nb_cotes = l_nb_cotes;
					rayon = l_rayon;
					pos = l_pos;
					orient = l_orient;


					vertices = new VertexPositionTexture[nb_cotes + 1];
					deplace();

					for (int j = 0; j <= nb_cotes; j++)
					{
						vertices[j].TextureCoordinate = new Vector2(1f - ((float)j) / ((float)nb_cotes), l_pos.X / longueur);
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

					for (int j = 0; j <= nb_cotes; j++)
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
				public VertexPositionTexture[] vertices;
				private VertexDeclaration vertexDeclaration;



				public tube(GraphicsDevice l_graph, int l_nb_cotes, cercle l_cercle_debut, cercle l_cercle_fin)
				{
					graph = l_graph;
					nb_cotes = l_nb_cotes;
					cercle_debut = l_cercle_debut;
					cercle_fin = l_cercle_fin;
					short[] indices = new short[nb_cotes * 6];
					for (int i = 0; i < nb_cotes; i++)
					{
						indices[i * 6 + 2] = indices[i * 6 + 3] = (short)(i);
						indices[i * 6 + 1] = (short)((i + 1));
						indices[i * 6] = indices[i * 6 + 5] = (short)(nb_cotes + i + 2);
						indices[i * 6 + 4] = (short)(nb_cotes + 1 + i);
					}

					this.indexBuffer = new IndexBuffer(graph, typeof(short), indices.Length, ResourceUsage.WriteOnly, ResourceManagementMode.Automatic);
					this.indexBuffer.SetData(indices);

					vertices = new VertexPositionTexture[(nb_cotes + 1) * 2];
					this.vertexBuffer = new VertexBuffer(graph, typeof(VertexPositionTexture), vertices.Length, ResourceUsage.WriteOnly, ResourceManagementMode.Automatic);




					cercle_debut.vertices.CopyTo(vertices, 0);
					cercle_fin.vertices.CopyTo(vertices, nb_cotes + 1);

					vertexDeclaration = new VertexDeclaration(graph, VertexPositionTexture.VertexElements);
				}


				public void draw()
				{
					//current_couleur=(current_couleur+1)%100;
					/*for (int j = 0; j < nb_cotes; j++)
					{
						vertices[j].Color = coul;
						vertices[j + nb_cotes].Color = coul;
					}*/

					this.vertexBuffer.SetData(vertices);

					graph.Vertices[0].SetSource(vertexBuffer, 0, VertexPositionTexture.SizeInBytes);
					graph.Indices = this.indexBuffer;
					graph.VertexDeclaration = vertexDeclaration;

					graph.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, nb_cotes * 2, 0, nb_cotes * 2);
				}

			}
		}
	}
}
