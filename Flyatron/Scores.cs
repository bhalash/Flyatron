using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace Flyatron
{
	class Scoreboard
	{
		public static int score = 0;
		List<int> scores;
		Stopwatch timer;
		string path;

		public Scoreboard(string path)
		{
			scores = Import(path);
			timer = new Stopwatch();
			timer.Start();
		}
	
		private List<int> Import(string inputPath)
		{
			// The top 10 scores are recorded in a separate text file, so scores.Count persist
			// between games. If the file does not exist, a new file is created with a 
			// series of dummy scores.
			path = inputPath;
			string[] tempA;
			List<int> tempB;

			if (!File.Exists(path))
			{
				File.Create(path);
				Dummy(path);
			}

			tempA = File.ReadAllLines(path);
			tempB = new List<int>();

			for (int i = 0; i < tempA.Length; i++)
				tempB.Add(Convert.ToInt32(tempA[i]));

			return tempB;
		}

		public void Increment()
		{
			// The basic score accumulates every 0.08 of a second (ticks 12.4 times per).
			// I want to balance obscene numbers being thrown around with the gratification of 
			// seeing your score climbing upward unto infinity. 
			if (timer.ElapsedMilliseconds >= 80)
			{
				score += 1;
				timer.Restart();
			}
		}

		public void Reset()
		{
			score = 0;
		}

		private void Dummy(string path)
		{
			using (StreamWriter file = new StreamWriter(path, true))
				for (int i = 999; i > 0; i -= 111)
					file.WriteLine(i);
		}

		public void Collate()
		{
			// Collate combines the current score along with the top 10 scores.
			// It then sorts this list, and overwrites the saved scores with the
			// new top ten scores.
			scores.Add(score);
			scores.Sort();
			scores.RemoveAt(0);
			scores.Reverse();
		}

		public void Report(SpriteFont font, SpriteBatch batch, int x, int y, Color color)
		{
			// Report() is used on the scores screen.
			// Current() is drawn during gameplay.
			Collate();
			Export(path);
			for (int i = 0; i < scores.Count; i++)
			{
				batch.DrawString(font, Convert.ToString(i) + ": " + scores[i], new Vector2(x, y), color);
				y += 25;
			}
		}

		public string Current()
		{
			return Convert.ToString(score);
		}

		public void Export(string path)
		{
			// Export the scores as-is to the text file, immediately. 
			// I would prefer if you ran Collate() beforehand.
			using (StreamWriter file = new StreamWriter(path))
				for (int i = 0; i < scores.Count; i++)
					file.WriteLine(scores[i]);
		}

		public static void Bump(int amount)
		{
			score += amount;
		}
	}
}
