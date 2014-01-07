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
// File Created: 6 December 2008, Pete Gonzalez
//-----------------------------------------------------------------------------
#endregion

using System;

namespace HBBB.GameComponents.PlayerComponents {

    //-------------------------------------------------------------------------------------------------------
    abstract class Behavior {
        protected PlayerAIGonz PlayerAI;
        
        // Used with CheckSleeping();  DoneSleepingTimemark specifies the Environment.TickCount
        // time to wake up, or 0 if disabled.
        protected int DoneSleepingTimemark;
        
        //---------------------------------------------------------------------------------------------------
        protected Behavior(PlayerAIGonz playerAI_) {
            PlayerAI = playerAI_;
            Reset();
        }

        // Call this every frame, even if the behavior is disabled
        abstract public void Process();
        
        //---------------------------------------------------------------------------------------------------
        public virtual void Reset() {
            DoneSleepingTimemark = 0;
        }
        
        //---------------------------------------------------------------------------------------------------
        protected void Log(string message) {
            PlayerAI.Log(GetType().ToString(), message);
        }
        
        //---------------------------------------------------------------------------------------------------
        // This is a general purpose "sleeping" feature that can be used by Process().
        protected bool CheckSleeping() {
            if (DoneSleepingTimemark != 0) {
                if ((Environment.TickCount - DoneSleepingTimemark) < 0) // (overflowable comparison)
                    return true;
                    
                // Wake up
                DoneSleepingTimemark = 0;
            } 
            return false;
        }
        
        //---------------------------------------------------------------------------------------------------
        protected void SleepFor(int milliseconds) {
            DoneSleepingTimemark = Environment.TickCount + milliseconds;
        }
        
        //---------------------------------------------------------------------------------------------------
        protected void SleepUntil(int timemark) {
            DoneSleepingTimemark = Environment.TickCount;
        }
    }
 
    //-------------------------------------------------------------------------------------------------------
    abstract class ContendingBehavior : Behavior {
        public enum TProgressState {
            // The behavior just started an operation, and it wouldn't be expensive to interrupt it
            Starting,

            // The behavior is in the middle of performing an action, and interrupting it now would
            // waste the time spent so far
            Progressing,
            
            // The behavior is taking too long, so maybe we should give up and try something else
            TakingTooLong
        }
        
        // A behavior is "actionable" if the relevant preconditions are satisfied.  For example, the
        // BuildBaseBehavior behavior cannot do anything unless blocks are available for building.
        protected bool Actionable;

        bool active;

        abstract public TProgressState Progress { get; }
        
        public bool Active { get { return active; } }
        
        // Returns true if the behavior is active but not actionable, i.e. it's waiting for Actionable
        // to become true.
        public bool Idle { get { return Active && !Actionable; } }

        abstract protected void OnActivated();
        abstract protected void OnDeactivated();

        //---------------------------------------------------------------------------------------------------
        protected ContendingBehavior(PlayerAIGonz playerAI_) : base(playerAI_) {
        }

        //---------------------------------------------------------------------------------------------------
        // This gives the behavior a chance to usurp another behavior, e.g. for emergencies
        public virtual void CheckUsurp() { }

        //---------------------------------------------------------------------------------------------------
        public void SetActive(bool active_) {
            if (active == active_) return;
            active = active_;
                            
            if (Active) OnActivated();
            else OnDeactivated();
        }

        //---------------------------------------------------------------------------------------------------
        public override void Reset() {
            base.Reset();
            SetActive(false);
            Actionable = false;
        }
    }
        
}
