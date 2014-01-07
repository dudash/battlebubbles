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
// File Created: 21 August 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace HBBB.Core.MassSpring.Verlet
{
    /// <summary>
    /// A particle system that uses Verlet integration rather than Euler integration to calculate
    /// position of the points.  Verlet integration offers greater stability by by calculating the
    /// position at the next time step from the positions at the previous and current time steps, 
    /// without using the velocity.
    /// </summary>
    class VerletSystem
    {
        public List<VerletPoint> PointsList = new List<VerletPoint>();

        /// <summary>
        /// Construct
        /// </summary>
        public VerletSystem()
        {
        }

        /// <summary>
        /// Add an already created point to the system
        /// </summary>
        /// <param name="point"></param>
        public void Add(VerletPoint point)
        {
            PointsList.Add(point);
        }

        /// <summary>
        /// Clear the forces from all of the points
        /// </summary>
        public void ClearPointForces()
        {
            foreach (VerletPoint p in PointsList)
            {
                p.Force = new Vector2(0.0f, 0.0f);
            }
        }

        /// <summary>
        /// Create a point and add it to the system
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public VerletPoint CreatePoint(Vector2 vector)
        {
            VerletPoint point = new VerletPoint();
            point.SetPosition(new Vector2(vector.X, vector.Y));
            this.Add(point);
            return point;
        }

        /// <summary>
        /// Create a point with mass and add it to the system
        /// </summary>
        /// <param name="vector"></param>
        /// <param name="mass"></param>
        /// <returns></returns>
        public VerletPoint CreatePoint(Vector2 vector, float mass)
        {
            VerletPoint p = CreatePoint(vector);
            p.Mass = mass;
            return p;
        }
    }
}
