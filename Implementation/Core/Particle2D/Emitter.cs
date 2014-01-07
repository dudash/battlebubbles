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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HBBB.Core.Particle2D
{
    /// <summary>
    /// The Emitter class is used for particle creation.
    /// Different emitters can control the various
    /// properties of each particle when emitting it.  eg: position,
    /// velocity, color.  An emitter can also vary the number of
    /// particles to be created on each emit call.
    /// <see cref="ParticleSystem"/>.
    /// </summary>
    public abstract class Emitter : Entity2D
    {
        /// <summary>
        /// Default color for particles
        /// </summary>
        public Color ParticleColor = Color.White;
        public Color ParticleColorTwo = Color.White;
        public Color ParticleColorThree = Color.White;

        /// <summary>
        /// Construct
        /// </summary>
        public Emitter() : base()
        {
        }

        /// <summary>
        /// Construct with a position
        /// </summary>
        /// <param name="newPosition"></param>
        public Emitter(Vector2 newPosition) : base(newPosition)
        {
            position = newPosition;
        }

        /// <summary>
        /// Delegate type for generated particles to be passed out of the
        /// emitter object
        /// </summary>
        /// <param name="p"></param>
        public delegate void AddParticleCallback(Particle p);

        /// <summary>
        /// Create particles from this emitter based on it's configuration
        /// </summary>
        /// <param name="deltaTime">time ellapse since last call to emit</param>
        /// <param name="addCallback">callback function to pass out emitted particles</param>
        public abstract void Emit(float deltaTime, AddParticleCallback addCallback);
    }
}
