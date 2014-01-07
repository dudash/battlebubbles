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
// File Created: 09 October 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Storage;
using HBBB.GameComponents.Globals;
using HBBB.GameComponents.PlayerComponents;
using HBBB.GameComponents.BoardComponents;
using HBBB.Core.MassSpring.Verlet;
using System.Diagnostics;

namespace HBBB.GameComponents
{
    /// <summary>
    /// This class contains the current game.  It contains the game state, the game Board, and Player objects.
    /// </summary>
    class GameSession : GameComponent
    {
        public enum SessionState { STATE_PREGAME, STATE_PREPARINGGAME, STATE_INGAME, STATE_PAUSEDGAME, STATE_ENDGAME };
        public enum GameType { GAMETYPE_LOCAL, GAMETYPE_XBOXLIVE};
        public enum WinType { NONE, POINTS_SLOTS_FILLED, POINTS_TIMER_EXPIRED, BOARD_WIN_RULES, SUDDEN_DEATH };

        // for saving and loading custom levels
        public StorageDevice storageDevice;
        public IAsyncResult storageDeviceResult;

        /// <summary>
        /// The game board for this session
        /// </summary>
        private Board board;
        public Board Board
        {
            get { return board; }
        }
        
        public new HBBBGame Game { get { return (HBBBGame)base.Game; } }
        
        // @@ Gonz: This is a kludge that can be removed when Player inherits from GameComponent
        void SetPlayerSpot(ref Player _player, Player value) {
            if (_player == value) return;
            if (_player != null) {
              Game.EntityManager.RemoveEntity(_player);
              Debug.Assert(_player.GameSession == this);
              _player.GameSession = null;
            }
            _player = value;
            if (_player != null) {
              Game.EntityManager.AddEntity(_player);
              Debug.Assert(_player.GameSession == null);
              _player.GameSession = this;
            }
        }

        Player _Player1 = null;
        public Player Player1 { 
            get { return _Player1; } 
            set { SetPlayerSpot(ref _Player1, value); }
        }

        Player _Player2 = null;
        public Player Player2 { 
            get { return _Player2; } 
            set { SetPlayerSpot(ref _Player2, value); }
        }

        Player _Player3 = null;
        public Player Player3 { 
            get { return _Player3; } 
            set { SetPlayerSpot(ref _Player3, value); }
        }

        Player _Player4 = null;
        public Player Player4 { 
            get { return _Player4; } 
            set { SetPlayerSpot(ref _Player4, value); }
        }

        int player1AIType;
        int player2AIType;
        int player3AIType;
        int player4AIType;

        /// <summary>
        /// The path to the selected level (as chosen by the player)
        /// </summary>
        string selectedLevelPath = @"W_A_D\Boards\test_default.lvl";
        public string SelectedLevelPath { set { selectedLevelPath = value; } }
        /// <summary>
        /// The amount of time spent in this session
        /// </summary>
        TimeSpan ellapsedSessionTime;
        public TimeSpan EllapsedSessionTime { get { return ellapsedSessionTime; } }
        /// <summary>
        /// Turn AI on/off
        /// </summary>
        bool aiEnabled = true;
        public bool AiEnabled
        {
            get { return aiEnabled; }
            set { aiEnabled = value; }
        }
        /// <summary>
        /// Text to display at the end of the game
        /// </summary>
        string gameOverText;
        public string GameOverText
        {
            get { return gameOverText; }
            set { gameOverText = value; }
        }
        /// <summary>
        /// Winners of the game
        /// </summary>
        List<Player> winners;
        public List<Player> Winners
        {
            get { return winners; }
            set { winners = value; }
        }
        /// <summary>
        /// The state of the game session
        /// </summary>
        SessionState state;
        public SessionState State
        {
            get { return state; }
        }
        /// <summary>
        /// The type of game to be played
        /// </summary>
        GameType typeOfGame;
        public GameType TypeOfGame
        {
            get { return typeOfGame; }
        }

        /// <summary>
        /// Construct
        /// </summary>
        public GameSession(Game game) : base(game)
        {
            state = SessionState.STATE_PREGAME;
            typeOfGame = GameType.GAMETYPE_LOCAL;
            board = new Board(game);
            ellapsedSessionTime = new TimeSpan(0, 0, 0);
            winners = new List<Player>();
        }

        /// <summary>
        /// Restart the game session
        /// </summary>
        public void InitializeSession()
        {
            _Player1 = null;
            _Player2 = null;
            _Player3 = null;
            _Player4 = null;
            state = SessionState.STATE_PREGAME;
            gameOverText = "";
            if (winners != null) winners.Clear();
        }

        /// <summary>
        /// Add a player to the session
        /// </summary>
        /// <param name="value"></param>
        public void AddPlayer(PlayerIndex index, Player player, int aiType)
        {
            if (state != SessionState.STATE_PREGAME)
            {
                throw new Exception("GameSession::SetPlayerCount - Cannot update game state while game is in progress");
            }
            switch (index)
            {
                case PlayerIndex.One:
                    Player1 = player;
                    player1AIType = aiType;
                    break;
                case PlayerIndex.Two:
                    Player2 = player;
                    player2AIType = aiType;
                    break;
                case PlayerIndex.Three:
                    Player3 = player;
                    player3AIType = aiType;
                    break;
                case PlayerIndex.Four:
                    Player4 = player;
                    player4AIType = aiType;
                    break;
            }
        }

        /// <summary>
        /// Return the coresponding player linked to the argument index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        Player GetPlayer(PlayerIndex index)
        {
            switch (index)
            {
                case PlayerIndex.One:
                    return Player1;
                case PlayerIndex.Two:
                    return Player2;
                case PlayerIndex.Three:
                    return Player3;
                case PlayerIndex.Four:
                    return Player4;
            }
            return null;
        }

        /// <summary>
        /// Set the winner of this game session
        /// </summary>
        public void SetWinner(Player winner, WinType winType, string winTypeString)
        {
            switch (winType)
            {
                case WinType.BOARD_WIN_RULES:
                    // string should be passed into the method
                    break;
                case WinType.POINTS_SLOTS_FILLED:
                    winTypeString = "having the most points";
                    break;
                case WinType.POINTS_TIMER_EXPIRED:
                    winTypeString = "having the most points";
                    break;
                case WinType.SUDDEN_DEATH:
                    winTypeString = "sudden death challenge";
                    break;
            }
            if (winner == null) gameOverText = "              Nobody likes a quitter...";
            else if (winner.PlayerIndex == PlayerIndex.One) gameOverText = "Player 1 Wins by " + winTypeString + "!";
            else if (winner.PlayerIndex == PlayerIndex.Two) gameOverText = "Player 2 Wins by " + winTypeString + "!";
            else if (winner.PlayerIndex == PlayerIndex.Three) gameOverText = "Player 3 Wins by " + winTypeString + "!";
            else if (winner.PlayerIndex == PlayerIndex.Four) gameOverText = "Player 4 Wins by " + winTypeString + "!";

            winners.Clear();
            winners.Add(winner);
        }

        /// <summary>
        /// Goto sudden death to determine the winner
        /// </summary>
        /// <param name="players"></param>
        public void GotoSuddenDeath(List<Player> players)
        {
            gameOverText = "SUDDEN DEATH";
            winners = players;
        }

        /// <summary>
        /// Set the type of the game
        /// </summary>
        /// <param name="value"></param>
        public void SetGameType(GameType value)
        {
            if (state != SessionState.STATE_PREGAME)
            {
                throw new Exception("GameSession::SetGameType - Cannot update game state while game is in progress");
            }
            typeOfGame = value;
        }

        /// <summary>
        /// Ask the session to start
        /// </summary>
        /// <returns></returns>
        public void RequestStartSession()
        {
            state = SessionState.STATE_PREPARINGGAME;
        }

        /// <summary>
        /// Helper funciton to setup the AI
        /// </summary>
        /// <param name="player"></param>
        private void CreatePlayerAI(ref Player player, int aiType)
        {
            player.IsCpu = true;
            IPlayerAIHandler ai = null;
            if (aiType == (int)GlobalResorces.BuiltInPlayerPicsIds.CLOWN) ai = new PlayerAIGonz(this);
            else if (aiType == (int)GlobalResorces.BuiltInPlayerPicsIds.FRANK) ai = new PlayerAIFrank(player.PlayerIndex, this);
            else if (aiType == (int)GlobalResorces.BuiltInPlayerPicsIds.BRIDE) ai = new PlayerAIBride(player.PlayerIndex, this);
            else
            {
                // Dummy do nothing AI
                ai = new PlayerAIBear(player.PlayerIndex,this);
                ai.Enabled = false;
                player.PlayerPic = GlobalResorces.GetPlayerPic((int)GlobalResorces.BuiltInPlayerPicsIds.FLOWER);
                return;
            }
            player.InputHandler = ai;
            ai.Reset();
            ai.Enabled = AiEnabled;
            player.PlayerPic = GlobalResorces.GetPlayerPic(aiType);
        }

        /// <summary>
        /// set the CPU and other players up
        /// </summary>
        public void CreatePlayers()
        {
            // if player is null create a new CPU player
            if (Player1 == null)
            {
                Player1 = new Player(PlayerIndex.One);
                CreatePlayerAI(ref _Player1, player1AIType);
            }
            else
            {
                PlayerInputHandler ih = new PlayerInputHandler(PlayerIndex.One);
                Player1.InputHandler = ih;
                SignedInGamer gamer = Gamer.SignedInGamers[PlayerIndex.One];
                if (gamer != null) Player1.PlayerPic = gamer.GetProfile().GamerPicture;
                else Player1.PlayerPic = GlobalResorces.GetRandomPlayerPic();
            }
            if (Player2 == null)
            {
                Player2 = new Player(PlayerIndex.Two);
                CreatePlayerAI(ref _Player2, player2AIType);
            }
            else
            {
                PlayerInputHandler ih = new PlayerInputHandler(PlayerIndex.Two);
                Player2.InputHandler = ih;
                SignedInGamer gamer = Gamer.SignedInGamers[PlayerIndex.Two];
                if (gamer != null) Player2.PlayerPic = gamer.GetProfile().GamerPicture;
                else Player2.PlayerPic = GlobalResorces.GetRandomPlayerPic();
            }
            if (Player3 == null)
            {
                Player3 = new Player(PlayerIndex.Three);
                CreatePlayerAI(ref _Player3, player3AIType);
            }
            else
            {
                PlayerInputHandler ih = new PlayerInputHandler(PlayerIndex.Three);
                Player3.InputHandler = ih;
                SignedInGamer gamer = Gamer.SignedInGamers[PlayerIndex.Three];
                if (gamer != null) Player3.PlayerPic = gamer.GetProfile().GamerPicture;
                else Player3.PlayerPic = GlobalResorces.GetRandomPlayerPic();
            }
            if (Player4 == null)
            {
                Player4 = new Player(PlayerIndex.Four);
                CreatePlayerAI(ref _Player4, player4AIType);
            }
            else
            {
                PlayerInputHandler ih = new PlayerInputHandler(PlayerIndex.Four);
                Player4.InputHandler = ih;
                SignedInGamer gamer = Gamer.SignedInGamers[PlayerIndex.Four];
                if (gamer != null) Player4.PlayerPic = gamer.GetProfile().GamerPicture;
                else Player4.PlayerPic = GlobalResorces.GetRandomPlayerPic();
            }
        }

        /// <summary>
        /// Move to ingame state
        /// </summary>
        public void StartSession()
        {            
            if (typeOfGame == GameType.GAMETYPE_XBOXLIVE)
            {
                // TODO check to see if all player have xbox live privlidges
            }

            if (PrepareBoardForGame() == false)
            {
                throw new Exception("GameSession::StartSession - could not prepare board for game");
            }

            // setup player 1
            Player1.Initialize();
            Player1.InitPlayerBubbleProperties(board.CurrentLevel.Player1BubbleSegments,
                board.CurrentLevel.Player1BubbleForce, board.CurrentLevel.Player1BubbleInnerForce,
                board.CurrentLevel.Player1OuterWidth, board.CurrentLevel.Player1InnerWidth);
            Player1.CreatePlayerBubble(board.CurrentLevel.Player1SpawnPoint);
            PreparePlayerForGame(Player1);

#if !DEBUG_PHYSICS
            // setup player 2
            Player2.Initialize();
            Player2.InitPlayerBubbleProperties(board.CurrentLevel.Player2BubbleSegments,
                board.CurrentLevel.Player2BubbleForce, board.CurrentLevel.Player2BubbleInnerForce,
                board.CurrentLevel.Player2OuterWidth, board.CurrentLevel.Player2InnerWidth);
            Player2.CreatePlayerBubble(board.CurrentLevel.Player2SpawnPoint);
            PreparePlayerForGame(Player2);
            // setup player 3
            Player3.Initialize();
            Player3.InitPlayerBubbleProperties(board.CurrentLevel.Player3BubbleSegments,
                board.CurrentLevel.Player3BubbleForce, board.CurrentLevel.Player3BubbleInnerForce,
                board.CurrentLevel.Player3OuterWidth, board.CurrentLevel.Player3InnerWidth);
            Player3.CreatePlayerBubble(board.CurrentLevel.Player3SpawnPoint);
            PreparePlayerForGame(Player3);
            // setup player 4
            Player4.Initialize();
            Player4.InitPlayerBubbleProperties(board.CurrentLevel.Player4BubbleSegments,
                board.CurrentLevel.Player4BubbleForce, board.CurrentLevel.Player4BubbleInnerForce,
                board.CurrentLevel.Player4OuterWidth, board.CurrentLevel.Player4InnerWidth);
            Player4.CreatePlayerBubble(board.CurrentLevel.Player4SpawnPoint);
            PreparePlayerForGame(Player4);
#endif

            winners.Clear();
            board.InGameFlag = true;
            state = SessionState.STATE_INGAME;
            //GameAudio.PlayMusic(board.CurrentLevel.MusicCueName);
        }

        /// <summary>
        /// Exit the current game session
        /// </summary>
        public void EndSession()
        {
            //GameAudio.StopMusic(board.CurrentLevel.MusicCueName);
            GameAudio.SetMusicVolume(1.0f);
            board.InGameFlag = false;
            state = SessionState.STATE_ENDGAME;

            // turn off any vibrations
            GamePad.SetVibration(Player1.PlayerIndex, 0.0f, 0.0f);
            GamePad.SetVibration(Player2.PlayerIndex, 0.0f, 0.0f);
            GamePad.SetVibration(Player3.PlayerIndex, 0.0f, 0.0f);
            GamePad.SetVibration(Player4.PlayerIndex, 0.0f, 0.0f);

            // turn off boost viz
            Player1.IsBoostingSpeed = false;
            Player2.IsBoostingSpeed = false;
            Player3.IsBoostingSpeed = false;
            Player4.IsBoostingSpeed = false;

            ellapsedSessionTime = new TimeSpan(0, 0, 0);
            CleanUpBoardAfterGame();
        }

        /// <summary>
        /// Move to a paused state
        /// </summary>
        public void PauseSession()
        {
            GameAudio.SetMusicVolume(0.3f);
            state = SessionState.STATE_PAUSEDGAME;
            board.InGameFlag = false;

            // turn off any vibrations
            GamePad.SetVibration(Player1.PlayerIndex, 0.0f, 0.0f);
            GamePad.SetVibration(Player2.PlayerIndex, 0.0f, 0.0f);
            GamePad.SetVibration(Player3.PlayerIndex, 0.0f, 0.0f);
            GamePad.SetVibration(Player4.PlayerIndex, 0.0f, 0.0f);

            // turn off boost viz
            Player1.IsBoostingSpeed = false;
            Player2.IsBoostingSpeed = false;
            Player3.IsBoostingSpeed = false;
            Player4.IsBoostingSpeed = false;
        }

        /// <summary>
        /// Move to a puased state
        /// </summary>
        public void UnpauseSession()
        {
            GameAudio.SetMusicVolume(1.0f);
            state = SessionState.STATE_INGAME;
            board.InGameFlag = true;
        }

        /// <summary>
        /// Update the session
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (state == SessionState.STATE_INGAME)
                ellapsedSessionTime = ellapsedSessionTime.Add(gameTime.ElapsedGameTime);
        }

        /// <summary>
        /// Init the board for this game
        /// </summary>
        private bool PrepareBoardForGame()
        {
            board.PlayersList.Add(PlayerIndex.One, Player1);
            board.PlayersList.Add(PlayerIndex.Two, Player2);
            board.PlayersList.Add(PlayerIndex.Three, Player3);
            board.PlayersList.Add(PlayerIndex.Four, Player4);
            return board.LoadLevel(selectedLevelPath, storageDevice);
        }

        /// <summary>
        /// Clean up any ephermeral game data
        /// </summary>
        private void CleanUpBoardAfterGame()
        {
            // TODO players aren't deleting.  They get recreated and the memory builds up!
            // need to figure out why .NET isn't cleaning up player objects?

            board.PlayersList.Clear();
        }

        /// <summary>
        /// init the play for this game session
        /// </summary>
        /// <param name="player"></param>
        private void PreparePlayerForGame(Player player)
        {
            // clear out the old statistics
            player.Statistics.Clear();

            // Constrain the outer circle of the player
            List<VerletPoint> verletPoints = player.Bubble.PointsList;
            for (int thisPoint = 0; thisPoint < verletPoints.Count; thisPoint++)
            {
                int nextPoint = (thisPoint + 1) % player.Bubble.Segments;  // modulo so that the last index will wrap around to the first
                verletPoints[thisPoint].AddCollisionConstraint(new VerletPointToPlayZoneCollision(board.GetBoardPlayZoneDimensions()));
                // Add a constraint to the outer ring point so it cannot penetrate world objects
                verletPoints[thisPoint].AddCollisionConstraint(new VerletPointToBoardCollision(board.Obstructions)); // pass in board collision data
                // Add a constraint to the outer ring line segment so it cannot penetrate world objects
                verletPoints[thisPoint].AddCollisionLSConstraint(new VerletLineToBoardCollision(board.Obstructions, verletPoints[nextPoint]));        // pass in board collision data
            }
        }
    }
}
