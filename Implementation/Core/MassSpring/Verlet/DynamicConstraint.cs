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
// File Created: 12 December 2007, Jason Dudash
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
    /// This constraint can be semi-rigid or rigid based on the mode you choose.  It can be
    /// changed from semi to fully rigid at runtime.  For use with VerletPoint in a VerletSystem.
    /// </summary>
    class DynamicConstraint : IVerletConstraint
    {
        public enum ConstraintMode { FULLY_RIGID, SEMI_RIGID, OFF };

        ConstraintMode mode = ConstraintMode.SEMI_RIGID;
        VerletPoint otherPoint;
        /// <summary>
        /// minimum length of constraint (only applicable in semi-rigid mode)
        /// </summary>
        float minLength;
        public float MinLength { get { return minLength; } }
        float originalMinLength;
        /// <summary>
        /// The resting length  of the constraint
        /// </summary>
        float restLength;
        public float RestLength { get { return restLength; } }
        float originalRestLength;
        /// <summary>
        /// The maximum length of the constraint (only applicable in semi-rigid mode)
        /// </summary>
        float maxLength;
        public float MaxLength { get { return maxLength; } }
        float originalMaxLength;
        /// <summary>
        /// The force of the constraint (only applicable in semi-rigid mode)
        /// </summary>
        float force;
        public float Force { get { return force; } }
        float originalForce;

        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="otherPoint">other point attached to this constraint</param>
        /// <param name="min">the min length (only applicable in semi-rigid mode)</param>
        /// <param name="mid">the rest length of the constraint</param>
        /// <param name="max">the max length (only applicable in semi-rigid mode)</param>
        /// <param name="force">the force to apply to return to rest length (only applicable in semi-rigid mode)</param>
        public DynamicConstraint(VerletPoint otherPoint, float min, float mid, float max, float force)
        {
            this.otherPoint = otherPoint;
            this.minLength = originalMinLength = min;
            this.restLength = originalRestLength = mid;
            this.maxLength = originalMaxLength = max;
            this.force = originalForce = force;
        }

        /// <summary>
        /// Set the constraint mode
        /// </summary>
        /// <param name="mode"></param>
        public void SetMode(ConstraintMode mode)
        {
            this.mode = mode;
        }

        /// <summary>
        /// Temporarily set the minimum length (only applicable in semi-rigid mode)
        /// </summary>
        /// <param name="value"></param>
        public void UseTempMinLength(float value)
        {
            this.minLength = value;
        }

        /// <summary>
        /// return to the original value
        /// </summary>
        public void ResetMinLength()
        {
            this.minLength = originalMinLength;
        }

        /// <summary>
        /// Temporarily set the default length of the constraint
        /// </summary>
        /// <param name="value"></param>
        public void UseTempRestLength(float value)
        {
            this.restLength = value;
        }

        /// <summary>
        /// return to the original value
        /// </summary>
        public void ResetRestLength()
        {
            this.restLength = originalRestLength;
        }

        /// <summary>
        /// Temporarily set the max length (only applicable in semi-rigid mode)
        /// </summary>
        /// <param name="value"></param>
        public void UseTempMaxLength(float value)
        {
            this.maxLength = value;
        }

        /// <summary>
        /// return to the original value
        /// </summary>
        public void ResetMaxLength()
        {
            this.maxLength = originalMaxLength;
        }

        /// <summary>
        /// Temporarily set the force applied to return to rest length (only applicable in semi-rigid mode)
        /// </summary>
        /// <param name="value"></param>
        public void UseTempSpringForce(float value)
        {
            this.force = value;
        }

        /// <summary>
        /// return to the original value
        /// </summary>
        public void ResetSpringForce()
        {
            this.force = originalForce;
        }

        /// <summary>
        /// Calculate and apply a semi-rigid spring constraint
        /// </summary>
        /// <param name="point"></param>
        public void Satisfy(VerletPoint point)
        {
            if (otherPoint == null || mode == ConstraintMode.OFF) return;

            Vector2 toMe = point.Position - otherPoint.Position;  // get a vector from the argument point to me
            Vector2 midVector = (point.Position + otherPoint.Position) / 2.0f;
            if (toMe.Length() < 0.0001) toMe.X = 1.0f;  // if the points are the same

            float radius;
            if (mode == ConstraintMode.SEMI_RIGID)  // in semi-rigid (spring) mode
            {
                radius = toMe.Length();
                if (radius < minLength) radius = minLength;  // check to make sure we are within min/max of the spring
                if (radius > maxLength) radius = maxLength;
            }
            else  // default to fully rigid
            {
                radius = restLength;
            }

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
            if (mode == ConstraintMode.SEMI_RIGID)  // in semi-rigid (spring) mode
            {
                Vector2 toMe = point.Position - otherPoint.Position;
                if (toMe.Length() < 0.0001)
                {
                    toMe = new Vector2(1.0f, 0.0f);
                }
                toMe.Normalize();
                Vector2 midVector = otherPoint.Position + toMe * restLength;
                Vector2 toMidVector = midVector - point.Position;

                return toMidVector * force;
            }
            else  // default to fully rigid
            {
                return new Vector2(0.0f, 0.0f);
            }
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
