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
// File Created: 06 July 2007, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using HBBB.Core;
using HBBB.Core.Input;
using HBBB.Core.Graphics;
using HBBB.Core.MassSpring.Verlet;
using HBBB.Core.Menus;
using HBBB.Core.Math;
using HBBB.GameComponents;
using HBBB.GameComponents.Globals;
using HBBB.GameComponents.PlayerComponents;
using HBBB.GameComponents.Menus;
using HBBB.GameComponents.BoardComponents;
using HBBB.GameComponents.HUD;
using HBBB.GameComponents.PowerUps;

namespace HBBB
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    class HBBBGame : Microsoft.Xna.Framework.Game
    {
        private const float BUBBLE_TO_BLOCK_REPLUSION_VELOCITY_COEFF = -0.5f;  // higher = replulse faster on contact, negative values make block attracted to bubble
        private const float SOLID_TO_BLOCK_REPLUSION_VELOCITY_COEFF = 1.5f;   // higher = replulse faster on contact
        private const float SOLID_TRANSPARENCY = 0.7f;
        private const float BUBBLE_TRANSPARENCY = 0.5f;

        /// <summary>
        /// graphics device
        /// </summary>
        private GraphicsDeviceManager graphics;
        /// <summary>
        /// The menu system draws and manages itself
        /// </summary>
        private BaseMenuSystem menuSystem;
        /// <summary>
        /// A list of HUD components
        /// </summary>
        private List<IHUDComponent> hudList;
        /// <summary>
        /// Support turning HUD on and off (in game)
        /// </summary>
        bool showHUDFlag = true;
        /// <summary>
        /// to draw sprites with
        /// </summary>
        SpriteBatch spriteBatch;
        /// <summary>
        /// A simple point texture for drawing the bubble points
        /// </summary>
        Texture2D pointTexture;
        /// <summary>
        /// for text sprites
        /// </summary>
        SpriteFont spriteFont;
        /// <summary>
        /// For drawing primitives
        /// </summary>
        PrimitiveBatch primitiveBatch;
        /// <summary>
        /// A logo splash screen
        /// </summary>
        SplashScreen splash;
        /// <summary>
        /// 2nd splash screen
        /// </summary>
        //SplashScreen splash2;
        /// <summary>
        /// pads
        /// </summary>
        GamePadsWrapper gamepads;

        public EntityManager EntityManager;

        /// <summary>
        /// The session associated with this game
        /// </summary>
        private GameSession session;
        public GameSession Session
        {
            get { return session; }
        }

        /// <summary>
        /// Construct the game
        /// </summary>
        public HBBBGame()
        {
            Guide.SimulateTrialMode = true; // for debugging the buy_now options and disabling other features

            EntityManager = new EntityManager(Components);
            graphics = new GraphicsDeviceManager(this);
            Components.Add(new GamerServicesComponent(this));

#if XBOX || XBOX360
            // TODO detect resolution and set appropiately
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
#endif
            // Comment out the line below to run in a window
            graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(graphics_PreparingDeviceSettings);
            GameAudio.Initialize(this.Content);
            GameAudio.PlayMusic("Gorillas");
#if !DEBUG
            splash = new SplashScreen(this, @"W_A_D\Textures\NoHandsLogoOnBlack");
            splash.SetText("www.nohandsgames.com");
            splash.ShowTime(5);  // show no hands games for 5 seconds
            this.Components.Add(splash);
            //splash2 = new SplashScreen(this, @"W_A_D\Textures\xna");
            //splash2.setTextureOffset(50, 120);
            //splash2.Show(3,2);  // atfter 3 seconds, show the dream build play on top for 2 seconds
            //this.Components.Add(splash2);
#endif
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                EntityManager.Dispose();
                EntityManager = null;
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Setup the graphics device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
#if XBOX || XBOX360
            e.GraphicsDeviceInformation.PresentationParameters.EnableAutoDepthStencil = true;
#else
/*
            // TODO this is my laptop resolution, make this code more generic
            foreach (DisplayMode mode in GraphicsAdapter.DefaultAdapter.SupportedDisplayModes)
            {
                if (mode.Width == 1280 && mode.Height == 800)
                {
                    e.GraphicsDeviceInformation.PresentationParameters.BackBufferFormat = mode.Format;
                    e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight = mode.Height;
                    e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth = mode.Width;
                    //e.GraphicsDeviceInformation.PresentationParameters.FullScreenRefreshRateInHz = mode.RefreshRate;
                    e.GraphicsDeviceInformation.PresentationParameters.IsFullScreen = false;
                    break;
                }
            }
*/            
            // Render into a 1280x800 window
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferWidth = 1280;
            e.GraphicsDeviceInformation.PresentationParameters.BackBufferHeight = 800;
            e.GraphicsDeviceInformation.PresentationParameters.IsFullScreen = false;
            
#endif
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related Content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            session = new GameSession(this);
            menuSystem = new MenuSystem(this, session);
            hudList = new List<IHUDComponent>();

            // generic input processing
            gamepads = new GamePadsWrapper(this);
            gamepads.ButtonClicked += new GamePadsWrapper.ButtonClickEventHandler(this.OnButtonClick);

            this.Components.Add(session);
            this.Components.Add(session.Board);
            this.Components.Add(menuSystem);

#if XBOX || XBOX360
            IsFixedTimeStep = false;
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 65);
#endif

            base.Initialize();

#if DEBUG_PHYSICS
            // Skip the menus and start a single-player game
            Session.RequestStartSession();
            Session.CreatePlayers();
            Session.Player1.IsCpu = false;
#endif
        }

        /// <summary>
        /// process input to turn on off debugging
        /// </summary>
        /// <param name="index"></param>
        /// <param name="details"></param>
        private void OnButtonClick(PlayerIndex index, GamePadButtonEventDetails details)
        {
            if (session.State != GameSession.SessionState.STATE_INGAME) return;

            if (details.Button == GamePadWrapper.ButtonId.START)
            {
                session.PauseSession();
                menuSystem.ShowPauseMenu(index);
            }

#if DEBUG
            if (details.Button == GamePadWrapper.ButtonId.Y)
            {
                GlobalFlags.DrawDebugJunk = !GlobalFlags.DrawDebugJunk;
            }
            if (details.Button == GamePadWrapper.ButtonId.X)
            {
                GlobalFlags.DrawDebugBoard = !GlobalFlags.DrawDebugBoard;
            }
            if (details.Button == GamePadWrapper.ButtonId.D_LEFT)
            {
                GlobalFlags.DrawDebugForces = !GlobalFlags.DrawDebugForces;
            }
            if (details.Button == GamePadWrapper.ButtonId.D_UP)
            {
                session.Player1.GotoNextOuterCircleDrawType();
                session.Player2.GotoNextOuterCircleDrawType();
                session.Player3.GotoNextOuterCircleDrawType();
                session.Player4.GotoNextOuterCircleDrawType();
            }
            if (details.Button == GamePadWrapper.ButtonId.D_DOWN)
            {
                session.Player1.GotoNextInnerCircleDrawType();
                session.Player2.GotoNextInnerCircleDrawType();
                session.Player3.GotoNextInnerCircleDrawType();
                session.Player4.GotoNextInnerCircleDrawType();
            }
#endif
        }

        /// <summary>
        /// Load your graphics Content.  If loadAllContent is true, you should
        /// load Content from both ResourceManagementMode pools.  Otherwise, just
        /// load ResourceManagementMode.Manual Content.
        /// </summary>
        protected override void LoadContent()
        {
            GlobalResorces.LoadContent(Content);
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>(@"W_A_D\Fonts\Berlin San FB Demi 28");
            pointTexture = Content.Load<Texture2D>(@"W_A_D\Textures\white");
            primitiveBatch = new PrimitiveBatch(graphics.GraphicsDevice);
        }

        /// <summary>
        /// Unload your graphics Content.  If unloadAllContent is true, you should
        /// unload Content from both ResourceManagementMode pools.  Otherwise, just
        /// unload ResourceManagementMode.Manual Content.  Manual Content will get
        /// Disposed by the GraphicsDevice during a Reset.
        /// </summary>
        protected override void UnloadContent()
        {
            Content.Unload();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (gameTime.TotalRealTime.Seconds % 2 == 0)  // check these things less often
            {
                // check to change music track
                GameAudio.UpdateMusic();

                // check for device connectivity
                if (session.storageDevice == null)
                {
                    if (session.storageDeviceResult == null)
                        session.storageDeviceResult = Guide.BeginShowStorageDeviceSelector(PlayerIndex.One, null, null);
                    else if (session.storageDeviceResult.IsCompleted)
                        session.storageDevice = Guide.EndShowStorageDeviceSelector(session.storageDeviceResult);
                }
                else if (session.storageDevice != null)
                {
                    session.storageDeviceResult = null;
                }
            }
#if !DEBUG_PHYSICS
#if DEBUG
            // no splash screen in debug mode
            if (session.State == GameSession.SessionState.STATE_PREGAME && menuSystem.CurrentMenu == null) menuSystem.TransitionToMenu(JoinUpMenu.MenuId);
#else
            /// move from splash into the menus
            if (splash.ShowTimeRemaining < 0 && session.State == GameSession.SessionState.STATE_PREGAME && menuSystem.CurrentMenu == null) menuSystem.TransitionToMenu(TitleMenu.MenuId);
#endif
#endif
            #region PREPARING GAME
            // pre-game countdown timer
            if (session.State == GameSession.SessionState.STATE_PREPARINGGAME)
            {
                // do any pre-session-prep, pre-game initilization here
                session.StartSession();
                session.Player1.FormChanging += new Player.FormChangingEventHandler(this.OnPlayerFormAboutToChange);
                session.Player2.FormChanging += new Player.FormChangingEventHandler(this.OnPlayerFormAboutToChange);
                session.Player3.FormChanging += new Player.FormChangingEventHandler(this.OnPlayerFormAboutToChange);
                session.Player4.FormChanging += new Player.FormChangingEventHandler(this.OnPlayerFormAboutToChange);
                session.Player1.FormChanged += new Player.FormChangedEventHandler(this.OnPlayerFormChanged);
                session.Player2.FormChanged += new Player.FormChangedEventHandler(this.OnPlayerFormChanged);
                session.Player3.FormChanged += new Player.FormChangedEventHandler(this.OnPlayerFormChanged);
                session.Player4.FormChanged += new Player.FormChangedEventHandler(this.OnPlayerFormChanged);

#if !DEBUG_PHYSICS
                // create player boxes
                hudList.Clear();
                int width = 315;  // fixed height and width
                int height = 40;
                width = session.Board.CurrentLevel.PlayZone.Width / 4;
                int xPosition = session.Board.CurrentLevel.PlayZone.Left;
                int yPosition = session.Board.CurrentLevel.PlayZone.Bottom + 2; // plus a couple for the gap
                Rectangle bounds = new Rectangle(xPosition, yPosition, width, height);
                PlayerBox pb = new PlayerBox(session.Player1, bounds);
                pb.Texture = pointTexture;
                hudList.Add(pb);
                xPosition += width + 1;
                bounds = new Rectangle(xPosition, yPosition, width, height);
                pb = new PlayerBox(session.Player2, bounds);
                pb.Texture = pointTexture;
                hudList.Add(pb);
                xPosition += width + 1;
                bounds = new Rectangle(xPosition, yPosition, width, height);
                pb = new PlayerBox(session.Player3, bounds);
                pb.Texture = pointTexture;
                hudList.Add(pb);
                xPosition += width + 1;
                bounds = new Rectangle(xPosition, yPosition, width, height);
                pb = new PlayerBox(session.Player4, bounds);
                pb.Texture = pointTexture;
                hudList.Add(pb);

                // create game clock
                bounds = new Rectangle((int)(GraphicsDevice.Viewport.Width / 3.0f), session.Board.CurrentLevel.PlayZone.Top - 20, 300, 20);
                Clock clock = new Clock(bounds, session.Board.CurrentLevel.TimeInSeconds, ref session);
                hudList.Add(clock);

                // create player transition indicators
                TransitionIndicator ti = new TransitionIndicator(session.Player1);
                Texture2D tit = Content.Load<Texture2D>(@"W_A_D\Textures\transition_indicator_ring");
                ti.TransitionCircleTexture = tit;
                hudList.Add(ti);
                ti = new TransitionIndicator(session.Player2);
                ti.TransitionCircleTexture = tit;
                hudList.Add(ti);
                ti = new TransitionIndicator(session.Player3);
                ti.TransitionCircleTexture = tit;
                hudList.Add(ti);
                ti = new TransitionIndicator(session.Player4);
                ti.TransitionCircleTexture = tit;
                hudList.Add(ti);

                // create player power up indicators
                PowerUpIndicator pupi = new PowerUpIndicator(session.Player1);
                Texture2D pupitPlus = Content.Load<Texture2D>(@"W_A_D\Textures\powerup_plus_indicator_ring");
                Texture2D pupitMinus = Content.Load<Texture2D>(@"W_A_D\Textures\powerup_minus_indicator_ring");
                pupi.PlusRingTexture = pupitPlus;
                pupi.MinusRingTexture = pupitMinus;
                hudList.Add(pupi);
                pupi = new PowerUpIndicator(session.Player2);
                pupi.PlusRingTexture = pupitPlus;
                pupi.MinusRingTexture = pupitMinus;
                hudList.Add(pupi);
                pupi = new PowerUpIndicator(session.Player3);
                pupi.PlusRingTexture = pupitPlus;
                pupi.MinusRingTexture = pupitMinus;
                hudList.Add(pupi);
                pupi = new PowerUpIndicator(session.Player4);
                pupi.PlusRingTexture = pupitPlus;
                pupi.MinusRingTexture = pupitMinus;
                hudList.Add(pupi);

                // create boost indicators
                BoostMeter boostMeter = new BoostMeter(this, session.Player1);
                hudList.Add(boostMeter);
                Components.Add(boostMeter);
                boostMeter = new BoostMeter(this, session.Player2);
                hudList.Add(boostMeter);
                Components.Add(boostMeter);
                boostMeter = new BoostMeter(this, session.Player3);
                hudList.Add(boostMeter);
                Components.Add(boostMeter);
                boostMeter = new BoostMeter(this, session.Player4);
                hudList.Add(boostMeter);
                Components.Add(boostMeter);

                // create defense indicators
                DefendingIndicator dind = new DefendingIndicator(session.Player1);
                dind.DefenseRingTexture = tit;
                hudList.Add(dind);
                dind = new DefendingIndicator(session.Player2);
                dind.DefenseRingTexture = tit;
                hudList.Add(dind);
                dind = new DefendingIndicator(session.Player3);
                dind.DefenseRingTexture = tit;
                hudList.Add(dind);
                dind = new DefendingIndicator(session.Player4);
                dind.DefenseRingTexture = tit;
                hudList.Add(dind);
#endif
            }
            #endregion
            #region END GAME CLEANUP
            else if (session.State == GameSession.SessionState.STATE_ENDGAME)
            {
                menuSystem.TransitionToMenu(GameOverMenu.MenuId);
                hudList.Clear();
            }
            #endregion
            #region IN GAME UPDATE
            // only handle input if we are in a running game session the menu system handles it's own input
            else if (session.State == GameSession.SessionState.STATE_INGAME)
            {
                session.Player1.Update(gameTime);
#if !DEBUG_PHYSICS
                session.Player2.Update(gameTime);
                session.Player3.Update(gameTime);
                session.Player4.Update(gameTime);
#endif

                EntityManager.UpdatePhysics(gameTime);
#if !DEBUG_PHYSICS

                #region Player Defense Check and Collisions
                HandlePotentialPlayerDefending(session.Player1);
                HandlePotentialPlayerDefending(session.Player2);
                HandlePotentialPlayerDefending(session.Player3);
                HandlePotentialPlayerDefending(session.Player4);
                if (session.Player1.IsAbleToDefend && session.Player1.IsTryingToDefend)
                {
                    HandlePotentialDefendingCollision(session.Player1.Bubble.CenterPoint.Position, session.Player2);
                    HandlePotentialDefendingCollision(session.Player1.Bubble.CenterPoint.Position, session.Player3);
                    HandlePotentialDefendingCollision(session.Player1.Bubble.CenterPoint.Position, session.Player4);
                }
                if (session.Player2.IsAbleToDefend && session.Player2.IsTryingToDefend)
                {
                    HandlePotentialDefendingCollision(session.Player2.Bubble.CenterPoint.Position, session.Player1);
                    HandlePotentialDefendingCollision(session.Player2.Bubble.CenterPoint.Position, session.Player4);
                    HandlePotentialDefendingCollision(session.Player2.Bubble.CenterPoint.Position, session.Player3);
                }
                if (session.Player3.IsAbleToDefend && session.Player3.IsTryingToDefend)
                {
                    HandlePotentialDefendingCollision(session.Player3.Bubble.CenterPoint.Position, session.Player4);
                    HandlePotentialDefendingCollision(session.Player3.Bubble.CenterPoint.Position, session.Player1);
                    HandlePotentialDefendingCollision(session.Player3.Bubble.CenterPoint.Position, session.Player2);
                }
                if (session.Player4.IsAbleToDefend && session.Player4.IsTryingToDefend)
                {
                    HandlePotentialDefendingCollision(session.Player4.Bubble.CenterPoint.Position, session.Player2);
                    HandlePotentialDefendingCollision(session.Player4.Bubble.CenterPoint.Position, session.Player3);
                    HandlePotentialDefendingCollision(session.Player4.Bubble.CenterPoint.Position, session.Player1);
                }
                #endregion

                #region Player Solid to Player Solid Collisions
                if (session.Player1.Form == Player.PlayerForm.SOLID)
                {
                    // check against p2
                    if (session.Player2.Form == Player.PlayerForm.SOLID) HandlePotentialPlayerToPlayerCollision(session.Player1, session.Player2);
                    // check against p3
                    if (session.Player3.Form == Player.PlayerForm.SOLID) HandlePotentialPlayerToPlayerCollision(session.Player1, session.Player3);
                    // check against p4
                    if (session.Player4.Form == Player.PlayerForm.SOLID) HandlePotentialPlayerToPlayerCollision(session.Player1, session.Player4);
                }
                if (session.Player2.Form == Player.PlayerForm.SOLID)
                {
                    // check against p3
                    if (session.Player3.Form == Player.PlayerForm.SOLID) HandlePotentialPlayerToPlayerCollision(session.Player2, session.Player3);
                    // check against p4
                    if (session.Player4.Form == Player.PlayerForm.SOLID) HandlePotentialPlayerToPlayerCollision(session.Player2, session.Player4);
                }
                if (session.Player3.Form == Player.PlayerForm.SOLID)
                {
                    // check against p4
                    if (session.Player4.Form == Player.PlayerForm.SOLID) HandlePotentialPlayerToPlayerCollision(session.Player3, session.Player4);
                }
                #endregion

                #region Player to Block Collisions
                HandlePotentialSolidPlayerToBlockCollision(session.Player1);
                HandlePotentialSolidPlayerToBlockCollision(session.Player2);
                HandlePotentialSolidPlayerToBlockCollision(session.Player3);
                HandlePotentialSolidPlayerToBlockCollision(session.Player4);
                HandlePotentialBubblePlayersToBlocksCollision();
                #endregion

                #region Player to Solid Slot Collisions
                HandlePotentialSolidPlayerToSlotCollision(session.Player1);
                HandlePotentialSolidPlayerToSlotCollision(session.Player2);
                HandlePotentialSolidPlayerToSlotCollision(session.Player3);
                HandlePotentialSolidPlayerToSlotCollision(session.Player4);
                #endregion

                #region Check For Game End
                // check for a win condition
                Player winner = null;
                string winString = "";
                if (AmIAWinner(session.Player1, session.Board.CurrentLevel.Player1WinRules, ref winString)) winner = session.Player1;
                else if (AmIAWinner(session.Player2, session.Board.CurrentLevel.Player2WinRules, ref winString)) winner = session.Player2;
                else if (AmIAWinner(session.Player3, session.Board.CurrentLevel.Player3WinRules, ref winString)) winner = session.Player3;
                else if (AmIAWinner(session.Player4, session.Board.CurrentLevel.Player4WinRules, ref winString)) winner = session.Player4;

                // quit, we found a winner
                if (winner != null)
                {
                    // TODO turn on display of player X wins graphics and animations
                    session.SetWinner(winner, GameSession.WinType.BOARD_WIN_RULES, winString);
                    session.EndSession();
                }
                // quit, if we can win by filling the board and if no more blocks avail do the win check
                else if (session.Board.CurrentLevel.Player1WinRules.LockTheMostBlocks &&
                    session.Board.BlockPreAllocationCount == 0 &&
                    session.Board.FreeBlocksInPlay.Count == 0)
                {
                    // check to see if all blocks in slots are locked
                    bool allLocked = true;
                    foreach (Block b in session.Board.BlocksInSlots)
                    {
                        if (!b.IsLocked)
                        {
                            allLocked = false;
                            break;
                        }
                    }
                    if (allLocked)
                    {
                        // TODO animate the block counting
                        List<Player> winners = GetPointsLeader();
                        if (winners.Count == 1) winner = winners[0];
                        if (winner != null) session.SetWinner(winner, GameSession.WinType.POINTS_SLOTS_FILLED, "");
                        else
                        {
                            session.GameOverText = "A tie!  Going to sudden death...";
                            session.GotoSuddenDeath(winners);
                        }
                        session.EndSession();
                    }
                }
                // quit, game time expired
                else if (session.EllapsedSessionTime.TotalSeconds >= session.Board.CurrentLevel.TimeInSeconds)
                {
                    // TODO animate the block counting 
                    List<Player> winners = GetPointsLeader();
                    if (winners.Count == 1) winner = winners[0];
                    if (winner != null) session.SetWinner(winner, GameSession.WinType.POINTS_TIMER_EXPIRED, "");
                    else
                    {
                        session.GameOverText = "A tie!  Going to sudden death...";
                        session.GotoSuddenDeath(winners);
                    }
                    session.EndSession();
                }
                #endregion
#endif
            }
            #endregion

            base.Update(gameTime);
        }

        /// <summary>
        /// If the player is trying to defend make sure they are allowed,
        /// if so then turn the defence on
        /// </summary>
        /// <param name="player"></param>
        private void HandlePotentialPlayerDefending(Player player)
        {
            if (!player.IsTryingToDefend)
            {
                player.IsAbleToDefend = false;
                return;
            }
            // check to see if we are above an unlocked slot that we own
            foreach (Block block in session.Board.BlocksInSlots)
            {
                // we need to be colliding with a block in a slot
                if (!CollisionDetective.CheckCollision(player.Bubble.CenterPoint.Position, block.Position, block.GrabRadius + player.GrabModifier)) continue;
                // we need to be the owner of that block
                if (block.OwningPlayer != player) continue;
                // that block needs to be unlocked
                if (block.IsLocked) continue;

                // if we got here then we can defend
                player.IsAbleToDefend = true;
                // stop the player bubble
                foreach (VerletPoint p in player.Bubble.InnerCircle)
                {
                    p.SetPosition(p.LastPosition);
                }
                return;
            }
            // if we got here then we couldn't find a block to defend
            player.IsAbleToDefend = false;
        }

        /// <summary>
        /// Check to see if the player should be pushed away from the defense
        /// </summary>
        /// <param name="defenseCenter"></param>
        /// <param name="player"></param>
        private void HandlePotentialDefendingCollision(Vector2 defenseCenter, Player player)
        {
            float defenseRadius = player.Bubble.MaxBoundingRadius;
            foreach (VerletPoint point in player.Bubble.OuterCircle)
            {
                if (CollisionDetective.CheckCollision(point.Position, defenseCenter, defenseRadius))
                {
                    // invert point velocity (makes players sticky)
                    //Vector2 tempPos = point.LastPosition;  // store current position
                    //point.SetPosition(point.Position); // hard set to last position
                    //point.MoveTo(tempPos); // now move back to where we are

                    // all stop the collided points
                    point.SetPosition(point.LastPosition);
                }
            }
        }

        /// <summary>
        /// Do a rough check for and handle a collision between 2 players (solid form only)
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        private void HandlePotentialPlayerToPlayerCollision(Player p1, Player p2)
        {
            foreach (VerletPoint point in p1.Bubble.OuterCircle)
            {
                if (CollisionDetective.CheckCollision(point.Position, p2.Bubble.CenterPoint.Position, p2.Bubble.OuterRadius))
                {
                    // invert point velocity (makes players sticky)
                    //Vector2 tempPos = point.LastPosition;  // store current position
                    //point.SetPosition(point.Position); // hard set to last position
                    //point.MoveTo(tempPos); // now move back to where we are

                    // all stop the collided points
                    point.SetPosition(point.LastPosition);
                }
            }
            foreach (VerletPoint point in p2.Bubble.OuterCircle)
            {
                if (CollisionDetective.CheckCollision(point.Position, p1.Bubble.CenterPoint.Position, p1.Bubble.OuterRadius))
                {
                    // invert point velocity (makes players sticky)
                    //Vector2 tempPos = point.LastPosition;  // store current position
                    //point.SetPosition(point.Position); // hard set to last position
                    //point.MoveTo(tempPos); // now move back to where we are

                    // all stop the collided points
                    point.SetPosition(point.LastPosition);
                }
            }
        }

        /// <summary>
        /// do a check to see if the solid player collides with any blocks and if so take appropiate action
        /// </summary>
        /// <param name="player"></param>
        private void HandlePotentialSolidPlayerToBlockCollision(Player player)
        {
            foreach (Block block in session.Board.AllBlocksInPlay)
            {
                if (block.OwningPlayer != null) continue;  // only check unowned "free" blocks
                if (player.Form != Player.PlayerForm.SOLID) continue; // only check solid players
                if (CollisionDetective.CheckCollision(player.Bubble.CenterPoint.Position, player.Bubble.OuterRadius, block.Position, block.BoundingRadius))
                {
                    // push block away from solid bubble
                    Vector2 diff = block.Position - player.Bubble.CenterPoint.Position;  // vector from bubble center to block center
                    diff.Normalize();
                    if (float.IsNaN(diff.X) || float.IsNaN(diff.Y))
                    {
                        Debug.WriteLine("HBBBGame - player collision is trying to move block to NaN!");
                        continue;
                    }
                    block.Position = player.Bubble.CenterPoint.Position + diff * player.Bubble.MaxBoundingRadius;
                    block.Velocity += SOLID_TO_BLOCK_REPLUSION_VELOCITY_COEFF * diff; // constant replusion along vector of collision
                }
                session.Board.EnsureBlockIsOnBoard(block);
            }
        }

        /// <summary>
        /// do a check to see if the solid player collides with any slots and if so take appropiate action
        /// </summary>
        /// <param name="player"></param>
        private void HandlePotentialSolidPlayerToSlotCollision(Player player)
        {
            foreach (Slot slot in session.Board.Slots)
            {
                if (!slot.PlayerSolidCollisionsEnabled && player.Form == Player.PlayerForm.SOLID) continue; // solid and solid collisions disabled
                if (!slot.PlayerBubbleCollisionsEnabled && player.Form != Player.PlayerForm.SOLID) continue; // bubble and bubble collisions disabled
                // test outer bounds before going through entire points list
                if (!CollisionDetective.CheckCollision(player.Bubble.CenterPoint.Position, player.Bubble.OuterRadius, slot.Position, slot.Bounds.X)) continue;
                // go through bubble points and check collisions
                foreach (VerletPoint point in player.Bubble.OuterCircle)
                {
                    if (CollisionDetective.CheckCollision(point.Position, slot.Bounds))
                    {
                        // invert point velocity
                        Vector2 tempPos = point.LastPosition;  // store current position
                        point.SetPosition(point.Position); // hard set to last position
                        point.MoveTo(tempPos); // now move back to where we are

                        // all stop the collided points
                        //point.SetPosition(point.LastPosition);

                        // pop player solid ball
                        if (player.Form == Player.PlayerForm.SOLID && slot.SpecialMode == Slot.SpecialModeType.BUBBLE_POPPING_SLOT) player.ChangeForm(Player.PlayerForm.BUBBLE);
                    }
                }
            }
        }

        /// <summary>
        /// do a check to see if bubble player collides with any blocks and if so suck it toward the center
        /// </summary>
        private void HandlePotentialBubblePlayersToBlocksCollision()
        {
            bool p1Collides;
            bool p2Collides;
            bool p3Collides;
            bool p4Collides;
            Player player;
            foreach (Block block in session.Board.AllBlocksInPlay)
            {
                p1Collides = false;
                p2Collides = false;
                p3Collides = false;
                p4Collides = false;
                // see who collides
                if (session.Player1.Form != Player.PlayerForm.SOLID &&
                    CollisionDetective.CheckCollision(session.Player1.Bubble.CenterPoint.Position,
                    session.Player1.Bubble.OuterRadius, block.Position, block.BoundingRadius))
                {
                    p1Collides = true;
                }
                if (session.Player2.Form != Player.PlayerForm.SOLID &&
                    CollisionDetective.CheckCollision(session.Player2.Bubble.CenterPoint.Position,
                    session.Player2.Bubble.OuterRadius, block.Position, block.BoundingRadius))
                {
                    p2Collides = true;
                }
                if (session.Player3.Form != Player.PlayerForm.SOLID &&
                    CollisionDetective.CheckCollision(session.Player3.Bubble.CenterPoint.Position,
                    session.Player3.Bubble.OuterRadius, block.Position, block.BoundingRadius))
                {
                    p3Collides = true;
                }
                if (session.Player4.Form != Player.PlayerForm.SOLID &&
                    CollisionDetective.CheckCollision(session.Player4.Bubble.CenterPoint.Position,
                    session.Player4.Bubble.OuterRadius, block.Position, block.BoundingRadius))
                {
                    p4Collides = true;
                }

                // check to see if any players collide (if more than one player collides, we ignore the whole thing)
                if (p1Collides && !p2Collides && !p3Collides && !p4Collides) player = session.Player1;
                else if (!p1Collides && p2Collides && !p3Collides && !p4Collides) player = session.Player2;
                else if (!p1Collides && !p2Collides && p3Collides && !p4Collides) player = session.Player3;
                else if (!p1Collides && !p2Collides && !p3Collides && p4Collides) player = session.Player4;
                else continue;

                // make the block want to come to or repulse from the player bubble
                Vector2 diff = block.Position - player.Bubble.CenterPoint.Position;
                block.Velocity = BUBBLE_TO_BLOCK_REPLUSION_VELOCITY_COEFF * diff;
            }
        }

        /// <summary>
        /// Player is about to change forms
        /// </summary>
        /// <param name="player"></param>
        /// <returns>false if player form change is denied</returns>
        public bool OnPlayerFormAboutToChange(Player player, Player.PlayerForm newForm)
        {
            if (newForm != Player.PlayerForm.SOLID) return true;  // only check transition to solid
            bool changeOk = true;
            // TODO check if other solid players are colliding with us
            return changeOk;
        }

        /// <summary>
        /// Handle game activity for player form changed
        /// </summary>
        public void OnPlayerFormChanged(Player player)
        {
            #region Transition from bubble to solid
            if (player.Form == Player.PlayerForm.SOLID)
            {
                foreach (Block block in session.Board.AllBlocksInPlay) // check to pickup blocks in play (free, other player payloads, powerups)
                {
                    // if block is close enough to bubble center
                    if (CollisionDetective.CheckCollision(player.Bubble.CenterPoint.Position, block.Position, block.GrabRadius + player.GrabModifier))
                    {
                        if (block.OwningPlayer != null) // player stole if from another player bubble
                        {
                            player.Statistics.Steals++;
                            block.OwningPlayer.Statistics.StolenFrom++;
                            block.OwningPlayer.PayloadStolen();  // clear other player ref to block
                        }
                        player.AddPayload(block);
                        return;  // only pick up one block
                    }
                }

                foreach (Block block in session.Board.BlocksInSlots) // check to pickup blocks from slots
                {
                    if (block.IsLocked) continue;

                    // if block is close enough to bubble center
                    if (CollisionDetective.CheckCollision(player.Bubble.CenterPoint.Position, block.Position, block.GrabRadius + player.GrabModifier))
                    {
                        if (block.OwningSlot != null)
                        {
                            if (block.OwningPlayer != player) player.Statistics.Steals++;  // player stole it from a slot
                            if (block.OwningPlayer != player) block.OwningPlayer.Statistics.StolenFrom++;
                            session.Board.PullBlockFromSlot(block);  // clear old slot ref to block
                        }
                        player.AddPayload(block);
                        return;  // only pick up one block
                    }
                }
            }
            #endregion
            #region Transition from solid to bubble
            else if (player.Form == Player.PlayerForm.BUBBLE)
            {
                if (player.Payload != null)
                {
                    Slot dropSlot = null;
                    #region special processing for power up type payloads
                    if (player.Payload is PowerUp)
                    {
                        foreach (Slot currentSlot in session.Board.Slots)
                        {
                            if (currentSlot.Block == null || currentSlot.Block.IsLocked == false) continue;  // cannot drop into an unoccupied or unlocked slot
                            // if player center is over slot
                            if (CollisionDetective.CheckCollision(player.Bubble.CenterPoint.Position, currentSlot.Bounds))
                            {
                                dropSlot = currentSlot;
                                break;
                            }
                        }

                        if (dropSlot == null ||
                            dropSlot.SpecialMode == Slot.SpecialModeType.SOURCE_SLOT ||
                            dropSlot.SpecialMode == Slot.SpecialModeType.DEAD_SLOT)
                        {
                            player.FreePayload();  // set powerup failed 
                            player.Statistics.MisDrops++;
                        }
                        else  // successful drop of powerup
                        {
                            PowerUp pup = (PowerUp)player.Payload;
                            player.DropPayload();
                            session.Board.SetPowerUpInSlot(pup, dropSlot);
                        }
                    }
                    #endregion
                    #region process as a normal block
                    else
                    {
                        foreach (Slot currentSlot in session.Board.Slots)
                        {
                            if (currentSlot.Block != null) continue;  // cannot drop into occupied slot
                            // if player center is over slot
                            if (CollisionDetective.CheckCollision(player.Bubble.CenterPoint.Position, currentSlot.Bounds))
                            {
                                // check to see if an adjacent slot has a corresponding player color
                                if (IsSlotAvailableForDrop(currentSlot, player) == false) continue;  // sorry, cant do it
                                dropSlot = currentSlot;
                                break;
                            }
                        }

                        if (dropSlot == null ||
                            dropSlot.SpecialMode == Slot.SpecialModeType.SOURCE_SLOT ||
                            dropSlot.SpecialMode == Slot.SpecialModeType.DEAD_SLOT)
                        {
                            player.FreePayload();  // set block failed 
                            player.Statistics.MisDrops++;
                        }
                        else  // successful drop
                        {
                            Block block = (Block)player.Payload;
                            player.DropPayload();
                            session.Board.SetBlockInSlot(block, dropSlot);
                            player.Statistics.BlocksSet++;  // update stats
                            CheckForCircumscribedSlots(dropSlot, player);// perform acquisition of enclosed slots on board
                        }
                    }
                    #endregion
                }
            }
            #endregion
        }

        /// <summary>
        /// Check to see if the argument slot is available for the argument player to drop
        /// a block into.  In order for a slot to be valid it must be adjacent to another
        /// player owned slot (there are 6 a max of adjacent slots).
        /// </summary>
        /// <param name="slotId"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool IsSlotAvailableForDrop(Slot s, Player player)
        {
            return s.IsSlotAdjacentToPlayerBlocks(ref player);
        }

        /// <summary>
        /// If any unlocked slots are surrounded by player locked blocks, he acquires them
        /// </summary>
        private void CheckForCircumscribedSlots(Slot dropSlot, Player player)
        {
            bool performedCircumscription = false; // ;)
            foreach (Slot adjSlot in dropSlot.adjacentSlots) // all the slots touching this one could now potentially be circumscribed
            {
                if (adjSlot.Block != null) continue;  // it already has a block goto next
                if (adjSlot.SpecialMode == Slot.SpecialModeType.DEAD_SLOT) continue;
                if (adjSlot.SpecialMode == Slot.SpecialModeType.SOURCE_SLOT) continue;
                if (adjSlot.IsPlayerOwnerOfAllAdjacent(ref player)) // it is circumscribed
                {
                    performedCircumscription = true;
                    // add a block to the board and set it into this slot for the player
                    Block b = session.Board.AddBlockToBoard(adjSlot.Position);
                    if (b == null)
                    {
                        Debug.WriteLine("HBBBGame::CheckForCircumscribedSlots - couldnt add block, new block is null");
                        continue;
                    }
                    b.OwningPlayer = player;
                    session.Board.SetBlockInSlot(b, adjSlot);
                    player.Statistics.BlocksSet++;  // update stats

                    GameAudio.PlayCue("bell_chimes");
                    foreach (Slot s in adjSlot.adjacentSlots) s.Block.AnimateFlash(0.5);  // this will get put back to normal in the block update function
                }
            }
            // give em a power up to lock it
            if (performedCircumscription) session.Board.CreateAndEmitPowerUp(PowerUpFactory.PowerUpId.POWERUP_LOCK_ALL, player.Bubble.CenterPoint.Position); // emit a lock all onto the board
        }

#if TODO_FIX_THIS
        private void CheckForCircumscribedSlots(Player player)
        {
            List<Slot> ignoreList = new List<Slot>();
            List<Slot> knownDependencies = new List<Slot>();
            List<Block> enclosingBlocks = new List<Block>();
            for (int iter = 0; iter < session.Board.Slots.Count; iter++)
            {
                Slot curSlot = session.Board.Slots[iter];
                if (curSlot.Block != null || ignoreList.Contains(curSlot)) continue;  // ignore and goto next block
                ignoreList.Clear();  // need to start a new ignore list
                knownDependencies.Clear();
                enclosingBlocks.Clear();
                knownDependencies.Add(curSlot);
                // check surrounding slots recursively until we've checked all (success) or found a reason to stop (failure)
                if (RecursiveCircumscribedDependencyCheck(player, curSlot, ref ignoreList, ref knownDependencies, ref enclosingBlocks))
                {
                    foreach (Slot s in knownDependencies)
                    {
                        if (s.SpecialMode == Slot.SpecialModeType.DEAD_SLOT) continue;
                        if (s.SpecialMode == Slot.SpecialModeType.SOURCE_SLOT) continue;
                        // add a block to the board and set it into this slot for the player
                        Block b = session.Board.AddBlockToBoard(s.Position);
                        if (b == null)
                        {
                            Debug.WriteLine("HBBBGame::CheckForCircumscribedSlots - couldnt add block, new block is null");
                            continue;
                        }
                        b.OwningPlayer = player;
                        session.Board.SetBlockInSlot(b, s);
                        player.Statistics.BlocksSet++;  // update stats
                    }
                    foreach (Block b in enclosingBlocks) b.AnimateFlash(0.5);  // this will get put back to normal in the block update function

                    session.Board.CreateAndEmitPowerUp(PowerUpFactory.PowerUpId.POWERUP_LOCK_ALL, player.Bubble.CenterPoint.Position); // emit a lock all onto the board
                }
                else
                {
                    // this slot is not part of an enclosed group
                }
            }
        }

        /// <summary>
        /// Recursive function to iterate dependencies to check for circumscribed blocks
        /// </summary>
        /// <param name="slot"></param>
        /// <param name="ignoreList"></param>
        /// <param name="knownDepends"></param>
        /// <param name="enclosingBlocks"></param>
        /// <returns></returns>
        private bool RecursiveCircumscribedDependencyCheck(Player player, Slot slot, ref List<Slot> ignoreList, ref List<Slot> knownDepends, ref List<Block> enclosingBlocks)
        {
            if (ignoreList.Contains(slot)) return true;  // it didn't pass or fail, but we are are already checking it higher in the list, just assume fail will happen then if necessary
            ignoreList.Add(slot);  // we are checking it now, don't check it again

            foreach (Slot adjSlot in slot.adjacentSlots)
            {
                Block b = adjSlot.Block;
                if (b != null && b.OwningPlayer == player) enclosingBlocks.Add(b);
                else if (b != null) return false;  // touching another player's blocks, no enclosure
                else
                {
                    // if our dependency failed return false for us too
                    if (RecursiveCircumscribedDependencyCheck(player, adjSlot, ref ignoreList, ref knownDepends, ref enclosingBlocks) == false) return false;
                }
            }

            if (!knownDepends.Contains(slot)) knownDepends.Add(slot);  // everything checked out, it's a dependency
            return true;
        }
#endif

        /// <summary>
        /// Check for the win
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        private bool AmIAWinner(Player player, BoardLevel.WinRules rules, ref string winText)
        {
            if (rules == null) return false;

            if (rules.TouchTheSource)
            {
                foreach (Slot s in session.Board.SourceSlots)
                {
                    // search surrounding slots for any owned and locked by the player
                    if (s.IsSlotAdjacentToLockedPlayerBlocks(ref player))
                    {
                        winText = "being first to reach the source";
                        return true;
                    }
                }
            }
            if (rules.TouchPlayZoneTop)
            {
                foreach (Slot s in session.Board.TopEdgeSlots)
                {
                    if (s.Block != null && s.Block.IsLocked && s.Block.OwningPlayer == player)
                    {
                        winText = "touching the top edge";
                        return true;
                    }
                }
            }
            if (rules.TouchPlayZoneLeft)
            {
                foreach (Slot s in session.Board.LeftEdgeSlots)
                {
                    if (s.Block != null && s.Block.IsLocked && s.Block.OwningPlayer == player)
                    {
                        winText = "touching the left edge";
                        return true;
                    }
                }
            }
            if (rules.TouchPlayZoneRight)
            {
                foreach (Slot s in session.Board.RightEdgeSlots)
                {
                    if (s.Block != null && s.Block.IsLocked && s.Block.OwningPlayer == player)
                    {
                        winText = "touching the right edge";
                        return true;
                    }
                }
            }
            if (rules.TouchPlayZoneBottom)
            {
                foreach (Slot s in session.Board.BottomEdgeSlots)
                {
                    if (s.Block != null && s.Block.IsLocked && s.Block.OwningPlayer == player)
                    {
                        winText = "touching the bottom edge";
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Check to see which player is leading in points
        /// </summary>
        /// <returns>A list of players with highest point count</returns>
        List<Player> GetPointsLeader()
        {
            List<Player> leaders = new List<Player>();
            // assume p1 and then check against other players
            leaders.Add(session.Player1);
            // check p2
            if (session.Player2.Statistics.Score > leaders[0].Statistics.Score)
            {
                leaders.Clear();
                leaders.Add(session.Player2);
            }
            else if (session.Player2.Statistics.Score == leaders[0].Statistics.Score)
            {
                leaders.Add(session.Player2);
            }
            // check p3
            if (session.Player3.Statistics.Score > leaders[0].Statistics.Score)
            {
                leaders.Clear();
                leaders.Add(session.Player3);
            }
            else if (session.Player3.Statistics.Score == leaders[0].Statistics.Score)
            {
                leaders.Add(session.Player3);
            }
            // check p4
            if (session.Player4.Statistics.Score > leaders[0].Statistics.Score)
            {
                leaders.Clear();
                leaders.Add(session.Player4);
            }
            else if (session.Player4.Statistics.Score == leaders[0].Statistics.Score)
            {
                leaders.Add(session.Player4);
            }


            return leaders;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (session.State == GameSession.SessionState.STATE_PREGAME) graphics.GraphicsDevice.Clear(Color.Black);
            else graphics.GraphicsDevice.Clear(Color.Gray);

            base.Draw(gameTime);  // this makes sure that the board draws first

            if (session.State == GameSession.SessionState.STATE_PREPARINGGAME)
            {
                string text = "Loading board, initializing game...";

                spriteBatch.Begin();
                spriteBatch.DrawString(spriteFont, text, new Vector2(400, 300) + new Vector2(1, 1), Color.Black); // font shadow
                spriteBatch.DrawString(spriteFont, text, new Vector2(400, 300), Color.White); // the actual text
                spriteBatch.End();
            }
            else if (session.State == GameSession.SessionState.STATE_INGAME)
            {
                // the board is a DrawableGameComponent that draws itself: GameBoard::Draw(GameTime)
                // the board draw routine also draws the source, the blocks and the slots
                spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
#if DEBUG
                if (GlobalFlags.DrawDebugJunk)
                {
                    // draw the game time
                    spriteBatch.DrawString(spriteFont, "Ellapsed: " + gameTime.TotalGameTime.Minutes.ToString() + ":" + gameTime.TotalGameTime.Seconds.ToString() + "." + gameTime.TotalGameTime.Milliseconds.ToString(),
                        new Vector2(10, 5), Color.White); // the actual text
                }
#endif

                // draw player bubbles
                DrawPlayer2D(PlayerIndex.One, session.Player1);

#if !DEBUG_PHYSICS
                DrawPlayer2D(PlayerIndex.Two, session.Player2);
                DrawPlayer2D(PlayerIndex.Three, session.Player3);
                DrawPlayer2D(PlayerIndex.Four, session.Player4);
#endif
                // draw HUD
                if (showHUDFlag)
                {
                    foreach (IHUDComponent hud in hudList)
                    {
                        hud.Draw(spriteBatch, spriteFont, primitiveBatch);
#if DEBUG
                        if (GlobalFlags.DrawDebugJunk) hud.DebugRender(primitiveBatch);
#endif
                    }
                }
                spriteBatch.End();
            }
        }

        /// <summary>
        /// Draw this player using sprites in 2D
        /// </summary>
        /// <param name="index"></param>
        private void DrawPlayer2D(PlayerIndex index, Player player)
        {
            primitiveBatch.SetAlpha(1.0f);  // reset alpha

            if (player == null) throw new Exception("HBBBGame::DrawPlayer - player is null");
            Bubble bubble = player.Bubble;

            #region Draw Outer Circle
            if (player.OuterCircleDrawType == Bubble.BubbleDrawType.TEXTURED)
            {
                // draw outer bubble
                if (player.Form == Player.PlayerForm.SOLID) primitiveBatch.SetAlpha(SOLID_TRANSPARENCY); // solid is a little darker
                else primitiveBatch.SetAlpha(BUBBLE_TRANSPARENCY);
                primitiveBatch.Begin(PrimitiveType.TriangleList);
                primitiveBatch.SetTexture(bubble.OuterCircleTexture);
                List<VerletPoint> outerCircle = bubble.OuterCircle;
                for (int thisPoint = 0; thisPoint < outerCircle.Count; thisPoint++)
                {
                    int nextPoint = (thisPoint + 1) % bubble.Segments;  // modulo so that the last index will wrap around to the first
                    primitiveBatch.AddVertex(outerCircle[thisPoint].Position, outerCircle[thisPoint].TextureCoords);
                    primitiveBatch.AddVertex(outerCircle[nextPoint].Position, outerCircle[nextPoint].TextureCoords);
                    primitiveBatch.AddVertex(bubble.CenterPoint.Position, bubble.CenterPoint.TextureCoords);
                }
                primitiveBatch.End();
            }
            else if (player.OuterCircleDrawType == Bubble.BubbleDrawType.TRIANGLES)
            {
                // draw outer bubble
                if (player.Form == Player.PlayerForm.SOLID) primitiveBatch.SetAlpha(SOLID_TRANSPARENCY); // solid is a little darker
                else primitiveBatch.SetAlpha(BUBBLE_TRANSPARENCY);
                primitiveBatch.Begin(PrimitiveType.TriangleList);
                List<VerletPoint> outerCircle = bubble.OuterCircle;
                for (int thisPoint = 0; thisPoint < outerCircle.Count; thisPoint++)
                {
                    int nextPoint = (thisPoint + 1) % bubble.Segments;  // modulo so that the last index will wrap around to the first
                    primitiveBatch.AddVertex(outerCircle[thisPoint].Position, player.PrimaryColor);
                    primitiveBatch.AddVertex(outerCircle[nextPoint].Position, player.PrimaryColor);
                    primitiveBatch.AddVertex(bubble.CenterPoint.Position, player.PrimaryColor);
                }
                primitiveBatch.End();
            }
            else if (player.OuterCircleDrawType == Bubble.BubbleDrawType.LINES)
            {
                // draw outer constraints
                foreach (VerletPoint p in bubble.OuterCircle)
                {
                    foreach (IVerletConstraint constraint in p.Constraints)
                    {
                        primitiveBatch.Begin(PrimitiveType.LineList);
                        constraint.DebugRender(primitiveBatch, p, player.PrimaryColor);
                        primitiveBatch.End();
                    }
                }
            }
            else if (player.OuterCircleDrawType == Bubble.BubbleDrawType.POINTS)
            {
                // draw outer points
                foreach (VerletPoint p in bubble.OuterCircle)
                {
                    spriteBatch.Draw(pointTexture, new Rectangle((int)p.Position.X, (int)p.Position.Y, 2, 2), player.PrimaryColor);
                }
            }
            #endregion

            #region Draw Inner Circle (over top of outer circle)
            if (!(player.Form == Player.PlayerForm.SOLID))  // hide inner circle in solid form
            {
                if (player.InnerCircleDrawType == Bubble.BubbleDrawType.TEXTURED)
                {
                    // draw inner bubble
                    primitiveBatch.Begin(PrimitiveType.TriangleList);
                    primitiveBatch.SetTexture(bubble.InnerCircleTexture);
                    List<VerletPoint> innerCircle = bubble.InnerCircle;
                    for (int thisPoint = 0; thisPoint < innerCircle.Count; thisPoint++)
                    {
                        int nextPoint = (thisPoint + 1) % bubble.Segments;  // modulo so that the last index will wrap around to the first
                        primitiveBatch.AddVertex(innerCircle[thisPoint].Position, innerCircle[thisPoint].TextureCoords);
                        primitiveBatch.AddVertex(innerCircle[nextPoint].Position, innerCircle[nextPoint].TextureCoords);
                        primitiveBatch.AddVertex(bubble.CenterPoint.Position, bubble.CenterPoint.TextureCoords);
                    }
                    primitiveBatch.End();
                }
                else if (player.InnerCircleDrawType == Bubble.BubbleDrawType.TRIANGLES)
                {
                    // draw inner bubble
                    primitiveBatch.Begin(PrimitiveType.TriangleList);
                    List<VerletPoint> innerCircle = bubble.InnerCircle;
                    for (int thisPoint = 0; thisPoint < innerCircle.Count; thisPoint++)
                    {
                        int nextPoint = (thisPoint + 1) % bubble.Segments;  // modulo so that the last index will wrap around to the first
                        primitiveBatch.AddVertex(innerCircle[thisPoint].Position, player.SecondaryColor);
                        primitiveBatch.AddVertex(innerCircle[nextPoint].Position, player.SecondaryColor);
                        primitiveBatch.AddVertex(bubble.CenterPoint.Position, player.SecondaryColor);
                    }
                    primitiveBatch.End();
                }
                else if (player.InnerCircleDrawType == Bubble.BubbleDrawType.LINES)
                {
                    // draw inner constraints
                    foreach (VerletPoint p in bubble.InnerCircle)
                    {
                        foreach (IVerletConstraint constraint in p.Constraints)
                        {
                            primitiveBatch.Begin(PrimitiveType.LineList);
                            constraint.DebugRender(primitiveBatch, p, player.SecondaryColor);
                            primitiveBatch.End();
                        }
                    }
                }
                else if (player.InnerCircleDrawType == Bubble.BubbleDrawType.POINTS)
                {
                    // draw inner points
                    foreach (VerletPoint p in bubble.InnerCircle)
                    {
                        spriteBatch.Draw(pointTexture, new Rectangle((int)p.Position.X, (int)p.Position.Y, 2, 2), player.SecondaryColor);
                    }
                }
            }
            #endregion

            // draw player pic
            spriteBatch.Draw(player.PlayerPic, new Vector2(bubble.CenterPoint.Position.X - player.PlayerPic.Width / 2, bubble.CenterPoint.Position.Y - player.PlayerPic.Height / 2), new Color(255, 255, 255, 64));

#if DEBUG
            #region Draw Debug
            if (GlobalFlags.DrawDebugForces)
            {
                bubble.DrawDebugForces(primitiveBatch);
            }

            if (GlobalFlags.DrawDebugJunk)
            {
                // draw outer constraints
                foreach (VerletPoint p in bubble.OuterCircle)
                {
                    foreach (IVerletConstraint constraint in p.Constraints)
                    {
                        primitiveBatch.Begin(PrimitiveType.LineList);
                        constraint.DebugRender(primitiveBatch, p, player.PrimaryColor);
                        primitiveBatch.End();
                    }
                }
                // draw inner constraints
                foreach (VerletPoint p in bubble.InnerCircle)
                {
                    foreach (IVerletConstraint constraint in p.Constraints)
                    {
                        primitiveBatch.Begin(PrimitiveType.LineList);
                        constraint.DebugRender(primitiveBatch, p, player.SecondaryColor);
                        primitiveBatch.End();
                    }
                }
                // draw player form indication text
                string text = "BUBBLE";
                if (player.Form == Player.PlayerForm.IN_TRANSITION)
                {
                    text = "TRANSITION: " + player.TimeInTransition + @"/" + player.TimeRequiredToTransition;
                }
                else if (player.Form == Player.PlayerForm.SOLID) text = "SOLID";
                spriteBatch.DrawString(spriteFont, text, player.Bubble.CenterPoint.Position, Color.White);
            }
            #endregion
#endif
        }
    }
}
