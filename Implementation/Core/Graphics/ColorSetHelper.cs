#region Copyright
//-----------------------------------------------------------------------------
// Copyright (C)2007 Jason Dudash, GNU GPLv3
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
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace HBBB.Core.Graphics
{
    /// <summary>
    /// This class provides access to a set of colors and functions to
    /// move through and randomly choose one
    /// </summary>
    class ColorSetHelper
    {
        static List<Color> colorsList;
        static System.Random baseRandom = new Random();

        /// <summary>
        /// Construction adds all the colors to the list
        /// </summary>
        static ColorSetHelper()
        {
            colorsList = new List<Color>();
            colorsList.Add(Color.White);
            colorsList.Add(Color.Silver);
            colorsList.Add(Color.DarkGray);
            colorsList.Add(Color.Gray);
            colorsList.Add(Color.Black);
            colorsList.Add(Color.Yellow);
            colorsList.Add(Color.Gold);
            colorsList.Add(Color.Pink);
            colorsList.Add(Color.Red);
            colorsList.Add(Color.DarkRed);
            colorsList.Add(Color.SkyBlue);
            colorsList.Add(Color.Blue);
            colorsList.Add(Color.DarkBlue);
            colorsList.Add(Color.LimeGreen);
            colorsList.Add(Color.Green);
            colorsList.Add(Color.DarkGreen);
            colorsList.Add(Color.Tan);
            colorsList.Add(Color.Brown);
            colorsList.Add(Color.Orange);
            colorsList.Add(Color.Violet);
            colorsList.Add(Color.Purple);
        }

        /// <summary>
        /// randomly grab a color
        /// </summary>
        /// <returns></returns>
        public static Color RandomColor()
        {
            int index = baseRandom.Next(0, colorsList.Count-1);
            return colorsList[index];
        }

        /// <summary>
        /// Return the color after the argument color
        /// </summary>
        /// <returns></returns>
        public static Color NextColor(Color color)
        {
            int index = FindColorIndex(color);
            index++;
            if (index >= colorsList.Count) index = colorsList.Count - 1;
            return colorsList[index];
        }

        /// <summary>
        /// Return the color before the argument color
        /// </summary>
        /// <returns></returns>
        public static Color PrevColor(Color color)
        {
            int index = FindColorIndex(color);
            index--;
            if (index < 0) index = 0;
            return colorsList[index];
        }

        /// <summary>
        /// Find the index of the argument color
        /// </summary>
        /// <param name="color"></param>
        static private int FindColorIndex(Color color)
        {
            int cindex = 0;
            foreach (Color cIter in colorsList)
            {
                if (cIter == color) return cindex;
                cindex++;
            }
            return 0;
        }
    }
}
