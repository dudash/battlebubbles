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
using HBBB.GameComponents.PowerUps;
using HBBB.Core.Particle2D;

namespace HBBB.GameComponents.HUD
{
    /// <summary>
    /// A HUD that shows the players boost power.  DUDASH - this class is kinda hacked together right now
    /// </summary>
    class BoostMeter : DrawableGameComponent, IHUDComponent, IDisposable
    {
        /// <summary>
        /// The player related to this HUD
        /// </summary>
        private Player player;
        /// <summary>
        /// The texture of the meter background
        /// </summary>
        Texture2D meterTexture;
        public Texture2D MeterTexture { set { meterTexture = value; } }

        private ParticleSystem pSystem;  // we a hack using this, not letting XNA manage it, doing it ourselves

        /// <summary>
        /// Construct with no parameters
        /// </summary>
        public BoostMeter(Game game, Player player) : base(game)
        {
            this.player = player;
            pSystem = new ParticleSystem(game);
            pSystem.Emitter.ParticleColor = player.PrimaryColor;
            pSystem.Emitter.ParticleColorTwo = player.SecondaryColor;
            pSystem.Emitter.ParticleColorThree = Color.Yellow;
            game.Components.Add(pSystem);
        }

        /// <summary>
        /// Load textures
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
        }

        /// <summary>
        /// handle XNA update calls, pass to pSystem
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (pSystem != null) pSystem.Update(gameTime);
            if (player != null)
            {
                pSystem.Position = player.Bubble.CenterPoint.Position;
                pSystem.IsOn = player.IsBoostingSpeed;
            }
            else
            {
                pSystem.IsOn = false;
            }
        }

        /// <summary>
        /// Handle XNA draw calls
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            // the system will draw itself and we do additional drawing through the HUD draw calls
        }

        /// <summary>
        /// Draw this indicator, managed by HBBB game.  It doesn't do anything because this is also 
        /// a DrawableGameComponent that has it's own draw calls
        /// yeah it sucks, but i'm in crunch mode, we can fix it later...
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, SpriteFont spriteFont, PrimitiveBatch primitiveBatch)
        {
            if (player == null) return;
            if (!player.IsBoostingSpeed) return;

            // Do any other drawing here
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

        /// <summary>
        /// remove the game particle system
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Game.Components.Remove(pSystem);
        }
    }
}
