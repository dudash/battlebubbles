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
// File Created: 19 October 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HBBB.Core.Graphics;
using HBBB.Core.MassSpring.Verlet;

namespace HBBB.GameComponents.BoardComponents
{
    /// <summary>
    /// Constraint line segments to avoid world data
    /// </summary>
    class VerletLineToBoardCollision : IVerletConstraint
    {
        List<Obstruction> boardObstructions;
        VerletPoint otherPoint;

        /// <summary>
        /// Construct with the start point of the line
        /// </summary>
        /// <param name="otherPoint"></param>
        public VerletLineToBoardCollision(List<Obstruction> obstructions, VerletPoint otherPoint)
        {
            this.boardObstructions = obstructions;
            this.otherPoint = otherPoint;
        }

        /// <summary>
        /// /// Calculate if this line collides with the world and fix that if it does
        /// </summary>
        /// <param name="point"></param>
        public void Satisfy(VerletPoint point)
        {
            // We already checked for point to world collisions, so assuming that those
            // constraints have been resolved, we know that if we have a line segment collision,
            // that line segment must be straddling a box corner; so just push it back.
            // (assuming the box is larger than the line segment)
            foreach (Obstruction obstr in boardObstructions)
            {
                // TODO this isn't working!
                if (obstr.ContainsLine(point.Position, otherPoint.Position))
                {
                    // move LS away from obstruction
                    Vector2 a = point.LastPosition;
                    Vector2 b = otherPoint.LastPosition;
                    Vector2 av = point.Position - a;
                    Vector2 bv = otherPoint.Position - b;
                    Vector2 p = (a - b);
                    p.Normalize(); // p is now a normalized vector from a to b

                    // Project a and b onto p
                    Vector2 ap = p * Vector2.Dot(av, p);
                    Vector2 bp = p * Vector2.Dot(bv, p);
                    Vector2 a2 = a + ap;
                    Vector2 b2 = b + bp;

                    point.SetPosition(a2);
                    otherPoint.SetPosition(b2);
                }
            }

            //Vector2 a = point.LastPosition;
            //Vector2 b = otherPoint.LastPosition;
            //Vector2 av = point.Position - a;
            //Vector2 bv = otherPoint.Position - b;
            //Vector2 p = (a - b);
            //p.Normalize(); // p is now a normalized vector from a to b

            //// Project a and b onto p
            //Vector2 ap = p * Vector2.Dot(av, p);
            //Vector2 bp = p * Vector2.Dot(bv, p);
            //Vector2 a2 = a + ap;
            //Vector2 b2 = b + bp;

            // Might also need to check a to a2 and b to b2
            //if (!whisker.CheckCollision(a2, b2) && !whisker.CheckCollision(a, a2) && !whisker.CheckCollision(b, b2))
            //{
            //    point.Set(a2);
            //    otherPoint.Set(b2);
            //}
            //else
            //{
            //    point.Set(point.LastPosition);
            //    otherPoint.Set(otherPoint.LastPosition);
            //}
        }

        /// <summary>
        /// Force behind this constraint is zero because it is fully rigid
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2 GetForce(VerletPoint point)
        {
            return new Vector2(0, 0); // no force
        }

        /// <summary>
        /// Draw this constraint for debugging purposes
        /// </summary>
        /// <param name="device"></param>
        /// <param name="point"></param>
        public void DebugRender(PrimitiveBatch batch, VerletPoint point, Color color)
        {
            // no rendering of this
        }
    }
}
