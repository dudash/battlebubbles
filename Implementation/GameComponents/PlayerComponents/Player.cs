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
using HBBB.Core.Graphics;
using HBBB.Core.MassSpring.Verlet;
using HBBB.GameComponents.Globals;
using HBBB.GameComponents.PowerUps;
using HBBB.Core;

namespace HBBB.GameComponents.PlayerComponents
{
    // The player class represents a player in the game.  There are always 4 player objects
    // in the game.  If there are less then 4 human players, the remaining player object are AI.
    //
    // GONZ: This class needs to inherit from DrawableGameComponent and be added to the 
    // Game.Components list.
    class Player : EntityContainer
    {
#if XBOX360 || XBOX
        public const int AVOID_INVERSION_HACKER = 1;
#else
        public const int AVOID_INVERSION_HACKER = 1;
#endif
        public enum PlayerForm { SOLID, IN_TRANSITION, BUBBLE };
        public enum PlayerMovementType { NORMAL_MOVEMENT, SLOW_MOVEMENT, FAST_MOVEMENT };
        public enum Difficulty { EASY, NORMAL, HARD };
        public delegate bool FormChangingEventHandler(Player player, Player.PlayerForm newForm);
        public event FormChangingEventHandler FormChanging;
        public delegate void FormChangedEventHandler(Player player);
        public event FormChangedEventHandler FormChanged;

        public const float DEFAULT_TIME_REQUIRED_TO_TRANSITION_TO_SOLID = 0.75f;
        public const float DEFAULT_TIME_REQUIRED_TO_TRANSITION_TO_BUBBLE = 0.0f;

        Vector2 spawnLocation;
        int bubbleSegments;
        int bubbleForce;
        int bubbleInnerForce;
        float outerWidth;
        float innerWidth;

        // GONZ: This pointer is assigned when the player is added to the game session.  I have
        // started moving player code into the Player class as part of a general effort to
        // untangle the class relationships, and this means that Player's methods now need a
        // way to find other game objects.  I am choosing GameSession to be the global environment
        // because it's available even when the menus are running, and because the "Game" class
        // should be the generic one that's reusable across very different game stages.
        public GameSession GameSession;

        public bool IsBoostingSpeed = false;
        public bool IsTryingToDefend = false;
        public bool IsAbleToDefend = false;
        public float GrabModifier = 0.0f;
        private Difficulty difficulty = Difficulty.NORMAL;
        public Difficulty DifficultyLevel { get { return difficulty; } }
        public void SetDifficulty(Difficulty value)
        {
            difficulty = value;
            if (value == Difficulty.EASY) GrabModifier = 25.0f;
            else if (value == Difficulty.HARD) GrabModifier = -10.0f;
            else if (value == Difficulty.NORMAL) GrabModifier = 0.0f;
        }
        public void NextDifficultyLevel()
        {
            if (difficulty == Difficulty.EASY) SetDifficulty(Difficulty.NORMAL);
            else if (difficulty == Difficulty.NORMAL) SetDifficulty(Difficulty.HARD);
            //else if (difficulty == Difficulty.HARD) SetDifficulty(Difficulty.EASY);
        }
        public void PreviousDifficultyLevel()
        {
            //if (difficulty == Difficulty.EASY) SetDifficulty(Difficulty.HARD);
            if (difficulty == Difficulty.NORMAL) SetDifficulty(Difficulty.EASY);
            else if (difficulty == Difficulty.HARD) SetDifficulty(Difficulty.NORMAL);
        }

        /// <summary>
        /// an avatar for the player
        /// </summary>
        Texture2D playerPic;
        public Texture2D PlayerPic
        {
            get { return playerPic; }
            set { playerPic = value; }
        }
        /// <summary>
        /// Statistic for this player
        /// </summary>
        PlayerStats statistics;
        public PlayerStats Statistics
        {
            get { return statistics; }
            set { statistics = value; }
        }
        /// <summary>
        /// The current form
        /// </summary>
        PlayerForm currentForm;
        public PlayerForm Form { get { return currentForm; } }
        /// <summary>
        /// the previous form
        /// </summary>
        PlayerForm lastForm;
        public PlayerForm LastForm { get { return lastForm; } }
        /// <summary>
        /// type that specifies how the player movement physics are performed
        /// </summary>
        PlayerMovementType movementType;
        public PlayerMovementType MovementType
        {
            get { return movementType; }
            set { movementType = value; }
        }
        /// <summary>
        /// Anything the player is carrying
        /// </summary>
        IBubblePayload payload;
        public IBubblePayload Payload { get { return payload; } }
        /// <summary>
        /// player acceleration
        /// </summary>
        Vector2 acceleration;
        public Vector2 Acceleration
        {
            get { return acceleration; }
        }
        /// <summary>
        /// A multiplier to use when player is boosting
        /// </summary>
        float boostMultiplier;
        public float BoostMultiplier
        {
            get { return boostMultiplier; }
            set { boostMultiplier = value; }
        }
        /// <summary>
        /// The time (in seconds) that this player has been attemping to transition 
        /// into a different form
        /// </summary>
        float timeInTransition;
        public float TimeInTransition
        {
            get { return timeInTransition; }
            set { timeInTransition = value; }
        }
        /// <summary>
        /// The time required to transition to solid
        /// </summary>
        float timeRequiredToTransitionToSolid = DEFAULT_TIME_REQUIRED_TO_TRANSITION_TO_SOLID;
        public float TimeRequiredToTransitionToSolid
        {
            get { return timeRequiredToTransitionToSolid; }
            set { timeRequiredToTransitionToSolid = value; }
        }
        /// <summary>
        /// The time required to transition to bubble
        /// </summary>
        float timeRequiredToTransitionToBubble = DEFAULT_TIME_REQUIRED_TO_TRANSITION_TO_BUBBLE;
        public float TimeRequiredToTransitionToBubble
        {
            get { return timeRequiredToTransitionToBubble; }
            set { timeRequiredToTransitionToBubble = value; }
        }
        /// <summary>
        /// time based on current form
        /// </summary>
        public float TimeRequiredToTransition
        {
            get
            {
                if (currentForm == PlayerForm.IN_TRANSITION && lastForm == PlayerForm.BUBBLE) return timeRequiredToTransitionToSolid;
                if (currentForm == PlayerForm.BUBBLE) return timeRequiredToTransitionToSolid;
                else return timeRequiredToTransitionToBubble;
            }
        }
        /// <summary>
        /// Flag to indicate this is a CPU player
        /// </summary>
        bool isCpu = false;
        public bool IsCpu
        {
            get { return isCpu; }
            set { isCpu = value; }
        }
        /// <summary>
        /// The main color for this player
        /// </summary>
        Color primaryColor;
        public Color PrimaryColor
        {
            get { return primaryColor; }
            set { primaryColor = value; }
        }
        /// <summary>
        /// The secondary color for this player
        /// </summary>
        Color secondaryColor;
        public Color SecondaryColor
        {
            get { return secondaryColor; }
            set { secondaryColor = value; }
        }
        /// <summary>
        /// This player's id
        /// </summary>
        PlayerIndex playerIndex;
        public PlayerIndex PlayerIndex
        {
            get { return playerIndex; }
        }
        /// <summary>
        /// The player visualization
        /// </summary>
        Bubble bubble;
        public Bubble Bubble
        {
            get { return bubble; }
        }
        /// <summary>
        /// How to draw the inner circle
        /// </summary>
        Bubble.BubbleDrawType innerCircleDrawType = Bubble.BubbleDrawType.TRIANGLES;
        public Bubble.BubbleDrawType InnerCircleDrawType { get { return innerCircleDrawType; } }
        /// <summary>
        /// How to draw the outer circle
        /// </summary>
        Bubble.BubbleDrawType outerCircleDrawType = Bubble.BubbleDrawType.TRIANGLES;
        public Bubble.BubbleDrawType OuterCircleDrawType { get { return outerCircleDrawType; } }
        /// <summary>
        /// The list of powerups currently affecting this player
        /// </summary>
        List<PowerUp> activePowerUps;
        public List<PowerUp> ActivePowerUps { get { return activePowerUps; } }
        /// <summary>
        /// to handler player input
        /// </summary>
        IPlayerInputHandler inputHandler;
        public IPlayerInputHandler InputHandler
        {
            get { return inputHandler; }
            set { inputHandler = value; }
        }

        /// <summary>
        /// Construct a new player
        /// </summary>
        /// <param name="id"></param>
        public Player(PlayerIndex playerIndex)
        {
            this.playerIndex = playerIndex;
            switch (playerIndex)
            {
                case PlayerIndex.One:
                    primaryColor = Color.DarkBlue;
                    secondaryColor = Color.Cyan;
                    break;
                case PlayerIndex.Two:
                    primaryColor = Color.Yellow;
                    secondaryColor = Color.Orange;
                    break;
                case PlayerIndex.Three:
                    primaryColor = Color.DarkRed;
                    secondaryColor = Color.Red;
                    break;
                case PlayerIndex.Four:
                    primaryColor = Color.Green;
                    secondaryColor = Color.Pink;
                    break;
            }
            Initialize();
        }

        /// <summary>
        /// Initialize the player properties back to defaults
        /// </summary>
        public void Initialize()
        {
            statistics = new PlayerStats();
            acceleration = new Vector2();
            boostMultiplier = 3.5f;
            timeInTransition = 0;
            currentForm = PlayerForm.BUBBLE;
            lastForm = PlayerForm.BUBBLE;
            movementType = PlayerMovementType.NORMAL_MOVEMENT;
            payload = null;
            activePowerUps = new List<PowerUp>();
        }

        /// <summary>
        /// Setup the bubble properties
        /// </summary>
        /// <param name="bubbleSegments"></param>
        /// <param name="bubbleForce"></param>
        /// <param name="bubbleInnerForce"></param>
        /// <param name="outerWidth"></param>
        /// <param name="innerWidth"></param>
        public void InitPlayerBubbleProperties(int bubbleSegments, int bubbleForce, int bubbleInnerForce, float outerWidth, float innerWidth)
        {
            this.bubbleSegments = bubbleSegments;
            this.bubbleForce = bubbleForce;
            this.bubbleInnerForce = bubbleInnerForce;
            this.outerWidth = outerWidth;
            this.innerWidth = innerWidth;
        }

        /// <summary>
        /// Call to create the player bubble
        /// </summary>
        /// <param name="location">creation location</param>
        public Bubble CreatePlayerBubble(Vector2 location)
        {
            spawnLocation = location;

            if (bubble != null)
                RemoveChildEntity(bubble);

            bubble = new Bubble(bubbleSegments, spawnLocation.X, spawnLocation.Y, innerWidth, outerWidth, bubbleForce, bubbleInnerForce);
            bubble.TextureOuterCircle(playerPic, outerWidth);
            bubble.TextureInnerCircle(playerPic, innerWidth);

            AddChildEntity(bubble);
            return bubble;
        }

        /// <summary>
        /// Change the player form
        /// </summary>
        public void ChangeForm(PlayerForm newForm)
        {
            if (currentForm == newForm) return;
            if (FormChanging != null)
            {
                bool okToContinue = FormChanging(this, newForm);
                if (!okToContinue) return;
            }

            switch (newForm)
            {
                case PlayerForm.SOLID:
                    GameAudio.PlayCue("transition_to_solid");
                    this.currentForm = Player.PlayerForm.SOLID;
                    this.timeInTransition = 0.0f;
                    bubble.CenterHalfLifeSecs = Bubble.CHALFLIFE_DEFAULT_SOLID;
                    bubble.SpokeHalfLifeSecs = Bubble.SHALFLIFE_DEFAULT_SOLID;
                    break;
                case PlayerForm.IN_TRANSITION:
                    this.currentForm = PlayerForm.IN_TRANSITION;
                    break;
                case PlayerForm.BUBBLE:
                    if (currentForm != PlayerForm.IN_TRANSITION) GameAudio.PlayCue("transition_to_bubble");
                    this.currentForm = Player.PlayerForm.BUBBLE; // go back to bubble
                    this.timeInTransition = 0.0f;
                    bubble.CenterHalfLifeSecs = Bubble.CHALFLIFE_DEFAULT_BUB;
                    bubble.SpokeHalfLifeSecs = Bubble.SHALFLIFE_DEFAULT_BUB;
                    break;
            }
            if (FormChanged != null) FormChanged(this);
        }

        /// <summary>
        /// Add a payload to this player
        /// </summary>
        /// <param name="payload"></param>
        public void AddPayload(IBubblePayload payload)
        {
            if (this.payload != null) Debug.WriteLine("Player::AddPayload - payload being stomped!");
            this.payload = payload;
            this.payload.OwningPlayer = this;
        }

        /// <summary>
        /// Drop the payload this player is carrying but retain ownership
        /// </summary>
        public void DropPayload()
        {
            if (payload == null)
            {
                Debug.WriteLine("Player::DropPayload - no payload to drop!");
                return;
            }
            payload = null;
        }

        /// <summary>
        /// Drop the payload this player is carrying and make it free to acquire
        /// </summary>
        public void FreePayload()
        {
            if (payload == null)
            {
                Debug.WriteLine("Player::FreePayload - no payload to free!");
                return;
            }
            payload.OwningPlayer = null;
            payload = null;
        }

        /// <summary>
        /// Drop the payload, and make it free, transition into a bubble
        /// </summary>
        public void PayloadStolen()
        {
            FreePayload();
            ChangeForm(PlayerForm.BUBBLE);
        }

        /// <summary>
        /// Return the position of the player
        /// </summary>
        /// <returns></returns>
        public Vector2 GetPosition()
        {
            return bubble.CenterPoint.Position;
        }

        /// <summary>
        /// Set the player velocity
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetAcceleration(float x, float y)
        {
            acceleration.X = x;
            acceleration.Y = y;
            IsBoostingSpeed = false;
        }

        /// <summary>
        /// Set the player velocity
        /// </summary>
        /// <param name="value"></param>
        public void SetAcceleration(Vector2 value)
        {
            acceleration = value;
            IsBoostingSpeed = false;
        }

        /// <summary>
        /// Apply a velocity boost to the player
        /// </summary>
        /// <param name="boost"></param>
        public void BoostAcceleration()
        {
            acceleration *= boostMultiplier;
            IsBoostingSpeed = true;
        }

        /// <summary>
        /// Determine a velocity for this bubble
        /// </summary>
        public void GetVelocity()
        {
            // TODO calc velocity from bubble point velocities
        }

        /// <summary>
        /// Add a power up to affect this player
        /// </summary>
        /// <param name="value"></param>
        public void AddActivePowerUp(PowerUp value)
        {
            activePowerUps.Add(value);
        }

        /// <summary>
        /// Remove a power up affecting this player
        /// </summary>
        /// <param name="value"></param>
        public void RemoveActivePowerUp(PowerUp value)
        {
            activePowerUps.Remove(value);
        }

#if DEBUG
        /// <summary>
        /// cycle through view types
        /// </summary>
        public void GotoNextOuterCircleDrawType()
        {
            switch (outerCircleDrawType)
            {
                case Bubble.BubbleDrawType.POINTS:
                    outerCircleDrawType = Bubble.BubbleDrawType.LINES;
                    break;
                case Bubble.BubbleDrawType.LINES:
                    outerCircleDrawType = Bubble.BubbleDrawType.TRIANGLES;
                    break;
                case Bubble.BubbleDrawType.TRIANGLES:
                    outerCircleDrawType = Bubble.BubbleDrawType.TEXTURED;
                    break;
                case Bubble.BubbleDrawType.TEXTURED:
                    outerCircleDrawType = Bubble.BubbleDrawType.POINTS;
                    break;
            }
        }

        /// <summary>
        /// cycle through view types
        /// </summary>
        public void GotoNextInnerCircleDrawType()
        {
            switch (innerCircleDrawType)
            {
                case Bubble.BubbleDrawType.POINTS:
                    innerCircleDrawType = Bubble.BubbleDrawType.LINES;
                    break;
                case Bubble.BubbleDrawType.LINES:
                    innerCircleDrawType = Bubble.BubbleDrawType.TRIANGLES;
                    break;
                case Bubble.BubbleDrawType.TRIANGLES:
                    innerCircleDrawType = Bubble.BubbleDrawType.TEXTURED;
                    break;
                case Bubble.BubbleDrawType.TEXTURED:
                    innerCircleDrawType = Bubble.BubbleDrawType.POINTS;
                    break;
            }
        }

#endif

        // @@ Gonz: This should be handled via the GameComponent IUpdateable interface
        // in order to accomplish that, the player argument will need to be removed and the
        // input handler should receive the player reference via a different method call (Reset / Init)
        // also, then IPlayerInputHandler might be unnecessary.
        public void Update(GameTime gameTime)
        {
            if (InputHandler != null) InputHandler.Update(gameTime, this);

            // Subscribe to the level's ambient forces
            Debug.Assert(GameSession != null);
            Bubble.AddStaticForce(GameSession.Board.CurrentLevel.AmbientForces);

            // if you're defending, you just float.  you don't add any forces
            if (IsAbleToDefend && IsTryingToDefend) return;

            // process player controller input
            if (movementType == Player.PlayerMovementType.NORMAL_MOVEMENT)  // normal movement
            {
                Bubble.AddStaticForce(acceleration);
            }
            else if (movementType == Player.PlayerMovementType.SLOW_MOVEMENT)
            {
                Bubble.AddStaticForce(acceleration * 0.2f);
            }
            else if (movementType == Player.PlayerMovementType.FAST_MOVEMENT)
            {
                Bubble.AddStaticForce(acceleration * 5);
            }
#if DEBUG
            if (float.IsNaN(Bubble.CenterPoint.Position.X) || float.IsNaN(Bubble.CenterPoint.Position.Y))
            {
                Debug.WriteLine("Player::Update - bubble center at NaN!");
            }
#endif
        }
    }
}
