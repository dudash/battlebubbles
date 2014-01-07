#region Copyright
//-----------------------------------------------------------------------------
// Copyright (C)2007-2008 Jason Dudash, GNU GPLv3.
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
// File Created: 30 December 2008, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Storage;
using HBBB.Core;
using HBBB.Core.Graphics;
using HBBB.GameComponents.BoardComponents;
using HBBB.GameComponents.Globals;
using HBBB.GameComponents.PowerUps;
using HBBB.GameComponents.PlayerComponents;

namespace HBBB.GameComponents.LevelBuilder
{
    /// <summary>
    /// This is a game component that represents a game board.  The board is
    /// composed of a tesselation of hexagons
    /// </summary>
    class LevelBuilderBoard
    {
        public int myWidth;
        public int myHeight;

        /// <summary>
        /// The currently load level
        /// </summary>
        private BoardLevel currentLevel;
        public BoardLevel CurrentLevel { get { return currentLevel; } }
        /// <summary>
        /// The tessellation is the tiling of slots
        /// </summary>
        private Tessellation hexTiles;
        public int TileCountHorizontal { get { return hexTiles.CountHorizontal; } }
        public int TileCountVertical { get { return hexTiles.CountVertical; } }
        /// <summary>
        /// The list of slots on this board
        /// </summary>
        public List<Slot> Slots { get { return hexTiles.Slots; } }
        /// <summary>
        /// All the blocks attached to slots
        /// </summary>
        public List<Block> blocksInSlots;
        /// <summary>
        /// For managing game texture and stuff
        /// </summary>
        protected ContentManager content;

        public int SelectedSlotId = 0;

        /// <summary>
        /// The hex texture used to create new blocks
        /// </summary>
        public Texture2D hexTexture;
        /// <summary>
        /// slot selector
        /// </summary>
        public Texture2D slotSelectorTexture;
        /// <summary>
        /// Texture for the background loaded from a file
        /// </summary>
        public Texture2D gameBackground;
        /// <summary>
        /// holds fake players
        /// </summary>
        private SortedList<PlayerIndex, Player> playersList;

        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="game"></param>
        public LevelBuilderBoard(Game game)
        {
            blocksInSlots = new List<Block>();
            content = new ContentManager(game.Services);
            playersList = new SortedList<PlayerIndex, Player>(4);
            playersList.Add(PlayerIndex.One, new Player(PlayerIndex.One));
            playersList.Add(PlayerIndex.Two, new Player(PlayerIndex.Two));
            playersList.Add(PlayerIndex.Three, new Player(PlayerIndex.Three));
            playersList.Add(PlayerIndex.Four, new Player(PlayerIndex.Four));
        }

        public void SelectSlotLeft()
        {
            SelectedSlotId--;
            if (SelectedSlotId < 0) SelectedSlotId = 0;
        }

        public void SelectSlotRight()
        {
            SelectedSlotId++;
            if (SelectedSlotId >= hexTiles.Slots.Count) SelectedSlotId = hexTiles.Slots.Count - 1;
        }

        public void SelectSlotUp()
        {
            SelectedSlotId -= TileCountHorizontal;
            if ((SelectedSlotId / TileCountHorizontal + 0.5) % 2 == 0 && SelectedSlotId != 0) SelectedSlotId++;
            if (SelectedSlotId < 0) SelectedSlotId = 0;
            if (SelectedSlotId >= hexTiles.Slots.Count) SelectedSlotId = hexTiles.Slots.Count - 1;
        }

        public void SelectSlotDown()
        {
            SelectedSlotId += TileCountHorizontal;
            if ((SelectedSlotId / TileCountHorizontal + 0.5) % 2 == 0) SelectedSlotId--;
            if (SelectedSlotId < 0) SelectedSlotId = 0;
            if (SelectedSlotId >= hexTiles.Slots.Count) SelectedSlotId = hexTiles.Slots.Count - 1;
        }

        public void ClearCurrentSlot()
        {
            if (hexTiles.Slots[SelectedSlotId].Block != null)
            {
                blocksInSlots.Remove(hexTiles.Slots[SelectedSlotId].Block);
                hexTiles.Slots[SelectedSlotId].ClearBlock();
            }
            hexTiles.Slots[SelectedSlotId].SpecialMode = Slot.SpecialModeType.NONE;
            hexTiles.Slots[SelectedSlotId].PointValue = 1;
            hexTiles.Slots[SelectedSlotId].BlockCollisionsEnabled = false;
            hexTiles.Slots[SelectedSlotId].PlayerBubbleCollisionsEnabled = false;
            hexTiles.Slots[SelectedSlotId].PlayerSolidCollisionsEnabled = false;
            Texture2D slotTexture = content.Load<Texture2D>(@"W_A_D\Textures\slot_vertical");
            AnimatedTexture2D animTexture = new AnimatedTexture2D();
            animTexture.LoadContent(ref slotTexture, 1, 1);
            hexTiles.Slots[SelectedSlotId].Texture = animTexture;
        }

        public void SetCurrentSlot_P1()
        {
            ClearCurrentSlot();
            Block b = new Block(0, new Vector2());
            b.Texture = hexTexture;
            b.OwningPlayer = playersList[PlayerIndex.One];
            b.TimeTillLock = 0.0;
            hexTiles.Slots[SelectedSlotId].SetBlock(b);
            b.Lock();
            blocksInSlots.Add(b);
        }

        public void SetCurrentSlot_P2()
        {
            ClearCurrentSlot();
            ClearCurrentSlot();
            Block b = new Block(0, new Vector2());
            b.Texture = hexTexture;
            b.OwningPlayer = playersList[PlayerIndex.Two];
            b.TimeTillLock = 0.0;
            hexTiles.Slots[SelectedSlotId].SetBlock(b);
            b.Lock();
            blocksInSlots.Add(b);
        }

        public void SetCurrentSlot_P3()
        {
            ClearCurrentSlot();
            ClearCurrentSlot();
            Block b = new Block(0, new Vector2());
            b.Texture = hexTexture;
            b.OwningPlayer = playersList[PlayerIndex.Three];
            b.TimeTillLock = 0.0;
            hexTiles.Slots[SelectedSlotId].SetBlock(b);
            b.Lock();
            blocksInSlots.Add(b);
        }

        public void SetCurrentSlot_P4()
        {
            ClearCurrentSlot();
            ClearCurrentSlot();
            Block b = new Block(0, new Vector2());
            b.Texture = hexTexture;
            b.OwningPlayer = playersList[PlayerIndex.Four];
            b.TimeTillLock = 0.0;
            hexTiles.Slots[SelectedSlotId].SetBlock(b);
            b.Lock();
            blocksInSlots.Add(b);
        }

        public void SetCurrentSlot_SolidStickyBlock()
        {
            ClearCurrentSlot();
            hexTiles.Slots[SelectedSlotId].SpecialMode = Slot.SpecialModeType.DEAD_SLOT;
            Texture2D slotTexture = content.Load<Texture2D>(@"W_A_D\Textures\dead_slot_vertical");
            AnimatedTexture2D animTexture = new AnimatedTexture2D();
            animTexture.LoadContent(ref slotTexture, 1, 1);
            hexTiles.Slots[SelectedSlotId].Texture = animTexture;
            hexTiles.Slots[SelectedSlotId].PlayerSolidCollisionsEnabled = true;
        }

        public void SetCurrentSlot_BubbleStickyBlock()
        {
            ClearCurrentSlot();
            hexTiles.Slots[SelectedSlotId].SpecialMode = Slot.SpecialModeType.DEAD_SLOT;
            Texture2D slotTexture = content.Load<Texture2D>(@"W_A_D\Textures\dead_slot_vertical");
            AnimatedTexture2D animTexture = new AnimatedTexture2D();
            animTexture.LoadContent(ref slotTexture, 1, 1);
            hexTiles.Slots[SelectedSlotId].Texture = animTexture;
            hexTiles.Slots[SelectedSlotId].PlayerBubbleCollisionsEnabled = true;
        }

        public void SetCurrentSlot_BubbleAndSolidStickyBlock()
        {
            ClearCurrentSlot();
            hexTiles.Slots[SelectedSlotId].SpecialMode = Slot.SpecialModeType.DEAD_SLOT;
            Texture2D slotTexture = content.Load<Texture2D>(@"W_A_D\Textures\dead_slot_vertical");
            AnimatedTexture2D animTexture = new AnimatedTexture2D();
            animTexture.LoadContent(ref slotTexture, 1, 1);
            hexTiles.Slots[SelectedSlotId].Texture = animTexture;
            hexTiles.Slots[SelectedSlotId].PlayerSolidCollisionsEnabled = true;
            hexTiles.Slots[SelectedSlotId].PlayerBubbleCollisionsEnabled = true;
        }

        public void SetCurrentSlot_Dead()
        {
            ClearCurrentSlot();
            hexTiles.Slots[SelectedSlotId].SpecialMode = Slot.SpecialModeType.DEAD_SLOT;
            Texture2D slotTexture = content.Load<Texture2D>(@"W_A_D\Textures\dead_slot_vertical");
            AnimatedTexture2D animTexture = new AnimatedTexture2D();
            animTexture.LoadContent(ref slotTexture, 1, 1);
            hexTiles.Slots[SelectedSlotId].Texture = animTexture;
        }

        public void SetCurrentSlot_BonusPoints(int points)
        {
            ClearCurrentSlot();
            hexTiles.Slots[SelectedSlotId].PointValue = points;
        }

        public void SetCurrentSlot_Source()
        {
            ClearCurrentSlot();
            hexTiles.Slots[SelectedSlotId].SpecialMode = Slot.SpecialModeType.SOURCE_SLOT;
            Texture2D slotTexture = content.Load<Texture2D>(@"W_A_D\Textures\source_slot_vertical_SHEET");
            AnimatedTexture2D animTexture = new AnimatedTexture2D();
            animTexture.LoadContent(ref slotTexture, 3, 8);
            hexTiles.Slots[SelectedSlotId].Texture = animTexture;
        }

        /// <summary>
        /// Load the game board definition from a file
        /// </summary>
        /// <param name="fileName"></param>
        public bool LoadLevel(string fileName, StorageDevice device)
        {
            StorageContainer container = device.OpenContainer(GlobalStrings.CustomLevelsStorageIdentifier);
            XmlSerializer serializer = new XmlSerializer(typeof(BoardLevel));
            TextReader reader = new StreamReader(fileName);
            currentLevel = (BoardLevel)serializer.Deserialize(reader);
            container.Dispose();
            if (currentLevel == null) return false;
            return InitializeLevel();
        }

        /// <summary>
        /// Save the loaded level back to a file
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool SaveLevel(string fileName, StorageDevice device)
        {
            currentLevel.BonusPointsSlotIds = new List<Point>();
            currentLevel.PlayerSolidCollisionSlotIds = new List<int>();
            currentLevel.PlayerBubbleCollisionSlotIds = new List<int>();
            currentLevel.DeadSlotIds = new List<int>();
            currentLevel.SourceSlotIds = new List<int>();
            foreach (Slot s in hexTiles.Slots)
            {
                if (s.PointValue > 1)
                {
                    Point p = new Point(s.Id, s.PointValue);
                    currentLevel.BonusPointsSlotIds.Add(p);
                }
                if (s.PlayerBubbleCollisionsEnabled) currentLevel.PlayerBubbleCollisionSlotIds.Add(s.Id);
                if (s.PlayerSolidCollisionsEnabled) currentLevel.PlayerSolidCollisionSlotIds.Add(s.Id);
                if (s.SpecialMode == Slot.SpecialModeType.DEAD_SLOT) currentLevel.DeadSlotIds.Add(s.Id);
                if (s.SpecialMode == Slot.SpecialModeType.SOURCE_SLOT) currentLevel.SourceSlotIds.Add(s.Id);
            }

            // owned slots
            currentLevel.OwnedSlotIds = new List<IntPlayerIndexPair>();
            foreach (Block b in blocksInSlots)
            {
                IntPlayerIndexPair pair = new IntPlayerIndexPair(b.OwningSlot.Id, b.OwningPlayer.PlayerIndex);
                currentLevel.OwnedSlotIds.Add(pair);
            }

            // write the level to disk
            StorageContainer container = device.OpenContainer(GlobalStrings.CustomLevelsStorageIdentifier);
            XmlSerializer serializer = new XmlSerializer(typeof(BoardLevel));
            TextWriter writer = new StreamWriter(fileName);
            serializer.Serialize(writer, currentLevel);
            container.Dispose();
            return false;
        }

        /// <summary>
        /// Setup the level
        /// </summary>
        private bool InitializeLevel()
        {
            if (currentLevel == null) return false;

            SelectedSlotId = 0;
            blocksInSlots = new List<Block>();

            // create the board slots
            Color slotColor = new Color(currentLevel.SlotsColor);
            hexTiles = new Tessellation(currentLevel.PlayZone, (int)currentLevel.SlotsCount.X, (int)currentLevel.SlotsCount.Y, currentLevel.SlotOrientation);

            Texture2D slotTexture;
            if (currentLevel.SlotTextureName != "") slotTexture = content.Load<Texture2D>(@currentLevel.SlotTextureName);
            else slotTexture = content.Load<Texture2D>(@"W_A_D\Textures\white");
            foreach (Slot s in hexTiles.Slots)
            {
                AnimatedTexture2D animTexture = new AnimatedTexture2D();
                animTexture.LoadContent(ref slotTexture, 1, 1);
                s.Texture = animTexture;
                s.Color = slotColor;
            }
            // fit the playzone to the generated slots
            //currentLevel.PlayZone = hexTiles.GetTightPlayzone();

            // set the source slots
            if (currentLevel.SourceSlotIds != null)
            {
                Texture2D sourceTexture = content.Load<Texture2D>(@"W_A_D\Textures\source_slot_vertical_SHEET");
                foreach (int i in currentLevel.SourceSlotIds)
                {
                    if (i >= hexTiles.Slots.Count) continue;
                    AnimatedTexture2D animTexture = new AnimatedTexture2D();
                    animTexture.LoadContent(ref sourceTexture, 3, 8);
                    hexTiles.Slots[i].Texture = animTexture;
                    hexTiles.Slots[i].SpecialMode = Slot.SpecialModeType.SOURCE_SLOT;
                }
            }
            // set the dead slots
            if (currentLevel.DeadSlotIds != null)
            {
                Texture2D deadSlotTexture;
                if (currentLevel.DeadSlotTextureName != "") deadSlotTexture = content.Load<Texture2D>(@currentLevel.DeadSlotTextureName);
                else deadSlotTexture = content.Load<Texture2D>(@"W_A_D\Textures\black");
                foreach (int i in currentLevel.DeadSlotIds)
                {
                    if (i >= hexTiles.Slots.Count) continue;
                    AnimatedTexture2D animTexture = new AnimatedTexture2D();
                    animTexture.LoadContent(ref deadSlotTexture, 1, 1);
                    hexTiles.Slots[i].Texture = animTexture;
                    hexTiles.Slots[i].SpecialMode = Slot.SpecialModeType.DEAD_SLOT;
                }
            }
            if (currentLevel.PlayerSolidCollisionSlotIds != null)
            {
                // make these slots solid so players collide with them
                foreach (int i in currentLevel.PlayerSolidCollisionSlotIds)
                {
                    if (i >= hexTiles.Slots.Count) continue;
                    hexTiles.Slots[i].PlayerSolidCollisionsEnabled = true;
                }
            }
            if (currentLevel.PlayerBubbleCollisionSlotIds != null)
            {
                // make these slots solid so players collide with them
                foreach (int i in currentLevel.PlayerBubbleCollisionSlotIds)
                {
                    if (i >= hexTiles.Slots.Count) continue;
                    hexTiles.Slots[i].PlayerBubbleCollisionsEnabled = true;
                }
            }
            if (currentLevel.BonusPointsSlotIds != null)
            {
                foreach (Point p in currentLevel.BonusPointsSlotIds)
                {
                    if (hexTiles.Slots.Count > p.X) // if the slot exists
                    {
                        hexTiles.Slots[p.X].PointValue = p.Y;
                    }
                }
            }

            // load hex block textures
            hexTexture = content.Load<Texture2D>(@"W_A_D\Textures\hex_vertical");

            // distribute a block into each owned slot
            int curId = 0;
            foreach (IntPlayerIndexPair pair in currentLevel.OwnedSlotIds)
            {
                Block b = new Block(curId, new Vector2());
                b.Texture = hexTexture;
                curId++;
                b.OwningPlayer = playersList[pair.PlayerIndex];
                b.TimeTillLock = 0.0;
                if (pair.IntValue > hexTiles.Slots.Count) continue;
                Slot s = hexTiles.Slots[pair.IntValue];
                s.SetBlock(b);
                b.Lock();
                blocksInSlots.Add(b);
            }

            return true;
        }

        /// <summary>
        /// Draw this game components
        /// </summary>
        /// <param name="gameTime"></param>
        public void DrawBoard(GameTime gameTime, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            // draw background texture
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            spriteBatch.Draw(gameBackground,
                new Vector2(myWidth / 2, myHeight / 2),
                null, Color.White, (float)gameTime.TotalGameTime.TotalMinutes,
                new Vector2(gameBackground.Width / 2, gameBackground.Height / 2),
                1.5f, SpriteEffects.None, 0.0f);

            // draw slots
            foreach (Slot slot in hexTiles.Slots)
            {
                slot.Draw(spriteBatch, spriteFont);

                //indicate sticky blocks
                if (slot.PlayerBubbleCollisionsEnabled || slot.PlayerSolidCollisionsEnabled) spriteBatch.DrawString(spriteFont, "Stick", slot.Position+new Vector2(-20, -20), Color.Gold);
                if (slot.PlayerBubbleCollisionsEnabled) spriteBatch.DrawString(spriteFont, "(B)", slot.Position + new Vector2(-20, -5), Color.Gold);
                if (slot.PlayerSolidCollisionsEnabled) spriteBatch.DrawString(spriteFont, "(S)", slot.Position + new Vector2(0, -5), Color.Gold);
            }

            // draw blocks
            foreach (Block block in blocksInSlots)
            {
                block.Draw(spriteBatch, spriteFont);
            }

            // draw selected slot identifier
            Slot s = hexTiles.Slots[SelectedSlotId];
            Color color = Color.Red;
            if (gameTime.TotalGameTime.Milliseconds % 500 > 250) color = Color.Black;
            spriteBatch.Draw(slotSelectorTexture, s.Bounds, color);
            spriteBatch.End();
        }
    }
}
