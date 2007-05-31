using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using XeFramework.GUI;
using Microsoft.Xna.Framework.Input;

namespace XeFramework.GameScreen
{
	class LevelSelectionScreen : IGameScreen
	{
		XeFramework.GUI.Button buttonBack;
		XeFramework.GUI.Button buttonPlaySpaceRace;
		XeFramework.GUI.Button buttonPlayTimeTime;

		Slider levelSlider;


		public LevelSelectionScreen(GameScreenManager gameScreenManager)
			: base(gameScreenManager, true)
		{
			buttonBack = new Button(GameScreenManager.Game, GameScreenManager.GuiManager);
			buttonBack.Text = "Back";
			buttonBack.Width = 120;
			buttonBack.Height = 30;
			buttonBack.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth / 4 - buttonBack.Width / 2;
			buttonBack.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - buttonBack.Height / 2;
			buttonBack.Click += new ClickHandler(buttonBack_Click);
			GameScreenManager.GuiManager.AddControl(buttonBack);

			buttonPlaySpaceRace = new Button(GameScreenManager.Game, GameScreenManager.GuiManager);
			buttonPlaySpaceRace.Text = "Play SpaceRace";
			buttonPlaySpaceRace.Width = 120;
			buttonPlaySpaceRace.Height = 30;
			buttonPlaySpaceRace.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth * 3 / 4 - buttonPlaySpaceRace.Width / 2;
			buttonPlaySpaceRace.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - buttonPlaySpaceRace.Height / 2;
			buttonPlaySpaceRace.Click += new ClickHandler(buttonPlaySpaceRace_Click);
			GameScreenManager.GuiManager.AddControl(buttonPlaySpaceRace);

			buttonPlayTimeTime = new Button(GameScreenManager.Game, GameScreenManager.GuiManager);
			buttonPlayTimeTime.Text = "Play TimeTime";
			buttonPlayTimeTime.Width = 120;
			buttonPlayTimeTime.Height = 30;
			buttonPlayTimeTime.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth * 3 / 4 - buttonPlayTimeTime.Width / 2;
			buttonPlayTimeTime.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 4 / 5 - buttonPlayTimeTime.Height / 2;
			buttonPlayTimeTime.Click += new ClickHandler(buttonPlayTimeTime_Click);
			GameScreenManager.GuiManager.AddControl(buttonPlayTimeTime);

			levelSlider = new Slider(GameScreenManager.Game, GameScreenManager.GuiManager);
			levelSlider.Width = (buttonPlaySpaceRace.X - buttonBack.X - buttonBack.Width) * 4 / 5;
			levelSlider.Height = 30;
			levelSlider.X = buttonBack.X + buttonBack.Width + ((buttonPlaySpaceRace.X - buttonBack.X - buttonBack.Width) * 1 / 10);
			levelSlider.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - levelSlider.Height / 2;
			levelSlider.MinValue = 0;
			levelSlider.MaxValue = 4;
			levelSlider.Step = 1;
			levelSlider.Value = 2;
			levelSlider.ValueChanged += new ValueChangedHandler(levelSlider_ValueChanged);
			GameScreenManager.GuiManager.AddControl(levelSlider);
		}

		void levelSlider_ValueChanged(object sender, float value)
		{
			//this.GameScreenManager.Game.Window.Title = value.ToString();
		}

		void buttonPlaySpaceRace_Click(object sender, XeFramework.Input.MouseEventArgs args)
		{
			// save difficulty level as levelSlider won't exists after Cleanup();
			float levelPercent = levelSlider.ValuePercent;

			ExitScreen();

			GameScreenManager.RemoveLeftGameScreen(MainMenuScreen.BackgroundScreenType);

			SpaceRaceScreen g = new SpaceRaceScreen(GameScreenManager, levelPercent);
		}

		void buttonPlayTimeTime_Click(object sender, XeFramework.Input.MouseEventArgs args)
		{
			// save difficulty level as levelSlider won't exists after Cleanup();
			float levelPercent = levelSlider.ValuePercent;

			ExitScreen();

			GameScreenManager.RemoveLeftGameScreen(MainMenuScreen.BackgroundScreenType);

			TimeTimeScreen g = new TimeTimeScreen(GameScreenManager, levelPercent);
		}

		void buttonBack_Click(object sender, XeFramework.Input.MouseEventArgs args)
		{
			ExitScreen();

			MainMenuScreen s = new MainMenuScreen(GameScreenManager);
		}

		protected override void LoadGraphicsContent(bool loadAllContent)
		{
			base.LoadGraphicsContent(loadAllContent);

			if (loadAllContent)
			{
			}
			else
			{
				buttonBack.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth / 4 - buttonBack.Width / 2;
				buttonBack.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - buttonBack.Height / 2;

				buttonPlaySpaceRace.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth * 3 / 4 - buttonPlaySpaceRace.Width / 2;
				buttonPlaySpaceRace.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - buttonPlaySpaceRace.Height / 2;

				buttonPlayTimeTime.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth * 3 / 4 - buttonPlayTimeTime.Width / 2;
				buttonPlayTimeTime.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 4 / 5 - buttonPlayTimeTime.Height / 2;

				levelSlider.Width = (buttonPlaySpaceRace.X - buttonBack.X - buttonBack.Width) * 4 / 5;
				levelSlider.X = buttonBack.X + buttonBack.Width + ((buttonPlaySpaceRace.X - buttonBack.X - buttonBack.Width) * 1 / 10);
				levelSlider.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - levelSlider.Height / 2;
			}
		}



		protected override void Cleanup()
		{
			GameScreenManager.GuiManager.RemoveControl(buttonBack);
			buttonBack.Dispose();

			GameScreenManager.GuiManager.RemoveControl(buttonPlaySpaceRace);
			buttonPlaySpaceRace.Dispose();

			GameScreenManager.GuiManager.RemoveControl(buttonPlayTimeTime);
			buttonPlayTimeTime.Dispose();

			GameScreenManager.GuiManager.RemoveControl(levelSlider);
			levelSlider.Dispose();
		}


		#region IGameScreen Members

		public override bool IsBlockingUpdate { get { return false; } }

		public override bool IsBlockingDraw { get { return false; } }

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			Button s;

			if (Keyboard.GetState().GetPressedKeys().Length > 0)
			{
				s = new Button(this.GameScreenManager.Game, this.GameScreenManager.GuiManager);
				GameScreenManager.GuiManager.AddControl(s);
			}

		}

		public override void Draw(GameTime gameTime)
		{
			

			if (m_isExiting)
			{
				int d = 255 - (int)TransitionOffTime.TotalMilliseconds / 10;
				this.GameScreenManager.Game.Window.Title = TransitionOffTime.ToString() + " - " + d.ToString(); ;
				GameScreenManager.FadeBackBufferToBlack(d);
			}


			base.Draw(gameTime);

		}

		#endregion
	}
}
