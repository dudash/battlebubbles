#region Copyright
//-----------------------------------------------------------------------------
// Copyright (C)2008 Jason Dudash, GNU GPLv3.
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
//
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.
//-----------------------------------------------------------------------------
// File Created: 30 July 2006, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using Microsoft.Xna.Framework;

namespace HBBB.Core.Particle2D
{
    /// <summary>
    /// Emits particles from a circle.  The circle center and radius is
    /// configurable as well as the number of particle per emission and
    /// the outward or inward velocity.
    /// </summary>
    public class RadialEmitter : Emitter
    {
        protected int radius = 10;
        protected int particlesPerEmission = 10;
        protected int velocity = 10;

        #region Getters and Setters
        public int Radius
        {
            get { return radius; }
            set { radius = value; }
        }
        public int ParticlesPerEmission
        {
            get { return particlesPerEmission; }
            set { particlesPerEmission = value; }
        }
        public int Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        #endregion

        /// <summary>
        /// Construct with a center position and radius
        /// </summary>
        /// <param name="thePosition"></param>
        public RadialEmitter(Vector2 newPosition, int newRadius)
        {
            position = newPosition;
            radius = newRadius;
        }

        /// <summary>
        /// Emit particle out from circle
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="addCallback"></param>
        public override void Emit(float deltaTime, Emitter.AddParticleCallback addCallback)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
