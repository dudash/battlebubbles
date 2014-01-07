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
// File Created: 15 October 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;

namespace HBBB.GameComponents.PlayerComponents
{
    /// <summary>
    /// This class holds statistical information about a player
    /// as it pertains to a game session
    /// </summary>
    class PlayerStats
    {
        /// <summary>
        /// A score calculated from the statistics
        /// </summary>
        int score;
        public int Score { get { return score; } }
        /// <summary>
        /// The number of blocks set and locked
        /// </summary>
        int lockedBlockPoints;
        public int LockedBlockPoints
        {
            get { return lockedBlockPoints; }
            set { lockedBlockPoints = value; RecalculateScore(); }
        }
        /// <summary>
        /// The number of blocks currently set but not locked
        /// </summary>
        int blocksSet;
        public int BlocksSet
        {
            get { return blocksSet; }
            set { blocksSet = value; RecalculateScore(); }
        }
        /// <summary>
        /// The total number of steal that occured during the game
        /// </summary>
        int steals;
        public int Steals
        {
            get { return steals; }
            set { steals = value; RecalculateScore(); }
        }
        int stolenFrom;
        public int StolenFrom
        {
            get { return stolenFrom; }
            set { stolenFrom = value; RecalculateScore(); }
        }
        /// <summary>
        /// The number of powerups played
        /// </summary>
        int powerUpsPlayed;
        public int PowerUpsPlayed
        {
            get { return powerUpsPlayed; }
            set { powerUpsPlayed = value; }
        }
        /// <summary>
        /// The number of drops that did not set
        /// </summary>
        int misDrops;
        public int MisDrops
        {
            get { return misDrops; }
            set { misDrops = value; }
        }

        /// <summary>
        /// Clear out the old statistics
        /// </summary>
        public void Clear()
        {
            score = 0;
            lockedBlockPoints = 0;
            blocksSet = 0;
            steals = 0;
            powerUpsPlayed = 0;
            misDrops = 0;
            stolenFrom = 0;
        }

        /// <summary>
        /// Recalculate statistics
        /// </summary>
        public void RecalculateScore()
        {
            score = lockedBlockPoints + blocksSet + steals - stolenFrom;
        }
    }
}
