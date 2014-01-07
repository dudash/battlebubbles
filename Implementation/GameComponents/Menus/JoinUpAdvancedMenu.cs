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
// File Created: 30 December 2008, Jason Dudash
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
    /// The advanced join-up menu that allows configuring the AI
    /// </summary>
    class JoinUpAdvancedMenu : BaseMenu
    {
        private static string MENU_ID = "JOIN_UP_ADVANCED_MENU";
        public static string MenuId { get { return MENU_ID; } }

        enum MenuOption { P1_AI, P2_AI, P3_AI, P4_AI }
        MenuOption currentOption = MenuOption.P1_AI;
        double forcedInputWaitTime = 0.0;
        const double FORCED_INPUT_DELAY = 0.2;

        Texture2D backgroundTexture;
        Texture2D hexIcon;
        Texture2D bButton;
        Rectangle P1_AI_POSITION = new Rectangle(300, 250, 50, 50);
        Rectangle P2_AI_POSITION = new Rectangle(300, 320, 50, 50);
        Rectangle P3_AI_POSITION = new Rectangle(300, 390, 50, 50);
        Rectangle P4_AI_POSITION = new Rectangle(300, 460, 50, 50);

        public int p1AI = (int)GlobalResorces.BuiltInPlayerPicsIds.CLOWN;
        public int p2AI = (int)GlobalResorces.BuiltInPlayerPicsIds.BRIDE;
        public int p3AI = (int)GlobalResorces.BuiltInPlayerPicsIds.FRANK;
        public int p4AI = (int)GlobalResorces.BuiltInPlayerPicsIds.CLOWN;
        public string p1AIText = "Hard";
        public string p2AIText = "Medium";
        public string p3AIText = "Easy";
        public string p4AIText = "Hard";

        /// <summary>
        /// Construct the main menu
        /// </summary>
        /// <param name="parentSystem"></param>
        public JoinUpAdvancedMenu(Game game, BaseMenuSystem parentSystem)
            : base(game, parentSystem)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            backgroundTexture = content.Load<Texture2D>(@"W_A_D\Textures\menus\joinup_advanced_menu");
            hexIcon = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\button_a");
            bButton = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\button_b");
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

            Color reddish = new Color(200, 55, 50);
            spriteBatch.DrawString(spriteFont, "P1 AI - ", new Vector2(P1_AI_POSITION.X + 54, P1_AI_POSITION.Y + 14), Color.Black);
            spriteBatch.DrawString(spriteFont, "P1 AI - ", new Vector2(P1_AI_POSITION.X + 50, P1_AI_POSITION.Y + 10), reddish);
            if (p1AI == -1)
            {
                spriteBatch.DrawString(spriteFont, "Off", new Vector2(P1_AI_POSITION.X + 195 + 4, P1_AI_POSITION.Y + 10 + 4), Color.Black);
                spriteBatch.DrawString(spriteFont, "Off", new Vector2(P1_AI_POSITION.X + 195, P1_AI_POSITION.Y + 10), reddish);
            }
            else
            {
                spriteBatch.Draw(GlobalResorces.GetPlayerPic(p1AI), new Rectangle(P1_AI_POSITION.X + 195, P1_AI_POSITION.Y, 64, 64), Color.White);
                spriteBatch.DrawString(spriteFont, p1AIText, new Vector2(P1_AI_POSITION.X + 195 + 80 + 4, P1_AI_POSITION.Y + 10 + 4), Color.Black);
                spriteBatch.DrawString(spriteFont, p1AIText, new Vector2(P1_AI_POSITION.X + 195 + 80, P1_AI_POSITION.Y + 10), reddish);
            }

            spriteBatch.DrawString(spriteFont, "P2 AI - ", new Vector2(P2_AI_POSITION.X + 54, P2_AI_POSITION.Y + 14), Color.Black);
            spriteBatch.DrawString(spriteFont, "P2 AI - ", new Vector2(P2_AI_POSITION.X + 50, P2_AI_POSITION.Y + 10), reddish);
            if (p2AI == -1)
            {
                spriteBatch.DrawString(spriteFont, "Off", new Vector2(P2_AI_POSITION.X + 195 + 4, P2_AI_POSITION.Y + 10 + 4), Color.Black);
                spriteBatch.DrawString(spriteFont, "Off", new Vector2(P2_AI_POSITION.X + 195, P2_AI_POSITION.Y + 10), reddish);
            }
            else
            {
                spriteBatch.Draw(GlobalResorces.GetPlayerPic(p2AI), new Rectangle(P2_AI_POSITION.X + 195, P2_AI_POSITION.Y, 64, 64), Color.White);
                spriteBatch.DrawString(spriteFont, p2AIText, new Vector2(P2_AI_POSITION.X + 195 + 80 + 4, P2_AI_POSITION.Y + 10 + 4), Color.Black);
                spriteBatch.DrawString(spriteFont, p2AIText, new Vector2(P2_AI_POSITION.X + 195 + 80, P2_AI_POSITION.Y + 10), reddish);
            }

            spriteBatch.DrawString(spriteFont, "P3 AI - ", new Vector2(P3_AI_POSITION.X + 54, P3_AI_POSITION.Y + 14), Color.Black);
            spriteBatch.DrawString(spriteFont, "P3 AI - ", new Vector2(P3_AI_POSITION.X + 50, P3_AI_POSITION.Y + 10), reddish);
            if (p3AI == -1)
            {
                spriteBatch.DrawString(spriteFont, "Off", new Vector2(P3_AI_POSITION.X + 195 + 4, P3_AI_POSITION.Y + 10 + 4), Color.Black);
                spriteBatch.DrawString(spriteFont, "Off", new Vector2(P3_AI_POSITION.X + 195, P3_AI_POSITION.Y + 10), reddish);
            }
            else
            {
                spriteBatch.Draw(GlobalResorces.GetPlayerPic(p3AI), new Rectangle(P3_AI_POSITION.X + 195, P3_AI_POSITION.Y, 64, 64), Color.White);
                spriteBatch.DrawString(spriteFont, p3AIText, new Vector2(P3_AI_POSITION.X + 195 + 80 + 4, P3_AI_POSITION.Y + 10 + 4), Color.Black);
                spriteBatch.DrawString(spriteFont, p3AIText, new Vector2(P3_AI_POSITION.X + 195 + 80, P3_AI_POSITION.Y + 10), reddish);
            }

            spriteBatch.DrawString(spriteFont, "P4 AI - ", new Vector2(P4_AI_POSITION.X + 54, P4_AI_POSITION.Y + 14), Color.Black);
            spriteBatch.DrawString(spriteFont, "P4 AI - ", new Vector2(P4_AI_POSITION.X + 50, P4_AI_POSITION.Y + 10), reddish);
            if (p4AI == -1)
            {
                spriteBatch.DrawString(spriteFont, "Off", new Vector2(P4_AI_POSITION.X + 195 + 4, P4_AI_POSITION.Y + 10 + 4), Color.Black);
                spriteBatch.DrawString(spriteFont, "Off", new Vector2(P4_AI_POSITION.X + 195, P4_AI_POSITION.Y + 10), reddish);
            }
            else
            {
                spriteBatch.Draw(GlobalResorces.GetPlayerPic(p4AI), new Rectangle(P4_AI_POSITION.X + 195, P4_AI_POSITION.Y, 64, 64), Color.White);
                spriteBatch.DrawString(spriteFont, p4AIText, new Vector2(P4_AI_POSITION.X + 195 + 80 + 4, P4_AI_POSITION.Y + 10 + 4), Color.Black);
                spriteBatch.DrawString(spriteFont, p4AIText, new Vector2(P4_AI_POSITION.X + 195 + 80, P4_AI_POSITION.Y + 10), reddish);
            }

            switch (currentOption)
            {
                case MenuOption.P1_AI:
                    spriteBatch.Draw(hexIcon, P1_AI_POSITION, Color.White);
                    break;
                case MenuOption.P2_AI:
                    spriteBatch.Draw(hexIcon, P2_AI_POSITION, Color.White);
                    break;
                case MenuOption.P3_AI:
                    spriteBatch.Draw(hexIcon, P3_AI_POSITION, Color.White);
                    break;
                case MenuOption.P4_AI:
                    spriteBatch.Draw(hexIcon, P4_AI_POSITION, Color.White);
                    break;
            }

            spriteBatch.Draw(bButton, new Rectangle(this.GraphicsDevice.Viewport.Width/2-bButton.Width, this.GraphicsDevice.Viewport.Height - bButton.Height - 50,
                bButton.Width, bButton.Height), Color.White);
            spriteBatch.DrawString(spriteFont, "Back to JoinUp Menu", new Vector2(this.GraphicsDevice.Viewport.Width / 2+4, this.GraphicsDevice.Viewport.Height - bButton.Height - 40 + 4), Color.Black);
            spriteBatch.DrawString(spriteFont, "Back to JoinUp Menu", new Vector2(this.GraphicsDevice.Viewport.Width / 2, this.GraphicsDevice.Viewport.Height - bButton.Height - 40), reddish);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// 
        /// </summary>
        private int ToggleToNextAi(int aiId)
        {
            if (aiId == (int)GlobalResorces.BuiltInPlayerPicsIds.CLOWN) return (int)GlobalResorces.BuiltInPlayerPicsIds.BRIDE;
            if (aiId == (int)GlobalResorces.BuiltInPlayerPicsIds.BRIDE) return (int)GlobalResorces.BuiltInPlayerPicsIds.FRANK;
            if (aiId == (int)GlobalResorces.BuiltInPlayerPicsIds.FRANK) return -1;
            if (aiId == -1) return (int)GlobalResorces.BuiltInPlayerPicsIds.CLOWN;
            
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="aiId"></param>
        /// <returns></returns>
        private string GetAIText(int aiId)
        {
            if (aiId == (int)GlobalResorces.BuiltInPlayerPicsIds.CLOWN) return "Hard";
            if (aiId == (int)GlobalResorces.BuiltInPlayerPicsIds.BRIDE) return "Medium";
            if (aiId == (int)GlobalResorces.BuiltInPlayerPicsIds.FRANK) return "Easy";
            return "Unknown AI";
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

            if (details.Button == GamePadWrapper.ButtonId.B ||
                details.Button == GamePadWrapper.ButtonId.BACK )
            {
                GameAudio.PlayCue("dink");
                parentSystem.TransitionToMenu(JoinUpMenu.MenuId);
            }
            else if (details.Button == GamePadWrapper.ButtonId.A)
            {
                // toggle AI types
                if (currentOption == MenuOption.P1_AI)
                {
                    p1AI = ToggleToNextAi(p1AI);
                    p1AIText = GetAIText(p1AI);
                }
                if (currentOption == MenuOption.P2_AI)
                {
                    p2AI = ToggleToNextAi(p2AI);
                    p2AIText = GetAIText(p2AI);
                }
                if (currentOption == MenuOption.P3_AI)
                {
                    p3AI = ToggleToNextAi(p3AI);
                    p3AIText = GetAIText(p3AI);
                }
                if (currentOption == MenuOption.P4_AI)
                {
                    p4AI = ToggleToNextAi(p4AI);
                    p4AIText = GetAIText(p4AI);
                }
            }
            else if (details.Button == GamePadWrapper.ButtonId.D_UP)
            {
                GameAudio.PlayCue("dink");
                // move up an item
                currentOption--;
                if (currentOption < MenuOption.P1_AI) currentOption = MenuOption.P4_AI;
            }
            else if (details.Button == GamePadWrapper.ButtonId.D_DOWN)
            {
                GameAudio.PlayCue("dink");
                // move down an item
                currentOption++;
                if (currentOption > MenuOption.P4_AI) currentOption = MenuOption.P1_AI;
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
                    if (currentOption < MenuOption.P1_AI) currentOption = MenuOption.P4_AI;
                }
                else if (details.StickValue.Y < -0.0)
                {
                    GameAudio.PlayCue("dink");
                    // move down an item
                    currentOption++;
                    if (currentOption > MenuOption.P4_AI) currentOption = MenuOption.P1_AI;
                }
            }
        }
    }
}
