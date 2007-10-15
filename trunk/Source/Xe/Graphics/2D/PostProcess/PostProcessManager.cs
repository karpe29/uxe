#region About
/* Based on the work of Mahdi Khodadadi
 * http://www.mahdi-khodadadi.com/
 * Effects and Manager by Mahdi Khodadadi
 * Additional Effects by Glenn Wilson (Mykre)
 * Enhancements by jbriguet.
 * */
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Xe.Tools;
using Microsoft.Xna.Framework.Content;
using System.IO;
#endregion

namespace Xe.Graphics2D.PostProcess
{
	public class PostProcessManager : DrawableGameComponent, IDisposable
	{
		public static string EFFECT_PATH = @"Content\Effects\PostProcess\";

		private GraphicsDevice device = null;
		private ContentManager ContentManager = null;
		private Viewport viewPort = new Viewport();

		private Texture2D heatHazeMap = null;
		private Texture2D resolveTarget = null;

		private SpriteBatch spriteBatch = null;
		private RenderTarget2D renderTarget1 = null;
		private RenderTarget2D renderTarget2 = null;
		private RenderTarget2D renderTargetcurrent = null;
		private RenderTarget2D renderTargetHeatHaze = null;

		Dictionary<String, PostProcessEffect> EffectDictionary = new Dictionary<String, PostProcessEffect>();

		public PostProcessManager(Game game, GraphicsDevice graphicsDevice, ContentManager contentManager)
			: base(game)
		{
			device = graphicsDevice;
			ContentManager = contentManager;
			viewPort = device.Viewport;
			device.DeviceReset += new EventHandler(device_DeviceReset);

			spriteBatch = new SpriteBatch(graphicsDevice);

			heatHazeMap = new Texture2D(device, 1, 1, 1,
							  ResourceUsage.None,
							  device.PresentationParameters.BackBufferFormat,
							  ResourceManagementMode.Manual);

			resolveTarget = new Texture2D(device, viewPort.Width, viewPort.Height, 1,
				   ResourceUsage.ResolveTarget,
				   device.PresentationParameters.BackBufferFormat,
				   ResourceManagementMode.Manual);

			renderTarget1 = new RenderTarget2D(device, viewPort.Width, viewPort.Height, 1, device.PresentationParameters.BackBufferFormat, MultiSampleType.None, 0);
			renderTarget2 = new RenderTarget2D(device, viewPort.Width, viewPort.Height, 1, device.PresentationParameters.BackBufferFormat, MultiSampleType.None, 0);
			renderTargetcurrent = renderTarget1;
			renderTargetHeatHaze = new RenderTarget2D(device, viewPort.Width, viewPort.Height, 1, device.PresentationParameters.BackBufferFormat, MultiSampleType.None, 0);

			////////////////////////////////////////////////////////
			////////////////////////////////////////////////////////
			// Loads all effect from EFFECT_PATH
			////////////////////////////////////////////////////////
			////////////////////////////////////////////////////////

			foreach (string s in Directory.GetFiles(EFFECT_PATH))
			{
				string s2 = s.Remove(0, EFFECT_PATH.Length);
				string s3 = "";

				if (s2.EndsWith(".xnb"))
				{
					s3 = s2.Substring(0, s2.Length -4);
					this.AddEffect(s3);
				}	
			}
		}

		private void AddEffect(string p)
		{
			try
			{
				Type t = Type.GetType(this.GetType().Namespace + "." + p, false);

				PostProcessEffect ppe = null;

				if (t != null && t.BaseType == typeof(PostProcessEffect))
					ppe = (PostProcessEffect)Activator.CreateInstance(t, this.device, this.ContentManager);
				else
					if (!p.StartsWith("Advanced"))
						ppe = new PostProcessEffect(this.device, this.ContentManager, p);

				if (ppe != null)
					EffectDictionary.Add(p, ppe);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		private void device_DeviceReset(object sender, EventArgs e)
		{
			viewPort = device.Viewport;
		}

		private void SwitchSetRenderTarget()
		{
			renderTargetcurrent = renderTargetcurrent == renderTarget1 ? renderTarget2 : renderTarget1;
			device.SetRenderTarget(0, renderTargetcurrent);
		}

		private void ResolveRenderTarget()
		{
			device.ResolveRenderTarget(0);
			resolveTarget = renderTargetcurrent.GetTexture();
		}

		public PostProcessResult RetrieveFrameBuffer()
		{
			RenderTarget2D rt = device.GetRenderTarget(0) as RenderTarget2D;

			if (rt == null)
			{
				device.ResolveBackBuffer(resolveTarget);
				return new PostProcessResult(resolveTarget);
			}
			else
			{
				device.ResolveRenderTarget(0);
				return new PostProcessResult(rt.GetTexture());
			}
		}

		public void Present(RenderTarget2D renderTarget, PostProcessResult lastScene)
		{
			device.SetRenderTarget(0, renderTarget);
			spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
			spriteBatch.Draw(lastScene.SceneTexture, new Rectangle(viewPort.X, viewPort.Y, viewPort.Width, viewPort.Height), Color.White);
			spriteBatch.End();
		}

		/// <summary>
		/// Apply basic effect (and advanced too, if they dont need per pass parameters)
		/// See example with Gaussian Vert/Hor blur - under -
		/// </summary>
		/// <param name="lastScene"></param>
		/// <returns></returns>
		public PostProcessResult ApplyEffect(PostProcessEffect effect, PostProcessResult lastScene)
		{
			SwitchSetRenderTarget();

			spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);

			effect.BeginPostProcess();

			spriteBatch.Draw(lastScene.SceneTexture, new Rectangle(viewPort.X, viewPort.Y, viewPort.Width, viewPort.Height), new Rectangle(viewPort.X, viewPort.Y, viewPort.Width, viewPort.Height), Color.White);

			spriteBatch.End();
			effect.EndPostProcess();

			ResolveRenderTarget();
			return new PostProcessResult(resolveTarget);
		}
		/*
		public PostProcessResult ApplyGaussianBlurWithBloomV(PostProcessResult lastScene)
		{
			SwitchSetRenderTarget();
			gaussianBlur.SetBlurParameters(0, 1.0f / (float)viewPort.Height);
			spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);

			gaussianBlur.Begin();
			spriteBatch.Draw(lastScene.SceneTexture, new Rectangle(viewPort.X, viewPort.Y, viewPort.Width, viewPort.Height), new Rectangle(viewPort.X, viewPort.Y, viewPort.Width, viewPort.Height), Color.White);

			spriteBatch.End();
			gaussianBlur.End();

			ResolveRenderTarget();
			return new PostProcessResult(resolveTarget);
		}

		public PostProcessResult ApplyGaussianBlurWithBloomH(PostProcessResult lastScene)
		{
			SwitchSetRenderTarget();
			gaussianBlur.SetBlurParameters(1.0f / (float)viewPort.Width, 0);
			spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);

			gaussianBlur.Begin();
			spriteBatch.Draw(lastScene.SceneTexture, new Rectangle(viewPort.X, viewPort.Y, viewPort.Width, viewPort.Height), new Rectangle(viewPort.X, viewPort.Y, viewPort.Width, viewPort.Height), Color.White);

			spriteBatch.End();
			gaussianBlur.End();

			ResolveRenderTarget();
			return new PostProcessResult(resolveTarget);
		}
				
		public PostProcessResult CombineScreens(PostProcessResult scene1, PostProcessResult scene2)
		{
			SwitchSetRenderTarget();

			spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);

			combineEffect.Begin();
			device.Textures[1] = scene2.SceneTexture;
			spriteBatch.Draw(scene1.SceneTexture, new Rectangle(viewPort.X, viewPort.Y, viewPort.Width, viewPort.Height), new Rectangle(viewPort.X, viewPort.Y, viewPort.Width, viewPort.Height), Color.White);

			spriteBatch.End();
			combineEffect.End();

			ResolveRenderTarget();
			return new PostProcessResult(resolveTarget);
		}
		*/

		#region IDisposable Members

		public new void Dispose()
		{
			spriteBatch.Dispose();
			renderTarget1.Dispose();
			renderTarget2.Dispose();
			resolveTarget.Dispose();
			heatHazeMap.Dispose();

			base.Dispose();
		}

		#endregion

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			this.Dispose();
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
