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
using Microsoft.Xna.Framework.Graphics;

namespace HBBB.Core.Particle2D
{
    /// <summary>
    /// An Emitter that throws particles out mostly randomly with mostly random
    /// properties.
    /// </summary>
    public sealed class RandomEmitter : Emitter
    {
        private int minEmissionCount = 1;
        private int maxEmissionCount = 3;
        private double minParticleLife = 0.5;
        private double maxParticleLife = 2.0;
        private int minVelocity = 0;
        private int maxVelocity = 50;
        private int minPosition = 0;
        private int maxPosition = 100;

        #region Getters and Setters
        public int MinEmissionCount
        {
            get { return minEmissionCount; }
            set { minEmissionCount = value; }
        }
        public int MaxEmissionCount
        {
            get { return maxEmissionCount; }
            set { maxEmissionCount = value; }
        }
        public double MinParticleLife
        {
            get { return minParticleLife; }
            set { minParticleLife = value; }
        }
        public double MaxParticleLife
        {
            get { return maxParticleLife; }
            set { maxParticleLife = value; }
        }
        public int MinVelocity
        {
            get { return minVelocity; }
            set { minVelocity = value; }
        }
        public int MaxVelocity
        {
            get { return maxVelocity; }
            set { maxVelocity = value; }
        }
        public int MinPosition
        {
            get { return minPosition; }
            set { minPosition = value; }
        }
        public int MaxPosition
        {
            get { return maxPosition; }
            set { maxPosition = value; }
        }
        #endregion
        /// <summary>
        /// Construct
        /// </summary>
        public RandomEmitter() : base()
        {
        }

        /// <summary>
        /// Construct with a position
        /// </summary>
        /// <param name="newPosition"></param>
        public RandomEmitter(Vector2 newPosition)
            : base(newPosition)
        {
        }

        /// <summary>
        /// Create some random particles and give them out
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <param name="addCallback"></param>
        public override void Emit(float deltaTime, AddParticleCallback addCallback)
        {
            Random random = new Random(System.Environment.TickCount);
            int particlesToEmit = random.Next(minEmissionCount, maxEmissionCount);
            for (int i = 0; i < particlesToEmit; i++)
            {
                Particle p = new Particle();
                p.Color = this.ParticleColor;
                if (particlesToEmit == 2) p.Color = this.ParticleColorTwo;
                if (particlesToEmit == 3) p.Color = this.ParticleColorThree;
                p.LifeSpan = Math.Random.NextDouble(minParticleLife, maxParticleLife);
                float xVel = Math.Random.NextFloat(minVelocity, maxVelocity);
                float yVel = Math.Random.NextFloat(minVelocity, maxVelocity);
                p.Velocity = new Vector2(xVel, yVel);
                float xPos = position.X + Math.Random.NextFloat(minPosition, maxPosition) - maxPosition / 2;
                float yPos = position.Y + Math.Random.NextFloat(minPosition, maxPosition) - maxPosition / 2;
                p.Position = new Vector2(xPos, yPos);
                addCallback(p);
            }
        }
    }
}
