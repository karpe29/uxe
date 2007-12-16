using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Xe.Graphics3D;
using Microsoft.Xna.Framework;
using Xe.GameScreen;


namespace Xe.SpaceRace
{
	class SpaceRaceScreen : IGameScreen
	{
		SpaceRaceInitDatas m_datas;

		

		Race m_race;

		public Race Race
		{
			get { return m_race; }
		}

		public SpaceRaceInitDatas InitDatas
		{
			get { return m_datas; }
		}

		public SpaceRaceScreen(GameScreenManager gameScreenManager, SpaceRaceInitDatas datas)
			: base(gameScreenManager, true)
		{
			// disable EBI
			//Ebi e = (Ebi)Game.Services.GetService(typeof(IEbiService));
			//e.Enabled = false;

			// should show the loading screen here

			m_datas = datas;


			m_race = new Race(gameScreenManager, m_datas);

			// should hide the loading screen here
		}

		protected override void LoadGraphicsContent(bool loadAllContent)
		{
			base.LoadGraphicsContent(loadAllContent);

			if (loadAllContent)
			{
			}
		}

		public override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			m_race.Draw(gameTime);
		}


		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);

			m_race.Update(gameTime);	
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
