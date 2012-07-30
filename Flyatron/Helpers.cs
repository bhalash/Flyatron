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

		public static bool CircleCollision(Rectangle a, Rectangle b)
		{
			// Circular generic collisions.
			if (Math.Sqrt((b.X - a.X) * (b.X - a.X) + (b.Y - a.Y) * (b.Y - a.Y)) < (a.Width / 2 + b.Width / 2))
				return true;

			return false;
		}

		public static bool LeftClick()
		{
			// Left mouse click.
			if ((Game.currentMouseState.LeftButton == ButtonState.Released) && (Game.lastMouseState.LeftButton == ButtonState.Pressed))
				return true;

			else return false;
		}

		public static bool RightClick()
		{
			// Right mouse click.
			if ((Game.currentMouseState.RightButton == ButtonState.Released) && (Game.lastMouseState.RightButton == ButtonState.Pressed))
				return true;

			else return false;
		}

		public static bool Keypress(Keys inputKey)
		{
			if (Game.currentKeyboardState.IsKeyUp(inputKey) && (Game.lastKeyboardState.IsKeyDown(inputKey)))
				return true;

			return false;
		}
	}
}
