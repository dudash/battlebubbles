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
using System.Collections.Generic;
using HBBB.GameComponents.BoardComponents;
using HBBB.GameComponents.PowerUps;
using Microsoft.Xna.Framework;

namespace HBBB.GameComponents.PlayerComponents {
        
    //-------------------------------------------------------------------------------------------------------
    // This behavior uses BlockTransportingBehavior to collect free blocks and place them in slots
    // to expand the player's "base".
    class ExpandTheBaseBehavior : Behavior, IBlockTransportingBehaviorOwner {
        enum TState {
            Start=0,
            // 
            AssessSituation
        }
        TState state = TState.Start;
        
        //---------------------------------------------------------------------------------------------------
        public ExpandTheBaseBehavior(PlayerAIGonz playerAI_) : base(playerAI_) {
        }

        //---------------------------------------------------------------------------------------------------
        public override void Reset() {
            base.Reset();
            ChangeState(TState.AssessSituation);
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
            Log("Entering state " + newState);
        }

        //---------------------------------------------------------------------------------------------------
        bool FindATargetBlock() {
            Block nearestBlock = null;
            float nearestDistanceSq = float.MaxValue;
            foreach (Block block in PlayerAI.GameSession.Board.AllBlocksInPlay)
            {
                if (block is PowerUp) continue;
                
                float distSq = (block.Position - PlayerAI.Player.GetPosition()).LengthSquared();
                if (distSq < nearestDistanceSq)
                {
                    nearestDistanceSq = distSq;
                    nearestBlock = block;
                }
            }
            if (nearestBlock != null) {
                PlayerAI.BlockTransportingBehavior.SetTargetBlock(nearestBlock);
                return true;
            }
            return false;
        }
        
        //---------------------------------------------------------------------------------------------------
        void FindATargetSlot() {
            // We don't need to do this until the transporter is ready to place the block
            if (PlayerAI.BlockTransportingBehavior.State != BlockTransportingBehavior.TState.PlacingBlock)
                return;
                
            // Find a target slot
            foreach (Slot s in PlayerAI.GameSession.Board.Slots)
            {
                if (s.SpecialMode != Slot.SpecialModeType.NONE) continue;

                if (s.Block != null) continue;
                
                if (!PlayerAI.IsSlotAdjacentToPlayerBlocks(s, PlayerAI.Player)) continue;
                
                PlayerAI.BlockTransportingBehavior.SetTargetSlot(s);
                return;
            }
        }

        //---------------------------------------------------------------------------------------------------
        public override void Process() { // abstract
            if (CheckSleeping()) return;
            
            switch (state) {
                case TState.AssessSituation: {
                    // Take control of the BlockTransportingBehavior
                    PlayerAI.BlockTransportingBehavior.Owner = this;

                    // Find a block near the player
                    if (!FindATargetBlock()) {
                        SleepFor(3000); // sleep for three seconds and try again
                        return;
                    }
                    
                    FindATargetSlot();
                    break;
                }
            }
        }

        //---------------------------------------------------------------------------------------------------
        void IBlockTransportingBehaviorOwner.NotifyStateChange(BlockTransportingBehavior sender) {
            FindATargetSlot();
        }
    }

}
