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
// File Created: 03 March 2008, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HBBB.Core.Graphics
{
    /// <summary>
    /// A splashscreen
    /// </summary>
    class SplashScreen : DrawableGameComponent
    {
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        string texturePath;
        string splashText;
        Vector2 textPosition;
        Texture2D texture;
        Vector2 textureOffset = new Vector2();
        float showTimeRemaining;
        float delayTimeRemaining;
        public float ShowTimeRemaining { get { return showTimeRemaining; } }
        public float DelayTimeRemaining { get { return delayTimeRemaining; } }
        
        public bool IsVisible
        {
            get { return (showTimeRemaining > 0 && delayTimeRemaining <= 0); }
        }

        public void setTextureOffset(int x, int y)
        {
            textureOffset = new Vector2(x, y);
        }

        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="game"></param>
        public SplashScreen(Game game, string texturePath) : base(game)
        {
            this.texturePath = texturePath;
            showTimeRemaining = 0;
            delayTimeRemaining = 0;
            splashText = "";
            textPosition = new Vector2();
        }

        /// <summary>
        /// Called to display the logo for the specific number of seconds
        /// </summary>
        public void ShowTime(int timeInSeconds)
        {
            showTimeRemaining = timeInSeconds;
        }

        /// <summary>
        /// show with a delay
        /// </summary>
        /// <param name="showTime"></param>
        /// <param name="delayTime"></param>
        public void Show(int showTime, int delayTime)
        {
            showTimeRemaining = showTime;
            delayTimeRemaining = delayTime;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        public void SetText(string text)
        {
            splashText = text;
            if (this.Game.GraphicsDevice == null) return;
            CenterSplashText();
        }

        /// <summary>
        /// Load graphics content
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            spriteBatch = new SpriteBatch(this.Game.GraphicsDevice);
            spriteFont = this.Game.Content.Load<SpriteFont>(@"W_A_D\Fonts\Arial 14");
            texture = this.Game.Content.Load<Texture2D>(texturePath);
            CenterSplashText(0.0f, 150.0f);
        }

        /// <summary>
        /// Center text
        /// </summary>
        private void CenterSplashText()
        {
            CenterSplashText(0.0f, 0.0f);
        }

        /// <summary>
        /// center with additional offsets
        /// </summary>
        /// <param name="extraX"></param>
        /// <param name="extraY"></param>
        private void CenterSplashText(float extraX, float extraY)
        {
            float charCountOffset = splashText.Length * 5.5f + extraX;
            float heightOffset = extraY;
            textPosition = new Vector2(this.Game.GraphicsDevice.Viewport.Width / 2 - charCountOffset, this.Game.GraphicsDevice.Viewport.Height / 2 + heightOffset);
        }

        /// <summary>
        /// draw the splash
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            if (showTimeRemaining < 0) return;
            if (delayTimeRemaining > 0) return;
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            spriteBatch.Draw(texture,
                new Vector2(this.Game.GraphicsDevice.Viewport.Width / 2 + textureOffset.X, this.Game.GraphicsDevice.Viewport.Height / 2 + textureOffset.Y),
                null, Color.White, 0,
                new Vector2(texture.Width/2, texture.Height/2),
                0.5f, SpriteEffects.None, 0.0f);
            spriteBatch.DrawString(spriteFont, splashText, textPosition, Color.White);
            spriteBatch.End();
        }

        /// <summary>
        /// Update the splash screen
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (delayTimeRemaining > 0) delayTimeRemaining -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            else showTimeRemaining -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
}