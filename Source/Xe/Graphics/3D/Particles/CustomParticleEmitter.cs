using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace Xe.Graphics3D.Particles
{
	class CustomParticleEmitter
	{
		#region Fields

        ParticleSystem particleSystem;
        float timeBetweenParticles;
        float timeLeftOver;

		public delegate void AddParticleDelegate(ParticleSystem p);

		AddParticleDelegate m_delegate = null;

        #endregion


        /// <summary>
        /// Constructs a new particle emitter object.
        /// </summary>
		public CustomParticleEmitter(ParticleSystem particleSystem, float particlesPerSecond, AddParticleDelegate addParticleDelegate)
        {
            this.particleSystem = particleSystem;

            timeBetweenParticles = 1.0f / particlesPerSecond;

			m_delegate = addParticleDelegate;
        }

		/// <summary>
        /// Updates the emitter, creating the appropriate number of particles
        /// in the appropriate positions.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if (gameTime == null)
                throw new ArgumentNullException("gameTime");

            // Work out how much time has passed since the previous update.
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (elapsedTime > 0)
            {
                // If we had any time left over that we didn't use during the
                // previous update, add that to the current elapsed time.
                float timeToSpend = timeLeftOver + elapsedTime;
                
                // Counter for looping over the time interval.
                float currentTime = -timeLeftOver;

                // Create particles as long as we have a big enough time interval.
                while (timeToSpend > timeBetweenParticles)
                {
                    currentTime += timeBetweenParticles;
                    timeToSpend -= timeBetweenParticles;

                    // Create the particle.
                    //particleSystem.AddParticle(position, velocity);
					m_delegate(this.particleSystem);
                }

                // Store any time we didn't use, so it can be part of the next update.
                timeLeftOver = timeToSpend;
            }
        }
    }
}