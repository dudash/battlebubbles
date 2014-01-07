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
// File Created: 19 October 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using HBBB.Core.MassSpring.Verlet;

namespace HBBB.GameComponents.PlayerComponents
{
    /// <summary>
    /// Handles game input per player gamepad, managing state and gamepad activity
    /// </summary>
    class PlayerInputHandler : IPlayerInputHandler
    {
        GamePadState prevState;
        GamePadState currentState;
        PlayerIndex playerIndex = PlayerIndex.One;
        
        // X_ACCELERATION_MULTIPLIER and Y_ACCELERATION_MULTIPLIER indirectly determine the maximum amount
        // of acceleration that can be applied by the player per frame.
        //
        // @@ GONZ:  This assumes that gameTime.ElapsedGameTime will always be constant, which is
        // currently true but by no means guaranteed, since the engine treats GameTime as a variable.
        // My recommendation is to eliminate GameTime and officially lock everything to a fixed
        // frame rate.  Alternatively, constants like X_ACCELERATION_MULTIPLIER should be converted to
        // units-per-second instead of units-per-frame.
        const float X_ACCELERATION_MULTIPLIER = 300.0f;
        const float Y_ACCELERATION_MULTIPLIER = -300.0f;

        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="player"></param>
        public PlayerInputHandler(PlayerIndex index)
        {
            this.playerIndex = index;
            prevState = currentState = GamePad.GetState(index);
        }

        /// <summary>
        /// Process input for this player
        /// </summary>
        public void Update(GameTime gameTime, Player player)
        {
            if (player == null) return;

            prevState = currentState;
            currentState = GamePad.GetState(playerIndex);
            
            // --------- MOVE THIS PLAYER --------------
            // left or right stick moves the player bubble (diff is -1.0 to 1.0)
            float xDiff = currentState.ThumbSticks.Left.X;
            float yDiff = currentState.ThumbSticks.Left.Y;

#if DEBUG_PHYSICS
            if (playerIndex == PlayerIndex.One) {
                // GONZ: I tried to put this in GamePadWrapper.cs (or GamePadsWrapper.cs?) but the code I found there
                // needs to be improved first
                KeyboardState keyboardState = Keyboard.GetState();
                if (keyboardState.IsKeyDown(Keys.I)) yDiff = 1.0f;
                if (keyboardState.IsKeyDown(Keys.J)) xDiff = -1.0f;
                if (keyboardState.IsKeyDown(Keys.K)) yDiff = -1.0f;
                if (keyboardState.IsKeyDown(Keys.L)) xDiff = 1.0f;
            }
#endif
            
            // modify input to make higher range values faster and lower slower
            player.SetAcceleration(xDiff * X_ACCELERATION_MULTIPLIER, yDiff * Y_ACCELERATION_MULTIPLIER);

            // A button boosts
            if (currentState.Buttons.A == ButtonState.Pressed) player.BoostAcceleration();

            // left and right bumpers together attempt to defend
            if (currentState.Buttons.LeftShoulder == ButtonState.Pressed &&
                currentState.Buttons.RightShoulder == ButtonState.Pressed) player.IsTryingToDefend = true;
            else player.IsTryingToDefend = false;

            // --------- TRANSITION INTO BUBBLE --------------
            // starting a transition into bubble state
            if (currentState.Triggers.Left > 0 && prevState.Triggers.Left <= 0)
            {
                player.ChangeForm(Player.PlayerForm.BUBBLE);
            }
            // --------- TRANSITION INTO SOLID --------------
            // starting a transition into solid state
            else if (currentState.Triggers.Right > 0 && prevState.Triggers.Right <= 0)
            {
                if (player.Form != Player.PlayerForm.SOLID)  // already solid skip this
                    player.ChangeForm(Player.PlayerForm.IN_TRANSITION);
            }
            // continuing a transition to solid state
            else if (currentState.Triggers.Right > 0 && prevState.Triggers.Right > 0)
            {
                if (player.Form != Player.PlayerForm.SOLID) // already solid skip this
                {
                    player.ChangeForm(Player.PlayerForm.IN_TRANSITION);
                    float transitionAmount = (float)gameTime.ElapsedGameTime.Milliseconds / (Player.AVOID_INVERSION_HACKER * 1000.0f);
                    transitionAmount *= currentState.Triggers.Right; // scale transtion by trigger depression amount
                    player.TimeInTransition += transitionAmount;
                    if (player.TimeInTransition >= player.TimeRequiredToTransition)
                    {
                        player.ChangeForm(Player.PlayerForm.SOLID);
                        GamePad.SetVibration(playerIndex, 0.0f, 0.0f);  // finished transition, turn off rumble
                    }
                    float rumbleSpeed = 1.0f - (player.TimeRequiredToTransition - player.TimeInTransition) / player.TimeRequiredToTransition;
                    GamePad.SetVibration(playerIndex, 0.7f, 0.9f*rumbleSpeed);  // rumble faster as we approach solid state
                    return;
                }
            }
            // giving up on a transition to solid state
            else if (currentState.Triggers.Right <= 0 && prevState.Triggers.Right > 0)
            {
                if (player.Form != Player.PlayerForm.SOLID) // already bubble skip this
                    player.ChangeForm(Player.PlayerForm.BUBBLE);
            }
            if (player.IsAbleToDefend && player.IsTryingToDefend) GamePad.SetVibration(playerIndex, 0.5f, 0.5f);
            else if (player.IsBoostingSpeed) GamePad.SetVibration(playerIndex, 0.2f, 0.3f);
            else if (player.IsTryingToDefend) GamePad.SetVibration(playerIndex, 0.2f, 0.1f);
            else GamePad.SetVibration(playerIndex, 0.0f, 0.0f);  // turn off rumble
        }
    }
}
