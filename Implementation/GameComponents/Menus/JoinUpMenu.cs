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
// File Created: 21 August 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using HBBB.Core;
using HBBB.Core.Graphics;
using HBBB.Core.Input;
using HBBB.Core.Menus;
using HBBB.GameComponents.Globals;
using HBBB.GameComponents.PlayerComponents;

namespace HBBB.GameComponents.Menus
{
    /// <summary>
    /// This is a complex menu that allows multiple players to join the game.
    /// It also allows the configuration of players colors and addition of CPU players
    /// </summary>
    class JoinUpMenu : BaseMenu
    {
        private static string MENU_ID = "JOIN_UP_MENU";
        public static string MenuId { get { return MENU_ID; } }

        enum JoinUpMenuOptions { PRI_COLORS, SEC_COLORS, DIFFICULTY }
        JoinUpMenuOptions p1MenuSelection = JoinUpMenuOptions.PRI_COLORS;
        JoinUpMenuOptions p2MenuSelection = JoinUpMenuOptions.PRI_COLORS;
        JoinUpMenuOptions p3MenuSelection = JoinUpMenuOptions.PRI_COLORS;
        JoinUpMenuOptions p4MenuSelection = JoinUpMenuOptions.PRI_COLORS;

        double forcedInputWaitTime = 0.0;
        const double FORCED_INPUT_DELAY = 0.25;

        bool warnNoPlayersJoined = false;

        Texture2D stickTexture;
        Texture2D selectBarTexture;
        Texture2D backgroundTexture;
        Texture2D aToJoinTexture;
        Texture2D bToExitTexture;
        Texture2D yToAiButtonTexture;
        Texture2D startToStartTexture;
        Texture2D whiteTexture;
        Texture2D blackTexture;
        const int CHOOSE_COLORS_X_POSITION = 145;
        const int CHOOSE_COLORS_Y_POSITION = 10;
        const int JOIN_X_POSITION = 170;
        const int JOIN_Y_POSITION = 145;
        const int LEAVE_X_POSITION = 170;
        const int LEAVE_Y_POSITION = 300;
        const int PRICOLOR_X_POSITION = 155;
        const int PRICOLOR_Y_POSITION = 75;
        const int SECCOLOR_X_POSITION = 155;
        const int SECCOLOR_Y_POSITION = 150;
        const int PRICOLORBOX_X_POSITION = PRICOLOR_X_POSITION + 95;
        const int PRICOLORBOX_Y_POSITION = PRICOLOR_Y_POSITION + 25;
        const int SECCOLORBOX_X_POSITION = SECCOLOR_X_POSITION + 95;
        const int SECCOLORBOX_Y_POSITION = SECCOLOR_Y_POSITION + 25;
        const int DIFFICULTY_X_POSITION = 180;
        const int DIFFICULTY_Y_POSITION = 240;

        public Player Player1
        {
            get
            { 
                if (allPlayers.ContainsKey(PlayerIndex.One)) return allPlayers[PlayerIndex.One];
                else return null;
            }
        }
        const int PLAYER_1_X_OFFSET = 0;
        const int PLAYER_1_Y_OFFSET = 0;
        public Player Player2
        {
            get
            { 
                if (allPlayers.ContainsKey(PlayerIndex.Two)) return allPlayers[PlayerIndex.Two];
                else return null;
            }
        }
        const int PLAYER_2_X_OFFSET = 620;
        const int PLAYER_2_Y_OFFSET = 0;
        public Player Player3
        {
            get
            { 
                if (allPlayers.ContainsKey(PlayerIndex.Three)) return allPlayers[PlayerIndex.Three];
                else return null;
            }
        }
        const int PLAYER_3_X_OFFSET = 0;
        const int PLAYER_3_Y_OFFSET = 400;
        public Player Player4
        {
            get
            {
                if (allPlayers.ContainsKey(PlayerIndex.Four)) return allPlayers[PlayerIndex.Four];
                else return null;
            }
        }
        const int PLAYER_4_X_OFFSET = 620;
        const int PLAYER_4_Y_OFFSET = 400;
        Dictionary<PlayerIndex, Player> allPlayers = new Dictionary<PlayerIndex, Player>();
        public Dictionary<PlayerIndex, Player> AllPlayers { get { return allPlayers; } }

        /// <summary>
        /// Construct the JoinUpMenu
        /// </summary>
        /// <param name="parentSystem"></param>
        public JoinUpMenu(Game game, BaseMenuSystem parentSystem)
            : base(game, parentSystem)
        {
        }

        /// <summary>
        /// Reset the players
        /// </summary>
        public void ResetPlayers()
        {
            allPlayers.Clear();
        }

        /// <summary>
        /// Set the player associtated with the arg index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public void SetPlayer(PlayerIndex index, Player player)
        {
            if (!allPlayers.ContainsKey(index))
            {
                allPlayers.Add(index, player);
            }
            else
            {
                allPlayers[index] = player;
            }
        }

        /// <summary>
        /// Return the player associtated with the arg index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Player GetPlayer(PlayerIndex index)
        {
            if (allPlayers.ContainsKey(index))
            {
                return allPlayers[index];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            backgroundTexture = content.Load<Texture2D>(@"W_A_D\Textures\menus\joinup_menu_bkg");
            aToJoinTexture = content.Load<Texture2D>(@"W_A_D\Textures\menus\press_a_to_join");
            bToExitTexture = content.Load<Texture2D>(@"W_A_D\Textures\menus\press_b_to_exit");
            startToStartTexture = content.Load<Texture2D>(@"W_A_D\Textures\menus\press_to_start");
            whiteTexture = content.Load<Texture2D>(@"W_A_D\Textures\menus\white_16x16");
            blackTexture = content.Load<Texture2D>(@"W_A_D\Textures\black");
            yToAiButtonTexture = content.Load<Texture2D>(@"W_A_D\Textures\menus\press_y_to_AI");
            stickTexture = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\stick_left");
            selectBarTexture = content.Load<Texture2D>(@"W_A_D\Textures\menus\select_bar");
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
        /// Draw this screen
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (parentSystem.CurrentMenu != this) return;

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height), Color.White);
            spriteBatch.End();

            // Draw the player information

            if (GamePad.GetState(PlayerIndex.One).IsConnected) DrawPlayer(PlayerIndex.One);
            if (GamePad.GetState(PlayerIndex.Two).IsConnected) DrawPlayer(PlayerIndex.Two);
            if (GamePad.GetState(PlayerIndex.Three).IsConnected) DrawPlayer(PlayerIndex.Three);
            if (GamePad.GetState(PlayerIndex.Four).IsConnected) DrawPlayer(PlayerIndex.Four);

            if (warnNoPlayersJoined)
            {
                // draw warning
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
                spriteBatch.DrawString(spriteFont, "No Players Joined, Press start to watch the CPU", new Vector2(this.GraphicsDevice.Viewport.Width / 5, this.GraphicsDevice.Viewport.Height/3), Color.Black);
                spriteBatch.DrawString(spriteFont, "No Players Joined, Press start to watch the CPU", new Vector2(this.GraphicsDevice.Viewport.Width / 5 + 4, this.GraphicsDevice.Viewport.Height/3+4), Color.White);
                spriteBatch.End();
            }
            if (!(GamePad.GetState(PlayerIndex.One).IsConnected || GamePad.GetState(PlayerIndex.Two).IsConnected || GamePad.GetState(PlayerIndex.Three).IsConnected || GamePad.GetState(PlayerIndex.Four).IsConnected))
            {
                // draw warning
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
                spriteBatch.DrawString(spriteFont, "No controllers connected, at least one is required", new Vector2(this.GraphicsDevice.Viewport.Width / 5, this.GraphicsDevice.Viewport.Height / 3), Color.Black);
                spriteBatch.DrawString(spriteFont, "No controllers connected, at least one is required", new Vector2(this.GraphicsDevice.Viewport.Width / 5 + 4, this.GraphicsDevice.Viewport.Height / 3 + 4), Color.White);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }

        /// <summary>
        /// Update this screen, process input, update vars, etc
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

            // handle special case where no players have joined
            if (warnNoPlayersJoined)
            {
                warnNoPlayersJoined = false;

                // START THE GAME with no players
                if (details.Button == GamePadWrapper.ButtonId.START)  // start game
                {
                    GameAudio.PlayCue("click");
                    parentSystem.RequestCreatePlayers();
                    parentSystem.TransitionToMenu(LevelSelectionMenu.MenuId);
                    return;
                }
                // player canceled and wants to join up
                return;
            }

            Player curPlayer = null;
            if (allPlayers.ContainsKey(index)) curPlayer = allPlayers[index];

            // JOIN A PLAYER
            if (curPlayer == null)
            {
                if (details.Button == GamePadWrapper.ButtonId.A)
                {
                    GameAudio.PlayCue("click");
                    Player newPlayer = new Player(index);
                    SetPlayer(index, newPlayer);
                    return;
                }
            }
            else
            {
                if (details.Button == GamePadWrapper.ButtonId.B)
                {
                    GameAudio.PlayCue("click");
                    SetPlayer(index, null);
                    return;
                }
            }
            
            // RETURN TO MAIN MENU
            if (details.Button == GamePadWrapper.ButtonId.BACK ||
                details.Button == GamePadWrapper.ButtonId.B)
            {
                GameAudio.PlayCue("back");
                parentSystem.TransitionToMenu(MainMenu.MenuId);
                return;
            }

            // ADVANCED JOIN MENU
            if (details.Button == GamePadWrapper.ButtonId.Y)
            {
                GameAudio.PlayCue("click");
                parentSystem.TransitionToMenu(JoinUpAdvancedMenu.MenuId);
                return;
            }

            // START THE GAME
            if (details.Button == GamePadWrapper.ButtonId.START ||
                details.Button == GamePadWrapper.ButtonId.A)  // start game
            {
                GameAudio.PlayCue("click");
                if (Player1 == null && Player2 == null && Player3 == null && Player4 == null)
                {
                    warnNoPlayersJoined = true;  // turn warning on
                    return;
                }
                parentSystem.RequestCreatePlayers();
                parentSystem.TransitionToMenu(LevelSelectionMenu.MenuId);
                return;
            }
            
            if (curPlayer != null)
            {
                // CHANGE PRIMARY COLOR
                if (details.Button == GamePadWrapper.ButtonId.D_UP)
                {
                    PreviousMenuOption(index);
                    GameAudio.PlayCue("click");
                }
                else if (details.Button == GamePadWrapper.ButtonId.D_DOWN)
                {
                    NextMenuOption(index);
                    GameAudio.PlayCue("click");
                }
                else if (details.Button == GamePadWrapper.ButtonId.D_LEFT)
                {
                    SelectPreviousItem(index, curPlayer);
                    GameAudio.PlayCue("click");
                }
                else if (details.Button == GamePadWrapper.ButtonId.D_RIGHT)
                {
                    SelectNextItem(index, curPlayer);
                    GameAudio.PlayCue("click");
                }
            }
        }

        /// <summary>
        /// Handle analog buttons
        /// </summary>
        /// <param name="index"></param>
        /// <param name="details"></param>
        public override void OnAnalogMovement(PlayerIndex index, GamePadAnalogEventDetails details)
        {
            if (parentSystem.CurrentMenu != this) return;

            if (forcedInputWaitTime < FORCED_INPUT_DELAY) return;
            else forcedInputWaitTime = 0.0;

            Player curPlayer = null;
            if (allPlayers.ContainsKey(index)) curPlayer = allPlayers[index];
            else return;

            if (details.AnalogButton == GamePadWrapper.AnalogId.LEFT_STICK)
            {
                if (details.StickValue.X > 0.1f)
                {
                    SelectNextItem(index, curPlayer);
                    GameAudio.PlayCue("click");
                }
                else if (details.StickValue.X < -0.1f)
                {
                    SelectPreviousItem(index, curPlayer);
                    GameAudio.PlayCue("click");
                }
                if (details.StickValue.Y > 0.1f)
                {
                    PreviousMenuOption(index);
                    GameAudio.PlayCue("click");
                }
                else if (details.StickValue.Y < -0.1f)
                {
                    NextMenuOption(index);
                    GameAudio.PlayCue("click");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void PreviousMenuOption(PlayerIndex index)
        {
            if (index == PlayerIndex.One)
            {
                p1MenuSelection--;
                if (p1MenuSelection < JoinUpMenuOptions.PRI_COLORS) p1MenuSelection = JoinUpMenuOptions.DIFFICULTY;
                if (p1MenuSelection > JoinUpMenuOptions.DIFFICULTY) p1MenuSelection = JoinUpMenuOptions.PRI_COLORS;
            }
            else if (index == PlayerIndex.Two)
            {
                p2MenuSelection--;
                if (p2MenuSelection < JoinUpMenuOptions.PRI_COLORS) p2MenuSelection = JoinUpMenuOptions.DIFFICULTY;
                if (p2MenuSelection > JoinUpMenuOptions.DIFFICULTY) p2MenuSelection = JoinUpMenuOptions.PRI_COLORS;
            }
            else if (index == PlayerIndex.Three)
            {
                p3MenuSelection--;
                if (p3MenuSelection < JoinUpMenuOptions.PRI_COLORS) p3MenuSelection = JoinUpMenuOptions.DIFFICULTY;
                if (p3MenuSelection > JoinUpMenuOptions.DIFFICULTY) p3MenuSelection = JoinUpMenuOptions.PRI_COLORS;
            }
            else if (index == PlayerIndex.Four)
            {
                p4MenuSelection--;
                if (p4MenuSelection < JoinUpMenuOptions.PRI_COLORS) p4MenuSelection = JoinUpMenuOptions.DIFFICULTY;
                if (p4MenuSelection > JoinUpMenuOptions.DIFFICULTY) p4MenuSelection = JoinUpMenuOptions.PRI_COLORS;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void NextMenuOption(PlayerIndex index)
        {
            if (index == PlayerIndex.One)
            {
                p1MenuSelection++;
                if (p1MenuSelection < JoinUpMenuOptions.PRI_COLORS) p1MenuSelection = JoinUpMenuOptions.DIFFICULTY;
                if (p1MenuSelection > JoinUpMenuOptions.DIFFICULTY) p1MenuSelection = JoinUpMenuOptions.PRI_COLORS;
            }
            else if (index == PlayerIndex.Two)
            {
                p2MenuSelection++;
                if (p2MenuSelection < JoinUpMenuOptions.PRI_COLORS) p2MenuSelection = JoinUpMenuOptions.DIFFICULTY;
                if (p2MenuSelection > JoinUpMenuOptions.DIFFICULTY) p2MenuSelection = JoinUpMenuOptions.PRI_COLORS;
            }
            else if (index == PlayerIndex.Three)
            {
                p3MenuSelection++;
                if (p3MenuSelection < JoinUpMenuOptions.PRI_COLORS) p3MenuSelection = JoinUpMenuOptions.DIFFICULTY;
                if (p3MenuSelection > JoinUpMenuOptions.DIFFICULTY) p3MenuSelection = JoinUpMenuOptions.PRI_COLORS;
            }
            else if (index == PlayerIndex.Four)
            {
                p4MenuSelection++;
                if (p4MenuSelection < JoinUpMenuOptions.PRI_COLORS) p4MenuSelection = JoinUpMenuOptions.DIFFICULTY;
                if (p4MenuSelection > JoinUpMenuOptions.DIFFICULTY) p4MenuSelection = JoinUpMenuOptions.PRI_COLORS;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        private void SelectNextItem(PlayerIndex index, Player curPlayer)
        {
            if (index == PlayerIndex.One && p1MenuSelection == JoinUpMenuOptions.DIFFICULTY ||
                index == PlayerIndex.Two && p2MenuSelection == JoinUpMenuOptions.DIFFICULTY ||
                index == PlayerIndex.Three && p3MenuSelection == JoinUpMenuOptions.DIFFICULTY ||
                index == PlayerIndex.Four && p4MenuSelection == JoinUpMenuOptions.DIFFICULTY)
            {
                curPlayer.NextDifficultyLevel();
            }
            else if (index == PlayerIndex.One && p1MenuSelection == JoinUpMenuOptions.PRI_COLORS ||
                index == PlayerIndex.Two && p2MenuSelection == JoinUpMenuOptions.PRI_COLORS ||
                index == PlayerIndex.Three && p3MenuSelection == JoinUpMenuOptions.PRI_COLORS ||
                index == PlayerIndex.Four && p4MenuSelection == JoinUpMenuOptions.PRI_COLORS)
            {
                curPlayer.PrimaryColor = ColorSetHelper.NextColor(curPlayer.PrimaryColor);
            }
            else if (index == PlayerIndex.One && p1MenuSelection == JoinUpMenuOptions.SEC_COLORS ||
                index == PlayerIndex.Two && p2MenuSelection == JoinUpMenuOptions.SEC_COLORS ||
                index == PlayerIndex.Three && p3MenuSelection == JoinUpMenuOptions.SEC_COLORS ||
                index == PlayerIndex.Four && p4MenuSelection == JoinUpMenuOptions.SEC_COLORS)
            {
                curPlayer.SecondaryColor = ColorSetHelper.NextColor(curPlayer.SecondaryColor);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        private void SelectPreviousItem(PlayerIndex index, Player curPlayer)
        {
            if (index == PlayerIndex.One && p1MenuSelection == JoinUpMenuOptions.DIFFICULTY ||
                 index == PlayerIndex.Two && p2MenuSelection == JoinUpMenuOptions.DIFFICULTY ||
                 index == PlayerIndex.Three && p3MenuSelection == JoinUpMenuOptions.DIFFICULTY ||
                 index == PlayerIndex.Four && p4MenuSelection == JoinUpMenuOptions.DIFFICULTY)
            {
                curPlayer.PreviousDifficultyLevel();
            }
            else if (index == PlayerIndex.One && p1MenuSelection == JoinUpMenuOptions.PRI_COLORS ||
                 index == PlayerIndex.Two && p2MenuSelection == JoinUpMenuOptions.PRI_COLORS ||
                 index == PlayerIndex.Three && p3MenuSelection == JoinUpMenuOptions.PRI_COLORS ||
                 index == PlayerIndex.Four && p4MenuSelection == JoinUpMenuOptions.PRI_COLORS)
            {
                curPlayer.PrimaryColor = ColorSetHelper.PrevColor(curPlayer.PrimaryColor);
            }
            else if (index == PlayerIndex.One && p1MenuSelection == JoinUpMenuOptions.SEC_COLORS ||
                index == PlayerIndex.Two && p2MenuSelection == JoinUpMenuOptions.SEC_COLORS ||
                index == PlayerIndex.Three && p3MenuSelection == JoinUpMenuOptions.SEC_COLORS ||
                index == PlayerIndex.Four && p4MenuSelection == JoinUpMenuOptions.SEC_COLORS)
            {
                curPlayer.SecondaryColor = ColorSetHelper.PrevColor(curPlayer.SecondaryColor);
            }
        }

        /// <summary>
        /// Draw the player info into that player's spot on the screen
        /// </summary>
        /// <param name="index"></param>
        private void DrawPlayer(PlayerIndex index)
        {
            Player curPlayer = null;
            if (allPlayers.ContainsKey(index))
            {
                curPlayer = allPlayers[index];
            }

            int xOffset = 0;
            int yOffset = 0;
            JoinUpMenuOptions selectedIndex = p1MenuSelection;
            switch (index)
            {
                case PlayerIndex.One:
                    xOffset = PLAYER_1_X_OFFSET;
                    yOffset = PLAYER_1_Y_OFFSET;
                    selectedIndex = p1MenuSelection;
                    break;
                case PlayerIndex.Two:
                    xOffset = PLAYER_2_X_OFFSET;
                    yOffset = PLAYER_2_Y_OFFSET;
                    selectedIndex = p2MenuSelection;
                    break;
                case PlayerIndex.Three:
                    xOffset = PLAYER_3_X_OFFSET;
                    yOffset = PLAYER_3_Y_OFFSET;
                    selectedIndex = p3MenuSelection;
                    break;
                case PlayerIndex.Four:
                    xOffset = PLAYER_4_X_OFFSET;
                    yOffset = PLAYER_4_Y_OFFSET;
                    selectedIndex = p4MenuSelection;
                    break;
            }

            if (curPlayer == null) // player not joined
            {
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
                spriteBatch.Draw(aToJoinTexture, new Rectangle(JOIN_X_POSITION+xOffset, JOIN_Y_POSITION+yOffset, 375, 50), Color.White);
                spriteBatch.Draw(yToAiButtonTexture, new Rectangle(JOIN_X_POSITION+xOffset, JOIN_Y_POSITION+yOffset+75, 375, 50), Color.White);
                spriteBatch.End();
            }
            else  // player already joined
            {
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

                // draw choose colors text
                spriteBatch.DrawString(spriteFont, "Choose Your Colors", new Vector2(PRICOLOR_X_POSITION + xOffset + 3, PRICOLOR_Y_POSITION - 47 + yOffset), Color.Black);
                spriteBatch.DrawString(spriteFont, "Choose Your Colors", new Vector2(PRICOLOR_X_POSITION + xOffset, PRICOLOR_Y_POSITION - 50 + yOffset), Color.White);

                // draw currently selected item
                if (selectedIndex == JoinUpMenuOptions.PRI_COLORS)
                {
                    spriteBatch.Draw(stickTexture, new Rectangle(DIFFICULTY_X_POSITION - 100 + xOffset, PRICOLORBOX_Y_POSITION - 20 + yOffset, 50, 50), Color.White);
                    spriteBatch.Draw(selectBarTexture, new Rectangle(DIFFICULTY_X_POSITION - 30 + xOffset, PRICOLORBOX_Y_POSITION + yOffset + 8, 350, 10), Color.White);
                }
                else if (selectedIndex == JoinUpMenuOptions.SEC_COLORS)
                {
                    spriteBatch.Draw(stickTexture, new Rectangle(DIFFICULTY_X_POSITION - 100 + xOffset, SECCOLORBOX_Y_POSITION - 20 + yOffset, 50, 50), Color.White);
                    spriteBatch.Draw(selectBarTexture, new Rectangle(DIFFICULTY_X_POSITION - 30 + xOffset, SECCOLORBOX_Y_POSITION + yOffset + 8, 350, 10), Color.White);
                }
                else if (selectedIndex == JoinUpMenuOptions.DIFFICULTY)
                {
                    spriteBatch.Draw(stickTexture, new Rectangle(DIFFICULTY_X_POSITION - 100 + xOffset, DIFFICULTY_Y_POSITION + yOffset, 50, 50), Color.White);
                    spriteBatch.Draw(selectBarTexture, new Rectangle(DIFFICULTY_X_POSITION - 30 + xOffset, DIFFICULTY_Y_POSITION + 20 + yOffset, 350, 10), Color.White);
                }

                // draw the player controls
                string difficultyString = "Normal Difficulty";
                if (curPlayer.DifficultyLevel == Player.Difficulty.EASY) difficultyString = "Easy Difficulty";
                else if (curPlayer.DifficultyLevel == Player.Difficulty.HARD) difficultyString = "Hard Difficulty";
                spriteBatch.DrawString(spriteFont, difficultyString, new Vector2(DIFFICULTY_X_POSITION + xOffset + 3.0f, DIFFICULTY_Y_POSITION + yOffset + 3.0f), Color.Black);
                spriteBatch.DrawString(spriteFont, difficultyString, new Vector2(DIFFICULTY_X_POSITION + xOffset, DIFFICULTY_Y_POSITION + yOffset), Color.White);
                spriteBatch.Draw(bToExitTexture, new Rectangle(LEAVE_X_POSITION + xOffset, LEAVE_Y_POSITION + yOffset, 375, 50), Color.White);

                // draw player colors
                spriteBatch.Draw(blackTexture, new Rectangle(PRICOLORBOX_X_POSITION + xOffset - 2, PRICOLORBOX_Y_POSITION + yOffset - 2, 129, 29), curPlayer.PrimaryColor);
                spriteBatch.Draw(whiteTexture, new Rectangle(PRICOLORBOX_X_POSITION + xOffset, PRICOLORBOX_Y_POSITION + yOffset, 125, 25), curPlayer.PrimaryColor);
                spriteBatch.Draw(blackTexture, new Rectangle(SECCOLORBOX_X_POSITION + xOffset - 2, SECCOLORBOX_Y_POSITION + yOffset - 2, 129, 29), curPlayer.SecondaryColor);
                spriteBatch.Draw(whiteTexture, new Rectangle(SECCOLORBOX_X_POSITION + xOffset, SECCOLORBOX_Y_POSITION + yOffset, 125, 25), curPlayer.SecondaryColor);

                spriteBatch.End();
            }
        }
    }
}
