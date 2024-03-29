using System;
using System.Collections.Generic;
using System.Text;
using Xe.Gui;
using Xe.GameScreen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xe.Graphics3D;
using Xe.Tools;
using Xe.Physics3D;
using Xe.Input;

namespace Xe.SpaceRace
{
	class ShipSelectionScreen : IGameScreen
	{
		Button buttonBack;
		Button buttonAccept;
		Label labelPlayer;

		SliderString sliderShip;
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

			DetermineNextPlayerIndex();

			labelPlayer = new Label(GameScreenManager.Game, XeGame.GuiManager);
			labelPlayer.TextAlign = TextAlign.Center;
			labelPlayer.Text = "Player " + (m_datas.CurrentPlayerNumber).ToString() + " Ship";
			labelPlayer.Width = 120;
			labelPlayer.Height = 30;
			labelPlayer.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth / 4 - labelPlayer.Width / 2;
			labelPlayer.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 1 / 4 - labelPlayer.Height / 2;
			XeGame.GuiManager.Controls.Add(labelPlayer);

			buttonBack = new Button(GameScreenManager.Game, XeGame.GuiManager);
			buttonBack.Text = "Back";
			buttonBack.Width = 120;
			buttonBack.Height = 30;
			buttonBack.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth / 4 - buttonBack.Width / 2;
			buttonBack.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - buttonBack.Height / 2;
			buttonBack.Click += new ClickHandler(buttonBack_Click);
			XeGame.GuiManager.Controls.Add(buttonBack);

			buttonAccept = new Button(GameScreenManager.Game, XeGame.GuiManager);
			buttonAccept.Text = "Accept";
			buttonAccept.Width = 120;
			buttonAccept.Height = 30;
			buttonAccept.X = this.GraphicsDevice.PresentationParameters.BackBufferWidth * 3 / 4 - buttonAccept.Width / 2;
			buttonAccept.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - buttonAccept.Height / 2;
			buttonAccept.Click += new ClickHandler(buttonAccept_Click);
			XeGame.GuiManager.Controls.Add(buttonAccept);
			
			sliderShip = new SliderString(GameScreenManager.Game, XeGame.GuiManager);
			sliderShip.Width = (int) ((buttonAccept.X - buttonBack.X - buttonBack.Width) * 4 / 5);
			sliderShip.Height = 30;
			sliderShip.X = (int) (buttonBack.X + buttonBack.Width + ((buttonAccept.X - buttonBack.X - buttonBack.Width) * 1 / 10));
			sliderShip.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - sliderShip.Height / 2;

			sliderShip.Strings = new List<string>();
			foreach(ShipType type in ShipType.Types)
			{
				sliderShip.Strings.Add(type.Name);
			}
			sliderShip.Index = 0;
			sliderShip.Loopable = true;

			sliderShip.IndexChanged += new SliderString.IndexChangedHandler(sliderShip_IndexChanged);

			XeGame.GuiManager.Controls.Add(sliderShip);

			m_ship = new Ship(this.GameScreenManager, ShipType.Types[sliderShip.Index]);
			m_ship.ratioParticles = 2;

			ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1.0f, 1, viewDistance * 2.0f);
		}

		void sliderShip_IndexChanged(EventArgs e)
		{
			m_ship = new Ship(this.GameScreenManager, ShipType.Types[(int)sliderShip.Index]);
			m_ship.ratioParticles = 2;

			
		}

		private void DetermineNextPlayerIndex()
		{
			m_datas.GamePadIndexes.Add(-1);

			// seek for a non taken gamepad slot
			for (int i = (int)PlayerIndex.Four /* max player index value*/; i > 0 ; i--)
			{
				if (InputHelper.GamePad[i].IsConnected == false)
				{
					continue;
				}
				else
				{
					if (!m_datas.GamePadIndexes.Contains(i))
						m_datas.GamePadIndexes[m_datas.CurrentPlayerNumber -1] = i;
				}
			}

			// no free gamepad found, set it to gamepad 0;
			// can be useful for testing with different ships
			if (m_datas.GamePadIndexes[m_datas.CurrentPlayerNumber-1] == -1)
				m_datas.GamePadIndexes[m_datas.CurrentPlayerNumber-1] = 0;

		}

		void buttonAccept_Click(object sender, MouseEventArgs args)
		{
			ExitScreen();

			m_datas.ShipTypes.Add(ShipType.Types[sliderShip.Index]);

			// dernier joueur ?
			if (m_datas.CurrentPlayerNumber < m_datas.TotalPlayerCount)
			{// non
				m_datas.CurrentPlayerNumber++;
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

				sliderShip.Width = (int)((buttonAccept.X - buttonBack.X - buttonBack.Width) * 4 / 5);
				sliderShip.X = (int)(buttonBack.X + buttonBack.Width + ((buttonAccept.X - buttonBack.X - buttonBack.Width) * 1 / 10));
				sliderShip.Y = this.GraphicsDevice.PresentationParameters.BackBufferHeight * 3 / 4 - sliderShip.Height / 2;
			}

			/*
			if (sliderShip != null)
			{
				m_selectedShip = m_ships[(int)sliderShip.Value];
				//m_selectedModel = m_shipModels[(int)sliderShip.Value];
			}
			else
			{
				m_selectedShip = m_ships[0];
				//m_selectedModel = m_shipModels[0];
			}
			 * */
		}



		protected override void Cleanup()
		{
			XeGame.GuiManager.Controls.Remove(labelPlayer);
			labelPlayer.Dispose();

			XeGame.GuiManager.Controls.Remove(buttonBack);
			buttonBack.Dispose();

			XeGame.GuiManager.Controls.Remove(buttonAccept);
			buttonAccept.Dispose();

			XeGame.GuiManager.Controls.Remove(sliderShip);
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
			ViewMatrix = Matrix.CreateLookAt(Vector3.Transform(new Vector3(0, 50, 100), Matrix.CreateRotationY(angle)), new Vector3(0, 0, 0), Vector3.Up);
			m_ship.Model.View = ViewMatrix;
			m_ship.Model.Projection = ProjectionMatrix;

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
