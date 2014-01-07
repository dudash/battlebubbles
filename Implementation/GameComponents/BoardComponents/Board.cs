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
using HBBB.GameComponents.Globals;
using HBBB.GameComponents.PowerUps;
using HBBB.GameComponents.PlayerComponents;

namespace HBBB.GameComponents.BoardComponents
{
    /// <summary>
    /// This is a game component that represents a game board.  The board is
    /// composed of a tesselation of hexagons
    /// </summary>
    class Board : DrawableGameComponent
    {
        private const float DEFAULT_TIME_TILL_NEXT_POWERUP = 30.0f;
        private const int MINIMUM_BLOCK_EMISSION_INTERVAL = 15;

        /// <summary>
        /// Tell the board if the game is running
        /// </summary>
        private bool inGameFlag = false;
        public bool InGameFlag
        {
            get { return inGameFlag; }
            set { inGameFlag = value; }
        }
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
        /// List of source slots
        /// </summary>
        private List<Slot> sourceSlots;
        public List<Slot> SourceSlots { get { return sourceSlots; } }
        /// <summary>
        /// List of dead slots
        /// </summary>
        private List<Slot> deadSlots;
        public List<Slot> DeadSlots { get { return deadSlots; } }
        /// <summary>
        /// List of slots along the playzone border
        /// </summary>
        private List<Slot> bottomEdgeSlots;
        public List<Slot> BottomEdgeSlots { get { return bottomEdgeSlots; } }
        /// <summary>
        /// List of slots along the playzone border
        /// </summary>
        private List<Slot> leftEdgeSlots;
        public List<Slot> LeftEdgeSlots { get { return leftEdgeSlots; } }
        /// <summary>
        /// List of slots along the playzone border
        /// </summary>
        private List<Slot> rightEdgeSlots;
        public List<Slot> RightEdgeSlots { get { return rightEdgeSlots; } }
        /// <summary>
        /// List of slots along the playzone border
        /// </summary>
        private List<Slot> topEdgeSlots;
        public List<Slot> TopEdgeSlots { get { return topEdgeSlots; } }
        /// <summary>
        /// All blocks on the board (in play and in slots)
        /// </summary>
        private List<Block> blocks;
        public List<Block> Blocks { get { return blocks; } }
        /// <summary>
        /// All the blocks free on the board
        /// </summary>
        private List<Block> freeBlocksInPlay;
        public List<Block> FreeBlocksInPlay { get { return freeBlocksInPlay; } }
        /// <summary>
        /// All the blocks attached to slots
        /// </summary>
        public List<Block> blocksInSlots;
        public List<Block> BlocksInSlots { get { return blocksInSlots; } }
        /// <summary>
        /// All the blocks we are doing collision detection on and drawing
        /// </summary>
        public List<Block> allBlocksInPlay;
        public List<Block> AllBlocksInPlay { get { return allBlocksInPlay; } }
        /// <summary>
        /// All the pre-allocated blocks to be used in the level
        /// </summary>
        private Queue<Block> blockPreallocation;
        public int BlockPreAllocationCount { get { return blockPreallocation.Count; } }
        /// <summary>
        /// All the pre-allocated powerups to be used in the level
        /// </summary>
        private Queue<PowerUp> powerupPreallocation;
        /// <summary>
        /// All Powerups that are currently on the board (inactive and active)
        /// </summary>
        private List<PowerUp> powerups;
        public List<PowerUp> PowerUps { get { return powerups; } }
        /// <summary>
        /// Time in seconds until the next powerup is emitted
        /// </summary>
        float timeTillNextPowerUp;
        /// <summary>
        /// Time in seconds that has ellapsed since the last block was emitted
        /// </summary>
        float timeSinceLastBlockEmit;
        /// <summary>
        /// A list of obstructions for all the off limit parts of the board
        /// </summary>
        public List<Obstruction> obstructions;
        public List<Obstruction> Obstructions { get { return obstructions; } }
        /// <summary>
        /// A set of players in the game
        /// </summary>
        private SortedList<PlayerIndex, Player> playersList;
        public SortedList<PlayerIndex, Player> PlayersList { get { return playersList; } }
        /// <summary>
        /// For managing game texture and stuff
        /// </summary>
        protected ContentManager content;
        /// <summary>
        /// Primitive batch for drawing
        /// </summary>
        PrimitiveBatch primitiveBatch;
        /// <summary>
        /// Sprite batch for drawing textures
        /// </summary>
        SpriteBatch spriteBatch;
        /// <summary>
        /// for text sprites
        /// </summary>
        SpriteFont spriteFont;
        /// <summary>
        /// The hex texture used to create new blocks
        /// </summary>
        Texture2D hexTexture;
        /// <summary>
        /// Texture for the background loaded from a file
        /// </summary>
        Texture2D gameBackground;
        /// <summary>
        /// A testing effect to apply to the board
        /// </summary>
        Effect boardEffect;
#if DEBUG
        /// <summary>
        /// in debug mode, draw some info on the screen
        /// </summary>
        public string DebugText = "No Debug Info";
#endif

        /// <summary>
        /// Construct
        /// </summary>
        /// <param name="game"></param>
        public Board(Game game)
            : base(game)
        {
            freeBlocksInPlay = new List<Block>();
            allBlocksInPlay = new List<Block>();
            blocks = new List<Block>();
            blocksInSlots = new List<Block>();
            powerups = new List<PowerUp>();
            obstructions = new List<Obstruction>();
            content = new ContentManager(game.Services);
            playersList = new SortedList<PlayerIndex, Player>(4);
            timeTillNextPowerUp = DEFAULT_TIME_TILL_NEXT_POWERUP;
        }

        /// <summary>
        /// Return the dimension of the valid play zone of the board
        /// </summary>
        /// <returns></returns>
        public Rectangle GetBoardPlayZoneDimensions()
        {
            if (currentLevel != null) return currentLevel.PlayZone;
            return new Rectangle(0, 0, this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height);
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
            // TODO player win rules and spawn locations
            // TODO write obstructions to level
            // TODO write slots color,texture
            // TODO ambient forces
            // TODO playzone
            // TODO slots count
            // TODO slots orientation
            // TODO powerups

            // source slots
            currentLevel.SourceSlotIds.Clear();
            foreach (Slot s in SourceSlots)
            {
                currentLevel.SourceSlotIds.Add(s.Id);
            }

            // bonus points slots, player solid collision slots, player bubble collision slots
            currentLevel.BonusPointsSlotIds.Clear();
            currentLevel.PlayerSolidCollisionSlotIds.Clear();
            currentLevel.PlayerBubbleCollisionSlotIds.Clear();
            foreach (Slot s in hexTiles.Slots)
            {
                if (s.PointValue > 1)
                {
                    Point p = new Point(s.Id, s.PointValue);
                    currentLevel.BonusPointsSlotIds.Add(p);
                }
                if (s.PlayerBubbleCollisionsEnabled) currentLevel.PlayerBubbleCollisionSlotIds.Add(s.Id);
                if (s.PlayerSolidCollisionsEnabled) currentLevel.PlayerSolidCollisionSlotIds.Add(s.Id);
            }

            // dead slots
            currentLevel.DeadSlotIds.Clear();
            foreach (Slot s in deadSlots)
            {
                currentLevel.DeadSlotIds.Add(s.Id);
            }

            // owned slots
            currentLevel.OwnedSlotIds.Clear();
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

#if DEBUG
        /// <summary>
        /// Temp for testing
        /// </summary>
        /// <param name="fileName"></param>
        public void MakeLevel(string fileName)
        {
            currentLevel = new BoardLevel();

            currentLevel.Player1SpawnPoint = new Vector2(100, 100);
            currentLevel.Player2SpawnPoint = new Vector2(1180, 100);
            currentLevel.Player3SpawnPoint = new Vector2(100, 700);
            currentLevel.Player4SpawnPoint = new Vector2(1180, 700);

            currentLevel.PlayZone = new Rectangle(0, 0, this.GraphicsDevice.Viewport.Width, this.GraphicsDevice.Viewport.Height);

            List<ObstructionRectangle> rectObstrs = new List<ObstructionRectangle>();
            currentLevel.ObstructionRectangles = rectObstrs;
            rectObstrs.Add(new ObstructionRectangle(new Rectangle(500, 250, 100, 100))); // temp testing obstruction
            rectObstrs.Add(new ObstructionRectangle(new Rectangle(150, 150, 200, 200))); // temp testing obstruction

            currentLevel.BackgroundTextureName = @"W_A_D\Textures\game_background";

            List<string> puIds = new List<string>();
            puIds.Add(PowerUpFactory.POWERUP_STR_SLOW_SPEED);
            currentLevel.PowerUpIds = puIds;

            currentLevel.SourceSlotTextureName = @"W_A_D\Textures\white";

            List<Point> bonusSlots = new List<Point>();
            bonusSlots.Add(new Point(20, 2));
            bonusSlots.Add(new Point(40, 3));
            bonusSlots.Add(new Point(60, 5));
            currentLevel.BonusPointsSlotIds = bonusSlots;

            currentLevel.SlotOrientation = SlotOrientation.Type.ORIENTED_POINTS_VERTICAL;
            currentLevel.SlotsCount = new Vector2(16, 10);
            currentLevel.SlotTextureName = @"W_A_D\Textures\slot_vertical";
            currentLevel.SlotsColor = Color.Green.ToVector4();

            List<IntPlayerIndexPair> bases = new List<IntPlayerIndexPair>();
            bases.Add(new IntPlayerIndexPair(1, PlayerIndex.One));
            bases.Add(new IntPlayerIndexPair(2, PlayerIndex.Two));
            bases.Add(new IntPlayerIndexPair(3, PlayerIndex.Three));
            bases.Add(new IntPlayerIndexPair(4, PlayerIndex.Four));
            currentLevel.OwnedSlotIds = bases;

            BoardLevel.WinRules rules = new BoardLevel.WinRules();
            currentLevel.Player1WinRules = rules;
            currentLevel.Player2WinRules = rules;
            currentLevel.Player3WinRules = rules;
            currentLevel.Player4WinRules = rules;

            XmlSerializer serializer = new XmlSerializer(typeof(BoardLevel));
            TextWriter writer = new StreamWriter(fileName);
            serializer.Serialize(writer, currentLevel);
        }
#endif

        /// <summary>
        /// Setup the level
        /// </summary>
        private bool InitializeLevel()
        {
            if (currentLevel == null) return false;

            // clear out old board variables
            freeBlocksInPlay = new List<Block>();
            allBlocksInPlay = new List<Block>();
            blocks = new List<Block>();
            blocksInSlots = new List<Block>();
            powerups = new List<PowerUp>();
            obstructions = new List<Obstruction>();
            sourceSlots = new List<Slot>();
            deadSlots = new List<Slot>();
            topEdgeSlots = new List<Slot>();
            bottomEdgeSlots = new List<Slot>();
            leftEdgeSlots = new List<Slot>();
            rightEdgeSlots = new List<Slot>();

            // load background from level data
            //backgroundTexture = content.Load<Texture2D>(@currentLevel.BackgroundTextureName);

            // load obstructions from level data
            foreach (ObstructionRectangle or in currentLevel.ObstructionRectangles)
            {
                if (or.TextureName != "") or.Texture = content.Load<Texture2D>(@or.TextureName);
                obstructions.Add(or);
            }
            foreach (ObstructionCircle oc in currentLevel.ObstructionCircles)
            {
                if (oc.TextureName != "") oc.Texture = content.Load<Texture2D>(@oc.TextureName);
                obstructions.Add(oc);
            }
            foreach (ObstructionLattice ol in currentLevel.ObstructionLattices)
            {
                if (ol.TextureName != "") ol.Texture = content.Load<Texture2D>(@ol.TextureName);
                obstructions.Add(ol);
            }

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
            currentLevel.PlayZone = hexTiles.GetTightPlayzone();

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
                    sourceSlots.Add(hexTiles.Slots[i]);
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
                    deadSlots.Add(hexTiles.Slots[i]);
                }
            }
            // set the bubble popping slots
            if (currentLevel.BubblePoppingSlotIds != null)
            {
                Texture2D bubblePoppingSlotTexture = content.Load<Texture2D>(@"W_A_D\Textures\bubble_popping_slot_vertical_SHEET");
                foreach (int i in currentLevel.BubblePoppingSlotIds)
                {
                    if (i >= hexTiles.Slots.Count) continue;
                    AnimatedTexture2D animTexture = new AnimatedTexture2D();
                    animTexture.LoadContent(ref bubblePoppingSlotTexture, 4, 8);
                    animTexture.Paused = false;
                    //animTexture.OffsetStartTime();  // TODO is this hanging the game?
                    hexTiles.Slots[i].Texture = animTexture;
                    hexTiles.Slots[i].SpecialMode = Slot.SpecialModeType.BUBBLE_POPPING_SLOT;
                    hexTiles.Slots[i].PlayerSolidCollisionsEnabled = true;
                    hexTiles.Slots[i].PlayerBubbleCollisionsEnabled = true;
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
            // load the edge slot lists
            foreach (Slot s in hexTiles.Slots)
            {
                if (s.Edge == Slot.EdgeType.TOP_EDGE) topEdgeSlots.Add(s);
                else if (s.Edge == Slot.EdgeType.BOTTOM_EDGE) bottomEdgeSlots.Add(s);
                else if (s.Edge == Slot.EdgeType.LEFT_EDGE) leftEdgeSlots.Add(s);
                else if (s.Edge == Slot.EdgeType.RIGHT_EDGE) rightEdgeSlots.Add(s);
            }

            // load hex block textures
            hexTexture = content.Load<Texture2D>(@"W_A_D\Textures\hex_vertical");

            // pre-create the blocks to be emitted
            int curId = 0;
            blockPreallocation = new Queue<Block>();
            foreach (Slot s in hexTiles.Slots)
            {
                if (s.SpecialMode != Slot.SpecialModeType.DEAD_SLOT &&
                    s.SpecialMode != Slot.SpecialModeType.SOURCE_SLOT)
                {
                    Block b = new Block(curId, new Vector2());
                    b.Texture = hexTexture;
                    blockPreallocation.Enqueue(b);
                    curId++;
                }
            }
            // pre-create the powerups to be emitted
            powerupPreallocation = new Queue<PowerUp>();
            foreach (string str in currentLevel.PowerUpIds)
            {
                PowerUp pup = PowerUpFactory.CreatePowerUp(str, new Vector2());
                powerupPreallocation.Enqueue(pup);
                curId++;
            }
            timeTillNextPowerUp = currentLevel.TimeBetweenPowerups;

            // distribute a block into each owned slot
            foreach (IntPlayerIndexPair pair in currentLevel.OwnedSlotIds)
            {
                Block b = blockPreallocation.Dequeue();
                b.OwningPlayer = playersList[pair.PlayerIndex];
                b.TimeTillLock = 0.0;
                if (pair.IntValue > hexTiles.Slots.Count) continue;
                Slot s = hexTiles.Slots[pair.IntValue];
                s.SetBlock(b);
                b.Lock();
                blocksInSlots.Add(b);
                blocks.Add(b);
            }

            return true;
        }

        /// <summary>
        /// Load graphics device dependant objects
        /// </summary>
        protected override void LoadContent()
        {
            gameBackground = content.Load<Texture2D>(@"W_A_D\Textures\game_background");
            // load power up textures
            Texture2D pupTexture = content.Load<Texture2D>(@"W_A_D\Textures\powerup_slow_vertical");
            PowerUpFactory.SetTexture(PowerUpFactory.PowerUpId.POWERUP_SLOW_SPEED, pupTexture);
            pupTexture = content.Load<Texture2D>(@"W_A_D\Textures\powerup_lockall_vertical");
            PowerUpFactory.SetTexture(PowerUpFactory.PowerUpId.POWERUP_LOCK_ALL, pupTexture);
            pupTexture = content.Load<Texture2D>(@"W_A_D\Textures\powerup_popunlocked_vertical");
            PowerUpFactory.SetTexture(PowerUpFactory.PowerUpId.POWERUP_POP_UNLOCKED, pupTexture);
            pupTexture = content.Load<Texture2D>(@"W_A_D\Textures\powerup_fasttransition_vertical");
            PowerUpFactory.SetTexture(PowerUpFactory.PowerUpId.POWERUP_FAST_TRANSITION, pupTexture);
            pupTexture = content.Load<Texture2D>(@"W_A_D\Textures\powerup_slowtransition_vertical");
            PowerUpFactory.SetTexture(PowerUpFactory.PowerUpId.POWERUP_SLOW_TRANSITION, pupTexture);
            pupTexture = content.Load<Texture2D>(@"W_A_D\Textures\powerup_emissionfrenzy_vertical");
            PowerUpFactory.SetTexture(PowerUpFactory.PowerUpId.EMISSION_FRENZY, pupTexture);
            pupTexture = content.Load<Texture2D>(@"W_A_D\Textures\powerup_fast_vertical");
            PowerUpFactory.SetTexture(PowerUpFactory.PowerUpId.POWERUP_FAST_SPEED, pupTexture);
            pupTexture = content.Load<Texture2D>(@"W_A_D\Textures\powerup_blocksteal_vertical");
            PowerUpFactory.SetTexture(PowerUpFactory.PowerUpId.POWERUP_BLOCK_STEAL, pupTexture);

            // load effects
            boardEffect = content.Load<Effect>(@"W_A_D\Shaders\shader_boardtest1");

            // setup rendering objects
            spriteBatch = new SpriteBatch(this.GraphicsDevice);
            primitiveBatch = new PrimitiveBatch(this.GraphicsDevice);
            spriteFont = content.Load<SpriteFont>(@"W_A_D\Fonts\Arial 14");

            // call the base class
            base.LoadContent();
        }

        /// <summary>
        /// Unload this bitch
        /// </summary>
        protected override void UnloadContent()
        {
            content.Unload();
            base.UnloadContent();
        }

        /// <summary>
        /// Dequeue a block for use on the board
        /// </summary>
        /// <returns></returns>
        public Block AddBlockToBoard(Vector2 position)
        {
            if (blockPreallocation.Count < 1)
            {
                Debug.WriteLine("Board::AddBlockToBoard - No more blocks to add");
                return null;
            }
            Block block = blockPreallocation.Dequeue();
            block.Position = position;
            freeBlocksInPlay.Add(block);
            allBlocksInPlay.Add(block);
            blocks.Add(block);
            GameAudio.PlayCue("spawn_block");

            return block;
        }

        /// <summary>
        /// Set the argument block in the argument slot
        /// </summary>
        /// <param name="b"></param>
        /// <param name="s"></param>
        public void SetBlockInSlot(Block block, Slot slot)
        {
            slot.SetBlock(block);
            freeBlocksInPlay.Remove(block);
            allBlocksInPlay.Remove(block);
            blocksInSlots.Add(block);
            GameAudio.PlayCue("drop_block");
#if DEBUG
            DebugText = "player: " + block.OwningPlayer.PlayerIndex + " dropping block: " + block.Id + " into slot: " + slot.Id;
#endif
        }

        /// <summary>
        ///  pulls the argument block out of its slot (steal)
        /// </summary>
        /// <param name="block"></param>
        public void PullBlockFromSlot(Block block)
        {
            block.OwningPlayer.Statistics.BlocksSet--;
            block.OwningSlot.ClearBlock(); // pop it out
            blocksInSlots.Remove(block);
            freeBlocksInPlay.Add(block);
            allBlocksInPlay.Add(block);
            GameAudio.PlayCue("pull_block");
        }

        /// <summary>
        /// Create a powerup and release it onto the board
        /// </summary>
        /// <param name="pupId"></param>
        public void CreateAndEmitPowerUp(PowerUpFactory.PowerUpId pupId, Vector2 position)
        {
            PowerUp pup = PowerUpFactory.CreatePowerUp(pupId, position);
            blocks.Add(pup);  // master list
            powerups.Add(pup);  // for processing the powerup
            allBlocksInPlay.Add(pup); // for collisions and drawing
        }

        /// <summary>
        /// Dequeue a powerup for use on the board
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public PowerUp AddPowerUpToBoard(Vector2 position)
        {
            if (powerupPreallocation.Count < 1) return null;
            PowerUp pup = powerupPreallocation.Dequeue();
            pup.Position = position;
            blocks.Add(pup);  // master list
            powerups.Add(pup);  // for processing the powerup
            allBlocksInPlay.Add(pup); // for collisions and drawing
            GameAudio.PlayCue("spawn_block");

            return pup;
        }

        /// <summary>
        /// The argument power is finished, remove it from execution
        /// </summary>
        /// <param name="pup"></param>
        public void RemovePowerUpFromPlay(PowerUp pup)
        {
            pup.FinishExecute();
            powerups.Remove(pup);
            pup.OwningPlayer = null;
        }

        /// <summary>
        /// Set the arg powerup to execute in the arg slot
        /// </summary>
        /// <param name="pup"></param>
        /// <param name="slot"></param>
        public void SetPowerUpInSlot(PowerUp pup, Slot slot)
        {
            if (pup.IsPositive) GameAudio.PlayCue("powerup");
            else GameAudio.PlayCue("powerdown");

            blocks.Remove(pup);  // no more collision detection needed
            allBlocksInPlay.Remove(pup);  // no more drawing needed

            Player affectedPlayer = slot.Block.OwningPlayer;
            pup.Execute(this, ref affectedPlayer, ref slot);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (!inGameFlag) return;  // dont update if we aren't in game

            #region process all blocks
            foreach (Block b in blocks)
            {
                b.Update(gameTime);
                if (b.OwningPlayer != null) continue;  // no collision for owned blocks

                #region Collision Detection
                // check block to block collision
                foreach (Block b2 in allBlocksInPlay)
                {
                    if (b2 == b || b2.OwningPlayer != null) continue;  // cant collide ourselves of owned blocks
                    if (Core.Math.CollisionDetective.CheckCollision(b.Position, b.BoundingRadius, b2.Position, b2.BoundingRadius))
                    {
                        Vector2 diff = b.Position - b2.Position;
                        diff.Normalize();
                        if (float.IsNaN(diff.X) || float.IsNaN(diff.Y))
                        {
                            Debug.WriteLine("Board::Update - block to block collision trying to move block to NaN! ignoring");
                            continue;
                        }
                        b.Position += diff;
                        b2.Position -= diff;
                        Vector2 vAdd = b.Velocity + b2.Velocity;
                        vAdd.Normalize();
                        b.Velocity *= vAdd;  // TODO this is a little mucked.  Same direction collisions invert velocity (should stick and push)
                        b2.Velocity *= -vAdd;
                    }
                }

                // check block to obstructions collision (to keep out)
                foreach (ObstructionRectangle or in currentLevel.ObstructionRectangles)
                {
                    //if (Core.Math.CollisionDetective.CheckCollision(b.Position, b.BoundingRadius, or.Position, or.Bounds.X))
                    //{
                    //    b.ResetToLastPosition();
                    //}
                }
                foreach (ObstructionCircle oc in currentLevel.ObstructionCircles)
                {
                    if (Core.Math.CollisionDetective.CheckCollision(oc.CenterPoint, oc.Radius, b.Position, b.BoundingRadius))
                    {
                        Vector2 diff = b.Position - oc.CenterPoint;
                        diff.Normalize();
                        b.Position = oc.CenterPoint + diff * oc.Radius;
                    }
                }
                foreach (ObstructionLattice ol in currentLevel.ObstructionLattices)
                {
                    // TODO
                }
                // check block to board collision (to keep in)
                if (b.Position.Y - b.BoundingRadius < currentLevel.PlayZone.Top)
                {
                    b.Position = new Vector2(b.Position.X, currentLevel.PlayZone.Top + b.BoundingRadius);  // bump it down
                    float newVelocityY = -0.5f * b.Velocity.Y;
                    if (newVelocityY < 0) newVelocityY *= -1;
                    b.Velocity = new Vector2(b.Velocity.X, newVelocityY); // bounce it off the wall
                }
                if (b.Position.Y + b.BoundingRadius > currentLevel.PlayZone.Bottom)
                {
                    b.Position = new Vector2(b.Position.X, currentLevel.PlayZone.Bottom - b.BoundingRadius);  // bump it up
                    float newVelocityY = -0.5f * b.Velocity.Y;
                    if (newVelocityY > 0) newVelocityY *= -1;
                    b.Velocity = new Vector2(b.Velocity.X, newVelocityY); // bounce it off the wall
                }
                if (b.Position.X - b.BoundingRadius < currentLevel.PlayZone.Left)
                {
                    b.Position = new Vector2(currentLevel.PlayZone.Left + b.BoundingRadius, b.Position.Y);  // bump it right
                    float newVelocityX = -0.5f * b.Velocity.X;
                    if (newVelocityX < 0) newVelocityX *= -1;
                    b.Velocity = new Vector2(newVelocityX, b.Velocity.Y); // bounce it off the wall
                }
                if (b.Position.X + b.BoundingRadius > currentLevel.PlayZone.Right)
                {
                    b.Position = new Vector2(currentLevel.PlayZone.Right - b.BoundingRadius, b.Position.Y);  // bump it left
                    float newVelocityX = -0.5f * b.Velocity.X;
                    if (newVelocityX > 0) newVelocityX *= -1;
                    b.Velocity = new Vector2(-0.5f * b.Velocity.X, b.Velocity.Y); // bounce it off the wall
                }
                #endregion
            }
            #endregion

            #region process process power ups
            timeTillNextPowerUp -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            for (int pupIter = 0; pupIter < powerups.Count; ++pupIter)
            {
                PowerUp pu = powerups[pupIter];
                if (pu.IsActive && pu.IsTimeExpired(gameTime))
                {
                    RemovePowerUpFromPlay(pu);
                    pupIter--;
                }
            }
            #endregion

            #region process all slots
            foreach (Slot s in hexTiles.Slots)
            {
                s.Update(gameTime);
            }
            #endregion

            #region process source slots
            // choose a random source slot and emit from it
            int index = 0;
            if (sourceSlots.Count > 1) index = HBBB.Core.Math.Random.NextInt(0, sourceSlots.Count - 1);
            Slot sourceSlot = sourceSlots[index];

            // emit a power up every once in a while
            if (timeTillNextPowerUp <= 0.0f)
            {
                PowerUp pup = AddPowerUpToBoard(new Vector2(sourceSlot.Bounds.X + (float)sourceSlot.Bounds.Width / 2.0f, sourceSlot.Bounds.Y + (float)sourceSlot.Bounds.Height / 2.0f));
                if (pup != null)
                {
                    pup.Velocity = new Vector2(Core.Math.Random.NextFloat(1.0f, 2.0f), Core.Math.Random.NextFloat(1.0f, 2.0f));
                    pup.RotationSpeed = Core.Math.Random.NextFloat(0.5f, 1.5f);
                }
                timeTillNextPowerUp = currentLevel.TimeBetweenPowerups;
            }

            // create a block if there aren't enough in play or min interval seconds has ellapsed without one
            timeSinceLastBlockEmit += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if ((sourceSlot != null && freeBlocksInPlay.Count < sourceSlots.Count) || timeSinceLastBlockEmit > currentLevel.MinimumBlockEmissionTime)
            {
                timeSinceLastBlockEmit = 0.0f; // reset the counter
                Block block = AddBlockToBoard(new Vector2(sourceSlot.Bounds.X + (float)sourceSlot.Bounds.Width / 2.0f, sourceSlot.Bounds.Y + (float)sourceSlot.Bounds.Height / 2.0f));
                if (block != null)
                {
                    block.Velocity = new Vector2(Core.Math.Random.NextFloat(1.0f, 2.0f), Core.Math.Random.NextFloat(1.0f, 2.0f));
                    block.RotationSpeed = Core.Math.Random.NextFloat(0.5f, 1.5f);
                    sourceSlot.Texture.PlayCycle();
                }
            }
            #endregion

            base.Update(gameTime);
        }

        /// <summary>
        /// Hack to fix problem when lots of blocks collide with a player
        /// </summary>
        /// <param name="b"></param>
        public void KeepBlockOnBoard(Block b)
        {
            // check block to board collision (to keep in)
            if (b.Position.Y - b.BoundingRadius < currentLevel.PlayZone.Top)
            {
                b.Position = new Vector2(b.Position.X, currentLevel.PlayZone.Top + b.BoundingRadius);  // bump it down
                float newVelocityY = -0.5f * b.Velocity.Y;
                if (newVelocityY < 0) newVelocityY *= -1;
                b.Velocity = new Vector2(b.Velocity.X, newVelocityY); // bounce it off the wall
            }
            if (b.Position.Y + b.BoundingRadius > currentLevel.PlayZone.Bottom)
            {
                b.Position = new Vector2(b.Position.X, currentLevel.PlayZone.Bottom - b.BoundingRadius);  // bump it up
                float newVelocityY = -0.5f * b.Velocity.Y;
                if (newVelocityY > 0) newVelocityY *= -1;
                b.Velocity = new Vector2(b.Velocity.X, newVelocityY); // bounce it off the wall
            }
            if (b.Position.X - b.BoundingRadius < currentLevel.PlayZone.Left)
            {
                b.Position = new Vector2(currentLevel.PlayZone.Left + b.BoundingRadius, b.Position.Y);  // bump it right
                float newVelocityX = -0.5f * b.Velocity.X;
                if (newVelocityX < 0) newVelocityX *= -1;
                b.Velocity = new Vector2(newVelocityX, b.Velocity.Y); // bounce it off the wall
            }
            if (b.Position.X + b.BoundingRadius > currentLevel.PlayZone.Right)
            {
                b.Position = new Vector2(currentLevel.PlayZone.Right - b.BoundingRadius, b.Position.Y);  // bump it left
                float newVelocityX = -0.5f * b.Velocity.X;
                if (newVelocityX > 0) newVelocityX *= -1;
                b.Velocity = new Vector2(-0.5f * b.Velocity.X, b.Velocity.Y); // bounce it off the wall
            }
        }

        /// <summary>
        /// Hack check to make sure the physics didn't push blocks off the board
        /// </summary>
        public bool EnsureBlockIsOnBoard(Block b)
        {
            if (float.IsNaN(b.Position.X) || float.IsNaN(b.Position.Y))
            {
                Debug.WriteLine("Board::EnsureBlockIsOnBoard - oh super shit, Block " + b.Id + " has NaNs! Moving to 0,0");
                // uncomment this line to move it to 0,0
                b.Position = new Vector2(currentLevel.PlayZone.Left + currentLevel.PlayZone.Width / 2, currentLevel.PlayZone.Top + currentLevel.PlayZone.Height / 2);
                return false;

            }
            /*
            // last ditch effort to make sure nothing got fucked up - this happens sometimes on really full boards
            if (b.Position.Y - b.BoundingRadius < currentLevel.PlayZone.Top ||
                b.Position.Y + b.BoundingRadius > currentLevel.PlayZone.Bottom ||
                b.Position.X - b.BoundingRadius < currentLevel.PlayZone.Left ||
                b.Position.X + b.BoundingRadius > currentLevel.PlayZone.Right)
            {
                Debug.WriteLine("Board::EnsureBlockIsOnBoard - oh shit, Block " + b.Id + ":p=" + b.Position.ToString() + ",v=" + b.Velocity.ToString());
                // uncomment this line to move it to 0,0
                //b.Position = new Vector2(currentLevel.PlayZone.Left + currentLevel.PlayZone.Width / 2, currentLevel.PlayZone.Top + currentLevel.PlayZone.Height / 2);
                return false;
            }
             */
            return true;
        }

        /// <summary>
        /// Draw this game component
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (!inGameFlag) return;  // dont draw if we aren't in game

            // draw background texture
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            spriteBatch.Draw(gameBackground,
                new Vector2(this.GraphicsDevice.Viewport.Width / 2, this.GraphicsDevice.Viewport.Height / 2),
                null, Color.White, (float)gameTime.TotalGameTime.TotalMinutes,
                new Vector2(gameBackground.Width / 2, gameBackground.Height / 2),
                1.5f, SpriteEffects.None, 0.0f);

            // draw slots
            foreach (Slot slot in hexTiles.Slots)
            {
                slot.Draw(spriteBatch, spriteFont);
#if DEBUG
                if (GlobalFlags.DrawDebugBoard)
                {
                    // Draw the square bounds around the hexagon
                    slot.DebugRender(primitiveBatch);
                    // Draw slot ids
                    spriteBatch.DrawString(spriteFont, slot.Id.ToString(), new Vector2(slot.Bounds.X + 8, slot.Bounds.Y + 8), Color.Red);
                    switch (slot.Edge)
                    {
                        case Slot.EdgeType.TOP_EDGE:
                            spriteBatch.DrawString(spriteFont, "T", new Vector2(slot.Bounds.X + 20, slot.Bounds.Y + 30), Color.Red);
                            break;
                        case Slot.EdgeType.LEFT_EDGE:
                            spriteBatch.DrawString(spriteFont, "L", new Vector2(slot.Bounds.X + 20, slot.Bounds.Y + 30), Color.Red);
                            break;
                        case Slot.EdgeType.RIGHT_EDGE:
                            spriteBatch.DrawString(spriteFont, "R", new Vector2(slot.Bounds.X + 20, slot.Bounds.Y + 30), Color.Red);
                            break;
                        case Slot.EdgeType.BOTTOM_EDGE:
                            spriteBatch.DrawString(spriteFont, "B", new Vector2(slot.Bounds.X + 20, slot.Bounds.Y + 30), Color.Red);
                            break;
                        case Slot.EdgeType.TOP_LEFT_EDGE:
                            spriteBatch.DrawString(spriteFont, "TL", new Vector2(slot.Bounds.X + 20, slot.Bounds.Y + 30), Color.Red);
                            break;
                        case Slot.EdgeType.TOP_RIGHT_EDGE:
                            spriteBatch.DrawString(spriteFont, "TR", new Vector2(slot.Bounds.X + 20, slot.Bounds.Y + 30), Color.Red);
                            break;
                        case Slot.EdgeType.BOTTOM_LEFT_EDGE:
                            spriteBatch.DrawString(spriteFont, "BL", new Vector2(slot.Bounds.X + 20, slot.Bounds.Y + 30), Color.Red);
                            break;
                        case Slot.EdgeType.BOTTOM_RIGHT_EDGE:
                            spriteBatch.DrawString(spriteFont, "BR", new Vector2(slot.Bounds.X + 20, slot.Bounds.Y + 30), Color.Red);
                            break;
                    }
                }
#endif
            }

            // draw obstructions
            foreach (Obstruction obstr in obstructions) obstr.Draw(spriteBatch);

#if DEBUG
            if (GlobalFlags.DrawDebugJunk)
            {
                // draw debug text
                spriteBatch.DrawString(spriteFont, DebugText, new Vector2(200, 5), Color.Red);
            }
#endif

#if DEBUG
            if (GlobalFlags.DrawDebugBoard)
            {
                // draw obstrcution bounding boxes
                if (obstructions != null)
                {
                    foreach (Obstruction obstr in obstructions)
                    {
                        obstr.DebugRender(primitiveBatch);
                    }
                }

                // draw the play zone bounds
                if (currentLevel != null && currentLevel.PlayZone != null)
                {
                    primitiveBatch.Begin(PrimitiveType.LineList);
                    primitiveBatch.AddVertex(new Vector2(currentLevel.PlayZone.Left, currentLevel.PlayZone.Top), Color.Red);
                    primitiveBatch.AddVertex(new Vector2(currentLevel.PlayZone.Left, currentLevel.PlayZone.Bottom), Color.Red);

                    primitiveBatch.AddVertex(new Vector2(currentLevel.PlayZone.Left, currentLevel.PlayZone.Bottom), Color.Red);
                    primitiveBatch.AddVertex(new Vector2(currentLevel.PlayZone.Right, currentLevel.PlayZone.Bottom), Color.Red);

                    primitiveBatch.AddVertex(new Vector2(currentLevel.PlayZone.Right, currentLevel.PlayZone.Bottom), Color.Red);
                    primitiveBatch.AddVertex(new Vector2(currentLevel.PlayZone.Right, currentLevel.PlayZone.Top), Color.Red);

                    primitiveBatch.AddVertex(new Vector2(currentLevel.PlayZone.Right, currentLevel.PlayZone.Top), Color.Red);
                    primitiveBatch.AddVertex(new Vector2(currentLevel.PlayZone.Left, currentLevel.PlayZone.Top), Color.Red);
                    primitiveBatch.End();
                }
            }
#endif

            // draw blocks (broken up so special drawing can be done)
            foreach (Block block in blocksInSlots)
            {
                block.Draw(spriteBatch, spriteFont);
#if DEBUG
                if (GlobalFlags.DrawDebugJunk)
                {
                    //block.DebugRender(primitiveBatch);
                    spriteBatch.DrawString(spriteFont, block.Id.ToString(), new Vector2(block.Position.X-10, block.Position.Y), Color.Black);
                }
#endif
            }
            foreach (Block block in allBlocksInPlay)
            {
                block.Draw(spriteBatch, spriteFont);
#if DEBUG
                if (GlobalFlags.DrawDebugJunk)
                {
                    //block.DebugRender(primitiveBatch);
                    spriteBatch.DrawString(spriteFont, block.Id.ToString(), new Vector2(block.Position.X-10, block.Position.Y), Color.Black);
                }
#endif
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
