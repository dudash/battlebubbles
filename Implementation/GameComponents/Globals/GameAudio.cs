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
// File Created: 03 March 2008, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace HBBB.GameComponents.Globals
{
    /// <summary>
    /// This is a staic class acessible by all game classes.  It
    /// provides a common interface to play music and sounds effects.
    /// </summary>
    static class GameAudio
    {
        private static AudioEngine audioEngine;
        private static WaveBank waveBank;
        private static SoundBank soundBank;
        private static AudioCategory defaultCategory;

        public static Song Music_Gorillas;
        public static Song Music_RocketFunkster;
        public static string currentMusic;

        /// <summary>
        /// Need to keep track of these so we can stop them
        /// </summary>
        private static SortedList<string, Cue> musicCues;
        /// <summary>
        /// Flag to indicate audio has been muted
        /// </summary>
        private static bool mutedFlag = false;
        public static bool Muted
        {
            get { return mutedFlag; }
            set { mutedFlag = value; }
        }

        /// <summary>
        /// Initialize the game audio
        /// </summary>
        public static void Initialize(ContentManager content)
        {
            audioEngine = new AudioEngine(@"W_A_D\Audio\battlebubbles.xgs");
            waveBank = new WaveBank(audioEngine, @"W_A_D\Audio\battlebubbles.xwb");
            soundBank = new SoundBank(audioEngine, @"W_A_D\Audio\battlebubbles.xsb");
            musicCues = new SortedList<string, Cue>();
            defaultCategory = audioEngine.GetCategory("Default");

            Music_Gorillas = content.Load<Song>(@"W_A_D\Audio\JelloKnee - Gorillas");
            Music_RocketFunkster = content.Load<Song>(@"W_A_D\Audio\JelloKnee - RocketFunkster");
            currentMusic = "Gorillas";

            MediaPlayer.IsRepeating = false;
            MediaPlayer.IsShuffled = false;
        }

        /// <summary>
        ///  Play a cue
        /// </summary>
        /// <param name="cueName"></param>
        public static void PlayCue(string cueName)
        {
            if (mutedFlag) return;
            if (cueName == null) return;
            soundBank.PlayCue(cueName);
        }

        /// <summary>
        /// Stop a looping cue
        /// </summary>
        /// <param name="cueName"></param>
        public static void StopCue(string cueName)
        {
            if (cueName == null) return;
            Cue cue = soundBank.GetCue(cueName);
            cue.Stop(AudioStopOptions.Immediate);
        }

        /// <summary>
        /// Lower the volume of the argument cue
        /// </summary>
        /// <param name="cueName"></param>
        public static void SetMusicVolume(float volume)
        {
            MediaPlayer.Volume = volume;
        }

        /// <summary>
        /// Stop playing the argument que
        /// </summary>
        /// <param name="cueName"></param>
        public static void StopMusic(string name)
        {
            MediaPlayer.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public static void PlayMusic(string name)
        {
            MediaPlayer.Stop();
            if (name.Contains("Gorillas"))
            {
                MediaPlayer.Play(Music_Gorillas);
                currentMusic = "Gorillas";
            }
            else if (name.Contains("RocketFunkster"))
            {
                MediaPlayer.Play(Music_RocketFunkster);
                currentMusic = "RocketFunkster";
            }
        }

        /// <summary>
        /// Called from HBBBgame Update to make sure music is playing
        /// </summary>
        public static void UpdateMusic()
        {
            if (MediaPlayer.State == MediaState.Stopped)
            {
                if (currentMusic == "Gorillas")
                {
                    currentMusic = "RocketFunkster";  // next track inlist
                    PlayMusic(currentMusic);
                }
                else
                {
                    currentMusic = "Gorillas";
                    PlayMusic(currentMusic);
                }
            }
        }
    }
}
