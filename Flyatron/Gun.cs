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
		Vector2[] vector;
		Rectangle[] rectangle;
		Color color;
		float rotation;
		SpriteEffects effects;

		Vector2 mouse;

		List<Missile> missiles;

		public Gun(Texture2D[] inputTexture)
		{
			xSize = 35;
			ySize = 18;

			xOffset = 0;
			yOffset = 25;

			texture = inputTexture;

			vector = new Vector2[]
			{
				new Vector2(150, 150),
				new Vector2(17, 9)
			};

			missiles = new List<Missile>();

			rectangle = new Rectangle[]
			{
				new Rectangle(0, 0, 35, 18),
				new Rectangle(0, 0, 55, 11),
			};

			color = Color.White;
			rotation = 0;
			effects = SpriteEffects.None;
		}

		public void Update(Vector2 reference)
		{
			vector[0].X = reference.X + xOffset;
			vector[0].Y = reference.Y + yOffset;

			Animate();

			if (Helper.LeftClick())
				missiles.Add(new Missile(texture, vector[0]));

			if (missiles.Count > 0)
				for (int i = 0; i < missiles.Count; i++)
				{
					missiles[i].Update();

					if (missiles[i].Expired())
						missiles.RemoveAt(i);
				}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture[0], vector[0], rectangle[0], color, rotation, vector[1], 1, effects, 0);

			if (missiles.Count != 0)
				for (int i = 0; i < missiles.Count; i++)
					missiles[i].Draw(spriteBatch);
		}

		private void Animate()
		{
			mouse.X = Game.MOUSE.X;
			mouse.Y = Game.MOUSE.Y;

			Vector2 leftFacing = new Vector2(vector[0].X - Game.MOUSE.X, vector[0].Y - mouse.Y);
			Vector2 rightFacing = new Vector2(mouse.X - vector[0].X, mouse.Y - vector[0].Y);

			float leftAngle = (float)(Math.Atan2(leftFacing.Y, leftFacing.X));
			float rightAngle = (float)(Math.Atan2(rightFacing.Y, rightFacing.X));

			if (mouse.X < vector[0].X)
			{
				rotation = leftAngle;
				rectangle[0] = new Rectangle(39, 0, 35, 18);
			}
			if (mouse.X > vector[0].X)
			{
				rotation = rightAngle;
				rectangle[0] = new Rectangle(0, 0, 35, 18);
			}
		}

		public Rectangle Rectangle()
		{
			return new Rectangle((int)vector[0].X - xSize / 2, (int)vector[0].Y - ySize / 2, xSize, ySize);
		}
	}
}
