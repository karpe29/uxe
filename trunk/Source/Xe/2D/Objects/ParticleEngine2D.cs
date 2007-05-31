#region License
/*
 *  Xna5D.Objects2D.dll
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 *  
 *  This software is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * 
 *  Date Created: December 04, 2006
 */
#endregion

#region Using Statements
using System;
using System.Reflection;
using System.Collections.Generic;

using XeFramework;
using XeFramework.Physics;
using XeFramework.Geometry;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace XeFramework.Objects2D
{
    public partial class ParticleEngine2D : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Members
        // The default min and max speed for each particle
        // on each axis.
        private const int MAX_SPEED = 500;
        private const int MIN_SPEED = -500;

        // The SpriteBatch and ContentManager, used for drawing
        // each particle.
        protected SpriteBatch m_sBatch;
        protected ContentManager m_conManager;

        // The Texture2D object holds the texture representation
        // of the m_texFile (Content Pipeline) string.
        protected Texture2D m_texture;
        protected string m_texFile = "Content/Images/particle_small";

        // The list of particles.
        protected List<Particle2D> m_particles = new List<Particle2D>();

        // The minimum and maximum speed for each new particle.
        protected Vector2 m_minSpeed = new Vector2(MIN_SPEED, MIN_SPEED);
        protected Vector2 m_maxSpeed = new Vector2(MAX_SPEED, MAX_SPEED);

        // The start position for each particle.
        protected Vector2 m_startPos = new Vector2(0, 0);

        // The minimum and maximum life span for each particle.
        // The life span determines how fast or slow each particle's
        // alpha channel decreases.
        protected double m_minLifeSpan = 1;
        protected double m_maxLifeSpan = 5;

        // Whether or not the particle engine should loop.
        protected bool m_isLooping = true;

        // The string to tag each particle with and whether
        // or not to use tagging.
        protected string m_tag = String.Empty;
        protected bool m_useTag = false;

        // The list of colors to randomly choose from for each
        // paricle.
        protected List<Color> m_colors = new List<Color>();

        // The random number generator.
        Random m_random = new Random();

        // The gravity service.
        IGravity2DService m_gravity;

        // The Stats component
        Stats m_stats;
        #endregion

        #region Construction & Destruction
        public ParticleEngine2D(Game game)
            : base(game)
        {
            m_colors.Add(Color.Red);
            m_colors.Add(Color.Pink);
            m_colors.Add(Color.Purple);
            m_colors.Add(Color.Yellow);
            m_colors.Add(Color.Orange);
            m_colors.Add(Color.Blue);
            m_colors.Add(Color.White);
        }

        public override void Initialize()
        {
            base.Initialize();

            m_gravity = (IGravity2DService)this.Game.Services.GetService(typeof(IGravity2DService));
            m_stats = (Stats)this.Game.Services.GetService(typeof(Stats));
        }

        ~ParticleEngine2D()
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                UnloadGraphicsContent(true);

            base.Dispose(disposing);
        }
        #endregion

        #region Load/Unload Graphics Content
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            base.LoadGraphicsContent(loadAllContent);

            if (loadAllContent)
            {
                m_sBatch = new SpriteBatch(this.GraphicsDevice);

                m_conManager = new ContentManager(this.Game.Services);

                m_texture = m_conManager.Load<Texture2D>(m_texFile);
            }
        }

        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            base.UnloadGraphicsContent(unloadAllContent);

            if (unloadAllContent)
            {
                if (m_sBatch != null)
                    m_sBatch.Dispose();
                m_sBatch = null;

                if (m_conManager != null)
                    m_conManager.Dispose();
                m_conManager = null;

                if (m_texture != null)
                    m_texture.Dispose();
                m_texture = null;
            }
        }
        #endregion

        #region Updating and Drawing
        public override void Update(GameTime gameTime)
        {
            // Get the current amount of particles and loop
            int _count = m_particles.Count;
            for (int i = 0; i < m_particles.Count; i++)
            {
                // Conserve momentum.
                // TODO: Add particles to Physics2D as well.
                m_particles[i].Position.X += (float)(m_particles[i].Velocity.X * gameTime.ElapsedRealTime.TotalSeconds);
                m_particles[i].Position.Y += (float)(m_particles[i].Velocity.Y * gameTime.ElapsedRealTime.TotalSeconds);

                // Decrease the particle's alpha channel
                m_particles[i].DecreaseAlpha(gameTime);

                // If we are looping the particle engine,
                // recreate the particle if it is necessary.
                if (m_isLooping)
                {
                    // Check the particle against the bounds of the screen
                    // and if its Alpha channel is 0 (transparent).
                    if (CheckWalls(m_particles[i]) || m_particles[i].Alpha <= 0)
                    {
                        // If we have a valid reference to a gravity system,
                        // remove the particle.
                        if (m_gravity != null)
                            m_gravity.Remove(m_particles[i].ID);

                        // Also remove it from out current array
                        m_particles.Remove(m_particles[i]);

                        // Create the new particle
                        Particle2D _particle = CreateParticle(i);

                        // Add the particle to our array
                        m_particles.Add(_particle);

                        // If we have a valid reference,
                        // add it to the Gravity2D object.
                        if (m_gravity != null)
                            m_gravity.Add(_particle);
                    }
                }
                else
                {
                    // Check the particle against the bounds of the screen
                    // and if its Alpha channel is 0 (transparent).
                    if (CheckWalls(m_particles[i]) || m_particles[i].Alpha <= 0)
                    {
                        // Remove the particle from Gravity2D
                        if (m_gravity != null)
                            m_gravity.Remove(m_particles[i].ID);

                        // Remove the particle from our array.
                        m_particles.RemoveAt(i);
                    }
                }
            }

            base.Update(gameTime);
        }

        protected virtual bool CheckWalls(Particle2D particle)
        {
            int _width = this.GraphicsDevice.Viewport.Width;
            int _height = this.GraphicsDevice.Viewport.Height;

            if (particle.Position.X < 0 || particle.Position.X > _width ||
                particle.Position.Y < 0 || particle.Position.Y > _height)
                return true;

            return false;
        }

        public override void Draw(GameTime gameTime)
        {
            m_sBatch.Begin(SpriteBlendMode.AlphaBlend);
            foreach (Particle2D _particle in m_particles)
            {
                m_sBatch.Draw(m_texture, _particle.Position.ToVector2(), _particle.Color);
            }
            m_sBatch.End();

            if (m_stats != null && m_stats.Visible)
                m_stats.PolygonCount += m_particles.Count * 2;

            base.Draw(gameTime);
        }
        #endregion

        #region Creating Explosions
        #region Non Tagged Explosions
        /// <summary>
        /// Create a Particle Explosion with default
        /// Minimum and Maximum speeds and looping.
        /// </summary>
        /// <param name="numOfParticles">The number of particles to create.</param>
        /// <param name="position">The starting position of each particle.</param>
        public void CreateExplosion(int numOfParticles, Vector2 position)
        {
            CreateExplosion(numOfParticles, position, new Vector2(MIN_SPEED, MIN_SPEED), new Vector2(MAX_SPEED, MAX_SPEED), true);
        }

        /// <summary>
        /// Create a Particle Explosion with looping
        /// </summary>
        /// <param name="numOfParticles">The number of particles to create.</param>
        /// <param name="position">The starting position of each particle.</param>
        /// <param name="minSpeed">The minimum allowed speed of each particle.</param>
        /// <param name="maxSpeed">The maximum allowed speed of each particle.</param>
        public void CreateExplosion(int numOfParticles, Vector2 position, Vector2 minSpeed, Vector2 maxSpeed)
        {
            CreateExplosion(numOfParticles, position, minSpeed, maxSpeed, true);
        }

        /// <summary>
        /// Create a Particle Explosion with default
        /// Minimum and Maximum speeds.
        /// </summary>
        /// <param name="numOfParticles">The number of particles to create.</param>
        /// <param name="position">The starting position of each particle.</param>
        /// <param name="loop">Whether or not particles get recreated as they die.</param>
        public void CreateExplosion(int numOfParticles, Vector2 position, bool loop)
        {
            CreateExplosion(numOfParticles, position, new Vector2(MIN_SPEED, MIN_SPEED), new Vector2(MAX_SPEED, MAX_SPEED), loop);
        }

        /// <summary>
        /// Create a Particle Explosion with no defaults.
        /// </summary>
        /// <param name="numOfParticles">The number of particles to create.</param>
        /// <param name="position">The starting position of each particle.</param>
        /// <param name="minSpeed">The minimum allowed speed of each particle.</param>
        /// <param name="maxSpeed">The maximum allowed speed of each particle.</param>
        /// <param name="loop">Whether or not particles get recreated as they die.</param>
        public void CreateExplosion(int numOfParticles, Vector2 position, Vector2 minSpeed, Vector2 maxSpeed, bool loop)
        {
            m_isLooping = loop;

            m_startPos = new Vector2(position.X, position.Y);
            m_minSpeed = new Vector2(minSpeed.X, minSpeed.Y);
            m_maxSpeed = new Vector2(maxSpeed.X, maxSpeed.Y);

            m_useTag = false;

            for (int i = 0; i < numOfParticles; i++)
            {
                Particle2D _particle = CreateParticle(i);

                m_particles.Add(_particle);

                if (m_gravity != null)
                    m_gravity.Add(_particle);
            }
        }
        #endregion

        #region Tagged Explosions
        /// <summary>
        /// Create a Tagged Explosion with default
        /// Minimum and Maximum speeds.
        /// </summary>
        /// <param name="tag">The tag associated with each particle.</param>
        /// <param name="numOfParticles">The number of particles to create.</param>
        /// <param name="position">The position at which to create each particle.</param>
        public void CreateTaggedExplosion(string tag, int numOfParticles, Vector2 position)
        {
            CreateTaggedExplosion(tag, numOfParticles, position, new Vector2(MIN_SPEED, MIN_SPEED), new Vector2(MAX_SPEED, MAX_SPEED), true);
        }

        /// <summary>
        /// Create a Tagged Explosion with looping.
        /// </summary>
        /// <param name="tag">The tag associated with each particle.</param>
        /// <param name="numOfParticles">The number of particles to create.</param>
        /// <param name="position">The position at which to create each particle.</param>
        /// <param name="minSpeed">The minimum allowed speed for each particle.</param>
        /// <param name="maxSpeed">The maximum allowed speed for each particle.</param>
        public void CreateTaggedExplosion(string tag, int numOfParticles, Vector2 position, Vector2 minSpeed, Vector2 maxSpeed)
        {
            CreateTaggedExplosion(tag, numOfParticles, position, minSpeed, maxSpeed, true);
        }

        /// <summary>
        /// Create a Tagged Explosion with default
        /// Minimum and Maximum speeds.
        /// </summary>
        /// <param name="tag">The tag associated with each particle.</param>
        /// <param name="numOfParticles">The number of particles to create.</param>
        /// <param name="position">The position at which to create each particle.</param>
        /// <param name="loop">Whether or not to loop each particle as it dies.</param>
        public void CreateTaggedExplosion(string tag, int numOfParticles, Vector2 position, bool loop)
        {
            CreateTaggedExplosion(tag, numOfParticles, position, new Vector2(MIN_SPEED, MIN_SPEED), new Vector2(MAX_SPEED, MAX_SPEED), loop);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag">The tag associated with each particle.</param>
        /// <param name="numOfParticles">The number of particles to create.</param>
        /// <param name="position">The position at which to create each particle.</param>
        /// <param name="minSpeed">The minimum allowed speed for each particle.</param>
        /// <param name="maxSpeed">The maximum allowed speed for each particle.</param>
        /// <param name="loop">Whether or not to loop each particle as it dies.</param>
        public void CreateTaggedExplosion(string tag, int numOfParticles, Vector2 position, Vector2 minSpeed, Vector2 maxSpeed, bool loop)
        {
            m_isLooping = loop;

            m_startPos = new Vector2(position.X, position.Y);
            m_minSpeed = new Vector2(minSpeed.X, minSpeed.Y);
            m_maxSpeed = new Vector2(maxSpeed.X, maxSpeed.Y);

            m_tag = tag;
            m_useTag = true;

            for (int i = 0; i < numOfParticles; i++)
            {
                Particle2D _particle = CreateParticle(i);

                m_particles.Add(_particle);

                if (m_gravity != null)
                    m_gravity.Add(_particle);
            }
        }
        #endregion
        #endregion

        #region Creating a Particle
        protected virtual Particle2D CreateParticle(int index)
        {
            // Instance a new particle object
            Particle2D _particle = new Particle2D();

            // Create a random ID for the particle.
            _particle.ID = "PARTICLE_" + index.ToString() + "_" + m_random.Next(int.MinValue, int.MaxValue).ToString();

            // Give it some mass, this increases the effect
            // the gravity points have on the particle.
            _particle.Mass = 40;

            // Give it a random Color.
            _particle.Color = m_colors[(int)(m_random.NextDouble() * m_colors.Count - 0.5)];

            // Give it our start position
            _particle.Position = new Vector(m_startPos.X, m_startPos.Y);

            // Give it a random Velocity.
            _particle.Velocity = new Vector(m_random.Next((int)m_minSpeed.X, (int)m_maxSpeed.X), m_random.Next((int)m_minSpeed.Y, (int)m_maxSpeed.Y));

            // Give it a random Life Span
            _particle.LifeSpan = ((m_maxLifeSpan - m_minLifeSpan) * m_random.NextDouble()) + m_minLifeSpan;

            // If we are using tags, give it the tag.
            if (m_useTag)
                _particle.Tags.Add(m_tag);

            // Return the particle.
            return _particle;
        }
        #endregion

        #region Properties
        public double MinLifeSpan
        {
            get
            {
                return m_minLifeSpan;
            }
            set
            {
                m_minLifeSpan = value;
            }
        }

        public double MaxLifeSpan
        {
            get
            {
                return m_maxLifeSpan;
            }
            set
            {
                m_maxLifeSpan = value;
            }
        }

        public List<Color> Colors
        {
            get
            {
                return m_colors;
            }
            set
            {
                m_colors = value;
            }
        }

        public int DefaultMaxSpeed
        {
            get
            {
                return MAX_SPEED;
            }
        }

        public int DefaultMinSpeed
        {
            get
            {
                return MIN_SPEED;
            }
        }

        public string ContentFile
        {
            get
            {
                return m_texFile;
            }
            set
            {
                m_texFile = value;
            }
        }

        public Vector2 StartPos
        {
            get
            {
                return m_startPos;
            }
            set
            {
                m_startPos = value;
            }
        }

        public Vector2 MinSpeed
        {
            get
            {
                return m_minSpeed;
            }
            set
            {
                m_minSpeed = value;
            }
        }

        public Vector2 MaxSpeed
        {
            get
            {
                return m_maxSpeed;
            }
            set
            {
                m_maxSpeed = value;
            }
        }
        #endregion
    }

    public class Particle2D : IPhysical
    {
        #region Members
        protected Vector m_vel = new Vector(0, 0);
        protected Vector m_pos = new Vector(0, 0);

        protected double m_mass = 1;

        protected string m_id = "PARTICLE";

        protected List<string> m_tags = new List<string>();

        protected double m_alpha = 255;

        protected Color m_color = Color.White;

        protected double m_lifeSpan = 2.0;
        #endregion

        #region Constructor
        public Particle2D()
        {
        }
        #endregion

        #region Changing Alpha
        public void DecreaseAlpha(GameTime gameTime)
        {
            if (Alpha > 0)
            {
                double val = 255 / m_lifeSpan;

                if (m_alpha - val < 0)
                    m_alpha = 0;
                else
                    m_alpha -= val * gameTime.ElapsedRealTime.TotalSeconds;

                this.Color = new Color(Color.R, Color.G, Color.B, (byte)Alpha);
            }
        }

        public void IncreaseAlpha(GameTime gameTime)
        {
            if (Alpha < 255)
            {
                double val = 255 / m_lifeSpan;

                if (m_alpha + val > 255)
                    m_alpha = 255;
                else
                    m_alpha += val * gameTime.ElapsedRealTime.TotalSeconds;

                this.Color = new Color(Color.R, Color.G, Color.B, (byte)m_alpha);
            }
        }

        public void ResetAlpha()
        {
            m_alpha = 255;
            this.Color = new Color(Color.R, Color.G, Color.B, (byte)m_alpha);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Color
        ///     The current tint color of the particle where
        /// Color.White is 100% visible with no tint.
        /// </summary>
        public Color Color
        {
            get
            {
                return m_color;
            }
            set
            {
                m_color = value;
            }
        }

        /// <summary>
        /// Alpha:
        ///     The current byte value of the alpha channel
        /// where 255 is 100% visible and 0 is 0% visible.
        /// </summary>
        public double Alpha
        {
            get
            {
                return m_alpha;
            }
            set
            {
                m_alpha = value;
            }
        }

        /// <summary>
        /// LifeSpan:
        ///     The total life span of the particle in seconds.
        /// Default value is 2.0 seconds.
        /// </summary>
        public double LifeSpan
        {
            get
            {
                return m_lifeSpan;
            }
            set
            {
                m_lifeSpan = value;
            }
        }
        #endregion

        #region IPhysical Members
        public string ID
        {
            get
            {
                return m_id;
            }
            set
            {
                m_id = value;
            }
        }

        public bool IsMoveable
        {
            get
            {
                return true;
            }
            set
            {
                //throw new Exception("The method or operation is not implemented.");
            }
        }

        public bool IsActive
        {
            get
            {
                return true;
            }
            set
            {
                //throw new Exception("The method or operation is not implemented.");
            }
        }

        public Vector Velocity
        {
            get
            {
                return m_vel;
            }
            set
            {
                m_vel = value;
            }
        }

        public Vector Position
        {
            get
            {
                return m_pos;
            }
            set
            {
                m_pos = value;
            }
        }

        public double X
        {
            get
            {
                return m_pos.X;
            }
            set
            {
                m_pos.X = value;
            }
        }

        public double Y
        {
            get
            {
                return m_pos.Y;
            }
            set
            {
                m_pos.Y = value;
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
                return 0;
            }
            set
            {
                //
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
        #endregion
    }
}


