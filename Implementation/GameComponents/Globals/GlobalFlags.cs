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
// File Created: 03 March 2008, Jason Dudash
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace HBBB.GameComponents.Globals
{
    /// <summary>
    /// This class holds come flags that are global to the entire game
    /// </summary>
    static class GlobalFlags
    {
#if DEBUG
        static bool drawDebugJunk = false;
        public static bool DrawDebugJunk
        {
            get { return drawDebugJunk; }
            set { drawDebugJunk = value; }
        }

        static bool drawDebugForces = false;
        public static bool DrawDebugForces
        {
            get { return drawDebugForces; }
            set { drawDebugForces = value; }
        }

        static bool drawDebugBoard = false;
        public static bool DrawDebugBoard
        {
            get { return drawDebugBoard; }
            set { drawDebugBoard = value; }
        }
#endif
    }
}