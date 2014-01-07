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
// File Created: 06 July 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using HBBB.Core;
using HBBB.Core.Input;
using Microsoft.Xna.Framework;

namespace HBBB.Core.Menus
{
    /// <summary>
    /// Interface implemented by BaseMenu and used in BaseMenuSystem
    /// </summary>
    interface IMenu
    {
        /// <summary>
        /// Draw the menu
        /// </summary>
        void Draw(GameTime gameTime);

        /// <summary>
        /// Read and take action based on user input to the menu
        /// </summary>
        /// <param name="gameTime"></param>
        void Update(GameTime gameTime);

        /// <summary>
        /// Click handling
        /// </summary>
        /// <param name="index"></param>
        /// <param name="details"></param>
        void OnButtonClick(PlayerIndex index, GamePadButtonEventDetails details);

        /// <summary>
        /// Analog handling
        /// </summary>
        /// <param name="index"></param>
        /// <param name="details"></param>
        void OnAnalogMovement(PlayerIndex index, GamePadAnalogEventDetails details);

        /// <summary>
        /// Transition args
        /// </summary>
        /// <param name="args"></param>
        void ProcessTransitionArgs(string args);
    }
}
