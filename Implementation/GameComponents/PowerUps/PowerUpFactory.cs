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
using Microsoft.Xna.Framework.Graphics;

namespace HBBB.GameComponents.PowerUps
{
    /// <summary>
    /// A factory that can create powerups based on the type desired
    /// </summary>
    static class PowerUpFactory
    {
        static int instanceId = 0;

        static Texture2D slowSpeedTexture;
        static Texture2D popUnlockedTexture;
        static Texture2D lockAllTexture;
        static Texture2D fastTransitionTexture;
        static Texture2D slowTransitionTexture;
        static Texture2D emissionFrenzyTexture;
        static Texture2D fastSpeedTexture;
        static Texture2D blockStealTexture;

        /// <summary>
        /// A set of the powerups that can be created
        /// </summary>
        public enum PowerUpId
        {
            POWERUP_SLOW_SPEED        = 0
            , POWERUP_POP_UNLOCKED    = 1
            , POWERUP_LOCK_ALL        = 2
            , POWERUP_FAST_TRANSITION = 3
            , POWERUP_SLOW_TRANSITION = 4
            , EMISSION_FRENZY         = 5
            , POWERUP_FAST_SPEED      = 6
            , POWERUP_BLOCK_STEAL     = 7
            , LAST_POWER_UP
        };

        public const string POWERUP_STR_SLOW_SPEED = "POWERUP_SLOW_SPEED";
        public const string POWERUP_STR_POP_UNLOCKED = "POWERUP_POP_UNLOCKED";
        public const string POWERUP_STR_LOCK_ALL = "POWERUP_LOCK_ALL";
        public const string POWERUP_STR_FAST_TRANSITION = "POWERUP_FAST_TRANSITION";
        public const string POWERUP_STR_SLOW_TRANSITION = "POWERUP_SLOW_TRANSITION";
        public const string POWERUP_STR_EMISSION_FRENZY = "POWERUP_EMISSION_FRENZY";
        public const string POWERUP_STR_FAST_SPEED = "POWERUP_FAST_SPEED";
        public const string POWERUP_STR_BLOCK_STEAL = "POWERUP_BLOCK_STEAL";

        /// <summary>
        /// Get the texture for the argument power up id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Texture2D GetTexture(PowerUpId id)
        {
            switch (id)
            {
                case PowerUpId.POWERUP_SLOW_SPEED:
                    return slowSpeedTexture;
                case PowerUpId.POWERUP_POP_UNLOCKED:
                    return popUnlockedTexture;
                case PowerUpId.POWERUP_LOCK_ALL:
                    return lockAllTexture;
                case PowerUpId.POWERUP_FAST_TRANSITION:
                    return fastTransitionTexture;
                case PowerUpId.POWERUP_SLOW_TRANSITION:
                    return slowTransitionTexture;
                case PowerUpId.EMISSION_FRENZY:
                    return emissionFrenzyTexture;
                case PowerUpId.POWERUP_FAST_SPEED:
                    return fastSpeedTexture;
                case PowerUpId.POWERUP_BLOCK_STEAL:
                    return blockStealTexture;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Set the texture for the argument power up
        /// </summary>
        /// <param name="id"></param>
        public static void SetTexture(PowerUpId id, Texture2D texture)
        {
            switch (id)
            {
                case PowerUpId.POWERUP_SLOW_SPEED:
                    slowSpeedTexture = texture;
                    break;
                case PowerUpId.POWERUP_POP_UNLOCKED:
                    popUnlockedTexture = texture;
                    break;
                case PowerUpId.POWERUP_LOCK_ALL:
                    lockAllTexture = texture;
                    break;
                case PowerUpId.POWERUP_FAST_TRANSITION:
                    fastTransitionTexture = texture;
                    break;
                case PowerUpId.POWERUP_SLOW_TRANSITION:
                    slowTransitionTexture = texture;
                    break;
                case PowerUpId.EMISSION_FRENZY:
                    emissionFrenzyTexture = texture;
                    break;
                case PowerUpId.POWERUP_FAST_SPEED:
                    fastSpeedTexture = texture;
                    break;
                case PowerUpId.POWERUP_BLOCK_STEAL:
                    blockStealTexture = texture;
                    break;
            }
        }

        /// <summary>
        /// create the power up identified
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static PowerUp CreatePowerUp(PowerUpId id, Vector2 position)
        {
            switch (id)
            {
                case PowerUpId.POWERUP_SLOW_SPEED:
                    instanceId++;
                    return new SlowSpeedPowerUp(instanceId, position);
                case PowerUpId.POWERUP_POP_UNLOCKED:
                    instanceId++;
                    return new PopUnlockedPowerUp(instanceId, position);
                case PowerUpId.POWERUP_LOCK_ALL:
                    instanceId++;
                    return new LockAllPowerUp(instanceId, position);
                case PowerUpId.POWERUP_FAST_TRANSITION:
                    instanceId++;
                    return new FastTransitionPowerUp(instanceId, position);
                case PowerUpId.POWERUP_SLOW_TRANSITION:
                    instanceId++;
                    return new SlowTransitionPowerUp(instanceId, position);
                case PowerUpId.EMISSION_FRENZY:
                    instanceId++;
                    return new EmissionFrenzyPowerUp(instanceId, position);
                case PowerUpId.POWERUP_FAST_SPEED:
                    instanceId++;
                    return new FastSpeedPowerUp(instanceId, position);
                case PowerUpId.POWERUP_BLOCK_STEAL:
                    instanceId++;
                    return new BlockStealPowerUp(instanceId, position);
                default:
                    throw new Exception("Unknown or unimplemented powerup id");
            }
        }

        /// <summary>
        /// Create powerup based on sring
        /// </summary>
        /// <param name="id"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static PowerUp CreatePowerUp(string id, Vector2 position)
        {
            switch (id)
            {
                case POWERUP_STR_SLOW_SPEED:
                    instanceId++;
                    return new SlowSpeedPowerUp(instanceId, position);
                case POWERUP_STR_POP_UNLOCKED:
                    instanceId++;
                    return new PopUnlockedPowerUp(instanceId, position);
                case POWERUP_STR_LOCK_ALL:
                    instanceId++;
                    return new LockAllPowerUp(instanceId, position);
                case POWERUP_STR_FAST_TRANSITION:
                    instanceId++;
                    return new FastTransitionPowerUp(instanceId, position);
                case POWERUP_STR_SLOW_TRANSITION:
                    instanceId++;
                    return new SlowTransitionPowerUp(instanceId, position);
                case POWERUP_STR_EMISSION_FRENZY:
                    instanceId++;
                    return new EmissionFrenzyPowerUp(instanceId, position);
                case POWERUP_STR_FAST_SPEED:
                    instanceId++;
                    return new FastSpeedPowerUp(instanceId, position);
                case POWERUP_STR_BLOCK_STEAL:
                    instanceId++;
                    return new BlockStealPowerUp(instanceId, position);
                default:
                    throw new Exception("Unknown or unimplemented powerup id");
            }
        }
    }
}

