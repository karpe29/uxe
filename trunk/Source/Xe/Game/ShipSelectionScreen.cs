using System;
using System.Collections.Generic;
using System.Text;
using XeFramework.GUI;
using XeFramework.GameScreen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

		Model[] m_shipModels = new Model[4];

		float dst = 2000;

		private Matrix ViewMatrix = Matrix.CreateLookAt(new Vector3(0,1000,0), new Vector3(1500,0,0), Vector3.Up);
		private Matrix ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, 1.0f, 1, 10000);
		
		public ShipSelectionScreen(GameScreenManager gameScreenManager, int player, SpaceRaceScreen spaceRaceScreen)
			: base(gameScreenManager, true)
		{
			m_spaceRaceScreen = spaceRaceScreen;
			m_player = player;

			labelPlayer = new Label(GameScreenManager.Game, GameScreenManager.GuiManager);
			labelPlayer.TextAlign = TextAlignment.Center;
			labelPlayer.Text = "Player " + (m_player +1).ToString() + " Ship";
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
			sliderShip.Value = 2;
			sliderShip.ValueChanged += new ValueChangedHandler(sliderShip_ValueChanged);
			GameScreenManager.GuiManager.AddControl(sliderShip);

		}

		void sliderShip_ValueChanged(object sender, float value)
		{
			this.ViewMatrix = Matrix.CreateLookAt(new Vector3(0,1000,0), new Vector3(1500,0,0), Vector3.Up) * Matrix.CreateRotationY((int)value * MathHelper.PiOver2); 
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

			m_shipModels[0] = GameScreenManager.ContentManager.Load<Model>(@"Content\Models\StarChaser1");
			m_shipModels[1] = GameScreenManager.ContentManager.Load<Model>(@"Content\Models\StarChaser2");
			m_shipModels[2] = GameScreenManager.ContentManager.Load<Model>(@"Content\Models\StarChaser3");
			m_shipModels[3] = GameScreenManager.ContentManager.Load<Model>(@"Content\Models\StarChaser4");
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

			buttonAccept.Visible = this.Visible;
			buttonBack.Visible = this.Visible;
			sliderShip.Visible = this.Visible;
    	}

		public override void Draw(GameTime gameTime)
		{
			if (!this.Visible)
				return;

			base.Draw(gameTime);

			

			//Copy any parent transforms
			Matrix[] transforms = new Matrix[m_shipModels[0].Bones.Count];
			m_shipModels[0].CopyAbsoluteBoneTransformsTo(transforms);
			//Draw the model, a model can have multiple meshes, so loop
			for (int i = 0; i < m_shipModels[0].Meshes.Count; i++)
			{
				//This is where the mesh orientation is set, as well as our camera and projection
				for (int j = 0; j < m_shipModels[0].Meshes[i].Effects.Count; j++)
				{
					(m_shipModels[0].Meshes[i].Effects[j] as BasicEffect).EnableDefaultLighting();
					(m_shipModels[0].Meshes[i].Effects[j] as BasicEffect).World = 
						transforms[m_shipModels[0].Meshes[i].ParentBone.Index] * Matrix.CreateTranslation(dst,0,0);

					(m_shipModels[0].Meshes[i].Effects[j] as BasicEffect).View = this.ViewMatrix;
					(m_shipModels[0].Meshes[i].Effects[j] as BasicEffect).Projection = this.ProjectionMatrix;
				}

				m_shipModels[0].Meshes[i].Draw();
			}

			//Copy any parent transforms
			transforms = new Matrix[m_shipModels[1].Bones.Count];
			m_shipModels[1].CopyAbsoluteBoneTransformsTo(transforms);

			//Draw the model, a model can have multiple meshes, so loop
			for (int i = 0; i < m_shipModels[1].Meshes.Count; i++)
			{
				//This is where the mesh orientation is set, as well as our camera and projection
				for (int j = 0; j < m_shipModels[1].Meshes[i].Effects.Count; j++)
				{
					(m_shipModels[1].Meshes[i].Effects[j] as BasicEffect).EnableDefaultLighting();
					(m_shipModels[1].Meshes[i].Effects[j] as BasicEffect).World = transforms[m_shipModels[1].Meshes[i].ParentBone.Index] * Matrix.CreateTranslation(0, 0, -dst);

					(m_shipModels[1].Meshes[i].Effects[j] as BasicEffect).View = this.ViewMatrix;
					(m_shipModels[1].Meshes[i].Effects[j] as BasicEffect).Projection = this.ProjectionMatrix;
				}

				m_shipModels[1].Meshes[i].Draw();
			}

			//Copy any parent transforms
			transforms = new Matrix[m_shipModels[2].Bones.Count];
			m_shipModels[2].CopyAbsoluteBoneTransformsTo(transforms);

			//Draw the model, a model can have multiple meshes, so loop
			for (int i = 0; i < m_shipModels[2].Meshes.Count; i++)
			{
				//This is where the mesh orientation is set, as well as our camera and projection
				for (int j = 0; j < m_shipModels[2].Meshes[i].Effects.Count; j++)
				{
					(m_shipModels[2].Meshes[i].Effects[j] as BasicEffect).EnableDefaultLighting();
					(m_shipModels[2].Meshes[i].Effects[j] as BasicEffect).World = transforms[m_shipModels[2].Meshes[i].ParentBone.Index] * Matrix.CreateTranslation(-dst, 0, 0);
					(m_shipModels[2].Meshes[i].Effects[j] as BasicEffect).View = this.ViewMatrix;
					(m_shipModels[2].Meshes[i].Effects[j] as BasicEffect).Projection = this.ProjectionMatrix;
				}

				m_shipModels[2].Meshes[i].Draw();
			}

			//Copy any parent transforms
			transforms = new Matrix[m_shipModels[3].Bones.Count];
			m_shipModels[3].CopyAbsoluteBoneTransformsTo(transforms);

			//Draw the model, a model can have multiple meshes, so loop
			for (int i = 0; i < m_shipModels[3].Meshes.Count; i++)
			{
				//This is where the mesh orientation is set, as well as our camera and projection
				for (int j = 0; j < m_shipModels[3].Meshes[i].Effects.Count; j++)
				{
					(m_shipModels[3].Meshes[i].Effects[j] as BasicEffect).EnableDefaultLighting();
					(m_shipModels[3].Meshes[i].Effects[j] as BasicEffect).World = transforms[m_shipModels[3].Meshes[i].ParentBone.Index] * Matrix.CreateTranslation(0, 0, dst);
					(m_shipModels[3].Meshes[i].Effects[j] as BasicEffect).View = this.ViewMatrix;
					(m_shipModels[3].Meshes[i].Effects[j] as BasicEffect).Projection = this.ProjectionMatrix;
				}

				m_shipModels[3].Meshes[i].Draw();
			}
		}

		#endregion
	}
}
