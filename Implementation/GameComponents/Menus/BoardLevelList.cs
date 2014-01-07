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
// File Created: 29 Feb 2008, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;

namespace HBBB.GameComponents.Menus
{
    /// <summary>
    /// A list of board level infos that can be read from disk
    /// </summary>
    [Serializable]
    public class BoardLevelList
    {
        /// <summary>
        /// Just the one list
        /// </summary>
        List<BoardLevelInfo> boardLevels = new List<BoardLevelInfo>();
        public List<BoardLevelInfo> BoardLevels
        {
            get { return boardLevels; }
            set { boardLevels = value; }
        }

        /// <summary>
        /// A nested class containing info about the board levels
        /// </summary>
        public class BoardLevelInfo
        {
            /// <summary>
            /// Name of this level
            /// </summary>
            string name;
            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            /// <summary>
            /// Filename of this level
            /// </summary>
            string filename;
            public string Filename
            {
                get { return filename; }
                set { filename = value; }
            }

            /// <summary>
            /// Desription of this level
            /// </summary>
            string description;
            public string Description
            {
                get { return description; }
                set { description = value; }
            }

            /// <summary>
            /// Desription of this level
            /// </summary>
            string description2;
            public string Description2
            {
                get { return description2; }
                set { description2 = value; }
            }

            /// <summary>
            /// The texture displayed for a thumbnail
            /// </summary>
            string thumbnailTextureName;
            public string ThumbnailTextureName
            {
                get { return thumbnailTextureName; }
                set { thumbnailTextureName = value; }
            }
        }
    }
}
