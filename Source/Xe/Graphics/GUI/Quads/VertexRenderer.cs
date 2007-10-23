#region Using Statements
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Xe.GUI
{
    public class VertexRenderer : QuadRenderer
    {
        #region Members
		VertexDeclaration vertexDecl = null;
		VertexPositionTexture[] verts = null;
		short[] ib = null;

		Effect quadRenderEffect;

        private Queue<QuadBase> m_quads = new Queue<QuadBase>();

        private Vector2 m_vectorOne = new Vector2(1, 1);
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

				verts[0].Position.X = _quad.DestRect.Right;
				verts[0].Position.Y = _quad.DestRect.Top;

				verts[1].Position.X = _quad.DestRect.Left;
				verts[1].Position.Y = _quad.DestRect.Top;

				verts[2].Position.X = _quad.DestRect.Left;
				verts[2].Position.Y = _quad.DestRect.Bottom;

				verts[3].Position.X = _quad.DestRect.Right;
				verts[3].Position.Y = _quad.DestRect.Bottom;

				device.DrawUserIndexedPrimitives<VertexPositionTexture>
					(PrimitiveType.TriangleStrip, verts, 0, 4, ib, 0, 2);
            }

			quadRenderEffect.Techniques[0].Passes[0].End();
			quadRenderEffect.End();
        }

        #region Properties
        protected Queue<QuadBase> Quads
        {
            get { return m_quads; }
            set { m_quads = value; }
        }
        #endregion
    }
}
