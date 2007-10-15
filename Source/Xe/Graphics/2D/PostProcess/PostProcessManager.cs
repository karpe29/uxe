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
using System.Collections;
#endregion

namespace Xe.Graphics2D.PostProcess
{
	public class PostProcessManager : DrawableGameComponent, IDisposable
	{
		public static string EFFECT_PATH = @"Content\Effects\PostProcess\";

		private GraphicsDevice m_device = null;
		private ContentManager m_contentManager = null;
		private Viewport m_viewPort = new Viewport();

		private Texture2D m_heatHazeMap = null;
		private Texture2D m_resolveTarget = null;

		private SpriteBatch m_spriteBatch = null;
		private RenderTarget2D m_renderTarget1 = null;
		private RenderTarget2D m_renderTarget2 = null;
		private RenderTarget2D m_renderTargetcurrent = null;
		private RenderTarget2D m_renderTargetHeatHaze = null;

		public SortedList<String, PostProcessEffect> EffectDictionary = new SortedList<String, PostProcessEffect>();
		public Queue<String> AppliedEffects = new Queue<string>();

		public PostProcessManager(Game game, GraphicsDevice graphicsDevice, ContentManager contentManager)
			: base(game)
		{
			m_device = graphicsDevice;
			m_contentManager = contentManager;

			CreateRenderTargets();

			////////////////////////////////////////////////////////
			////////////////////////////////////////////////////////
			// Loads all effect located in EFFECT_PATH
			////////////////////////////////////////////////////////
			////////////////////////////////////////////////////////

			foreach (string s in Directory.GetFiles(EFFECT_PATH))
			{
				string s2 = s.Remove(0, EFFECT_PATH.Length);
				string s3 = "";

				if (s2.EndsWith(".xnb"))
				{
					s3 = s2.Substring(0, s2.Length - 4);
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
					ppe = (PostProcessEffect)Activator.CreateInstance(t, this.m_device, this.m_contentManager);
				else
					if (!p.StartsWith("Advanced"))
						ppe = new PostProcessEffect(this.m_device, this.m_contentManager, p);

				if (ppe != null)
					EffectDictionary.Add(p, ppe);
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

		protected override void LoadGraphicsContent(bool loadAllContent)
		{
			base.LoadGraphicsContent(loadAllContent);

			CreateRenderTargets();
		}

		private void CreateRenderTargets()
		{
			m_viewPort = m_device.Viewport;

			m_spriteBatch = new SpriteBatch(this.m_device);

			m_heatHazeMap = new Texture2D(m_device, 1, 1, 1,
							  ResourceUsage.None,
							  m_device.PresentationParameters.BackBufferFormat,
							  ResourceManagementMode.Manual);

			m_resolveTarget = new Texture2D(m_device, m_viewPort.Width, m_viewPort.Height, 1,
				   ResourceUsage.ResolveTarget,
				   m_device.PresentationParameters.BackBufferFormat,
				   ResourceManagementMode.Manual);

			m_renderTarget1 = new RenderTarget2D(m_device, m_viewPort.Width, m_viewPort.Height, 1, m_device.PresentationParameters.BackBufferFormat,
				m_device.PresentationParameters.MultiSampleType, 0);
			m_renderTarget2 = new RenderTarget2D(m_device, m_viewPort.Width, m_viewPort.Height, 1, m_device.PresentationParameters.BackBufferFormat,
				m_device.PresentationParameters.MultiSampleType, 0);

			m_renderTargetcurrent = m_renderTarget1;

			m_renderTargetHeatHaze = new RenderTarget2D(m_device, m_viewPort.Width, m_viewPort.Height, 1, m_device.PresentationParameters.BackBufferFormat,
				m_device.PresentationParameters.MultiSampleType, 0);
		}
		
		private void SwitchSetRenderTarget()
		{
			m_renderTargetcurrent = m_renderTargetcurrent == m_renderTarget1 ? m_renderTarget2 : m_renderTarget1;
			m_device.SetRenderTarget(0, m_renderTargetcurrent);
		}

		private void ResolveRenderTarget()
		{
			m_device.ResolveRenderTarget(0);
			m_resolveTarget = m_renderTargetcurrent.GetTexture();
		}

		public PostProcessResult RetrieveFrameBuffer()
		{
			RenderTarget2D rt = m_device.GetRenderTarget(0) as RenderTarget2D;

			if (rt == null)
			{
				m_device.ResolveBackBuffer(m_resolveTarget);
				return new PostProcessResult(m_resolveTarget);
			}
			else
			{
				m_device.ResolveRenderTarget(0);
				return new PostProcessResult(rt.GetTexture());
			}
		}

		public void Present(RenderTarget2D renderTarget, PostProcessResult lastScene)
		{
			m_device.SetRenderTarget(0, renderTarget);
			m_spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
			m_spriteBatch.Draw(lastScene.SceneTexture, new Rectangle(m_viewPort.X, m_viewPort.Y, m_viewPort.Width, m_viewPort.Height), Color.White);
			m_spriteBatch.End();
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

			m_spriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);

			effect.BeginPostProcess();

			m_spriteBatch.Draw(lastScene.SceneTexture, new Rectangle(m_viewPort.X, m_viewPort.Y, m_viewPort.Width, m_viewPort.Height), new Rectangle(m_viewPort.X, m_viewPort.Y, m_viewPort.Width, m_viewPort.Height), Color.White);

			m_spriteBatch.End();
			effect.EndPostProcess();

			ResolveRenderTarget();
			return new PostProcessResult(m_resolveTarget);
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
			m_spriteBatch.Dispose();
			m_renderTarget1.Dispose();
			m_renderTarget2.Dispose();
			m_resolveTarget.Dispose();
			m_heatHazeMap.Dispose();

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
		public override void  Draw(GameTime gameTime)
		{
			PostProcessResult rlast = this.RetrieveFrameBuffer();

			while (AppliedEffects.Count > 0)
			{
				rlast = ApplyEffect(EffectDictionary[AppliedEffects.Dequeue()], rlast);
			}

			this.Present(null, rlast);
		}
		#endregion
	}
}
