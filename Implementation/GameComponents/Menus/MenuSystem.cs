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
// File Created: 14 October 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using HBBB.Core.Menus;
using HBBB.Core.Input;
using HBBB.GameComponents.LevelBuilder;

namespace HBBB.GameComponents.Menus
{
    /// <summary>
    /// A MenuSystem provides a series of menus and a mechanisim to switch between
    /// them based upon commands from the currecntly executing menu.
    /// </summary>
    class MenuSystem : BaseMenuSystem
    {
        /// <summary>
        /// pointer to the master game session
        /// </summary>
        private GameSession session;

        TitleMenu titleMenu;
        QuitMenu quitMenu;
        MainMenu mainmenu;
        OptionsMenu opmenu;
        Options2Menu opmenu2;
        Options3Menu opmenu3;
        JoinUpMenu jupmenu;
        JoinUpAdvancedMenu jupadvmenu;
        LevelSelectionMenu levSelectMenu;
        PauseMenu pausemenu;
        GameOverMenu gameovermenu;
        CreditsMenu creditsMenu;
        ShowControlsMenu showControlsMenu;
        SuddenDeathMenu suddenDeathMenu;
        CustomLevelsMenu customLevelsMenu;
        EditLevelMenu editLevelMenu;

        GamePadsWrapper gamepads;
        public GamePadsWrapper GamePads { get { return gamepads; } }

        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="game"></param>
        public MenuSystem(Game game, GameSession session) : base(game)
        {
            this.session = session;

            titleMenu = new TitleMenu(game, this);
            menus.Add(TitleMenu.MenuId, titleMenu);
            game.Components.Add(titleMenu);

            quitMenu = new QuitMenu(game, this);
            menus.Add(QuitMenu.MenuId, quitMenu);
            game.Components.Add(quitMenu);

            mainmenu = new MainMenu(game, this);
            menus.Add(MainMenu.MenuId, mainmenu);
            game.Components.Add(mainmenu);

            opmenu = new OptionsMenu(game, this);
            menus.Add(OptionsMenu.MenuId, opmenu);
            game.Components.Add(opmenu);

            opmenu2 = new Options2Menu(game, this);
            menus.Add(Options2Menu.MenuId, opmenu2);
            game.Components.Add(opmenu2);

            opmenu3 = new Options3Menu(game, this);
            menus.Add(Options3Menu.MenuId, opmenu3);
            game.Components.Add(opmenu3);

            jupmenu = new JoinUpMenu(game, this);
            menus.Add(JoinUpMenu.MenuId, jupmenu);
            game.Components.Add(jupmenu);

            jupadvmenu = new JoinUpAdvancedMenu(game, this);
            menus.Add(JoinUpAdvancedMenu.MenuId, jupadvmenu);
            game.Components.Add(jupadvmenu);

            levSelectMenu = new LevelSelectionMenu(game, this);
            menus.Add(LevelSelectionMenu.MenuId, levSelectMenu);
            game.Components.Add(levSelectMenu);

            pausemenu = new PauseMenu(game, this);
            menus.Add(PauseMenu.MenuId, pausemenu);
            game.Components.Add(pausemenu);

            gameovermenu = new GameOverMenu(game, this);
            menus.Add(GameOverMenu.MenuId, gameovermenu);
            game.Components.Add(gameovermenu);

            creditsMenu = new CreditsMenu(game, this);
            menus.Add(CreditsMenu.MenuId, creditsMenu);
            game.Components.Add(creditsMenu);

            showControlsMenu = new ShowControlsMenu(game, this);
            menus.Add(ShowControlsMenu.MenuId, showControlsMenu);
            game.Components.Add(showControlsMenu);

            suddenDeathMenu = new SuddenDeathMenu(game, this);
            menus.Add(SuddenDeathMenu.MenuId, suddenDeathMenu);
            game.Components.Add(suddenDeathMenu);

            customLevelsMenu = new CustomLevelsMenu(game, this);
            menus.Add(CustomLevelsMenu.MenuId, customLevelsMenu);
            game.Components.Add(customLevelsMenu);

            editLevelMenu = new EditLevelMenu(game, this);
            menus.Add(EditLevelMenu.MenuId, editLevelMenu);
            game.Components.Add(editLevelMenu);

            gamepads = new GamePadsWrapper(game);
            gamepads.ButtonClicked += new GamePadsWrapper.ButtonClickEventHandler(this.OnButtonClick);
            gamepads.AnalogMoved += new GamePadsWrapper.AnalogMoveEventHandler(this.OnAnalogMovement);
        }

        /// <summary>
        /// Transition to a new menu special case processing
        /// </summary>
        /// <param name="menuId"></param>
        public override void TransitionToMenu(string menuId, string args)
        {
            if (!menus.ContainsKey(menuId)) return;
            ProcessTransitionSpecialCases(menuId, args);
            base.TransitionToMenu(menuId, args);
        }

        /// <summary>
        /// Transition to a new menu special case processing
        /// </summary>
        /// <param name="newMenu"></param>
        public override void TransitionToMenu(string menuId)
        {
            if (!menus.ContainsKey(menuId)) return;
            ProcessTransitionSpecialCases(menuId, "");
            base.TransitionToMenu(menuId);
        }

        /// <summary>
        /// Handle special cases
        /// </summary>
        /// <param name="menuId"></param>
        /// <param name="args"></param>
        private void ProcessTransitionSpecialCases(string menuId, string args)
        {
            // reset the game session if last menu was the gameover menu
            if (currentMenu == gameovermenu && menuId == MainMenu.MenuId)
            {
                jupmenu.ResetPlayers();
                session.InitializeSession();
            }
            else if (menuId == GameOverMenu.MenuId && currentMenu == suddenDeathMenu)
            {
                session.SetWinner(suddenDeathMenu.Winner, GameSession.WinType.SUDDEN_DEATH, "");
                gameovermenu.GameOverText = session.GameOverText;
                gameovermenu.p1 = session.Player1;
                gameovermenu.p2 = session.Player2;
                gameovermenu.p3 = session.Player3;
                gameovermenu.p4 = session.Player4;
                gameovermenu.winner = suddenDeathMenu.Winner;
            }
            else if (menuId == GameOverMenu.MenuId)
            {
                gameovermenu.GameOverText = session.GameOverText;
                gameovermenu.p1 = session.Player1;
                gameovermenu.p2 = session.Player2;
                gameovermenu.p3 = session.Player3;
                gameovermenu.p4 = session.Player4;
                gameovermenu.winner = session.Winners[0]; // the 1st is the winner
            }
            else if (menuId == SuddenDeathMenu.MenuId)
            {
                if (session.Winners.Count == 0)
                {
                    // can can secretly play from the level selection menu
                    suddenDeathMenu.SetupDeathClick(session.Player1, session.Player2, session.Player3, session.Player4);
                }
                else suddenDeathMenu.SetupDeathClick(session.Winners);
            }
        }

        /// <summary>
        /// JUP menu is dene, create the players
        /// </summary>
        public override void RequestCreatePlayers()
        {
            session.AddPlayer(PlayerIndex.One, jupmenu.Player1, jupadvmenu.p1AI);
            session.AddPlayer(PlayerIndex.Two, jupmenu.Player2, jupadvmenu.p2AI);
            session.AddPlayer(PlayerIndex.Three, jupmenu.Player3, jupadvmenu.p3AI);
            session.AddPlayer(PlayerIndex.Four, jupmenu.Player4, jupadvmenu.p4AI);
            session.CreatePlayers();
        }

        /// <summary>
        /// User wants to start the game
        /// </summary>
        public override void RequestStartSession()
        {
            currentMenu = null;  // turn off menus
            //menuMusicLoop.Stop(AudioStopOptions.Immediate);
            if (levSelectMenu.SelectedLevel != null) session.SelectedLevelPath = levSelectMenu.SelectedLevel.Filename;
            session.RequestStartSession();
        }

        /// <summary>
        /// User wants to quit the game
        /// </summary>
        public override void RequestEndSession()
        {
            session.SetWinner(null, GameSession.WinType.NONE, "");
            session.EndSession();
            gameovermenu.GameOverText = session.GameOverText;
        }

        /// <summary>
        /// User wants to pause the game
        /// </summary>
        public override void ShowPauseMenu(PlayerIndex pausingPlayer)
        {
            pausemenu.SetPausingPlayer(pausingPlayer);
            currentMenu = pausemenu;
        }

        /// <summary>
        /// User wants to pause the game
        /// </summary>
        public override void HidePauseMenu()
        {
            session.UnpauseSession();
            currentMenu = null;
        }

        /// <summary>
        /// User wants to quit out completely
        /// </summary>
        public override void RequestQuitGame()
        {
            this.Game.Exit();
        }

        /// <summary>
        /// Update the menu system
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// Tell the menus about button clicks
        /// </summary>
        /// <param name="index"></param>
        /// <param name="details"></param>
        public void OnButtonClick(PlayerIndex index, GamePadButtonEventDetails details)
        {
            if (currentMenu != null) currentMenu.OnButtonClick(index, details);
        }

        /// <summary>
        /// Tell the menus about analog activity
        /// </summary>
        /// <param name="index"></param>
        /// <param name="details"></param>
        public void OnAnalogMovement(PlayerIndex index, GamePadAnalogEventDetails details)
        {
            if (currentMenu != null) currentMenu.OnAnalogMovement(index, details);
        }
    }
}
