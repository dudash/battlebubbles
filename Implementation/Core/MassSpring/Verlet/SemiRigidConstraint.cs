#region Copyright
//-----------------------------------------------------------------------------
// Copyright (C)2007 Jason Dudash, GNU GPLv3.
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
// File Created: 21 August 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HBBB.Core.Graphics;

namespace HBBB.Core.MassSpring.Verlet
{
    /// <summary>
    /// A constraint that acts like a spring: flexing when force is applied to it, but always returning
    /// to a prefered rest length.  For use with VerletPoint in a VerletSystem.
    /// </summary>
    class SemiRigidConstraint : IVerletConstraint
    {
        VerletPoint otherPoint;
        float min;
        float mid;
        float max;
        float force;

        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="otherPoint"></param>
        /// <param name="min"></param>
        /// <param name="mid"></param>
        /// <param name="max"></param>
        /// <param name="force"></param>
        public SemiRigidConstraint(VerletPoint otherPoint, float min, float mid, float max, float force)
        {
            this.otherPoint = otherPoint;
            this.min = min;
            this.mid = mid;
            this.max = max;
            this.force = force;
        }

        /// <summary>
        /// Calculate and apply a semi-rigid spring constraint
        /// </summary>
        /// <param name="point"></param>
        public void Satisfy(VerletPoint point)
        {
            if (otherPoint == null) return;
            Vector2 toMe = point.Position - otherPoint.Position;  // get a vector from the argument point to me
            Vector2 midVector = (point.Position + otherPoint.Position) / 2.0f;
            if (toMe.Length() < 0.0001) toMe.X = 1.0f;  // if the points are the same
            
            float radius = toMe.Length();
            if (radius < min) radius = min;  // check to make sure we are within min/max of the spring
            if (radius > max) radius = max;

            toMe.Normalize();
            toMe = radius * toMe;

            // Apply to the points
            point.MoveTo(midVector + toMe / 2.0f);
            otherPoint.MoveTo(midVector - toMe / 2.0f);
        }

        /// <summary>
        /// Get the spring force of this constraint
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2 GetForce(VerletPoint point)
        {
            Vector2 toMe = point.Position - otherPoint.Position;
            if (toMe.Length() < 0.0001)
            {
                toMe = new Vector2(1.0f, 0.0f);
            }
            toMe.Normalize();
            Vector2 midVector = otherPoint.Position + toMe * mid;
            Vector2 toMidVector = midVector - point.Position;

            return toMidVector * force;
        }

        /// <summary>
        /// Draw this constraint for debugging purposes
        /// </summary>
        /// <param name="device"></param>
        /// <param name="point"></param>
        public void DebugRender(PrimitiveBatch batch, VerletPoint point, Color color)
        {
            batch.AddVertex(point.Position, color);
            batch.AddVertex(otherPoint.Position, color);
        }
    }
}
