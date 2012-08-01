using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Flyatron
{
	class Gun
	{
		int xSize;
		int ySize;

		float xOffset;
		float yOffset;

		// Gun/bullet.
		Texture2D[] texture;
		Vector2 vector;
		Rectangle[] rectangle;
		Color color;
		float rotation;
		Vector2 origin;
		SpriteEffects effects;

		Vector2 mouse;

		List<Bullet> bullets;

		public Gun(Texture2D[] inputTexture)
		{
			xSize = 35;
			ySize = 18;

			xOffset = 0;
			yOffset = 25;

			texture = inputTexture;
			vector = new Vector2(150, 150);
			bullets = new List<Bullet>();

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

			if (Helper.LeftClick())
				bullets.Add(new Bullet(texture, Rectangle()));

			if (bullets.Count > 0)
				for (int i = 0; i < bullets.Count; i++)
				{
					bullets[i].Update();

					if (bullets[i].Expired())
						bullets.RemoveAt(i);
				}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture[0], vector, rectangle[0], color, rotation, origin, 1, effects, 0);
		}

		private void Animate()
		{
			mouse.X = Game.CURR_MOUSE.X;
			mouse.Y = Game.CURR_MOUSE.Y;

			Vector2 leftFacing = new Vector2(vector.X - Game.CURR_MOUSE.X, vector.Y - mouse.Y);
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
			return new Rectangle((int)vector.X - xSize / 2, (int)vector.Y - ySize / 2, xSize, ySize);
		}
	}
}
