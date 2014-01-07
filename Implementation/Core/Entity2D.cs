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
// File Created: 24 August 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;

namespace HBBB.Core
{
    /// <summary>
    /// Base entity class provides an inheritable position and rotation in 2D
    /// </summary>
    public class Entity2D
    {
        protected Vector2 lastPosition = new Vector2(0, 0);
        public virtual Vector2 LastPosition
        {
            get { return lastPosition; }
            set { lastPosition = value; }

        }

        protected Vector2 position = new Vector2(0,0);
        public virtual Vector2 Position
        {
            get { return position; }
            set
            {
                lastPosition = position;
                position = value;
            }
        }

        protected float rotation = 0.0f;
        public virtual float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        public Entity2D()
        {
        }

        public Entity2D(Vector2 position)
        {
            this.position = position;
            this.lastPosition = position;
        }

        public void ResetToLastPosition()
        {
            this.position = this.lastPosition;
        }
    }
}
