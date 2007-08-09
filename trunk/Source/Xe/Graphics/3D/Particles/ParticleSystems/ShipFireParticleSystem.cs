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
    class ShipFireParticleSystem : ParticleSystem
    {
        public ShipFireParticleSystem(Game game, ContentManager content)
            : base(game, content)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
			settings.TextureName = @"Content\Particles\fire";

            settings.MaxParticles = 2400;

            settings.Duration = TimeSpan.FromSeconds(.5);

            settings.DurationRandomness = 1;

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 5;

            settings.MinVerticalVelocity = 0;
            settings.MaxVerticalVelocity = 10;

            // Set gravity upside down, so the flames will 'fall' upward.
            settings.Gravity = new Vector3(0, 0, 0);

            settings.MinColor = new Color(255, 255, 255, 10);
            settings.MaxColor = new Color(255, 255, 255, 40);

            settings.MinStartSize = 10;
            settings.MaxStartSize = 15;

            settings.MinEndSize = 5;
            settings.MaxEndSize = 10;

            // Use additive blending.
            settings.SourceBlend = Blend.SourceAlpha;
            settings.DestinationBlend = Blend.One;
        }
    }
}
