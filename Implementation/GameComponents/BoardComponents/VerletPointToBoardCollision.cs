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
using HBBB.Core.Math;

namespace HBBB.GameComponents.BoardComponents
{
    /// <summary>
    /// Constrain verlet points to avoid world data
    /// </summary>
    class VerletPointToBoardCollision : IVerletConstraint
    {
        List<Obstruction> boardObstructions;

        /// <summary>
        /// Construct with the worldborder
        /// </summary>
        /// <param name="boardDimensions"></param>
        public VerletPointToBoardCollision(List<Obstruction> obstructions)
        {
            this.boardObstructions = obstructions;
        }

        /// <summary>
        /// Calculate if this point collides with the world and fix that if it does
        /// </summary>
        /// <param name="point"></param>
        public void Satisfy(VerletPoint point)
        {
            // Collisiion with complex boards
            foreach (Obstruction obstr in boardObstructions)
            {
                if (obstr.ContainsPoint(point.Position))
                {
                    // The attempted movment of the point
                    Vector2 movement = point.Position - point.LastPosition;
                    // normal of the surface that was collided with
                    Vector2 normal = obstr.GetCollisionNormal(point.Position);
                    // magnitude of the component of motion perpendicular to the surface
                    float perp = Vector2.Dot(movement, normal);
                    // project vector onto the surface
                    Vector2 parallel = movement - normal * perp;

                    // slide the point along the direction of surface along with surface friction
                    //if (!obstr.ContainsPoint(point.LastPosition, point.LastPosition + parallel))
                    //point.MoveTo(point.LastPosition + parallel * obstr.Friction);
                    // fallback case where point is rasied slightly off the collision surface
                    //else
                    point.SetPosition(obstr.GetCollisionPoint(point.Position) + 0.01f * normal);
                }
            }
        }

        /// <summary>
        /// Force behind this constraint is zero because it is fully rigid
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public Vector2 GetForce(VerletPoint point)
        {
            return new Vector2(0, 0);  // no force
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
