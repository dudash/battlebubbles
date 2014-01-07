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
// File Created: 11 April 2008, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HBBB.Core.Input
{
    /// <summary>
    /// This class instantiates all the XBOX360 gamepads wrappers
    /// and emits event callbacks whenever any gamepad state changes.
    /// </summary>
    class GamePadsWrapper
    {
        public delegate void DisconnectedEventHandler(PlayerIndex index, GamePadEventDetails e);
        public event DisconnectedEventHandler Disconnected;
        public delegate void ConnectedEventHandler(PlayerIndex index, GamePadEventDetails e);
        public event ConnectedEventHandler Connected;

        public delegate void ButtonClickEventHandler(PlayerIndex index, GamePadButtonEventDetails e);
        public event ButtonClickEventHandler ButtonClicked;
        public delegate void ButtonPressEventHandler(PlayerIndex index, GamePadButtonEventDetails e);
        public event ButtonPressEventHandler ButtonPressed;
        public delegate void ButtonReleaseEventHandler(PlayerIndex index, GamePadButtonEventDetails e);
        public event ButtonReleaseEventHandler ButtonReleased;

        public delegate void AnalogMoveEventHandler(PlayerIndex index, GamePadAnalogEventDetails e);
        public event AnalogMoveEventHandler AnalogMoved;

        GamePadWrapper gamePadOneWrapper;
        GamePadWrapper gamePadTwoWrapper;
        GamePadWrapper gamePadThreeWrapper;
        GamePadWrapper gamePadFourWrapper;

        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="game"></param>
        public GamePadsWrapper(Game game)
        {
            gamePadOneWrapper = new GamePadWrapper(game, PlayerIndex.One);
            gamePadOneWrapper.Disconnected += new GamePadWrapper.DisconnectedEventHandler(this.GamePadOneDisconnected);
            gamePadOneWrapper.Connected += new GamePadWrapper.ConnectedEventHandler(this.GamePadOneConnected);
            gamePadOneWrapper.ButtonClicked += new GamePadWrapper.ButtonClickEventHandler(this.GamePadOneButtonClicked);
            gamePadOneWrapper.ButtonReleased += new GamePadWrapper.ButtonReleaseEventHandler(this.GamePadOneButtonReleased);
            gamePadOneWrapper.ButtonPressed += new GamePadWrapper.ButtonPressEventHandler(this.GamePadOneButtonPressed);
            gamePadOneWrapper.AnalogMoved += new GamePadWrapper.AnalogMoveEventHandler(this.GamePadOneAnalogMoved);

            gamePadTwoWrapper = new GamePadWrapper(game, PlayerIndex.Two);
            gamePadTwoWrapper.Disconnected += new GamePadWrapper.DisconnectedEventHandler(this.GamePadTwoDisconnected);
            gamePadTwoWrapper.Connected += new GamePadWrapper.ConnectedEventHandler(this.GamePadTwoConnected);
            gamePadTwoWrapper.ButtonClicked += new GamePadWrapper.ButtonClickEventHandler(this.GamePadTwoButtonClicked);
            gamePadTwoWrapper.ButtonReleased += new GamePadWrapper.ButtonReleaseEventHandler(this.GamePadTwoButtonReleased);
            gamePadTwoWrapper.ButtonPressed += new GamePadWrapper.ButtonPressEventHandler(this.GamePadTwoButtonPressed);
            gamePadTwoWrapper.AnalogMoved += new GamePadWrapper.AnalogMoveEventHandler(this.GamePadTwoAnalogMoved);

            gamePadThreeWrapper = new GamePadWrapper(game, PlayerIndex.Three);
            gamePadThreeWrapper.Disconnected += new GamePadWrapper.DisconnectedEventHandler(this.GamePadThreeDisconnected);
            gamePadThreeWrapper.Connected += new GamePadWrapper.ConnectedEventHandler(this.GamePadThreeConnected);
            gamePadThreeWrapper.ButtonClicked += new GamePadWrapper.ButtonClickEventHandler(this.GamePadThreeButtonClicked);
            gamePadThreeWrapper.ButtonReleased += new GamePadWrapper.ButtonReleaseEventHandler(this.GamePadThreeButtonReleased);
            gamePadThreeWrapper.ButtonPressed += new GamePadWrapper.ButtonPressEventHandler(this.GamePadThreeButtonPressed);
            gamePadThreeWrapper.AnalogMoved += new GamePadWrapper.AnalogMoveEventHandler(this.GamePadThreeAnalogMoved);

            gamePadFourWrapper = new GamePadWrapper(game, PlayerIndex.Four);
            gamePadFourWrapper.Disconnected += new GamePadWrapper.DisconnectedEventHandler(this.GamePadFourDisconnected);
            gamePadFourWrapper.Connected += new GamePadWrapper.ConnectedEventHandler(this.GamePadFourConnected);
            gamePadFourWrapper.ButtonClicked += new GamePadWrapper.ButtonClickEventHandler(this.GamePadFourButtonClicked);
            gamePadFourWrapper.ButtonReleased += new GamePadWrapper.ButtonReleaseEventHandler(this.GamePadFourButtonReleased);
            gamePadFourWrapper.ButtonPressed += new GamePadWrapper.ButtonPressEventHandler(this.GamePadFourButtonPressed);
            gamePadFourWrapper.AnalogMoved += new GamePadWrapper.AnalogMoveEventHandler(this.GamePadFourAnalogMoved);
        }

        #region Disconnected Gamepad
        /// <summary>
        /// Redistribute the disconnected event
        /// </summary>
        /// <param name="details"></param>
        private void GamePadOneDisconnected(GamePadEventDetails details)
        {
            if (Disconnected != null) Disconnected(PlayerIndex.One, details);
        }

        /// <summary>
        /// Redistribute the disconnected event
        /// </summary>
        /// <param name="details"></param>
        private void GamePadTwoDisconnected(GamePadEventDetails details)
        {
            if (Disconnected != null) Disconnected(PlayerIndex.Two, details);
        }

        /// <summary>
        /// Redistribute the disconnected event
        /// </summary>
        /// <param name="details"></param>
        private void GamePadThreeDisconnected(GamePadEventDetails details)
        {
            if (Disconnected != null) Disconnected(PlayerIndex.Three, details);
        }

        /// <summary>
        /// Redistribute the disconnected event
        /// </summary>
        /// <param name="details"></param>
        private void GamePadFourDisconnected(GamePadEventDetails details)
        {
            if (Disconnected != null) Disconnected(PlayerIndex.Four, details);
        }
        #endregion

        #region Connected Gamepad
        /// <summary>
        /// Redistribute the connected event
        /// </summary>
        /// <param name="details"></param>
        private void GamePadOneConnected(GamePadEventDetails details)
        {
            if (Connected != null) Connected(PlayerIndex.One, details);
        }

        /// <summary>
        /// Redistribute the connected event
        /// </summary>
        /// <param name="details"></param>
        private void GamePadTwoConnected(GamePadEventDetails details)
        {
            if (Connected != null) Connected(PlayerIndex.Two, details);
        }

        /// <summary>
        /// Redistribute the connected event
        /// </summary>
        /// <param name="details"></param>
        private void GamePadThreeConnected(GamePadEventDetails details)
        {
            if (Connected != null) Connected(PlayerIndex.Three, details);
        }

        /// <summary>
        /// Redistribute the connected event
        /// </summary>
        /// <param name="details"></param>
        private void GamePadFourConnected(GamePadEventDetails details)
        {
            if (Connected != null) Connected(PlayerIndex.Four, details);
        }
        #endregion

        #region Button Clicked
        /// <summary>
        /// Redistribute the button clicked event
        /// </summary>
        /// <param name="details"></param>
        private void GamePadOneButtonClicked(GamePadButtonEventDetails details)
        {
            if (ButtonClicked != null) ButtonClicked(PlayerIndex.One, details);
        }

        /// <summary>
        /// Redistribute the button clicked event
        /// </summary>
        /// <param name="details"></param>
        private void GamePadTwoButtonClicked(GamePadButtonEventDetails details)
        {
            if (ButtonClicked != null) ButtonClicked(PlayerIndex.Two, details);
        }

        /// <summary>
        /// Redistribute the button clicked event
        /// </summary>
        /// <param name="details"></param>
        private void GamePadThreeButtonClicked(GamePadButtonEventDetails details)
        {
            if (ButtonClicked != null) ButtonClicked(PlayerIndex.Three, details);
        }

        /// <summary>
        /// Redistribute the button clicked event
        /// </summary>
        /// <param name="details"></param>
        private void GamePadFourButtonClicked(GamePadButtonEventDetails details)
        {
            if (ButtonClicked != null) ButtonClicked(PlayerIndex.Four, details);
        }
        #endregion

        #region Button Released
        /// <summary>
        /// Redistribute the button released event
        /// </summary>
        /// <param name="details"></param>
        private void GamePadOneButtonReleased(GamePadButtonEventDetails details)
        {
            if (ButtonReleased != null) ButtonReleased(PlayerIndex.One, details);
        }

        /// <summary>
        /// Redistribute the button released event
        /// </summary>
        /// <param name="details"></param>
        private void GamePadTwoButtonReleased(GamePadButtonEventDetails details)
        {
            if (ButtonReleased != null) ButtonReleased(PlayerIndex.Two, details);
        }

        /// <summary>
        /// Redistribute the button released event
        /// </summary>
        /// <param name="details"></param>
        private void GamePadThreeButtonReleased(GamePadButtonEventDetails details)
        {
            if (ButtonReleased != null) ButtonReleased(PlayerIndex.Three, details);
        }

        /// <summary>
        /// Redistribute the button released event
        /// </summary>
        /// <param name="details"></param>
        private void GamePadFourButtonReleased(GamePadButtonEventDetails details)
        {
            if (ButtonReleased != null) ButtonReleased(PlayerIndex.Four, details);
        }
        #endregion

        #region Button Pressed
        /// <summary>
        /// Redistribute the buton pressed event
        /// </summary>
        /// <param name="details"></param>
        private void GamePadOneButtonPressed(GamePadButtonEventDetails details)
        {
            if (ButtonPressed != null) ButtonPressed(PlayerIndex.One, details);
        }

        /// <summary>
        /// Redistribute the buton pressed event
        /// </summary>
        /// <param name="details"></param>
        private void GamePadTwoButtonPressed(GamePadButtonEventDetails details)
        {
            if (ButtonPressed != null) ButtonPressed(PlayerIndex.Two, details);
        }

        /// <summary>
        /// Redistribute the buton pressed event
        /// </summary>
        /// <param name="details"></param>
        private void GamePadThreeButtonPressed(GamePadButtonEventDetails details)
        {
            if (ButtonPressed != null) ButtonPressed(PlayerIndex.Three, details);
        }

        /// <summary>
        /// Redistribute the buton pressed event
        /// </summary>
        /// <param name="details"></param>
        private void GamePadFourButtonPressed(GamePadButtonEventDetails details)
        {
            if (ButtonPressed != null) ButtonPressed(PlayerIndex.Four, details);
        }
        #endregion

        #region Analog Movement
        /// <summary>
        /// Redistribute the analog movement event
        /// </summary>
        /// <param name="details"></param>
        private void GamePadOneAnalogMoved(GamePadAnalogEventDetails details)
        {
            if (AnalogMoved != null) AnalogMoved(PlayerIndex.One, details);
        }

        /// <summary>
        /// Redistribute the analog movement event
        /// </summary>
        /// <param name="details"></param>
        private void GamePadTwoAnalogMoved(GamePadAnalogEventDetails details)
        {
            if (AnalogMoved != null) AnalogMoved(PlayerIndex.Two, details);
        }

        /// <summary>
        /// Redistribute the analog movement event
        /// </summary>
        /// <param name="details"></param>
        private void GamePadThreeAnalogMoved(GamePadAnalogEventDetails details)
        {
            if (AnalogMoved != null) AnalogMoved(PlayerIndex.Three, details);
        }

        /// <summary>
        /// Redistribute the analog movement event
        /// </summary>
        /// <param name="details"></param>
        private void GamePadFourAnalogMoved(GamePadAnalogEventDetails details)
        {
            if (AnalogMoved != null) AnalogMoved(PlayerIndex.Four, details);
        }
        #endregion
    }
}