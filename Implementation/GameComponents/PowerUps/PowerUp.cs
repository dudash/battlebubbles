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
// File Created: 24 October 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HBBB.Core;
using HBBB.GameComponents.PlayerComponents;
using HBBB.GameComponents.BoardComponents;

namespace HBBB.GameComponents.PowerUps
{
    /// <summary>
    /// The base class for power up modifiers.  Power ups affect a specific player
    /// when used.
    /// </summary>
    abstract class PowerUp : BoardComponents.Block
    {
        protected const float POWERUP_ROTATION_SPEED = 0.03f;
        /// <summary>
        /// if this power up is currently executing
        /// </summary>
        protected bool isActiveFlag = false;
        public bool IsActive { get { return isActiveFlag; } }
        /// <summary>
        /// how much execution time remoains
        /// </summary>
        protected float remainingExecutionTime = 0.0f;
        /// <summary>
        /// is this a good powerup
        /// </summary>
        protected bool isPositive = false;
        public bool IsPositive { get { return isPositive; } }

        /// <summary>
        /// Conbstruct
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bounds"></param>
        public PowerUp(int id, Vector2 position)
            : base(id, position)
        {
            this.rotationSpeed = POWERUP_ROTATION_SPEED;
        }

        /// <summary>
        /// Called to execute the power up on a specific player
        /// </summary>
        abstract public void Execute(Board board, ref Player affectedPlayer, ref Slot affectedSlot);

        /// <summary>
        /// Called to revert the changes the powerup made to the player
        /// </summary>
        abstract public void FinishExecute();

        /// <summary>
        /// The name of this powerup
        /// </summary>
        /// <returns></returns>
        abstract public string Name();

        /// <summary>
        /// Check to see if the time has expired
        /// </summary>
        /// <returns></returns>
        virtual public bool IsTimeExpired(GameTime gameTime)
        {
            if (!isActiveFlag) return false;

            remainingExecutionTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (remainingExecutionTime <= 0.0f) return true;
            return false;
        }
    }
}
