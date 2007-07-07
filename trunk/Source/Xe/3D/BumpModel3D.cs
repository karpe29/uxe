#region Using Statements
using System;
using System.Collections.Generic;

using Xe;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Xe.GameScreen;
using Xe.Tools;
#endregion

namespace Xe.Graphics3D
{
	public class BumpModel3D : DrawableGameComponent
	{
		#region Members
		private Model m_model;
		private Effect m_effect;
		private Texture2D m_mapTexture;
		private Texture m_bumpTexture;
		private ContentManager m_conManager;

		private Matrix m_view;
		private Matrix m_projection;
		private Matrix m_world = Matrix.Identity;

		private string m_assetName;
		private bool m_useAsset = false;

		protected IReporterService m_reporter;
		protected Stats m_stats;
		#endregion

		#region Constructors
		private BumpModel3D(GameScreenManager gameScreenManager)
			: base(gameScreenManager.Game)
		{
			m_conManager = gameScreenManager.ContentManager;
		}

		public BumpModel3D(GameScreenManager gameScreenManager, string assetName)
			: base(gameScreenManager.Game)
		{
			m_conManager = gameScreenManager.ContentManager;

			this.AssetName = assetName;

			this.Initialize();
		}
		#endregion

		#region Load / Unload Graphics Content
		protected override void LoadGraphicsContent(bool loadAllContent)
		{
			base.LoadGraphicsContent(loadAllContent);

			if (loadAllContent)
			{
				LoadAsset();
			}
		}

		private void LoadAsset()
		{
			if (m_useAsset)
			{
				if (!String.IsNullOrEmpty(m_assetName))
				{
					m_model = m_conManager.Load<Model>(@"Content\Models\"+m_assetName);


					m_mapTexture = m_conManager.Load<Texture2D>(@"Content\Textures\"+ m_assetName +"_map~0");
					m_bumpTexture = m_conManager.Load<Texture>(@"Content\Textures\"+ m_assetName +"_bump");

					m_effect = m_conManager.Load<Effect>(@"Content\Effects\NormalMapping");

					m_effect.Parameters["LightPosition"].SetValue(new Vector3(200, 0, 200));
					m_effect.Parameters["Texture"].SetValue(m_mapTexture);
					m_effect.Parameters["NormalMap"].SetValue(m_bumpTexture);

					Vector4 lightColor = new Vector4(1, 1, 1, 1);
					Vector4 ambientLightColor = new Vector4(.2f, .2f, .2f, 1);
					float shininess = .3f;
					float specularPower = 4.0f;

					m_effect.Parameters["LightColor"].SetValue(lightColor);
					m_effect.Parameters["AmbientLightColor"].SetValue(ambientLightColor);
					m_effect.Parameters["Shininess"].SetValue(shininess);
					m_effect.Parameters["SpecularPower"].SetValue(specularPower);

					m_effect.CurrentTechnique = m_effect.Techniques[0];
				}
			}
		}

		protected override void UnloadGraphicsContent(bool unloadAllContent)
		{
			base.UnloadGraphicsContent(unloadAllContent);

			if (unloadAllContent)
			{
				/*
				if (m_conManager != null)
				{
					m_conManager.Unload();
					m_conManager.Dispose();
				}
				m_conManager = null;
				*/
				if (m_model != null)
					m_model = null;
			}
		}
		#endregion

		#region Game Component Overrides
		public override void Initialize()
		{
			base.Initialize();

			m_stats = (Stats)this.Game.Services.GetService(typeof(Stats));
		}

		public override void Draw(GameTime gameTime)
		{
			try
			{
				if (m_model != null)
					DrawModel(gameTime);
			}
			catch (Exception e)
			{
				if (m_reporter != null)
					m_reporter.BroadcastError(this, e.Message, e);
			}

			base.Draw(gameTime);
		}

		protected virtual void DrawModel(GameTime gameTime)
		{
			/*
			this.GraphicsDevice.RenderState.FillMode = FillMode.Solid;
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
			
			m_effect.Begin();
			m_effect.Parameters["View"].SetValue(this.m_view);
			m_effect.Parameters["Projection"].SetValue(this.m_projection);

			//Draw the model, a model can have multiple meshes, so loop
			for (int i = 0; i < m_model.Meshes.Count; i++)
			{
				//This is where the mesh orientation is set, as well as our camera and projection
				for (int j = 0; j < m_model.Meshes[i].Effects.Count; j++)
				{
				    m_effect.Parameters["World"].SetValue(transforms[m_model.Meshes[0].ParentBone.Index] * this.m_world);
				}
				
				foreach (EffectPass pass in m_effect.CurrentTechnique.Passes)
				{
					pass.Begin();

					//Draw the mesh, will use the effects set above.
					foreach (ModelMeshPart part in m_model.Meshes[i].MeshParts)
					{
						this.GraphicsDevice.VertexDeclaration = part.VertexDeclaration;
						this.GraphicsDevice.Vertices[0].SetSource(m_model.Meshes[i].VertexBuffer, part.StreamOffset, part.VertexStride);
						this.GraphicsDevice.Indices = m_model.Meshes[i].IndexBuffer;
						this.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, part.BaseVertex, 0, part.NumVertices, part.StartIndex, part.PrimitiveCount);
					}

					pass.End();
				}
			}
			m_effect.End();
		}
		#endregion

		#region Properties
		public Matrix View
		{
			get
			{
				return m_view;
			}
			set
			{
				m_view = value;
			}
		}

		public Matrix Projection
		{
			get
			{
				return m_projection;
			}
			set
			{
				m_projection = value;
			}
		}

		public Matrix World
		{
			get
			{
				return m_world;
			}
			set
			{
				m_world = value;
			}
		}

		public Model Model
		{
			get
			{
				return m_model;
			}
			set
			{
				if (value != m_model)
				{
					m_useAsset = false;
					m_model = value;
				}
			}
		}

		public string AssetName
		{
			get
			{
				return m_assetName;
			}
			set
			{
				if (value != m_assetName)
				{
					m_useAsset = true;
					m_assetName = value;
					LoadAsset();
				}
			}
		}
		#endregion
	}
}
