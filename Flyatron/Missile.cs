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
		Texture2D missileTexture, borderTexture;
		Vector2 mousePosition, missilePosition, rotationOffset, missilePath;
		Rectangle animationFrame, debugBorder;
		int frameX, frameY, frameWidth, frameHeight;
		float rotation, scale, velocity, velocityMaster;
		SpriteEffects effects;
		Color color;

		enum Bulletstate { Traversing, Detonating, Expired };
		Bulletstate state;

		public Missile(Texture2D[] inputTexture, Vector2 gunPosition)
		{
			// Initial bullet state should be "in-flight."
			state = Bulletstate.Traversing;
			// x/y velocity; pixels per frame.
			velocityMaster = 16;
			scale = 1;
			// Textures.
			missileTexture = inputTexture[1];
			borderTexture = inputTexture[2];
			// Effects and colour.
			effects = SpriteEffects.None;
			color = Color.White;
			// frameX is set based on mouse position.
			frameY = 0;
			frameWidth = 55;
			frameHeight = 11;
			// Gun vector.
			missilePosition = gunPosition;
			// Offset, used for animatng rotation.
			rotationOffset = new Vector2(27.5F, 10.5F);
			// Mouse. Snapstopped. 
			mousePosition = new Vector2(Game.MOUSE.X, Game.MOUSE.Y);

			// If mouse is left of gun.
			if (mousePosition.X < missilePosition.X)
			{
				// Animation frame.
				frameX = 59;
				// Left or right velocity? 
				velocity = -velocityMaster;
				// The path the bullet will travel after firing.
				missilePath = mousePosition - missilePosition;
			}

			// If mouse is right of gun.
			if (mousePosition.X >= missilePosition.X)
			{
				frameX = 0;
				velocity = velocityMaster;
				missilePath = missilePosition - mousePosition;
			}

			// Rotation is set once.
			rotation = (float)(Math.Atan2(missilePath.Y, missilePath.X));

			// Frame within texture for animation purposes.
			animationFrame = new Rectangle(
				frameX,
				frameY,
				frameWidth,
				frameHeight
			);

			// Debug.
			debugBorder = new Rectangle(
				(int)mousePosition.X,
				(int)mousePosition.Y,
				50, 
				50
			);
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
			if (missilePath != Vector2.Zero)
				missilePath.Normalize();

			missilePosition -= missilePath * velocity;
			
			if (!Rectangle().Intersects(Game.BOUNDS))
				state = Bulletstate.Expired;
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
						spriteBatch.Draw(missileTexture, missilePosition, animationFrame, color, rotation, rotationOffset, scale, effects, 0F);

						if (Game.DEBUG)
							spriteBatch.Draw(borderTexture, missilePosition, animationFrame, color * 0.3F, rotation, rotationOffset, scale, effects, 0F);

						break;
					}
			}
		}

		public Rectangle Rectangle()
		{
			return new Rectangle((int)missilePosition.X, (int)missilePosition.Y, frameWidth, frameHeight);
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
			return missilePosition;
		}
	}
}
