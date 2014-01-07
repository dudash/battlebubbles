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
using System;

namespace HBBB.Core
{
    /// <summary>
    /// An specialized exception
    /// </summary>
    public class CoreException : System.Exception
    {
        /// <summary>
        /// Exception with no arguments
        /// </summary>
        public CoreException() : base() {}
        /// <summary>
        /// Exception with a string argument
        /// </summary>
        /// <param name="message">the exception message</param>
        public CoreException(string message) : base(message) {}
    }
}
