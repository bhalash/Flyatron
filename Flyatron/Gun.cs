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
		public static List<Missile> MISSILES;

		int frameX, frameY, frameWidth, frameHeight;
		float placementXOffset, placementYOffset, rotation;
		Texture2D gunTexture, borderTexture;
		Vector2 gunPosition, rotationOffset;
		Rectangle animationFrame;
		SpriteEffects effects;
		Color color;

		// Gun/bullet.
		Texture2D[] textureHolding;

		public Gun(Texture2D[] inputTexture)
		{
			// Gun's textures.
			textureHolding = inputTexture;
			gunTexture = inputTexture[0];
			borderTexture = inputTexture[2];
			// Munitions-in-flight.
			MISSILES = new List<Missile>();
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
			placementXOffset = 0;
			placementYOffset = 25;

			animationFrame = new Rectangle(
				frameX,
				frameY,
				frameWidth,
				frameHeight
			);
		}

		public void Update(Vector2 reference)
		{
			gunPosition.X = reference.X + placementXOffset;
			gunPosition.Y = reference.Y + placementYOffset;

			Animate();

			if (Helper.LeftClick())
				MISSILES.Add(new Missile(textureHolding, gunPosition));

			for (int i = 0; i < MISSILES.Count; i++)
				if (MISSILES[i].Expired())
					MISSILES.RemoveAt(i);

			for (int i = 0; i < MISSILES.Count; i++)
				MISSILES[i].Update();
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(gunTexture, gunPosition, animationFrame, color, rotation, rotationOffset, 1, effects, 0);

			for (int i = 0; i < MISSILES.Count; i++)
				MISSILES[i].Draw(spriteBatch);

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
		}

		private void Animate()
		{
			Vector2 leftFacing = new Vector2(gunPosition.X - Game.MOUSE.X, gunPosition.Y - Game.MOUSE.Y);
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
