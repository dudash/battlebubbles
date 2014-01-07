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
// File Created: 10 April 2008, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using HBBB.Core.Math;

namespace HBBB.Core.Graphics
{
    /// <summary>
    /// This is a class to animate a sprite from a texture using a custom class. 
    /// It presumes that the texture being loaded is a strip of equal-sized images.
    /// </summary>
    class AnimatedTexture2D
    {
        /// <summary>
        /// A sheet of textures to animate (spaced horizontally)
        /// </summary>
        protected Texture2D textureSheet;
        public Texture2D TextureSheet
        {
            get { return textureSheet; }
            set { textureSheet = value; }
        }
        /// <summary>
        /// The number of frames in this sheet
        /// </summary>
        protected int frameCount;
        public int FrameCount
        {
            get { return frameCount; }
            set { frameCount = value; }
        }
        /// <summary>
        /// how long to display each frame for
        /// </summary>
        protected float timePerFrame;
        public float TimePerFrame
        {
            get { return timePerFrame; }
            set { timePerFrame = value; }
        }
        /// <summary>
        /// The amount of time ellapsed for the current frame (in seconds)
        /// </summary>
        protected float ellapsedFrameTime;
        /// <summary>
        /// index of the current frame
        /// </summary>
        int index;
        public int Index { get { return index; } }
        /// <summary>
        /// Pause the animation
        /// </summary>
        protected bool paused;
        public bool Paused { get { return paused; } set { paused = value; } }
        /// <summary>
        /// playing a single cycle of frames
        /// </summary>
        protected bool playCycleFlag;
        
        /// <summary>
        /// Construct
        /// </summary>
        public AnimatedTexture2D()
        {
            index = 0;
            timePerFrame = 0.2f;
            frameCount = 0;
            ellapsedFrameTime = 0.0f;
            paused = true;
            playCycleFlag = false;
        }

        /// <summary>
        /// load the sprite sheet
        /// </summary>
        /// <param name="textureSheet"></param>
        /// <param name="frames"></param>
        /// <param name="fps"></param>
        public void LoadContent(ref Texture2D textureSheet, int frameCount, int fps)
        {
            this.textureSheet = textureSheet;
            this.frameCount = frameCount;
            if (fps < 1) fps = 1;
            this.timePerFrame = 1.0f/(float)fps;
        }

        /// <summary>
        /// modify the start time by randomly shortening the frist frame play time
        /// </summary>
        /// <param name="offset"></param>
        public void OffsetStartTime()
        {
            ellapsedFrameTime += Random.NextFloat(0.0f, timePerFrame);
        }

        /// <summary>
        /// Update the texture with the gametime
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (paused) return;
            // update index
            ellapsedFrameTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (ellapsedFrameTime > timePerFrame)
            {
                index++;
                if (index > frameCount - 1)
                {
                    index = 0;
                    if (playCycleFlag) // if playing a single cycle, reset the cycle
                    {
                        playCycleFlag = false;
                        paused = true;
                    }
                }
                ellapsedFrameTime -= timePerFrame; // reset the ellapsed (but include the current ellapsed overflow)
            }
        }

        /// <summary>
        /// Play one cycle of the entire set of frames
        /// </summary>
        public void PlayCycle()
        {
            playCycleFlag = true;
            index = 0;
            paused = false;
        }

        /// <summary>
        /// Draw the texture
        /// </summary>
        /// <param name="batch"></param>
        /// <param name="frame"></param>
        /// <param name="screenPos"></param>
        public void Draw(SpriteBatch batch, Rectangle bounds, Color color)
        {
            int frameWidth = textureSheet.Width / frameCount;
            Rectangle sourcerect = new Rectangle(frameWidth * index, 0, frameWidth, textureSheet.Height);
            batch.Draw(textureSheet, bounds, sourcerect, color); //, Rotation, Origin, Scale, SpriteEffects.None, Depth);
        }
    }
}
