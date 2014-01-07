#region Copyright
//-----------------------------------------------------------------------------
// Copyright (C)2008 Jason Dudash, GNU GPLv3.
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
// File Created: 13 September 2008, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HBBB.Core.Graphics;
using HBBB.Core.MassSpring.Verlet;
using HBBB.GameComponents.PlayerComponents;
using HBBB.GameComponents.PowerUps;

namespace HBBB.GameComponents.HUD
{
    /// <summary>
    /// A HUD that shows a visualization for the powerups being applied to
    /// the player
    /// </summary>
    class PowerUpIndicator : IHUDComponent
    {
        const float RING_SCALE = 0.5f;
        int blinkFlag = 0;

        /// <summary>
        /// The player related to this indicator display
        /// </summary>
        private Player player;
        /// <summary>
        /// The texture for good powerups
        /// </summary>
        Texture2D plusRingTexture;
        public Texture2D PlusRingTexture { set { plusRingTexture = value; } }
        /// <summary>
        /// The texture for bad powerups
        /// </summary>
        Texture2D minusRingTexture;
        public Texture2D MinusRingTexture { set { minusRingTexture = value; } }
        /// <summary>
        /// Construct with no parameters
        /// </summary>
        public PowerUpIndicator(Player player)
        {
            this.player = player;
        }

        /// <summary>
        /// Draw this indicator
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont, PrimitiveBatch primitiveBatch)
        {
            if (player.ActivePowerUps.Count < 1) return;  // no active pups to draw

            if (blinkFlag > 100) blinkFlag = 0;

            // TODO this could be a lot more interesting.  for now just draw a red ring for negative
            // powerups and a greeen ring for positive powerups affecting the player
            bool hasAPositive = false;
            bool hasANegative = false;
            foreach (PowerUp pup in player.ActivePowerUps)
            {
                if (pup.IsPositive) hasAPositive = true;
                else hasANegative = true;
            }

            // draw positive and negative rings
            if (hasAPositive && blinkFlag < 50)
            {
                spriteBatch.Draw(plusRingTexture,
                    player.Bubble.CenterPoint.Position,
                    null, Color.Green, 0.0f,
                    new Vector2(plusRingTexture.Width / 2, plusRingTexture.Height / 2),
                    RING_SCALE, SpriteEffects.None, 0.0f);
            }
            if (hasANegative && blinkFlag > 50)
            {
                spriteBatch.Draw(minusRingTexture,
                    player.Bubble.CenterPoint.Position,
                    null, Color.Red, 0.0f,
                    new Vector2(minusRingTexture.Width / 2, minusRingTexture.Height / 2),
                    RING_SCALE, SpriteEffects.None, 0.0f);
            }
            blinkFlag++;
        }

#if DEBUG
        /// <summary>
        /// Render as debug mode
        /// </summary>
        /// <param name="batch"></param>
        public void DebugRender(PrimitiveBatch batch)
        {
            // TODO
        }
#endif
    }
}
