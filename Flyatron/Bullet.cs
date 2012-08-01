using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Flyatron
{
	class Bullet
	{
		int xSize, ySize;
		float xOffset, yOffset;

		Rectangle rectangle, frame;
		// Animation.
		Texture2D[] texture;
		Vector2 origin;
		Color color = Color.White;
		float rotation, scale;
		SpriteEffects effects = SpriteEffects.None;

		Vector2 source, destination;

		enum Bulletstate { Traversing, Detonating, Detonated };
		Bulletstate state = Bulletstate.Traversing;

		public Bullet(Texture2D[] newTexture, Rectangle gunPosition)
		{
			xSize = 55;
			ySize = 11;

			xOffset = 27.5F;
			yOffset = 10.5F;

			source.X = gunPosition.X;
			source.Y = gunPosition.Y;

			destination.X = Game.CURR_MOUSE.X;
			destination.Y = Game.CURR_MOUSE.Y;

			rectangle = new Rectangle((int)source.X, (int)source.Y, xSize, ySize);
			frame = new Rectangle(0, 0, xSize, ySize);
			color = Color.White;
			rotation = 0;
			scale = 1;
			effects = SpriteEffects.None;
			origin = new Vector2(0, 0);

			texture = newTexture;
		}

		public void Update()
		{
			source.X += 10;
		}

		public bool Expired()
		{
			// Report the bullet's state for the purposes of expiring it.

			Rectangle bounds = new Rectangle(0, 0, Game.WIDTH, Game.HEIGHT);

			if (state == Bulletstate.Detonated)
				return true;
			// Failsafe. The bullet should never leave the screen.
			if (!rectangle.Intersects(bounds))
				return true;

			return false;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			switch (state)
			{
				case Bulletstate.Traversing:
					{
						spriteBatch.Draw(texture[2], Rectangle(), color * 0.3F);

						break;
					}
				case Bulletstate.Detonating:
					{
						break;
					}
			}
		}

		public Rectangle Rectangle()
		{
			return new Rectangle((int)rectangle.X, (int)rectangle.Y, xSize, ySize);
		}

		public void State(int newState)
		{
			if (newState == 1)
				state = Bulletstate.Traversing;
			if (newState == 2)
				state = Bulletstate.Detonating;
			if (newState == 3)
				state = Bulletstate.Detonated;
		}
	}
}
