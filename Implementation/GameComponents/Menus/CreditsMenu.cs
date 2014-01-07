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
// File Created: 07 March 2008, Jason Dudash
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
    /// Menu that shows the game credits
    /// </summary>
    class CreditsMenu : BaseMenu
    {
        private static string MENU_ID = "CREDITS_MENU";
        public static string MenuId { get { return MENU_ID; } }

        Texture2D nohandsLogo;
        float rotationLeft;
        Vector2 velocityLeft = new Vector2(100.0f, 150.0f);
        Vector2 positionLeft = new Vector2(150.0f, 400.0f);
        SpriteFont miniSpriteFont;
        bool showVersionFlag = false;

        /// <summary>
        /// Construct the PauseMenu
        /// </summary>
        /// <param name="parentSystem"></param>
        public CreditsMenu(Game game, BaseMenuSystem parentSystem)
            : base(game, parentSystem)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            nohandsLogo = content.Load<Texture2D>(@"W_A_D\Textures\NoHandsLogoOnBlack");
            miniSpriteFont = content.Load<SpriteFont>(@"W_A_D\Fonts\Arial 10");
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

            spriteBatch.Draw(nohandsLogo,
                positionLeft,
                null, Color.White, rotationLeft,
                new Vector2(nohandsLogo.Width / 2, nohandsLogo.Height / 2),
                0.25f, SpriteEffects.None, 0.0f);

            string credit = "* Designed and Developed by Jason Dudash *";
            Vector2 size = spriteFont.MeasureString(credit);
            spriteBatch.DrawString(spriteFont, credit, new Vector2(this.GraphicsDevice.Viewport.Width/2 - size.X/2, 100), Color.White);
            credit = "No Hands Games - http://www.nohandsgames.com/";
            size = spriteFont.MeasureString(credit);
            spriteBatch.DrawString(spriteFont, credit, new Vector2(this.GraphicsDevice.Viewport.Width/2 - size.X/2, 140), Color.White);
            credit = "* Brilliant Music By *";
            size = spriteFont.MeasureString(credit);
            spriteBatch.DrawString(spriteFont, credit, new Vector2(this.GraphicsDevice.Viewport.Width/2 - size.X/2, 200), Color.White);
            credit = "JelloKnee - http://www.jelloknee.com/";
            size = spriteFont.MeasureString(credit);
            spriteBatch.DrawString(spriteFont, credit, new Vector2(this.GraphicsDevice.Viewport.Width/2 - size.X/2, 240), Color.White);
            credit = "* Special Thanks *";
            size = spriteFont.MeasureString(credit);
            spriteBatch.DrawString(spriteFont, credit, new Vector2(this.GraphicsDevice.Viewport.Width/2 - size.X/2, 300), Color.White);
            credit = "to Pete \"Hotrod\" Gonzalez for doing additional programming";
            size = spriteFont.MeasureString(credit);
            spriteBatch.DrawString(spriteFont, credit, new Vector2(this.GraphicsDevice.Viewport.Width/2 - size.X/2, 340), Color.White);
            credit = "to Phil P!nb411 Matarese for the Megabytes";
            size = spriteFont.MeasureString(credit);
            spriteBatch.DrawString(spriteFont, credit, new Vector2(this.GraphicsDevice.Viewport.Width/2 - size.X/2, 380), Color.White);
            credit = "to Chris Nelson for play testing and levels work";
            size = spriteFont.MeasureString(credit);
            spriteBatch.DrawString(spriteFont, credit, new Vector2(this.GraphicsDevice.Viewport.Width/2 - size.X/2, 420), Color.White);
            credit = "to Bryan and Atsuko for play testing";
            size = spriteFont.MeasureString(credit);
            spriteBatch.DrawString(spriteFont, credit, new Vector2(this.GraphicsDevice.Viewport.Width/2 - size.X/2, 460), Color.White);
            credit = "to Luc for play testing";
            size = spriteFont.MeasureString(credit);
            spriteBatch.DrawString(spriteFont, credit, new Vector2(this.GraphicsDevice.Viewport.Width/2 - size.X/2, 500), Color.White);
            credit = "to Chris Allport for insightful technical discussions";
            size = spriteFont.MeasureString(credit);
            spriteBatch.DrawString(spriteFont, credit, new Vector2(this.GraphicsDevice.Viewport.Width/2 - size.X/2, 540), Color.White);
            credit = " ...and of course to Kricket for being awesome";
            size = spriteFont.MeasureString(credit);
            spriteBatch.DrawString(spriteFont, credit, new Vector2(this.GraphicsDevice.Viewport.Width/2 - size.X/2, 580), Color.White);

            if (showVersionFlag) spriteBatch.DrawString(miniSpriteFont, GlobalStrings.VersionString, positionLeft + new Vector2(-60.0f, 60.0f), Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
        
        /// <summary>
        /// update rotations
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (parentSystem.CurrentMenu != this) return;

            rotationLeft += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (rotationLeft > 2*MathHelper.Pi) rotationLeft -= 2*MathHelper.Pi;

            positionLeft += velocityLeft * (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (positionLeft.X > this.Game.GraphicsDevice.Viewport.Width - nohandsLogo.Width / 8) velocityLeft.X = -velocityLeft.X;
            if (positionLeft.X < nohandsLogo.Width / 8) velocityLeft.X = -velocityLeft.X;
            if (positionLeft.Y > this.Game.GraphicsDevice.Viewport.Height - nohandsLogo.Height / 8) velocityLeft.Y = -velocityLeft.Y;
            if (positionLeft.Y < nohandsLogo.Height / 8) velocityLeft.Y = -velocityLeft.Y;
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
                details.Button == GamePadWrapper.ButtonId.B)
            {
                GameAudio.PlayCue("back");
                parentSystem.TransitionToMenu(MainMenu.MenuId);
            }
            else if (details.Button == GamePadWrapper.ButtonId.Y) showVersionFlag = true;
        }
    }
}
