#region File Description
//-----------------------------------------------------------------------------
// SmokePlumeParticleSystem.cs
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
    /// Custom particle system for creating a giant plume of long lasting smoke.
    /// </summary>
    class ShipSmokePlumeParticleSystem : ParticleSystem
    {
        public ShipSmokePlumeParticleSystem(Game game, ContentManager content)
            : base(game, content)
        { }


        protected override void InitializeSettings(ParticleSettings settings)
        {
			settings.TextureName = @"Content\Particles\smoke";

            settings.MaxParticles = 2400;

            settings.Duration = TimeSpan.FromSeconds(.5);

            settings.MinHorizontalVelocity = 0;
            settings.MaxHorizontalVelocity = 0;

            settings.MinVerticalVelocity = 0;
            settings.MaxVerticalVelocity = 15;

            // Create a wind effect by tilting the gravity vector sideways.
            settings.Gravity = new Vector3(0, 0, 0);

            //settings.EndVelocity = 0.75f;

            settings.MinRotateSpeed = -1;
            settings.MaxRotateSpeed = 1;

            settings.MinStartSize = 10;
            settings.MaxStartSize = 15;

            settings.MinEndSize = 20;
            settings.MaxEndSize = 30;
        }
    }
}
