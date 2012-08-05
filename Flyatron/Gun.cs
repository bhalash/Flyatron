using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;

namespace Flyatron
{
	class Gun
	{
		// Used in Game.cs to determine collisions.
		public static List<Bullet> BULLETS;

		int frameWidth, frameHeight;
		float placementXOffset, placementYOffset, rotation;
		Texture2D gunTexture, borderTexture;
		Vector2 gunPosition, rotationOffset;
		Rectangle animationFrame;
		SpriteEffects effects;
		Color color;

		Stopwatch mineSpawn;

		// Gun/bullet.
		Texture2D[] textureHolding;

		public Gun(Texture2D[] inputTexture)
		{
			// Gun's textures.
			textureHolding = inputTexture;
			gunTexture = inputTexture[0];
			borderTexture = inputTexture[2];
			// Munitions-in-flight.
			BULLETS = new List<Bullet>();
			// Offset for animation rotation.
			rotationOffset = new Vector2(17.5F, 9);
			// Width and height of the gun's sprite.
			frameWidth = 35;
			frameHeight = 18;
			// Color, rotation, and effects.
			color = Color.White;
			rotation = 0;
			effects = SpriteEffects.None;
			// Where do you want the gun placed relative to the vector it is attached too?
			placementXOffset = 15;
			placementYOffset = 35;

			animationFrame = new Rectangle(0, 0, frameWidth, frameHeight);

			mineSpawn = new Stopwatch();
		}

		public void Update(Vector2 reference)
		{
			gunPosition.X = reference.X + placementXOffset;
			gunPosition.Y = reference.Y + placementYOffset;

			Animate();

			// Shoot on click.
			if (Helper.LeftClick())
				BULLETS.Add(new Bullet(textureHolding, gunPosition));

			// Shoot on button being held down.
			if (Game.MOUSE.LeftButton == ButtonState.Pressed)
			{
				if (!mineSpawn.IsRunning)
					mineSpawn.Start();

				if (mineSpawn.ElapsedMilliseconds > 170)
				{
					BULLETS.Add(new Bullet(textureHolding, gunPosition));
					mineSpawn.Restart();
				}
			}

			// If no longer on screen, remove.
			for (int i = 0; i < BULLETS.Count; i++)
				if (BULLETS[i].Expired())
					BULLETS.RemoveAt(i);

			// Increment position.
			for (int i = 0; i < BULLETS.Count; i++)
				BULLETS[i].Update();
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(gunTexture, gunPosition, animationFrame, color, rotation, rotationOffset, 1, effects, 0);

			for (int i = 0; i < BULLETS.Count; i++)
				BULLETS[i].Draw(spriteBatch);

			/*
			if (Game.DEBUG)
			{
				int y = 110;

				spriteBatch.DrawString(Game.FONT10, "Bullets:", new Vector2(30, 90), Color.Black);

				for (int i = 0; i < MISSILES.Count; i++)
				{
					spriteBatch.DrawString(
						Game.FONT10, 
						i + ": " + MISSILES[i].Rotation() + " V: " + MISSILES[i].Velocity() + "X: " + MISSILES[i].Position().X + " Y: "  + MISSILES[i].Position().Y, 
						new Vector2(30, y), 
						Color.Black
					);

					// Debug. Outline the rectangle.
					spriteBatch.Draw(borderTexture, gunPosition, animationFrame, color * 0.3F, rotation, rotationOffset, 1, effects, 0);

					y += 15;
				}

				if (MISSILES.Count == 0)
					y = 90;
			}
			*/
		}

		private void Animate()
		{
			Vector2 leftFacing  = new Vector2(gunPosition.X - Game.MOUSE.X, gunPosition.Y - Game.MOUSE.Y);
			Vector2 rightFacing = new Vector2(Game.MOUSE.X - gunPosition.X, Game.MOUSE.Y - gunPosition.Y);

			float leftAngle = (float)(Math.Atan2(leftFacing.Y, leftFacing.X));
			float rightAngle = (float)(Math.Atan2(rightFacing.Y, rightFacing.X));

			if (Game.MOUSE.X < gunPosition.X)
			{
				rotation = leftAngle;
				animationFrame.X = 39;
			}
			if (Game.MOUSE.X > gunPosition.X)
			{
				rotation = rightAngle;
				animationFrame.X = 0;
			}
		}

		public Rectangle Rectangle()
		{
			return new Rectangle(
				(int)gunPosition.X - frameWidth / 2,
				(int)gunPosition.Y - frameHeight / 2,
				frameWidth,
				frameHeight);
		}
	}
}
