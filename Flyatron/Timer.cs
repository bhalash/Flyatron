using System;
using Microsoft.Xna.Framework;
using Flyatron;

namespace Flyatron
{
	class mTimer
	{
		// Kull wahad!
		// Virtually the simplest timer I could conceive: 
		// It counts up from 0 continually. Reset can be called as necessary.

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
			if (timer >= TimeSpan.FromSeconds(inputTime))
				return true;

			return false;
		}

		public string ReportSeconds()
		{
			return Convert.ToString(timer.Seconds);
		}

		public string ReportMilliseconds()
		{
			return Convert.ToString(timer.Milliseconds);
		}

		public void Update(TimeSpan inputGameTime)
		{
			timer += inputGameTime;
		}
	}
}

