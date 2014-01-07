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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace HBBB.GameComponents.Globals
{
    /// <summary>
    /// This class contains static methods for accessing static global objects
    /// </summary>
    static class GlobalResorces
    {
        public enum BuiltInPlayerPicsIds
        {
            APE = 0, PIRATE, FLOWER, BEAR, FRANK, BRIDE, COON, CLOWN
        }
        private static List<Texture2D> builtInPlayerPics;
        private static System.Random baseRandom;
        private static bool initialized = false;
        /// <summary>
        /// Load textures when the graphics device is ready
        /// </summary>
        /// <param name="content"></param>
        public static void LoadContent(ContentManager content)
        {
            builtInPlayerPics = new List<Texture2D>();
            builtInPlayerPics.Insert((int)BuiltInPlayerPicsIds.APE, content.Load<Texture2D>(@"W_A_D\Textures\PlayerPics\ape"));
            builtInPlayerPics.Insert((int)BuiltInPlayerPicsIds.PIRATE, content.Load<Texture2D>(@"W_A_D\Textures\PlayerPics\pirate"));
            builtInPlayerPics.Insert((int)BuiltInPlayerPicsIds.FLOWER, content.Load<Texture2D>(@"W_A_D\Textures\PlayerPics\flower"));
            builtInPlayerPics.Insert((int)BuiltInPlayerPicsIds.BEAR, content.Load<Texture2D>(@"W_A_D\Textures\PlayerPics\bear"));
            builtInPlayerPics.Insert((int)BuiltInPlayerPicsIds.FRANK, content.Load<Texture2D>(@"W_A_D\Textures\PlayerPics\frank"));
            builtInPlayerPics.Insert((int)BuiltInPlayerPicsIds.BRIDE, content.Load<Texture2D>(@"W_A_D\Textures\PlayerPics\bride"));
            builtInPlayerPics.Insert((int)BuiltInPlayerPicsIds.COON, content.Load<Texture2D>(@"W_A_D\Textures\PlayerPics\coon"));
            builtInPlayerPics.Insert((int)BuiltInPlayerPicsIds.CLOWN, content.Load<Texture2D>(@"W_A_D\Textures\PlayerPics\clown"));

            baseRandom = new System.Random(System.Environment.TickCount);

            initialized = true;
        }

        /// <summary>
        /// return a random player pic
        /// </summary>
        /// <returns></returns>
        public static Texture2D GetRandomPlayerPic()
        {
            if (!initialized) throw new Exception("GlobalResources::GetRandomPlayerPic - Global Resources not initialized");
            if (builtInPlayerPics == null || builtInPlayerPics.Count < 1) throw new Exception("GlobalResources::GetRandomPlayerPic - builtin player pics null or empty");

            return builtInPlayerPics[baseRandom.Next(0, builtInPlayerPics.Count)];
        }

        /// <summary>
        /// return a random player pic
        /// </summary>
        /// <returns></returns>
        public static Texture2D GetPlayerPic(int index)
        {
            if (index >= builtInPlayerPics.Count) return builtInPlayerPics[0];
            return builtInPlayerPics[index];
        }
    }
}
