#region Using Statements
using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#endregion

namespace Xe.Gui
{
    public sealed class Path
    {
        #region Members
        private Path m_nextPath;

        private float m_totalTime = 0.25f;
        private float m_curTime = 0;

        private string m_xFunc = "iX-100*t";
        private string m_yFunc = "y";
        private string m_wFunc = "iW+200*t";
        private string m_hFunc = "h";

        private IMoveable m_object;

        private bool m_goingForward = true;
        #endregion

        public Path(IMoveable obj)
        {
            m_object = obj;
        }

        public bool Update(GameTime gameTime)
        {
            bool _return = true;

            float _et = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (m_goingForward)
            {
                if (m_curTime >= m_totalTime)
                {
                    m_curTime = m_totalTime;
                    _return = false;
                }
                else
                    m_curTime += _et;
            }
            else
            {
                if (m_curTime <= 0)
                {
                    m_curTime = 0.0f;
                    _return = false;

                    //Console.WriteLine(m_object.Width);
                }
                else
                    m_curTime -= _et;
            }

            //string _time = String.Format("{0:##.####}", m_curTime);
            //string _iX = String.Format("{0:##.####}", m_object.InitialPos.X);
            //string _iW = String.Format("{0:##.####}", m_object.InitialSize.X);
            //m_object.Width = (float)EquationSolver.SolvePar(m_wFunc.Replace("t", _time).Replace("iW", _iW));
            //m_object.X = (float)EquationSolver.SolvePar(m_xFunc.Replace("t", _time).Replace("iX", _iX));

            return _return;
            //m_object.X = (float)EquationSolver.SolvePar(m_xFunc.Replace("x", m_object.X.ToString()).Replace("t", m_curTime.ToString()));
            //m_object.Y = (float)EquationSolver.SolvePar(m_xFunc.Replace("y", m_object.Y.ToString()).Replace("t", m_curTime.ToString()));
        }

        public void MoveTo(Path p)
        {
        }

        #region Properties
        public bool GoingForward
        {
            get { return m_goingForward; }
            set { m_goingForward = value; }
        }

        public float TotalTime
        {
            get { return m_totalTime; }
            set { m_totalTime = value; }
        }

        public float CurrentTime
        {
            get { return m_curTime; }
        }

        public string WidthFunction
        {
            get { return m_wFunc; }
            set { m_wFunc = value; }
        }

        public string XFunction
        {
            get { return m_xFunc; }
            set { m_xFunc = value; }
        }
        #endregion
    }
}
