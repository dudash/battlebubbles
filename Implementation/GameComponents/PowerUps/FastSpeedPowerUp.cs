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
// File Created: 13 Septeber 2008, Jason Dudash
//-----------------------------------------------------------------------------
#endregion

using HBBB.GameComponents.PlayerComponents;
using HBBB.GameComponents.BoardComponents;
using Microsoft.Xna.Framework;

namespace HBBB.GameComponents.PowerUps
{
    /// <summary>
    /// Derived from PowerUp, this class fasts the affected players speed, making it 
    /// easy from him to move around the board quickly.
    /// </summary>
    class FastSpeedPowerUp : PowerUp
    {
        Player affectedPlayer;

        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bounds"></param>
        public FastSpeedPowerUp(int id, Vector2 position)
            : base(id, position)
        {
            this.Texture = PowerUpFactory.GetTexture(PowerUpFactory.PowerUpId.POWERUP_FAST_SPEED);
            isPositive = true;
        }

        /// <summary>
        /// Slow the player
        /// </summary>
        public override void Execute(Board board, ref Player affectedPlayer, ref Slot affectedSlot)
        {
            isActiveFlag = true;

            // TODO see if this power up cancels any existing affected player powerups or aggregates

            this.remainingExecutionTime = 15.0f;  // by default it lasts 15 seconds
            this.affectedPlayer = affectedPlayer;
            affectedPlayer.MovementType = Player.PlayerMovementType.FAST_MOVEMENT;
            affectedPlayer.AddActivePowerUp(this);
        }

        /// <summary>
        /// Return the player to his normal speed
        /// </summary>
        public override void FinishExecute()
        {
            affectedPlayer.MovementType = Player.PlayerMovementType.NORMAL_MOVEMENT;
            affectedPlayer.RemoveActivePowerUp(this);
            isActiveFlag = false;
        }

        /// <summary>
        /// Get the name of the powerup
        /// </summary>
        /// <returns></returns>
        public override string Name()
        {
            return "Fast Speed";
        }
    }
}
