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
// File Created: 1 April 2008, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using HBBB.Core;
using HBBB.Core.Menus;
using HBBB.Core.Input;
using HBBB.GameComponents.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace HBBB.GameComponents.Menus
{
    /// <summary>
    /// Display user controls
    /// </summary>
    class ShowControlsMenu : BaseMenu
    {
        private static string MENU_ID = "SHOWCONTROLS_MENU";
        public static string MenuId { get { return MENU_ID; } }

        Texture2D backgroundTexture;
        Texture2D startToStartTexture;

        double flashTime = 15.0;
        bool showStartToStart = false;

        /// <summary>
        /// Construct the OptionsMenu
        /// </summary>
        /// <param name="parentSystem"></param>
        public ShowControlsMenu(Game game, BaseMenuSystem parentSystem)
            : base(game, parentSystem)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            backgroundTexture = content.Load<Texture2D>(@"W_A_D\Textures\menus\controls_screen");
            startToStartTexture = content.Load<Texture2D>(@"W_A_D\Textures\menus\press_to_start");
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void UnloadContent()
        {
            content.Unload();
            base.UnloadContent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (parentSystem.CurrentMenu != this) return;

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height), Color.White);
            if (showStartToStart) spriteBatch.Draw(startToStartTexture,
                new Rectangle(this.Game.GraphicsDevice.Viewport.Width / 2 - 180, this.Game.GraphicsDevice.Viewport.Height / 2, 375, 50),
                Color.White);

            //Color reddish = new Color(200, 55, 50);
            //spriteBatch.DrawString(spriteFont, "Controls", new Vector2(this.GraphicsDevice.Viewport.Width / 2.5f, 103), Color.Black); // the shadow
            //spriteBatch.DrawString(spriteFont, "Controls", new Vector2(this.GraphicsDevice.Viewport.Width / 2.5f, 100), reddish); // font text

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (parentSystem.CurrentMenu != this) return;

            flashTime -= gameTime.ElapsedGameTime.TotalSeconds;
            if (flashTime <= 0.0)
            {
                flashTime = 1.0;
                showStartToStart = !showStartToStart;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// Handle button clicks
        /// </summary>
        /// <param name="index"></param>
        /// <param name="details"></param>
        public override void OnButtonClick(PlayerIndex index, GamePadButtonEventDetails details)
        {
            if (parentSystem.CurrentMenu != this) return;

            if (details.Button == GamePadWrapper.ButtonId.START ||
                details.Button == GamePadWrapper.ButtonId.A)
            {
                GameAudio.PlayCue("click");
                parentSystem.RequestStartSession();
                return;
            }
            else if (details.Button == GamePadWrapper.ButtonId.BACK ||
                details.Button == GamePadWrapper.ButtonId.B)
            {
                GameAudio.PlayCue("back");
                parentSystem.TransitionToMenu(LevelSelectionMenu.MenuId);
                return;
            }
        }
    }
}
