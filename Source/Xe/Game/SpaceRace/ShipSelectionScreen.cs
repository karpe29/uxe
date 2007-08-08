using System;
using System.Collections.Generic;
using System.Text;
using Xe.GUI;
using Xe.GameScreen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xe.Graphics3D;
using Xe.Tools;

namespace Xe.SpaceRace
{
	class ShipSelectionScreen : IGameScreen
	{
		Button buttonBack;
		Button buttonAccept;
		Label labelPlayer;

		Slider sliderShip;
		SpaceRaceInitDatas m_datas;

		float angle = 0;

		float viewDistance = 120;

		Ship m_ship;

		private Matrix ViewMatrix;
		private Matrix ProjectionMatrix;
		
		public ShipSelectionScreen(GameScreenManager gameScreenManager, SpaceRaceInitDatas datas)
			: base(gameScreenManager, true)
		{
			m_datas = datas;

			labelPlayer = new Label(GameScreenManager.Game, XeGame.GuiManager);
			labelPlayer.TextAlign = TextAlignment.Center;
			labelPlayer.Text = "Player " + (m_datas.CurrentPlayerNumber+1).ToString() + " Ship";
			labelPlayer.Width = 120;
			labelPlayer.Height = 30;
			labelPlayer.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth / 4 - labelPlayer.Width / 2;
			labelPlayer.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 1 / 4 - labelPlayer.Height / 2;
			XeGame.GuiManager.AddControl(labelPlayer);

			buttonBack = new Button(GameScreenManager.Game, XeGame.GuiManager);
			buttonBack.Text = "Back";
			buttonBack.Width = 120;
			buttonBack.Height = 30;
			buttonBack.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth / 4 - buttonBack.Width / 2;
			buttonBack.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - buttonBack.Height / 2;
			buttonBack.Click += new ClickHandler(buttonBack_Click);
			XeGame.GuiManager.AddControl(buttonBack);

			buttonAccept = new Button(GameScreenManager.Game, XeGame.GuiManager);
			buttonAccept.Text = "Accept";
			buttonAccept.Width = 120;
			buttonAccept.Height = 30;
			buttonAccept.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth * 3 / 4 - buttonAccept.Width / 2;
			buttonAccept.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - buttonAccept.Height / 2;
			buttonAccept.Click += new ClickHandler(buttonAccept_Click);
			XeGame.GuiManager.AddControl(buttonAccept);

			sliderShip = new Slider(GameScreenManager.Game, XeGame.GuiManager);
			sliderShip.Width = (buttonAccept.X - buttonBack.X - buttonBack.Width) * 4 / 5;
			sliderShip.Height = 30;
			sliderShip.X = buttonBack.X + buttonBack.Width + ((buttonAccept.X - buttonBack.X - buttonBack.Width) * 1 / 10);
			sliderShip.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - sliderShip.Height / 2;
			sliderShip.MinValue = 0;
			sliderShip.MaxValue = ShipType.Types.Length-1;
			sliderShip.Step = 1;
			sliderShip.Value = 0;
			sliderShip.ValueChanged += new ValueChangedHandler(sliderShip_ValueChanged);
			XeGame.GuiManager.AddControl(sliderShip);

			m_ship = new Ship(this.GameScreenManager, ShipType.Types[(int)sliderShip.Value]);
			
			ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1.0f, 1, viewDistance * 2.0f);
		}

		void sliderShip_ValueChanged(object sender, float value)
		{
			//m_model.AssetName = ShipType.Types[(int)value].ModelAsset;
		}

		void buttonAccept_Click(object sender, MouseEventArgs args)
		{

			ExitScreen();

			m_datas.ShipTypes.Add(ShipType.Types[(int)sliderShip.Value]);

			// dernier joueur ?
			if (m_datas.CurrentPlayerNumber++ < m_datas.TotalPlayerCount )
			{// non
				ShipSelectionScreen s = new ShipSelectionScreen(this.GameScreenManager, m_datas);
			}
			else
			{// oui -> lancement du jeu :)
				GameScreenManager.RemoveLeftGameScreen(MainMenuScreen.BackgroundScreenType);
				SpaceRaceScreen s = new SpaceRaceScreen(this.GameScreenManager, m_datas);
			}
		}
		

		void buttonBack_Click(object sender, MouseEventArgs args)
		{
			ExitScreen();

			this.GameScreenManager.RemoveLeftGameScreen(this.GetType());

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
			XeGame.GuiManager.RemoveControl(labelPlayer);
			labelPlayer.Dispose();

			XeGame.GuiManager.RemoveControl(buttonBack);
			buttonBack.Dispose();

			XeGame.GuiManager.RemoveControl(buttonAccept);
			buttonAccept.Dispose();

			XeGame.GuiManager.RemoveControl(sliderShip);
			sliderShip.Dispose();
		}


		#region IGameScreen Members

		public override bool IsBlockingUpdate { get { return false; } }

		public override bool IsBlockingDraw { get { return false; } }

		public override void Update(GameTime gameTime)
		{
			
			base.Update(gameTime);

			angle += ((float)(gameTime.ElapsedGameTime).Ticks) / 10000000f;

			angle %= MathHelper.TwoPi;

			m_ship.Update(gameTime);
			ViewMatrix = Matrix.CreateLookAt(Vector3.Transform(new Vector3(0, 0, viewDistance), Matrix.CreateRotationY(angle)), new Vector3(0, 0, 0), Vector3.Up);

    	}

		public override void Draw(GameTime gameTime)
		{
			if (!this.Visible)
				return;

			/*XeGame.Stats.AddModelPolygonsCount(m_model.Model);


			m_model.View = this.ViewMatrix;
			m_model.Projection = this.ProjectionMatrix;

			m_model.Draw(gameTime);
			*/

			m_ship.setParticlesView(ViewMatrix);
			m_ship.Draw(gameTime);
			base.Draw(gameTime);

		}

		#endregion
	}
}
