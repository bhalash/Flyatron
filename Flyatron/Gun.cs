using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Flyatron
{
	class Gun
	{
		Texture2D texture;
		Vector2 vector = new Vector2(150, 150);
		Rectangle rectangle = new Rectangle(0, 0, 35, 18);
		Color color = Color.White;
		float rotation = 0;
		Vector2 origin = new Vector2(17, 9);
		SpriteEffects effects = SpriteEffects.None;

		Vector2 mouse;

		int xOffset = 0;
		int yOffset = 25;

		public Gun(Texture2D inputTexture)
		{
			texture = inputTexture;
		}

		public void Update(Vector2 reference)
		{
			vector.X = reference.X + xOffset;
			vector.Y = reference.Y + yOffset;

			Animate();
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, vector, rectangle, color, rotation, origin, 1, effects, 0);
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
				rectangle = new Rectangle(39, 0, 35, 18);
			}
			if (mouse.X > vector.X)
			{
				rotation = rightAngle;
				rectangle = new Rectangle(0, 0, 35, 18);
			}
		}
	}
}
