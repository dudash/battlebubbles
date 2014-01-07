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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using HBBB.Core;
using HBBB.Core.Menus;
using HBBB.Core.Input;
using HBBB.GameComponents.Globals;

namespace HBBB.GameComponents.Menus
{
    /// <summary>
    /// A menu that is displayed when a game has just ended.  This menu shows
    /// statistics and scores from the previous game
    /// </summary>
    class GameOverMenu : BaseMenu
    {
        private static string MENU_ID = "GAME_OVER_MENU";
        public static string MenuId { get { return MENU_ID; } }
        const double WAIT_TIME = 3.0;
        double waitTimeRemaining = WAIT_TIME;

        string gameOverText;
        public string GameOverText { set { gameOverText = value; } }
        
        public PlayerComponents.Player p1;
        public PlayerComponents.Player p2;
        public PlayerComponents.Player p3;
        public PlayerComponents.Player p4;

        public PlayerComponents.Player winner;

        Texture2D backgroundTexture;
        //Texture2D backButtonTexture;

        /// <summary>
        /// Construct the OptionsMenu
        /// </summary>
        /// <param name="parentSystem"></param>
        public GameOverMenu(Game game, BaseMenuSystem parentSystem)
            : base(game, parentSystem)
        {
            gameOverText = "";
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            backgroundTexture = content.Load<Texture2D>(@"W_A_D\Textures\menus\gameover_menu_bkg");
            //backButtonTexture = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\button_back");
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

            waitTimeRemaining -= gameTime.ElapsedGameTime.TotalSeconds;

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height), Color.White);
            /*
            spriteBatch.Draw(backButtonTexture,
                new Vector2(this.Game.GraphicsDevice.Viewport.Width / 2, this.Game.GraphicsDevice.Viewport.Height / 2 + 250),
                null, Color.White, 0,
                new Vector2(backButtonTexture.Width / 2, backButtonTexture.Height / 2),
                1.0f, SpriteEffects.None, 0.0f);
             */
            spriteBatch.DrawString(spriteFont, gameOverText, new Vector2(this.Game.GraphicsDevice.Viewport.Width / 5, this.Game.GraphicsDevice.Viewport.Height / 2 + 20), Color.Black);
            spriteBatch.DrawString(spriteFont, gameOverText, new Vector2(this.Game.GraphicsDevice.Viewport.Width / 5, this.Game.GraphicsDevice.Viewport.Height / 2 + 17), Color.White);

            // draw winning player avatar
            if (winner != null && winner.PlayerPic != null)
                spriteBatch.Draw(winner.PlayerPic, new Rectangle(this.GraphicsDevice.Viewport.Width/2 - winner.PlayerPic.Width, this.GraphicsDevice.Viewport.Height / 5, winner.PlayerPic.Width*2, winner.PlayerPic.Height*2), Color.White);

            spriteBatch.DrawString(spriteFont, "P1: " + p1.Statistics.Score, new Vector2(this.Game.GraphicsDevice.Viewport.Width / 2 - 50, this.Game.GraphicsDevice.Viewport.Height / 2 + 60), Color.Black);
            spriteBatch.DrawString(spriteFont, "P1: " + p1.Statistics.Score, new Vector2(this.Game.GraphicsDevice.Viewport.Width / 2 - 47, this.Game.GraphicsDevice.Viewport.Height / 2 + 57), p1.PrimaryColor);
            spriteBatch.DrawString(spriteFont, "P2: " + p2.Statistics.Score, new Vector2(this.Game.GraphicsDevice.Viewport.Width / 2 - 50, this.Game.GraphicsDevice.Viewport.Height / 2 + 90), Color.Black);
            spriteBatch.DrawString(spriteFont, "P2: " + p2.Statistics.Score, new Vector2(this.Game.GraphicsDevice.Viewport.Width / 2 - 47, this.Game.GraphicsDevice.Viewport.Height / 2 + 87), p2.PrimaryColor);
            spriteBatch.DrawString(spriteFont, "P3: " + p3.Statistics.Score, new Vector2(this.Game.GraphicsDevice.Viewport.Width / 2 - 50, this.Game.GraphicsDevice.Viewport.Height / 2 + 120), Color.Black);
            spriteBatch.DrawString(spriteFont, "P3: " + p3.Statistics.Score, new Vector2(this.Game.GraphicsDevice.Viewport.Width / 2 - 47, this.Game.GraphicsDevice.Viewport.Height / 2 + 117), p3.PrimaryColor);
            spriteBatch.DrawString(spriteFont, "P4: " + p4.Statistics.Score, new Vector2(this.Game.GraphicsDevice.Viewport.Width / 2 - 50, this.Game.GraphicsDevice.Viewport.Height / 2 + 150), Color.Black);
            spriteBatch.DrawString(spriteFont, "P4: " + p4.Statistics.Score, new Vector2(this.Game.GraphicsDevice.Viewport.Width / 2 - 47, this.Game.GraphicsDevice.Viewport.Height / 2 + 147), p4.PrimaryColor);
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
            if (waitTimeRemaining > 0.0) return;

            if (details.Button == GamePadWrapper.ButtonId.BACK ||
                details.Button == GamePadWrapper.ButtonId.A ||
                details.Button == GamePadWrapper.ButtonId.B)
            {
                GameAudio.PlayCue("back");
                waitTimeRemaining = WAIT_TIME;
                parentSystem.TransitionToMenu(MainMenu.MenuId);
            }
        }
    }
}
