using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Xe.Graphics2D.PostProcess
{
	/// <summary>
	/// A simple layer over PostProcess engine
	/// TODO : add properties to enable/disable/parametrize effects
	/// </summary>
	public class PostProcessManager
	{
		PostProcess m_postProcess;

		public PostProcessManager()
		{
			m_postProcess = new PostProcess(XeGame.Device);
			
			m_postProcess.GaussianBlur.BloomScale = 1f;
			m_postProcess.GaussianBlur.BlurAmount = 1.0f;

		}
		
		/// <summary>
		/// Apply Post Process effects depending on settings
		/// </summary>
		public void ApplyPostProcess()
		{
			if (!EnablePostProcess)
				return;

			m_postProcess.ResolveBackBuffer();

			if (EnableToneMapping)
			{
				m_postProcess.ApplyToneMapping();
			}

			if (EnableColorInverse)
			{
				m_postProcess.ApplyColorInverse();
			}

			if (EnableMonochrome)
			{
				m_postProcess.ApplyMonochrome();
			}


			if (EnableBloom)
			{
				m_postProcess.ApplyBloomExtract();
			}

			if (EnableRadialBlur)
			{
				m_postProcess.ApplyRadialBlur();
			}

			if (EnableGaussianBlur)
			{
				m_postProcess.ApplyGaussianBlurH();
				m_postProcess.ApplyGaussianBlurV();
			}

			m_postProcess.CombineWithBackBuffer();

			m_postProcess.Present(null);
		}

		#region Properties

		public bool EnablePostProcess
		{
			get { return	EnableToneMapping ||
							EnableMonochrome ||
							EnableGaussianBlur ||
							EnableColorInverse ||
							EnableBloom ||
							EnableRadialBlur; }
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

		public Vector2 RadialBludCenter 
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
