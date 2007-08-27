using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Xe.Graphics2D.PostProcess
{
	/// <summary>
	/// A simple layer over PostProcess engine
	/// TODO : add properties to enable/disable/parametrize effects
	/// </summary>
	public class PostProcessManager : DrawableGameComponent
	{
		PostProcess m_postProcess;

		public PostProcessManager(Game game)
			:base (game)
		{
		}

		protected override void LoadGraphicsContent(bool loadAllContent)
		{
			base.LoadGraphicsContent(loadAllContent);

			m_postProcess = new PostProcess(XeGame.Device);
			m_postProcess.GaussianBlur.BloomScale = 1f;
			m_postProcess.GaussianBlur.BlurAmount = 1.0f;
		}
		
		/// <summary>
		/// Apply Post Process effects depending on settings
		/// </summary>
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
		}

		#region Properties

		public bool IsPostProcessActive
		{
			get
			{
				return EnableToneMapping ||
						  EnableMonochrome ||
						  EnableGaussianBlur ||
						  EnableColorInverse ||
						  EnableBloom ||
						  EnableRadialBlur;
			}
		}

		public bool IsColorVariationActive
		{
			get
			{
				return //EnableColorInverse ||
						  EnableMonochrome ||
						  EnableToneMapping;
			}
		}

		public bool EnableColorInverse = false;

		public bool EnableMonochrome = false;

		#region Radial Blur
		public bool EnableRadialBlur = false;

		public float RadialBlurStart
		{
			set { m_postProcess.RadialBlur.BlurStart = value; }
			get { return m_postProcess.RadialBlur.BlurStart; }
		}

		public float RadialBlurWidth
		{
			set { m_postProcess.RadialBlur.BlurWidth = value; }
			get { return m_postProcess.RadialBlur.BlurWidth; }
		}

		public Vector2 RadialBlurCenter 
		{
			set { m_postProcess.RadialBlur.Center = value; }
			get { return m_postProcess.RadialBlur.Center; }
		}
		#endregion
		
		#region Bloom
		public bool EnableBloom = false;

		public float BloomThresold
		{
			set { m_postProcess.BloomExtract.Threshold = value; }
			get { return m_postProcess.BloomExtract.Threshold; }
		}
		#endregion

		#region Gaussian Blur
		public bool EnableGaussianBlur = false;

		public float GaussianBlurBloomScale
		{
			set { m_postProcess.GaussianBlur.BloomScale = value; }
			get { return m_postProcess.GaussianBlur.BloomScale; }
		}

		public float GaussianBlurAmount
		{
			set { m_postProcess.GaussianBlur.BlurAmount = value; }
			get { return m_postProcess.GaussianBlur.BlurAmount; }
		}
		#endregion

		#region Tone Mapping

		public bool EnableToneMapping = false;

		public float ToneMappingDefog
		{
			set { m_postProcess.ToneMapping.DeFog = value; }
			get { return m_postProcess.ToneMapping.DeFog; }
		}

		public float ToneMappingExposure
		{
			set { m_postProcess.ToneMapping.Exposure = value; }
			get { return m_postProcess.ToneMapping.Exposure; }
		}

		public float ToneMappingGamma
		{
			set { m_postProcess.ToneMapping.Gamma = value; }
			get { return m_postProcess.ToneMapping.Gamma; }
		}
		#endregion
		#endregion
	}
}
