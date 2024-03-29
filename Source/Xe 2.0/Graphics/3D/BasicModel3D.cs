#region Using Statements
using System;
using System.Collections.Generic;

using Xe;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Xe.GameScreen;
using Xe.Tools;
using Xe.Gui;
#endregion

namespace Xe.Graphics3D
{
	public class BasicModel3D : DrawableGameComponent
	{
		#region Members
		private Model m_model;
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
		public BasicModel3D(GameScreenManager gameScreenManager)
			: base(gameScreenManager.Game)
		{
			m_conManager = XeGame.ContentManager;
		}

		public BasicModel3D(GameScreenManager gameScreenManager, string assetName)
			: base(gameScreenManager.Game)
		{
			m_conManager = XeGame.ContentManager;

			this.AssetName = assetName;
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
					m_model = m_conManager.Load<Model>(m_assetName);
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
			//Draw the model, a model can have multiple meshes, so loop
			for (int i = 0; i < m_model.Meshes.Count; i++)
			{
				//This is where the mesh orientation is set, as well as our camera and projection
				for (int j = 0; j < m_model.Meshes[i].Effects.Count; j++)
				{
					(m_model.Meshes[i].Effects[j] as BasicEffect).EnableDefaultLighting();
					(m_model.Meshes[i].Effects[j] as BasicEffect).World = transforms[m_model.Meshes[i].ParentBone.Index] * this.m_world;

					(m_model.Meshes[i].Effects[j] as BasicEffect).View = this.m_view;
					(m_model.Meshes[i].Effects[j] as BasicEffect).Projection = this.m_projection;
				}

				m_model.Meshes[i].Draw();
			}
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
