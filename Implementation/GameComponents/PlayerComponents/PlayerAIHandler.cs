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
// File Created: 24 October 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using HBBB.Core.MassSpring.Verlet;
using HBBB.GameComponents.BoardComponents;

namespace HBBB.GameComponents.PlayerComponents
{
    interface IPlayerAIHandler : IPlayerInputHandler
    {
        void Reset();
        bool Enabled { get; set; }
    }

    /// <summary>
    /// Handles AI for a CPU player.  This is generic.  Better AIs can derive from this.
    /// Ghost Color Original Puck Man[20] American Pac-Man 
    /// Character (Personality) Translation Nickname Translation Alternate
    /// character Alternate
    /// nickname Character (Personality) Nickname 
    /// Red Oikake - chaser Akabei - red guy Urchin Macky Shadow Blinky 
    /// Pink Machibuse - ambusher Pinky - pink guy Romp Micky Speedy Pinky 
    /// Cyan Kimagure - fickle Aosuke - blue guy Stylist Mucky Bashful Inky 
    /// Orange Otoboke - stupid Guzuta - slow guy Crybaby Mocky Pokey Clyde 
    /// </summary>
    abstract class PlayerAIHandler : IPlayerAIHandler, IPlayerInputHandler
    {
        protected const float X_ACCELERATION_MULTIPLIER = 100.0f;
        protected const float Y_ACCELERATION_MULTIPLIER = -100.0f;

        protected static Random baseRandom;

        protected PlayerIndex playerIndex;
        protected Player player;
        protected GameSession session;

        /// <summary>
        /// The direction we are heading
        /// </summary>
        protected Vector2 currentDirection;
        /// <summary>
        /// Where to drop our payload
        /// </summary>
        protected Slot dropTarget;
        /// <summary>
        /// What we want to pickup
        /// </summary>
        protected Block blockTarget;
        /// <summary>
        /// Should I boost my speed?
        /// </summary>
        protected bool boostSpeedFlag;
        /// <summary>
        /// on/off
        /// </summary>
        protected bool enabled = false;
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="player"></param>
        public PlayerAIHandler(PlayerIndex index, ref GameSession session)
        {
            this.playerIndex = index;
            this.session = session;
            baseRandom = new System.Random(System.Environment.TickCount);
            Reset();
        }

        /// <summary>
        /// Clear out all the logic and gameplay state stored
        /// </summary>
        public virtual void Reset()
        {
            blockTarget = null;
            dropTarget = null;
            boostSpeedFlag = false;
            currentDirection = new Vector2();
        }

        /// <summary>
        /// Process AI for this player, called manually by the HBBBGame Update method
        /// </summary>
        public abstract void Update(GameTime gameTime, Player player);

        #region Helper functions
        /// <summary>
        /// Find the nearest one
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        protected Block FindNearestBlock()
        {
            List<Block> blocks = session.Board.AllBlocksInPlay;
            Block nearest = null;
            float smallestDist = 0.0f;
            foreach (Block block in blocks)
            {
                float dist = Vector2.Distance(block.Position, player.Bubble.CenterPoint.Position);
                if (nearest == null || dist < smallestDist)
                {
                    smallestDist = dist;
                    nearest = block;
                }
            }
            return nearest;
        }

        /// <summary>
        /// Find nearest ememy owned block
        /// </summary>
        protected Block FindOpponentBlock()
        {
            List<Block> blocks = session.Board.BlocksInSlots;
            foreach (Block block in blocks)
            {
                if (block.OwningPlayer != null && block.OwningPlayer != player && block.IsLocked == false) return block;
            }
            return null;
        }

        /// <summary>
        /// Find any block
        /// </summary>
        /// <returns></returns>
        protected Block FindRandomBlock()
        {
            return session.Board.AllBlocksInPlay[baseRandom.Next(session.Board.AllBlocksInPlay.Count)];
        }

        /// <summary>
        /// Find the nearest one
        /// </summary>
        /// <returns></returns>
        protected PowerUps.PowerUp FindNearestPowerUp()
        {
            List<PowerUps.PowerUp> powerups = session.Board.PowerUps;
            PowerUps.PowerUp nearest = null;
            float smallestDist = 0.0f;
            foreach (PowerUps.PowerUp pup in powerups)
            {
                if (pup.IsActive) continue;  // ignore already running powerups
                float dist = Vector2.Distance(pup.Position, player.Bubble.CenterPoint.Position);
                if (nearest == null || dist < smallestDist)
                {
                    smallestDist = dist;
                    nearest = pup;
                }
            }
            return nearest;
        }

        /// <summary>
        /// Find nearest slot to drop into
        /// </summary>
        /// <returns></returns>
        protected Slot FindBestBlockDropSlot()
        {
            Slot nearest = null;
            float smallestDist = 0.0f;
            foreach (Slot s in session.Board.Slots)
            {
                if (s.SpecialMode == Slot.SpecialModeType.BUBBLE_POPPING_SLOT) continue;
                if (s.SpecialMode == Slot.SpecialModeType.DEAD_SLOT) continue;
                if (s.SpecialMode == Slot.SpecialModeType.SOURCE_SLOT) continue;
                if (s.Block != null) continue;
                if (IsSlotAdjacentToPlayerBlocks(s, player))
                {
                    float dist = Vector2.Distance(s.Position, player.Bubble.CenterPoint.Position);
                    if (nearest == null || dist < smallestDist)
                    {
                        smallestDist = dist;
                        nearest = s;
                    }
                }
            }
            return nearest;
        }

        /// <summary>
        /// find a slot
        /// </summary>
        /// <returns></returns>
        protected Slot FindFirstBlockDropSlot()
        {
            foreach (Slot s in session.Board.Slots)
            {
                if (s.SpecialMode == Slot.SpecialModeType.BUBBLE_POPPING_SLOT) continue;
                if (s.SpecialMode == Slot.SpecialModeType.DEAD_SLOT) continue;
                if (s.SpecialMode == Slot.SpecialModeType.SOURCE_SLOT) continue;
                if (s.Block != null) continue;
                if (IsSlotAdjacentToPlayerBlocks(s, player)) return s;
            }
            return null;
        }

        /// <summary>
        /// Return the 1st found source slot
        /// </summary>
        /// <returns></returns>
        protected Slot FindSourceSlot()
        {
            foreach (Slot s in session.Board.Slots)
                if (s.SpecialMode == Slot.SpecialModeType.SOURCE_SLOT) return s;
            return null;
        }

        /// <summary>
        /// Get a player to pick on
        /// </summary>
        /// <returns></returns>
        protected Player FindEmemyPlayerToPickOn()
        {
            int pId = baseRandom.Next(4); // hack assumes 4 players
            Player pickonHim = session.Player1;
            if (pId == 0) pickonHim = session.Player1;
            if (pId == 1) pickonHim = session.Player2;
            if (pId == 2) pickonHim = session.Player3;
            if (pId == 3) pickonHim = session.Player4;
            // make sure not to pick on ourselves
            if (pickonHim == player && player != session.Player1) pickonHim = session.Player1;
            else if (pickonHim == player) pickonHim = session.Player2;
            return pickonHim;
        }

        /// <summary>
        /// Find a slot to drop a negative powerup into
        /// </summary>
        /// <returns></returns>
        protected Slot FindEmemyPowerUpDropSlot()
        {
            Player pickonHim = FindEmemyPlayerToPickOn();
            foreach (Block b in session.Board.BlocksInSlots)
            {
                if (b.OwningPlayer == pickonHim) return b.OwningSlot;
            }
            // that failed, return anything
            foreach (Block b in session.Board.BlocksInSlots)
            {
                if (b.OwningPlayer != player) return b.OwningSlot;
            }
            return null;
        }

        /// <summary>
        /// Find a slot to drop a positive powerup into
        /// </summary>
        /// <returns></returns>
        protected Slot FindPowerUpDropSlot()
        {
            foreach (Block b in session.Board.BlocksInSlots)
            {
                if (b.OwningPlayer == player) return b.OwningSlot;
            }
            return null;
        }

        /// <summary>
        /// Helper function to find is a slot has player blocks adjacent to it.
        /// 
        /// TODO move this into the Slot data structure!
        /// </summary>
        /// <param name="slotId"></param>
        /// <returns></returns>
        protected bool IsSlotAdjacentToPlayerBlocks(Slot slot, Player player)
        {
            foreach (Slot s in slot.adjacentSlots)
            {
                if (s.Block != null && s.Block.OwningPlayer == player) return true;
            }
            return false;
        }
        #endregion
    }
}
