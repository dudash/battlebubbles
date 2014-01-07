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
// File Created: 09 October 2007, Jason Dudash
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
    /// Menu that is displayed when a game is in progess but paused
    /// </summary>
    class PauseMenu : BaseMenu
    {
        private static string MENU_ID = "PAUSE_MENU";
        public static string MenuId { get { return MENU_ID; } }

        enum PauseMenuOption { RETURN_TO_GAME, QUIT }
        PauseMenuOption currentOption = PauseMenuOption.RETURN_TO_GAME;
        PlayerIndex pausingPlayer = PlayerIndex.One;

        double forcedInputWaitTime = 0.0;
        const double FORCED_INPUT_DELAY = 0.2;

        Texture2D backgroundTexture;
        Texture2D hexIcon;
        Texture2D white16x16Texture;
        Texture2D controlsInfoTexture;
        Rectangle RETURN_TO_GAME_POSITION = new Rectangle(500, 350, 50, 50);
        Rectangle QUIT_GAME_POSITION = new Rectangle(500, 400, 50, 50);
        Rectangle CONTROLS_INFO_POSITION = new Rectangle(350, 550, 50, 50);

        /// <summary>
        /// Construct the PauseMenu
        /// </summary>
        /// <param name="parentSystem"></param>
        public PauseMenu(Game game, BaseMenuSystem parentSystem)
            : base(game, parentSystem)
        {
            currentState = prevState = GamePad.GetState(pausingPlayer);
        }

        /// <summary>
        /// The player who paused the game
        /// </summary>
        /// <param name="pausingPlayer"></param>
        public void SetPausingPlayer(PlayerIndex pausingPlayer)
        {
            this.pausingPlayer = pausingPlayer;
            currentState = prevState = GamePad.GetState(pausingPlayer);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            backgroundTexture = content.Load<Texture2D>(@"W_A_D\Textures\menus\paused_menu_bkg");
            controlsInfoTexture = content.Load<Texture2D>(@"W_A_D\Textures\menus\controls_screen");
            hexIcon = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\button_a");
            white16x16Texture = content.Load<Texture2D>(@"W_A_D\Textures\menus\white_16x16");
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

            if (GamePad.GetState(pausingPlayer).Buttons.Back == ButtonState.Pressed)
            {
                spriteBatch.Draw(controlsInfoTexture, new Rectangle(0, 0, this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height), Color.White);
            }
            else
            {
                spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height), Color.White);
                Color reddish = new Color(200, 55, 50);
                spriteBatch.DrawString(spriteFont, "Return to Game", new Vector2(RETURN_TO_GAME_POSITION.X + 54, RETURN_TO_GAME_POSITION.Y + 14), Color.Black);
                spriteBatch.DrawString(spriteFont, "Return to Game", new Vector2(RETURN_TO_GAME_POSITION.X + 50, RETURN_TO_GAME_POSITION.Y + 10), reddish);
                spriteBatch.DrawString(spriteFont, "Quit Game", new Vector2(QUIT_GAME_POSITION.X + 54, QUIT_GAME_POSITION.Y + 14), Color.Black);
                spriteBatch.DrawString(spriteFont, "Quit Game", new Vector2(QUIT_GAME_POSITION.X + 50, QUIT_GAME_POSITION.Y + 10), reddish);

                spriteBatch.DrawString(spriteFont, "** Hold Back To Show Controls Info **", new Vector2(CONTROLS_INFO_POSITION.X + 4, CONTROLS_INFO_POSITION.Y + 4), Color.Black);
                spriteBatch.DrawString(spriteFont, "** Hold Back To Show Controls Info **", new Vector2(CONTROLS_INFO_POSITION.X, CONTROLS_INFO_POSITION.Y), Color.DarkGray);

                switch (currentOption)
                {
                    case PauseMenuOption.RETURN_TO_GAME:
                        spriteBatch.Draw(hexIcon, RETURN_TO_GAME_POSITION, Color.White);
                        break;
                    case PauseMenuOption.QUIT:
                        spriteBatch.Draw(hexIcon, QUIT_GAME_POSITION, Color.White);
                        break;
                }
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (parentSystem.CurrentMenu != this) return;
            forcedInputWaitTime += gameTime.ElapsedGameTime.TotalSeconds;  // forced delay in gamepad input

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

            if (details.Button == GamePadWrapper.ButtonId.A)
            {
                GameAudio.PlayCue("back");
                if (currentOption == PauseMenuOption.RETURN_TO_GAME)
                    parentSystem.HidePauseMenu();
                else if (currentOption == PauseMenuOption.QUIT)
                    parentSystem.RequestEndSession();
            }

            if (details.Button == GamePadWrapper.ButtonId.D_UP)
            {
                GameAudio.PlayCue("dink");
                // move up an item
                currentOption--;
                if (currentOption < PauseMenuOption.RETURN_TO_GAME) currentOption = PauseMenuOption.QUIT;
            }

            if (details.Button == GamePadWrapper.ButtonId.D_DOWN)
            {
                GameAudio.PlayCue("dink");
                // move down an item
                currentOption++;
                if (currentOption > PauseMenuOption.QUIT) currentOption = PauseMenuOption.RETURN_TO_GAME;
            }
        }

        /// <summary>
        /// 
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
                if (details.StickValue.Y > 0.01)
                {
                    GameAudio.PlayCue("dink");
                    // move up an item
                    currentOption--;
                    if (currentOption < PauseMenuOption.RETURN_TO_GAME) currentOption = PauseMenuOption.QUIT;
                }
                else if (details.StickValue.Y < -0.01)
                {
                    GameAudio.PlayCue("dink");
                    // move down an item
                    currentOption++;
                    if (currentOption > PauseMenuOption.QUIT) currentOption = PauseMenuOption.RETURN_TO_GAME;
                }
            }
        }
    }
}
