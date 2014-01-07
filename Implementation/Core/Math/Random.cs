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
using Microsoft.Xna.Framework.Graphics;

namespace HBBB.Core.Math
{
    /// <summary>
    /// A random number class that is better than the
    /// standard System.Random
    /// </summary>
    public class Random
    {
        private static System.Random baseRandom;

        #region Getters and Setters
        public System.Random BaseRandom
        {
            get { return baseRandom; }
        }
        #endregion

        #region Construction and Initialization
        /// <summary>
        /// Construct and initialize the random seed
        /// </summary>
        static Random()
        {
            GetNewSeed();
        }

        /// <summary>
        /// Re-seed this random using the current tickcount
        /// </summary>
        public static void GetNewSeed()
        {
           baseRandom = new System.Random(System.Environment.TickCount);
        }

        /// <summary>
        /// Re-seed this random using the argument int
        /// </summary>
        /// <param name="seed"></param>
        public static void GetNewSeed(int seed)
        {
            baseRandom = new System.Random(seed);
        }
        #endregion

        /// <summary>
        /// Returns a random number between the argument min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double NextDouble(double min, double max)
        {
            if (min >= max) throw new RandomException("Max must be greater than min");
            return (max - min) * baseRandom.NextDouble() + min;
        }

        /// <summary>
        /// Returns a random number between the arg min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float NextFloat(float min, float max)
        {
            if (min >= max) throw new RandomException("Max must be greater than min");
            return (float) ((max - min) * baseRandom.NextDouble() + min);
        }

        /// <summary>
        /// Returns a random number between the arg min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int NextInt(int min, int max)
        {
            if (min >= max) throw new RandomException("Max must be greater than min");
            return baseRandom.Next(min, max);
        }

#if !XBOX && !XBOX360
        /// <summary>
        /// Returns an int corresponding to a random entry in the enum type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int NextEnum(Type type)
        {
            int[] enumList = (int[])Enum.GetValues(type);
            int index = baseRandom.Next(0, enumList.Length);
            return enumList[index];
        }
#endif

        /// <summary>
        /// Returns a random color
        /// </summary>
        /// <returns></returns>
        public static Color NextColor()
        {
            byte r = (byte)baseRandom.Next(0, 255);
            byte g = (byte)baseRandom.Next(0, 255);
            byte b = (byte)baseRandom.Next(0, 255);
            return new Color(r, g, b);
        }
    }
}
