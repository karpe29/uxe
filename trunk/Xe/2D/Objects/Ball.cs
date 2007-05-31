#region License
/*
 *  Xna5D.Objects2D.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 *  
 *  This software is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 *  Date Created: January 02, 2007
 */
#endregion

#region Using Statements
using System;
using System.Reflection;
using System.Collections.Generic;

using XeFramework;
using XeFramework.Physics;
using XeFramework.Geometry;
using XeFramework.Graphics2D;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace XeFramework.Objects2D
{
    public class Ball : Sprite, IPhysical, ICollidable
    {
        #region Members
        protected string m_id;

        protected bool m_isMoveable = true;
        protected bool m_isCollidable = true;

        protected Walls m_walls = Walls.Bottom | Walls.Left | Walls.Top | Walls.Right;

        protected Vector m_position = new Vector(10, 10);
        protected Vector m_velocity = new Vector(100, 0);

        protected IPhysics2DService m_physics;
        protected ICollisions2DService m_collisions;

        protected double m_dissipation = 0.0;
        protected double m_mass = 1;

        protected List<string> m_tags = new List<string>();
        #endregion

        #region Constructor && Initialization
		public Ball(Game game)
            : base(game)
        { 
        }

        public override void Initialize()
        {
            base.Initialize();

            m_physics = (IPhysics2DService)this.Game.Services.GetService(typeof(IPhysics2DService));

            m_collisions = (ICollisions2DService)this.Game.Services.GetService(typeof(ICollisions2DService));
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            base.X = (int)m_position.X;
            base.Y = (int)m_position.Y;
        }

        #region IPhysical Members
        public string ID
        {
            get 
            {
                return m_id;
            }
        }

        public List<string> Tags
        {
            get
            {
                return m_tags;
            }
            set
            {
                m_tags = value;
            }
        }

        public bool IsMoveable
        {
            get
            {
                return m_isMoveable;
            }
            set
            {
                m_isMoveable = value;
            }
        }

        public bool IsActive
        {
            get
            {
                return this.Enabled;
            }
            set
            {
                this.Enabled = value;
            }
        }

        public Vector Velocity
        {
            get
            {
                return m_velocity;
            }
            set
            {
                m_velocity = value;
            }
        }

        public Vector Position
        {
            get
            {
                return m_position;
            }
            set
            {
                m_position = value;
            }
        }

        public new double X
        {
            get
            {
                return m_position.X;
            }
            set
            {
                m_position.X = value;
            }
        }

        public new double Y
        {
            get
            {
                return m_position.Y;
            }
            set
            {
                m_position.Y = value;
            }
        }

        public double Mass
        {
            get
            {
                return m_mass;
            }
            set
            {
                m_mass = value;
            }
        }

        public double DissipationFactor
        {
            get
            {
                return m_dissipation;
            }
            set
            {
                m_dissipation = value;
            }
        }
        #endregion

        #region ICollidable Members
        public List<Vector> Vertices
        {
            get 
            {
                if (m_collisions != null)
                    return m_collisions.MakeOctagon(m_position.ToPoint(), new Rectangle((int)this.X, (int)this.Y, this.Width, this.Height));

                List<Vector> _verts = new List<Vector>();
                _verts.Add(new Vector(this.X, this.Y));
                _verts.Add(new Vector(this.X + this.Width, this.Y));
                _verts.Add(new Vector(this.X + this.Width, this.Y + this.Height));
                _verts.Add(new Vector(this.X, this.Y + this.Height));

                return _verts;
            }
        }

        public Walls Walls
        {
            get 
            {
                return m_walls;
            }
        }

        public bool IsCollidable
        {
            get
            {
                return m_isCollidable;
            }
        }
        #endregion
    }
}
