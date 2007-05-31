using System;

using XeFramework.XeGame;

namespace Xe
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main(string[] args)
		{
			using (XeGame game = new XeGame())
			{
				game.Run();
			}
		}
	}
}

