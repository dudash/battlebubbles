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
// File Created: 14 September 2008, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using HBBB.GameComponents.BoardComponents;

namespace HBBB.GameComponents.PlayerComponents
{
    /// <summary>
    /// Clown tries to ambush opponents he's sneaky, and he likes to boost to
    /// go fast.
    /// </summary>
    class PlayerAIClown : PlayerAIHandler
    {
        public PlayerAIClown(PlayerIndex index, GameSession session)
            : base(index, ref session)
        {
            // TODO
        }

        /// <summary>
        /// Process AI for this player, called manually by the HBBBGame Update method
        /// </summary>
        public override void Update(GameTime gameTime, Player player)
        {
            if (!enabled) return;
            if (player == null) return;
            if (this.player == null) this.player = player;

            //TODO
        }
    }
}