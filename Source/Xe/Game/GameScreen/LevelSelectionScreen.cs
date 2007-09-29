using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using Xe.GUI;
using Microsoft.Xna.Framework.Input;
using Xe.SpaceRace;
using Xe.TimeTime;
using Xe.Tools;

namespace Xe.GameScreen
{
	class LevelSelectionScreen : IGameScreen
	{
		Button buttonBack;
		Button buttonPlaySpaceRace;
		Button buttonPlayTimeTime;

		Slider sliderLevel;
		Slider sliderPlayerCount;


		public LevelSelectionScreen(GameScreenManager gameScreenManager)
			: base(gameScreenManager, false)
		{
			// if we don't already have a backgroundScreen, create one
			if (this.GameScreenManager.CurrentGameScreen.GetType() != MainMenuScreen.BackgroundScreenType)
				if (MainMenuScreen.BackgroundScreenType.BaseType == typeof(IGameScreen))
					Activator.CreateInstance(MainMenuScreen.BackgroundScreenType, this.GameScreenManager);

			// add it to the list of GameScreens
			this.GameScreenManager.AddGameScreen(this);

			buttonBack = new Button(GameScreenManager.Game, XeGame.GuiManager);
			buttonBack.Text = "Back";
			buttonBack.Width = 120;
			buttonBack.Height = 30;
			buttonBack.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth / 4 - buttonBack.Width / 2;
			buttonBack.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - buttonBack.Height / 2;
			buttonBack.Click += new ClickHandler(buttonBack_Click);
			XeGame.GuiManager.AddControl(buttonBack);

			buttonPlaySpaceRace = new Button(GameScreenManager.Game, XeGame.GuiManager);
			buttonPlaySpaceRace.Text = "Play SpaceRace";
			buttonPlaySpaceRace.Width = 120;
			buttonPlaySpaceRace.Height = 30;
			buttonPlaySpaceRace.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth * 3 / 4 - buttonPlaySpaceRace.Width / 2;
			buttonPlaySpaceRace.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - buttonPlaySpaceRace.Height / 2;
			buttonPlaySpaceRace.Click += new ClickHandler(buttonPlaySpaceRace_Click);
			XeGame.GuiManager.AddControl(buttonPlaySpaceRace);

			buttonPlayTimeTime = new Button(GameScreenManager.Game, XeGame.GuiManager);
			buttonPlayTimeTime.Text = "Play TimeTime";
			buttonPlayTimeTime.Width = 120;
			buttonPlayTimeTime.Height = 30;
			buttonPlayTimeTime.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth * 3 / 4 - buttonPlayTimeTime.Width / 2;
			buttonPlayTimeTime.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 4 / 5 - buttonPlayTimeTime.Height / 2;
			buttonPlayTimeTime.Click += new ClickHandler(buttonPlayTimeTime_Click);
			XeGame.GuiManager.AddControl(buttonPlayTimeTime);

			sliderLevel = new Slider(GameScreenManager.Game, XeGame.GuiManager);
			sliderLevel.Width = (buttonPlaySpaceRace.X - buttonBack.X - buttonBack.Width) * 4 / 5;
			sliderLevel.Height = 30;
			sliderLevel.X = buttonBack.X + buttonBack.Width + ((buttonPlaySpaceRace.X - buttonBack.X - buttonBack.Width) * 1 / 10);
			sliderLevel.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - sliderLevel.Height / 2;
			sliderLevel.MinValue = 0;
			sliderLevel.MaxValue = 3;
			sliderLevel.Step = 1;
			sliderLevel.Value = 2;
			//sliderLevel.ValueChanged += new ValueChangedHandler(levelSlider_ValueChanged);
			XeGame.GuiManager.AddControl(sliderLevel);

			sliderPlayerCount = new Slider(GameScreenManager.Game, XeGame.GuiManager);
			sliderPlayerCount.Width = (buttonPlaySpaceRace.X - buttonBack.X - buttonBack.Width) * 4 / 5;
			sliderPlayerCount.Height = 30;
			sliderPlayerCount.X = buttonBack.X + buttonBack.Width + ((buttonPlaySpaceRace.X - buttonBack.X - buttonBack.Width) * 1 / 10);
			sliderPlayerCount.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 4 / 5 - sliderLevel.Height / 2;
			sliderPlayerCount.MinValue = 0;
			sliderPlayerCount.MaxValue = 3;
			sliderPlayerCount.Step = 1;
			sliderPlayerCount.Value = 0;
			//sliderPlayerCount.ValueChanged += new ValueChangedHandler(levelPlayerCount_ValueChanged);
			XeGame.GuiManager.AddControl(sliderPlayerCount);

		}

		void buttonPlaySpaceRace_Click(object sender, MouseEventArgs args)
		{
			// save difficulty level as levelSlider won't exists after Cleanup();
			SpaceRaceInitDatas datas = new SpaceRaceInitDatas(sliderLevel.ValuePercent, (int)sliderPlayerCount.Value);
		
			ExitScreen();

			ShipSelectionScreen screen = new ShipSelectionScreen(this.GameScreenManager, datas);
		}

		void buttonPlayTimeTime_Click(object sender, MouseEventArgs args)
		{
			/*
			// save difficulty level as levelSlider won't exists after Cleanup();
			float levelPercent = sliderLevel.ValuePercent;

			ExitScreen();

			GameScreenManager.RemoveLeftGameScreen(MainMenuScreen.BackgroundScreenType);

			TimeTimeScreen g = new TimeTimeScreen(GameScreenManager, levelPercent);
			*/
		}

		void buttonBack_Click(object sender, MouseEventArgs args)
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

				sliderLevel.Width = (buttonPlaySpaceRace.X - buttonBack.X - buttonBack.Width) * 4 / 5;
				sliderLevel.X = buttonBack.X + buttonBack.Width + ((buttonPlaySpaceRace.X - buttonBack.X - buttonBack.Width) * 1 / 10);
				sliderLevel.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - sliderLevel.Height / 2;

				sliderPlayerCount.Width = (buttonPlaySpaceRace.X - buttonBack.X - buttonBack.Width) * 4 / 5;
				sliderPlayerCount.X = buttonBack.X + buttonBack.Width + ((buttonPlaySpaceRace.X - buttonBack.X - buttonBack.Width) * 1 / 10);
				sliderPlayerCount.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 4 / 5 - sliderLevel.Height / 2;
			}
		}



		protected override void Cleanup()
		{
			XeGame.GuiManager.RemoveControl(buttonBack);
			buttonBack.Dispose();

			XeGame.GuiManager.RemoveControl(buttonPlaySpaceRace);
			buttonPlaySpaceRace.Dispose();

			XeGame.GuiManager.RemoveControl(buttonPlayTimeTime);
			buttonPlayTimeTime.Dispose();

			XeGame.GuiManager.RemoveControl(sliderLevel);
			sliderLevel.Dispose();

			XeGame.GuiManager.RemoveControl(sliderPlayerCount);
			sliderPlayerCount.Dispose();
		}


		#region IGameScreen Members

		public override bool IsBlockingUpdate { get { return false; } }

		public override bool IsBlockingDraw { get { return false; } }

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);
		}

		#endregion
	}
}
