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
// File Created: 26 October 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HBBB.Core.Graphics;
using HBBB.Core.MassSpring.Verlet;

namespace HBBB.GameComponents.BoardComponents
{
    /// <summary>
    /// Constrain verlet points to avoid world data
    /// </summary>
    class VerletPointToPlayZoneCollision : IVerletConstraint
    {
        Rectangle playZone;

        /// <summary>
        /// Construct with the worldborder
        /// </summary>
        /// <param name="boardDimensions"></param>
        public VerletPointToPlayZoneCollision(Rectangle playZone)
        {
            this.playZone = playZone;
        }

        /// <summary>
        /// Calculate if this point collides with the viewport and fix that if it does.
        /// </summary>
        /// <param name="point"></param>
        public void Satisfy(VerletPoint point)
        {
            if (point.Position.X < playZone.Left || point.Position.X > playZone.Right ||
                point.Position.Y < playZone.Top || point.Position.Y > playZone.Bottom)
            {
                Vector2 failSafePos = new Vector2(point.Position.X, point.Position.Y);
                if (point.Position.X < playZone.Left) failSafePos.X = playZone.Left + 1;
                else if (point.Position.X > playZone.Right) failSafePos.X = playZone.Right - 1;
                if (point.Position.Y < playZone.Top) failSafePos.Y = playZone.Top + 1;
                else if (point.Position.Y > playZone.Bottom) failSafePos.Y = playZone.Bottom - 1;
                
                // TODO try to move back a tick, and move along the normal of the surface
                
                // fail safe set us back to a known point in the viewport
                point.SetPosition(failSafePos);
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
