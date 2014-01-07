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
// File Created: 07 February 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HBBB.Core.Graphics;
using HBBB.Core.MassSpring.Verlet;
using HBBB.GameComponents.PlayerComponents;
using HBBB.GameComponents.PowerUps;

namespace HBBB.GameComponents.HUD
{
    /// <summary>
    /// A HUD that shows the player score, powerup status, colors, and
    /// any other relevant player info.  TODO redo this with a better version located in each corner
    /// </summary>
    class PlayerBox : IHUDComponent
    {
        const int OUTTER_LINE_SIZE = 8;
        const int BOX1_SIZE = 40;  // for drawtype two boxes
        const int TRANSITION_INDICATION_PRECISION = 30;  // greater is better (but potentially slower)
        const int TRANSITION_INDICATION_SIZE = 14;
        const int POWERUP_DISPLAY_SIZE = 35;

        /// <summary>
        /// The position of the player points display
        /// </summary>
        Vector2 pointPosition;
        /// <summary>
        /// The position and size of the entire box
        /// </summary>
        private Rectangle bounds;
        /// <summary>
        /// DRAW_TYPE = FULL, The position and size of the inner color box
        /// </summary>
        private Rectangle innerBounds;
        /// <summary>
        /// DRAW_TYPE = TWO_BOXES, A box on the left for displaying player colors
        /// </summary>
        private Rectangle innerBox1;
        /// <summary>
        /// DRAW_TYPE = TWO_BOXES, A box on the right filled black for displaying player stats
        /// </summary>
        private Rectangle innerBox2;
        /// <summary>
        /// Bounds of a box to display the player bubble and payload
        /// </summary>
        private Rectangle bubbleBox;
        /// <summary>
        /// Bounds of a box to display powers affecting the player
        /// </summary>
        private Rectangle powerUpBox;
        /// <summary>
        /// The player related to this box display
        /// </summary>
        private Player player;
        /// <summary>
        ///  A Texture used to color the player box
        /// </summary>
        private Texture2D texture;
        public Texture2D Texture { set { texture = value; } }

        /// <summary>
        /// Construct with no parameters
        /// </summary>
        public PlayerBox(Player player, Rectangle bounds)
        {
            this.player = player;
            this.bounds = bounds;
            this.innerBounds = new Rectangle(bounds.X + OUTTER_LINE_SIZE/2, bounds.Y + OUTTER_LINE_SIZE/2, bounds.Width - OUTTER_LINE_SIZE, bounds.Height - OUTTER_LINE_SIZE);
            this.innerBox1 = new Rectangle(bounds.X + OUTTER_LINE_SIZE / 2, bounds.Y + OUTTER_LINE_SIZE / 2, BOX1_SIZE, bounds.Height - OUTTER_LINE_SIZE);
            this.innerBox2 = new Rectangle(bounds.X + OUTTER_LINE_SIZE + BOX1_SIZE, bounds.Y + OUTTER_LINE_SIZE / 2,
                bounds.Width - OUTTER_LINE_SIZE - OUTTER_LINE_SIZE / 2 - BOX1_SIZE, bounds.Height - OUTTER_LINE_SIZE);
            this.pointPosition = new Vector2(innerBox2.X + OUTTER_LINE_SIZE, innerBox2.Y - OUTTER_LINE_SIZE);
            this.bubbleBox = new Rectangle(innerBox1.X + innerBox1.Width / 2 - TRANSITION_INDICATION_SIZE/2, innerBox1.Y + TRANSITION_INDICATION_SIZE/2, TRANSITION_INDICATION_SIZE, TRANSITION_INDICATION_SIZE);  // square
            this.powerUpBox = new Rectangle(innerBox2.Right - POWERUP_DISPLAY_SIZE, innerBox2.Top, POWERUP_DISPLAY_SIZE, POWERUP_DISPLAY_SIZE);  // square
        }

        /// <summary>
        /// Draw this player box
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont, PrimitiveBatch primitiveBatch)
        {
            spriteBatch.Draw(texture, bounds, player.PrimaryColor);
            spriteBatch.Draw(texture, innerBox1, Color.Black);
            spriteBatch.Draw(texture, innerBox2, Color.Black);
            
            // if affected by powerup display in box
            int offset = 0;
            foreach (PowerUp pup in player.ActivePowerUps)
            {
                Rectangle pupRect = powerUpBox;
                pupRect.Offset(-offset, 0);
                spriteBatch.Draw(pup.Texture, pupRect, Color.White);
                offset += POWERUP_DISPLAY_SIZE;
            }

            // draw player score
            spriteBatch.DrawString(spriteFont, player.Statistics.Score.ToString(), pointPosition, Color.White);

            // draw avatar
            spriteBatch.Draw(player.PlayerPic, innerBox1, Color.White);
        }

#if DEBUG
        /// <summary>
        /// Render square bounds of the playerbox
        /// </summary>
        /// <param name="batch"></param>
        public void DebugRender(PrimitiveBatch batch)
        {
            batch.Begin(PrimitiveType.LineList);
            batch.AddVertex(new Vector2(bounds.Left, bounds.Top), player.PrimaryColor);
            batch.AddVertex(new Vector2(bounds.Left, bounds.Bottom), player.PrimaryColor);

            batch.AddVertex(new Vector2(bounds.Left, bounds.Bottom), player.PrimaryColor);
            batch.AddVertex(new Vector2(bounds.Right, bounds.Bottom), player.PrimaryColor);

            batch.AddVertex(new Vector2(bounds.Right, bounds.Bottom), player.PrimaryColor);
            batch.AddVertex(new Vector2(bounds.Right, bounds.Top), player.PrimaryColor);

            batch.AddVertex(new Vector2(bounds.Right, bounds.Top), player.PrimaryColor);
            batch.AddVertex(new Vector2(bounds.Left, bounds.Top), player.PrimaryColor);
            batch.End();
        }
#endif
    }
}
