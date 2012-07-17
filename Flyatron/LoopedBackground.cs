using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Flyatron
{
	class BackgroundLayer
	{
		Texture2D texture;
		Vector2 vector1;
		Vector2 vector2;

		// 1a. Input a texture and rectangle.
		public BackgroundLayer(Texture2D inputTexture, Vector2 inputVector)
		{
			texture = inputTexture;
			vector1 = inputVector;
			vector2 = inputVector;
			// 1b. Shift vector2 to the right of vector1.
			vector2.X += texture.Width;
		}

		// 2. Draw the textures.
		public void DrawLoop(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, vector1, Color.White);
			spriteBatch.Draw(texture, vector2, Color.White);
		}

		// 3. Update it as you go.
		public void Update(int x, int y)
		{
			// 4a. Horizontal loop check.
			if (vector1.X + texture.Width <= 0)
				vector1.X = vector2.X + texture.Width;
			if (vector2.X + texture.Width <= 0)
				vector2.X = vector1.X + texture.Width;

			// 5. Move the texture. There is no bounds checking here,
			// because class should *only* be concerned with seamlessly 
			// tiling a texture.
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
