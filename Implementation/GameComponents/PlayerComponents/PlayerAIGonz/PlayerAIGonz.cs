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
using System.Diagnostics;
using HBBB.GameComponents.BoardComponents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace HBBB.GameComponents.PlayerComponents {

    class PlayerAIGonz : IPlayerAIHandler {
        internal readonly MovementController MovementController;
        internal readonly BlockTransportingBehavior BlockTransportingBehavior;
        internal readonly ExpandTheBaseBehavior ExpandTheBaseBehavior;
        
        readonly List<Behavior> behaviors = new List<Behavior>();

        internal GameSession GameSession;
        internal Player Player;
        internal GameTime CurrentGameTime;
        internal Random Random = new Random();

        // These track GamePad.GetState(0) for debugging purposes
        GamePadState debugGamePadState;
        GamePadState debugGamePadPrevState;
        
        // Update() will try to transition player.Form to match DesiredPlayerForm.  It may take some time,
        // and it may fail (e.g. if the player is above an obstacle).  Check player.Form to see the actual
        // player form.
        internal Player.PlayerForm DesiredPlayerForm;
        
        public bool Enabled { get; set; } // IPlayerAIHandler
       
        //---------------------------------------------------------------------------------------------------
        public PlayerAIGonz(GameSession gameSession_) {
            GameSession = gameSession_;
            
            MovementController = new MovementController(this);

            BlockTransportingBehavior = new BlockTransportingBehavior(this);
            behaviors.Add(BlockTransportingBehavior);

            ExpandTheBaseBehavior = new ExpandTheBaseBehavior(this);
            behaviors.Add(ExpandTheBaseBehavior);

            Reset();
        }
        
        //---------------------------------------------------------------------------------------------------
        // Generates message such as:
        // "PlayerAIGonz_2.ProcessBlockState: Can't find a block, sleeping"
        internal void Log(string subsystem, string message) {
            string line = "PlayerAIGonz";
            if (Player != null)
              line += "_" + Player.PlayerIndex;
            line += "." + subsystem;
            if (message != "") line += ": " + message;
            Debug.WriteLine(line);
        }

        //---------------------------------------------------------------------------------------------------
        public void Reset() { // IPlayerAIHandler
            Log("Reset", "");
            
            debugGamePadState = GamePad.GetState(0);
            debugGamePadPrevState = debugGamePadState;

            Player = null;
            CurrentGameTime = null;
            DesiredPlayerForm = Player.PlayerForm.BUBBLE;

            MovementController.Reset();

            foreach (Behavior behavior in behaviors)
                behavior.Reset();
                
        }

        #region functions borrowed from Nut's PlayerAIHandler

        //---------------------------------------------------------------------------------------------------
        protected Block FindNearestBlock() {
            List<Block> blocks = GameSession.Board.AllBlocksInPlay;
            Block nearest = null;
            float smallestDist = 0.0f;
            foreach (Block block in blocks)
            {
                float dist = Vector2.Distance(block.Position, Player.Bubble.CenterPoint.Position);
                if (nearest == null || dist < smallestDist)
                {
                    smallestDist = dist;
                    nearest = block;
                }
            }
            return nearest;
        }

        //---------------------------------------------------------------------------------------------------
        protected Block FindRandomBlock()
        {
            if (GameSession.Board.AllBlocksInPlay.Count == 0) 
                return null;
            return GameSession.Board.AllBlocksInPlay[Random.Next(GameSession.Board.AllBlocksInPlay.Count)];
        }

        //---------------------------------------------------------------------------------------------------
        // Helper function to find is a slot has player blocks adjacent to it.
        internal bool IsSlotAdjacentToPlayerBlocks(Slot slot, Player player)
        {
            foreach (Slot s in slot.adjacentSlots)
            {
                if (s.Block != null && s.Block.OwningPlayer == player) return true;
            }
            return false;
        }

        //---------------------------------------------------------------------------------------------------
        protected Slot FindFirstBlockDropSlot()
        {
            foreach (Slot s in this.GameSession.Board.Slots)
            {
                if (s.SpecialMode == Slot.SpecialModeType.BUBBLE_POPPING_SLOT) continue;
                if (s.SpecialMode == Slot.SpecialModeType.DEAD_SLOT) continue;
                if (s.SpecialMode == Slot.SpecialModeType.SOURCE_SLOT) continue;
                if (s.Block != null) continue;
                if (IsSlotAdjacentToPlayerBlocks(s, Player)) return s;
            }
            return null;
        }

        #endregion

        //---------------------------------------------------------------------------------------------------
        void ProcessDesiredPlayerForm() {
            if (Player.Form == DesiredPlayerForm) return; // nothing to do
            
            Debug.Assert(DesiredPlayerForm != Player.PlayerForm.IN_TRANSITION); // don't do this
            if (DesiredPlayerForm == Player.PlayerForm.SOLID) Player.ChangeForm(Player.PlayerForm.IN_TRANSITION); // set form to in transition so we visualize the transition indicator HUD
            
            float transitionAmount = (float)CurrentGameTime.ElapsedGameTime.Milliseconds / (Player.AVOID_INVERSION_HACKER * 1000.0f);
            Player.TimeInTransition += transitionAmount;
            if (Player.TimeInTransition >= Player.TimeRequiredToTransition)
            {
                Log("ProcessDesiredPlayerForm", "Successful transition to " + DesiredPlayerForm);
                Player.ChangeForm(DesiredPlayerForm);
            }
        }
        
        //---------------------------------------------------------------------------------------------------
        // @@ GONZ: It's strange that player is passed repeatedly to this function -- maybe it should
        // be passed once to Reset()
        public void Update(GameTime gameTime, Player player_) { // IPlayerInputHandler
            //Log("Update", "");

            CurrentGameTime = gameTime;
            Player = player_;
            if (Player == null) return;

            debugGamePadPrevState = debugGamePadState;
            debugGamePadState = GamePad.GetState(0);
            
            // General processing
            ProcessDesiredPlayerForm();

            foreach (Behavior behavior in behaviors)
                behavior.Process();
            
            MovementController.Update();
        }

    }

}
