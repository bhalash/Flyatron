using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Flyatron
{
	class Flymouse
	{
		Texture2D texture;
		Vector2 vector;
		Rectangle rectangle;
		Color color;
		Vector2 origin;
		SpriteEffects effects;

		public Flymouse(Texture2D inputTexture)
		{
			texture = inputTexture;
			vector = new Vector2(150, 150);
			origin = new Vector2(0, 0);
			rectangle = new Rectangle(0, 0, 34, 34);
			effects = SpriteEffects.None;
			color = Color.White;
		}

		public void Update()
		{
			vector.X = Mouse.GetState().X - rectangle.Width / 2;
			vector.Y = Mouse.GetState().Y - rectangle.Height / 2;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, vector, rectangle, color, 0, origin, 1, effects, 0);
		}

		public Rectangle Rectangle()
		{
			return new Rectangle(
				(int)vector.X + rectangle.Width / 4,
				(int)vector.Y + rectangle.Height / 4,
				rectangle.Width / 2,
				rectangle.Height / 2
			);
		}
	}
}
