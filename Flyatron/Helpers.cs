using System;
using Microsoft.Xna.Framework;

namespace Flyatron
{
	class Helper
	{
		public static Random RANDOM = new Random();

		public Helper()
		{
		}

		public static int Rng(int a, int b)
		{
			// RNG.
			return RANDOM.Next(a, b);
		}

		public static bool Rectangle(Rectangle a, Rectangle b)
		{
			// Rectangular collisions.
			if (a.Intersects(b))
				return true;

			return false;
		}

		public static bool Circle(Rectangle a, Rectangle b)
		{
			// Circular collisions.
			if (Math.Sqrt((b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y)) < (a.Width / 2 + b.Width / 2))
				return true;

			return false;
		}
	}
}
