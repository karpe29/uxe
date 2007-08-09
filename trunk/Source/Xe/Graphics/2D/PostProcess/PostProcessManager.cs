using System;
using System.Collections.Generic;
using System.Text;

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
		}
		
		/// <summary>
		/// Apply Post Process effects depending on settings
		/// </summary>
		public void ApplyPostProcess()
		{
			if (!EnablePostProcess)
				return;

			m_postProcess.ResolveBackBuffer();

			//ppe.BloomExtract.Threshold = 1.0f;

			//ppe.ApplyBloomExtract();

			if (EnableBlur)
			{
				//ppe.RadialBlur.BlurStart = 1;
				//ppe.RadialBlur.BlurWidth = -0.2f;
				//ppe.ApplyRadialBlur();
				//ppe.ApplyGaussianBlurH();
				//ppe.ApplyGaussianBlurV();
			}


			//ppe.ApplyToneMapping();



			//m_postProcess.CombineWithBackBuffer();

			m_postProcess.Present(null);
		}

		#region Properties

		public bool EnableBlur = false;

		public bool EnablePostProcess = false;

		#endregion
	}
}
