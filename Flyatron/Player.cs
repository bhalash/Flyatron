using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using Flyatron;

namespace Flyatron
{
	class Player
	{
		float scale = 0.7F; 

		List<Texture2D> textures;

		static int[] frames = new int[] { 35, 72, 35, 35, 35, 18, 23, 46 };
		static int[] frameOffset = new int[] { 0, 0, 13, 13, 13, 35, 4, 40 };

		static int x = 400;
		static int y = 300;

		// Default keys are WSAD, but are changable via Rebind().
		Keys up    = Keys.W;
		Keys down  = Keys.S;
		Keys left  = Keys.A;
		Keys right = Keys.D;

		Vector2[] vectors = new Vector2[]
		{		
			// Vectors should be updated relative to index 0.
			new Vector2(x + frameOffset[0], y + frameOffset[1]),
			new Vector2(x + frameOffset[2], y + frameOffset[3]),
			new Vector2(x + frameOffset[4], y + frameOffset[5]),
			new Vector2(x + frameOffset[6], y + frameOffset[7])
		};

		Rectangle[] rectangles = new Rectangle[]
		{
			new Rectangle(0,0,35,72), // Body.
			new Rectangle(0,0,35,35), // Head.
			new Rectangle(0,0,35,18), // Gun.
			new Rectangle(0,0,23,46)  // Flames.
		};

		// Player speed.
		int lives, currentLives, velocity, walkingVel;

		// These are the centre of the respective texture frames, used to correctly rotate them.
		Vector2 headOffset = new Vector2(17.5F, 17.5F);
		Vector2 gunOffset = new Vector2(17.5F, 9F);

		float headRotation = 0;
		float gunRotation  = 0;

		Stopwatch flamesTimer = new Stopwatch();
		Stopwatch bobTimer = new Stopwatch();

		public Player(int inputLives, int inputVelocity,Color inputTint, List<Texture2D> inTex)
		{
			textures = inTex;
			walkingVel = inputVelocity;
			velocity = walkingVel;
			lives = currentLives = inputLives;

			flamesTimer.Start();
		}

		public Vector2 Position()
		{
			return vectors[1];
		}

		public void Update(GameTime inputGameTime)
		{
			UpdateBodyAnimation(Game.currentMouseState);
			UpdateGunAnimation(Game.currentMouseState);
			UpdateHeadAnimation(Game.currentMouseState);
			UpdateFlamesAnimation();

			// Mofidul: Here is the movement code I use.
			// Works absolutely fine for me, but I am moving a lot of things.
			// I have separate code (see backgrounds.cs) for passing x/y values.

			if (Game.currentKeyboardState.IsKeyDown(left))
				if (vectors[0].X > 0)
					for (int i = 0; i < vectors.Length; i++)
						vectors[i].X -= velocity;

			if (Game.currentKeyboardState.IsKeyDown(up))
				if (vectors[0].Y > 0)
					for (int i = 0; i < vectors.Length; i++)
						vectors[i].Y -= velocity;

			if (Game.currentKeyboardState.IsKeyDown(right))
				if (vectors[0].X + textures[0].Width < Game.WIDTH)
					for (int i = 0; i < vectors.Length; i++)
						vectors[i].X += velocity;

			if (Game.currentKeyboardState.IsKeyDown(down))
				if (vectors[0].Y + textures[0].Height < Game.WIDTH)
					for (int i = 0; i < vectors.Length; i++)
						vectors[i].Y += velocity;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			// Player Flames.
			spriteBatch.Draw(textures[3], vectors[3], rectangles[3], Color.White, 0, new Vector2(0, 0), scale, SpriteEffects.None, 0);
			// Player body.
			spriteBatch.Draw(textures[0], vectors[0], rectangles[0], Color.White, 0, new Vector2(0, 0), scale, SpriteEffects.None, 0);
			// Player head.
			spriteBatch.Draw(textures[1], vectors[1], rectangles[1], Color.White, headRotation, headOffset, scale, SpriteEffects.None, 0);
			// Player weapon.
			spriteBatch.Draw(textures[2], vectors[2], rectangles[2], Color.White, gunRotation, gunOffset, scale, SpriteEffects.None, 0);
		}

		public Rectangle Rectangle()
		{
			return new Rectangle((int)vectors[0].X, (int)vectors[0].Y, textures[0].Width, textures[0].Height);
		}

		public void X(int newX)
		{
			// Streamline.
			vectors[0].X = newX;
			vectors[1].X = newX + 13;
			vectors[2].X = newX + 13;
			vectors[3].X = newX + 4;
		}

		public void Y(int newY)
		{
			vectors[0].Y = newY;
			vectors[1].Y = newY + 13;
			vectors[2].Y = newY + 45;
			vectors[3].Y = newY + 40;
		}

		public int RemainingLives()
		{
			return currentLives;
		}

		public void Lives(int inputLives)
		{
			currentLives += inputLives;
		}

		public void ResetLives()
		{
			currentLives = lives;
		}

		private void UpdateHeadAnimation(MouseState mouse)
		{
			Vector2 mouseLoc = new Vector2(mouse.X, mouse.Y);

			Vector2 leftFacing = new Vector2(vectors[1].X - mouse.X, vectors[1].Y - mouse.Y);
			Vector2 rightFacing = new Vector2(mouse.X - vectors[1].X, mouse.Y - vectors[1].Y);

			float leftAngle = (float)(Math.Atan2(leftFacing.Y, leftFacing.X));
			float rightAngle = (float)(Math.Atan2(rightFacing.Y, rightFacing.X));

			if (mouse.X < vectors[1].X)
			{
				headRotation = leftAngle;
				rectangles[1] = new Rectangle(44, 0, frames[2], frames[3]);
			}
			if (mouse.X > vectors[0].X)
			{
				headRotation = rightAngle;
				rectangles[1] = new Rectangle(0, 0, frames[2], frames[3]);
			}
		}

		private void UpdateGunAnimation(MouseState mouse)
		{
			Vector2 mouseLoc = new Vector2(mouse.X, mouse.Y);

			Vector2 leftFacing = new Vector2(vectors[2].X - mouse.X, vectors[2].Y - mouse.Y);
			Vector2 rightFacing = new Vector2(mouse.X - vectors[2].X, mouse.Y - vectors[2].Y);

			float leftAngle = (float)(Math.Atan2(leftFacing.Y, leftFacing.X));
			float rightAngle = (float)(Math.Atan2(rightFacing.Y, rightFacing.X));

			if (mouse.X < vectors[2].X)
			{
				gunRotation = leftAngle;
				rectangles[2] = new Rectangle(39, 0, frames[4], frames[5]);
			}
			if (mouse.X > vectors[2].X)
			{
				gunRotation = rightAngle;
				rectangles[2] = new Rectangle(0, 0, frames[4], frames[5]);
			}
		}

		private void UpdateBodyAnimation(MouseState mouse)
		{
			if (mouse.X < vectors[0].X)
			{
				rectangles[0] = new Rectangle(40, 0, frames[0], frames[1]);
			}
			if (mouse.X > vectors[0].X)
			{
				rectangles[0] = new Rectangle(0, 0, frames[0], frames[1]);
			}
		}

		private void UpdateFlamesAnimation()
		{
			if ((flamesTimer.ElapsedMilliseconds >= 0) && (flamesTimer.ElapsedMilliseconds < 300))
				rectangles[3] = new Rectangle(52, 0, frames[6], frames[7]);
			if ((flamesTimer.ElapsedMilliseconds >= 300) && (flamesTimer.ElapsedMilliseconds < 600))
				rectangles[3] = new Rectangle(25, 0, frames[6], frames[7]);
			if ((flamesTimer.ElapsedMilliseconds >= 600) && (flamesTimer.ElapsedMilliseconds < 900))
				rectangles[3] = new Rectangle(0, 0, frames[6], frames[7]);

			if (flamesTimer.ElapsedMilliseconds > 900)
				flamesTimer.Restart();
		}
	}
}
