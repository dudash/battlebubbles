#region Copyright
//-----------------------------------------------------------------------------
// Copyright (C)2007-2008 Jason Dudash, GNU GPLv3.
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
// File Created: 31 March 2008, Jason Dudash
//-----------------------------------------------------------------------------
#endregion

using HBBB.GameComponents.PlayerComponents;
using HBBB.GameComponents.BoardComponents;
using Microsoft.Xna.Framework;

namespace HBBB.GameComponents.PowerUps
{
    /// <summary>
    /// Derived from PowerUp, this class locks all of the affected players unlocked but set blocks.
    /// </summary>
    class LockAllPowerUp : PowerUp
    {
        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bounds"></param>
        public LockAllPowerUp(int id, Vector2 position)
            : base(id, position)
        {
            this.Texture = PowerUpFactory.GetTexture(PowerUpFactory.PowerUpId.POWERUP_LOCK_ALL);
            isPositive = true;
        }

        /// <summary>
        /// Slow the player
        /// </summary>
        public override void Execute(Board board, ref Player affectedPlayer, ref Slot affectedSlot)
        {
            isActiveFlag = true;
            foreach (Block b in board.blocksInSlots)
            {
                if (b.IsLocked) continue;
                if (b.OwningPlayer != affectedPlayer) continue;
                b.TimeTillLock = 0.0f;
            }
        }

        /// <summary>
        /// Return the player to his normal speed
        /// </summary>
        public override void FinishExecute()
        {
            isActiveFlag = false;
        }

        /// <summary>
        /// Get the name of the powerup
        /// </summary>
        /// <returns></returns>
        public override string Name()
        {
            return "Lock All";
        }
    }
}
