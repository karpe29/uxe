using System;
using System.Collections.Generic;
using System.Text;
using XeFramework.GUI;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace XeFramework.GameScreen
{
	class CreditsScreen : IGameScreen
	{
		Label m_labelGameName;
		Label m_labelLoulou;
		Label m_labelJB;
		Label m_labelClick;

		static float sizeRatio = 1.2f;

		public CreditsScreen(GameScreenManager gameScreeManager)
			: base(gameScreeManager, true)
		{
			m_labelGameName = new Label(GameScreenManager.Game,GameScreenManager.GuiManager);
			m_labelGameName.Text = "Xe, A Game proposed by :";
			m_labelGameName.Width = (int)(m_labelGameName.FontWidth * m_labelGameName.Text.Length * sizeRatio);
			m_labelGameName.Height = 30;
			m_labelGameName.TextAlign = TextAlignment.Center;
			m_labelGameName.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - m_labelGameName.Width / 2;
			m_labelGameName.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 10;
			GameScreenManager.GuiManager.AddControl(m_labelGameName);

			m_labelLoulou = new Label(GameScreenManager.Game, GameScreenManager.GuiManager);
			m_labelLoulou.Text = "Paysplat (Tranchand Louis)";
			m_labelLoulou.Width = (int)(m_labelLoulou.FontWidth * m_labelLoulou.Text.Length * sizeRatio);
			m_labelLoulou.Height = 30;
			m_labelLoulou.TextAlign = TextAlignment.Center;
			m_labelLoulou.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - m_labelLoulou.Width / 2;
			m_labelLoulou.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 4 / 10;
			GameScreenManager.GuiManager.AddControl(m_labelLoulou);

			m_labelJB = new Label(GameScreenManager.Game, GameScreenManager.GuiManager);
			m_labelJB.Text = "AnA-l (Riguet J-B)";
			m_labelJB.Width = (int)(m_labelJB.FontWidth * m_labelJB.Text.Length * sizeRatio);
			m_labelJB.Height = 30;
			m_labelJB.TextAlign = TextAlignment.Center;
			m_labelJB.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - m_labelJB.Width / 2;
			m_labelJB.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 5 / 10;
			GameScreenManager.GuiManager.AddControl(m_labelJB);

			m_labelClick = new Label(GameScreenManager.Game, GameScreenManager.GuiManager);
			m_labelClick.Text = "Press anything to quit. Thanks for playing.";
			m_labelClick.Width = (int)(m_labelClick.FontWidth * m_labelClick.Text.Length * sizeRatio);
			m_labelClick.Height = 30;
			m_labelClick.TextAlign = TextAlignment.Center;
			m_labelClick.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - m_labelClick.Width / 2;
			m_labelClick.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 8 / 10;
			GameScreenManager.GuiManager.AddControl(m_labelClick);
		}

		protected override void LoadGraphicsContent(bool loadAllContent)
		{
			base.LoadGraphicsContent(loadAllContent);

			if (loadAllContent)
			{
			}
			else
			{
				m_labelGameName.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - m_labelGameName.Width / 2;
				m_labelGameName.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 10;

				m_labelLoulou.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - m_labelLoulou.Width / 2;
				m_labelLoulou.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 4 / 10;

				m_labelJB.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - m_labelJB.Width / 2;
				m_labelJB.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 5 / 10;

				m_labelClick.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth / 2 - m_labelClick.Width / 2;
				m_labelClick.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 8 / 10;
			}
		}

		private TimeSpan m_delay;

		public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
		{
			base.Update(gameTime);

			m_delay += gameTime.ElapsedGameTime;
			if (m_delay < TimeSpan.FromMilliseconds(500))
				return;

			if (m_delay > TimeSpan.FromMilliseconds(10000))
				GameScreenManager.Game.Exit();

			if (Keyboard.GetState().GetPressedKeys().Length > 0)
				GameScreenManager.Game.Exit();

			MouseState m = Mouse.GetState();

			if (m.LeftButton == ButtonState.Pressed || m.MiddleButton == ButtonState.Pressed || m.RightButton == ButtonState.Pressed ||
				m.XButton1 == ButtonState.Pressed || m.XButton2 == ButtonState.Pressed)
				GameScreenManager.Game.Exit();

			//GamePadState g = GamePad.GetState(Microsoft.Xna.Framework.PlayerIndex.One);
			//if (g.Buttons.A etc etc
		}

		public override bool IsBlockingUpdate
		{
			get { return false; }
		}

		public override bool IsBlockingDraw
		{
			get { return false; }
		}
	}
}
