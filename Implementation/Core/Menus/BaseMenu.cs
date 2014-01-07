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
// File Created: 14 October 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using HBBB.Core.Input;

namespace HBBB.Core.Menus
{
    /// <summary>
    /// The base class for game menus provides a common background and
    /// common sounds effects
    /// </summary>
    abstract class BaseMenu : DrawableGameComponent, IMenu
    {
        /// <summary>
        /// The parent system that created this menu
        /// </summary>
        protected BaseMenuSystem parentSystem;
        /// <summary>
        /// For managing game texture and stuff
        /// </summary>
        protected ContentManager content;
        /// <summary>
        /// A common batch for drawing sprites
        /// </summary>
        protected SpriteBatch spriteBatch;
        /// <summary>
        /// A common font for drawing text
        /// </summary>
        protected SpriteFont spriteFont;
        /// <summary>
        /// kepp track of what happened last
        /// </summary>
        protected GamePadState prevState;
        /// <summary>
        /// keep track of what is happening now
        /// </summary>
        protected GamePadState currentState;

        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="parentSystem"></param>
        public BaseMenu(Game game, BaseMenuSystem parentSystem) : base(game)
        {
            content = new ContentManager(game.Services);
            this.parentSystem = parentSystem;
        }

        /// <summary>
        /// load
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            spriteBatch = new SpriteBatch(this.GraphicsDevice);
            spriteFont = content.Load<SpriteFont>(@"W_A_D\Fonts\Berlin San FB Demi 28");
        }

        /// <summary>
        /// unload
        /// </summary>
        protected override void UnloadContent()
        {
            content.Unload();
            base.UnloadContent();
        }

        /// <summary>
        /// Darw this menu
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            // any consistant menu details can go here it will be done after the derived class drawing
        }

        /// <summary>
        /// Update this menu
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            // any common updates, calculations, etc go here it will be done after the derived class drawing
        }

        /// <summary>
        /// Handle button clicks
        /// </summary>
        /// <param name="index"></param>
        /// <param name="details"></param>
        public virtual void OnButtonClick(PlayerIndex index, GamePadButtonEventDetails details)
        {
            // common button actions
        }

        /// <summary>
        /// Handle the analog sticks
        /// </summary>
        /// <param name="index"></param>
        /// <param name="details"></param>
        public virtual void OnAnalogMovement(PlayerIndex index, GamePadAnalogEventDetails details)
        {
            // common analog activity
        }

        /// <summary>
        /// Process a transtion command line
        /// </summary>
        /// <param name="args"></param>
        public virtual void ProcessTransitionArgs(string args)
        {
            // common arg processing
        }
    }
}
