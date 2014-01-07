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
// File Created: 11 April 2008, Jason Dudash
//-----------------------------------------------------------------------------
#endregion

namespace HBBB.Core.Input
{
    /// <summary>
    /// Contains all the parameters that describe a gamepad button event
    /// </summary>
    class GamePadButtonEventDetails
    {
        /// <summary>
        /// The button the event occured on
        /// </summary>
        private GamePadWrapper.ButtonId button;
        public GamePadWrapper.ButtonId Button { get { return button; } }

        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="button"></param>
        public GamePadButtonEventDetails(GamePadWrapper.ButtonId button)
        {
            this.button = button;
        }
    }
}
