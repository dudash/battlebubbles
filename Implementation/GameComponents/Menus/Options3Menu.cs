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
// File Created: 07 September 2008, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using HBBB.Core;
using HBBB.Core.Input;
using HBBB.Core.Menus;
using HBBB.GameComponents.Globals;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace HBBB.GameComponents.Menus
{
    /// <summary>
    /// Player and game options
    /// </summary>
    class Options3Menu : BaseMenu
    {
        private static string MENU_ID = "OPTIONS_3_MENU";
        public static string MenuId { get { return MENU_ID; } }

        double forcedInputWaitTime = 0.0;
        const double FORCED_INPUT_DELAY = 0.2;

        Texture2D backgroundTexture;
        //Texture2D startToStartTexture;

        //double flashTime = 10.0;
        //bool showStartToStart = false;

        /// <summary>
        /// Construct the OptionsMenu
        /// </summary>
        /// <param name="parentSystem"></param>
        public Options3Menu(Game game, BaseMenuSystem parentSystem)
            : base(game, parentSystem)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            backgroundTexture = content.Load<Texture2D>(@"W_A_D\Textures\menus\instructions_menu_3");
            //startToStartTexture = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\button_back");
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
        /// Override draw
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (parentSystem.CurrentMenu != this) return;

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height), Color.White);
            /*
            if (showStartToStart) spriteBatch.Draw(startToStartTexture,
                new Rectangle(this.Game.GraphicsDevice.Viewport.Width / 2 - 32, this.Game.GraphicsDevice.Viewport.Height / 2, 64, 64),
                Color.White);
             */
            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Override update
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (parentSystem.CurrentMenu != this) return;
            forcedInputWaitTime += gameTime.ElapsedGameTime.TotalSeconds;  // forced delay in gamepad input
/*
            flashTime -= gameTime.ElapsedGameTime.TotalSeconds;
            if (flashTime <= 0.0)
            {
                flashTime = 1.0;
                showStartToStart = !showStartToStart;
            }
            */
            base.Update(gameTime);
        }

        /// <summary>
        /// Called when the player clicks a button on the gamepad
        /// </summary>
        /// <param name="index"></param>
        /// <param name="details"></param>
        public override void OnButtonClick(PlayerIndex index, GamePadButtonEventDetails details)
        {
            if (parentSystem.CurrentMenu != this) return;

            if (details.Button == GamePadWrapper.ButtonId.BACK || 
                details.Button == GamePadWrapper.ButtonId.B ||
                details.Button == GamePadWrapper.ButtonId.A)
            {
                GameAudio.PlayCue("back");
                parentSystem.TransitionToMenu(MainMenu.MenuId);
            }
            else if (details.Button == GamePadWrapper.ButtonId.D_LEFT ||
                details.Button == GamePadWrapper.ButtonId.LEFT_SHOULDER)
            {
                GameAudio.PlayCue("click");
                parentSystem.TransitionToMenu(Options2Menu.MenuId);
            }
        }

        /// <summary>
        /// left right input
        /// </summary>
        /// <param name="index"></param>
        /// <param name="details"></param>
        public override void OnAnalogMovement(PlayerIndex index, GamePadAnalogEventDetails details)
        {
            if (parentSystem.CurrentMenu != this) return;

            if (forcedInputWaitTime < FORCED_INPUT_DELAY) return;
            else forcedInputWaitTime = 0.0;

            if (details.AnalogButton == GamePadWrapper.AnalogId.LEFT_STICK)
            {
                if (details.StickValue.X > 0.1)
                {
                    GameAudio.PlayCue("click");
                }
                else if (details.StickValue.X < -0.1)
                {
                    GameAudio.PlayCue("click");
                    parentSystem.TransitionToMenu(Options2Menu.MenuId);
                }
            }
        }
    }
}
