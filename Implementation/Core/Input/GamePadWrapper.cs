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
// File Created: 11 April 2008, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace HBBB.Core.Input
{
    /// <summary>
    /// This class wraps the XBOX360 gamepad and provides some convience methods
    /// for the gamepad being wrapped.  This class automatically adds itself
    /// to the game in order to recieve gametime updates.
    /// </summary>
    class GamePadWrapper : GameComponent
    {
        /// <summary>
        /// Button identifiers
        /// </summary>
        public enum ButtonId
        {
            A,
            B,
            BACK,
            D_DOWN,
            D_LEFT,
            D_RIGHT,
            D_UP,
            LEFT_SHOULDER,
            LEFT_STICK,
            RIGHT_SHOULDER,
            RIGHT_STICK,
            START,
            X,
            Y
        };
        /// <summary>
        /// Analog button identifiers
        /// </summary>
        public enum AnalogId
        {
            LEFT_STICK,
            LEFT_TRIGGER,
            RIGHT_STICK,
            RIGHT_TRIGGER
        };
        
        /// <summary>
        /// Keyboard Mappings
        /// </summary>
        public Dictionary<Keys, ButtonId> KeyboardId = new Dictionary<Keys, ButtonId>();
        
        #region GamePad Events
        public delegate void DisconnectedEventHandler(GamePadEventDetails e);
        public event DisconnectedEventHandler Disconnected;
        public delegate void ConnectedEventHandler(GamePadEventDetails e);
        public event ConnectedEventHandler Connected;
        #endregion

        #region Button Events
        public delegate void ButtonClickEventHandler(GamePadButtonEventDetails e);
        public event ButtonClickEventHandler ButtonClicked;
        public delegate void ButtonPressEventHandler(GamePadButtonEventDetails e);
        public event ButtonPressEventHandler ButtonPressed;
        public delegate void ButtonReleaseEventHandler(GamePadButtonEventDetails e);
        public event ButtonReleaseEventHandler ButtonReleased;
        #endregion

        #region Analog Events
        public delegate void AnalogMoveEventHandler(GamePadAnalogEventDetails e);
        public event AnalogMoveEventHandler AnalogMoved;
        #endregion

        /// <summary>
        /// The player gamepad being wrapped
        /// </summary>
        private PlayerIndex index;
        public PlayerIndex Index { get { return index; } }
        /// <summary>
        /// The previous state of the gamepad
        /// </summary>
        private GamePadState lastPadState;
        public GamePadState LastPadState { get { return lastPadState; } }
        /// <summary>
        /// The previous state of the keyboard
        /// </summary>
        private KeyboardState lastKeyboardState;
        public KeyboardState LastKeyboardState { get { return lastKeyboardState; } }

        /// <summary>
        /// Construct for a specific gamepad
        /// </summary>
        /// <param name="game">ref to the game</param>
        /// <param name="index">The player gamepad to wrap</param>
        public GamePadWrapper(Game game, PlayerIndex index) : base(game)
        {
            this.index = index;
            game.Components.Add(this);
            
            KeyboardId[Keys.Z] = ButtonId.A;
            KeyboardId[Keys.S] = ButtonId.B;
            KeyboardId[Keys.Back] = ButtonId.BACK;
            KeyboardId[Keys.Down] = ButtonId.D_DOWN;
            KeyboardId[Keys.Left] = ButtonId.D_LEFT;
            KeyboardId[Keys.Right] = ButtonId.D_RIGHT;
            KeyboardId[Keys.Up] = ButtonId.D_UP;
            KeyboardId[Keys.X] = ButtonId.LEFT_SHOULDER;
            KeyboardId[Keys.C] = ButtonId.LEFT_STICK;
            KeyboardId[Keys.B] = ButtonId.RIGHT_SHOULDER;
            KeyboardId[Keys.V] = ButtonId.RIGHT_STICK;
            KeyboardId[Keys.Enter] = ButtonId.START;
            KeyboardId[Keys.A] = ButtonId.X;
            KeyboardId[Keys.W] = ButtonId.Y;
        }

        /// <summary>
        /// remove ourselves from the game when we destruct
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            this.Game.Components.Remove(this);
            base.Dispose(disposing);
        }

        /// <summary>
        /// On an update poll the state and look for updates
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            GamePadState padState = GamePad.GetState(index);
            KeyboardState keyboardState = Keyboard.GetState(index);

            // a disconnect or reconnect event
            if (lastPadState.IsConnected != padState.IsConnected)
            {
                #region Call Delegate Events For Disconnected/Reconnected Pads
                if (padState.IsConnected)
                {
                    GamePadEventDetails details = new GamePadEventDetails();
                    if (Connected != null) Connected(details);
                }
                else
                {
                    GamePadEventDetails details = new GamePadEventDetails();
                    if (Disconnected != null) Disconnected(details);
                }
                #endregion
            }

            // keyboard changes
            // Utilize the entire keyboard for player 1 right now.
            // A smarter idea might be to use the keyboard in lieu of a GamePad.
            if (index == PlayerIndex.One)
            {
                foreach (KeyValuePair<Keys, ButtonId> key in KeyboardId)
                {
                    if (keyboardState.IsKeyDown(key.Key) != lastKeyboardState.IsKeyDown(key.Key))
                    {
                        GamePadButtonEventDetails details = new GamePadButtonEventDetails(key.Value);
                        if (keyboardState.IsKeyDown(key.Key))
                        {
                            if (ButtonPressed != null) ButtonPressed(details);
                        }
                        else
                        {
                            if (ButtonClicked != null) ButtonClicked(details);
                            if (ButtonReleased != null) ButtonReleased(details);
                        }
                    }
                }
            }

            // button changes
            if (lastPadState.PacketNumber != padState.PacketNumber)
            {
                #region Call Delegate Events For Changed Buttons
                if (padState.Buttons.A != lastPadState.Buttons.A)
                {
                    GamePadButtonEventDetails details = new GamePadButtonEventDetails(ButtonId.A);
                    if (padState.Buttons.A == ButtonState.Pressed)
                    {
                        if (ButtonPressed != null) ButtonPressed(details);
                    }
                    else
                    {
                        if (ButtonClicked != null) ButtonClicked(details);
                        if (ButtonReleased != null) ButtonReleased(details);
                    }
                }
                if (padState.Buttons.B != lastPadState.Buttons.B)
                {
                    GamePadButtonEventDetails details = new GamePadButtonEventDetails(ButtonId.B);
                    if (padState.Buttons.B == ButtonState.Pressed)
                    {
                        if (ButtonPressed != null) ButtonPressed(details);
                    }
                    else
                    {
                        if (ButtonClicked != null) ButtonClicked(details);
                        if (ButtonReleased != null) ButtonReleased(details);
                    }
                }
                if (padState.Buttons.Back != lastPadState.Buttons.Back)
                {
                    GamePadButtonEventDetails details = new GamePadButtonEventDetails(ButtonId.BACK);
                    if (padState.Buttons.Back == ButtonState.Pressed)
                    {
                        if (ButtonPressed != null) ButtonPressed(details);
                    }
                    else
                    {
                        if (ButtonClicked != null) ButtonClicked(details);
                        if (ButtonReleased != null) ButtonReleased(details);
                    }
                }
                if (padState.DPad.Down != lastPadState.DPad.Down)
                {
                    GamePadButtonEventDetails details = new GamePadButtonEventDetails(ButtonId.D_DOWN);
                    if (padState.DPad.Down == ButtonState.Pressed)
                    {
                        if (ButtonPressed != null) ButtonPressed(details);
                    }
                    else
                    {
                        if (ButtonClicked != null) ButtonClicked(details);
                        if (ButtonReleased != null) ButtonReleased(details);
                    }
                }
                if (padState.DPad.Left != lastPadState.DPad.Left)
                {
                    GamePadButtonEventDetails details = new GamePadButtonEventDetails(ButtonId.D_LEFT);
                    if (padState.DPad.Left == ButtonState.Pressed)
                    {
                        if (ButtonPressed != null) ButtonPressed(details);
                    }
                    else
                    {
                        if (ButtonClicked != null) ButtonClicked(details);
                        if (ButtonReleased != null) ButtonReleased(details);
                    }
                }
                if (padState.DPad.Right != lastPadState.DPad.Right)
                {
                    GamePadButtonEventDetails details = new GamePadButtonEventDetails(ButtonId.D_RIGHT);
                    if (padState.DPad.Right == ButtonState.Pressed)
                    {
                        if (ButtonPressed != null) ButtonPressed(details);
                    }
                    else
                    {
                        if (ButtonClicked != null) ButtonClicked(details);
                        if (ButtonReleased != null) ButtonReleased(details);
                    }
                }
                if (padState.DPad.Up != lastPadState.DPad.Up)
                {
                    GamePadButtonEventDetails details = new GamePadButtonEventDetails(ButtonId.D_UP);
                    if (padState.DPad.Up == ButtonState.Pressed)
                    {
                        if (ButtonPressed != null) ButtonPressed(details);
                    }
                    else
                    {
                        if (ButtonClicked != null) ButtonClicked(details);
                        if (ButtonReleased != null) ButtonReleased(details);
                    }
                }
                if (padState.Buttons.LeftShoulder != lastPadState.Buttons.LeftShoulder)
                {
                    GamePadButtonEventDetails details = new GamePadButtonEventDetails(ButtonId.LEFT_SHOULDER);
                    if (padState.Buttons.LeftShoulder == ButtonState.Pressed)
                    {
                        if (ButtonPressed != null) ButtonPressed(details);
                    }
                    else
                    {
                        if (ButtonClicked != null) ButtonClicked(details);
                        if (ButtonReleased != null) ButtonReleased(details);
                    }
                }
                if (padState.Buttons.LeftStick != lastPadState.Buttons.LeftStick)
                {
                    GamePadButtonEventDetails details = new GamePadButtonEventDetails(ButtonId.LEFT_STICK);
                    if (padState.Buttons.LeftStick == ButtonState.Pressed)
                    {
                        if (ButtonPressed != null) ButtonPressed(details);
                    }
                    else
                    {
                        if (ButtonClicked != null) ButtonClicked(details);
                        if (ButtonReleased != null) ButtonReleased(details);
                    }
                }
                if (padState.Buttons.RightShoulder != lastPadState.Buttons.RightShoulder)
                {
                    GamePadButtonEventDetails details = new GamePadButtonEventDetails(ButtonId.RIGHT_SHOULDER);
                    if (padState.Buttons.RightShoulder == ButtonState.Pressed)
                    {
                        if (ButtonPressed != null) ButtonPressed(details);
                    }
                    else
                    {
                        if (ButtonClicked != null) ButtonClicked(details);
                        if (ButtonReleased != null) ButtonReleased(details);
                    }
                }
                if (padState.Buttons.RightStick != lastPadState.Buttons.RightStick)
                {
                    GamePadButtonEventDetails details = new GamePadButtonEventDetails(ButtonId.RIGHT_STICK);
                    if (padState.Buttons.RightStick == ButtonState.Pressed)
                    {
                        if (ButtonPressed != null) ButtonPressed(details);
                    }
                    else
                    {
                        if (ButtonClicked != null) ButtonClicked(details);
                        if (ButtonReleased != null) ButtonReleased(details);
                    }
                }
                if (padState.Buttons.Start != lastPadState.Buttons.Start)
                {
                    GamePadButtonEventDetails details = new GamePadButtonEventDetails(ButtonId.START);
                    if (padState.Buttons.Start == ButtonState.Pressed)
                    {
                        if (ButtonPressed != null) ButtonPressed(details);
                    }
                    else
                    {
                        if (ButtonClicked != null) ButtonClicked(details);
                        if (ButtonReleased != null) ButtonReleased(details);
                    }
                }
                if (padState.Buttons.X != lastPadState.Buttons.X)
                {
                    GamePadButtonEventDetails details = new GamePadButtonEventDetails(ButtonId.X);
                    if (padState.Buttons.X == ButtonState.Pressed)
                    {
                        if (ButtonPressed != null) ButtonPressed(details);
                    }
                    else
                    {
                        if (ButtonClicked != null) ButtonClicked(details);
                        if (ButtonReleased != null) ButtonReleased(details);
                    }
                }
                if (padState.Buttons.Y != lastPadState.Buttons.Y)
                {
                    GamePadButtonEventDetails details = new GamePadButtonEventDetails(ButtonId.Y);
                    if (padState.Buttons.Y == ButtonState.Pressed)
                    {
                        if (ButtonPressed != null) ButtonPressed(details);
                    }
                    else
                    {
                        if (ButtonClicked != null) ButtonClicked(details);
                        if (ButtonReleased != null) ButtonReleased(details);
                    }
                }
                #endregion

                #region Call Delegate Events For Changed Analog Sticks/Buttons
                if (padState.ThumbSticks.Left != lastPadState.ThumbSticks.Left)
                {
                    GamePadAnalogEventDetails details = new GamePadAnalogEventDetails(AnalogId.LEFT_STICK, padState.ThumbSticks.Left);
                    if (AnalogMoved != null) AnalogMoved(details);
                }
                if (padState.ThumbSticks.Right != lastPadState.ThumbSticks.Right)
                {
                    GamePadAnalogEventDetails details = new GamePadAnalogEventDetails(AnalogId.RIGHT_STICK, padState.ThumbSticks.Right);
                    if (AnalogMoved != null) AnalogMoved(details);
                }
                if (padState.Triggers.Left != lastPadState.Triggers.Left)
                {
                    GamePadAnalogEventDetails details = new GamePadAnalogEventDetails(AnalogId.LEFT_TRIGGER, padState.Triggers.Left);
                    if (AnalogMoved != null) AnalogMoved(details);
                }
                if (padState.Triggers.Right != lastPadState.Triggers.Right)
                {
                    GamePadAnalogEventDetails details = new GamePadAnalogEventDetails(AnalogId.RIGHT_TRIGGER, padState.Triggers.Right);
                    if (AnalogMoved != null) AnalogMoved(details);
                }
                #endregion
            }

            lastKeyboardState = keyboardState;
            lastPadState = padState;
        }
    }
}
