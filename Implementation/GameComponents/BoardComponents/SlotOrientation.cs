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
// File Created: 28 November 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace HBBB.GameComponents.BoardComponents
{
    /// <summary>
    /// This is a simple enum that is used throughout the BoardComponents namespace
    /// to identify slot orientation. It is in a separate class so it can be serialized
    /// in the BoardLevel class.
    /// </summary>
    [Serializable]
    public class SlotOrientation
    {
        public enum Type { ORIENTED_POINTS_HORIZONTAL, ORIENTED_POINTS_VERTICAL };
    }
}
