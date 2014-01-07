#region Copyright
//-----------------------------------------------------------------------------
// Copyright (C)2008 Jason Dudash, GNU GPLv3.
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
// File Created: 14 August 2006, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;

namespace HBBB.Core.Particle2D
{
    /// <summary>
    /// This class is the base class for particle modifiers.  A modifier
    /// registers with a particle system in order to affect the set of
    /// particles in that system.  A modifier can be turned on and off
    /// but still remain registered with the system.  A modifier operates
    /// on a per particle basis.
    /// </summary>
    public abstract class Modifier
    {
        private bool enabled = true;

        #region Getters and Setters
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }
        #endregion

        /// <summary>
        /// Must be overriden by derived classes to modify a particle
        /// </summary>
        /// <param name="deltaTime"></param>
        public abstract void Modify(float deltaTime, Particle p);
    }
}
