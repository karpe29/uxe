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
    public partial class Physics2D : Microsoft.Xna.Framework.GameComponent, IPhysics2DService, IService
    {
        #region Members
        protected Collisions2D m_collisions;
        protected Gravity2D m_gravity;

        protected List<IPhysical> m_items = new List<IPhysical>();
        #endregion

        #region Constructors & Initialization
		public Physics2D(Game game)
            : base(game)
        {
            m_collisions = new Collisions2D(game);
            m_gravity = new Gravity2D(game);

            if (game != null)
            {
                game.Services.AddService(typeof(IPhysics2DService), this);
                game.Components.Add(m_collisions);
                game.Components.Add(m_gravity);
            }
        }

		public Physics2D(Game game, Collisions2D collisions, Gravity2D gravity)
            : base(game)
        {
            m_collisions = collisions;
            m_gravity = gravity;

            if (game != null)
                game.Services.AddService(typeof(IPhysics2DService), this);
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
                _item.X += (_item.Velocity.X * gameTime.ElapsedRealTime.TotalSeconds);
                _item.Y += (_item.Velocity.Y * gameTime.ElapsedRealTime.TotalSeconds);
            }
        }

        #region IService Members
        public string ID
        {
            get
            {
                return "Xna5D_Physics2D";
            }
        }

        public void LoadSettings(XmlNode node)
        {
        }
        #endregion

        #region IPhysics2DService Members
        public void Add(IPhysical obj)
        {
            if (obj != null)
            {
                m_items.Add(obj);

                if (m_collisions != null)
                    m_collisions.Add(obj);

                if (m_gravity != null)
                    m_gravity.Add(obj);
            }
        }

        public void Remove(IPhysical obj)
        {
            if (obj != null)
            {
                m_items.Remove(obj);

                if (m_collisions != null)
                    m_collisions.Remove(obj);

                if (m_gravity != null)
                    m_gravity.Remove(obj);
            }
        }

        public void Remove(string id)
        {
            for (int i = 0; i < m_items.Count; i++)
            {
                if (m_items[i].ID.Equals(id))
                {
                    Remove(m_items[i]);

                    return;
                }
            }
        }
        #endregion

        #region Properties
        public Collisions2D Collisions2D
        {
            get
            {
                return m_collisions;
            }
        }

        public Gravity2D Gravity2D
        {
            get
            {
                return m_gravity;
            }
        }
        #endregion
    }

    public interface IPhysics2DService
    {
        void Add(IPhysical obj);

        void Remove(IPhysical obj);
        void Remove(string id);
    }
}


