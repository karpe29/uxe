#region Using Statements
using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Xe.GUI
{
    /// <summary>
    /// This is a game component that provides basic, 2D font drawing
    /// and management support.
    /// </summary>
    public class Font2D : Microsoft.Xna.Framework.GameComponent
    {
        #region Members
        // Holds all the fonts.
        private Dictionary<string, SpriteFont> m_fonts = new Dictionary<string, SpriteFont>();

        // Reference to the Graphics Device.
        private GraphicsDevice m_graphicsDevice;

        // Used for drawing text.
        private SpriteBatch m_sBatch;

        // Used for loading fonts.
        private ContentManager m_conManager;

        // Whether or not the graphics have been loaded.
        private bool m_graphicsLoaded = false;

        private Queue<object> m_strings = new Queue<object>();
        #endregion

        public Font2D(Game game)
            : base(game)
        {
            // TODO: Construct any child components here
        }

        #region Loading / Unloading Content
        public void LoadGraphicsContent(bool loadAllContent)
        {
            if (loadAllContent)
            {
                // Get the graphics device
                m_graphicsDevice = ((IGraphicsDeviceService)this.Game.Services.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;

                // Instantiate the ContentManager
                m_conManager = new ContentManager(this.Game.Services);

                // Instantiate the SpriteBatch
                m_sBatch = new SpriteBatch(this.GraphicsDevice);

                // We have loaded the graphics data...
                m_graphicsLoaded = true;
            }
        }

        public void UnloadGraphicsContent(bool unloadAllContent)
        {
            if (unloadAllContent)
            {
                // Unload and Dispose of the ContentManager
                if (m_conManager != null)
                {
                    m_conManager.Unload();
                    m_conManager.Dispose();
                }
                m_conManager = null;

                // Get rid of the SpriteBatch
                if (m_sBatch != null)
                    m_sBatch.Dispose();
                m_sBatch = null;

                // We have unloaded the graphics data...
                m_graphicsLoaded = false;
            }
        }
        #endregion

        #region Loading and Unloading a Font
        public void LoadFont(string assetName, string fontName)
        {
            try
            {
                if (!m_graphicsLoaded || m_fonts.ContainsKey(fontName))
                    return;

                m_fonts[fontName] = m_conManager.Load<SpriteFont>(assetName);
            }
            catch (Exception e)
            {
                // TODO: Do something with the error...
                Console.WriteLine(e.Message);
            }
        }

        public void UnloadFont(string fontName)
        {
            try
            {
                // If it doesn't exist, just return
                if (!m_fonts.ContainsKey(fontName))
                    return;

                // Get a local copy
                SpriteFont _font = m_fonts[fontName];

                // Remove the font
                m_fonts.Remove(fontName);

                // Set the font to null
                _font = null;
            }
            catch (Exception e)
            {
                // TODO: Do something with the error...
                Console.WriteLine(e.Message);
            }
        }
        #endregion

        #region Drawing a String
        /// <summary>
        /// Draws a string.
        /// </summary>
        /// <param name="fontName">The font to be used.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="position">The position of the text.</param>
        /// <param name="color">The fore color of the text.</param>
        public void DrawString(string fontName, string text, Vector2 position, Color color)
        {
            // If the font doesn't exist, don't bother
            if (!m_fonts.ContainsKey(fontName))
                return;

            this.DrawString(fontName, text, position, color, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);
        }

        /// <summary>
        /// Draws a string.
        /// </summary>
        /// <param name="fontName">The font to be used.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="position">The position of the text.</param>
        /// <param name="color">The fore color of the text.</param>
        /// <param name="scale">The size of the text. Default is 1.0f.</param>
        public void DrawString(string fontName, string text, Vector2 position, Color color, float scale)
        {
            // Return if the font doesn't exist
            if (!m_fonts.ContainsKey(fontName))
                return;

            // Get the center of the test
            Vector2 _origin = m_fonts[fontName].MeasureString(text) / 2.0f;

            // Draw the string
            this.DrawString(fontName, text, position, color, 0.0f, _origin, new Vector2(scale, scale), SpriteEffects.None, 0.0f);
        }

        /// <summary>
        /// Draws a string.
        /// </summary>
        /// <param name="fontName">The font to be used.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="position">The position of the text.</param>
        /// <param name="color">The fore color of the text.</param>
        /// <param name="scale">The size of the text. Default is 1.0f.</param>
        /// <param name="effects">The sprite effects to apply to the string.</param>
        /// <param name="layerDepth">The depth at which to draw. Default is 0.0f.</param>
        /// <param name="origin">The point at which to draw from.</param>
        /// <param name="rotation">The rotation to apply to the string.</param>
        public void DrawString(string fontName, string text, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
        {
            // Draw the strng
            this.DrawString(fontName, text, position, color, rotation, origin, new Vector2(scale, scale), effects, layerDepth);
        }

        /// <summary>
        /// Draws a string.
        /// </summary>
        /// <param name="fontName">The font to be used.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="position">The position of the text.</param>
        /// <param name="color">The fore color of the text.</param>
        /// <param name="scale">The size of the text. Default is 1.0f.</param>
        /// <param name="effects">The sprite effects to apply to the string.</param>
        /// <param name="layerDepth">The depth at which to draw. Default is 0.0f.</param>
        /// <param name="origin">The point at which to draw from.</param>
        /// <param name="rotation">The rotation to apply to the string.</param>
        public void DrawString(string fontName, string text, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
        {
            // If the font doesn't exist, don't bother
            if (!m_fonts.ContainsKey(fontName))
                return;

            // Setup the string struct
            StringToDraw _string = new StringToDraw();
            _string.FontName = fontName;
            _string.Text = text;
            _string.Position = position;
            _string.Color = color;
            _string.Rotation = rotation;
            _string.Origin = origin;
            _string.Scale = scale;
            _string.Effects = effects;
            _string.LayerDepth = layerDepth;

            m_strings.Enqueue(_string);
        }

        #region Drawing a TextBox
        /// <summary>
        /// Draws a TextBox
        /// </summary>
        /// <param name="fontName">The name of the Font to draw with.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="position">The position to draw.</param>
        /// <param name="size">The size of the TextBox.</param>
        /// <param name="color">The Color of the text.</param>
        public void DrawTextBox(string fontName, string text, Vector2 position, Vector2 size, Color color)
        {
            DrawTextBox(fontName, text, TextAlign.Left, BreakStyle.Word, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), color);
        }

        /// <summary>
        /// Draws a TextBox
        /// </summary>
        /// <param name="fontName">The name of the Font to draw with.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="box">The rectangle parameters of the TextBox.</param>
        /// <param name="color">The Color of the text.</param>
        public void DrawTextBox(string fontName, string text, Rectangle box, Color color)
        {
            DrawTextBox(fontName, text, 0, TextAlign.Left, BreakStyle.Word, box, color);
        }

        /// <summary>
        /// Draws a TextBox
        /// </summary>
        /// <param name="fontName">The name of the Font to draw with.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="align">The alignment of the text in the TextBox.</param>
        /// <param name="position">The position to draw.</param>
        /// <param name="size">The size of the TextBox.</param>
        /// <param name="color">The Color of the text.</param>
        public void DrawTextBox(string fontName, string text, TextAlign align, Vector2 position, Vector2 size, Color color)
        {
            DrawTextBox(fontName, text, 0, align, BreakStyle.Word, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), color);
        }

        /// <summary>
        /// Draws a TextBox
        /// </summary>
        /// <param name="fontName">The name of the Font to draw with.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="align">The alignment of the text in the TextBox.</param>
        /// <param name="box">The rectangle parameters of the TextBox.</param>
        /// <param name="color">The Color of the text.</param>
        public void DrawTextBox(string fontName, string text, TextAlign align, Rectangle box, Color color)
        {
            DrawTextBox(fontName, text, 0, align, BreakStyle.Word, box, color);
        }

        public void DrawTextBox(string fontName, string text, int lineIndex, TextAlign align, Rectangle box, Color color)
        {
            DrawTextBox(fontName, text, lineIndex, align, BreakStyle.Word, box, color);
        }

        /// <summary>
        /// Draws a TextBox
        /// </summary>
        /// <param name="fontName">The name of the Font to draw with.</param>
        /// <param name="text">The text to draw.</param>
        /// <param name="align">The alignment of the text in the TextBox.</param>
        /// <param name="breakStyle">The style of line breaking / word wrapping to use.</param>
        /// <param name="box">The rectangle parameters of the TextBox.</param>
        /// <param name="color">The Color of the text.</param>
        public void DrawTextBox(string fontName, string text, TextAlign align, BreakStyle breakStyle, Rectangle box, Color color)
        {
            DrawTextBox(fontName, text, 0, align, breakStyle, box, color);
        }

        public void DrawTextBox(string fontName, string text, int lineIndex, TextAlign align, BreakStyle breakStyle, Rectangle box, Color color)
        {
            DrawTextBox(fontName, text, lineIndex, 0, align, breakStyle, box, color);
        }

        public void DrawTextBox(string fontName, string text, int lineIndex, int charIndex, TextAlign align, BreakStyle breakStyle, Rectangle box, Color color)
        {
            // If the font doesn't exist, don't bother
            if (!m_fonts.ContainsKey(fontName))
                return;

            // Setup the string struct
            TextBoxText _string = new TextBoxText();
            _string.FontName = fontName;
            _string.Font = m_fonts[fontName];
            _string.Text = text;
            _string.Color = color;

            _string.Rotation = 0f;
            _string.Scale.X = 1.0f;
            _string.Scale.Y = 1.0f;
            _string.Effects = SpriteEffects.None;
            _string.LayerDepth = 0.0f;

            _string.Break = breakStyle;

            _string.Dest = box;
            _string.Align = align;

            _string.LineIndex = lineIndex;
            _string.HorizontalIndex = charIndex;

            _string.Build();

            m_strings.Enqueue(_string);
        }
        #endregion

        /// <summary>
        /// Draws all the cached strings.
        /// </summary>
        public void Flush()
        {
            // Begin the SpriteBatch
            m_sBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.SaveState);

            // Draw the string
            while (m_strings.Count > 0)
            {
                // Dequeue the next string
                object _string = m_strings.Dequeue();

                // Handle drawing a textbox as a special case!
                if (_string is TextBoxText)
                {
                    TextBoxText _temp = _string as TextBoxText;

                    //m_sBatch.DrawString(m_fonts[_temp.FontName], _temp.Text, _temp.Position, _temp.Color, _temp.Rotation, _temp.Origin, _temp.Scale, _temp.Effects, _temp.LayerDepth);
                    //foreach(TextBoxLine _line in _temp.Lines)

                    for(int i = _temp.LineIndex; i < _temp.LineIndex + _temp.NumVisibleLines; i++)
                    {
                        if ( i >= _temp.Lines.Count )
                            break;

                        m_sBatch.DrawString(m_fonts[_temp.FontName], _temp.Lines[i].Text, _temp.Lines[i].Position + _temp.Offset, _temp.Color, _temp.Rotation, _temp.Origin, _temp.Scale, _temp.Effects, _temp.LayerDepth);
                    }
                }
                else
                {
                    StringToDraw _temp = _string as StringToDraw;

                    // Draw the string
                    m_sBatch.DrawString(m_fonts[_temp.FontName], _temp.Text, _temp.Position, _temp.Color, _temp.Rotation, _temp.Origin, _temp.Scale, _temp.Effects, _temp.LayerDepth);
                }
            }

            // End the SpriteBatch
            m_sBatch.End();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or Sets the Graphics Device object.
        /// </summary>
        protected GraphicsDevice GraphicsDevice
        {
            get { return m_graphicsDevice; }
            set { m_graphicsDevice = value; }
        }

        /// <summary>
        /// Gets the collection of fonts.
        /// </summary>
        public Dictionary<string, SpriteFont> Fonts
        {
            get { return m_fonts; }
            set { m_fonts = value; }
        }

        /// <summary>
        /// Gets whether or not the graphics have been
        /// initialized and loaded.
        /// </summary>
        public bool GraphicsLoaded
        {
            get { return m_graphicsLoaded; }
        }

        /// <summary>
        /// Gets or Sets the Content Manager.
        /// </summary>
        public ContentManager ContentManager
        {
            get { return m_conManager; }
            set { m_conManager = value; }
        }

        /// <summary>
        /// Gets or Sets the Sprite Batch.
        /// </summary>
        public SpriteBatch SpriteBatch
        {
            get { return m_sBatch; }
            set { m_sBatch = value; }
        }

        /// <summary>
        /// Gets or Sets a SpriteFont in the Fonts collection.
        /// </summary>
        /// <param name="index">Font Name to get.</param>
        /// <returns>A SpriteFont object.</returns>
        public SpriteFont this[string index]
        {
            get { return m_fonts[index]; }
            set { m_fonts[index] = value; }
        }
        #endregion
    }

    internal class StringToDraw
    {
        public string FontName;
        public string Text;
        public Vector2 Position;
        public Color Color;
        public float Rotation;
        public Vector2 Origin;
        public Vector2 Scale;
        public SpriteEffects Effects;
        public float LayerDepth;
    }

    public enum TextAlign
    {
        Left,
        Center,
        Right
    }
}


