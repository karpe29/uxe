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
    internal class TextBoxText
    {
        #region Members
        public string FontName;
        public SpriteFont Font;
        public Color Color;
        public float Rotation;
        public Vector2 Position;
        public Vector2 Offset;
        public Vector2 Origin;
        public Vector2 Scale;
        public SpriteEffects Effects;
        public float LayerDepth;

        public Rectangle Dest;
        public TextAlign Align = TextAlign.Left;
        public BreakStyle Break = BreakStyle.Word;

        public List<TextBoxLine> Lines = new List<TextBoxLine>();
        public string Text;

        public int LineIndex = 0;
        public int OverflowIndex = 0;
        public int NumVisibleLines = 0;

        public int HorizontalIndex = 0;
        public int HorizOverflowIndex = 0;
        public int NumVisibleCharacters = 0;

        public float m_maxLineWidth = 0;
        #endregion

        public void Build()
        {
            Position.X = Dest.X;
            Position.Y = Dest.Y;

            switch (Break)
            {
                default:
                case BreakStyle.Word:
                    Text = HandleWordBreak(Text, Dest);
                    break;
                case BreakStyle.Letter:
                    Text = HandleLetterBreak(Text, Dest);
                    break;
                case BreakStyle.None:
                    Text = HandleNoBreak(Text, Dest);
                    break;
            }

            for (int i = 0; i < LineIndex; i++)
            {
                if (i >= Lines.Count)
                    break;

                Offset.Y -= Font.MeasureString(Lines[i].Text).Y;
            }

            switch (Align)
            {
                case TextAlign.Center:
                    //Origin += new Vector2(m_maxLineWidth / 2, 0);
                    for (int i = 0; i < Lines.Count; i++)
                    {
                        Lines[i].Position.X += (((float)Dest.Width - Lines[i].Measure.X) / 2.0f) + 0.5f;
                    }
                    break;
                case TextAlign.Right:
                    for (int i = 0; i < Lines.Count; i++)
                    {
                        Lines[i].Position.X += (float)Dest.Width - Lines[i].Measure.X;
                    }
                    break;
            }
        }

        #region Handle Breaking
        protected virtual string HandleWordBreak(string text, Rectangle box)
        {
            Lines.Clear();

            if (text.Length == 1)
            {
                Lines.Add(new TextBoxLine(text, new Vector2(box.X, box.Y), false));
                NumVisibleLines = 1;

                return text;
            }

            // Holds the measure for text
            Vector2 _measure;

            // The current text to be returned
            string _text = "";
            string _fullText = "";

            bool _overflowed = false;

            // The next text to be added.
            string _next = "";

            // Current index in the text
            int _curIndex = 0;

            // The next index in the text to read to.
            int _nextIndex = (text.IndexOf(" ", _curIndex) != -1) ? text.IndexOf(" ", _curIndex) : text.IndexOf("\n", _curIndex);

            // If it is still -1, try to get the last index in the string.
            if (_nextIndex == -1 && _curIndex < text.Length - 1)
                _nextIndex = text.Length - 1;

            int _curX = box.X, _curY = box.Y;

            int _lineCount = 0;

            // While there is more text to be added.
            while (_nextIndex != -1)
            {
                // Get the next text to add
                _next = text.Substring(_curIndex, _nextIndex - _curIndex);

                // Measure the new "possible" string
                _measure = Font.MeasureString(_text + _next);

                // Check against the width bound of the box
                if (_measure.X > box.Width)
                {
                    _measure = Font.MeasureString(_text);

                    // If it goes past, add a break.
                    Lines.Add(new TextBoxLine(_text, new Vector2(_curX, _curY), _overflowed, _measure));

                    if (_measure.X > m_maxLineWidth)
                        m_maxLineWidth = _measure.X;

                    _curY += (int)_measure.Y;

                    if(!_overflowed)
                        _lineCount++;

                    _fullText += "\n";
                    _text = "";

                    // Remeasure to check against the height
                    _measure = Font.MeasureString(_fullText + _next);

                    // If it breaks the height, break
                    // out of the loop
                    if (_measure.Y > box.Height && !_overflowed)
                    {
                        _overflowed = true;

                        OverflowIndex = _lineCount;
                        NumVisibleLines = _lineCount;
                    }
                }

                //if (!_overflowed)
                //{
                    // Add the next string to the current text
                    _text += text.Substring(_curIndex, _nextIndex - _curIndex + 1);
                //}
                _fullText += text.Substring(_curIndex, _nextIndex - _curIndex + 1);

                // Move to the next index
                _curIndex = _nextIndex + 1;

                // Get the next index of the next space or newline
                _nextIndex = (text.IndexOf(" ", _curIndex) != -1) ? text.IndexOf(" ", _curIndex) : text.IndexOf("\n", _curIndex);

                // If it is still -1, try to get the last index in the string.
                if (_nextIndex == -1 && _curIndex < text.Length - 1)
                    _nextIndex = text.Length - 1;
            }

            Lines.Add(new TextBoxLine(_text, new Vector2(_curX, _curY), _overflowed, Font.MeasureString(_text)));

            if ( !_overflowed )
            {
                OverflowIndex = _lineCount;
                NumVisibleLines = _lineCount + 1;
            }

            // Return the formatted text.
            return _fullText;
        }

        protected virtual string HandleLetterBreak(string text, Rectangle box)
        {
            Lines.Clear();

            if (text.Length == 1)
            {
                Lines.Add(new TextBoxLine(text, new Vector2(box.X, box.Y), false));
                NumVisibleLines = 1;
                NumVisibleCharacters = 1;

                return text;
            }

            // Used to measure the text
            Vector2 _measure;

            // The current text to return.
            string _text = "";
            string _fullText = "";

            bool _overflowed = false;

            int _curX = box.X, _curY = box.Y;

            int _lineCount = 0;

            // Loop through all the characters in the text.
            for (int i = 0; i < text.Length; i++)
            {
                // Measure with the character added.
                _measure = Font.MeasureString(_text + text[i].ToString());

                // If it breaks the width.
                if (_measure.X > box.Width)
                {
                    _measure = Font.MeasureString(_text);

                    // Add a newline.
                    Lines.Add(new TextBoxLine(_text, new Vector2(_curX, _curY), _overflowed, _measure));

                    if (_measure.X > m_maxLineWidth)
                        m_maxLineWidth = _measure.X;

                    _curY += (int)_measure.Y;
                    _text = "";
                    _fullText += "\n";

                    if (!_overflowed)
                        _lineCount++;

                    // Retest with the newline and the character.
                    _measure = Font.MeasureString(_fullText + text[i].ToString());

                    // Check the height bound, if this fails, just exit.
                    if (_measure.Y > box.Height && !_overflowed)
                    {
                        _overflowed = true;

                        OverflowIndex = _lineCount;
                        NumVisibleLines = _lineCount;
                    }
                }

                //if (!_overflowed)
                //{
                    // Add the character to the string.
                    _text += text[i].ToString();
                //}
                _fullText += text[i].ToString();
            }

            Lines.Add(new TextBoxLine(_text, new Vector2(_curX, _curY), _overflowed, Font.MeasureString(_text)));

            if ( !_overflowed )
            {
                OverflowIndex = _lineCount;
                NumVisibleLines = _lineCount + 1;
            }

            // Return the formatted text.
            return _fullText;
        }

        protected virtual string HandleNoBreak(string text, Rectangle box)
        {
            Lines.Clear();

            Vector2 _measure;

            string _text = "";
            string _fullText = "";

            bool _overflowed = false;

            for (int i = HorizontalIndex; i < text.Length; i++)
            {
                if (text[i] == '\n')
                    continue;

                _measure = Font.MeasureString(_text + text[i]);

                if (_measure.X > box.Width)
                {
                    if (!_overflowed)
                    {
                        _overflowed = true;

                        HorizOverflowIndex = i;
                        NumVisibleCharacters = i - HorizontalIndex;
                    }
                }
                else
                {
                    _text += text[i];
                }

                _fullText += text[i];
            }

            Lines.Add(new TextBoxLine(_text, new Vector2(box.X, box.Y), false, Font.MeasureString(_text)));

            LineIndex = 0;
            NumVisibleLines = 1;
            OverflowIndex = 2;

            return _fullText;
        }
        #endregion
    }

    internal class TextBoxLine
    {
        public string Text;
        public Vector2 Position;
        public bool IsOverflow = false;
        public Vector2 Measure;

        public TextBoxLine()
        {
        }

        public TextBoxLine(string text)
        {
            Text = text;
        }

        public TextBoxLine(string text, Vector2 position)
            : this(text)
        {
            Position = position;
        }

        public TextBoxLine(string text, Vector2 position, bool isOverflow)
            : this(text, position)
        {
            isOverflow = false;
        }

        public TextBoxLine(string text, Vector2 position, bool isOverflow, Vector2 measure)
            : this(text, position, isOverflow)
        {
            Measure = measure;
        }
    }

    public enum BreakStyle
    {
        Letter,
        Word,
        None
    }
}
