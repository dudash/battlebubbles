#region Copyright
//-----------------------------------------------------------------------------
// Copyright (C)2008 Jason Dudash, GNU GPLv3.
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
// File Created: 08 February 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HBBB.Core.Graphics;

namespace HBBB.GameComponents.HUD
{
    /// <summary>
    /// This HUD component provides a clock that displays time remaining in the current game.
    /// </summary>
    class Clock : IHUDComponent
    {
        /// <summary>
        /// The position and size of the clock
        /// </summary>
        private Rectangle bounds;
        /// <summary>
        /// The game session
        /// </summary>
        private GameSession session;
        /// <summary>
        /// Starting seconds on clock
        /// </summary>
        int startSeconds;

        /// <summary>
        /// construct with a game session
        /// </summary>
        /// <param name="session"></param>
        public Clock(Rectangle bounds, int startSeconds, ref GameSession session)
        {
            this.session = session;
            this.bounds = bounds;
            this.startSeconds = startSeconds;
        }

        /// <summary>
        /// Draw this clock
        /// </summary>
        public void Draw(SpriteBatch batch, SpriteFont spriteFont, PrimitiveBatch primitiveBatch)
        {
            double secondsRemaining = (startSeconds - session.EllapsedSessionTime.TotalSeconds);
            double minutesRemaining = secondsRemaining / 60.0;
            int wholeMinutesRemaining = (int) minutesRemaining;
            int wholeSecondsRemaining = (int)((minutesRemaining - wholeMinutesRemaining) * 60);
            string timeText = String.Format("Time Remaining    {0:00}:{1:00}", wholeMinutesRemaining, wholeSecondsRemaining);
            batch.DrawString(spriteFont, timeText, new Vector2(bounds.X + 4, bounds.Y + 4), Color.Black);
            batch.DrawString(spriteFont, timeText, new Vector2(bounds.X, bounds.Y), Color.White);
        }

#if DEBUG
        /// <summary>
        /// Render square bounds of the playerbox
        /// </summary>
        /// <param name="batch"></param>
        public void DebugRender(PrimitiveBatch batch)
        {
            batch.Begin(PrimitiveType.LineList);
            batch.AddVertex(new Vector2(bounds.Left, bounds.Top), Color.White);
            batch.AddVertex(new Vector2(bounds.Left, bounds.Bottom), Color.White);

            batch.AddVertex(new Vector2(bounds.Left, bounds.Bottom), Color.White);
            batch.AddVertex(new Vector2(bounds.Right, bounds.Bottom), Color.White);

            batch.AddVertex(new Vector2(bounds.Right, bounds.Bottom), Color.White);
            batch.AddVertex(new Vector2(bounds.Right, bounds.Top), Color.White);

            batch.AddVertex(new Vector2(bounds.Right, bounds.Top), Color.White);
            batch.AddVertex(new Vector2(bounds.Left, bounds.Top), Color.White);
            batch.End();
        }
#endif
    }
}
