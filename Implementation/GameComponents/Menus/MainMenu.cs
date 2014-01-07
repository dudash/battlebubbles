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
// File Created: 06 July 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System.Collections.Generic;
using HBBB.Core;
using HBBB.Core.Menus;
using HBBB.GameComponents.Globals;
using HBBB.Core.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;

namespace HBBB.GameComponents.Menus
{
    /// <summary>
    /// The main pre-game menu allows the sarting of a new game, options config, and exit
    /// </summary>
    class MainMenu : BaseMenu
    {
        private static string MENU_ID = "MAIN_MENU";
        public static string MenuId { get { return MENU_ID; } }

        enum MainMenuOption { NEW_GAME, INSTRUCTIONS, CREDITS, BUY_NOW_OR_LEVEL_BUILDER, QUIT }
        MainMenuOption currentOption = MainMenuOption.NEW_GAME;
        double forcedInputWaitTime = 0.0;
        const double FORCED_INPUT_DELAY = 0.2;

        Texture2D backgroundTexture;
        Texture2D hexIcon;
        Rectangle NEW_GAME_POSITION = new Rectangle(300, 330, 50, 50);
        Rectangle INSTRUCTIONS_POSITION = new Rectangle(300, 380, 50, 50);
        Rectangle CREDITS_POSITION = new Rectangle(300, 430, 50, 50);
        Rectangle BUY_NOW_LEVEL_BUILDER_POSITION = new Rectangle(300, 480, 50, 50);
        Rectangle QUIT_POSITION = new Rectangle(300, 530, 50, 50);

        /// <summary>
        /// Construct the main menu
        /// </summary>
        /// <param name="parentSystem"></param>
        public MainMenu(Game game, BaseMenuSystem parentSystem)
            : base(game, parentSystem)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            backgroundTexture = content.Load<Texture2D>(@"W_A_D\Textures\menus\main_menu_bkg");
            hexIcon = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\button_a");
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
            spriteBatch.Draw(backgroundTexture, new Rectangle(0,0,this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height), Color.White);

            Color reddish = new Color(200, 55, 50);
            spriteBatch.DrawString(spriteFont, "New Game", new Vector2(NEW_GAME_POSITION.X + 54, NEW_GAME_POSITION.Y + 14), Color.Black);
            spriteBatch.DrawString(spriteFont, "New Game", new Vector2(NEW_GAME_POSITION.X + 50, NEW_GAME_POSITION.Y + 10), reddish);
            spriteBatch.DrawString(spriteFont, "How to Play", new Vector2(INSTRUCTIONS_POSITION.X + 54, INSTRUCTIONS_POSITION.Y + 14), Color.Black);
            spriteBatch.DrawString(spriteFont, "How to Play", new Vector2(INSTRUCTIONS_POSITION.X + 50, INSTRUCTIONS_POSITION.Y + 10), reddish);
            spriteBatch.DrawString(spriteFont, "Credits", new Vector2(CREDITS_POSITION.X + 54, CREDITS_POSITION.Y + 14), Color.Black);
            spriteBatch.DrawString(spriteFont, "Credits", new Vector2(CREDITS_POSITION.X + 50, CREDITS_POSITION.Y + 10), reddish);
            spriteBatch.DrawString(spriteFont, "Quit", new Vector2(QUIT_POSITION.X + 54, QUIT_POSITION.Y + 14), Color.Black);
            spriteBatch.DrawString(spriteFont, "Quit", new Vector2(QUIT_POSITION.X + 50, QUIT_POSITION.Y + 10), reddish);
            if (Guide.IsTrialMode)
            {
                spriteBatch.DrawString(spriteFont, "Buy Full Game", new Vector2(BUY_NOW_LEVEL_BUILDER_POSITION.X + 54, BUY_NOW_LEVEL_BUILDER_POSITION.Y + 14), Color.Black);
                spriteBatch.DrawString(spriteFont, "Buy Full Game", new Vector2(BUY_NOW_LEVEL_BUILDER_POSITION.X + 50, BUY_NOW_LEVEL_BUILDER_POSITION.Y + 10), reddish);
            }
            else
            {
                spriteBatch.DrawString(spriteFont, "Level Builder", new Vector2(BUY_NOW_LEVEL_BUILDER_POSITION.X + 54, BUY_NOW_LEVEL_BUILDER_POSITION.Y + 14), Color.Black);
                spriteBatch.DrawString(spriteFont, "Level Builder", new Vector2(BUY_NOW_LEVEL_BUILDER_POSITION.X + 50, BUY_NOW_LEVEL_BUILDER_POSITION.Y + 10), reddish);
            }

            switch (currentOption)
            {
                case MainMenuOption.NEW_GAME:
                    spriteBatch.Draw(hexIcon, NEW_GAME_POSITION, Color.White);
                    break;
                case MainMenuOption.INSTRUCTIONS:
                    spriteBatch.Draw(hexIcon, INSTRUCTIONS_POSITION, Color.White);
                    break;
                case MainMenuOption.CREDITS:
                    spriteBatch.Draw(hexIcon, CREDITS_POSITION, Color.White);
                    break;
                case MainMenuOption.QUIT:
                    spriteBatch.Draw(hexIcon, QUIT_POSITION, Color.White);
                    break;
                case MainMenuOption.BUY_NOW_OR_LEVEL_BUILDER:
                    spriteBatch.Draw(hexIcon, BUY_NOW_LEVEL_BUILDER_POSITION, Color.White);
                    break;
            }
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
            forcedInputWaitTime += gameTime.ElapsedGameTime.TotalSeconds;  // forced delay in gamepad input
            
            base.Update(gameTime);
        }

        /// <summary>
        /// When a button is clicked
        /// </summary>
        /// <param name="details"></param>
        public override void OnButtonClick(PlayerIndex index, GamePadButtonEventDetails details)
        {
            if (parentSystem.CurrentMenu != this) return;

            if (details.Button == GamePadWrapper.ButtonId.A ||
                details.Button == GamePadWrapper.ButtonId.START)
            {
                GameAudio.PlayCue("click");
                if (currentOption == MainMenuOption.NEW_GAME)
                    parentSystem.TransitionToMenu(JoinUpMenu.MenuId);
                else if (currentOption == MainMenuOption.INSTRUCTIONS)
                    parentSystem.TransitionToMenu(OptionsMenu.MenuId);
                else if (currentOption == MainMenuOption.CREDITS)
                    parentSystem.TransitionToMenu(CreditsMenu.MenuId);
                else if (currentOption == MainMenuOption.QUIT)
                    parentSystem.TransitionToMenu(QuitMenu.MenuId);
                else if (currentOption == MainMenuOption.BUY_NOW_OR_LEVEL_BUILDER && Guide.IsTrialMode)
                {
                    Guide.ShowMarketplace(index);
                    currentOption = MainMenuOption.NEW_GAME;
                }
                else if (currentOption == MainMenuOption.BUY_NOW_OR_LEVEL_BUILDER && !Guide.IsTrialMode)
                {
                    parentSystem.TransitionToMenu(LevelBuilder.CustomLevelsMenu.MenuId);
                }
            }
            else if (details.Button == GamePadWrapper.ButtonId.D_UP)
            {
                GameAudio.PlayCue("dink");
                // move up an item
                currentOption--;
                if (currentOption < MainMenuOption.NEW_GAME) currentOption = MainMenuOption.QUIT;
            }
            else if (details.Button == GamePadWrapper.ButtonId.D_DOWN)
            {
                GameAudio.PlayCue("dink");
                // move down an item
                currentOption++;
                if (currentOption > MainMenuOption.QUIT) currentOption = MainMenuOption.NEW_GAME;
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
                if (details.StickValue.Y > 0.0)
                {
                    GameAudio.PlayCue("dink");
                    // move up an item
                    currentOption--;
                    if (currentOption < MainMenuOption.NEW_GAME) currentOption = MainMenuOption.QUIT;
                }
                else if (details.StickValue.Y < -0.0)
                {
                    GameAudio.PlayCue("dink");
                    // move down an item
                    currentOption++;
                    if (currentOption > MainMenuOption.QUIT) currentOption = MainMenuOption.NEW_GAME;
                }
            }
        }
    }
}
