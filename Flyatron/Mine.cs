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
		Texture2D[] textures;
		Stopwatch rotation;

		float angle1 = 0;
		float angle2 = 0;

		Vector2 vector = new Vector2(300, 300);
		Rectangle rectangle = new Rectangle(0, 0, 40, 40);
		Vector2 offset = new Vector2(20, 20);

		public Mine(Texture2D[] inputTextures)
		{
			textures = inputTextures;

			rotation = new Stopwatch();
			rotation.Start();
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(textures[0], vector, rectangle, Color.White, angle1, offset, 1, SpriteEffects.None, 0);
			spriteBatch.Draw(textures[1], vector, rectangle, Color.White, angle2, offset, 1, SpriteEffects.None, 0);
		}

		public void Update()
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
