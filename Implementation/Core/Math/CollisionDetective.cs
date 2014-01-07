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
using Microsoft.Xna.Framework;

namespace HBBB.Core.Math
{
    /// <summary>
    /// A helper class to check if collisions have occured between objects
    /// </summary>
    class CollisionDetective
    {
        #region Point to Rectangle Collision
        /// <summary>
        /// See if this argument point collides with this argument box
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        public static bool CheckCollision(Vector2 point1, Rectangle box)
        {
            if (point1.X < box.Left) return false;
            else if (point1.X > box.Right) return false;
            else if (point1.Y > box.Bottom) return false;
            else if (point1.Y < box.Top) return false;
            return true;
        }

        /// <summary>
        /// Return the point of the collision
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        public static Vector2 GetCollisionNormal(Vector2 point1, Rectangle box)
        {
            float dL = System.Math.Abs(box.Left - point1.X);
            float dR = System.Math.Abs(box.Right - point1.X);
            float dT = System.Math.Abs(box.Top - point1.Y);
            float dB = System.Math.Abs(box.Bottom - point1.Y);

            if (dL < dR && dL < dT && dL < dB) return new Vector2(-1, 0);
            else if (dR < dL && dR < dT && dR < dB) return new Vector2(1, 0);
            else if (dT < dL && dT < dR && dT < dB) return new Vector2(0, -1);
            else return new Vector2(0, 1);
        }

        /// <summary>
        /// Return the point of collision
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="box"></param>
        /// <returns></returns>
        public static Vector2 GetCollisionPoint(Vector2 point1, Rectangle box)
        {
            float dL = System.Math.Abs(box.Left - point1.X);
            float dR = System.Math.Abs(box.Right - point1.X);
            float dT = System.Math.Abs(box.Top - point1.Y);
            float dB = System.Math.Abs(box.Bottom - point1.Y);

            if (dL < dR && dL < dT && dL < dB) return new Vector2(box.Left, point1.Y);
            else if (dR < dL && dR < dT && dR < dB) return new Vector2(box.Right, point1.Y);
            else if (dT < dL && dT < dR && dT < dB) return new Vector2(point1.X, box.Top);
            else return new Vector2(point1.X, box.Bottom);
        }
        #endregion

        #region Point to Circle Collision
        /// <summary>
        /// See if the argument point collides with the argument circle
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static bool CheckCollision(Vector2 point1, Vector2 centerPoint, float radius)
        {
            // check to see if distance between points is greater than radius
            double distance = System.Math.Sqrt(System.Math.Pow(centerPoint.X - point1.X, 2) + System.Math.Pow(centerPoint.Y - point1.Y, 2));
            if (distance < radius) return true;
            return false;
        }
        #endregion

        #region Circle to Circle Collision
        /// <summary>
        /// See if the argument circles collide
        /// </summary>
        /// <param name="center1"></param>
        /// <param name="radius1"></param>
        /// <param name="center2"></param>
        /// <param name="radius2"></param>
        /// <returns></returns>
        public static bool CheckCollision(Vector2 center1, float radius1, Vector2 center2, float radius2)
        {
            // check to see if distance between points is greater than the radius' added together
            double distance = System.Math.Sqrt(System.Math.Pow(center1.X - center2.X, 2) + System.Math.Pow(center1.Y - center2.Y, 2));
            if (distance < radius1 + radius2) return true;
            return false;
        }
        #endregion

        #region Circle to Rectangle Collision
        /// <summary>
        /// See if the argument circles collide (rough calc)
        /// </summary>
        public static bool CheckCollision(Vector2 center, float radius, Rectangle box)
        {
            Vector2 point1;
            point1 = new Vector2(center.X + radius, center.Y);
            if (point1.X < box.Left) return false;

            point1 = new Vector2(center.X - radius, center.Y);
            if (point1.X > box.Right) return false;

            point1 = new Vector2(center.X, center.Y - radius);
            if (point1.Y > box.Bottom) return false;

            point1 = new Vector2(center.X, center.Y + radius);
            if (point1.Y < box.Top) return false;

            return true;
        }
        #endregion

        #region Line Segment to Line Segment Collision
        /// <summary>
        /// Check to see if the argument lines collide
        /// </summary>
        /// <param name="start1"></param>
        /// <param name="end1"></param>
        /// <param name="start2"></param>
        /// <param name="end2"></param>
        /// <returns></returns>
        public static bool CheckCollision(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
        {
            float a = (end1.Y - start1.Y)/(end1.X - start1.X);
            float b = (end1.X*start1.Y - start1.X*end1.Y)/(end1.X - start1.X);
            float c = (end2.Y - start2.Y)/(end2.X - start2.X);
            float d = (end2.X*start2.Y - start2.X*end2.Y)/(end2.X - start2.X);
            float x = (d - b) / (a - c);
            float y = (a * d - b * c) / (a - c);
            if (start1.X < x && x < end1.X &&
                start1.Y < y && y < end1.Y &&
                start2.X < x && x < end2.X &&
                start2.Y < y && y < end2.Y)
                return true;
            
            return false;
        }

        /// <summary>
        /// REturn the where lines will collide
        /// </summary>
        /// <param name="start1"></param>
        /// <param name="end1"></param>
        /// <param name="start2"></param>
        /// <param name="end2"></param>
        /// <returns></returns>
        public static Vector2 GetCollisionPoint(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2)
        {
            float a = (end1.Y - start1.Y) / (end1.X - start1.X);
            float b = (end1.X * start1.Y - start1.X * end1.Y) / (end1.X - start1.X);
            float c = (end2.Y - start2.Y) / (end2.X - start2.X);
            float d = (end2.X * start2.Y - start2.X * end2.Y) / (end2.X - start2.X);
            float x = (d - b) / (a - c);
            float y = (a * d - b * c) / (a - c);
            return new Vector2(x, y);
        }
        #endregion

        #region Line Segment to Rectangle Collision
        #endregion

        #region Line Segment to Circle Collision
        #endregion
    }
}
