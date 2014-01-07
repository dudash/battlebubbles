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
// File Created: 14 November 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework.Graphics;
using HBBB.GameComponents.BoardComponents;

namespace HBBB.GameComponents.PlayerComponents
{
    /// <summary>
    /// this is the interface for classes capable of being carted around inside
    /// of a player bubble
    /// </summary>
    interface IBubblePayload
    {
        /// <summary>
        /// The player that currently owns this item, if no one owns it the value is null
        /// </summary>
        Player OwningPlayer { get; set;}

        /// <summary>
        /// The slot this block is attavhed to, null if no slot owns it
        /// </summary>
        Slot OwningSlot { get; set;}

        /// <summary>
        /// A texture used to show the payload
        /// </summary>
        Texture2D Texture { get; }
    }
}
