#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xe.Tools;
#endregion

namespace Xe.Graphics2D
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SafeArea : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public SafeArea(Game game)
            : base(game)
        {
            // Adjust the Draw Order of the component so that it can be Draw last.
            // Currently set to 99, but this can be changed at any time.
            this.DrawOrder = 99;

            // TODO: Construct any child components here
        }

        SpriteBatch spriteBatch;
        Texture2D tex; // Holds a 1x1 texture containing a single white texel
        int width; // Viewport width
        int height; // Viewport height
        int dx; // 5% of width
        int dy; // 5% of height
        Color notActionSafeColor = new Color(255, 0, 0, 127); // Red, 50% opacity
        Color notTitleSafeColor = new Color(255, 255, 0, 127); // Yellow, 50% opacity

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            base.LoadGraphicsContent(loadAllContent);

            spriteBatch = new SpriteBatch(GraphicsDevice);
            tex = new Texture2D(GraphicsDevice, 1, 1, 1, ResourceUsage.None, SurfaceFormat.Color);
            Color[] texData = new Color[1];
            texData[0] = Color.White;
            tex.SetData<Color>(texData);
            width = GraphicsDevice.Viewport.Width;
            height = GraphicsDevice.Viewport.Height;
            dx = (int)(width * 0.05);
            dy = (int)(height * 0.05);

        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

            // Tint the non-action-safe area red
            spriteBatch.Draw(tex, new Rectangle(0, 0, width, dy), notActionSafeColor);
            spriteBatch.Draw(tex, new Rectangle(0, height - dy, width, dy), notActionSafeColor);
            spriteBatch.Draw(tex, new Rectangle(0, dy, dx, height - 2 * dy), notActionSafeColor);
            spriteBatch.Draw(tex, new Rectangle(width - dx, dy, dx, height - 2 * dy), notActionSafeColor);

            // Tint the non-title-safe area yellow
            spriteBatch.Draw(tex, new Rectangle(dx, dy, width - 2 * dx, dy), notTitleSafeColor);
            spriteBatch.Draw(tex, new Rectangle(dx, height - 2 * dy, width - 2 * dx, dy), notTitleSafeColor);
            spriteBatch.Draw(tex, new Rectangle(dx, 2 * dy, dx, height - 4 * dy), notTitleSafeColor);
            spriteBatch.Draw(tex, new Rectangle(width - 2 * dx, 2 * dy, dx, height - 4 * dy), notTitleSafeColor);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}


