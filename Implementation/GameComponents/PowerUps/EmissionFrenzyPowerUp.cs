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
// File Created: 12 April 2008, Jason Dudash
//-----------------------------------------------------------------------------
#endregion

using HBBB.GameComponents.PlayerComponents;
using HBBB.GameComponents.BoardComponents;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace HBBB.GameComponents.PowerUps
{
    /// <summary>
    /// Derived from PowerUp, this class emits a free block 
    /// from each of the players locked blocks
    /// </summary>
    class EmissionFrenzyPowerUp : PowerUp
    {
        //Player affectedPlayer;

        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bounds"></param>
        public EmissionFrenzyPowerUp(int id, Vector2 position)
            : base(id, position)
        {
            this.Texture = PowerUpFactory.GetTexture(PowerUpFactory.PowerUpId.EMISSION_FRENZY);
            isPositive = true;
        }

        /// <summary>
        /// Slow the player
        /// </summary>
        public override void Execute(Board board, ref Player affectedPlayer, ref Slot affectedSlot)
        {
            isActiveFlag = true;
            // find slots to emit from
            List<Slot> emissionSlots = new List<Slot>();
            foreach (Block b in board.BlocksInSlots)
            {
                if (!b.IsLocked) continue;
                if (b.OwningPlayer != affectedPlayer) continue;
                emissionSlots.Add(b.OwningSlot);
            }
            // emit from them
            foreach (Slot s in emissionSlots)
            {
                Block block = board.AddBlockToBoard(new Vector2(s.Bounds.X + (float)s.Bounds.Width / 2.0f, s.Bounds.Y + (float)s.Bounds.Height / 2.0f));
                if (block != null)
                {
                    block.RotationSpeed = Core.Math.Random.NextFloat(0.5f, 1.5f);
                }
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
            return "Emission Frenzy";
        }
    }
}
