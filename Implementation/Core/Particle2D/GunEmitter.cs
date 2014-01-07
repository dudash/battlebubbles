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
// File Created: 23 August 2006, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using Microsoft.Xna.Framework;

namespace HBBB.Core.Particle2D
{
    /// <summary>
    /// Emits particles from a gun.
    /// </summary>
    public class GunEmitter : Emitter
    {
        protected Vector2 direction;
        protected double rateOfFire;

        #region Getters and Setters
        public Vector2 Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        public double RateOfFire
        {
            get { return rateOfFire; }
            set { rateOfFire = value; }
        }
        #endregion

        /// <summary>
        /// Construct with a center position
        /// </summary>
        /// <param name="thePosition"></param>
        public GunEmitter(Vector2 newPosition, Vector2 newDirection)
        {
            position = newPosition;
            direction = newDirection;
        }

        /// <summary>
        /// Emit particle out from center
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="addCallback"></param>
        public override void Emit(float deltaTime, Emitter.AddParticleCallback addCallback)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
