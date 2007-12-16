using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Xe.Graphics2D.PostProcess
{
    public class AdvancedCombine : PostProcessEffect
	{
		#region Variables and Properties

		private EffectParameter Intensity1Parameter;
		private EffectParameter Intensity2Parameter;
		private EffectParameter Saturation1Parameter;
		private EffectParameter Saturation2Parameter;
		private EffectParameter SaturationColorParameter;
		
		public float Intensity1
        {
            get
            {
                return Intensity1Parameter.GetValueSingle();
            }
            set
            {
                Intensity1Parameter.SetValue(value);
            }
        }

        public float Intensity2
        {
            get
            {
                return Intensity2Parameter.GetValueSingle();
            }
            set
            {
                Intensity2Parameter.SetValue(value);
            }
        }
        public float Saturation1
        {
            get
            {
                return Saturation1Parameter.GetValueSingle();
            }
            set
            {
                Saturation1Parameter.SetValue(value);
            }
        }
        public float Saturation2
        {
            get
            {
                return Saturation2Parameter.GetValueSingle();
            }
            set
            {
                Saturation2Parameter.SetValue(value);
            }
        }

        public float SaturationColor
        {
            get
            {
                return SaturationColorParameter.GetValueSingle();
            }
            set
            {
                SaturationColorParameter.SetValue(value);
            }
		}

		#endregion

        public AdvancedCombine(PostProcessManager manager)
			: base (manager, "AdvancedCombine")
        {
            Intensity1Parameter = m_effect.Parameters["Intensity1"];
            Intensity2Parameter = m_effect.Parameters["Intensity2"];
            Saturation1Parameter = m_effect.Parameters["Saturation1"];
            Saturation2Parameter = m_effect.Parameters["Saturation2"];
            SaturationColorParameter = m_effect.Parameters["SaturationColor"];

			this.ApplyEffect = new ApplyEffectDelegate(CombineScreens);
        }

		public PostProcessResult CombineScreens(PostProcessEffect ppe, PostProcessResult scene)
		{
			m_manager.SwitchSetRenderTarget();

			m_manager.SpriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);

			this.BeginPostProcess();

			m_manager.GraphicsDevice.Textures[1] = m_manager.RetrieveFrameBuffer().SceneTexture;

			m_manager.SpriteBatch.Draw(scene.SceneTexture, new Rectangle(m_manager.Viewport.X, m_manager.Viewport.Y, m_manager.Viewport.Width, m_manager.Viewport.Height),
				new Rectangle(m_manager.Viewport.X, m_manager.Viewport.Y, m_manager.Viewport.Width, m_manager.Viewport.Height), Color.White);

			m_manager.SpriteBatch.End();
			this.EndPostProcess();

			return new PostProcessResult(m_manager.ResolveRenderTarget());
		}
    }
}
