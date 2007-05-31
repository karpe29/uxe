#region License
/*
 *  Xna5D
 *  Copyright (C)2007 John Sedlak (http://jsedlak.org)
 *  
 *  This software is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
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
#endregion

namespace XeFramework.Physics
{
    public interface IPhysical
    {
        /// <summary>
        /// ID: A unique identification string to ensure no duplicate entries.
        /// </summary>
        string ID { get; }

        /// <summary>
        /// Tags:
        ///     Provides a way to coordinate objects with services that act on
        /// categories. For example: Gravity2D.
        /// </summary>
        List<string> Tags { get; set; }

        /// <summary>
        /// IsMoveable: Determines whether or not the object can be moved or not.
        /// </summary>
        bool IsMoveable { get; set; }

        /// <summary>
        /// IsActive: 
        /// </summary>
        bool IsActive { get; set; }

        Vector Velocity { get; set; }

        Vector Position { get; set; }

        double X { get; set; }

        /// <summary>
        /// Y
        ///     This is the position of an object along
        /// the Y Axis.
        /// </summary>
        double Y { get; set; }

        /// <summary>
        /// Mass
        ///     This scalar represents the mass of an object
        /// in space and time. Normal mass is 1 unit.
        /// </summary>
        double Mass { get; set; }

        /// <summary>
        /// Dissipation Factor:
        ///     This is a factor of how much energy the object
        /// dissipates. This allows objects to essentially act
        /// as real life ground, dispersing a portion of the
        /// energy. Normal Dissipation is 0.5, returning 100%
        /// of the force.
        /// </summary>
        double DissipationFactor { get; set; }
    }

    [FlagsAttribute]
    public enum Walls
    {
        None = 0,
        Left = 1,
        Right = 2,
        Top = 4,
        Bottom = 8
    }
}

