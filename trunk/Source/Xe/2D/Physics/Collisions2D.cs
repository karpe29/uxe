#region License
/*
 *  Xna5D.Physics2D.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 *  
 *  This software is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 *  Date Created: December 03, 2006
*/
#endregion

#region Using Statements
using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Collections.Generic;

using XeFramework;
using XeFramework.Geometry;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace XeFramework.Physics
{
    public delegate void CollisionResponseHandler(IPhysical a, IPhysical b, Vector N);

    public partial class Collisions2D : Microsoft.Xna.Framework.GameComponent, ICollisions2DService, IService
    {
        #region Members
        protected List<IPhysical> m_items = new List<IPhysical>();

        protected int m_width, m_height;

        protected bool m_active = true;

        protected float m_futureCheck;
        protected bool m_useFrameTiming = true;

        protected double m_lowEndBuffer = 0.17;

        public event CollisionResponseHandler CollisionResponse;

        protected IReporterService m_reporter;
        #endregion

        #region Constructor and Initialization
        public Collisions2D(Game game)
            : base(game)
        {
            if (game != null)
                game.Services.AddService(typeof(ICollisions2DService), this);
        }

        public override void Initialize()
        {
            base.Initialize();

            m_reporter = (IReporterService)Game.Services.GetService(typeof(IReporterService));
        }
        #endregion

        #region Update
        public override void Update(GameTime gameTime)
        {
            if (!this.Enabled)
                return;

            if (m_useFrameTiming)
                m_futureCheck = (float)gameTime.ElapsedRealTime.TotalSeconds;

            m_width = ((IGraphicsDeviceService)Game.Services.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice.Viewport.Width;
            m_height = ((IGraphicsDeviceService)Game.Services.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice.Viewport.Height;

            for (int i = 0; i < m_items.Count; i++)
            {
                if (((ICollidable)m_items[i]).IsCollidable)
                    WallCheck((ICollidable)m_items[i]);
                else
                    continue;

                for (int j = i + 1; j < m_items.Count; j++)
                {
                    if (Collide(m_items[i], m_items[j]))
                    {
                        #if DEBUG
                        if (m_reporter != null)
                        {
                            string toWrite = String.Format("Collision between items: {0} and {1}.", m_items[i].ID, m_items[j].ID);
                            toWrite += String.Format("\r\n" + m_items[i].ID + ".X,Y: {0},{1}", m_items[i].X, m_items[i].Y);
                            toWrite += String.Format("\r\n" + m_items[j].ID + ".X,Y: {0},{1}", m_items[j].X, m_items[j].Y);

                            Message _msg = new Message(this);
                            _msg.Source = this.ID;
                            _msg.Destination = ((IService)m_reporter).ID;
                            _msg.Msg = toWrite;

                            m_reporter.BroadcastMessage(_msg);
                        }
                        #endif
                    }
                }
            }

            base.Update(gameTime);
        }
        #endregion

        #region Wall Check
        private void WallCheck(ICollidable obj)
        {
            double _width = m_width, _height = m_height;

            if ((Walls.Left & obj.Walls) == Walls.Left)
            {
                foreach (Vector p in obj.Vertices)
                {
                    if (p.X < 0)
                    {
                        ((IPhysical)obj).X++;
                        ((IPhysical)obj).Velocity.X *= (-1);

                        break;
                    }
                }
            }

            if ((Walls.Right & obj.Walls) == Walls.Right)
            {
                foreach (Vector p in obj.Vertices)
                {
                    if (p.X > _width)
                    {
                        ((IPhysical)obj).X--;
                        ((IPhysical)obj).Velocity.X *= (-1);

                        break;
                    }
                }
            }

            if ((Walls.Bottom & obj.Walls) == Walls.Bottom)
            {
                foreach (Vector p in obj.Vertices)
                {
                    if (p.Y > _height)
                    {
                        //Console.WriteLine("Collision on the Bottom!");

                        ((IPhysical)obj).Y--;
                        ((IPhysical)obj).Velocity.Y *= (-1);

                        break;
                    }
                }
            }

            if ((Walls.Top & obj.Walls) == Walls.Top)
            {
                foreach (Vector p in obj.Vertices)
                {
                    if (p.Y < 0)
                    {
                        ((IPhysical)obj).Y++;
                        ((IPhysical)obj).Velocity.Y *= (-1);

                        break;
                    }
                }
            }
        }
        #endregion

        #region Collision Detection
        private bool Collide(IPhysical a, IPhysical b)
        {
            Vector xMTD = new Vector();
            float t = 0.0f;
            Vector N = new Vector();

            Vector xRelPos = a.Position - b.Position;
            Vector xRelVel = a.Velocity - b.Velocity;

            if (Collide(((ICollidable)a).Vertices, ((ICollidable)b).Vertices, xRelPos, xRelVel, ref N, ref t))
            {
                if (t < 0.0f)
                {
                    #region DEBUG Output
#if DEBUG
                    if (m_reporter != null)
                    {
                        Message _msg = new Message(this);
                        _msg.Source = this.ID;
                        _msg.Destination = ((IService)m_reporter).ID;
                        _msg.Msg = "Collision detected, now processing overlap and collision!";

                        m_reporter.BroadcastMessage(_msg);
                    }
#endif
                    #endregion

                    ProcessOverlap(a, b, N * -t);

                    if (null != CollisionResponse)
                        CollisionResponse.Invoke(a, b, N);
                }
                else
                {
                    #region DEBUG Output
#if DEBUG
                    if (m_reporter != null)
                    {
                        Message _msg = new Message(this);
                        _msg.Source = this.ID;
                        _msg.Destination = ((IService)m_reporter).ID;
                        _msg.Msg = "Collision detected, now processing!";

                        m_reporter.BroadcastMessage(_msg);
                    }
#endif
                    #endregion

                    ProcessCollision(a, b, N, t);

                    if (null != CollisionResponse)
                        CollisionResponse.Invoke(a, b, N);
                }
                return true;
            }

            return false;
        }

        private bool Collide(List<Vector> vA, List<Vector> vB, Vector xOffset, Vector xVel, ref Vector N, ref float t)
        {
            List<Vector> xAxis = new List<Vector>();
            List<float> tAxis = new List<float>();

            #region Test Edges in A
            for (int j = vA.Count - 1, i = 0; i < vA.Count; j = i, i++)
            {
                Vector E0 = vA[j];
                Vector E1 = vA[i];
                Vector E = E1 - E0;

                xAxis.Add(new Vector(-E.Y, E.X));

                if (!IntervalIntersect(vA, vB, xAxis[i], xOffset, xVel, tAxis))
                    return false;
            }
            #endregion

            #region Test Edges in B
            for (int j = vB.Count - 1, i = 0; i < vB.Count; j = i, i++)
            {
                Vector E0 = vB[j];
                Vector E1 = vB[i];
                Vector E = E1 - E0;

                xAxis.Add(new Vector(-E.Y, E.X));

                if (!IntervalIntersect(vA, vB, xAxis[i], xOffset, xVel, tAxis))
                    return false;
            }
            #endregion

            // Push Vectors
            if (!FindMTD(xAxis, tAxis, ref N, ref t))
                return false;

            if (Vector.DotProduct(N, xOffset) < 0.0f)
                N = -N;

            return true;
        }

        private bool IntervalIntersect(List<Vector> vA, List<Vector> vB,
                                              Vector xAxis, Vector xOffset, Vector xVel,
                                              List<float> tAxis)
        {
            double min0 = 0, max0 = 0;
            double min1 = 0, max1 = 0;
            GetInterval(vA, xAxis, ref min0, ref max0);
            GetInterval(vB, xAxis, ref min1, ref max1);

            double h = Vector.DotProduct(xOffset, xAxis);
            /*min0 += h;
            max0 += h;*/

            double d0 = min0 - max1;
            double d1 = min1 - max0;

            if (d0 > 0.0f || d1 > 0.0f)
            {
                double v = Vector.DotProduct(xVel, xAxis);

                if (Math.Abs(v) < 0.0000001)
                    return false;

                double t0 = -d0 / v;
                double t1 = d1 / v;

                if (t0 > t1)
                {
                    double temp = t0;
                    t0 = t1;
                    t1 = temp;
                }

                float tAxisAdd = (t0 > 0.0) ? (float)t0 : (float)t1;
                tAxis.Add(tAxisAdd);

                if (tAxisAdd < 0.0f || tAxisAdd > m_futureCheck)
                    return false;

                return true;
            }
            else
            {
                // Changed from d0 < d1 - 11/15/06 (POSSIBLE FIX)
                float temp = (d0 > d1) ? (float)d0 : (float)d1;

                tAxis.Add(temp);

                return true;
            }
        }

        private bool FindMTD(List<Vector> xAxis, List<float> tAxis, ref Vector N, ref float t)
        {
            // Find the collision first
            int mini = -1;
            t = 0.0f;

            for (int i = 0; i < xAxis.Count; i++)
            {
                if (tAxis[i] > 0)
                {
                    if (tAxis[i] > t)
                    {
                        mini = i;
                        t = tAxis[i];
                        N = xAxis[i];
                        N.Normalize();
                    }
                }
            }

            if (mini != -1)
                return true;

            // No? Find overlaps
            mini = -1;
            for (int i = 0; i < xAxis.Count; i++)
            {
                double n = xAxis[i].Normalize();

                tAxis[i] /= (float)n;

                if (mini == -1 || tAxis[i] > t)
                {
                    mini = i;
                    t = tAxis[i];
                    N = xAxis[i];
                }
            }

            if (mini == -1)
            {
                #region DEBUG Output
#if DEBUG
                //System.Console.WriteLine("iCollisions.FindMTD(...): Error!");
                if (m_reporter != null)
                {
                    Message _msg = new Message(this);
                    _msg.Source = this.ID;
                    _msg.Destination = ((IService)m_reporter).ID;
                    _msg.Msg = "Collisions2D.FindMTD(...): Error!";

                    m_reporter.BroadcastMessage(_msg);
                }
#endif
                #endregion
            }

            return (mini != -1);
        }

        private void GetInterval(List<Vector> axVertices, Vector xAxis, ref double min, ref double max)
        {
            if (null == axVertices)
            {
                return;
            }

            min = max = Vector.DotProduct(axVertices[0], xAxis);

            for (int i = 1; i < axVertices.Count; i++)
            {
                double d = Vector.DotProduct(axVertices[i], xAxis);
                if (d < min)
                    min = d;
                else if (d > max)
                    max = d;
            }
        }
        #endregion

        #region Collision Response
        private void ProcessCollision(IPhysical a, IPhysical b, Vector N, double t)
        {
            /* Notes -
             *      11/15/05 -
             *          Added Dissipation Factor
             *          Went back to old method.
             *              Added Low End Buffer.
             */
            #region New & Commented Out
            /*double dissipation = 1.0 - (a.DissipationFactor + b.DissipationFactor);

            if (!a.IsMoveable)
            {
                if (b.IsMoveable)
                {
                    b.Velocity = (b.Velocity - (Vector.DotProduct(b.Velocity, N) * N));

                    if (b.Velocity.Y < m_lowEndBuffer && b.Velocity.Y > -m_lowEndBuffer)
                        b.Velocity.Y = 0;
                    if (b.Velocity.X < m_lowEndBuffer && b.Velocity.X > -m_lowEndBuffer)
                        b.Velocity.X = 0;
                }
                return;
            }

            if (!b.IsMoveable)
            {
                if (a.IsMoveable)
                {
                    System.Console.WriteLine("PRE: " + a.Velocity.ToString());
                    Vector result = (a.Velocity - (Vector.DotProduct(a.Velocity, N) * N));
                    a.Velocity = result;

                    if (a.Velocity.Y < m_lowEndBuffer && a.Velocity.Y > -m_lowEndBuffer)
                        a.Velocity.Y = 0;
                    if (a.Velocity.X < m_lowEndBuffer && a.Velocity.X > -m_lowEndBuffer)
                        a.Velocity.X = 0;

                    System.Console.WriteLine("POST: " + a.Velocity.ToString());
                }

                return;
            }

            #region Conservation of Momentum
            double _massA = a.Mass;
            double _massB = b.Mass;

            Vector _aF = new Vector();
            Vector _bF = new Vector();

            _aF = dissipation * ((_massA - _massB) / (_massA + _massB)) * a.Velocity +
                    ((2 * _massB) / (_massA + _massB)) * b.Velocity;

            _bF = dissipation * ((2 * _massA) / (_massA + _massB)) * a.Velocity +
                    ((_massB - _massA) / (_massA + _massB)) * b.Velocity;

            a.Velocity = _aF;
            b.Velocity = _bF;
            #endregion

            if (a.Velocity.Y < m_lowEndBuffer && a.Velocity.Y > -m_lowEndBuffer)
                a.Velocity.Y = 0;
            if (a.Velocity.X < m_lowEndBuffer && a.Velocity.X > -m_lowEndBuffer)
                a.Velocity.X = 0;

            if (b.Velocity.Y < m_lowEndBuffer && b.Velocity.Y > -m_lowEndBuffer)
                b.Velocity.Y = 0;
            if (b.Velocity.X < m_lowEndBuffer && b.Velocity.X > -m_lowEndBuffer)
                b.Velocity.X = 0;*/
            #endregion

            #region Process Collision (Old)
            Vector D = a.Velocity - b.Velocity;

            double n = Vector.DotProduct(D, N);

            Vector Dn = N * n;
            Vector Dt = D - Dn;

            if (n > 0.0)
                Dn = new Vector(0, 0);

            double dt = Vector.DotProduct(Dt, Dt);
            double CoF = a.DissipationFactor + b.DissipationFactor;

            D = -(1.0) * Dn - (CoF) * Dt;

            double m0 = 1.0 / a.Mass;
            double m1 = 1.0 / b.Mass;
            double m = m0 + m1;
            double r0 = m0 / m;
            double r1 = m1 / m;

            //System.Console.WriteLine(N.ToString());

            if (a.IsMoveable)
            {
                if (N.X == 0)
                    a.Velocity.Y += D.Y;
                else
                    a.Velocity += D;

                if (a.Velocity.Y < m_lowEndBuffer && a.Velocity.Y > -m_lowEndBuffer)
                    a.Velocity.Y = 0;
                if (a.Velocity.X < m_lowEndBuffer && a.Velocity.X > -m_lowEndBuffer)
                    a.Velocity.X = 0;

                //System.Console.WriteLine(a.Velocity.ToString());
            }

            if (b.IsMoveable)
            {
                if (N.X == 0)
                    b.Velocity.Y -= D.Y;
                else
                    b.Velocity -= D;

                if (b.Velocity.Y < m_lowEndBuffer && b.Velocity.Y > -m_lowEndBuffer)
                    b.Velocity.Y = 0;
                if (b.Velocity.X < m_lowEndBuffer && b.Velocity.X > -m_lowEndBuffer)
                    b.Velocity.X = 0;
            }
            #endregion
        }

        private void ProcessOverlap(IPhysical a, IPhysical b, Vector xMTD)
        {
            /* Notes -
             *      11/15/05 -
             *          Added Removed _tA, _tB. Put original code in.
             *          Took out Normalization of xMTD.
             */

            //Vector _tA = new Vector(a.Velocity.X * m_futureCheck, a.Velocity.Y * m_futureCheck);
            //Vector _tB = new Vector(b.Velocity.X * m_futureCheck, b.Velocity.Y * m_futureCheck);

            if (!a.IsMoveable)
            {
                //b.X -= _tB.X;
                //b.Y -= _tB.Y;

                b.X -= xMTD.X;
                b.Y -= xMTD.Y;
            }
            else if (!b.IsMoveable)
            {
                //a.X += _tA.X;
                //a.Y += _tA.Y;

                a.X += xMTD.X;
                a.Y += xMTD.Y;
            }
            else if(a.IsMoveable && b.IsMoveable)
            {
                /*a.X += (_tA.X * 0.5);
                a.Y += (_tA.Y * 0.5);

                b.X -= (_tB.X * 0.5);
                b.Y -= (_tB.Y * 0.5);*/

                a.X += (xMTD.X * 0.5);
                a.Y += (xMTD.Y * 0.5);

                b.X -= (xMTD.X * 0.5);
                b.Y -= (xMTD.Y * 0.5);
            }

            Vector N = xMTD;
            N.Normalize();

            ProcessCollision(a, b, xMTD, 0.0);
        }
        #endregion

        #region ICollisions2DService Members
        #region Public Vertex Setup Providers
        /// <summary>
        /// Make Triangle
        /// </summary>
        /// <param name="TopLeft">The Top Left Position</param>
        /// <param name="Size">The Width and Height</param>
        /// <returns>3 Vertices in the Shape of a Triangle</returns>
        public List<Vector> MakeTriangle(Point TopLeft, Rectangle Size)
        {
            try
            {
                List<Vector> _verts = new List<Vector>();

                _verts.Add(new Vector(TopLeft.X + (Size.Width / 2), TopLeft.Y));
                _verts.Add(new Vector(TopLeft.X + Size.Width, TopLeft.Y + Size.Height));
                _verts.Add(new Vector(TopLeft.X, TopLeft.Y + Size.Height));

                return _verts;
            }
            catch (Exception e)
            {
                if (m_reporter != null)
                    m_reporter.BroadcastError(this, "Collisions2D Error: " + e.Source, e);

                return null;
            }
        }

        /// <summary>
        /// Make Box
        /// </summary>
        /// <param name="TopLeft">The Top Left Position</param>
        /// <param name="Size">The Width and Height</param>
        /// <returns>4 Vertices in the Shape of a Box</returns>
        public List<Vector> MakeBox(Point TopLeft, Rectangle Size)
        {
            try
            {
                List<Vector> _verts = new List<Vector>();

                _verts.Add(new Vector(TopLeft.X, TopLeft.Y));
                _verts.Add(new Vector(TopLeft.X + Size.Width, TopLeft.Y));
                _verts.Add(new Vector(TopLeft.X + Size.Width, TopLeft.Y + Size.Height));
                _verts.Add(new Vector(TopLeft.X, TopLeft.Y + Size.Height));

                return _verts;
            }
            catch (Exception e)
            {
                if (m_reporter != null)
                    m_reporter.BroadcastError(this, "Collisions2D Error: " + e.Source, e);

                return null;
            }
        }

        /// <summary>
        /// Make Octagon
        /// </summary>
        /// <param name="TopLeft">The Top Left Position</param>
        /// <param name="Size">The Width and Height</param>
        /// <returns>8 Vertices in the Shape of a Octagon</returns>
        public List<Vector> MakeOctagon(Point TopLeft, Rectangle Size)
        {
            try
            {
                List<Vector> _verts = new List<Vector>();

                double tW = Size.Width / 3;
                double tH = Size.Height / 3;

                _verts.Add(new Vector(TopLeft.X + tW, TopLeft.Y));
                _verts.Add(new Vector(TopLeft.X + 2 * tW, TopLeft.Y));
                _verts.Add(new Vector(TopLeft.X + Size.Width, TopLeft.Y + tH));
                _verts.Add(new Vector(TopLeft.X + Size.Width, TopLeft.Y + 2 * tH));
                _verts.Add(new Vector(TopLeft.X + 2 * tW, TopLeft.Y + Size.Height));
                _verts.Add(new Vector(TopLeft.X + tW, TopLeft.Y + Size.Height));
                _verts.Add(new Vector(TopLeft.X, TopLeft.Y + 2 * tH));
                _verts.Add(new Vector(TopLeft.X, TopLeft.Y + tH));

                return _verts;
            }
            catch (Exception e)
            {
                if (m_reporter != null)
                    m_reporter.BroadcastError(this, "Collisions2D Error: " + e.Source, e);

                return null;
            }
        }
        #endregion

        #region Adding / Removing
        public void Add(IPhysical obj)
        {
            if (obj != null)
            {
                if (obj is ICollidable)
                {
                    foreach (IPhysical _temp in m_items)
                        if (_temp.ID.Equals(obj.ID))
                            return;

                    m_items.Add(obj);
                }
            }
        }

        public void Remove(IPhysical obj)
        {
            m_items.Remove(obj);
        }

        public void Remove(string id)
        {
            foreach (IPhysical _temp in m_items)
            {
                if (_temp.ID.Equals(id))
                    Remove(_temp);
            }
        }
        #endregion
        #endregion

        #region IService Members
        public string ID
        {
            get
            {
                return "Xna5D_Collisions2D";
            }
        }

        public void LoadSettings(XmlNode node)
        {
        }
        #endregion
    }

    public interface ICollisions2DService
    {
        void Add(IPhysical obj);

        void Remove(IPhysical obj);
        void Remove(string id);

        List<Vector> MakeTriangle(Point TopLeft, Rectangle Size);
        List<Vector> MakeBox(Point TopLeft, Rectangle Size);
        List<Vector> MakeOctagon(Point TopLeft, Rectangle Size);
    }

    public interface ICollidable
    {
        List<Vector> Vertices { get; }

        Walls Walls { get; }

        bool IsCollidable { get; }
    }

}


