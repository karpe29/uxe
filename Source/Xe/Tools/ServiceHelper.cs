using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Xe.Tools
{
	static class ServiceHelper
	{
		static Game s_game;

		public static void Add<T>(T service) where T : class
		{
			s_game.Services.AddService(typeof(T), service);
		}

		public static T Get<T>() where T : class
		{
			return s_game.Services.GetService(typeof(T)) as T;
		}

		public static Game Game
		{
			get { return s_game; }
			set { s_game = value; }
		}

		public static GameServiceContainer Services
		{
			get { return s_game.Services; }
		}
	}

}
