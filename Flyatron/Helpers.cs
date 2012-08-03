/*
 * Miscellaneous helper functions.
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Flyatron
{
	class Helper
	{
		public static Random RANDOM = new Random();

		public Helper()
		{
		}

		public static int Rng(int a)
		{
			return RANDOM.Next(a);
		}

		public static int Rng2(int a, int b)
		{
			return RANDOM.Next(a, b);
		}

		public static bool SquareCollision(Rectangle a, Rectangle b)
		{
			// Generic collisions.
			if (a.Intersects(b))
				return true;

			return false;	
		}

		public static bool CircleCollision(Rectangle a, Rectangle b, float multiplier1 = 1F, float multiplier2 = 1F)
		{
			// Circular generic collisions.
			if (Math.Sqrt((b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y)) < ((a.Width * multiplier1) / 2 + (b.Width * multiplier2) / 2))
				return true;

			return false;
		}

		public static bool LeftClick()
		{
			// Left mouse click.
			if ((Game.MOUSE.LeftButton == ButtonState.Released) && (Game.PREV_MOUSE.LeftButton == ButtonState.Pressed))
				return true;

			else return false;
		}

		public static bool RightClick()
		{
			// Right mouse click.
			if ((Game.MOUSE.RightButton == ButtonState.Released) && (Game.PREV_MOUSE.RightButton == ButtonState.Pressed))
				return true;

			else return false;
		}

		public static bool Keypress(Keys inputKey)
		{
			if (Game.KEYBOARD.IsKeyUp(inputKey) && (Game.PREV_KEYBOARD.IsKeyDown(inputKey)))
				return true;

			return false;
		}
	}
}
