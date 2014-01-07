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
// File Created: 6 December 2008, Pete Gonzalez
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;

namespace HBBB.GameComponents.PlayerComponents {

    //-------------------------------------------------------------------------------------------------------
    // This helper class implements a dynamics controller that calculates forces for 
    // player.SetAcceleration() to move the player towards a specified TargetLocation, 
    // with minimal overshooting.
    class MovementController {
        readonly PlayerAIGonz owner;

        // This should match PlayerInputHandler.X_ACCELERATION_MULTIPLIER.  It specifies the maximum
        // amount of acceleration allowed to be applied per rendering frame.
        // @@ GONZ: See my note in PlayerInputHandler.cs about the problem with units-per-frame.
        public const float MAX_ACCELERATION = 300.0f;

        // This is the maximum speed the player can attain (i.e. terminal velocity of MAX_ACCELERATION).
        // TODO: An adaptive controller could measure this empirically, since it might change in different
        // environments or if the bubble constants are altered.
        public const float MAX_SPEED = 250.0f;  // pixels/sec

        // CalculateControllerForce() starts braking when the player is within BRAKING_DISTANCE
        // pixels of the target.  
        // TODO: An adaptive controller could calculate this by estimating the maximum possible velocity
        // change per pixel (i.e. measuring the result of MAX_ACCELERATION), and then determining the
        // number of pixels needed to decellerate from MAX_SPEED.
        public const float BRAKING_DISTANCE = 100.0f; // pixels
        
        // Ideally, the CalculateControllerForce() algorithm would be a "bang-bang" controller, i.e.
        // alternating between MAX_ACCELERATION and -MAX_ACCELERATION for maximum affect.  However, a
        // human player cannot move the joystick very far in a single rendering frame, so for
        // fairness/realism, we restrict the maximum change per frame.
        //
        // Small values of MAX_ACCELERATION_CHANGE make the controller less accurate.
        // Large values are more accurate and approach the bang-bang behavior of MAX_ACCELERATION.
        public const float MAX_ACCELERATION_CHANGE = MAX_ACCELERATION/5.0f;

        // The player will move towards TargetLocation when Update() is called.  Assign TargetLocatin=null
        // to disable the controller.
        public Vector2? TargetLocation = null;

        Vector2 lastUpdate_PlayerPosition; // Player.Position as of the last call to Update()
        TimeSpan lastUpdate_TotalTime; // GameTime.TotalGameTime as of the last call to Update()

        // Used by MoveToLocation()
        Vector2 lastControllerForce = Vector2.Zero;
        
        //---------------------------------------------------------------------------------------------------
        public MovementController(PlayerAIGonz owner_) {
          owner = owner_;
        }
        
        //---------------------------------------------------------------------------------------------------
        // This moves the currentPosition vector towards targetPosition by distanceToMove units, without
        // overshooting targetPosition.
        static Vector2 GetMovedTowardsTarget(Vector2 targetPosition, Vector2 currentPosition,
            float distanceToMove) {
            
            Vector2 delta = targetPosition - currentPosition;
            float deltaLength = delta.Length();
            
            if (distanceToMove > deltaLength) 
              return targetPosition;  // don't overshoot the target

            Vector2 direction = delta * (1.0f / deltaLength); // i.e. delta.Normalize()
            return currentPosition + (direction * distanceToMove);
        }
        
        //---------------------------------------------------------------------------------------------------
        // Calculates the force that will move the player towards the target location
        Vector2 CalculateControllerForce(Vector2 target) {
            Vector2 position = owner.Player.GetPosition();

            // We will calculate the current player velocity as the distance moved divided by the total
            // time.  We measure this empirically rather than using the physics system, since the physics
            // system models individual points (not the aggregate player object), and is subject to
            // arbitrary hacks by the game code.
            Vector2 deltaPos = position - lastUpdate_PlayerPosition;
            int deltaMs = (owner.CurrentGameTime.TotalGameTime - lastUpdate_TotalTime).Milliseconds;
            
            if (deltaMs < 1 || deltaMs > 500) {
                owner.Log("MovementController", "Ignoring invalid deltaMs");
                return Vector2.Zero;
            }
            
            if (deltaPos.LengthSquared() > 500*500) {
                owner.Log("MovementController", "Ignoring invalid deltaPos");
                return Vector2.Zero;
            }
            
            // The current player velocity is the distance moved divided by the total time.  
            Vector2 currentVelocity = deltaPos * 1000.0f / (float)deltaMs;  // pixels/sec
            
            Vector2 targetOffset = target - position;
            
            float targetOffsetLength = targetOffset.Length();
            
            Vector2 direction = Vector2.Zero;
            if (targetOffsetLength > 1.0f) {
                direction = targetOffset * (1.0f/targetOffsetLength);
            }
            
            // If we are far away, then the targetSpeed is MAX_SPEED; otherwise, taper it to zero as
            // we approach the target
            float targetSpeed = 0;
            if (targetOffsetLength > BRAKING_DISTANCE) {
                targetSpeed = MAX_SPEED;
            } else if (targetOffsetLength > 1.0f) {
                targetSpeed = (targetOffsetLength/BRAKING_DISTANCE) * MAX_SPEED;
            }
            
            Vector2 targetVelocity = targetSpeed * direction; // pixels/sec
            
            Vector2 force = (targetVelocity - currentVelocity);
            if (force.LengthSquared() < 0.1f) 
                return Vector2.Zero;

            force.Normalize();
            force *= MAX_ACCELERATION;

            //owner.Log("MovementController", "dt=" + deltaMs + "  offset=" + targetOffsetLength 
            //    + "  curV=" + currentVelocity + "  tarV=" + targetVelocity 
            //    + "  force=" + force + "  pos=" + position);
            return force;
        }

        //---------------------------------------------------------------------------------------------------
        public void Reset() {
            lastUpdate_PlayerPosition = Vector2.Zero;
            lastUpdate_TotalTime = TimeSpan.Zero;
            lastControllerForce = Vector2.Zero;
        }
        
        //---------------------------------------------------------------------------------------------------
        public void Update() {
            // Calculate the ideal force
            Vector2 controllerForce = Vector2.Zero;
            
            if (TargetLocation.HasValue) {
                controllerForce = CalculateControllerForce(TargetLocation.Value);
            }
            
            // Limit how fast the "joystick" can be moved, i.e. don't move more than MAX_ACCELERATION_CHANGE
            // units from lastControllerForce.
            Vector2 force = GetMovedTowardsTarget(controllerForce, lastControllerForce, MAX_ACCELERATION_CHANGE);
            
            //if (controllerForce.LengthSquared() > 0.01) {
                owner.Player.SetAcceleration(force);
            //}
            
            // Update lastControllerForce
            lastControllerForce = force;

            // Update the physics counters
            lastUpdate_PlayerPosition = owner.Player.GetPosition();
            lastUpdate_TotalTime = owner.CurrentGameTime.TotalGameTime;
        }
    }

}
