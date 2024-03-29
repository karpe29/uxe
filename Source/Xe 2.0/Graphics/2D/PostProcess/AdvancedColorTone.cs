using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Xe.Graphics2D.PostProcess
{
	public sealed class AdvancedColorTone : PostProcessEffect
	{
		#region Variables and Properties

		private EffectParameter lightColorParameter;
		private EffectParameter darkColorParameter;
		private EffectParameter desatParameter;
		private EffectParameter tonedParameter;

		public Vector3 LightColor
		{
			get
			{
				return lightColorParameter.GetValueVector3();
			}
			set
			{
				lightColorParameter.SetValue(value);
			}
		}

		public Vector3 DarkColor
		{
			get
			{
				return darkColorParameter.GetValueVector3();
			}
			set
			{
				darkColorParameter.SetValue(value);
			}
		}

		public float Desat
		{
			get
			{
				return desatParameter.GetValueSingle();
			}
			set
			{
				desatParameter.SetValue(value);
			}
		}

		public float Toned
		{
			get
			{
				return tonedParameter.GetValueSingle();
			}
			set
			{
				tonedParameter.SetValue(value);
			}
		}

		#endregion

		public AdvancedColorTone(PostProcessManager manager)
			: base(manager, "AdvancedColorTone")
		{
			lightColorParameter = m_effect.Parameters["LightColor"];
			darkColorParameter = m_effect.Parameters["DarkColor"];
			desatParameter = m_effect.Parameters["Desat"];
			tonedParameter = m_effect.Parameters["Toned"];
		}
	}
}
