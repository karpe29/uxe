using System;
using System.Collections.Generic;
using System.Text;
using XeFramework.GUI;
using XeFramework.GameScreen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XeFramework.XeGame.SpaceRace;
using XeFramework.Graphics3D;

namespace Xe.Game
{
	class ShipSelectionScreen : IGameScreen
	{
		Button buttonBack;
		Button buttonAccept;
		Label labelPlayer;

		Slider sliderShip;

		SpaceRaceScreen m_spaceRaceScreen;

		int m_player;
		float angle = 0f;

		string[] m_ships = { @"Content\Models\StarChaser1", @"Content\Models\StarChaser2", @"Content\Models\StarChaser3", @"Content\Models\StarChaser4" };

		float dst = 2000;

		BasicModel3D m_model;

		private Matrix ViewMatrix = Matrix.CreateLookAt(new Vector3(0,1000,0), new Vector3(1500,0,0), Vector3.Up);
		private Matrix ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1.0f, 1, 10000);
		
		public ShipSelectionScreen(GameScreenManager gameScreenManager, int player, SpaceRaceScreen spaceRaceScreen)
			: base(gameScreenManager, true)
		{
			m_spaceRaceScreen = spaceRaceScreen;
			m_player = player;

			labelPlayer = new Label(GameScreenManager.Game, GameScreenManager.GuiManager);
			labelPlayer.TextAlign = TextAlignment.Center;
			labelPlayer.Text = "Player " + m_player.ToString() + " Ship";
			labelPlayer.Width = 120;
			labelPlayer.Height = 30;
			labelPlayer.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth / 4 - labelPlayer.Width / 2;
			labelPlayer.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 1 / 4 - labelPlayer.Height / 2;
			GameScreenManager.GuiManager.AddControl(labelPlayer);

			buttonBack = new Button(GameScreenManager.Game, GameScreenManager.GuiManager);
			buttonBack.Text = "Back";
			buttonBack.Width = 120;
			buttonBack.Height = 30;
			buttonBack.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth / 4 - buttonBack.Width / 2;
			buttonBack.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - buttonBack.Height / 2;
			buttonBack.Click += new ClickHandler(buttonBack_Click);
			GameScreenManager.GuiManager.AddControl(buttonBack);

			buttonAccept = new Button(GameScreenManager.Game, GameScreenManager.GuiManager);
			buttonAccept.Text = "Accept";
			buttonAccept.Width = 120;
			buttonAccept.Height = 30;
			buttonAccept.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth * 3 / 4 - buttonAccept.Width / 2;
			buttonAccept.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - buttonAccept.Height / 2;
			buttonAccept.Click += new ClickHandler(buttonAccept_Click);
			GameScreenManager.GuiManager.AddControl(buttonAccept);

			sliderShip = new Slider(GameScreenManager.Game, GameScreenManager.GuiManager);
			sliderShip.Width = (buttonAccept.X - buttonBack.X - buttonBack.Width) * 4 / 5;
			sliderShip.Height = 30;
			sliderShip.X = buttonBack.X + buttonBack.Width + ((buttonAccept.X - buttonBack.X - buttonBack.Width) * 1 / 10);
			sliderShip.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - sliderShip.Height / 2;
			sliderShip.MinValue = 0;
			sliderShip.MaxValue = 3;
			sliderShip.Step = 1;
			sliderShip.Value = 0;
			sliderShip.ValueChanged += new ValueChangedHandler(sliderShip_ValueChanged);
			GameScreenManager.GuiManager.AddControl(sliderShip);

			m_model = new BasicModel3D(this.GameScreenManager, m_ships[(int)sliderShip.Value]);
		}

		void sliderShip_ValueChanged(object sender, float value)
		{
			m_model.AssetName = m_ships[(int)value];
		}

		void buttonAccept_Click(object sender, XeFramework.Input.MouseEventArgs args)
		{
			ExitScreen();

			this.GameScreenManager.CurrentGameScreen.Visible = true;

			if (GameScreenManager.CurrentGameScreen.GetType() == typeof(SpaceRaceScreen))
				GameScreenManager.RemoveLeftGameScreen(MainMenuScreen.BackgroundScreenType);
		}
		

		void buttonBack_Click(object sender, XeFramework.Input.MouseEventArgs args)
		{
			ExitScreen();

			this.GameScreenManager.RemoveLeftGameScreen(this.GetType());
			
			this.GameScreenManager.RemoveLeftGameScreen(typeof(SpaceRaceScreen));

			LevelSelectionScreen l = new LevelSelectionScreen(this.GameScreenManager);
		}

		protected override void LoadGraphicsContent(bool loadAllContent)
		{
			base.LoadGraphicsContent(loadAllContent);

			if (loadAllContent)
			{
			}
			else
			{
				labelPlayer.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth / 4 - labelPlayer.Width / 2;
				labelPlayer.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 1 / 4 - labelPlayer.Height / 2;

				buttonBack.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth / 4 - buttonBack.Width / 2;
				buttonBack.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - buttonBack.Height / 2;

				buttonAccept.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth * 3 / 4 - buttonAccept.Width / 2;
				buttonAccept.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - buttonAccept.Height / 2;

				sliderShip.Width = (buttonAccept.X - buttonBack.X - buttonBack.Width) * 4 / 5;
				sliderShip.X = buttonBack.X + buttonBack.Width + ((buttonAccept.X - buttonBack.X - buttonBack.Width) * 1 / 10);
				sliderShip.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - sliderShip.Height / 2;
			}
					
			//if (sliderShip != null)
			//{
			//    m_selectedShip = m_ships[(int)sliderShip.Value];
			//    //m_selectedModel = m_shipModels[(int)sliderShip.Value];
			//}
			//else
			//{
			//    m_selectedShip = m_ships[0];
			//    //m_selectedModel = m_shipModels[0];
			//}
		}



		protected override void Cleanup()
		{
			GameScreenManager.GuiManager.RemoveControl(labelPlayer);
			labelPlayer.Dispose();

			GameScreenManager.GuiManager.RemoveControl(buttonBack);
			buttonBack.Dispose();

			GameScreenManager.GuiManager.RemoveControl(buttonAccept);
			buttonAccept.Dispose();

			GameScreenManager.GuiManager.RemoveControl(sliderShip);
			sliderShip.Dispose();
		}


		#region IGameScreen Members

		public override bool IsBlockingUpdate { get { return false; } }

		public override bool IsBlockingDraw { get { return false; } }

		public override void Update(GameTime gameTime)
		{
			
			base.Update(gameTime);

			labelPlayer.Visible = this.Visible;
			buttonAccept.Visible = this.Visible;
			buttonBack.Visible = this.Visible;
			sliderShip.Visible = this.Visible;

			angle += (float)gameTime.ElapsedGameTime.Milliseconds/1000f;
			angle %= MathHelper.TwoPi;
			m_model.World = Matrix.CreateRotationY(angle)*Matrix.CreateTranslation(dst, 0, 0);

    	}

		public override void Draw(GameTime gameTime)
		{
			if (!this.Visible)
				return;

			


			m_model.View = this.ViewMatrix;
			m_model.Projection = this.ProjectionMatrix;

			m_model.Draw(gameTime);

			base.Draw(gameTime);
		}

		#endregion
	}
}
