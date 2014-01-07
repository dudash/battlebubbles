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
// File Created: 18 September 2008, Jason Dudash
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
    /// A HUD that shows that the player is defending a block
    /// </summary>
    class DefendingIndicator : IHUDComponent
    {
        private float scale = 0.5f;
        /// <summary>
        /// The player related to this indicator display
        /// </summary>
        private Player player;
        /// <summary>
        /// The texture of the ring
        /// </summary>
        Texture2D defenseRingTexture;
        public Texture2D DefenseRingTexture { set { defenseRingTexture = value; } }

        /// <summary>
        /// Construct with no parameters
        /// </summary>
        public DefendingIndicator(Player player)
        {
            this.player = player;
        }

        /// <summary>
        /// Draw this indicator
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont, PrimitiveBatch primitiveBatch)
        {
            if (!player.IsTryingToDefend) return;
            if (!player.IsAbleToDefend) return;
            Debug.Assert(defenseRingTexture != null);
            
            // draw 5 rings at randomly changing locations around the player
            float xOffset, yOffset;
            xOffset = HBBB.Core.Math.Random.NextFloat(-15.0f, 15.0f);
            yOffset = HBBB.Core.Math.Random.NextFloat(-15.0f, 15.0f);
            spriteBatch.Draw(defenseRingTexture,
                new Vector2(player.Bubble.CenterPoint.Position.X + xOffset, player.Bubble.CenterPoint.Position.Y + yOffset),
                null, player.PrimaryColor, 0.0f,
                new Vector2(defenseRingTexture.Width / 2, defenseRingTexture.Height / 2),
                scale, SpriteEffects.None, 0.0f);

            //2
            xOffset = HBBB.Core.Math.Random.NextFloat(-15.0f, 15.0f);
            yOffset = HBBB.Core.Math.Random.NextFloat(-15.0f, 15.0f); spriteBatch.Draw(defenseRingTexture,
                new Vector2(player.Bubble.CenterPoint.Position.X + xOffset, player.Bubble.CenterPoint.Position.Y + yOffset),
                null, player.SecondaryColor, 0.0f,
                new Vector2(defenseRingTexture.Width / 2, defenseRingTexture.Height / 2),
                scale, SpriteEffects.None, 0.0f);

            //3
            xOffset = HBBB.Core.Math.Random.NextFloat(-15.0f, 15.0f);
            yOffset = HBBB.Core.Math.Random.NextFloat(-15.0f, 15.0f);
            spriteBatch.Draw(defenseRingTexture,
                new Vector2(player.Bubble.CenterPoint.Position.X + xOffset, player.Bubble.CenterPoint.Position.Y + yOffset),
                null, player.PrimaryColor, 0.0f,
                new Vector2(defenseRingTexture.Width / 2, defenseRingTexture.Height / 2),
                scale, SpriteEffects.None, 0.0f);

            //4
            xOffset = HBBB.Core.Math.Random.NextFloat(-15.0f, 15.0f);
            yOffset = HBBB.Core.Math.Random.NextFloat(-15.0f, 15.0f); ;
            spriteBatch.Draw(defenseRingTexture,
                new Vector2(player.Bubble.CenterPoint.Position.X + xOffset, player.Bubble.CenterPoint.Position.Y + yOffset),
                null, player.SecondaryColor, 0.0f,
                new Vector2(defenseRingTexture.Width / 2, defenseRingTexture.Height / 2),
                scale+0.1f, SpriteEffects.None, 0.0f);

            //5
            xOffset = HBBB.Core.Math.Random.NextFloat(-15.0f, 15.0f);
            yOffset = HBBB.Core.Math.Random.NextFloat(-15.0f, 15.0f);
            spriteBatch.Draw(defenseRingTexture,
                new Vector2(player.Bubble.CenterPoint.Position.X + xOffset, player.Bubble.CenterPoint.Position.Y + yOffset),
                null, player.PrimaryColor, 0.0f,
                new Vector2(defenseRingTexture.Width / 2, defenseRingTexture.Height / 2),
                scale+0.05f, SpriteEffects.None, 0.0f);

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
