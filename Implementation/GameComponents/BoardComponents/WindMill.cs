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
// File Created: 04 September 2008, Jason Dudash
//-----------------------------------------------------------------------------
#endregion

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HBBB.GameComponents.BoardComponents
{
    /// <summary>
    /// A spinning windmill the the player bubbles can collide and be pushed
    /// around by
    /// </summary>
    class WindMill : Core.Entity2D
    {
        /// <summary>
        /// The radius of the bounding circle
        /// </summary>
        protected float boundingRadius;
        public float BoundingRadius { get { return boundingRadius; } }
        /// <summary>
        /// A speed to rotate the wind mill in full cycles per second
        /// </summary>
        protected float rotationSpeed;
        public float RotationSpeed
        {
            get { return rotationSpeed; }
            set { rotationSpeed = value; }
        }

        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="position"></param>
        /// <param name="boundingRadius"></param>
        public WindMill(Vector2 position, float boundingRadius)
        {
            this.position = position;
            this.boundingRadius = boundingRadius;
        }

        /// <summary>
        /// Update the wind mill rotation
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            // use rotation speed and time ellapsed to convert to a rotation angle in radians
            rotation += (float)MathHelper.Pi * rotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        /// <summary>
        /// Draw the windmill
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="spriteFont"></param>
        public void Draw(SpriteBatch batch, SpriteFont spriteFont)
        {
            // TODO draw center
            // TODO draw arms
        }
    }
}