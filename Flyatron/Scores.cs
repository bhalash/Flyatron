﻿using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace Flyatron
{
	class Scoreboard
	{
		public static int SCORE;
		List<int> scores;
		Stopwatch timer;
		string path;

		public Scoreboard()
		{
			scores = Import("Content\\scores.txt");
			timer  = new Stopwatch();
			timer.Start();

			SCORE = 0;
		}

		public void Update()
		{
			// The basic score accumulates every 0.08 of a second (ticks 12.4 times per).
			// I want to balance obscene numbers being thrown around with the gratification of 
			// seeing your score climbing upward unto infinity. 
			if (timer.ElapsedMilliseconds >= 80)
			{
				SCORE += 1;
				timer.Restart();
			}
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
				using (StreamWriter file = new StreamWriter(path, true))
					for (int i = 1000; i > 0; i -= 100)
						file.WriteLine(i);

			tempA = File.ReadAllLines(path);
			tempB = new List<int>();

			for (int i = 0; i < tempA.Length; i++)
				tempB.Add(Convert.ToInt32(tempA[i]));

			return tempB;
		}

		public void Reset()
		{
			SCORE = 0;
		}

		public void Collate()
		{
			scores.Add(SCORE);
			scores.Sort();
			scores.RemoveAt(0);
			scores.Reverse();
		}

		public void Report(SpriteFont font, SpriteBatch batch, int x, int y, Color color)
		{
			Export(path);
			for (int i = 0; i < scores.Count; i++)
			{
				batch.DrawString(font, Convert.ToString(i) + ": " + scores[i], new Vector2(x, y), color);
				y += 25;
			}
		}

		public string Current()
		{
			return Convert.ToString(SCORE);
		}

		public void Export(string path)
		{
			// Export the scores as-is to the text file, immediately. 
			using (StreamWriter file = new StreamWriter(path))
				for (int i = 0; i < scores.Count; i++)
					file.WriteLine(scores[i]);
		}

		public void Bump(int amount)
		{
			SCORE += amount;
		}
	}
}
