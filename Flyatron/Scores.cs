using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;


namespace Flyatron
{
	class Scoreboard
	{
		int score = 0;
		string[] scores;
		Stopwatch timer;
		string path;

		public Scoreboard(string path)
		{
			scores = Import(path);
			timer = new Stopwatch();
		}

		private void Increment()
		{
			timer.Restart();

			if (timer.ElapsedMilliseconds >= 1000)
				score += 10;
		}
		
		private string[] Import(string inputPath)
		{
			path = inputPath;

			if (!File.Exists(path))
			{
				File.Create(path);
				Dummy(path);
			}

			return File.ReadAllLines(path);
		}

		private void Export(string path)
		{
			using (StreamWriter file = new StreamWriter(path))
				for (int i = 0; i < scores.Length; i++)
					file.WriteLine(scores[i]);
		}

		private void Collate()
		{
			// Collate combines the current score along with the top 10 scores.
			// It then sorts this list, and overwrites the saved scores with it.
			List<int> temp = new List<int>();

			for (int i = 0; i < scores.Length; i++)
				temp.Add(Convert.ToInt32(scores[i]));

			temp.Add(score);

			temp.Sort();

			using (StreamWriter file = new StreamWriter(path, true))
				for (int i = 0; i < 10; i++)
					file.WriteLine(temp[i]);
		}

		private void Dummy(string path)
		{
			using (StreamWriter file = new StreamWriter(path, true))
				for (int i = 999999; i > 0; i -= 10000)
					file.WriteLine(i);
		}

		public string Current()
		{
			return Convert.ToString(score);
		}
	}
}
