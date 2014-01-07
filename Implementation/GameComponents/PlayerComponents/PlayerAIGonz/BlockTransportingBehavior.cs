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
using HBBB.GameComponents.BoardComponents;
using Microsoft.Xna.Framework;

namespace HBBB.GameComponents.PlayerComponents {

    //-------------------------------------------------------------------------------------------------------
    interface IBlockTransportingBehaviorOwner {
        void NotifyStateChange(BlockTransportingBehavior sender);
    }

    //-------------------------------------------------------------------------------------------------------
    // This is a primitive behavior that picks up the specified TargetBlock and drops it on the 
    // specified TargetSlot.
    class BlockTransportingBehavior : Behavior {
        public enum TState {
            Start=0,
            // Analyze the situation and decide what to do
            AssessSituation,
            // Chase after TargetBlock
            ChasingBlock,
            // Try to pick up TargetBlock
            GrabbingBlock,
            // Go to TargetSlot and drop the block
            PlacingBlock,
            // Wait for the block to be "locked"
            WaitForLock,
            // Mission accomplished
            Succeeded,
            // Cannot proceed, e.g. the TargetBlock disappeared, the TargetSlot became full, etc.
            Stuck
        }
        TState state = TState.Start;

        public TState State { get { return state; } }

        // The block we want to capture
        Block targetBlock;
        Slot targetSlot;
        
        // A general purpose timemark
        int stateTimemark;
        
        public IBlockTransportingBehaviorOwner Owner = null;

        // The block to pick up.
        public Block TargetBlock { get { return targetBlock; } } 

        // The slot where TargetBlock should be dropped
        public Slot TargetSlot { get { return targetSlot; } }

        //---------------------------------------------------------------------------------------------------
        public BlockTransportingBehavior(PlayerAIGonz playerAI_) : base(playerAI_) {
        }
        
        //---------------------------------------------------------------------------------------------------
        public void SetTargetBlock(Block targetBlock_) {
            if (targetBlock == targetBlock_) return;
            targetBlock = targetBlock_;
            
            if (targetBlock == null) Log("Clearing TargetBlock");
            else Log("Setting TargetBlock = " + targetBlock.Id);
            
            ChangeState(TState.AssessSituation);
        }

        //---------------------------------------------------------------------------------------------------
        public void SetTargetSlot(Slot targetSlot_) {
            if (targetSlot == targetSlot_) return;
            targetSlot = targetSlot_;

            if (TargetSlot == null) Log("Clearing TargetSlot");
            else Log("Setting TargetSlot = " + TargetSlot.Id);

            ChangeState(TState.AssessSituation);
        }

        //---------------------------------------------------------------------------------------------------
        public override void Reset() {
            base.Reset();
            ChangeState(TState.AssessSituation);
            targetBlock = null;
            targetSlot = null;
        }

        //---------------------------------------------------------------------------------------------------
        void ChangeState(TState newState) {
            if (state == newState) return;
            
            OnLeaveState(state);
            state = newState;
            OnEnterState(state);
        }
        
        //---------------------------------------------------------------------------------------------------
        void OnLeaveState(TState oldState) {
        
        }

        //---------------------------------------------------------------------------------------------------
        void OnEnterState(TState newState) {
            stateTimemark = 0;
            
            Log("Entering state " + newState);
            
            // Enter a new state
            switch (newState) {
                case TState.ChasingBlock:
                    PlayerAI.DesiredPlayerForm = Player.PlayerForm.BUBBLE;
                    break;
                case TState.GrabbingBlock:
                    PlayerAI.DesiredPlayerForm = Player.PlayerForm.SOLID;
                    stateTimemark = Environment.TickCount + 5000; // give up waiting after this time
                    break;
                case TState.WaitForLock:
                    PlayerAI.DesiredPlayerForm = Player.PlayerForm.BUBBLE;
                    stateTimemark = Environment.TickCount + 2000;  // wait 2 seconds for it to lock
                    break;
                case TState.Stuck:
                    SleepFor(1000);
                    break;
            }

            if (Owner != null) Owner.NotifyStateChange(this);
        }
        
        //---------------------------------------------------------------------------------------------------
        public override void Process() { // abstract
            if (CheckSleeping()) return;
            
            // The player is considered to be "over" the slot/block if the distance is less than this
            // many pixels
            const float PICKUP_RADIUS = Block.DEFAULT_GRAB_RADIUS;
            
            Player player = PlayerAI.Player;
            
            switch (state) {
                case TState.AssessSituation: {
                    if (TargetBlock == null) {
                        ChangeState(TState.Stuck);
                        break;
                    }
                    
                    // Is the player carrying something?
                    if (player.Payload != null) {
                        if (player.Payload == TargetBlock) {
                            // Player has the block; go place it
                            ChangeState(TState.PlacingBlock);
                            break;
                        } else {
                            // It's the wrong object, so dump it and remain in State.Start
                            Log("Dumping wrong block: " + player.Payload.ToString());
                            PlayerAI.DesiredPlayerForm = Player.PlayerForm.BUBBLE;
                            
                            SleepFor(1000);
                            
                            break;
                        }
                    } else {
                        // Player is not carrying anything; go find the block
                        ChangeState(TState.ChasingBlock);
                    }
                    break;
                }
                
                case TState.ChasingBlock: {
                    PlayerAI.MovementController.TargetLocation = TargetBlock.Position;
                    
                    // Are we over the intended block?
                    Vector2 playerPosition = player.GetPosition();
                    float distanceToTarget = (targetBlock.Position - playerPosition).Length();
                    
                    if (distanceToTarget < PICKUP_RADIUS) {
                        // Try to get the block
                        ChangeState(TState.GrabbingBlock);
                    }
                    
                    break;
                }
                
                case TState.GrabbingBlock: {
                    // Were we able to change to solid form?
                    if (player.Form == Player.PlayerForm.SOLID) {
                        // Did we capture something?
                        if (player.Payload != targetBlock) {
                            // @@ if we capture the wrong block, we could do something intelligent here
                            ChangeState(TState.AssessSituation); // start over
                            break;
                        }
                        
                        // We captured the TargetBlock
                        ChangeState(TState.PlacingBlock);
                        break;
                    }
                    
                    if ((Environment.TickCount - stateTimemark) > 0) { // overflowable comparison
                        Log("Gave up trying to grab block");
                        ChangeState(TState.ChasingBlock);
                    }
                    break;
                }
                
                case TState.PlacingBlock: {
                    if (TargetSlot == null) {
                        ChangeState(TState.Stuck);
                        break;
                    }
                    
                    PlayerAI.MovementController.TargetLocation = TargetSlot.Position;
                    
                    // Are we over the intended slot?
                    Vector2 playerPosition = player.GetPosition();
                    float distanceToTarget = (targetSlot.Position - playerPosition).Length();
                    
                    if (distanceToTarget < PICKUP_RADIUS) {
                        // Drop the block
                        ChangeState(TState.WaitForLock);
                    }
                    break;
                }
                
                case TState.WaitForLock: {
                    if (targetBlock.IsLocked) {
                        ChangeState(TState.Succeeded);
                    }
                    if ((Environment.TickCount - stateTimemark) > 0) {
                        // Taking too long to lock; start over
                        Log("Block did not bind to its slot -- trying again");
                        ChangeState(TState.AssessSituation);
                    }
                    break;
                }
            }
            
        }
    }
        
}
