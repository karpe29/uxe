using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

namespace Xe.SpaceRace
{
	enum WormHoleType
	{
		Red = 0,	// bad hole, rewind time
		Green = 1,	// good hole, forward time
		Blue = 2,	// random hole :p ( red or green, never black )
		Black = -1	// very bad hole, desctuction and chaos 
	}

	class WormHole
	{
		public WormHole(Microsoft.Xna.Framework.Game game)
		{

		}
	}
}
