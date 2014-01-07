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
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HBBB.GameComponents.PlayerComponents;
using HBBB.Core.Graphics;
using HBBB.GameComponents.Globals;

namespace HBBB.GameComponents.BoardComponents
{
    /// <summary>
    /// A block that is the shape of a hexagon.
    /// </summary>
    internal class Block : Core.Entity2D, IBubblePayload
    {
        public float BLOCK_SCALE_MAGIC_NUMBER = 0.20f;  // temp scaling of hex texture to proper size
        public float SLOT_SCALE_MAGIC_NUMBER = 0.30f;  // temp scaling of hex texture to proper size
        public float FLASH_SCALE_MAGIC_NUMBER = 0.40f; // temp scaling of hex texture to proper size

        public const float DEFAULT_BOUNDING_RADIUS = 25.0f;
        public const float DEFAULT_GRAB_RADIUS = 30.0f;
        private const int DEFAULT_TIME_TILL_LOCK = 20;
        private const int TEXT_OFFSET_X = 5;
        private const int TEXT_OFFSET_Y = 10;

        /// <summary>
        /// A bounding box around this block (also used for texture size)
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
        /// The radius of the bounding circle
        /// </summary>
        protected float boundingRadius;
        public float BoundingRadius
        {
            get { return boundingRadius; }
            set { boundingRadius = value; }
        }
        /// <summary>
        /// The radius of the bounding circle used for bubble pickup checks
        /// </summary>
        protected float grabRadius;
        public float GrabRadius
        {
            get { return grabRadius; }
            set { grabRadius = value; }
        }
        /// <summary>
        /// Override the base class position set to sync with bounds
        /// </summary>
        public override Vector2 Position
        {
            set
            {
                position = value;
                // move the bounds to be centered around the new position
                bounds.X = (int)position.X - (int)((float)bounds.Width / 2.0f);
                bounds.Y = (int)position.Y - (int)((float)bounds.Height / 2.0f);
            }
        }
        /// <summary>
        /// The owner id of this block, 0 if no owner
        /// </summary>
        protected Player owningPlayer;
        public Player OwningPlayer
        {
            get { return owningPlayer; }
            set { owningPlayer = value; }
        }
        /// <summary>
        /// Reference to a slot owner (null if none)
        /// </summary>
        protected Slot owningSlot;
        public Slot OwningSlot
        {
            get { return owningSlot; }
            set
            {
                owningSlot = value;
                if (owningSlot == null) // detach from a slot
                {
                    scale = BLOCK_SCALE_MAGIC_NUMBER;
                    timeTillLock = DEFAULT_TIME_TILL_LOCK;
                    isLocked = false;
                }
                else // attach ourselves to the slot
                {
                    rotation = 0.0f;
                    position = owningSlot.Position;
                }
            }
        }
        /// <summary>
        /// Flag to indicate if this block has been lock to the board
        /// </summary>
        private bool isLocked = false;
        public bool IsLocked
        {
            get { return isLocked; }
        }
        /// <summary>
        /// The amount of time remaining before this block locks
        /// </summary>
        private double timeTillLock = DEFAULT_TIME_TILL_LOCK;  // in seconds
        public double TimeTillLock
        {
            get { return timeTillLock; }
            set { timeTillLock = value; }
        }
        /// <summary>
        /// The amount of time this block has been in the slot
        /// </summary>
        private double timeInSlot = 0.0;  // in seconds
        public double TimeInSlot
        {
            get { return timeInSlot; }
            set { timeInSlot = value; }
        }
        /// <summary>
        /// An amount of time to flash the block big
        /// </summary>
        private double timeForFlash = 0.0f;
        /// <summary>
        /// An identifier for this block
        /// </summary>
        protected int id;
        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        /// <summary>
        /// The texture that will be drawn
        /// </summary>
        protected Texture2D texture;
        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }
        /// <summary>
        /// The texture scale
        /// </summary>
        protected float scale;
        public float Scale { set { scale = value; } }
        /// <summary>
        /// The speed at which to rotate the block
        /// </summary>
        protected float rotationSpeed;
        public float RotationSpeed
        {
            get { return rotationSpeed; }
            set { rotationSpeed = value; }
        }
        /// <summary>
        /// The velocity of this block in 2 dimensions
        /// </summary>
        protected Vector2 velocity;
        public Vector2 Velocity
        {
            get { return velocity; }
            set 
            {
                velocity = value;
                if (float.IsNaN(velocity.X) || float.IsNaN(velocity.Y)) velocity = new Vector2(0.0f, 0.0f);
            }
        }

        /// <summary>
        /// Construct
        /// </summary>
        public Block(int id, Vector2 position, float boundingRadius)
        {
            this.id = id;
            this.position = position;
            this.velocity = new Vector2(0.0f, 0.0f);
            this.boundingRadius = boundingRadius;
            this.grabRadius = DEFAULT_GRAB_RADIUS;
            scale = BLOCK_SCALE_MAGIC_NUMBER;
        }

        /// <summary>
        /// Construct
        /// </summary>
        public Block(int id, Vector2 position)
        {
            this.id = id;
            this.position = position;
            this.velocity = new Vector2(0.0f, 0.0f);
            this.boundingRadius = DEFAULT_BOUNDING_RADIUS;
            this.grabRadius = DEFAULT_GRAB_RADIUS;
            scale = BLOCK_SCALE_MAGIC_NUMBER;
        }

        /// <summary>
        /// Update the block physics
        /// </summary>
        public virtual void Update(GameTime gameTime)
        {
            // check for flash
            timeForFlash -= gameTime.ElapsedGameTime.TotalSeconds;
            if (timeForFlash <= 0.1)
            {
                if (isLocked) scale = SLOT_SCALE_MAGIC_NUMBER;
                else scale = BLOCK_SCALE_MAGIC_NUMBER;
            }
            else
            {
                scale = FLASH_SCALE_MAGIC_NUMBER;
            }

            if (isLocked) return;
            if (owningPlayer == null)  // if we are "open" move around and spin
            {
                rotation += rotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                position += velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else // a player is carrying us or we are in a slot
            {
                if (owningSlot == null) // we are in a bubble
                {
                    position = owningPlayer.Bubble.CenterPoint.Position;
                }
                else // we are attached to a slot
                {
                    if (!isLocked) // check to see if it's time to lock into our slot
                    {
                        timeInSlot += gameTime.ElapsedGameTime.TotalSeconds;
                        timeTillLock -= gameTime.ElapsedGameTime.TotalSeconds;
                        if (timeTillLock <= 0.1)
                        {
                            Lock();
                            owningPlayer.Statistics.LockedBlockPoints += owningSlot.PointValue; // update stats
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Lock this block
        /// </summary>
        public void Lock()
        {
            if (owningPlayer == null || owningSlot == null) return; // cant lock unless we have an owner and a slot
            isLocked = true;
            scale = SLOT_SCALE_MAGIC_NUMBER;
            GameAudio.PlayCue("lock_block");
        }

        /// <summary>
        /// Flash this block big for the specified period of time
        /// </summary>
        public void AnimateFlash(double flashTime)
        {
            timeForFlash = flashTime;
        }

        /// <summary>
        /// Draw this block
        /// </summary>
        public virtual void Draw(SpriteBatch batch, SpriteFont spriteFont)
        {
            if (owningPlayer != null && owningSlot != null)  // draw owning player color
            {
                batch.Draw(texture, position, new Rectangle(0, 0, texture.Width, texture.Height), owningPlayer.PrimaryColor,
                    rotation, new Vector2(texture.Width / 2.0f, texture.Height / 2.0f), scale, SpriteEffects.None, 0);
            }
            else  // draw white
            {
                batch.Draw(texture, position, new Rectangle(0, 0, texture.Width, texture.Height), Color.White,
                    rotation, new Vector2(texture.Width / 2.0f, texture.Height / 2.0f), scale, SpriteEffects.None, 0);
            }

            if (owningSlot != null && !isLocked)  // draw time till lock
            {
                if (timeTillLock <= 10.0)
                {
                    batch.DrawString(spriteFont, timeTillLock.ToString("0"),
                        new Vector2(position.X - TEXT_OFFSET_X, position.Y - TEXT_OFFSET_Y),
                        owningPlayer.SecondaryColor);
                }
                else
                {
                    batch.DrawString(spriteFont, timeTillLock.ToString("0"),
                        new Vector2(position.X - TEXT_OFFSET_X - 5, position.Y - TEXT_OFFSET_Y),
                        owningPlayer.SecondaryColor);
                }
            }
        }

#if DEBUG
        /// <summary>
        /// Render square bounds of the hexagon
        /// </summary>
        /// <param name="batch"></param>
        public void DebugRender(PrimitiveBatch batch)
        {
            batch.Begin(PrimitiveType.LineList);

            batch.AddVertex(new Vector2(position.X - boundingRadius, position.Y), Color.Red);
            batch.AddVertex(new Vector2(position.X + boundingRadius, position.Y), Color.Red);

            batch.AddVertex(new Vector2(position.X, position.Y - boundingRadius), Color.Red);
            batch.AddVertex(new Vector2(position.X, position.Y + boundingRadius), Color.Red);

            batch.End();
        }
#endif
    }
}
