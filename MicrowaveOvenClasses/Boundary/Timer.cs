using System;
using MicrowaveOvenClasses.Interfaces;

namespace MicrowaveOvenClasses.Boundary
{
    public class Timer : ITimer
    {
        public int TimeRemaining { get; private set; }
        public int TimerInterval { get; set; } = 1000;

        public event EventHandler Expired;
        public event EventHandler TimerTick;

        private System.Timers.Timer timer;

        public Timer()
        {
            timer = new System.Timers.Timer();
            // Bind OnTimerEvent with an object of this, and set up the event
            timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimerEvent);
            timer.Interval = TimerInterval; // 1 second intervals
            timer.AutoReset = true;  // Repeatable timer
        }

        /// <summary>
        /// Starts the timer which ticks with the given <see cref="TimerInterval"/>
        /// </summary>
        /// <param name="time">The time before expiration, specified in seconds</param>
        public void Start(int time)
        {
            TimeRemaining = time*TimerInterval;
            timer.Enabled = true;
        }

        public void Stop()
        {
            timer.Enabled = false;
        }

        private void Expire()
        {
            timer.Enabled = false;
            Expired?.Invoke(this, System.EventArgs.Empty);
        }

        private void OnTimerEvent(object sender, System.Timers.ElapsedEventArgs args)
        {
            // One tick has passed
            // Do what I should
            TimeRemaining -= 1000;
            TimerTick?.Invoke(this, EventArgs.Empty);

            if (TimeRemaining <= 0)
            {
                Expire();
            }
        }

    }
}