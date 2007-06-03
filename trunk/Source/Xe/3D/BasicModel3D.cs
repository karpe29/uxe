#region Using Statements
using System;
using System.Collections.Generic;

using XeFramework;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using XeFramework.GameScreen;
#endregion

namespace XeFramework.Graphics3D
{
	public class BasicModel3D : DrawableGameComponent
	{
		#region Members
		private Model m_model;
		private ContentManager m_conManager;

		private Matrix m_view;
		private Matrix m_projection;
		private Matrix m_world;

		private string m_assetName;
		private bool m_useAsset = false;

		protected IReporterService m_reporter;
		protected Stats m_stats;
		#endregion

		#region Constructors
		public BasicModel3D(GameScreenManager gameScreenManager)
			: base(gameScreenManager.Game)
		{
		}

		public BasicModel3D(GameScreenManager gameScreenManager, string assetName)
			: base(gameScreenManager.Game)
		{
			this.AssetName = assetName;
		}
		#endregion

		#region Load / Unload Graphics Content
		protected override void LoadGraphicsContent(bool loadAllContent)
		{
			base.LoadGraphicsContent(loadAllContent);

			if (loadAllContent)
			{
				if (m_useAsset)
				{
					m_conManager = new ContentManager(this.Game.Services);
					if (!String.IsNullOrEmpty(m_assetName))
					{
						m_model = m_conManager.Load<Model>(m_assetName);
					}
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
			base.Draw(gameTime);

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
		}

		protected virtual void DrawModel(GameTime gameTime)
		{
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
			set
			{
				m_view = value;
			}
		}

		public Matrix Projection
		{
			set
			{
				m_projection = value;
			}
		}

		public Matrix World
		{
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
				m_model = value;

				m_useAsset = false;
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
				m_assetName = value;

				m_useAsset = true;
			}
		}
		#endregion
	}
}
