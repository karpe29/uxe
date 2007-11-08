using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

using Xe.Gui;
using Microsoft.Xna.Framework.Input;
using Xe.SpaceRace;
using Xe.TimeTime;
using Xe.Tools;
using Xe.Input;

namespace Xe.GameScreen
{
	class LevelSelectionScreen : IGameScreen
	{
		Button buttonBack;
		Button buttonPlaySpaceRace;
		Button buttonPlayTimeTime;

		SliderString sliderLevel;
		SliderString sliderPlayerCount;


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
			XeGame.GuiManager.Controls.Add(buttonBack);

			buttonPlaySpaceRace = new Button(GameScreenManager.Game, XeGame.GuiManager);
			buttonPlaySpaceRace.Text = "Play SpaceRace";
			buttonPlaySpaceRace.Width = 120;
			buttonPlaySpaceRace.Height = 30;
			buttonPlaySpaceRace.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth * 3 / 4 - buttonPlaySpaceRace.Width / 2;
			buttonPlaySpaceRace.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - buttonPlaySpaceRace.Height / 2;
			buttonPlaySpaceRace.Click += new ClickHandler(buttonPlaySpaceRace_Click);
			XeGame.GuiManager.Controls.Add(buttonPlaySpaceRace);

			buttonPlayTimeTime = new Button(GameScreenManager.Game, XeGame.GuiManager);
			buttonPlayTimeTime.Text = "Play TimeTime";
			buttonPlayTimeTime.Width = 120;
			buttonPlayTimeTime.Height = 30;
			buttonPlayTimeTime.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth * 3 / 4 - buttonPlayTimeTime.Width / 2;
			buttonPlayTimeTime.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 4 / 5 - buttonPlayTimeTime.Height / 2;
			buttonPlayTimeTime.Click += new ClickHandler(buttonPlayTimeTime_Click);
			XeGame.GuiManager.Controls.Add(buttonPlayTimeTime);

			sliderLevel = new SliderString(GameScreenManager.Game, XeGame.GuiManager);
			sliderLevel.Width = (int)((buttonPlaySpaceRace.X - buttonBack.X - buttonBack.Width) * 4 / 5);
			sliderLevel.Height = 30;
			sliderLevel.X = (int)(buttonBack.X + buttonBack.Width + ((buttonPlaySpaceRace.X - buttonBack.X - buttonBack.Width) * 1 / 10));
			sliderLevel.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - sliderLevel.Height / 2;
			sliderLevel.Strings = new List<string>();
			sliderLevel.Strings.Add("Easy");
			sliderLevel.Strings.Add("Medium");
			sliderLevel.Strings.Add("Hard");
			sliderLevel.Strings.Add("GOD");
			sliderLevel.Loopable = true;
			sliderLevel.Index = 0;
			XeGame.GuiManager.Controls.Add(sliderLevel);

			sliderPlayerCount = new SliderString(GameScreenManager.Game, XeGame.GuiManager);
			sliderPlayerCount.Width = (int)((buttonPlaySpaceRace.X - buttonBack.X - buttonBack.Width) * 4 / 5);
			sliderPlayerCount.Height = 30;
			sliderPlayerCount.X = (int)(buttonBack.X + buttonBack.Width + ((buttonPlaySpaceRace.X - buttonBack.X - buttonBack.Width) * 1 / 10));
			sliderPlayerCount.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 4 / 5 - sliderLevel.Height / 2;
			sliderPlayerCount.Strings = new List<string>();
			sliderPlayerCount.Strings.Add("1 Player");
			sliderPlayerCount.Strings.Add("2 Players");
			sliderPlayerCount.Strings.Add("3 Players");
			sliderPlayerCount.Strings.Add("4 Players");
			sliderPlayerCount.Loopable = true;
			sliderPlayerCount.Index = 0; 
			XeGame.GuiManager.Controls.Add(sliderPlayerCount);

		}

		void buttonPlaySpaceRace_Click(object sender, MouseEventArgs args)
		{
			// save difficulty level as levelSlider won't exists after Cleanup();
			SpaceRaceInitDatas datas = new SpaceRaceInitDatas(sliderLevel.Index/sliderLevel.Strings.Count, sliderPlayerCount.Index + 1);
		
			ExitScreen();

			//ShipSelectionScreen screen = new ShipSelectionScreen(this.GameScreenManager, datas);
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

				sliderLevel.Width = (int)((buttonPlaySpaceRace.X - buttonBack.X - buttonBack.Width) * 4 / 5);
				sliderLevel.X = (int)(buttonBack.X + buttonBack.Width + ((buttonPlaySpaceRace.X - buttonBack.X - buttonBack.Width) * 1 / 10));
				sliderLevel.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - sliderLevel.Height / 2;

				sliderPlayerCount.Width = (int)((buttonPlaySpaceRace.X - buttonBack.X - buttonBack.Width) * 4 / 5);
				sliderPlayerCount.X = (int)(buttonBack.X + buttonBack.Width + ((buttonPlaySpaceRace.X - buttonBack.X - buttonBack.Width) * 1 / 10));
				sliderPlayerCount.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 4 / 5 - sliderLevel.Height / 2;
			}
		}



		protected override void Cleanup()
		{
			XeGame.GuiManager.Controls.Remove(buttonBack);
			buttonBack.Dispose();

			XeGame.GuiManager.Controls.Remove(buttonPlaySpaceRace);
			buttonPlaySpaceRace.Dispose();

			XeGame.GuiManager.Controls.Remove(buttonPlayTimeTime);
			buttonPlayTimeTime.Dispose();

			XeGame.GuiManager.Controls.Remove(sliderLevel);
			sliderLevel.Dispose();

			XeGame.GuiManager.Controls.Remove(sliderPlayerCount);
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
