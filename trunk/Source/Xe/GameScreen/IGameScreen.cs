using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

namespace XeFramework.GameScreen
{
	public abstract class IGameScreen : DrawableGameComponent
	{
		public string Name { get { return this.GetType().ToString(); } }

		abstract public bool IsBlockingUpdate { get; }
		abstract public bool IsBlockingDraw { get; }

		public TimeSpan TransitionOnTime = TimeSpan.Zero;
		public TimeSpan TransitionOffTime = TimeSpan.Zero;

		public bool m_isExiting;

		private GameScreenManager m_gameScreenManager;

		public GameScreenManager GameScreenManager
		{ 
			get
			{
				return m_gameScreenManager;
			}
			set
			{
				if (value != null)
					m_gameScreenManager = value;
			}
		}

		public IGameScreen(GameScreenManager gameScreenManager, bool autoAdd)
			: base(gameScreenManager.Game)
		{
			this.GameScreenManager = gameScreenManager;

			if (autoAdd)
				this.GameScreenManager.AddGameScreen(this);
		}

		virtual protected void Cleanup() { }

		virtual public void ExitScreen()
		{
			if (TransitionOffTime <= TimeSpan.Zero)
			{
				// If the screen has a zero transition time, remove it and clean it.
				GameScreenManager.RemoveCurrentGameScreen(this.GetType());
				this.Cleanup();
			}
			else
			{
				// Otherwise flag that it should transition off and then exit.
				m_isExiting = true;
			}
		}

		#region IUpdateable
		public override void Update(GameTime gameTime)
		{
		    if (m_isExiting)
		    {
		        TransitionOffTime -= gameTime.ElapsedGameTime;
				
		        // check if we need to remove it
		        ExitScreen();
		    }
		}
		#endregion
	}
}
