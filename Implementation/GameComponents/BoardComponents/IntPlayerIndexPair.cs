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
// File Created: 5 February 2008, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using Microsoft.Xna.Framework;

namespace HBBB.GameComponents.BoardComponents
{
    /// <summary>
    /// This class associates a player index with an integer value.  It is used
    /// in the BoardLevel because of it ability to be seralized and deserialized.
    /// </summary>
    [Serializable]
    public class IntPlayerIndexPair
    {
        private int intValue;
        public int IntValue
        {
            get { return intValue; }
            set { intValue = value; }
        }
        private PlayerIndex playerIndex;
        public PlayerIndex PlayerIndex
        {
            get { return playerIndex; }
            set { playerIndex = value; }
        }

        /// <summary>
        /// Construct with default values
        /// </summary>
        public IntPlayerIndexPair()
        {
            intValue = 0;
            PlayerIndex = PlayerIndex.One;
        }

        /// <summary>
        /// Construct with specified argument values
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        public IntPlayerIndexPair(int value, PlayerIndex index)
        {
            this.intValue = value;
            this.playerIndex = index;
        }

    }
}
