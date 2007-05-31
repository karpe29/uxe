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
using System.Collections.Generic;

using XeFramework;
using XeFramework.Geometry;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace XeFramework.Physics
{
    public partial class Gravity2D : Microsoft.Xna.Framework.GameComponent, IGravity2DService, IService
    {
        #region Members
        protected List<IPhysical> m_items = new List<IPhysical>();
        protected double m_gravity = 9.81;

        protected List<GravityPoint> m_points = new List<GravityPoint>();
        #endregion

        #region Constructor & Initialization
		public Gravity2D(Game game)
            : base(game)
        {
            if (game != null)
                game.Services.AddService(typeof(IGravity2DService), this);

            GravityPoint p = new GravityPoint();
            p.GravityType = GravityType.Normal;
            p.Categories.Add("*");

            AddPoint(p);
        }

        public override void Initialize()
        {
            base.Initialize();
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (IPhysical _item in m_items)
            {
                if (_item.IsActive && _item.IsMoveable)
                {
                    foreach (GravityPoint _point in m_points)
                    {
                        #region Tags & Categories
                        if (_item.Tags.Count > 0)
                        {
                            bool _found = false;
                            foreach (string _cat in _point.Categories)
                            {
                                if (_cat.Equals("*") || _cat.Equals(""))
                                {
                                    _found = true;
                                    break;
                                }

                                foreach (string _tag in _item.Tags)
                                {
                                    if (_cat.Equals(_tag))
                                    {
                                        _found = true;
                                        break;
                                    }
                                }

                                if (_found)
                                    break;
                            }

                            if (!_found)
                                continue;
                        }
                        else
                        {
                            if (!_point.Categories.Contains("*"))
                                continue;
                        }
                        #endregion

                        switch (_point.GravityType)
                        {
                            case GravityType.Normal:
                                _item.Velocity.Y += (m_gravity * gameTime.ElapsedRealTime.TotalSeconds);
                                break;
                            case GravityType.Reverse:
                                _item.Velocity.Y -= (m_gravity * gameTime.ElapsedRealTime.TotalSeconds);
                                break;
                            case GravityType.Heavy:
                                _item.Velocity.Y += 2 * (m_gravity * gameTime.ElapsedRealTime.TotalSeconds);
                                break;
                            case GravityType.HeavyReverse:
                                _item.Velocity.Y -= 2 * (m_gravity * gameTime.ElapsedRealTime.TotalSeconds);
                                break;
                            case GravityType.BlackHole:
                                PreformBlackHole(_item, _point, gameTime);
                                break;
                            case GravityType.Anti:
                                PreformAnti(_item, _point, gameTime);
                                break;
                            case GravityType.Vortex:
                                PreformVortex(_item, _point, gameTime);
                                break;
                        }
                    }
                }
            }
        }

        protected virtual void PreformBlackHole(IPhysical obj, GravityPoint point, GameTime gameTime)
        {
            if (obj.X == point.Position.X && obj.Y == point.Position.Y)
                return;

            double _theta = FindAngle(point.Position.X - obj.X, point.Position.Y - obj.Y);
            double _distance = FindDistance(obj.Position.ToPoint(), point.Position);

            double _gForce = 1.5 * obj.Mass;
            obj.Velocity = new Vector(obj.Velocity.X + (_gForce * Math.Cos(_theta)),
                                      obj.Velocity.Y + (_gForce * Math.Sin(_theta)));
        }

        protected virtual void PreformVortex(IPhysical obj, GravityPoint point, GameTime gameTime)
        {
            double _theta = FindAngle(point.Position.X - obj.X, point.Position.Y - obj.Y);
            double _distance = FindDistance(obj.Position.ToPoint(), point.Position);
            double _gForce = 0.01 * _distance * obj.Mass;

            obj.Velocity = new Vector(obj.Velocity.X + (_gForce * Math.Sin(_theta)),
                                      obj.Velocity.Y + (_gForce * Math.Cos(_theta)));
        }

        protected virtual void PreformAnti(IPhysical obj, GravityPoint point, GameTime gameTime)
        {
            double _theta = FindAngle(point.Position.X - obj.X, point.Position.Y - obj.Y);
            double _distance = FindDistance(obj.Position.ToPoint(), point.Position);
            double _gForce = 1.5 * obj.Mass;

            obj.Velocity = new Vector(obj.Velocity.X - (_gForce * Math.Cos(_theta)),
                                      obj.Velocity.Y - (_gForce * Math.Sin(_theta)));
        }

        #region Math Helpers
        protected double FindAngle(double x, double y)
        {
            double theta = Math.Atan(y / x);
            if ((theta > 0 && y < 0) || (theta < 0 && x < 0))
                theta += Math.PI;
            if (theta < 0)
                theta += 2 * Math.PI;

            return theta;
        }

        protected double FindDistance(Point m, Point n)
        {
            return Math.Sqrt(Math.Pow(m.X - n.X, 2) + Math.Pow(m.Y - n.Y, 2));
        }
        #endregion

        #region IGravity2DService Members
        public void Add(IPhysical obj)
        {
            if (obj != null)
            {
                foreach (IPhysical _temp in m_items)
                    if (_temp.ID.Equals(obj.ID))
                        return;

                m_items.Add(obj);
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
                {
                    Remove(_temp);
                    return;
                }
            }
        }

        public void AddPoint(GravityPoint p)
        {
            m_points.Add(p);
        }

        public bool RemovePoint(GravityPoint p)
        {
            return m_points.Remove(p);
        }

        public double GravityConstant
        {
            get
            {
                return m_gravity;
            }
            set
            {
                m_gravity = value;
            }
        }
        #endregion

        #region IService Members
        public string ID
        {
            get
            {
                return "Xna5D_Gravity2D";
            }
        }

        public void LoadSettings(XmlNode node)
        {
        }
        #endregion
    }

    public interface IGravity2DService
    {
        void Add(IPhysical obj);

        void Remove(IPhysical obj);
        void Remove(string id);

        double GravityConstant { get; set; }

        void AddPoint(GravityPoint p);
        bool RemovePoint(GravityPoint p);
    }

    public class GravityPoint
    {
        public GravityType GravityType = GravityType.BlackHole;
        public Point Position = new Point(0, 0);
        public bool Enabled = true;
        public List<string> Categories = new List<string>();

        public GravityPoint()
        {
        }
    }

    public enum GravityType
    {
        /// <summary>
        /// This type represents normal gravity.
        /// </summary>
        Normal,

        Reverse,

        Heavy,

        HeavyReverse,

        /// <summary>
        /// This type represents gravity with an effect of a blackhole.
        /// </summary>
        BlackHole,

        /// <summary>
        /// Gravity that forces objects away from the center.
        /// </summary>
        Anti,

        /// <summary>
        /// This type represents gravity with an effect of a vortex.
        /// </summary>
        Vortex
    }
}


