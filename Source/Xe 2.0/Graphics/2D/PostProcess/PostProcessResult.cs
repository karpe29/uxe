using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Xe.Graphics2D.PostProcess
{
	public class PostProcessResult : IDisposable
	{
		internal Texture2D SceneTexture;

		internal PostProcessResult(Texture2D tex)
		{
			SceneTexture = tex;
		}

		void IDisposable.Dispose()
		{
			if (SceneTexture != null)
			{
				SceneTexture.Dispose();
			}
		}
	}
}
