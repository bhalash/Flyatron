using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Flyatron
{
	class Gun
	{
		// Gun/bullet.
		Texture2D[] texture;
		Vector2 vector;
		Vector2 vector2;
		Rectangle[] rectangle;
		Color color;
		float rotation;
		Vector2 origin;
		SpriteEffects effects;

		Vector2 mouse;

		int xOffset = 0;
		int yOffset = 25;

		public Gun(Texture2D[] inputTexture)
		{
			texture = inputTexture;

			vector = new Vector2(150, 150);
			vector2 = new Vector2(vector.X + 150, 150);

			rectangle = new Rectangle[]
			{
				new Rectangle(0, 0, 35, 18),
				new Rectangle(0, 0, 55, 11),
			};

			color = Color.White;
			rotation = 0;
			origin = new Vector2(17, 9);
			effects = SpriteEffects.None;
		}

		public void Update(Vector2 reference)
		{
			vector.X = reference.X + xOffset;
			vector.Y = reference.Y + yOffset;
			Animate();
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture[0], vector, rectangle[0], color, rotation, origin, 1, effects, 0);
			spriteBatch.Draw(texture[1], vector2, rectangle[1], color, rotation, origin, 1, effects, 0);
		}

		private void Animate()
		{
			mouse.X = Game.currentMouseState.X;
			mouse.Y = Game.currentMouseState.Y;

			Vector2 leftFacing = new Vector2(vector.X - Game.currentMouseState.X, vector.Y - mouse.Y);
			Vector2 rightFacing = new Vector2(mouse.X - vector.X, mouse.Y - vector.Y);

			float leftAngle = (float)(Math.Atan2(leftFacing.Y, leftFacing.X));
			float rightAngle = (float)(Math.Atan2(rightFacing.Y, rightFacing.X));

			if (mouse.X < vector.X)
			{
				rotation = leftAngle;
				rectangle[0] = new Rectangle(39, 0, 35, 18);
			}
			if (mouse.X > vector.X)
			{
				rotation = rightAngle;
				rectangle[0] = new Rectangle(0, 0, 35, 18);
			}
		}

		public Rectangle Rectangle()
		{
			return new Rectangle((int)vector.X, (int)vector.Y, texture[0].Width, texture[0].Height);
		}
	}
}
