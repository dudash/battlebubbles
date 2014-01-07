#region Copyright
//-----------------------------------------------------------------------------
// Copyright (C)2007 No Hands Entertainment, All rights reserved.
//-----------------------------------------------------------------------------
// File Created: 03 March 2008, Jason Dudash
//-----------------------------------------------------------------------------
#endregion
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;

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

        /// <summary>
        /// Need to keep track of these so we can stop them
        /// </summary>
        private static Dictionary<string, Cue> musicCues;

        /// <summary>
        /// Initialize the game audio
        /// </summary>
        public static void Initialize()
        {
            audioEngine = new AudioEngine(@"W_A_D\Audio\battlebubbles.xgs");
            waveBank = new WaveBank(audioEngine, @"W_A_D\Audio\battlebubbles.xwb");
            soundBank = new SoundBank(audioEngine, @"W_A_D\Audio\battlebubbles.xsb");
            musicCues = new Dictionary<string, Cue>();
        }

        /// <summary>
        ///  Play a cue
        /// </summary>
        /// <param name="cueName"></param>
        public static void PlayCue(string cueName)
        {
            soundBank.PlayCue(cueName);
        }

        /// <summary>
        /// Stop a looping cue
        /// </summary>
        /// <param name="cueName"></param>
        public static void StopCue(string cueName)
        {
            Cue cue = soundBank.GetCue(cueName);
            cue.Stop(AudioStopOptions.Immediate);
        }

        /// <summary>
        /// Play cue and keep track of it
        /// </summary>
        /// <param name="cueName"></param>
        public static void PlayMusic(string cueName)
        {
            Cue cue = soundBank.GetCue(cueName);
            Cue oldCue = null;
            musicCues.TryGetValue(cueName, out oldCue);
            if (oldCue != null) oldCue.Stop(AudioStopOptions.Immediate);
            musicCues.Add(cueName, cue);
            cue.Play();
        }

        public static void StopMusic(string cueName)
        {
            Cue cue = null;
            musicCues.TryGetValue(cueName, out cue);
            if (cue != null) cue.Stop(AudioStopOptions.Immediate);
        }
    }
}
