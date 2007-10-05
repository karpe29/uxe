using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Xe.Tools;

namespace Xe.Graphics2D.PostProcess
{
	public class PostProcessManager : DrawableGameComponent
	{
		int ScreenWidth = 800, ScreenHeight = 600;
		int PostWidth, PostHeight;

		Texture2D BackBuffer = null, PostBuffer = null;

		SpriteBatch SpriteBatch = null;

		RenderTarget2D FullTarget1 = null, FullTarget2 = null, /*HalfTarget = null,*/ CurrentTarget = null;

		GraphicsDevice m_graphicsDevice = null;
		ContentManager m_contentManager = null;

		AdvancedGaussianBlur gaussianBlur = null;
		AdvancedCombine combine = null;
		AdvancedToneMapping toneMapping = null;
		AdvancedRadialBlur radialBlur = null;

		PostProcessEffect bloomExtract = null;

		public PostProcessManager(Game game, GraphicsDevice graphicsDevice, ContentManager contentManager)
			: base(game)
		{
			m_graphicsDevice = graphicsDevice;
			m_contentManager = contentManager;

			SpriteBatch = new SpriteBatch(graphicsDevice);

			// Look up the resolution and format of our main backbuffer.
			PresentationParameters pp = graphicsDevice.PresentationParameters;

			ScreenWidth = pp.BackBufferWidth;
			ScreenHeight = pp.BackBufferHeight;

			SurfaceFormat format = pp.BackBufferFormat;

			// Create a texture for reading back the backbuffer contents.
			BackBuffer = new Texture2D(graphicsDevice, ScreenWidth, ScreenHeight, 1,
										  ResourceUsage.ResolveTarget, format,
										  ResourceManagementMode.Manual);
			PostBuffer = BackBuffer;
			//Get w, h

			PostWidth = ScreenWidth / 2;
			PostHeight = ScreenHeight / 2;

			FullTarget1 = new RenderTarget2D(graphicsDevice, ScreenWidth, ScreenHeight, 1, format);
			FullTarget2 = new RenderTarget2D(graphicsDevice, ScreenWidth, ScreenHeight, 1, format);
			//HalfTarget = new RenderTarget2D(graphicsDevice, PostWidth, PostHeight, 1, format);

			CurrentTarget = FullTarget1;


			gaussianBlur = new AdvancedGaussianBlur(graphicsDevice, m_contentManager);
			combine = new AdvancedCombine(graphicsDevice, m_contentManager);
			toneMapping = new AdvancedToneMapping(graphicsDevice, m_contentManager);
			radialBlur = new AdvancedRadialBlur(graphicsDevice, m_contentManager);

			bloomExtract = new PostProcessEffect(graphicsDevice, m_contentManager, "Bloom");
		}

		#region RenderTargets and Present

		public void ResolveBackBuffer()
		{
			RenderTarget2D rt = GraphicsDevice.GetRenderTarget(0) as RenderTarget2D;

			if (rt == null)
			{
				GraphicsDevice.ResolveBackBuffer(BackBuffer);
			}
			else
			{
				GraphicsDevice.ResolveRenderTarget(0);
				BackBuffer = rt.GetTexture();
			}

			PostBuffer = BackBuffer;

			PostWidth = ScreenWidth;
			PostHeight = ScreenHeight;
		}

		private void ResolveRenderTarget()
		{
			GraphicsDevice.ResolveRenderTarget(0);
			PostBuffer = CurrentTarget.GetTexture();
			GraphicsDevice.SetRenderTarget(0, null);
		}

		private void SetRenderTarget()
		{
			CurrentTarget = CurrentTarget == FullTarget1 ? FullTarget2 : FullTarget1;
			GraphicsDevice.SetRenderTarget(0, CurrentTarget);
		}

		public void Present(RenderTarget2D renderTarget2D)
		{
			GraphicsDevice.SetRenderTarget(0, renderTarget2D);
			GraphicsDevice.Clear(Color.Black);
			SpriteBatch.Begin();

			SpriteBatch.Draw(PostBuffer, new Rectangle(0, 0, PostWidth, PostHeight), new Rectangle(0, 0, PostWidth, PostHeight), Color.White);

			SpriteBatch.End();
		}

		#endregion

		#region Generic and Advanced Effects Process

		public void ApplyEffect(PostProcessEffect effect)
		{
			SetRenderTarget();

			SpriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);

			effect.Begin();
			SpriteBatch.Draw(PostBuffer, new Rectangle(0, 0, PostWidth, PostHeight), new Rectangle(0, 0, PostWidth, PostHeight), Color.White);

			SpriteBatch.End();
			effect.End();

			ResolveRenderTarget();
		}

		public void ApplyDownSample()
		{
			PostWidth /= 2;
			PostHeight /= 2;

			SetRenderTarget();

			SpriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
			//downSample.Begin();
			SpriteBatch.Draw(PostBuffer, new Rectangle(0, 0, PostWidth, PostHeight), new Rectangle(0, 0, PostWidth * 2, PostHeight * 2), Color.White);

			SpriteBatch.End();
			//downSample.End();

			ResolveRenderTarget();
		}

		public void ApplyUpSample()
		{
			PostWidth *= 2;
			PostHeight *= 2;
			SetRenderTarget();

			SpriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);

			SpriteBatch.Draw(PostBuffer, new Rectangle(0, 0, PostWidth, PostHeight), new Rectangle(0, 0, PostWidth / 2, PostHeight / 2), Color.White);

			SpriteBatch.End();

			ResolveRenderTarget();
		}

		public void ApplyBloomExtract()
		{
			SetRenderTarget();

			SpriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);

			bloomExtract.Begin();
			SpriteBatch.Draw(PostBuffer, new Rectangle(0, 0, PostWidth, PostHeight), new Rectangle(0, 0, PostWidth, PostHeight), Color.White);

			SpriteBatch.End();
			bloomExtract.End();

			ResolveRenderTarget();
		}

		public void ApplyGaussianBlurV()
		{
			SetRenderTarget();
			gaussianBlur.SetBlurParameters(0, 1.0f / (float)PostHeight);
			SpriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);

			gaussianBlur.Begin();
			SpriteBatch.Draw(PostBuffer, new Rectangle(0, 0, PostWidth, PostHeight), new Rectangle(0, 0, PostWidth, PostHeight), Color.White);

			SpriteBatch.End();
			gaussianBlur.End();

			ResolveRenderTarget();
		}

		public void ApplyGaussianBlurH()
		{
			SetRenderTarget();
			gaussianBlur.SetBlurParameters(1.0f / (float)PostWidth, 0);
			SpriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);

			gaussianBlur.Begin();
			SpriteBatch.Draw(PostBuffer, new Rectangle(0, 0, PostWidth, PostHeight), new Rectangle(0, 0, PostWidth, PostHeight), Color.White);

			SpriteBatch.End();
			gaussianBlur.End();

			ResolveRenderTarget();
		}

		public void CombineWithBackBuffer()
		{
			SetRenderTarget();
			GraphicsDevice.Textures[1] = BackBuffer;
			SpriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);

			combine.Begin();
			SpriteBatch.Draw(PostBuffer, new Rectangle(0, 0, PostWidth, PostHeight), new Rectangle(0, 0, PostWidth, PostHeight), Color.White);

			SpriteBatch.End();
			combine.End();

			ResolveRenderTarget();
		}

		#endregion

		#region Global PostProcessing Application

		/// <summary>
		/// Apply Post Process effects depending on settings
		/// </summary>
		/*
		public override void  Draw(GameTime gameTime)
		{
			if (!IsPostProcessActive)
				//return;

			m_postProcess.ResolveBackBuffer();

			if (EnableBloom)
				m_postProcess.ApplyBloomExtract();

			if (EnableRadialBlur)
				m_postProcess.ApplyRadialBlur();

			if (EnableGaussianBlur)
			{
				m_postProcess.ApplyGaussianBlurH();
				m_postProcess.ApplyGaussianBlurV();
			}

			if (EnableColorInverse)
				m_postProcess.ApplyColorInverse();


			//m_postProcess.ApplyMonochrome();


			m_postProcess.ToneMapping.DeFog = .2f;
			m_postProcess.ToneMapping.Exposure = 1.0f;
			m_postProcess.ToneMapping.Gamma = 1.0f;
			if (Keyboard.GetState().IsKeyDown(Keys.N))
			{
				m_postProcess.ApplyColorInverse();

				m_postProcess.CombineWithBackBuffer();
			}
			//m_postProcess.ApplyColorInverse();
			
			//m_postProcess.CombineWithBackBuffer();
			//m_postProcess.CombineWithBackBuffer();
			//m_postProcess.CombineWithBackBuffer();
			//m_postProcess.CombineWithBackBuffer();

			

			if (IsColorVariationActive)
			{
				//m_postProcess.Present(null);

				//m_postProcess.ResolveBackBuffer();

				if (EnableMonochrome)
					m_postProcess.ApplyMonochrome();

				if (EnableToneMapping)
					m_postProcess.ApplyToneMapping();

				
			}

			m_postProcess.Present(null);

			base.Draw(gameTime);
		}*/
		#endregion
	}
}
