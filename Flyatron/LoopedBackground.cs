using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Flyatron
{
	class BackgroundLayer
	{
		// Happy sunshine infinitely left-scrolling backgrounds.
		// TODO: Expand on this to manage multiple parallax layers.
		Texture2D texture;
		Vector2 vector1;
		Vector2 vector2;

		public BackgroundLayer(Texture2D inputTexture, Vector2 inputVector)
		{
			texture = inputTexture;
			vector1 = inputVector;
			vector2 = inputVector;
			vector2.X += texture.Width;
		}

		public void DrawLoop(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, vector1, Color.White);
			spriteBatch.Draw(texture, vector2, Color.White);
		}

		public void Update(int x, int y)
		{
			if (vector1.X + texture.Width <= 0)
				vector1.X = vector2.X + texture.Width;
			if (vector2.X + texture.Width <= 0)
				vector2.X = vector1.X + texture.Width;

			if (x >= 0)
			{
				vector1.X += x;
				vector2.X += x;
			}
			if (x < 0)
			{
				vector1.X -= x - (x * 2);
				vector2.X -= x - (x * 2);
			}

			if (y >= 0)
			{
				vector1.Y += y;
				vector2.Y += y;
			}
			if (y < 0)
			{
				vector1.Y -= y - (y * 2);
				vector2.Y -= y - (y * 2);
			}
		}
	}
}
