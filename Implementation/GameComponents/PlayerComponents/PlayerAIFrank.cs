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

namespace HBBB.GameComponents.PlayerComponents
{
    /// <summary>
    /// Frank is slow and ignorant, does dumb things
    /// </summary>
    class PlayerAIFrank : PlayerAIHandler
    {
        /// <summary>
        /// Defines the possible AI objectives
        /// </summary>
        enum Objective { NONE, ACQUIRE_BLOCK, ACQUIRE_POWERUP, DROP_PAYLOAD, BIG_BOOST_AROUND };
        /// <summary>
        /// what I'm doing
        /// </summary>
        Objective currentObjective;
        /// <summary>
        /// keep track of how long since we've done something different
        /// </summary>
        float timeSinceObjectiveChange = 0.0f;
        /// <summary>
        /// how long to try to do the same thing
        /// </summary>
        const float MAX_OBJECTIVE_ATTEMPT_TIME = 20.0f;
        /// <summary>
        /// time to dick around in seconds
        /// </summary>
        const float TIME_TO_BOOST_AROUND = 5.0f;
        /// <summary>
        /// The amount of time to boost around when stuck
        /// </summary>
        float boostTimeRemaining = TIME_TO_BOOST_AROUND;

        public PlayerAIFrank(PlayerIndex index, GameSession session)
            : base(index, ref session)
        {
            currentObjective = Objective.NONE;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Reset()
        {
            ChangeObjective(Objective.NONE);
            timeSinceObjectiveChange = 0.0f;
            boostTimeRemaining = TIME_TO_BOOST_AROUND;
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

            #region do a quick check to see if I should change my objective
            if (timeSinceObjectiveChange > MAX_OBJECTIVE_ATTEMPT_TIME)
            {
                ChangeObjective(Objective.BIG_BOOST_AROUND);  // if I've been doing the same thing for 15 seconds boost around and start over
                currentDirection = new Vector2(currentDirection.Y * -1, currentDirection.X * -1);
            }
            else if (player.Payload == null && currentObjective != Objective.BIG_BOOST_AROUND) // if I am colliding with any block and I don't have a payload
            {
                foreach (Block b in session.Board.AllBlocksInPlay)
                {
                    if (b.IsLocked == false && Vector2.Distance(player.Bubble.CenterPoint.Position, b.Position) < player.Bubble.MaxBoundingRadius)
                    {
                        ChangeObjective(Objective.ACQUIRE_BLOCK);
                        blockTarget = b;
                    }
                }
                if (blockTarget != null && blockTarget.IsLocked) ChangeObjective(Objective.NONE);  // block is lock, choose another
            }
            else
            {
                if (dropTarget != null && dropTarget.Block != null) dropTarget = null;  // block in target drop, choose another
            }
            #endregion

            #region Take action based on current objective
            if (currentObjective == Objective.NONE)
            {
                // find a new objective
                if (session.Board.PowerUps.Count > 0) ChangeObjective(Objective.ACQUIRE_POWERUP);
                else ChangeObjective(Objective.ACQUIRE_BLOCK);

                if (player.Form == Player.PlayerForm.SOLID) player.ChangeForm(Player.PlayerForm.BUBBLE);
                blockTarget = null;
                dropTarget = null;
            }
            // GET A PAYLOAD
            if (currentObjective == Objective.ACQUIRE_BLOCK || currentObjective == Objective.ACQUIRE_POWERUP)
            {
                if (blockTarget == null && currentObjective == Objective.ACQUIRE_BLOCK) blockTarget = FindRandomBlock();
                if (blockTarget == null && currentObjective == Objective.ACQUIRE_POWERUP) blockTarget = FindNearestPowerUp();
                if (blockTarget == null) return;

                if (blockTarget.OwningSlot != null)  // someone got it give up
                {
                    blockTarget = null;
                    return;
                }
                // find out how to get there
                currentDirection = blockTarget.Position - player.Bubble.CenterPoint.Position;
                float distance = Vector2.Distance(player.Bubble.CenterPoint.Position, blockTarget.Position);

                // start transitioning
                if (player.Form != Player.PlayerForm.SOLID)
                {
                    player.ChangeForm(Player.PlayerForm.IN_TRANSITION);
                    float transitionAmount = (float)gameTime.ElapsedGameTime.Milliseconds / (Player.AVOID_INVERSION_HACKER * 1000.0f);
                    player.TimeInTransition += transitionAmount;
                    if (player.TimeInTransition >= player.TimeRequiredToTransition)
                    {
                        player.ChangeForm(Player.PlayerForm.SOLID);
                    }
                }
                //continue transitioning only once we are close enough
                if (distance > player.Bubble.MaxBoundingRadius)// give up on solid transition
                {
                    player.ChangeForm(Player.PlayerForm.BUBBLE);
                }

                if (distance < player.Bubble.InnerRadius)
                {
                    // put on the brakes when we get ontop of a block
                    if (player.Bubble.CenterPoint.LastPosition != player.Bubble.CenterPoint.Position)
                        currentDirection = player.Bubble.CenterPoint.LastPosition - player.Bubble.CenterPoint.Position;
                    boostSpeedFlag = true;
                }

                // we just turned solid
                if (player.Form == Player.PlayerForm.SOLID)
                {
                    if (player.Payload != null)  // if we got a payload
                    {
                        ChangeObjective(Objective.DROP_PAYLOAD);
                        blockTarget = null;
                    }
                    else  // go back to bubble and try again
                    {
                        player.ChangeForm(Player.PlayerForm.BUBBLE);
                    }
                }
            }
            // DROP THE PAYLOAD
            else if (currentObjective == Objective.DROP_PAYLOAD)
            {
                if (dropTarget == null && player.Payload is PowerUps.PowerUp) // if we are carrying a powerup
                {
                    PowerUps.PowerUp pup = (PowerUps.PowerUp)player.Payload;
                    if (pup.IsPositive) dropTarget = FindPowerUpDropSlot();
                    else dropTarget = FindEmemyPowerUpDropSlot();

                    if (dropTarget == null)
                    {
                        ChangeObjective(Objective.NONE);  // cant find a drop, do something else
                        return;
                    }
                }
                else if (dropTarget == null) // else are carrying a block
                {
                    // find a slot adjacent to our base to drop into
                    dropTarget = FindFirstBlockDropSlot();
                    if (dropTarget == null) return;
                }

                currentDirection = dropTarget.Position - player.Bubble.CenterPoint.Position;
                float distance = Vector2.Distance(player.Bubble.CenterPoint.Position, dropTarget.Position);

                if (distance < player.Bubble.InnerRadius)
                {
                    // put on the brakes when we get ontop of a block
                    if (player.Bubble.CenterPoint.LastPosition != player.Bubble.CenterPoint.Position)
                        currentDirection = player.Bubble.CenterPoint.LastPosition - player.Bubble.CenterPoint.Position;
                    boostSpeedFlag = true;
                }

                // drop the payload once we are on top of the slot
                if (distance < player.Bubble.InnerRadius)
                {
                    player.ChangeForm(Player.PlayerForm.BUBBLE);
                    dropTarget = null;
                    ChangeObjective(Objective.NONE);
                }
            }
            else if (currentObjective == Objective.BIG_BOOST_AROUND)
            {
                player.ChangeForm(Player.PlayerForm.BUBBLE);
                boostSpeedFlag = true;
                boostTimeRemaining -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (boostTimeRemaining < 0.0f)
                {
                    ChangeObjective(Objective.NONE);
                    boostTimeRemaining = TIME_TO_BOOST_AROUND;
                }
            }
            #endregion

            if (currentObjective == Objective.NONE) return;

            // move toward our objective
            currentDirection.Normalize();
            if (float.IsNaN(currentDirection.X)) return;  // this happens sometime, not sure why
            if (float.IsNaN(currentDirection.Y)) return;

            float xDiff = currentDirection.X;
            float yDiff = -currentDirection.Y;
            player.SetAcceleration(xDiff * X_ACCELERATION_MULTIPLIER, yDiff * Y_ACCELERATION_MULTIPLIER);
            if (boostSpeedFlag) player.BoostAcceleration();
        }

        /// <summary>
        /// change what I'm doing
        /// </summary>
        /// <param name="newObjective"></param>
        private void ChangeObjective(Objective newObjective)
        {
            timeSinceObjectiveChange = 0.0f;
            currentObjective = newObjective;
        }
    }
}
