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
    /// A constraint that is inflexible, fully-rigid.  For use with VerletPoint in a VerletSystem.
    /// </summary>
    class RigidConstraint : IVerletConstraint
    {
        VerletPoint otherPoint;
        float radius;

        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="otherPoint"></param>
        /// <param name="radius"></param>
        public RigidConstraint(VerletPoint otherPoint, float radius)
        {
            this.otherPoint = otherPoint;
            this.radius = radius;
        }

        /// <summary>
        /// Calculate and apply a rigid spring constraint
        /// </summary>
        /// <param name="point"></param>
        public void Satisfy(VerletPoint point)
        {
            if (otherPoint == null) return;
            Vector2 toMe = point.Position - otherPoint.Position;  // get a vector from the argument point to me
            Vector2 midVector = (point.Position + otherPoint.Position) / 2.0f;
            if (toMe.Length() < 0.0001) toMe.X = 1.0f;  // if the points are the same
            
            toMe.Normalize();
            toMe = radius * toMe;

            // Apply to the points
            point.MoveTo(midVector + toMe / 2.0f);
            otherPoint.MoveTo(midVector - toMe / 2.0f);
        }

        /// <summary>
        /// Force behind this constraint is zero because it is fully rigid
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2 GetForce(VerletPoint point)
        {
            return new Vector2(0.0f, 0.0f); 
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
