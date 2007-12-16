#region Using Statements
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Xe.Gui
{
	public class VertexRenderer : QuadRenderer, IDisposable
	{
		#region Members
		VertexDeclaration vertexDecl = null;
		VertexPositionTexture[] verts = null;
		short[] ib = null;

		Effect quadRenderEffect;

		private Queue<QuadBase> m_quads = new Queue<QuadBase>();
		#endregion

		#region Constructors
		public VertexRenderer()
			: base()
		{
		}
		#endregion

		#region Initialization
		public override void Initialize(Game game, GraphicsDevice device)
		{
			base.Initialize(game, device);
		}

		protected override void LoadGraphicsContent(bool loadAllContent)
		{
			base.LoadGraphicsContent(loadAllContent);

			if (loadAllContent)
			{
				IGraphicsDeviceService graphicsService =
					(IGraphicsDeviceService)base.Game.Services.GetService(
												typeof(IGraphicsDeviceService));

				vertexDecl = new VertexDeclaration(
									graphicsService.GraphicsDevice,
									VertexPositionTexture.VertexElements);

				verts = new VertexPositionTexture[]
                        {
                            new VertexPositionTexture(
                                new Vector3(0,0,0),
                                new Vector2(1,1)),
                            new VertexPositionTexture(
                                new Vector3(0,0,0),
                                new Vector2(0,1)),
                            new VertexPositionTexture(
                                new Vector3(0,0,0),
                                new Vector2(0,0)),
                            new VertexPositionTexture(
                                new Vector3(0,0,0),
                                new Vector2(1,0))
                        };

				ib = new short[] { 0, 1, 2, 2, 3, 0 };

				quadRenderEffect = XeGame.ContentManager.Load<Effect>(@"Content\Effects\VertexRenderer");
			}
		}

		protected override void UnloadGraphicsContent(bool unloadAllContent)
		{
			base.UnloadGraphicsContent(unloadAllContent);

			if (unloadAllContent)
			{
				if (vertexDecl != null)
					vertexDecl.Dispose();

				vertexDecl = null;
			}
		}
		#endregion

		public override void RenderQuad(QuadBase quad)
		{
			// If the quad isn't worthy, don't add it.
			if (quad == null)
				return;

			base.RenderQuad(quad);

			// Add the quad to the queue
			m_quads.Enqueue(quad);

			if (!this.UseBreadthFirst)
				Flush();
		}

		public override void Flush()
		{
			base.Flush();

			// Return if there are no quads to draw
			if (m_quads.Count <= 0)
				return;

			IGraphicsDeviceService graphicsService = (IGraphicsDeviceService)
			   base.Game.Services.GetService(typeof(IGraphicsDeviceService));

			float width = graphicsService.GraphicsDevice.PresentationParameters.BackBufferWidth;
			float height = graphicsService.GraphicsDevice.PresentationParameters.BackBufferHeight;


			bool AlphaEnable = graphicsService.GraphicsDevice.RenderState.AlphaBlendEnable;
			Blend DestBlend = graphicsService.GraphicsDevice.RenderState.DestinationBlend;
			Blend SourceBlend = graphicsService.GraphicsDevice.RenderState.SourceBlend;

			//graphicsService.GraphicsDevice.RenderState.FillMode = FillMode.WireFrame;
			graphicsService.GraphicsDevice.RenderState.AlphaBlendEnable = true;
			graphicsService.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
			graphicsService.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;

			// Flush out the quads
			quadRenderEffect.Begin();
			quadRenderEffect.Techniques[0].Passes[0].Begin();


			while (m_quads.Count > 0)
			{
				// Get the quad.
				QuadBase _quad = m_quads.Dequeue();

				// If the texture is null, don't bother.
				if (_quad.Texture == null)
					continue;

				// Render the quad.
				GraphicsDevice device = graphicsService.GraphicsDevice;
				device.VertexDeclaration = vertexDecl;

				quadRenderEffect.Parameters["colorTexture"].SetValue(_quad.Texture);

				verts[0].Position = new Vector3(((_quad.DestRect.Right / width) - 0.5f) * 2,
												((_quad.DestRect.Bottom / -height) + 0.5f) * 2, 1);

				verts[1].Position = new Vector3(((_quad.DestRect.Left / width - 0.5f) * 2),
												((_quad.DestRect.Bottom / -height) + 0.5f) * 2, 1);

				verts[2].Position = new Vector3(((_quad.DestRect.Left / width) - 0.5f) * 2,
												((_quad.DestRect.Top / -height) + 0.5f) * 2, 1);

				verts[3].Position = new Vector3(((_quad.DestRect.Right / width) - 0.5f) * 2,
												((_quad.DestRect.Top / -height) + 0.5f) * 2, 1);


				verts[0].TextureCoordinate = new Vector2((float)_quad.SrcRect.Right / (float)_quad.Texture.Width,
														(float)_quad.SrcRect.Bottom / (float)_quad.Texture.Height);

				verts[1].TextureCoordinate = new Vector2((float)_quad.SrcRect.Left / (float)_quad.Texture.Width,
														(float)_quad.SrcRect.Bottom / (float)_quad.Texture.Height);

				verts[2].TextureCoordinate = new Vector2((float)_quad.SrcRect.Left / (float)_quad.Texture.Width,
														(float)_quad.SrcRect.Top / (float)_quad.Texture.Height);

				verts[3].TextureCoordinate = new Vector2((float)_quad.SrcRect.Right / (float)_quad.Texture.Width,
														(float)_quad.SrcRect.Top / (float)_quad.Texture.Height);

				device.DrawUserIndexedPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, verts, 0, 4, ib, 0, 2);
			}

			quadRenderEffect.Techniques[0].Passes[0].End();
			quadRenderEffect.End();

			graphicsService.GraphicsDevice.RenderState.AlphaBlendEnable = AlphaEnable;
			graphicsService.GraphicsDevice.RenderState.DestinationBlend = DestBlend;
			graphicsService.GraphicsDevice.RenderState.SourceBlend = SourceBlend;

		}


		#region Properties
		protected Queue<QuadBase> Quads
		{
			get { return m_quads; }
			set { m_quads = value; }
		}
		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			if (vertexDecl != null)
				vertexDecl.Dispose();

			vertexDecl = null;
		}

		#endregion
	}
}
