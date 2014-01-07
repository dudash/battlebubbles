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
// File Created: 31 July 2008, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using HBBB.Core;
using HBBB.Core.Input;
using HBBB.Core.Menus;
using HBBB.GameComponents.Globals;
using HBBB.GameComponents.PlayerComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace HBBB.GameComponents.Menus
{
    /// <summary>
    /// A menu that is displayed when a game has ended in a tie.
    /// Allows for tied player to go head to head for the win.
    /// </summary>
    class SuddenDeathMenu : BaseMenu
    {
        private static string MENU_ID = "SUDDEN_DEATH_MENU";
        public static string MenuId { get { return MENU_ID; } }

        double timeTillClick = 8.0;
        bool goFlag = false;
        Player winner = null;
        public Player Winner { get { return winner; } }

        List<Player> players;
        List<PlayerIndex> badGuesses;

        Texture2D blackTexture;
        Texture2D whiteTexture;
        Texture2D backgroundTexture;
        Texture2D backButtonTexture;
        Texture2D deathButtonTexture;

        GamePadWrapper.ButtonId deathButton;
        private System.Random baseRandom;

        /// <summary>
        /// Construct the OptionsMenu
        /// </summary>
        /// <param name="parentSystem"></param>
        public SuddenDeathMenu(Game game, BaseMenuSystem parentSystem)
            : base(game, parentSystem)
        {
            baseRandom = new System.Random(System.Environment.TickCount);
            badGuesses = new List<PlayerIndex>();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void LoadContent()
        {
            blackTexture = content.Load<Texture2D>(@"W_A_D\Textures\black");
            whiteTexture = content.Load<Texture2D>(@"W_A_D\Textures\white");
            backgroundTexture = content.Load<Texture2D>(@"W_A_D\Textures\menus\suddendeath_menu");
            backButtonTexture = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\button_back");
            base.LoadContent();
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
        /// grab a random button
        /// </summary>
        /// <returns></returns>
        private void SetRandomButton()
        {
            deathButton = (GamePadWrapper.ButtonId) baseRandom.Next(0, 15);

            switch (deathButton)
            {
                case GamePadWrapper.ButtonId.A:
                    deathButtonTexture = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\button_a");
                    break;
                case GamePadWrapper.ButtonId.B:
                    deathButtonTexture = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\button_b");
                    break;
                case GamePadWrapper.ButtonId.BACK:
                    deathButtonTexture = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\button_back");
                    break;
                case GamePadWrapper.ButtonId.D_DOWN:
                    deathButtonTexture = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\dpad_down");
                    break;
                case GamePadWrapper.ButtonId.D_LEFT:
                    deathButtonTexture = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\dpad_left");
                    break;
                case GamePadWrapper.ButtonId.D_RIGHT:
                    deathButtonTexture = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\dpad_right");
                    break;
                case GamePadWrapper.ButtonId.D_UP:
                    deathButtonTexture = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\dpad_up");
                    break;
                case GamePadWrapper.ButtonId.LEFT_SHOULDER:
                    deathButtonTexture = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\bumper_left");
                    break;
                case GamePadWrapper.ButtonId.LEFT_STICK:
                    deathButtonTexture = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\stick_left");
                    break;
                case GamePadWrapper.ButtonId.RIGHT_SHOULDER:
                    deathButtonTexture = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\bumper_right");
                    break;
                case GamePadWrapper.ButtonId.RIGHT_STICK:
                    deathButtonTexture = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\stick_right");
                    break;
                case GamePadWrapper.ButtonId.START:
                    deathButtonTexture = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\button_start");
                    break;
                case GamePadWrapper.ButtonId.X:
                    deathButtonTexture = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\button_x");
                    break;
                case GamePadWrapper.ButtonId.Y:
                    deathButtonTexture = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\button_y");
                    break;
                default:
                    deathButtonTexture = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\button_y");
                    deathButton = GamePadWrapper.ButtonId.Y;
                    break;
            }
        }

        /// <summary>
        /// setup for the sudden death click
        /// </summary>
        public void SetupDeathClick(List<Player> players)
        {
            SetRandomButton();
            goFlag = false;
            timeTillClick = 7.0;
            winner = null;
            this.players = players;
            badGuesses.Clear();
        }

        /// <summary>
        /// setup for a death click with all attached player controllers
        /// </summary>
        public void SetupDeathClick(Player p1, Player p2, Player p3, Player p4)
        {
            SetRandomButton();
            goFlag = false;
            timeTillClick = 7.0;
            winner = null;
            if (players != null) players.Clear();
            else players = new List<Player>();
            if (p1 != null) players.Add(p1);
            if (p2 != null) players.Add(p2);
            if (p3 != null) players.Add(p3);
            if (p4 != null) players.Add(p4);
            badGuesses.Clear();
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
            string countDownText = "Get Ready To Click!";
            if (timeTillClick < 5) countDownText = "5...";
            if (timeTillClick < 4) countDownText = "5...4...";
            if (timeTillClick < 3) countDownText = "5...4...3...";
            if (timeTillClick < 2) countDownText = "5...4...3...2...";
            if (timeTillClick < 1) countDownText = "5...4...3...2...1...";
            if (timeTillClick < 0) countDownText = "Click! Click! Click!";

            // draw winner
            if (winner != null) countDownText = "Player " + winner.PlayerIndex.ToString() + " Wins The Death Challenge!";
            if (timeTillClick < -8.0) countDownText = "Stuck?  Going back to menus...";

            spriteBatch.DrawString(spriteFont, countDownText, new Vector2(this.Game.GraphicsDevice.Viewport.Width / 3 + 3, this.Game.GraphicsDevice.Viewport.Height / 4 + 3), Color.Black);
            spriteBatch.DrawString(spriteFont, countDownText, new Vector2(this.Game.GraphicsDevice.Viewport.Width / 3, this.Game.GraphicsDevice.Viewport.Height / 4), Color.White);

            spriteBatch.Draw(whiteTexture, new Rectangle(this.GraphicsDevice.Viewport.Width / 2 - 102, this.GraphicsDevice.Viewport.Height / 2 - 102, 204, 204), Color.White);
            spriteBatch.Draw(blackTexture, new Rectangle(this.GraphicsDevice.Viewport.Width / 2 - 100, this.GraphicsDevice.Viewport.Height / 2 - 100, 200, 200), Color.White);

            if (timeTillClick < 0)
            {
                spriteBatch.Draw(deathButtonTexture, new Rectangle(this.GraphicsDevice.Viewport.Width/2 - deathButtonTexture.Width/2, this.GraphicsDevice.Viewport.Height/2 - deathButtonTexture.Height/2, deathButtonTexture.Width, deathButtonTexture.Height), Color.White);
            }

            // draw player vs player vs player
            int xOffset = 0;
            foreach (Player p in players)
            {
                if (p == null) continue;
                xOffset += (this.GraphicsDevice.Viewport.Width / (players.Count+1));

                if (winner != null && p.PlayerIndex == winner.PlayerIndex)
                {
                    spriteBatch.Draw(p.PlayerPic, new Rectangle(xOffset, this.GraphicsDevice.Viewport.Height / 2 + 225, p.PlayerPic.Width, p.PlayerPic.Height), Color.Green);
                }
                else if (badGuesses.Contains(p.PlayerIndex))
                {
                    spriteBatch.Draw(p.PlayerPic, new Rectangle(xOffset, this.GraphicsDevice.Viewport.Height / 2 + 225, p.PlayerPic.Width, p.PlayerPic.Height), Color.Red);
                }
                else
                {
                    spriteBatch.Draw(p.PlayerPic, new Rectangle(xOffset, this.GraphicsDevice.Viewport.Height / 2 + 225, p.PlayerPic.Width, p.PlayerPic.Height), Color.White);
                }
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

            timeTillClick -= gameTime.ElapsedGameTime.TotalSeconds;
            if (timeTillClick <= 0.0) goFlag = true;
            
            if (goFlag)
            {
                // TODO CPU guesses?

                // TODO if all players guess incorrectly restart sudden death

                // TODO if sudden death last longer than 10 after go quit to main menu
                if (timeTillClick < -10.0) parentSystem.TransitionToMenu(MainMenu.MenuId);

                // if winner wait 3 secs and the goto gameover menu
                if (winner != null && timeTillClick < 0.0) parentSystem.TransitionToMenu(GameOverMenu.MenuId);
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
            if (!goFlag) return;
            if (winner != null) return;

            // if index guessed wrong return
            if (badGuesses.Contains(index)) return;

            // check click and set winner
            if (details.Button == deathButton)
            {
                foreach (Player p in players)
                {
                    if (p.PlayerIndex == index)
                    {
                        winner = p;
                        timeTillClick = 3.0; // time to wait until transitioning to the gameover menu

                        // TODO play win sfx and animations

                        return;
                    }
                }
            }
            else
            {
                // mark this player as an incorrect guess
                badGuesses.Add(index);
                Debug.WriteLine(details.Button.ToString() + "!=" + deathButton.ToString());
            }
        }
    }
}
