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
// File Created: 14 September 2008, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using HBBB.GameComponents.BoardComponents;
using HBBB.Core.Math;

namespace HBBB.GameComponents.PlayerComponents
{
    /// <summary>
    /// Bride is aggressive and straight forward.  always attacking.
    /// </summary>
    class PlayerAIBride : PlayerAIHandler
    {
        enum Objective
        {
            NONE = 0,
            ACQUIRE_OPPONENT_BLOCK,
//            TRAIL_AND_STEAL,
            ACQUIRE_FREE_BLOCK,
//            BLOCK_WINNING_PLAYER,
            BOOST_TO_SOURCE,
            DROP_PAYLOAD
        }
        Objective currentObjective;
        float timeSinceObjectiveChange;
        const float MAX_OBJECTIVE_ATTEMPT_TIME = 20.0f;

        Slot targetSlot;
        Block targetBlock;
        Vector2 targetLocation;
        //Player targetPlayer;

        public PlayerAIBride(PlayerIndex index, GameSession session)
            : base(index, ref session)
        {
            currentObjective = Objective.NONE;
            timeSinceObjectiveChange = 0.0f;
        }

        /// <summary>
        /// Reset the AI for a new game
        /// </summary>
        public override void Reset()
        {
            ChangeObjective(Objective.NONE);
            timeSinceObjectiveChange = 0.0f;
            targetLocation = new Vector2();
            targetBlock = null;
            targetSlot = null;
            //targetPlayer = null;
            base.Reset();
        }

        /// <summary>
        /// Process AI for this player, called manually by the HBBBGame Update method
        /// </summary>
        public override void Update(GameTime gameTime, Player player)
        {
            if (!enabled) return;
            if (player == null) return;
            if (this.player == null) this.player = player;

            boostSpeedFlag = false;
            timeSinceObjectiveChange += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (player.Payload != null && currentObjective != Objective.DROP_PAYLOAD) ChangeObjective(Objective.DROP_PAYLOAD);
            if (timeSinceObjectiveChange > MAX_OBJECTIVE_ATTEMPT_TIME) ChangeObjective(Objective.NONE);
            if (currentObjective == Objective.DROP_PAYLOAD && player.Payload == null) ChangeObjective(Objective.NONE);
            if (currentObjective == Objective.NONE)
            {
                RandomlyChangeToAcquisitionObjective();
                if (player.Form == Player.PlayerForm.SOLID) player.ChangeForm(Player.PlayerForm.BUBBLE);
                return;
            }

            // do some common calculations
            Vector2 directionVector = player.Bubble.CenterPoint.Position - player.Bubble.CenterPoint.LastPosition;
            float distance = Vector2.Distance(player.Bubble.CenterPoint.Position, targetLocation);
            float speed = Vector2.Distance(player.Bubble.CenterPoint.Position, player.Bubble.CenterPoint.LastPosition);
            float timeTillTarget = distance / speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Perform Objective
            if (currentObjective == Objective.ACQUIRE_FREE_BLOCK || currentObjective == Objective.ACQUIRE_OPPONENT_BLOCK)
            {
                if (targetBlock == null && currentObjective == Objective.ACQUIRE_FREE_BLOCK) targetBlock = FindNearestBlock();
                if (targetBlock == null && currentObjective == Objective.ACQUIRE_OPPONENT_BLOCK) targetBlock = FindOpponentBlock();
                if (targetBlock == null)
                {
                    ChangeObjective(Objective.NONE);
                    return;
                }
                if (targetBlock.IsLocked)
                {
                    targetBlock = null;
                    return;
                }

                targetLocation = targetBlock.Position;
                if (player.Form != Player.PlayerForm.SOLID)// check if we should start transitioning to pick it up
                {
                    if ((distance < player.Bubble.OuterRadius) ||
                        (timeTillTarget < (player.TimeRequiredToTransition - player.TimeInTransition)))
                    {
                        player.ChangeForm(Player.PlayerForm.IN_TRANSITION);
                        float transitionAmount = (float)gameTime.ElapsedGameTime.Milliseconds / (Player.AVOID_INVERSION_HACKER * 1000.0f);
                        player.TimeInTransition += transitionAmount;
                        if (player.TimeInTransition >= player.TimeRequiredToTransition) player.ChangeForm(Player.PlayerForm.SOLID);
                    }
                }
                else if (player.Payload == null) // tried and we didn't get it
                {
                    player.ChangeForm(Player.PlayerForm.BUBBLE);
                }
                else                         // we got it
                {
                    ChangeObjective(Objective.DROP_PAYLOAD);
                    return;
                }
            }
/*            else if (currentObjective == Objective.BLOCK_WINNING_PLAYER)
            {
                if (player.Form != Player.PlayerForm.SOLID) // go solid and block
                {
                     player.ChangeForm(Player.PlayerForm.IN_TRANSITION);
                     float transitionAmount = (float)gameTime.ElapsedGameTime.Milliseconds / (Player.AVOID_INVERSION_HACKER * 1000.0f);
                     player.TimeInTransition += transitionAmount;
                     if (player.TimeInTransition >= player.TimeRequiredToTransition) player.ChangeForm(Player.PlayerForm.SOLID);
                }
                // TODO this should be winning player
                if (targetSlot == null) targetSlot = FindEmemyPowerUpDropSlot();
                targetLocation = targetSlot.Position;
            }
            else if (currentObjective == Objective.TRAIL_AND_STEAL)
            {
                if (targetPlayer == null) targetPlayer = FindEmemyPlayerToPickOn();
                targetLocation = targetPlayer.Bubble.CenterPoint.Position;
                if (targetPlayer.Payload != null)
                {
                    IBubblePayload thePayload = targetPlayer.Payload;
                    if (thePayload is Block)
                    {
                        ChangeObjective(Objective.ACQUIRE_OPPONENT_BLOCK);
                        targetBlock = (Block) thePayload;
                    }
                    else
                    {
                        // TODO powerups
                    }
                }
            }
 */
            else if (currentObjective == Objective.BOOST_TO_SOURCE)
            {
                if (targetSlot == null) targetSlot = FindSourceSlot();
                targetLocation = targetSlot.Position;
                if (Vector2.Distance(player.Bubble.CenterPoint.Position, targetSlot.Position) < player.Bubble.MaxBoundingRadius)
                {
                    // we made it there, change objectives
                    ChangeObjective(Objective.NONE);
                    return;
                }
                boostSpeedFlag = true;
            }
            else if (currentObjective == Objective.DROP_PAYLOAD)
            {
                if (targetSlot == null && player.Payload is PowerUps.PowerUp) targetSlot = FindEmemyPowerUpDropSlot();
                else if (targetSlot == null) targetSlot = FindBestBlockDropSlot();
                if (targetSlot == null) return;
                if (targetSlot.Block != null)
                {
                    targetSlot = null;
                    return;
                }
                targetLocation = targetSlot.Position;
                // drop the payload once we are on top of the slot
                if (Vector2.Distance(player.Bubble.CenterPoint.Position, targetSlot.Position) < player.Bubble.InnerRadius)
                {
                    player.ChangeForm(Player.PlayerForm.BUBBLE);
                    ChangeObjective(Objective.NONE);
                    return;
                }
            }

            // move toward a target location (TODO take into account the angle between the vectors?)
            Vector2 vectorToTarget = targetLocation - player.Bubble.CenterPoint.Position;
            currentDirection = Vector2.Add(directionVector, vectorToTarget);

            float distanceToTarget = Vector2.Distance(player.Bubble.CenterPoint.Position, targetLocation);
            if (distanceToTarget > player.Bubble.MaxBoundingRadius * 4) boostSpeedFlag = true;
            if (timeTillTarget > 4) boostSpeedFlag = true;
            
            currentDirection.Normalize();
            if (float.IsNaN(currentDirection.X)) return;  // this happens sometime, not sure why
            if (float.IsNaN(currentDirection.Y)) return;

            float xDiff = currentDirection.X;
            float yDiff = -currentDirection.Y; // swap yDiff so that positive is down
            float accelX = X_ACCELERATION_MULTIPLIER;
            float accelY = Y_ACCELERATION_MULTIPLIER;
            // apply our movement objective
            player.SetAcceleration(xDiff * accelX, yDiff * accelY);
            if (boostSpeedFlag) player.BoostAcceleration();
        }

        /// <summary>
        /// change what I'm doing
        /// </summary>
        /// <param name="newObjective"></param>
        private void ChangeObjective(Objective newObjective)
        {
            targetSlot = null;
            targetBlock = null;
            targetLocation = Vector2.Zero;
            //targetPlayer = null;
            timeSinceObjectiveChange = 0.0f;
            currentObjective = newObjective;
        }

        /// <summary>
        /// switch objective
        /// </summary>
        private void RandomlyChangeToAcquisitionObjective()
        {
            int percent = baseRandom.Next(0, 100);
            Objective newObjective = Objective.ACQUIRE_FREE_BLOCK; // 50 percent
            if (percent >= 50 && percent < 90) newObjective = Objective.ACQUIRE_OPPONENT_BLOCK; // 20 percent
            //else if (percent >= 70 && percent < 80) newObjective = Objective.BLOCK_WINNING_PLAYER; // 10 percent
            //else if (percent >= 80 && percent < 90) newObjective = Objective.TRAIL_AND_STEAL; // 10 percent
            else if (percent >= 90) newObjective = Objective.BOOST_TO_SOURCE; // 10 percent

            ChangeObjective(newObjective);
        }
    }
}
