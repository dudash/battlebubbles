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
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using HBBB.Core.Menus;
using HBBB.Core.Input;
using HBBB.GameComponents.Globals;

namespace HBBB.GameComponents.Menus
{
    /// <summary>
    /// This is a menu the allows the players to select which board level to battle in
    /// </summary>
    class LevelSelectionMenu : BaseMenu
    {
        private static string MENU_ID = "LEVEL_SELECTION_MENU";
        public static string MenuId { get { return MENU_ID; } }

        double forcedInputWaitTime = 0.0;
        const double FORCED_INPUT_DELAY = 0.25;

        //double flashTime = 5.0;
        //bool showStartToStart = false;

        const int PRESSSTART_X_POSITION = 500;
        const int PRESSSTART_Y_POSITION = 650;

        BoardLevelList builtinlevelsList;
        BoardLevelList levelsList;
        int currentLevelIndex = 0;
        BoardLevelList.BoardLevelInfo currentLevel;
        public BoardLevelList.BoardLevelInfo SelectedLevel { get { return currentLevel; } }

        Texture2D backgroundTexture;
        Texture2D levelSelectLeft;
        Texture2D levelSelectRight;
        Texture2D currentLevelThumb;
        Texture2D arrowLeft;
        Texture2D arrowRight;

        /// <summary>
        /// Construct the Menu
        /// </summary>
        /// <param name="parentSystem"></param>
        public LevelSelectionMenu(Game game, BaseMenuSystem parentSystem)
            : base(game, parentSystem)
        {
            builtinlevelsList = new BoardLevelList();
            levelsList = new BoardLevelList();
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            backgroundTexture = content.Load<Texture2D>(@"W_A_D\Textures\menus\level_select_menu_bkg");
            currentLevelThumb = content.Load<Texture2D>(@"W_A_D\Textures\menus\white_16x16");
            levelSelectLeft = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\trigger_left");
            levelSelectRight = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\trigger_right");
            arrowLeft = content.Load<Texture2D>(@"W_A_D\Textures\arrow_left");
            arrowRight = content.Load<Texture2D>(@"W_A_D\Textures\arrow_right");

            XmlSerializer serializer = new XmlSerializer(typeof(BoardLevelList));
#if DEBUG
            TextReader reader = new StreamReader(@"W_A_D\Boards\TestingBoardsList.xml");
#else
            TextReader reader = new StreamReader(@"W_A_D\Boards\BoardsList.xml");
#endif
            builtinlevelsList = (BoardLevelList)serializer.Deserialize(reader);
            levelsList.BoardLevels.AddRange(builtinlevelsList.BoardLevels);
            LoadCurrentLevel();
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
        /// Called each time the menu is loaded, refresh the custom levels list
        /// </summary>
        /// <param name="args"></param>
        public override void ProcessTransitionArgs(string args)
        {
            parentSystem.StorageDevice = null;
            parentSystem.StorageDeviceResult = Guide.BeginShowStorageDeviceSelector(PlayerIndex.One, null, null);

            base.ProcessTransitionArgs(args);
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadCustomLevels()
        {
            if (parentSystem.StorageDevice == null) return;
            BoardLevelList customLevelsList = new BoardLevelList();
            // loop through the custom storageandlook for levels
            StorageContainer container = parentSystem.StorageDevice.OpenContainer(GlobalStrings.CustomLevelsStorageIdentifier);
            ICollection<string> FileList = Directory.GetFiles(container.Path);
            int index = 1;
            foreach (string filename in FileList)
            {
                BoardLevelList.BoardLevelInfo bli = new BoardLevelList.BoardLevelInfo();
                bli.Filename = filename;
                bli.Name = "Your Custom Level #" + index.ToString();
                bli.Description = filename;
                bli.Description2 = "";
                bli.ThumbnailTextureName = @"W_A_D\Textures\BoardThumbs\custom";
                customLevelsList.BoardLevels.Add(bli);
                index++;
            }
            container.Dispose();

            levelsList.BoardLevels.Clear();
            levelsList.BoardLevels.AddRange(builtinlevelsList.BoardLevels);
            levelsList.BoardLevels.AddRange(customLevelsList.BoardLevels);
        }

        /// <summary>
        /// Draw this screen
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (parentSystem.CurrentMenu != this) return;

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            spriteBatch.Draw(backgroundTexture, new Rectangle(0, 0, this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height), Color.White);

            if (currentLevel != null)
            {
                Color reddish = new Color(200, 55, 50);
                Vector2 size = spriteFont.MeasureString(currentLevel.Name);
                spriteBatch.DrawString(spriteFont, currentLevel.Name, new Vector2(this.GraphicsDevice.Viewport.Width/2 - size.X/2+3, 263), Color.DarkGray); // font shadow
                spriteBatch.DrawString(spriteFont, currentLevel.Name, new Vector2(this.GraphicsDevice.Viewport.Width / 2 - size.X / 2, 260), Color.Black); // the actual text

                size = spriteFont.MeasureString(currentLevel.Description);
                spriteBatch.DrawString(spriteFont, currentLevel.Description, new Vector2(this.GraphicsDevice.Viewport.Width / 2 - size.X / 2+3, 453), Color.DarkGray); // font shadow
                spriteBatch.DrawString(spriteFont, currentLevel.Description, new Vector2(this.GraphicsDevice.Viewport.Width / 2 - size.X / 2, 450), Color.Black); // the actual text

                size = spriteFont.MeasureString(currentLevel.Description2);
                spriteBatch.DrawString(spriteFont, currentLevel.Description2, new Vector2(this.GraphicsDevice.Viewport.Width / 2 - size.X / 2+3, 493), Color.DarkGray); // font shadow
                spriteBatch.DrawString(spriteFont, currentLevel.Description2, new Vector2(this.GraphicsDevice.Viewport.Width / 2 - size.X / 2, 490), Color.Black); // the actual text

                spriteBatch.Draw(arrowLeft, new Rectangle(360, 310, 64, 64), reddish);
                spriteBatch.Draw(levelSelectLeft, new Rectangle(480, 310, 64, 64), Color.White);
                spriteBatch.Draw(currentLevelThumb, new Rectangle(600, 310, 64, 64), Color.White);
                spriteBatch.Draw(levelSelectRight, new Rectangle(720, 310, 64, 64), Color.White);
                spriteBatch.Draw(arrowRight, new Rectangle(840, 310, 64, 64), reddish);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }


        /// <summary>
        /// Update this screen, process input, update vars, etc
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (parentSystem.CurrentMenu != this) return;

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
                        if (parentSystem.StorageDevice.IsConnected) LoadCustomLevels();
                    }
                }
                else if (parentSystem.StorageDevice != null)
                {
                    parentSystem.StorageDeviceResult = null;
                }
            }

            forcedInputWaitTime += gameTime.ElapsedGameTime.TotalSeconds;  // forced delay in gamepad input

            base.Update(gameTime);
        }

        /// <summary>
        /// Handle button clicks
        /// </summary>
        /// <param name="index"></param>
        /// <param name="details"></param>
        public override void OnButtonClick(PlayerIndex index, GamePadButtonEventDetails details)
        {
            if (parentSystem.CurrentMenu != this) return;

            if (details.Button == GamePadWrapper.ButtonId.BACK ||
                details.Button == GamePadWrapper.ButtonId.B)
            {
                GameAudio.PlayCue("back");
                parentSystem.TransitionToMenu(JoinUpMenu.MenuId);
                return;
            }
            else if (details.Button == GamePadWrapper.ButtonId.START ||
                details.Button == GamePadWrapper.ButtonId.A)
            {
                GameAudio.PlayCue("click");
                parentSystem.TransitionToMenu(ShowControlsMenu.MenuId);
                return;
            }
            else if (details.Button == GamePadWrapper.ButtonId.Y)  // secret sudden death menu easter egg
            {
                GameAudio.PlayCue("dink");
                parentSystem.TransitionToMenu(SuddenDeathMenu.MenuId);
            }
            else if (details.Button == GamePadWrapper.ButtonId.LEFT_SHOULDER)
            {
                GameAudio.PlayCue("click");
                if (currentLevelIndex > 0) currentLevelIndex--;
                else currentLevelIndex = levelsList.BoardLevels.Count - 1;
                LoadCurrentLevel();
            }
            else if (details.Button == GamePadWrapper.ButtonId.RIGHT_SHOULDER)
            {
                GameAudio.PlayCue("click");
                if (currentLevelIndex < levelsList.BoardLevels.Count - 1) currentLevelIndex++;
                else currentLevelIndex = 0;
                LoadCurrentLevel();
            }
        }

        /// <summary>
        /// Handle analog activity
        /// </summary>
        /// <param name="index"></param>
        /// <param name="details"></param>
        public override void OnAnalogMovement(PlayerIndex index, GamePadAnalogEventDetails details)
        {
            if (parentSystem.CurrentMenu != this) return;

            if (forcedInputWaitTime < FORCED_INPUT_DELAY) return;
            else forcedInputWaitTime = 0.0;

            if (details.AnalogButton == GamePadWrapper.AnalogId.LEFT_TRIGGER)
            {
                if (details.StickValue.X > 0.0)
                {
                    GameAudio.PlayCue("click");
                    if (currentLevelIndex > 0) currentLevelIndex--;
                    else currentLevelIndex = levelsList.BoardLevels.Count - 1;
                    LoadCurrentLevel();
                }
            }
            else if (details.AnalogButton == GamePadWrapper.AnalogId.RIGHT_TRIGGER)
            {
                if (details.StickValue.X > 0.0)
                {
                    GameAudio.PlayCue("click");
                    if (currentLevelIndex < levelsList.BoardLevels.Count - 1) currentLevelIndex++;
                    else currentLevelIndex = 0;
                    LoadCurrentLevel();
                }
            }
            else if (details.AnalogButton == GamePadWrapper.AnalogId.LEFT_STICK)
            {
                if (details.StickValue.X > 0.1)
                {
                    GameAudio.PlayCue("click");
                    if (currentLevelIndex < levelsList.BoardLevels.Count - 1) currentLevelIndex++;
                    else currentLevelIndex = 0;
                    LoadCurrentLevel();
                }
                else if (details.StickValue.X < -0.1)
                {
                    GameAudio.PlayCue("click");
                    if (currentLevelIndex > 0) currentLevelIndex--;
                    else currentLevelIndex = levelsList.BoardLevels.Count - 1;
                    LoadCurrentLevel();
                }
            }
        }

        /// <summary>
        /// Load current level
        /// </summary>
        private void LoadCurrentLevel()
        {
            currentLevel = levelsList.BoardLevels[currentLevelIndex];
            try
            {
                currentLevelThumb = content.Load<Texture2D>(currentLevel.ThumbnailTextureName);
            }
            catch (System.Exception)
            {
                currentLevelThumb = content.Load<Texture2D>(@"W_A_D\Textures\BoardThumbs\custom");
            }
        }
    }
}
