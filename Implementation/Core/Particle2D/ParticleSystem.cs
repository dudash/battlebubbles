#region Copyright
//-----------------------------------------------------------------------------
// Copyright (C)2008 Jason Dudash, GNU GPLv3
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
// File Created: 30 July 2006, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System;
using System.Collections;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HBBB.Core.Particle2D
{
    /// <summary>
    /// The ParticleSystem class manages an emitter and a set of particles.
    /// the system uses the emitter to generate new particles.  Each time
    /// update is called, the system updates the properties of its particles.
    /// </summary>
    public class ParticleSystem : DrawableGameComponent
    {
        protected Vector2 position;
        protected ArrayList particles = null;
        protected ArrayList modifiers = null;
        protected Emitter emitter = null;
        protected Texture2D texture = null;
        protected float textureScale = 0.5f;
        protected bool collideWithScene = false;
        protected SpriteBatch spriteBatch;

        public bool IsOn = false;

        #region Getters and Setters
        public Vector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                emitter.Position = value;
            }
        }
        public float PositionX
        {
            get { return position.X; }
            set { position.X = value; }
        }
        public float PositionY
        {
            get { return position.Y; }
            set { position.Y = value; }
        }
        public Emitter Emitter
        {
            get { return emitter; }
            set { emitter = value; }
        }
        public Texture2D Texture
        {
            get { return texture; }
            set { texture = value; }
        }
        public int ParticleCount
        {
            get { return particles.Count; }
        }
        public float TextureScale
        {
            get { return textureScale; }
            set { textureScale = value; }
        }
        #endregion

        public ParticleSystem(Game game) : base(game)
        {
            particles = new ArrayList();
            modifiers = new ArrayList();
            emitter = new RandomEmitter(position);
        }

        /// <summary>
        /// Load textures
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
            spriteBatch = new SpriteBatch(Game.GraphicsDevice);
            texture = Game.Content.Load<Texture2D>(@"W_A_D\Textures\particles\white_star");
        }

        /// <summary>
        /// unload
        /// </summary>
        protected override void UnloadContent()
        {
            Game.Content.Unload();
            base.UnloadContent();
        }

        /// <summary>
        /// Update the system simulation data by removing old particles
        /// and generating new ones using the associated emitter
        /// </summary>
        /// <param name="deltaTime"></param>
        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Update existing particles
            ArrayList removeList = new ArrayList();
            foreach (Particle p in particles)
            {
                if (!p.Update(deltaTime))
                {
                    // mark particle for removal
                    removeList.Add(p);
                }
            }
            // remove dead particles from list
            foreach (Particle p in removeList)
            {
                particles.Remove(p);
            }

            if (emitter == null) return;

            // Emit new particles (using anonymous delegate method for add callback)
            if (IsOn) emitter.Emit(deltaTime, delegate(Particle p) { particles.Add(p); });

            // Modify the particles
            bool allOff = true;
            foreach (Modifier m in modifiers)
            {
                if (m.Enabled) allOff = false;
            }
            if (!allOff)
            {
                foreach (Particle p in particles)
                {
                    foreach (Modifier m in modifiers)
                    {
                        if (m.Enabled)
                        {
                            m.Modify(deltaTime, p);
                        }
                    }
                }
            }

            // TODO handle if collide with scene
            base.Update(gameTime);
        }

        /// <summary>
        /// Draw the system
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            if (texture == null) return;
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            foreach (Particle p in particles)
            {
                spriteBatch.Draw(texture, p.Position, new Rectangle(0, 0, texture.Width, texture.Height), p.Color,
                    p.Angle, new Vector2(texture.Width / 2, texture.Height / 2), p.Size * textureScale, SpriteEffects.None, 0.0f);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }

        #region Debug Methods
        [Conditional("Debug")]
        public void DebugDump()
        {
            Debug.WriteLine("---------- Particle System ----------");
            Debug.WriteLine("Position........." + position.ToString());
            Debug.WriteLine("Emitter Type ...." + emitter.GetType().ToString());
            Debug.WriteLine("Collisions On ..." + collideWithScene.ToString());
            Debug.WriteLine("Particle Count..." + particles.Count.ToString());
            Debug.IndentLevel = 3;
            foreach (Particle p in particles)
            {
                p.DebugDump();
            }
            Debug.IndentLevel = 0;
        }
        #endregion
    }
}
