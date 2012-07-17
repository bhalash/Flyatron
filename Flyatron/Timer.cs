using System;
using Microsoft.Xna.Framework;
using Flyatron;

namespace Flyatron
{
	class mTimer
	{
		// Kull wahad!
		// For some reason I just could not wrap my head around timers, so
		// I picked the simplest possible solution using TimeSpan. It works perfectly for 
		// my puroses. 

		// The timer and the duration.
		TimeSpan timer;

		public mTimer()
		{
			timer = TimeSpan.Zero;
		}

		public void Reset()
		{
			// Push the reset. 
			timer = TimeSpan.Zero;
		}

		public bool Elapsed(double inputTime)
		{
			// Checks to see if n time has elapsed since the last reset.
			if (timer >= TimeSpan.FromSeconds(inputTime))
				return true;

			return false;
		}

		public string ReportSeconds()
		{
			// Return seconds as a string.
			return Convert.ToString(timer.Seconds);
		}

		public string ReportMilliseconds()
		{
			// Return milliseconds as as tring.
			return Convert.ToString(timer.Milliseconds);
		}

		public void Update(TimeSpan inputGameTime)
		{
			timer += inputGameTime;
		}
	}
}
