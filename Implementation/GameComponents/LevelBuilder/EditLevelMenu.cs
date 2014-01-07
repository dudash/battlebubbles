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
using Microsoft.Xna.Framework.GamerServices;

namespace HBBB.GameComponents.LevelBuilder
{
    /// <summary>
    /// The menu to show board and allow editing it
    /// </summary>
    class EditLevelMenu : BaseMenu
    {
        private static string MENU_ID = "EDIT_LEVEL_MENU";
        public static string MenuId { get { return MENU_ID; } }

        double forcedInputWaitTime = 0.0;
        const double FORCED_INPUT_DELAY = 0.2;

        bool unsavedChangesPopupVisible = false;

        enum MenuOption { DEAD_SLOT = 0, P1_SLOT, P2_SLOT, P3_SLOT, P4_SLOT,
            SOURCE_SLOT, BUB_STICKY_SLOT, SOLID_STICKY_SLOT, ALL_STICKY_SLOT,
            X2_SLOT, X3_SLOT, X5_SLOT }
        MenuOption currentOption = MenuOption.DEAD_SLOT;
        Texture2D subMenuBkgTexture;
        Texture2D subMenuItemTexture;
        Texture2D controlsInfoTexture;
        Texture2D bToCancel;
        Texture2D confirmExitTexture;
        Rectangle DEAD_SLOT_POS = new Rectangle(350, 225, 50, 50);
        Rectangle P1_SLOT_POS = new Rectangle(350, 275, 50, 50);
        Rectangle P2_SLOT_POS = new Rectangle(350, 325, 50, 50);
        Rectangle P3_SLOT_POS = new Rectangle(350, 375, 50, 50);
        Rectangle P4_SLOT_POS = new Rectangle(350, 425, 50, 50);
        Rectangle SOURCE_SLOT_POS = new Rectangle(350, 475, 50, 50);
        Rectangle BUB_STICKY_SLOT_POS = new Rectangle(600, 225, 50, 50);
        Rectangle SOLID_STICKY_SLOT_POS = new Rectangle(600, 275, 50, 50);
        Rectangle ALL_STICKY_SLOT_POS = new Rectangle(600, 325, 50, 50);
        Rectangle X2_SLOT_POS = new Rectangle(600, 375, 50, 50);
        Rectangle X3_SLOT_POS = new Rectangle(600, 425, 50, 50);
        Rectangle X5_SLOT_POS = new Rectangle(600, 475, 50, 50);
        SpriteFont smallerFont;

        string boardFileName;
        LevelBuilderBoard board;
        bool dirtyFlag = false;
        bool subMenuVisibleFlag = false;
        int subMenuSelection = 0;

        /// <summary>
        /// Construct the main menu
        /// </summary>
        /// <param name="parentSystem"></param>
        public EditLevelMenu(Game game, BaseMenuSystem parentSystem)
            : base(game, parentSystem)
        {
            board = new LevelBuilderBoard(game);
            boardFileName = @"W_A_D\Boards\empty.lvl";
        }

        /// <summary>
        /// process the args
        /// </summary>
        /// <param name="args"></param>
        public override void ProcessTransitionArgs(string args)
        {
            boardFileName = args;
            subMenuVisibleFlag = false;
            unsavedChangesPopupVisible = false;
            board.LoadLevel(boardFileName, parentSystem.StorageDevice);
            dirtyFlag = false;
            base.ProcessTransitionArgs(args);
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            subMenuBkgTexture = content.Load<Texture2D>(@"W_A_D\Textures\menus\hexbacking_1_menu");
            subMenuItemTexture = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\button_a");
            controlsInfoTexture = content.Load<Texture2D>(@"W_A_D\Textures\menus\leveleditcontrols");
            bToCancel = content.Load<Texture2D>(@"W_A_D\Textures\Buttons\button_b");
            confirmExitTexture = content.Load<Texture2D>(@"W_A_D\Textures\menus\unsaved_changes_menu");
            smallerFont = content.Load<SpriteFont>(@"W_A_D\Fonts\Arial 14");

            board.gameBackground = content.Load<Texture2D>(@"W_A_D\Textures\game_background");
            board.slotSelectorTexture = content.Load<Texture2D>(@"W_A_D\Textures\slot_vertical");
            board.myWidth = this.GraphicsDevice.Viewport.Width;
            board.myHeight = this.GraphicsDevice.Viewport.Height;
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
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (parentSystem.CurrentMenu != this) return;

            // draw board
            board.DrawBoard(gameTime, spriteBatch, smallerFont);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            // draw filename up top
            string editingString = "Editing: " + boardFileName;
            if (dirtyFlag) editingString += " *";
            spriteBatch.DrawString(spriteFont, editingString, new Vector2(174, 24), Color.Black);
            spriteBatch.DrawString(spriteFont, editingString, new Vector2(170, 20), Color.Orange);

            if (unsavedChangesPopupVisible)
            {
                spriteBatch.Draw(confirmExitTexture, new Rectangle(this.GraphicsDevice.Viewport.Width / 2 - confirmExitTexture.Width / 2,
                    this.GraphicsDevice.Viewport.Height / 2 - confirmExitTexture.Height / 2,
                    confirmExitTexture.Width, confirmExitTexture.Height), Color.White);
            }
            else if (subMenuVisibleFlag)
            {
                spriteBatch.Draw(subMenuBkgTexture, new Rectangle(this.GraphicsDevice.Viewport.Width / 4, 150, this.GraphicsDevice.Viewport.Width / 2, this.GraphicsDevice.Viewport.Height / 2 + 100), Color.White);
                spriteBatch.DrawString(spriteFont, "Choose slot type to insert:", new Vector2(this.GraphicsDevice.Viewport.Width / 4 + 75 + 4, 155 + 4), Color.White);
                spriteBatch.DrawString(spriteFont, "Choose slot type to insert:", new Vector2(this.GraphicsDevice.Viewport.Width / 4 + 75, 155), Color.Black);

                // draw selection items
                spriteBatch.DrawString(spriteFont, "Sticky (Both)", new Vector2(ALL_STICKY_SLOT_POS.X + 50, ALL_STICKY_SLOT_POS.Y), Color.Black);
                spriteBatch.DrawString(spriteFont, "Sticky (Bubbles)", new Vector2(BUB_STICKY_SLOT_POS.X + 50, BUB_STICKY_SLOT_POS.Y), Color.Black);
                spriteBatch.DrawString(spriteFont, "Dead Slot", new Vector2(DEAD_SLOT_POS.X + 50, DEAD_SLOT_POS.Y), Color.Black);
                spriteBatch.DrawString(spriteFont, "Player 1 Base", new Vector2(P1_SLOT_POS.X + 50, P1_SLOT_POS.Y), Color.Black);
                spriteBatch.DrawString(spriteFont, "Player 2 Base", new Vector2(P2_SLOT_POS.X + 50, P2_SLOT_POS.Y), Color.Black);
                spriteBatch.DrawString(spriteFont, "Player 3 Base", new Vector2(P3_SLOT_POS.X + 50, P3_SLOT_POS.Y), Color.Black);
                spriteBatch.DrawString(spriteFont, "Player 4 Base", new Vector2(P4_SLOT_POS.X + 50, P4_SLOT_POS.Y), Color.Black);
                spriteBatch.DrawString(spriteFont, "Sticky (Solids)", new Vector2(SOLID_STICKY_SLOT_POS.X + 50, SOLID_STICKY_SLOT_POS.Y), Color.Black);
                spriteBatch.DrawString(spriteFont, "Source Slot", new Vector2(SOURCE_SLOT_POS.X + 50, SOURCE_SLOT_POS.Y), Color.Black);
                spriteBatch.DrawString(spriteFont, "Bonus x2", new Vector2(X2_SLOT_POS.X + 50, X2_SLOT_POS.Y), Color.Black);
                spriteBatch.DrawString(spriteFont, "Bonus x3", new Vector2(X3_SLOT_POS.X + 50, X3_SLOT_POS.Y), Color.Black);
                spriteBatch.DrawString(spriteFont, "Bonus x5", new Vector2(X5_SLOT_POS.X + 50, X5_SLOT_POS.Y), Color.Black);

                spriteBatch.Draw(bToCancel, new Rectangle(this.GraphicsDevice.Viewport.Width / 2, this.GraphicsDevice.Viewport.Height / 2 + 190, 50, 50), Color.White);
                spriteBatch.DrawString(spriteFont, "Cancel insert", new Vector2(this.GraphicsDevice.Viewport.Width / 2 + bToCancel.Width, this.GraphicsDevice.Viewport.Height / 2 + 194), Color.White);
                spriteBatch.DrawString(spriteFont, "Cancel insert", new Vector2(this.GraphicsDevice.Viewport.Width / 2 + bToCancel.Width, this.GraphicsDevice.Viewport.Height / 2 + 190), Color.Black);


                // draw sub-menu current item indicator
                switch (subMenuSelection)
                {
                    case (int)MenuOption.ALL_STICKY_SLOT:
                        spriteBatch.Draw(subMenuItemTexture, ALL_STICKY_SLOT_POS, Color.White);
                        break;
                    case (int)MenuOption.BUB_STICKY_SLOT:
                        spriteBatch.Draw(subMenuItemTexture, BUB_STICKY_SLOT_POS, Color.White);
                        break;
                    case (int)MenuOption.DEAD_SLOT:
                        spriteBatch.Draw(subMenuItemTexture, DEAD_SLOT_POS, Color.White);
                        break;
                    case (int)MenuOption.P1_SLOT:
                        spriteBatch.Draw(subMenuItemTexture, P1_SLOT_POS, Color.White);
                        break;
                    case (int)MenuOption.P2_SLOT:
                        spriteBatch.Draw(subMenuItemTexture, P2_SLOT_POS, Color.White);
                        break;
                    case (int)MenuOption.P3_SLOT:
                        spriteBatch.Draw(subMenuItemTexture, P3_SLOT_POS, Color.White);
                        break;
                    case (int)MenuOption.P4_SLOT:
                        spriteBatch.Draw(subMenuItemTexture, P4_SLOT_POS, Color.White);
                        break;
                    case (int)MenuOption.SOLID_STICKY_SLOT:
                        spriteBatch.Draw(subMenuItemTexture, SOLID_STICKY_SLOT_POS, Color.White);
                        break;
                    case (int)MenuOption.SOURCE_SLOT:
                        spriteBatch.Draw(subMenuItemTexture, SOURCE_SLOT_POS, Color.White);
                        break;
                    case (int)MenuOption.X2_SLOT:
                        spriteBatch.Draw(subMenuItemTexture, X2_SLOT_POS, Color.White);
                        break;
                    case (int)MenuOption.X3_SLOT:
                        spriteBatch.Draw(subMenuItemTexture, X3_SLOT_POS, Color.White);
                        break;
                    case (int)MenuOption.X5_SLOT:
                        spriteBatch.Draw(subMenuItemTexture, X5_SLOT_POS, Color.White);
                        break;
                }
            }
            else
            {
                // board editing controls info
                spriteBatch.Draw(controlsInfoTexture, new Rectangle(this.GraphicsDevice.Viewport.Width / 2 - controlsInfoTexture.Width / 2, this.GraphicsDevice.Viewport.Height - 75, 750, 50), Color.White);
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

            base.Update(gameTime);
        }

        /// <summary>
        /// When a button is clicked
        /// </summary>
        /// <param name="details"></param>
        public override void OnButtonClick(PlayerIndex index, GamePadButtonEventDetails details)
        {
            if (parentSystem.CurrentMenu != this) return;

            // *************** UNSAVED EXIT POPUP
            if (unsavedChangesPopupVisible)
            {
                // A to accept
                if (details.Button == GamePadWrapper.ButtonId.A)
                {
                    dirtyFlag = false;
                    unsavedChangesPopupVisible = false;
                    GameAudio.PlayCue("back");
                    parentSystem.TransitionToMenu(CustomLevelsMenu.MenuId);
                }
                else if (details.Button == GamePadWrapper.ButtonId.B)
                {
                    // don't exit
                    unsavedChangesPopupVisible = false;
                }
            }
            // **************** SUBMENU BUTTONS ***
            else if (subMenuVisibleFlag)
            {
                // A to accept
                if (details.Button == GamePadWrapper.ButtonId.A)
                {
                    // commit selection based on subMenuSelection
                    switch (subMenuSelection)
                    {
                        case (int)MenuOption.ALL_STICKY_SLOT:
                            board.SetCurrentSlot_BubbleAndSolidStickyBlock();
                            break;
                        case (int)MenuOption.BUB_STICKY_SLOT:
                            board.SetCurrentSlot_BubbleStickyBlock();
                            break;
                        case (int)MenuOption.DEAD_SLOT:
                            board.SetCurrentSlot_Dead();
                            break;
                        case (int)MenuOption.P1_SLOT:
                            board.SetCurrentSlot_P1();
                            break;
                        case (int)MenuOption.P2_SLOT:
                            board.SetCurrentSlot_P2();
                            break;
                        case (int)MenuOption.P3_SLOT:
                            board.SetCurrentSlot_P3();
                            break;
                        case (int)MenuOption.P4_SLOT:
                            board.SetCurrentSlot_P4();
                            break;
                        case (int)MenuOption.SOLID_STICKY_SLOT:
                            board.SetCurrentSlot_SolidStickyBlock();
                            break;
                        case (int)MenuOption.SOURCE_SLOT:
                            board.SetCurrentSlot_Source();
                            break;
                        case (int)MenuOption.X2_SLOT:
                            board.SetCurrentSlot_BonusPoints(2);
                            break;
                        case (int)MenuOption.X3_SLOT:
                            board.SetCurrentSlot_BonusPoints(3);
                            break;
                        case (int)MenuOption.X5_SLOT:
                            board.SetCurrentSlot_BonusPoints(5);
                            break;
                    }
                    subMenuVisibleFlag = false; // hide menu
                    dirtyFlag = true;
                }
                // B to cancel submenu
                else if (details.Button == GamePadWrapper.ButtonId.B) subMenuVisibleFlag = false;

                // DPAD
                else if (details.Button == GamePadWrapper.ButtonId.D_DOWN)
                {
                    subMenuSelection++;
                    if (subMenuSelection > 11) subMenuSelection = 0;
                }
                else if (details.Button == GamePadWrapper.ButtonId.D_UP)
                {
                    subMenuSelection--;
                    if (subMenuSelection < 0) subMenuSelection = 11;
                }
                else if (details.Button == GamePadWrapper.ButtonId.D_LEFT)
                {
                    subMenuSelection -= 6;
                    if (subMenuSelection < 0) subMenuSelection += 6;
                }
                else if (details.Button == GamePadWrapper.ButtonId.D_RIGHT)
                {
                    subMenuSelection += 6;
                    if (subMenuSelection > 11) subMenuSelection -= 6;
                }

                return;
            }

            // ***************** NORMAL MODE ***
            if (details.Button == GamePadWrapper.ButtonId.BACK)
            {
                if (dirtyFlag)
                {
                    unsavedChangesPopupVisible = true;
                    return;
                }
                GameAudio.PlayCue("back");
                parentSystem.TransitionToMenu(CustomLevelsMenu.MenuId);
                return;
            }
            else if (details.Button == GamePadWrapper.ButtonId.START)
            {
                // TODO ask for a filename open keyboard instead of autonaming

                // save the board
                board.SaveLevel(boardFileName, parentSystem.StorageDevice);
                dirtyFlag = false;
            }
            // A to show sub menu and add a new item
            else if (details.Button == GamePadWrapper.ButtonId.A)
            {
                subMenuVisibleFlag = true;
            }
            // B to remove selected item
            else if (details.Button == GamePadWrapper.ButtonId.B)
            {
                board.ClearCurrentSlot();
                dirtyFlag = true;
            }

            // dpad to move selection
            if (details.Button == GamePadWrapper.ButtonId.D_DOWN) board.SelectSlotDown();
            if (details.Button == GamePadWrapper.ButtonId.D_UP) board.SelectSlotUp();
            if (details.Button == GamePadWrapper.ButtonId.D_RIGHT) board.SelectSlotRight();
            if (details.Button == GamePadWrapper.ButtonId.D_LEFT) board.SelectSlotLeft();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="details"></param>
        public override void OnAnalogMovement(PlayerIndex index, GamePadAnalogEventDetails details)
        {
            if (parentSystem.CurrentMenu != this) return;

            if (forcedInputWaitTime < FORCED_INPUT_DELAY) return;
            else forcedInputWaitTime = 0.0;

            // ************** sub menu analog input
            if (subMenuVisibleFlag)
            {
                if (details.StickValue.Y < 0.0)
                {
                    subMenuSelection++;
                    if (subMenuSelection > 11) subMenuSelection = 0;
                }
                else if (details.StickValue.Y > 0.0)
                {
                    subMenuSelection--;
                    if (subMenuSelection < 0) subMenuSelection = 11;
                }
                else if (details.StickValue.X < 0.0)
                {
                    subMenuSelection -= 6;
                    if (subMenuSelection < 0) subMenuSelection += 6;
                }
                else if (details.StickValue.X > 0.0)
                {
                    subMenuSelection += 6;
                    if (subMenuSelection > 11) subMenuSelection -= 6;
                }
                return;
            }

            // move selection
            if (details.StickValue.Y < 0.0) board.SelectSlotDown();
            if (details.StickValue.Y > 0.0) board.SelectSlotUp();
            if (details.StickValue.X > 0.0) board.SelectSlotRight();
            if (details.StickValue.X < 0.0) board.SelectSlotLeft();
        }
    }
}
