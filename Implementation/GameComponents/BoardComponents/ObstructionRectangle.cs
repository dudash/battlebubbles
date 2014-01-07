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
// File Created: 05 October 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HBBB.Core.Graphics;
using HBBB.Core.Math;

namespace HBBB.GameComponents.BoardComponents
{
    /// <summary>
    /// A rectangular bound version of the abstract board obstruction class
    /// </summary>
    [Serializable]
    public class ObstructionRectangle : Obstruction
    {
        /// <summary>
        /// The bounding rectangle of this obstruction
        /// </summary>
        private Rectangle bounds;
        public Rectangle Bounds
        {
            get { return bounds; }
            set { bounds = value; }
        }

        /// <summary>
        /// Construct with no bounds
        /// </summary>
        /// <param name="bounds"></param>
        public ObstructionRectangle()
        {
            this.bounds = new Rectangle();
        }

        /// <summary>
        /// Construct with the bounds
        /// </summary>
        /// <param name="bounds"></param>
        public ObstructionRectangle(Rectangle bounds)
        {
            this.bounds = bounds;
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
            if (point.X < bounds.Left) return false;
            else if (point.X > bounds.Right) return false;
            else if (point.Y > bounds.Bottom) return false;
            else if (point.Y < bounds.Top) return false;
            return true;
        }

        /// <summary>
        /// Returns the point of collision
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        public override Vector2 GetCollisionPoint(Vector2 point)
        {
            // find the X distance from the left and right sides
            float dL = System.Math.Abs(bounds.Left - point.X);
            float dR = System.Math.Abs(bounds.Right - point.X);
            // find the Y distance from the top and bottom sides
            float dT = System.Math.Abs(bounds.Top - point.Y);
            float dB = System.Math.Abs(bounds.Bottom - point.Y);

            if (dL < dR && dL < dT && dL < dB) return new Vector2(bounds.Left, point.Y);
            else if (dR < dL && dR < dT && dR < dB) return new Vector2(bounds.Right, point.Y);
            else if (dT < dL && dT < dR && dT < dB) return new Vector2(point.X, bounds.Top);
            else return new Vector2(point.X, bounds.Bottom);
        }

        /// <summary>
        /// Return a normal vector to the collision surface
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        public override Vector2 GetCollisionNormal(Vector2 point)
        {
            // find the X distance from the left and right sides
            float dL = System.Math.Abs(bounds.Left - point.X);
            float dR = System.Math.Abs(bounds.Right - point.X);
            // find the Y distance from the top and bottom sides
            float dT = System.Math.Abs(bounds.Top - point.Y);
            float dB = System.Math.Abs(bounds.Bottom - point.Y);

            if (dL < dR && dL < dT && dL < dB) return new Vector2(-1, 0); // normal to the left
            else if (dR < dL && dR < dT && dR < dB) return new Vector2(1, 0); // normal to the right
            else if (dT < dL && dT < dR && dT < dB) return new Vector2(0, -1); // normal to the top
            else return new Vector2(0, 1); // normal to the bottom
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
            if (texture == null) return;
            batch.Draw(texture, new Rectangle(bounds.X, bounds.Y, bounds.Width, bounds.Height), Color.White);
        }

        /// <summary>
        /// Draw the bounds of this obstruction
        /// </summary>
        /// <param name="batch"></param>
        public override void DebugRender(PrimitiveBatch batch)
        {
            batch.Begin(PrimitiveType.LineList);
            batch.AddVertex(new Vector2(bounds.Left, bounds.Top), Color.Red);
            batch.AddVertex(new Vector2(bounds.Left, bounds.Bottom), Color.Red);

            batch.AddVertex(new Vector2(bounds.Left, bounds.Bottom), Color.Red);
            batch.AddVertex(new Vector2(bounds.Right, bounds.Bottom), Color.Red);

            batch.AddVertex(new Vector2(bounds.Right, bounds.Bottom), Color.Red);
            batch.AddVertex(new Vector2(bounds.Right, bounds.Top), Color.Red);

            batch.AddVertex(new Vector2(bounds.Right, bounds.Top), Color.Red);
            batch.AddVertex(new Vector2(bounds.Left, bounds.Top), Color.Red);
            batch.End();
        }
    }
}
