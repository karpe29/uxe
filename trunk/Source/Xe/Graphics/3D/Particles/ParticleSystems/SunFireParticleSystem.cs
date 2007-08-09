#region File Description
//-----------------------------------------------------------------------------
// FireParticleSystem.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Xe.SpaceRace;
#endregion

namespace Xe.Graphics3D.Particles
{
    /// <summary>
    /// Custom particle system for creating a flame effect.
    /// </summary>
    class SunFireParticleSystem : ParticleSystem
    {
        public SunFireParticleSystem(Game game, ContentManager content)
            : base(game, content)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
			settings.TextureName = @"Content\Particles\fire";

            settings.MaxParticles = 100000;

            settings.Duration = TimeSpan.FromSeconds(12);

            settings.DurationRandomness = 0f;

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 0;

            settings.MinVerticalVelocity = 0;
            settings.MaxVerticalVelocity = 0;

            // Set gravity upside down, so the flames will 'fall' upward.
            settings.Gravity = new Vector3(0, 0, 0);

            settings.MinColor = new Color(255, 255, 255, 10);
            settings.MaxColor = new Color(255, 255, 255, 40);

            settings.MinStartSize = 100000;
            settings.MaxStartSize = 100000;

            settings.MinEndSize = 100000;
            settings.MaxEndSize = 100000;

            // Use additive blending.
            settings.SourceBlend = Blend.SourceAlpha;
            settings.DestinationBlend = Blend.One;
        }
    }
}
