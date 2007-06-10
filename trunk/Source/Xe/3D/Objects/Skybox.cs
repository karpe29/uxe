using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Xe.Objects3D
{
	class SkyBox : DrawableGameComponent
	{

		Texture2D[] textures = new Texture2D[6];
		Effect effect;

		VertexBuffer vertices;
		IndexBuffer indices;
		VertexDeclaration vertexDecl;

		Vector3 vCameraDirection;
		Vector3 vCameraPosition;

		Matrix viewMatrix;
		Matrix projectionMatrix;
		Matrix worldMatrix;


		ContentManager content;

		string m_filepath;

		// will load file filepath + "_front/_top/etc" textures and filepath + "_effect" effect.
		public SkyBox(Microsoft.Xna.Framework.Game g, string filepath)
			: base(g)
		{
			m_filepath = filepath;
		}

		// A definir avant le draw
		public Vector3 CameraDirection
		{
			get { return vCameraDirection; }
			set { vCameraDirection = value; }
		}

		// A definir avant le draw
		public Vector3 CameraPosition
		{
			get { return vCameraPosition; }
			set
			{
				vCameraPosition = value;
				worldMatrix = Matrix.CreateTranslation(vCameraPosition);
			}
		}

		// A definir avant le draw
		public Matrix ViewMatrix
		{
			set { viewMatrix = value; }
		}

		// A definir avant le draw
		public Matrix ProjectionMatrix
		{
			set { projectionMatrix = value; }
		}

		public ContentManager ContentManager
		{
			set { content = value; }
		}

		public override void Initialize()
		{
			base.Initialize();

			textures[0] = content.Load<Texture2D>(m_filepath + "_back");
			textures[1] = content.Load<Texture2D>(m_filepath + "_front");
			textures[2] = content.Load<Texture2D>(m_filepath + "_bottom");
			textures[3] = content.Load<Texture2D>(m_filepath + "_top");
			textures[4] = content.Load<Texture2D>(m_filepath + "_left");
			textures[5] = content.Load<Texture2D>(m_filepath + "_right");

			effect = content.Load<Effect>(m_filepath + "_effect");
			
			IGraphicsDeviceService graphicsService = (IGraphicsDeviceService)
				Game.Services.GetService(typeof(IGraphicsDeviceService));

			vertexDecl = new VertexDeclaration(graphicsService.GraphicsDevice,
				new VertexElement[] {
                    new VertexElement(0,0,VertexElementFormat.Vector3,
                           VertexElementMethod.Default,
                            VertexElementUsage.Position,0),
                    new VertexElement(0,sizeof(float)*3,VertexElementFormat.Vector2,
                           VertexElementMethod.Default,
                            VertexElementUsage.TextureCoordinate,0)});


			vertices = new VertexBuffer(graphicsService.GraphicsDevice,
								typeof(VertexPositionTexture),
								4 * 6,
								ResourceUsage.WriteOnly);

			VertexPositionTexture[] data = new VertexPositionTexture[4 * 6];

			int i = 1024 * 20;
			Vector3 vExtents = new Vector3(i);
			//back
			data[0].Position = new Vector3(vExtents.X, -vExtents.Y, -vExtents.Z);
			data[0].TextureCoordinate.X = 1.0f; data[0].TextureCoordinate.Y = 1.0f;
			data[1].Position = new Vector3(vExtents.X, vExtents.Y, -vExtents.Z);
			data[1].TextureCoordinate.X = 1.0f; data[1].TextureCoordinate.Y = 0.0f;
			data[2].Position = new Vector3(-vExtents.X, vExtents.Y, -vExtents.Z);
			data[2].TextureCoordinate.X = 0.0f; data[2].TextureCoordinate.Y = 0.0f;
			data[3].Position = new Vector3(-vExtents.X, -vExtents.Y, -vExtents.Z);
			data[3].TextureCoordinate.X = 0.0f; data[3].TextureCoordinate.Y = 1.0f;

			//front
			data[4].Position = new Vector3(-vExtents.X, -vExtents.Y, vExtents.Z);
			data[4].TextureCoordinate.X = 1.0f; data[4].TextureCoordinate.Y = 1.0f;
			data[5].Position = new Vector3(-vExtents.X, vExtents.Y, vExtents.Z);
			data[5].TextureCoordinate.X = 1.0f; data[5].TextureCoordinate.Y = 0.0f;
			data[6].Position = new Vector3(vExtents.X, vExtents.Y, vExtents.Z);
			data[6].TextureCoordinate.X = 0.0f; data[6].TextureCoordinate.Y = 0.0f;
			data[7].Position = new Vector3(vExtents.X, -vExtents.Y, vExtents.Z);
			data[7].TextureCoordinate.X = 0.0f; data[7].TextureCoordinate.Y = 1.0f;

			//bottom
			data[8].Position = new Vector3(-vExtents.X, -vExtents.Y, -vExtents.Z);
			data[8].TextureCoordinate.X = 1.0f; data[8].TextureCoordinate.Y = 0.0f;
			data[9].Position = new Vector3(-vExtents.X, -vExtents.Y, vExtents.Z);
			data[9].TextureCoordinate.X = 1.0f; data[9].TextureCoordinate.Y = 1.0f;
			data[10].Position = new Vector3(vExtents.X, -vExtents.Y, vExtents.Z);
			data[10].TextureCoordinate.X = 0.0f; data[10].TextureCoordinate.Y = 1.0f;
			data[11].Position = new Vector3(vExtents.X, -vExtents.Y, -vExtents.Z);
			data[11].TextureCoordinate.X = 0.0f; data[11].TextureCoordinate.Y = 0.0f;

			//top
			data[12].Position = new Vector3(vExtents.X, vExtents.Y, -vExtents.Z);
			data[12].TextureCoordinate.X = 0.0f; data[12].TextureCoordinate.Y = 0.0f;
			data[13].Position = new Vector3(vExtents.X, vExtents.Y, vExtents.Z);
			data[13].TextureCoordinate.X = 0.0f; data[13].TextureCoordinate.Y = 1.0f;
			data[14].Position = new Vector3(-vExtents.X, vExtents.Y, vExtents.Z);
			data[14].TextureCoordinate.X = 1.0f; data[14].TextureCoordinate.Y = 1.0f;
			data[15].Position = new Vector3(-vExtents.X, vExtents.Y, -vExtents.Z);
			data[15].TextureCoordinate.X = 1.0f; data[15].TextureCoordinate.Y = 0.0f;


			//left
			data[16].Position = new Vector3(-vExtents.X, vExtents.Y, -vExtents.Z);
			data[16].TextureCoordinate.X = 1.0f; data[16].TextureCoordinate.Y = 0.0f;
			data[17].Position = new Vector3(-vExtents.X, vExtents.Y, vExtents.Z);
			data[17].TextureCoordinate.X = 0.0f; data[17].TextureCoordinate.Y = 0.0f;
			data[18].Position = new Vector3(-vExtents.X, -vExtents.Y, vExtents.Z);
			data[18].TextureCoordinate.X = 0.0f; data[18].TextureCoordinate.Y = 1.0f;
			data[19].Position = new Vector3(-vExtents.X, -vExtents.Y, -vExtents.Z);
			data[19].TextureCoordinate.X = 1.0f; data[19].TextureCoordinate.Y = 1.0f;

			//right
			data[20].Position = new Vector3(vExtents.X, -vExtents.Y, -vExtents.Z);
			data[20].TextureCoordinate.X = 0.0f; data[20].TextureCoordinate.Y = 1.0f;
			data[21].Position = new Vector3(vExtents.X, -vExtents.Y, vExtents.Z);
			data[21].TextureCoordinate.X = 1.0f; data[21].TextureCoordinate.Y = 1.0f;
			data[22].Position = new Vector3(vExtents.X, vExtents.Y, vExtents.Z);
			data[22].TextureCoordinate.X = 1.0f; data[22].TextureCoordinate.Y = 0.0f;
			data[23].Position = new Vector3(vExtents.X, vExtents.Y, -vExtents.Z);
			data[23].TextureCoordinate.X = 0.0f; data[23].TextureCoordinate.Y = 0.0f;

			vertices.SetData<VertexPositionTexture>(data);


			indices = new IndexBuffer(graphicsService.GraphicsDevice,
								typeof(short), 6 * 6,
								ResourceUsage.WriteOnly);

			short[] ib = new short[6 * 6];

			for (int x = 0; x < 6; x++)
			{
				ib[x * 6 + 0] = (short)(x * 4 + 0);
				ib[x * 6 + 2] = (short)(x * 4 + 1);
				ib[x * 6 + 1] = (short)(x * 4 + 2);

				ib[x * 6 + 3] = (short)(x * 4 + 2);
				ib[x * 6 + 5] = (short)(x * 4 + 3);
				ib[x * 6 + 4] = (short)(x * 4 + 0);
			}

			indices.SetData<short>(ib);

		}

		public override void Draw(GameTime gameTime)
		{
			if (vertices == null)
				return;

			effect.Begin();
			effect.Parameters["worldViewProjection"].SetValue(
							 worldMatrix * Matrix.CreateScale(0.5f) * viewMatrix * projectionMatrix);


			for (int x = 0; x < 6; x++)
			{
				float f = 0;
				switch (x)
				{
					case 0: //back
						f = Vector3.Dot(vCameraDirection, Vector3.Backward);
						break;
					case 1: //front
						f = Vector3.Dot(vCameraDirection, Vector3.Forward);
						break;
					case 2: //bottom
						f = Vector3.Dot(vCameraDirection, Vector3.Down);
						break;
					case 3: //top
						f = Vector3.Dot(vCameraDirection, Vector3.Up);
						break;
					case 4: //left
						f = Vector3.Dot(vCameraDirection, Vector3.Left);
						break;
					case 5: //right
						f = Vector3.Dot(vCameraDirection, Vector3.Right);
						break;
				}

				if (f <= 0)
				{
					IGraphicsDeviceService graphicsService = (IGraphicsDeviceService)Game.Services.GetService(typeof(IGraphicsDeviceService));

					GraphicsDevice device = graphicsService.GraphicsDevice;

					device.VertexDeclaration = vertexDecl;

					device.Vertices[0].SetSource(vertices, 0, vertexDecl.GetVertexStrideSize(0));

					device.Indices = indices;

					effect.Parameters["baseTexture"].SetValue(textures[x]);
					effect.Techniques[0].Passes[0].Begin();

					device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, x * 4, 4, x * 6, 2);
					effect.Techniques[0].Passes[0].End();
				}
			}

			effect.End();
		}

		public new int DrawOrder
		{
			get { return 0; }
		}

		public new bool Visible
		{
			get { return true; }
		}
	}
}
