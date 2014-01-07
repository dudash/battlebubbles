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
// File Created: 06 September 2008, Jason Dudash
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
using HBBB.GameComponents.BoardComponents;
using HBBB.GameComponents.PowerUps;

namespace HBBB.GameComponents.HUD
{
    /// <summary>
    /// A HUD that shows the time in transition for the player.  It uses the
    /// players position, timeintransition, and currentForm properties.  It
    /// is only visible when the player is in transition and looks like a ring
    /// with little boxes being filled with player colored dots as the time till
    /// completion nears
    /// </summary>
    class TransitionIndicator : IHUDComponent
    {
        private float scale = 0.4f;
        /// <summary>
        /// The player related to this indicator display
        /// </summary>
        private Player player;
        /// <summary>
        /// The texture of the base transition ring
        /// </summary>
        Texture2D transitionCircleTexture;
        public Texture2D TransitionCircleTexture { set { transitionCircleTexture = value; } }

        /// <summary>
        /// Construct with no parameters
        /// </summary>
        public TransitionIndicator(Player player)
        {
            this.player = player;
        }

        /// <summary>
        /// Draw this indicator
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont, PrimitiveBatch primitiveBatch)
        {
            if (player.Form != Player.PlayerForm.IN_TRANSITION) return;
            if (player.LastForm != Player.PlayerForm.BUBBLE) return;
            Debug.Assert(transitionCircleTexture != null);

            // big transition indicators
            //scale = (transitionCircleTexture.Width - (Block.DEFAULT_GRAB_RADIUS + player.GrabModifier)) / transitionCircleTexture.Width;

            // draw outer transitioning circle around bubble
            spriteBatch.Draw(transitionCircleTexture,
                player.Bubble.CenterPoint.Position,
                null, player.PrimaryColor, 0.0f,
                new Vector2(transitionCircleTexture.Width / 2, transitionCircleTexture.Height / 2),
                scale, SpriteEffects.None, 0.0f);

            // draw inner transitioning circle growing out toward outer circle
            float transitionPercent = (player.TimeRequiredToTransition - player.TimeInTransition) / player.TimeRequiredToTransition;
            if (transitionPercent > 1.0f) transitionPercent = 1.0f;
            spriteBatch.Draw(transitionCircleTexture,
                player.Bubble.CenterPoint.Position,
                null, player.PrimaryColor, 0.0f,
                new Vector2(transitionCircleTexture.Width / 2, transitionCircleTexture.Height / 2),
                scale * transitionPercent, SpriteEffects.None, 0.0f);

            // TODO draw the percent transition lights around the outer tranistioning circle
            
            //debug
            //spriteBatch.DrawString(spriteFont, transitionPercent.ToString(), player.Bubble.CenterPoint.Position,Color.White);
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
