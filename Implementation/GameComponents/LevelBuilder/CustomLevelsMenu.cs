#region Copyright
//-----------------------------------------------------------------------------
// Copyright (C)2007-2008 Jason Dudash, GNU GPLv3.
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
// File Created: 30 December 2008, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System.Collections.Generic;
using HBBB.Core;
using HBBB.Core.Menus;
using HBBB.GameComponents.Globals;
using HBBB.Core.Input;
using HBBB.GameComponents.Menus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using System.IO;

namespace HBBB.GameComponents.LevelBuilder
{
    /// <summary>
    /// The menu to add/remove custom levels
    /// </summary>
    class CustomLevelsMenu : BaseMenu
    {
        private static string MENU_ID = "CUSTOM_LEVELS_MENU";
        public static string MenuId { get { return MENU_ID; } }

        double forcedInputWaitTime = 0.0;
        const double FORCED_INPUT_DELAY = 0.2;

        List<string> customLevelFileNames;
        int currentIndex;
        bool showConfirmDeleteFlag = false;

        Texture2D backgroundTexture;
        Texture2D hexIcon;
        Texture2D controlsTexture;
        Texture2D bToExitTexture;
        Texture2D confirmDeleteTexture;

        /// <summary>
        /// Construct the main menu
        /// </summary>
        /// <param name="parentSystem"></param>
        public CustomLevelsMenu(Game game, BaseMenuSystem parentSystem)
            : base(game, parentSystem)
        {
            customLevelFileNames = new List<string>();
            currentIndex = 0;
        }

        /// <summary>
        /// on transition
        /// </summary>
        /// <param name="args"></param>
        public override void ProcessTransitionArgs(string args)
        {
            // no args expected, just reload the list of custom levels
            parentSystem.StorageDevice = null;
            parentSystem.StorageDeviceResult = Guide.BeginShowStorageDeviceSelector(PlayerIndex.One, null, null);
            base.ProcessTransitionArgs(args);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            backgroundTexture = content.Load<Texture2D>(@"W_A_D\Textures\menus\hexbacking_1_menu");
            hexIcon = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\button_a");
            controlsTexture = content.Load<Texture2D>(@"W_A_D\Textures\menus\create_delete");
            bToExitTexture = content.Load<Texture2D>(@"W_A_D\Textures\menus\press_b_to_exit");
            confirmDeleteTexture = content.Load<Texture2D>(@"W_A_D\Textures\menus\confirm_delete_menu");
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void UnloadContent()
        {
            content.Unload();
            base.UnloadContent();
        }

        /// <summary>
        /// search directorys for custom levels
        /// </summary>
        private void FindCustomLevels()
        {
            if (parentSystem.StorageDevice == null) return;

            customLevelFileNames.Clear();
            currentIndex = 0;

            // look for .lvl files and add them to the levels list
            StorageContainer container = parentSystem.StorageDevice.OpenContainer(GlobalStrings.CustomLevelsStorageIdentifier);
            ICollection<string> FileList = Directory.GetFiles(container.Path);
            foreach (string filename in FileList)
            {
                customLevelFileNames.Add(filename);
            }
            container.Dispose();
        }

        /// <summary>
        /// atttempt to remove the selected level from disk
        /// </summary>
        private void DeleteCurrentLevel()
        {
            string selectedFilename = customLevelFileNames[currentIndex];

            StorageContainer container = parentSystem.StorageDevice.OpenContainer(GlobalStrings.CustomLevelsStorageIdentifier);
            if (File.Exists(selectedFilename)) File.Delete(selectedFilename);
            customLevelFileNames.Remove(selectedFilename);
            currentIndex--;
            if (currentIndex < 0) currentIndex = 0;
            container.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (parentSystem.CurrentMenu != this) return;

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height), Color.White);

            if (customLevelFileNames.Count < 1)
            {
                spriteBatch.DrawString(spriteFont, "No custom levels could be found.", new Vector2(this.GraphicsDevice.Viewport.Width / 4, this.GraphicsDevice.Viewport.Height / 3), Color.Black);
                spriteBatch.DrawString(spriteFont, "No custom levels could be found.", new Vector2(this.GraphicsDevice.Viewport.Width / 4 + 4, this.GraphicsDevice.Viewport.Height / 3 + 4), Color.White);
            }
            else
            {
                int localIndex = currentIndex;
                int offsetSpacing = 40;
                int yOffset = 0;  // offset for filename in list
                int initialYOffset = 0;  // initial offset for scrolling list support (negative to scroll up off screen)
                if (currentIndex > 10)
                {
                    initialYOffset = (currentIndex - 10) * -offsetSpacing;
                    localIndex = 10;
                }
                int iter = 1;
                foreach (string fileName in customLevelFileNames)
                {
                    spriteBatch.DrawString(spriteFont, iter + ".  " + fileName, new Vector2(170, 100 + yOffset + initialYOffset), Color.Black);
                    spriteBatch.DrawString(spriteFont, iter + ".  " + fileName, new Vector2(174, 104 + yOffset + initialYOffset), Color.White);
                    yOffset += offsetSpacing;
                    iter++;
                }

                // draw hex icon next to selected level index
                spriteBatch.Draw(hexIcon, new Rectangle(110, 100 + offsetSpacing * localIndex, 50, 50), Color.White);
            }

            if (showConfirmDeleteFlag)
            {
                spriteBatch.Draw(confirmDeleteTexture, new Rectangle(this.GraphicsDevice.Viewport.Width / 2 - confirmDeleteTexture.Width / 2,
                    this.GraphicsDevice.Viewport.Height / 2 - confirmDeleteTexture.Height / 2,
                    confirmDeleteTexture.Width, confirmDeleteTexture.Height), Color.White);
            }
            else
            {
                spriteBatch.Draw(controlsTexture, new Rectangle(this.GraphicsDevice.Viewport.Width / 2 - controlsTexture.Width / 2, 50, 375, 50), Color.White);
                spriteBatch.Draw(bToExitTexture, new Rectangle(this.GraphicsDevice.Viewport.Width / 2 - bToExitTexture.Width / 2, this.GraphicsDevice.Viewport.Height - 100, 375, 50), Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (parentSystem.CurrentMenu != this) return;
            forcedInputWaitTime += gameTime.ElapsedGameTime.TotalSeconds;  // forced delay in gamepad input

            // check for device connectivity
            if (gameTime.TotalRealTime.Seconds % 2 == 0)  // check these things less often
            {
                if (parentSystem.StorageDevice == null)
                {
                    if (parentSystem.StorageDeviceResult == null)
                        parentSystem.StorageDeviceResult = Guide.BeginShowStorageDeviceSelector(PlayerIndex.One, null, null);
                    else if (parentSystem.StorageDeviceResult.IsCompleted)
                    {
                        parentSystem.StorageDevice = Guide.EndShowStorageDeviceSelector(parentSystem.StorageDeviceResult);
                        if (parentSystem.StorageDevice.IsConnected) FindCustomLevels();
                    }
                }
                else if (parentSystem.StorageDevice != null)
                {
                    parentSystem.StorageDeviceResult = null;
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// When a button is clicked
        /// </summary>
        /// <param name="details"></param>
        public override void OnButtonClick(PlayerIndex index, GamePadButtonEventDetails details)
        {
            if (parentSystem.CurrentMenu != this) return;

            // ************ Process confirmation box
            if (showConfirmDeleteFlag)
            {
                // A to delete
                if (details.Button == GamePadWrapper.ButtonId.A)
                {
                    GameAudio.PlayCue("dink");
                    DeleteCurrentLevel();
                    showConfirmDeleteFlag = false;
                }
                // B to cancel
                else if (details.Button == GamePadWrapper.ButtonId.B)
                {
                    GameAudio.PlayCue("dink");
                    showConfirmDeleteFlag = false;
                }
                return;
            }

            // **************** Process a normal level selection input
            if (details.Button == GamePadWrapper.ButtonId.B)
            {
                GameAudio.PlayCue("back");
                parentSystem.TransitionToMenu(MainMenu.MenuId);
                return;
            }
            else if (details.Button == GamePadWrapper.ButtonId.START)
            {
                GameAudio.PlayCue("dink");
                StorageContainer container = parentSystem.StorageDevice.OpenContainer(GlobalStrings.CustomLevelsStorageIdentifier);
                // start to add a new custom level by copying empty.lvl
                string newfilename = Path.Combine(container.Path, "Custom_" + System.DateTime.Now.ToString("yyyy_MM_dd-HH_mm_ss_ff") + ".lvl");
                File.Copy(@"W_A_D\Boards\empty.lvl", newfilename, true);
                customLevelFileNames.Add(newfilename);
                container.Dispose();
                return;
            }

            if (customLevelFileNames.Count < 1) return;  // we can only edit and remove and select if we have a list
            if (currentIndex < 0) currentIndex = 0;
            if (currentIndex > customLevelFileNames.Count - 1) currentIndex = customLevelFileNames.Count - 1;
            string selectedFilename = customLevelFileNames[currentIndex];
            // A to edit the selected file
            if (details.Button == GamePadWrapper.ButtonId.A)
            {
                GameAudio.PlayCue("dink");
                parentSystem.TransitionToMenu(EditLevelMenu.MenuId, selectedFilename);
                return;
            }
            // back to delete selected custom level
            else if (details.Button == GamePadWrapper.ButtonId.BACK)
            {
                GameAudio.PlayCue("dink");
                showConfirmDeleteFlag = true;
                return;
            }

            // dpad up to move up/down
            if (details.Button == GamePadWrapper.ButtonId.D_UP)
            {
                GameAudio.PlayCue("dink");
                currentIndex--;
                if (currentIndex < 0) currentIndex = 0;
                if (currentIndex > customLevelFileNames.Count - 1) currentIndex = customLevelFileNames.Count - 1;
            }
            else if (details.Button == GamePadWrapper.ButtonId.D_DOWN)
            {
                GameAudio.PlayCue("dink");
                currentIndex++;
                if (currentIndex < 0) currentIndex = 0;
                if (currentIndex > customLevelFileNames.Count - 1) currentIndex = customLevelFileNames.Count - 1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="details"></param>
        public override void OnAnalogMovement(PlayerIndex index, GamePadAnalogEventDetails details)
        {
            if (parentSystem.CurrentMenu != this) return;
            if (customLevelFileNames.Count < 1) return;  // we can move between items if we have a list

            if (forcedInputWaitTime < FORCED_INPUT_DELAY) return;
            else forcedInputWaitTime = 0.0;

            if (details.AnalogButton == GamePadWrapper.AnalogId.LEFT_STICK)
            {
                if (details.StickValue.Y > 0.0)
                {
                    GameAudio.PlayCue("dink");
                    currentIndex--;
                    if (currentIndex < 0) currentIndex = 0;
                    if (currentIndex > customLevelFileNames.Count - 1) currentIndex = customLevelFileNames.Count - 1;
                }
                else if (details.StickValue.Y < -0.0)
                {
                    GameAudio.PlayCue("dink");
                    currentIndex++;
                    if (currentIndex < 0) currentIndex = 0;
                    if (currentIndex > customLevelFileNames.Count - 1) currentIndex = customLevelFileNames.Count - 1;
                }
            }
        }
    }
}
