using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Xe.Graphics2D.PostProcess;

namespace Xe.Graphics2D.PostProcess
{
    /// <summary>
    /// Represents a class for GPU Post Processing from <br />www.mahdi-khodadadi.com
    /// </summary>
    public class PostProcess
    {
        int ScreenWidth = 800, ScreenHeight = 600;
        int PostWidth, PostHeight;

        Texture2D BackBuffer = null, PostBuffer = null;

        SpriteBatch SpriteBatch = null;

        RenderTarget2D FullTarget1 = null, FullTarget2 = null, /*HalfTarget = null,*/ CurrentTarget = null;

        GraphicsDevice GraphicsDevice = null;

        Monochrome monochrome = null;
        ColorInverse colorInverse = null;
        BloomExtract bloomExtract = null;
        GaussianBlur gaussianBlur = null;
        Combine combine = null;
        ToneMapping toneMapping = null;
        RadialBlur radialBlur = null;
        DownSample downSample = null;

        #region Properties
        public BloomExtract BloomExtract
        {
            get
            {
                return bloomExtract;
            }
        }

        public ToneMapping ToneMapping
        {
            get
            {
                return toneMapping;
            }
        }

        public GaussianBlur GaussianBlur
        {
            get
            {
                return gaussianBlur;
            }
        }

        public RadialBlur RadialBlur
        {
            get
            {
                return radialBlur;
            }
        }
        #endregion

        public PostProcess(GraphicsDevice graphicsDevice)
        {
            GraphicsDevice = graphicsDevice;
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

            bloomExtract = new BloomExtract(graphicsDevice);
            monochrome = new Monochrome(graphicsDevice);
            colorInverse = new ColorInverse(graphicsDevice);
            gaussianBlur = new GaussianBlur(graphicsDevice);
            combine = new Combine(graphicsDevice);
            toneMapping = new ToneMapping(graphicsDevice);
            radialBlur = new RadialBlur(graphicsDevice);
            downSample = new DownSample(graphicsDevice);
        }

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

        public void ApplyRadialBlur()
        {
            SetRenderTarget();

            SpriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);

            radialBlur.Begin();
            SpriteBatch.Draw(PostBuffer, new Rectangle(0, 0, PostWidth, PostHeight), new Rectangle(0, 0, PostWidth, PostHeight), Color.White);

            SpriteBatch.End();
            radialBlur.End();

            ResolveRenderTarget();
        }

        public void ApplyColorInverse()
        {
            SetRenderTarget();

            SpriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);
            colorInverse.Begin();

            SpriteBatch.Draw(PostBuffer, new Rectangle(0, 0, PostWidth, PostHeight), new Rectangle(0, 0, PostWidth, PostHeight), Color.White);

            SpriteBatch.End();
            colorInverse.End();

            ResolveRenderTarget();
        }

        public void ApplyToneMapping()
        {
            SetRenderTarget();

            SpriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);

            toneMapping.Begin();
            SpriteBatch.Draw(PostBuffer, new Rectangle(0, 0, PostWidth, PostHeight), new Rectangle(0, 0, PostWidth, PostHeight), Color.White);

            SpriteBatch.End();
            toneMapping.End();

            ResolveRenderTarget();
        }

        public void ApplyMonochrome()
        {
            SetRenderTarget();

            SpriteBatch.Begin(SpriteBlendMode.None, SpriteSortMode.Immediate, SaveStateMode.None);

            monochrome.Begin();
            SpriteBatch.Draw(PostBuffer, new Rectangle(0, 0, PostWidth, PostHeight), new Rectangle(0, 0, PostWidth, PostHeight), Color.White);

            SpriteBatch.End();
            monochrome.End();

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
    }
}
