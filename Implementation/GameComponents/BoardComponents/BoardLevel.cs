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
// File Created: 07 November 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HBBB.GameComponents.PowerUps;

namespace HBBB.GameComponents.BoardComponents
{
    /// <summary>
    /// A board level is the part of the board that can be changed to make
    /// the gameplay different.  It contains the definition of the obsticles,
    /// player spawn points, power up types, etc...
    /// </summary>
    [Serializable]
    public class BoardLevel
    {
        /// <summary>
        /// This nested class of BoardLevel provides ways the a player can win
        /// </summary>
        [Serializable]
        public class WinRules
        {
            public bool LockTheMostBlocks = true;
            public bool TouchTheSource = false;
            public bool TouchPlayZoneTop = false;
            public bool TouchPlayZoneLeft = false;
            public bool TouchPlayZoneRight = false;
            public bool TouchPlayZoneBottom = false;
        }

        /// <summary>
        /// The maximum number of seconds this game will last
        /// </summary>
        int timeInSeconds = 300;
        public int TimeInSeconds
        {
            get { return timeInSeconds; }
            set { timeInSeconds = value; }
        }

        /// <summary>
        /// The time in seconds between powerups
        /// </summary>
        float timeBetweenPowerups = 30;
        public float TimeBetweenPowerups
        {
            get { return timeBetweenPowerups; }
            set { timeBetweenPowerups = value; }
        }

        float minimumBlockEmissionTime = 8;
        public float MinimumBlockEmissionTime
        {
            get { return minimumBlockEmissionTime; }
            set { minimumBlockEmissionTime = value; }
        }

        /// <summary>
        /// The name of the music for the level
        /// </summary>
        string musicCueName = "";
        public string MusicCueName
        {
            get { return musicCueName; }
            set { musicCueName = value; }
        }

        #region Player1 Stuff
        /// <summary>
        /// How this player can win this level
        /// </summary>
        WinRules player1WinRules;
        public WinRules Player1WinRules
        {
            get { return player1WinRules; }
            set { player1WinRules = value; }
        }
        /// <summary>
        /// Location where the player will spawn
        /// </summary>
        Vector2 player1SpawnPoint;
        public Vector2 Player1SpawnPoint
        {
            get { return player1SpawnPoint; }
            set { player1SpawnPoint = value; }
        }
        /// <summary>
        /// Number of segment in circle
        /// </summary>
        int player1BubbleSegments;
        public int Player1BubbleSegments
        {
            get { return player1BubbleSegments; }
            set { player1BubbleSegments = value; }
        }
        /// <summary>
        /// Force connecting bubble skin masses
        /// </summary>
        int player1BubbleForce;
        public int Player1BubbleForce
        {
            get { return player1BubbleForce; }
            set { player1BubbleForce = value; }
        }
        /// <summary>
        /// Force between center point and inner skin
        /// </summary>
        int player1BubbleInnerForce;
        public int Player1BubbleInnerForce
        {
            get { return player1BubbleInnerForce; }
            set { player1BubbleInnerForce = value; }
        }
        /// <summary>
        /// Bubble outer skin width
        /// </summary>
        float player1OuterWidth;
        public float Player1OuterWidth
        {
            get { return player1OuterWidth; }
            set { player1OuterWidth = value; }
        }
        /// <summary>
        /// Bubble inner circle skin width
        /// </summary>
        float player1InnerWidth;
        public float Player1InnerWidth
        {
            get { return player1InnerWidth; }
            set { player1InnerWidth = value; }
        }
        #endregion

        #region Player2 Stuff
        /// <summary>
        /// How this player can win this level
        /// </summary>
        WinRules player2WinRules;
        public WinRules Player2WinRules
        {
            get { return player2WinRules; }
            set { player2WinRules = value; }
        }

        /// <summary>
        /// Location where the player will spawn
        /// </summary>
        Vector2 player2SpawnPoint;
        public Vector2 Player2SpawnPoint
        {
            get { return player2SpawnPoint; }
            set { player2SpawnPoint = value; }
        }
        /// <summary>
        /// Number of segment in circle
        /// </summary>
        int player2BubbleSegments;
        public int Player2BubbleSegments
        {
            get { return player2BubbleSegments; }
            set { player2BubbleSegments = value; }
        }
        /// <summary>
        /// Force connecting bubble skin masses
        /// </summary>
        int player2BubbleForce;
        public int Player2BubbleForce
        {
            get { return player2BubbleForce; }
            set { player2BubbleForce = value; }
        }
        /// <summary>
        /// Force between center point and inner skin
        /// </summary>
        int player2BubbleInnerForce;
        public int Player2BubbleInnerForce
        {
            get { return player2BubbleInnerForce; }
            set { player2BubbleInnerForce = value; }
        }
        /// <summary>
        /// Bubble outer skin width
        /// </summary>
        float player2OuterWidth;
        public float Player2OuterWidth
        {
            get { return player2OuterWidth; }
            set { player2OuterWidth = value; }
        }
        /// <summary>
        /// Bubble inner circle skin width
        /// </summary>
        float player2InnerWidth;
        public float Player2InnerWidth
        {
            get { return player2InnerWidth; }
            set { player2InnerWidth = value; }
        }
        #endregion

        #region Player3 Stuff
        /// <summary>
        /// How this player can win this level
        /// </summary>
        WinRules player3WinRules;
        public WinRules Player3WinRules
        {
            get { return player3WinRules; }
            set { player3WinRules = value; }
        }

        /// <summary>
        /// Location where the player will spawn
        /// </summary>
        Vector2 player3SpawnPoint;
        public Vector2 Player3SpawnPoint
        {
            get { return player3SpawnPoint; }
            set { player3SpawnPoint = value; }
        }
        /// <summary>
        /// Number of segment in circle
        /// </summary>
        int player3BubbleSegments;
        public int Player3BubbleSegments
        {
            get { return player3BubbleSegments; }
            set { player3BubbleSegments = value; }
        }
        /// <summary>
        /// Force connecting bubble skin masses
        /// </summary>
        int player3BubbleForce;
        public int Player3BubbleForce
        {
            get { return player3BubbleForce; }
            set { player3BubbleForce = value; }
        }
        /// <summary>
        /// Force between center point and inner skin
        /// </summary>
        int player3BubbleInnerForce;
        public int Player3BubbleInnerForce
        {
            get { return player3BubbleInnerForce; }
            set { player3BubbleInnerForce = value; }
        }
        /// <summary>
        /// Bubble outer skin width
        /// </summary>
        float player3OuterWidth;
        public float Player3OuterWidth
        {
            get { return player3OuterWidth; }
            set { player3OuterWidth = value; }
        }
        /// <summary>
        /// Bubble inner circle skin width
        /// </summary>
        float player3InnerWidth;
        public float Player3InnerWidth
        {
            get { return player3InnerWidth; }
            set { player3InnerWidth = value; }
        }
        #endregion

        #region Player4 Stuff
        /// <summary>
        /// How this player can win this level
        /// </summary>
        WinRules player4WinRules;
        public WinRules Player4WinRules
        {
            get { return player4WinRules; }
            set { player4WinRules = value; }
        }

        /// <summary>
        /// Location where the player will spawn
        /// </summary>
        Vector2 player4SpawnPoint;
        public Vector2 Player4SpawnPoint
        {
            get { return player4SpawnPoint; }
            set { player4SpawnPoint = value; }
        }
        /// <summary>
        /// Number of segment in circle
        /// </summary>
        int player4BubbleSegments;
        public int Player4BubbleSegments
        {
            get { return player4BubbleSegments; }
            set { player4BubbleSegments = value; }
        }
        /// <summary>
        /// Force connecting bubble skin masses
        /// </summary>
        int player4BubbleForce;
        public int Player4BubbleForce
        {
            get { return player4BubbleForce; }
            set { player4BubbleForce = value; }
        }
        /// <summary>
        /// Force between center point and inner skin
        /// </summary>
        int player4BubbleInnerForce;
        public int Player4BubbleInnerForce
        {
            get { return player4BubbleInnerForce; }
            set { player4BubbleInnerForce = value; }
        }
        /// <summary>
        /// Bubble outer skin width
        /// </summary>
        float player4OuterWidth;
        public float Player4OuterWidth
        {
            get { return player4OuterWidth; }
            set { player4OuterWidth = value; }
        }
        /// <summary>
        /// Bubble inner circle skin width
        /// </summary>
        float player4InnerWidth;
        public float Player4InnerWidth
        {
            get { return player4InnerWidth; }
            set { player4InnerWidth = value; }
        }
        #endregion

        /// <summary>
        /// Dimensions of the playable area on the board
        /// </summary>
        Rectangle playZone;
        public Rectangle PlayZone
        {
            get { return playZone; }
            set { playZone = value; }
        }
        /// <summary>
        /// The orientation type of the slots
        /// </summary>
        SlotOrientation.Type slotOrientation;
        public SlotOrientation.Type SlotOrientation
        {
            get { return slotOrientation; }
            set { slotOrientation = value; }
        }
        /// <summary>
        /// The ideal horizontal and vertical slot counts
        /// </summary>
        Vector2 slotsCount = new Vector2(17,9);
        public Vector2 SlotsCount
        {
            get { return slotsCount; }
            set { slotsCount = value; }
        }
        /// <summary>
        /// The texture displayed for slots
        /// </summary>
        string slotTextureName = @"W_A_D\Textures\slot_vertical";
        public string SlotTextureName
        {
            get { return slotTextureName; }
            set { slotTextureName = value; }
        }
        /// <summary>
        /// The background texture displayed for this level
        /// </summary>
        string backgroundTextureName = @"W_A_D\Textures\game_background";
        public string BackgroundTextureName
        {
            get { return backgroundTextureName; }
            set { backgroundTextureName = value; }
        }
        /// <summary>
        /// The gravity and wind forces that exist on this level
        /// </summary>
        Vector2 ambientForces = new Vector2();
        public Vector2 AmbientForces
        {
            get { return ambientForces; }
            set { ambientForces = value; }
        }
        /// <summary>
        /// Obstructions with rectangular bounds
        /// </summary>
        List<ObstructionRectangle> obstructionRectangles;
        public List<ObstructionRectangle> ObstructionRectangles
        {
            get { return obstructionRectangles; }
            set { obstructionRectangles = value; }
        }
        /// <summary>
        /// Obstructions with circular bounds
        /// </summary>
        List<ObstructionCircle> obstructionCircles;
        public List<ObstructionCircle> ObstructionCircles
        {
            get { return obstructionCircles; }
            set { obstructionCircles = value; }
        }
        /// <summary>
        /// Obstructions lattices (line boundaries on slot edges)
        /// </summary>
        List<ObstructionLattice> obstructionLattices;
        public List<ObstructionLattice> ObstructionLattices
        {
            get { return obstructionLattices; }
            set { obstructionLattices = value; }
        }
        /// <summary>
        /// Slot Ids for the source
        /// </summary>
        List<int> sourceSlotIds;
        public List<int> SourceSlotIds
        {
            get { return sourceSlotIds; }
            set { sourceSlotIds = value; }
        }
        List<Point> bonusPointsSlotIds;
        public List<Point> BonusPointsSlotIds
        {
            get { return bonusPointsSlotIds; }
            set { bonusPointsSlotIds = value; }
        }
        /// <summary>
        /// The texture for the source
        /// </summary>
        string sourceSlotTextureName;
        public string SourceSlotTextureName
        {
            get { return sourceSlotTextureName; }
            set { sourceSlotTextureName = value; }
        }
        /// <summary>
        /// A color modifier for all slots.  R, G, B, A
        /// </summary>
        private Vector4 slotsColor;
        public Vector4 SlotsColor
        {
            get { return slotsColor; }
            set { slotsColor = value; }
        }
        /// <summary>
        /// Slot Ids for the dead slots
        /// </summary>
        List<int> deadSlotIds;
        public List<int> DeadSlotIds
        {
            get { return deadSlotIds; }
            set { deadSlotIds = value; }
        }
        /// <summary>
        /// Slot Ids for the slots that pop player bubbles
        /// </summary>
        List<int> bubblePoppingSlots;
        public List<int> BubblePoppingSlotIds
        {
            get { return bubblePoppingSlots; }
            set { bubblePoppingSlots = value; }
        }
        /// <summary>
        /// Slot Ids for the slots that solid players collide with
        /// </summary>
        List<int> playerSolidCollisionSlotIds;
        public List<int> PlayerSolidCollisionSlotIds
        {
            get { return playerSolidCollisionSlotIds; }
            set { playerSolidCollisionSlotIds = value; }
        }
        /// <summary>
        /// Slot Ids for the slots that bubble players collide with
        /// </summary>
        List<int> playerBubbleCollisionSlotIds;
        public List<int> PlayerBubbleCollisionSlotIds
        {
            get { return playerBubbleCollisionSlotIds; }
            set { playerBubbleCollisionSlotIds = value; }
        }
        /// <summary>
        /// List of int slot ids paired with owning player indexes
        /// </summary>
        List<IntPlayerIndexPair> ownedSlotIds;
        public List<IntPlayerIndexPair> OwnedSlotIds
        {
            get { return ownedSlotIds; }
            set { ownedSlotIds = value; }
        }
        /// <summary>
        /// The texture for the dead slots
        /// </summary>
        string deadSlotTextureName;
        public string DeadSlotTextureName
        {
            get { return deadSlotTextureName; }
            set { deadSlotTextureName = value; }
        }
        /// <summary>
        /// This level's available powerups
        /// </summary>
        List<string> powerUpIds;
        public List<string> PowerUpIds
        {
            get { return powerUpIds; }
            set { powerUpIds = value; }
        }
    }
}
