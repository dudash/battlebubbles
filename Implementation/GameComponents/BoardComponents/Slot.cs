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
// File Created: 21 August 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HBBB.Core.Graphics;
using HBBB.GameComponents.PlayerComponents;

namespace HBBB.GameComponents.BoardComponents
{
    /// <summary>
    /// A Hexagon slot that is also a Entity2D with position referenced 
    /// in the center of the bounding rectangle
    /// </summary>
    class Slot : Core.Entity2D
    {
        private const int TEXT_OFFSET_X = 14;
        private const int TEXT_OFFSET_Y = 10;

        /// <summary>
        /// A list of adjacent neighbors
        /// </summary>
        public List<Slot> adjacentSlots;

        /// <summary>
        /// Any special characteristic of this slot
        /// </summary>
        public enum SpecialModeType
        {
            NONE,
            SOURCE_SLOT,
            DEAD_SLOT,
            BUBBLE_POPPING_SLOT
        }

        /// <summary>
        /// Identifies which edge of the gameboard this slot is located on
        /// </summary>
        public enum EdgeType
        {
            NONE,
            TOP_EDGE, BOTTOM_EDGE, LEFT_EDGE, RIGHT_EDGE,
            TOP_LEFT_EDGE, TOP_RIGHT_EDGE, BOTTOM_LEFT_EDGE, BOTTOM_RIGHT_EDGE
        }
        /// <summary>
        /// Do we need to do collision detection of blocks with this slot
        /// </summary>
        private bool blockCollisionsEnabled;
        public bool BlockCollisionsEnabled
        {
            get { return blockCollisionsEnabled; }
            set { blockCollisionsEnabled = value; }
        }
        /// <summary>
        /// Do we need to do collision detection of players with this slot.
        /// </summary>
        private bool playerSolidCollisionsEnabled;
        public bool PlayerSolidCollisionsEnabled
        {
            get { return playerSolidCollisionsEnabled; }
            set { playerSolidCollisionsEnabled = value; }
        }
        /// <summary>
        /// Do we need to do collision detection of players with this slot.
        /// </summary>
        private bool playerBubbleCollisionsEnabled;
        public bool PlayerBubbleCollisionsEnabled
        {
            get { return playerBubbleCollisionsEnabled; }
            set { playerBubbleCollisionsEnabled = value; }
        }
        /// <summary>
        /// A time interval to flash a specific color for
        /// </summary>
        private double timeForFlash = 0.0;
        /// <summary>
        /// The color to flash
        /// </summary>
        private Color flashColor = Color.Red;
        /// <summary>
        /// unique identifier for this slot
        /// </summary>
        protected int id;
        public int Id { get { return id; } }
        /// <summary>
        /// The block in this slot
        /// </summary>
        protected Block block = null;
        public Block Block { get { return block; } }
        /// <summary>
        /// A bounding box around this slot (also used for texture size)
        /// </summary>
        protected Rectangle bounds;
        public Rectangle Bounds
        {
            get { return bounds; }
            set
            {
                bounds = value;
                // move the position to the center of the bounds
                position.X = (float)bounds.X + ((float)bounds.Width / 2.0f);
                position.Y = (float)bounds.Y + ((float)bounds.Height / 2.0f);
            }
        }
        /// <summary>
        /// Override the base class position set to sync with bounds
        /// </summary>
        public override Vector2 Position
        {
            set
            {
                position = value;
                bounds.X = (int)position.X - (int)((float)bounds.Width / 2.0f);
                bounds.Y = (int)position.Y - (int)((float)bounds.Height / 2.0f);
            }
        }
        /// <summary>
        /// Get how this slot is oriented
        /// </summary>
        protected SlotOrientation.Type orientation;
        public SlotOrientation.Type Orientation
        {
            get { return orientation; }
            set { orientation = value; }
        }
        /// <summary>
        /// The type of slot
        /// </summary>
        protected SpecialModeType specialMode;
        public SpecialModeType SpecialMode
        {
            get { return specialMode; }
            set { specialMode = value; }
        }
        /// <summary>
        /// A point multiplie for the slot
        /// </summary>
        protected int pointValue = 1;
        public int PointValue
        {
            get { return pointValue; }
            set { pointValue = value; }
        }
        /// <summary>
        /// The edge type of the slot indicates which playzone border it is against (if any)
        /// </summary>
        protected EdgeType edge;
        public EdgeType Edge
        {
            get { return edge; }
            set { edge = value; }
        }
        /// <summary>
        /// If this slot is part of a short row on the board
        /// </summary>
        protected bool isOnShortRow = false;
        public bool IsOnShortRow { get { return isOnShortRow; } set { isOnShortRow = value; } }
        /// <summary>
        /// The animated texture that will be drawn
        /// </summary>
        protected AnimatedTexture2D texture;
        public AnimatedTexture2D Texture
        {
            set { texture = value; }
            get { return texture; }
        }
        /// <summary>
        /// The color to draw this slot as
        /// </summary>
        protected Color color;
        public Color Color { set { color = value; } }

        /// <summary>
        /// Construct
        /// </summary>
        public Slot(int id, Rectangle bounds, SlotOrientation.Type orientation)
        {
            this.id = id;
            this.bounds = bounds;
            this.orientation = orientation;
            this.specialMode = SpecialModeType.NONE;
            position = new Vector2((float)bounds.X + ((float)bounds.Width / 2.0f),
                                    (float)bounds.Y + ((float)bounds.Height / 2.0f));
            this.color = Color.Green;
            edge = EdgeType.NONE;
            blockCollisionsEnabled = false;
            playerSolidCollisionsEnabled = false;
            playerBubbleCollisionsEnabled = false;

            adjacentSlots = new List<Slot>();
        }

        /// <summary>
        /// Set the block placed in this slot
        /// </summary>
        /// <param name="block"></param>
        public void SetBlock(Block block)
        {
            if (this.block != null) Debug.WriteLine("Slot::SetBlock - block " + this.block.Id + " being stomped by block " + block.Id);
            this.block = block;
            block.OwningSlot = this;
            block.Position = position;
            block.Bounds = bounds;
        }

        /// <summary>
        /// Clear the block out of this slot
        /// </summary>
        public void ClearBlock()
        {
            if (this.block == null) return;
            this.block.OwningSlot = null;
            this.block.OwningPlayer = null;
            this.block = null;
        }

        /// <summary>
        /// Update the slot
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            texture.Update(gameTime);
            if (timeForFlash >= 0.1) timeForFlash -= gameTime.ElapsedGameTime.TotalSeconds;
        }

        /// <summary>
        /// Flash info
        /// </summary>
        /// <param name="time"></param>
        /// <param name="color"></param>
        public void Flash(double time, Color color)
        {
            timeForFlash = time;
            flashColor = color;
        }

        /// <summary>
        /// Check to see if this is the edge type
        /// </summary>
        /// <returns></returns>
        public bool isTopEdge()
        {
            if (edge == EdgeType.TOP_EDGE) return true;
            if (edge == EdgeType.TOP_LEFT_EDGE) return true;
            if (edge == EdgeType.TOP_RIGHT_EDGE) return true;
            return false;
        }

        /// <summary>
        /// Check to see if this is the edge type
        /// </summary>
        /// <returns></returns>
        public bool isBottomEdge()
        {
            if (edge == EdgeType.BOTTOM_EDGE) return true;
            if (edge == EdgeType.BOTTOM_LEFT_EDGE) return true;
            if (edge == EdgeType.BOTTOM_RIGHT_EDGE) return true;
            return false;
        }

        /// <summary>
        /// Check to see if this is the edge type
        /// </summary>
        /// <returns></returns>
        public bool isLeftEdge()
        {
            if (edge == EdgeType.LEFT_EDGE) return true;
            if (edge == EdgeType.BOTTOM_LEFT_EDGE) return true;
            if (edge == EdgeType.TOP_LEFT_EDGE) return true;
            return false;
        }

        /// <summary>
        /// Check to see if this is the edge type
        /// </summary>
        /// <returns></returns>
        public bool isRightEdge()
        {
            if (edge == EdgeType.RIGHT_EDGE) return true;
            if (edge == EdgeType.BOTTOM_RIGHT_EDGE) return true;
            if (edge == EdgeType.TOP_RIGHT_EDGE) return true;
            return false;
        }

        /// <summary>
        /// Helper function to find if all adjacent slots are filled with player blocks
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool IsPlayerOwnerOfAllAdjacent(ref Player player)
        {
            foreach (Slot s in adjacentSlots)
            {
                if (s.Block == null) return false;
                if (s.Block.OwningPlayer != player) return false;
            }
            return true;
        }

        /// <summary>
        /// Helper function to find is a slot has player blocks adjacent to it.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool IsSlotAdjacentToPlayerBlocks(ref Player player)
        {
            foreach (Slot s in adjacentSlots)
            {
                if (s.Block != null &&
                    s.Block.OwningPlayer == player) return true;
            }
            return false;
        }

        /// <summary>
        /// Check to see if this slot is adjacent to lock blocks of the ref player
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public bool IsSlotAdjacentToLockedPlayerBlocks(ref Player p)
        {
            foreach (Slot s in adjacentSlots)
            {
                if (s.Block != null &&
                    s.Block.IsLocked &&
                    s.Block.OwningPlayer == p) return true;
            }
            return false;
        }

#if DEBUG
        /// <summary>
        /// Render square bounds of the hexagon
        /// </summary>
        /// <param name="batch"></param>
        public void DebugRender(PrimitiveBatch batch)
        {
            batch.Begin(PrimitiveType.LineList);
            batch.AddVertex(new Vector2(bounds.Left, bounds.Top), Color.Red);
            batch.AddVertex(new Vector2(bounds.Left, bounds.Bottom), Color.Red);

            batch.AddVertex(new Vector2(bounds.Left, bounds.Bottom), Color.Red);
            batch.AddVertex(new Vector2(bounds.Right, bounds.Bottom), Color.Red);

            batch.AddVertex(new Vector2(bounds.Right, bounds.Bottom), Color.Red);
            batch.AddVertex(new Vector2(bounds.Right, bounds.Top), Color.Red);

            batch.AddVertex(new Vector2(bounds.Right, bounds.Top), Color.Red);
            batch.AddVertex(new Vector2(bounds.Left, bounds.Top), Color.Red);
            batch.End();
        }
#endif

        /// <summary>
        /// Draw this slot
        /// </summary>
        public void Draw(SpriteBatch batch, SpriteFont spriteFont)
        {
            if (timeForFlash >= 0.1) texture.Draw(batch, bounds, flashColor);
            else if (block != null && block.OwningPlayer != null) texture.Draw(batch, bounds, block.OwningPlayer.PrimaryColor);
            else if (pointValue > 1) texture.Draw(batch, bounds, Color.Yellow);
            else if (specialMode == SpecialModeType.DEAD_SLOT && (playerSolidCollisionsEnabled || playerBubbleCollisionsEnabled))
            {
                texture.Draw(batch, bounds, Color.Black);  // draw darker than normal dead slots
            }
            else texture.Draw(batch, bounds, color);

            if (pointValue > 1)
            {
                batch.DrawString(spriteFont, "x " + pointValue.ToString(),
                    new Vector2(position.X - TEXT_OFFSET_X + 3, position.Y - TEXT_OFFSET_Y + 3),
                    Color.Black);
                batch.DrawString(spriteFont, "x " + pointValue.ToString(),
                    new Vector2(position.X - TEXT_OFFSET_X, position.Y - TEXT_OFFSET_Y),
                    Color.Yellow);
            }
        }
    }
}
