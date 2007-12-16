#region Using Statements
using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Xe.Tools;
using System.Xml;
using Xe.Tools.Xml;
#endregion

namespace Xe.Gui
{
    public class AnimatedQuad : QuadBase
    {
        #region Members
        // Maintain the current index in the list of rectangles
        private int m_curIndex = 0;

        // The current frame's time
        private float m_curTime = 0.0f;

        // List of source rectangles
        private List<Frame> m_frames = new List<Frame>();

        private bool m_isLooping = true;
        private bool m_isReversing = true;

        private bool m_isForward = true;
        #endregion

        #region Constructor
        public AnimatedQuad(Game game)
            : base(game)
        {
        }

        public AnimatedQuad(Game game, float x, float y, float width, float height)
            : base(game, x, y, width, height)
        {
        }
        #endregion

        #region Adding Rectangles
        /// <summary>
        /// Adds a rectangle to the list of sources.
        /// </summary>
        /// <param name="src">The rectangle to add that specifies a texture location in pixels.</param>
        public void AddSourceRect(Rectangle src)
        {
            AddSourceRect(src, 0.15f);
        }

        /// <summary>
        /// Adds a rectangle to the list of sources.
        /// </summary>
        /// <param name="src">The rectangle to add that specifies a texture location in pixels.</param>
        /// <param name="timeSpan">Length of time the frame should last.</param>
        public void AddSourceRect(Rectangle src, float timeSpan)
        {
            if (m_frames.Count == 0)
            {
                this.U = src.X;
                this.V = src.Y;
                this.UVWidth = src.Width;
                this.UVHeight = src.Height;
            }

            Frame _frame = new Frame();
            _frame.SourceRect = src;
            _frame.TimeSpan = timeSpan;

            // Add a rectangle to the list
            m_frames.Add(_frame);
        }

        /// <summary>
        /// Adds a set of rectangles to the list of sources.
        /// </summary>
        /// <param name="initial">The initial rectangle to add that specifies a texture location in pixels.</param>
        /// <param name="columns">The number of columns to loop through.</param>
        /// <param name="rows">The number of rows to loop through.</param>
        /// <param name="amount">The amount of rectangles to add.</param>
        public void AddSourceSet(Rectangle initial, int columns, int rows, int amount)
        {
            // Pass in an empty offset
            AddSourceSet(initial, new Vector2(0, 0), columns, rows, amount, 0.15f);
        }

        /// <summary>
        /// Adds a set of rectangles to the list of sources.
        /// </summary>
        /// <param name="initial">The initial rectangle to add that specifies a texture location in pixels.</param>
        /// <param name="columns">The number of columns to loop through.</param>
        /// <param name="rows">The number of rows to loop through.</param>
        /// <param name="amount">The amount of rectangles to add.</param>
        /// <param name="timeSpan">Length of time the frame should last.</param>
        public void AddSourceSet(Rectangle initial, int columns, int rows, int amount, float timeSpan)
        {
            // Pass in an empty offset
            AddSourceSet(initial, new Vector2(0, 0), columns, rows, amount, timeSpan);
        }

        /// <summary>
        /// Adds a set of rectangles to the list of sources.
        /// </summary>
        /// <param name="initial">The initial rectangle to add that specifies a texture location in pixels.</param>
        /// <param name="columns">The number of columns to loop through.</param>
        /// <param name="rows">The number of rows to loop through.</param>
        /// <param name="amount">The amount of rectangles to add.</param>
        /// <param name="offset">The amount of offset each new rectangle has from the last offset,
        /// minus the width and height of the initial rectangle.</param>
        /// <param name="timeSpan">Length of time the frame should last.</param>
        public void AddSourceSet(Rectangle initial, Vector2 offset, int columns, int rows, int amount, float timeSpan)
        {
            // Set the current count to 1 to include
            // the initial rectangle
            int _count = 1;

            // Add the initial rectangle
            AddSourceRect(initial);

            // Loop through each row
            for (int k = 1; k <= rows; k++ )
            {
                // Loop through each column
                for (int i = 1; i <= columns; i++)
                {
                    // If the total number has been reached,
                    // return
                    if (_count >= amount)
                        return;

                    // Create a new rectangle
                    Rectangle _temp = new Rectangle();
                    _temp.X = i * initial.Width + (int)offset.X + initial.X;
                    _temp.Y = k * initial.Y + (int)offset.Y + initial.Y;
                    _temp.Width = initial.Width;
                    _temp.Height = initial.Height;

                    // Add it
                    AddSourceRect(_temp, timeSpan);

                    // Increase the count!
                    _count++;
                }
            }
        }

        public void AddSourceSet(List<Rectangle> rectangles)
        {
            for (int i = 0; i < rectangles.Count; i++)
                AddSourceRect(rectangles[i]);
        }

        public void AddSourceSet(List<Rectangle> rectangles, float timeSpan)
        {
            for (int i = 0; i < rectangles.Count; i++)
                AddSourceRect(rectangles[i], timeSpan);
        }
        #endregion

        #region Loading
        public void Load(XmlNode node)
        {
            if (node.Name.ToLower() != "AnimatedQuad")
                return;

            foreach (XmlNode _child in node)
            {
                if (_child.Name == "Set")
                {
                    Rectangle _initial = XmlLoader.BuildRectangle(_child);
                    int _rows = 0, _cols = 0, _count = 0, _offsetX = 0, _offsetY = 0;
                    float _timeSpan = 0.15f;

                    if (!Parser.ParseFloat(XmlLoader.GetAttribute(_child, "timeSpan"), out _timeSpan))
                        _timeSpan = 0.15f;

                    Parser.ParseInt(XmlLoader.GetAttribute(_child, "rows"), out _rows);
                    Parser.ParseInt(XmlLoader.GetAttribute(_child, "cols"), out _cols);
                    Parser.ParseInt(XmlLoader.GetAttribute(_child, "count"), out _count);
                    Parser.ParseInt(XmlLoader.GetAttribute(_child, "offsetX"), out _offsetX);
                    Parser.ParseInt(XmlLoader.GetAttribute(_child, "offsetY"), out _offsetY);

                    AddSourceSet(_initial, new Vector2(_offsetX, _offsetY), _cols, _rows, _count, _timeSpan);
                }
                else if (_child.Name == "Rectangle")
                    AddSourceRect(XmlLoader.BuildRectangle(_child));
                else if (_child.Name == "FrameSet")
                    LoadFrameSet(_child);
            }
        }

        public void LoadFrameSet(XmlNode node)
        {
            if (node.Name.ToLower() != "FrameSet")
                return;

            foreach (XmlNode _child in node.ChildNodes)
            {
                if (_child.Name == "Frame")
                {
                    Rectangle _rect = XmlLoader.BuildRectangle(_child);
                    float _timeSpan = 0.15f;

                    if (!Parser.ParseFloat(XmlLoader.GetAttribute(_child, "timeSpan"), out _timeSpan))
                        _timeSpan = 0.15f;

                    AddSourceRect(_rect, _timeSpan);
                }
            }
        }
        #endregion

        #region Updating
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Get the elapsed time
            float _et = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Add it to the current time
            m_curTime += _et;

            // If it is time to, update the frame
            if (m_curTime > m_frames[m_curIndex].TimeSpan)
            {
                // Reset the current time
                m_curTime = 0.0f;

                // Increment the index, and if it goes passed 
                // the list's count, reset it.
                if (m_isForward)
                    m_curIndex++;
                else
                    m_curIndex--;

                if (m_isLooping)
                {
                    if (m_isReversing)
                    {
                        if (m_curIndex >= m_frames.Count)
                        {
                            m_curIndex = m_frames.Count - 1;
                            m_isForward = false;
                        }
                        else if (m_curIndex < 0)
                        {
                            m_curIndex = 0;
                            m_isForward = true;
                        }
                    }
                    else
                    {
                        if (m_curIndex >= m_frames.Count)
                            m_curIndex = 0;
                    }
                }

                // Reset the UV values.
                this.U = m_frames[m_curIndex].SourceRect.X;
                this.V = m_frames[m_curIndex].SourceRect.Y;
                this.UVWidth = m_frames[m_curIndex].SourceRect.Width;
                this.UVHeight = m_frames[m_curIndex].SourceRect.Height;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or Sets the current frame index.
        /// </summary>
        public int Index
        {
            get { return m_curIndex; }
            set
            {
                if (value < 0)
                    m_curIndex = 0;
                else if (value >= m_frames.Count)
                    m_curIndex = m_frames.Count - 1;
                else
                    m_curIndex = value;
            }
        }
        #endregion
    }
}
