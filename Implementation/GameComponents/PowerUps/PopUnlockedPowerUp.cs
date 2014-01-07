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

using System.Collections.Generic;
using HBBB.GameComponents.PlayerComponents;
using HBBB.GameComponents.BoardComponents;
using Microsoft.Xna.Framework;

namespace HBBB.GameComponents.PowerUps
{
    /// <summary>
    /// Derived from PowerUp, this class pops all of the affected players set but unlocked blocks.
    /// </summary>
    class PopUnlockedPowerUp : PowerUp
    {
        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bounds"></param>
        public PopUnlockedPowerUp(int id, Vector2 position)
            : base(id, position)
        {
            this.Texture = PowerUpFactory.GetTexture(PowerUpFactory.PowerUpId.POWERUP_POP_UNLOCKED);
            isPositive = false;
        }

        /// <summary>
        /// Slow the player
        /// </summary>
        public override void Execute(Board board, ref Player affectedPlayer, ref Slot affectedSlot)
        {
            isActiveFlag = true;
            // find blocks to pop
            List<Block> blocksToPop = new List<Block>();
            foreach (Block b in board.BlocksInSlots)
            {
                if (b.IsLocked) continue;
                if (b.OwningPlayer != affectedPlayer) continue;
                blocksToPop.Add(b);
            }
            // pop them out
            foreach (Block b in blocksToPop)
            {
                board.PullBlockFromSlot(b);
                //this.OwningPlayer.Statistics.Steals++;
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
            return "Pop Unlocked";
        }
    }
}
