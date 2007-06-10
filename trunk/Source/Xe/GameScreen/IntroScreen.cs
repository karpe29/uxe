using System;
using System.Collections.Generic;
using System.Text;
using Scurvy.Media;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace Xe.GameScreen
{
	class IntroScreen : IGameScreen
	{
		Video m_introVideo;
		SpriteBatch m_spriteBatch;

		public IntroScreen(GameScreenManager gameScreenManager)
			: base(gameScreenManager, true)
		{
			// end right now... TODO: not end in demo version
			End();

			this.Initialize();

			m_spriteBatch = new SpriteBatch(this.GraphicsDevice);
			m_introVideo = gameScreenManager.ContentManager.Load<Video>(@"Content\logo");
			m_introVideo.Loop = false;
			m_introVideo.Play();
		}

		public void End()
		{
			if (m_introVideo != null)
				m_introVideo.Stop();

			this.ExitScreen();

			// Start Game Menus
			MainMenuScreen m = new MainMenuScreen(GameScreenManager);
		}

		public override bool IsBlockingUpdate
		{
			get { return false; }
		}

		public override bool IsBlockingDraw
		{
			get { return false; }
		}

		public bool started = false;
		public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
		{
			m_introVideo.Update();
			
			if (m_introVideo.CurrentIndex == 0 && started)
				End();

			if (m_introVideo.CurrentIndex == 0 && !started)
				started = true;
			
			if (Keyboard.GetState().GetPressedKeys().Length > 0)
				End();

			MouseState m = Mouse.GetState();
		
			if (m.LeftButton == ButtonState.Pressed || m.MiddleButton == ButtonState.Pressed || m.RightButton == ButtonState.Pressed ||
				m.XButton1 == ButtonState.Pressed || m.XButton2 == ButtonState.Pressed)
				End();

			base.Update(gameTime);
		}

		public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
		{
			m_spriteBatch.Begin();
			m_spriteBatch.Draw(m_introVideo.CurrentTexture, new Rectangle(0, 0, this.GraphicsDevice.PresentationParameters.BackBufferWidth, this.GraphicsDevice.PresentationParameters.BackBufferHeight), Color.White);
			m_spriteBatch.End();

			base.Draw(gameTime);
		}
	}
}
