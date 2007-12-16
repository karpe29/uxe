//
// http://blogs.msdn.com/etayrien/archive/2006/12/12/game-engine-structure.aspx
//

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using System.Collections;

using Xe.Gui;
using Xe;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Xe.Tools;

namespace Xe.GameScreen
{
	public class GameScreenManager : DrawableGameComponent
	{
		#region Variables
		private SpriteBatch m_spriteBatch;
		private Texture2D m_blankTexture;

		private List<IGameScreen> m_activeGameScreens = new List<IGameScreen>();

		private List<IGameScreen> m_toDrawGameScreens = new List<IGameScreen>();
		private List<IGameScreen> m_toUpdateGameScreens = new List<IGameScreen>();
		#endregion

		#region Properties

		//#pragma warning disable 108
		public new Microsoft.Xna.Framework.Game Game
		{
			get
			{
				return base.Game;
			}
		}
		//#pragma warning restore 108

		#endregion

		public GameScreenManager(Microsoft.Xna.Framework.Game game)
			: base(game)
		{
			ServiceHelper.Add<GameScreenManager>(this);
		}
		
		public IGameScreen CurrentGameScreen
		{
			get
			{
				return m_activeGameScreens[m_activeGameScreens.Count-1];
			}
		}

		#region Add/Remove
		public bool AddGameScreen(IGameScreen gameScreen)
		{
			m_activeGameScreens.Add(gameScreen);

			CurrentGameScreen.Initialize();

			BuildGameScreenLists();
			return true;
		}

		// Remove Specified GameScreen if it is the current one
		// Return it if removed, null otherwise
		public bool RemoveCurrentGameScreen(Type type)
		{
			bool b = false;
			if (CurrentGameScreen.GetType() == type)
			{
				b = m_activeGameScreens.Remove(CurrentGameScreen);
			}

			BuildGameScreenLists();
			return b;
		}

		public void RemoveLeftGameScreen(Type type)
		{
			List<IGameScreen> tmpList = new List<IGameScreen>(m_activeGameScreens);

			foreach (IGameScreen thisGameScreen in tmpList)
			{
				if (thisGameScreen.GetType() == type)
					m_activeGameScreens.Remove(thisGameScreen);
			}
		}

		public void FadeBackBufferToBlack(int alpha)
		{
			Viewport viewport = GraphicsDevice.Viewport;

			m_spriteBatch.Begin();

			m_spriteBatch.Draw(m_blankTexture,
							 new Rectangle(0, 0, viewport.Width, viewport.Height),
							 new Color(0, 0, 0, (byte)alpha));

			m_spriteBatch.End();
		}
		#endregion

		private void BuildGameScreenLists()
		{
			bool updateBlocked = false;
			bool drawBlocked = false;

			m_toUpdateGameScreens.Clear();
			m_toDrawGameScreens.Clear();

			for (int i = 0; i < m_activeGameScreens.Count; i++)
			{
				IGameScreen thisGameScreen = m_activeGameScreens[i] as IGameScreen;

				if (!updateBlocked && thisGameScreen.Enabled)
					m_toUpdateGameScreens.Add(thisGameScreen);

				if (thisGameScreen.IsBlockingUpdate)
					updateBlocked = true;

				if (!drawBlocked && thisGameScreen.Visible)
					m_toDrawGameScreens.Add(thisGameScreen);

				if (thisGameScreen.IsBlockingDraw)
					drawBlocked = true;
			}
		}

		#region GameComponent

		public override void Initialize()
		{
			base.Initialize();

			foreach (IGameScreen s in this.m_activeGameScreens)
			{
				s.Initialize();
			}

			m_spriteBatch = new SpriteBatch(XeGame.Device);
			m_blankTexture = XeGame.ContentManager.Load<Texture2D>(@"Content\Textures\blank");
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			BuildGameScreenLists();

			for (int i = 0; i < m_toUpdateGameScreens.Count; i++)
			{
				m_toUpdateGameScreens[i].Update(gameTime);
			}
		}

		public override void Draw(GameTime gameTime)
		{
			

			//for (int i = m_toDrawGameScreens.Count - 1; i >= 0; i--)
			for (int i = 0; i < m_toDrawGameScreens.Count; i++)
			{
				m_toDrawGameScreens[i].Draw(gameTime);
			}

			base.Draw(gameTime);
		}

		#endregion
	}
}
