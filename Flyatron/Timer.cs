/*
 * I am not wholly convinced of the value of wrappering most of the functionality of the Stopwatch class, 
 * save that it allows me to consolidate a decent amount of code, and expand upon it upon demand.
 */

using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Flyatron;

namespace Flyatron
{
	class Flytimer
	{
		Stopwatch timer;
		int limit;

		public Flytimer(int inputLimit)
		{
			limit = inputLimit;
			timer = new Stopwatch();
		}

		public void Update()
		{
		}

		public void ChangeLimit(int inputLimit)
		{
			limit = inputLimit * 1000;
		}

		public void Restart()
		{
			timer.Restart();
		}

		public bool Running()
		{
			if (timer.IsRunning)
				return true;

			return false;
		}

		public void Start()
		{
			timer.Start();
		}

		public void Stop()
		{
			timer.Stop();
		}

		public bool Expired()
		{
			if (timer.ElapsedMilliseconds > limit)
				return true;

			return false;
		}

		public string Report()
		{
			string secs = Convert.ToString(timer.Elapsed.Seconds);
			string mili = Convert.ToString(timer.Elapsed.Milliseconds);
			string coln = ":";
			string comb = secs + coln + mili; 
			return comb;	
		}
	}
}

