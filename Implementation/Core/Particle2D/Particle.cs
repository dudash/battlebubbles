#region Copyright
//-----------------------------------------------------------------------------
// Copyright (C)2008 Jason Dudash, GNU GPLv3.
//-----------------------------------------------------------------------------
// File Created: 30 July 2006, Jason Dudash
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
#endregion
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HBBB.Core.Particle2D
{
    /// <summary>
    /// Represents a single particle in the world
    /// </summary>
    public class Particle
    {
        protected bool alive = true;
        protected Vector2 prevPosition;
        protected Vector2 position;
        protected Vector2 velocity;
        protected float prevAngle;
        protected float angle;
        protected float angularVelocity;
        protected Color color = Color.Red;
        protected int energy = 100;
        protected float size = 1.0f;
        protected double lifeSpan = 1000.0;
        protected double age = 0.0;

        public delegate void ParticleDeathEventHandler(Particle p);
        public event ParticleDeathEventHandler ParticleExpiring;

        #region Getters and Setters
        public bool IsAlive
        {
            get { return alive; }
            set { alive = value; }
        }
        public Vector2 PreviousPosition
        {
            get { return prevPosition; }
            set { prevPosition = value; }
        }
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        public float PreviousAngle
        {
            get { return prevAngle; }
            set { prevAngle = value; }
        }
        public float Angle
        {
            get { return angle; }
            set { angle = value; }
        }
        public float AngularVelocity
        {
            get { return angularVelocity; }
            set { angularVelocity = value; }
        }
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }
        public int Energy
        {
            get { return energy; }
            set { energy = value; }
        }
        public float Size
        {
            get { return size; }
            set { size = value; }
        }
        public double LifeSpan
        {
            get { return lifeSpan; }
            set { lifeSpan = value; }
        }
        #endregion

        /// <summary>
        /// Default Constructor
        /// </summary>
        public Particle()
        {
            // TODO default these based on environment of parent system
        }

        /// <summary>
        /// Update the simulation for this particle
        /// </summary>
        /// <param name="deltaTime">the amount of time ellapsed since the last update</param>
        /// <returns></returns>
        public bool Update(float deltaTime)
        {
            if (!alive) return false;

            // update the particle's age
            age += deltaTime;
            if (age / lifeSpan > 1)
            {
                alive = false;
                if (ParticleExpiring!=null) ParticleExpiring(this); // tell anyone interested that I'm dead
                return alive;
            }

            // TODO update size, alpha, color

            // update position and angle
            prevPosition = position;
            position.X += velocity.X * deltaTime;
            position.Y += velocity.Y * deltaTime;
            prevAngle = angle;
            angle += angularVelocity * deltaTime;

            // keep angle within 0->360
            if (angle > System.Math.PI * 2) angle -= (float)System.Math.PI * 2;
            if (angle < -System.Math.PI * 2) angle += (float)System.Math.PI * 2;
            
            return true;
        }

        #region Debug Methods
        [Conditional("Debug")]
        public void DebugDump()
        {
            Debug.WriteLine("- Particle -");
            Debug.WriteLine("Is Alive ..........." + IsAlive.ToString());
            Debug.WriteLine("Position ..........." + Position.ToString());
            Debug.WriteLine("Previous Position..." + PreviousPosition.ToString());
            Debug.WriteLine("Velocity ..........." + Velocity.ToString());
            Debug.WriteLine("Color .............." + Color.ToString());
            Debug.WriteLine("Energy ............." + Energy.ToString());
            Debug.WriteLine("Size ..............." + Size.ToString());
            Debug.WriteLine("Angle .............." + Angle.ToString());
            Debug.WriteLine("Angular Velocity...." + AngularVelocity.ToString());
            Debug.WriteLine("Life Span..........." + LifeSpan.ToString());
        }
        #endregion
    }
}
