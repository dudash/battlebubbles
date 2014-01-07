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
// File Created: 08 February 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HBBB.Core.Graphics;

namespace HBBB.GameComponents.BoardComponents
{
    /// <summary>
    /// An obstruction that runs along an edge of a slot on the board.
    /// Combined each 
    /// </summary>
    [Serializable]
    public class ObstructionLattice : Obstruction
    {
        /// <summary>
        /// Parameterless constructor (needed for serialization)
        /// </summary>
        public ObstructionLattice()
        {
        }

        #region Circle Collisions
        /// <summary>
        /// Does the arg circle intersect the obstruction
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public override bool IntersectsCircle(Vector2 center, float radius)
        {
            // TODO
            return false;
        }
        #endregion

        #region Point Collisions
        /// <summary>
        /// Does this obstruction contain the argument point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public override bool ContainsPoint(Vector2 point)
        {
            // TODO
            return false;
        }

        /// <summary>
        /// Returns the point of collision
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        public override Vector2 GetCollisionPoint(Vector2 point)
        {
            // TODO
            return new Vector2();
        }

        /// <summary>
        /// Return a normal vector to the collision surface
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        public override Vector2 GetCollisionNormal(Vector2 point)
        {
            // TODO
            return new Vector2();
        }
        #endregion

        #region Line Segment Collisions
        /// <summary>
        /// Check to see if any of the argument line is contained in this obstruction
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public override bool ContainsLine(Vector2 startPoint, Vector2 endPoint)
        {
            // TODO 
            return false;
        }
        #endregion

        /// <summary>
        /// Draw this obstruction
        /// </summary>
        /// <param name="batch"></param>
        public override void Draw(SpriteBatch batch)
        {
            // TODO
        }

        /// <summary>
        /// Draw the bounds of this obstruction
        /// </summary>
        /// <param name="batch"></param>
        public override void DebugRender(PrimitiveBatch batch)
        {
            // TODO
        }
    }
}
