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

namespace HBBB.GameComponents.BoardComponents
{
    /// <summary>
    /// A base class common to all obstructions
    /// </summary>
    [Serializable]
    public abstract class Obstruction
    {
        /// <summary>
        ///  Coefficient of static friction, lower = more sticky
        /// </summary>
        protected float friction = 10.0f;
        public float Friction
        {
            get { return friction; }
            set { friction = value; }
        }
        /// <summary>
        /// Filename of image that visualizes the bounds of the obstruction
        /// </summary>
        protected string textureName = "";
        public string TextureName
        {
            get { return textureName; }
            set { textureName = value; }
        }
        /// <summary>
        /// Image that visualizes the bounds of the obstruction
        /// </summary>
#if !XBOX
        [NonSerialized]
#endif
        protected Texture2D texture;
        public Texture2D Texture
        {
            set { texture = value; }
        }

        #region Circle Intersections
        /// <summary>
        /// does this obstruct intersect the argument circle
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public abstract bool IntersectsCircle(Vector2 center, float radius);
        #endregion

        #region Point Collisions
        /// <summary>
        /// Does this obstruction contain the argument point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public abstract bool ContainsPoint(Vector2 point);

        /// <summary>
        /// Return the point of the collision (assuming there was a collision)
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public abstract Vector2 GetCollisionPoint(Vector2 point);

        /// <summary>
        /// Return a normal to the collision surface
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public abstract Vector2 GetCollisionNormal(Vector2 point);
        #endregion

        #region Line Segment Collisions
        /// <summary>
        /// Check to see if any of the argument line is contained in this obstruction
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public abstract bool ContainsLine(Vector2 startPoint, Vector2 endPoint);
        #endregion

        /// <summary>
        /// Draw this obstruction
        /// </summary>
        /// <param name="batch"></param>
        public abstract void Draw(SpriteBatch batch);

        /// <summary>
        /// Draw the bounds of this obstruction
        /// </summary>
        /// <param name="batch"></param>
        public abstract void DebugRender(PrimitiveBatch batch);
    }
}
