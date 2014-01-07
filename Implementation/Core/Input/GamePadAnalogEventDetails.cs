#region Copyright
//-----------------------------------------------------------------------------
// Copyright (C)2007 Jason Dudash, GNU GPLv3
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
// File Created: 12 April 2008, Jason Dudash
//-----------------------------------------------------------------------------
#endregion

using Microsoft.Xna.Framework;

namespace HBBB.Core.Input
{
    /// <summary>
    /// Contains all the parameters that describe a gamepad analog event
    /// (stick movement, trigger movement)
    /// </summary>
    class GamePadAnalogEventDetails
    {
        /// <summary>
        /// The analog button the event occured on
        /// </summary>
        private GamePadWrapper.AnalogId analogButton;
        public GamePadWrapper.AnalogId AnalogButton { get { return analogButton; } }
        /// <summary>
        /// The value of the analog button (X and Y)
        /// </summary>
        private Vector2 stickValue;
        public Vector2 StickValue { get { return stickValue; } }

        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="button"></param>
        public GamePadAnalogEventDetails(GamePadWrapper.AnalogId analogButton, Vector2 value)
        {
            this.analogButton = analogButton;
            this.stickValue = value;
        }

        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="button"></param>
        public GamePadAnalogEventDetails(GamePadWrapper.AnalogId analogButton, float value)
        {
            this.analogButton = analogButton;
            this.stickValue = new Vector2(value, value);  // just stuff it into both
        }
    }
}
