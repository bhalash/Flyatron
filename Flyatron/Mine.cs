using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using Flyatron;

namespace Flyatron
{
	class Mine
	{
		Stopwatch rotation;
		Stopwatch spawn;

		float angle1 = 0;
		float angle2 = 0;

		Texture2D[] textures;
		Vector2 vector = new Vector2(300, 300);
		Rectangle rectangle = new Rectangle(0, 0, 40, 40);
		Vector2 offset = new Vector2(20, 20);

		public Mine(Texture2D[] inputTextures)
		{
			textures = inputTextures;

			rotation = new Stopwatch();
			rotation.Start();

			spawn = new Stopwatch();
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(textures[0], vector, rectangle, Color.White, angle1, offset, 1, SpriteEffects.None, 0);
			spriteBatch.Draw(textures[1], vector, rectangle, Color.White, angle2, offset, 1, SpriteEffects.None, 0);
		}

		public void Update(KeyboardState keyboardState)
		{
			UpdateAnimation();
			Move(keyboardState);
		}

		private void Move(KeyboardState keyboardState)
		{
			// Mofidul, here is one example: 
			// I want the mine to move in lockstep with the player, but in the opposite direction, to give the illusion of
			// a bigger arena than actually exists. 
		}

		private void UpdateAnimation()
		{
			if (rotation.ElapsedMilliseconds >= 20F)
			{
				angle1 += 0.3F;
				angle2 -= 1;
				rotation.Restart();
			}

			if (angle1 >= 360)
				angle1 = 0;
			if (angle2 >= 360)
				angle2 = 0;
		}


	}
}
