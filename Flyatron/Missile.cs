using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Flyatron
{
	class Missile
	{
		Texture2D[]	texture;
		static Vector2[]   vector;
		static Rectangle[] rectangle;
		static int frameX, frameY, frameW, frameH;
		float rotation, scale, velocity;
		SpriteEffects effects;

		enum Bulletstate { Traversing, Detonating, Detonated };
		Bulletstate state;

		Color color;

		public Missile(Texture2D[] newTexture, Vector2 newVector)
		{
			texture = newTexture;

			color = Color.White;

			// frameX is set based on mouse position.
			frameY = 0;
			frameW = 55;
			frameH = 11;

			vector = new Vector2[]
			{
				// Gun vector.
				newVector,
				// Ofset, used for rotation.
				new Vector2(27.5F, 10.5F),
				// Mouse.
				new Vector2(Game.MOUSE.X, Game.MOUSE.Y),
				// Difference vector. Used for movement.
				new Vector2(0,0)
			};

			// If mouse is left of gun.
			if (vector[2].X < vector[0].X)
			{
				// Animation frame.
				frameX = 59;
				// Left or right velocity? 
				velocity = -10;
				// Difference. Uses angle of rotation.
				vector[3] = vector[2] - vector[0];
			}

			// If mouse is right of gun.
			if (vector[2].X >= vector[0].X)
			{
				frameX = 0;
				velocity = 10;
				vector[3] = vector[0] - vector[2];
			}

			rectangle = new Rectangle[]
			{
				// Movement frame.
				new Rectangle((int)vector[0].X, (int)vector[0].Y, frameW, frameH),
				// Animation frame.
				new Rectangle(frameX, frameY, frameW, frameH),
				// Debug.
				new Rectangle((int)Game.MOUSE.X, (int)Game.MOUSE.Y, 50,50)
			};

			rotation = (float)(Math.Atan2(vector[3].Y, vector[3].X));
			scale = 1;
			state = Bulletstate.Traversing;
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
				case Bulletstate.Detonated:
					{
						Detonated();
						break;
					}
			}
		}

		private void Traversing()
		{
			if (vector[3] != Vector2.Zero)
				vector[3].Normalize();

			vector[0] -= vector[3] * velocity;
		}

		private void Detonated()
		{
		}

		private void Detonating()
		{
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			switch (state)
			{
				case Bulletstate.Traversing:
					{
						spriteBatch.Draw(texture[1], vector[0], rectangle[1], color, rotation, vector[1], scale, effects, 0F);

						if (Game.DEBUG)
							spriteBatch.Draw(texture[2], vector[0], rectangle[1], color * 0.3F, rotation, vector[1], scale, effects, 0F);
						break;
					}
			}
		}

		public static Rectangle Rectangle()
		{
			return new Rectangle((int)vector[0].X, (int)vector[0].Y, frameW, frameH);
		}

		public bool Expired()
		{
			Rectangle bounds = new Rectangle(0, 0, Game.WIDTH, Game.HEIGHT);

			if (state == Bulletstate.Detonated)
				return true;
			if (!Rectangle().Intersects(bounds))
				return true;

			return false;
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
			return vector[0];
		}
	}
}
