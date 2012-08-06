using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Flyatron
{
	class Bullet
	{
		Texture2D bulletTexture, borderTexture;
		Vector2 mousePosition, bulletPosition, rotationOffset, bulletPath;
		Rectangle bullet;
		float rotation, scale, velocity;
		SpriteEffects effects;
		Color color;

		enum Bulletstate { Traversing, Detonating, Expired };
		Bulletstate state;

		public Bullet(Texture2D[] inputTexture, Vector2 gunPosition)
		{
			// Initial bullet state should be "in-flight."
			state = Bulletstate.Traversing;
			// x/y velocity; pixels per frame.
			velocity = 16;
			scale = 1;
			// Textures.
			bulletTexture = inputTexture[1];
			borderTexture = inputTexture[2];
			// Effects and colour.
			effects = SpriteEffects.None;
			color = Color.White;
			// Animation frame.
			bullet = new Rectangle(0, 0, 55, 11);
			// Gun vector.
			bulletPosition.X = gunPosition.X;
			bulletPosition.Y = gunPosition.Y - bulletTexture.Height / 2;
			// Offset, used for animatng rotation.
			rotationOffset = new Vector2(27.5F, 10.5F);
			// Mouse. Snapstopped. 
			mousePosition = new Vector2(Game.MOUSE.X, Game.MOUSE.Y);

			// If mouse is left of bullet.
			if (mousePosition.X < bulletPosition.X)
			{
				// Animation frame.
				bullet.X = 59;
				// Reverse velocity.
				velocity = -velocity;
				// The path the bullet will travel after firing.
				bulletPath = mousePosition - bulletPosition;
			}

			// If mouse is right of bullet.
			if (mousePosition.X >= bulletPosition.X)
			{
				bullet.X = 0;
				bulletPath = bulletPosition - mousePosition;
			}

			// Rotation is set once.
			rotation = (float)(Math.Atan2(bulletPath.Y, bulletPath.X));
		}

		public void Update()
		{
			switch (state)
			{
				case Bulletstate.Traversing:
					{
						Traversing();
						break;
					}
				case Bulletstate.Detonating:
					{
						Detonating();
						break;
					}
			}
		}

		private void Traversing()
		{
			if (bulletPath != Vector2.Zero)
				bulletPath.Normalize();

			bulletPosition -= bulletPath * velocity;
			
			if (!Rectangle().Intersects(Game.BOUNDS))
				state = Bulletstate.Expired;
		}

		private void Detonating()
		{
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if (state == Bulletstate.Traversing)
			{
				spriteBatch.Draw(bulletTexture, bulletPosition, bullet, color, rotation, rotationOffset, scale, effects, 0F);
				
				if (Game.DEBUG)
					spriteBatch.Draw(borderTexture, bulletPosition, bullet, color * 0.3F, rotation, rotationOffset, scale, effects, 0F);
			}
		}

		public Rectangle Rectangle()
		{
			return new Rectangle((int)bulletPosition.X, (int)bulletPosition.Y, bullet.X, bullet.Y);
		}

		public bool Expired()
		{
			if (state == Bulletstate.Expired)
				return true;

			return false;
		}

		public void State(int newState)
		{
			if (newState == 1)
				state = Bulletstate.Traversing;
			if (newState == 2)
				state = Bulletstate.Detonating;
			if (newState == 3)
				state = Bulletstate.Expired;
		}

		public string Rotation()
		{
			return Convert.ToString(rotation);
		}

		public string Velocity()
		{
			return Convert.ToString(velocity);
		}

		public Vector2 Position()
		{
			return bulletPosition;
		}
	}
}
