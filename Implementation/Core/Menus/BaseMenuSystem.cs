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
// File Created: 14 October 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;

namespace HBBB.Core.Menus
{
    /// <summary>
    /// A MenuSystem provides a series of menus and a mechanisim to switch between
    /// them based upon commands from the currecntly executing menu.
    /// </summary>
    abstract class BaseMenuSystem : GameComponent
    {
        public StorageDevice StorageDevice;
        public IAsyncResult StorageDeviceResult;

        /// <summary>
        /// The list of all menus
        /// </summary>
        protected SortedList<string, IMenu> menus;

        /// <summary>
        /// The currently active menu
        /// </summary>
        protected IMenu currentMenu;
        public IMenu CurrentMenu { get { return currentMenu; } }

        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="game"></param>
        public BaseMenuSystem(Game game) : base(game)
        {
            menus = new SortedList<string, IMenu>();
        }

        /// <summary>
        /// Add players to the game
        /// </summary>
        public abstract void RequestCreatePlayers();

        /// <summary>
        /// Start the game
        /// </summary>
        public abstract void RequestStartSession();

        /// <summary>
        /// Start the game
        /// </summary>
        public abstract void RequestEndSession();

        /// <summary>
        /// pause the game
        /// </summary>
        public abstract void ShowPauseMenu(PlayerIndex pausingPlayer);

        /// <summary>
        /// resume the game
        /// </summary>
        public abstract void HidePauseMenu();

        /// <summary>
        /// Quit the menu system and everything
        /// </summary>
        public abstract void RequestQuitGame();

        /// <summary>
        /// Transition to a new menu
        /// </summary>
        /// <param name="newMenu"></param>
        public virtual void TransitionToMenu(string menuId, string args)
        {
            if ( menus[menuId] != null )
            {
                currentMenu = menus[menuId];
                currentMenu.ProcessTransitionArgs(args);
            }
        }

        /// <summary>
        /// Transition with no args
        /// </summary>
        /// <param name="menuId"></param>
        public virtual void TransitionToMenu(string menuId)
        {
            TransitionToMenu(menuId, "");
        }
    }
}
