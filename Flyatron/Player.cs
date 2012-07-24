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
		Texture2D blah;

		// The player's texture, assigned vector, and tint (if any).
		Color tint;

		Texture2D[] textures;
		Vector2[] vectors;
		Rectangle[] rectangles;

		KeyboardState lastKeyboardState, currentKeyboardState;
		MouseState lastMouseState, currentMouseState;

		// Player speed.
		int lives, velocity, walkingVel, runningVel;
		// Player bounds.
		int xBound, yBound;
		// Player index. For multiplayer.
		int index;

		// Default keys are WSAD, but are changable via Rebind().
		Keys up			= Keys.W;
		Keys down		= Keys.S;
		Keys left		= Keys.A;
		Keys right		= Keys.D;
		Keys dash		= Keys.Space;
		Keys teleport	= Keys.F;

		enum Velocity { Walking, Dashing };
		Velocity velocityType = Velocity.Walking;

		// These are the centre of the respective texture frames, used to correctly rotate them.
		Vector2 headOffset = new Vector2(17.5F, 17.5F);
		Vector2 gunOffset = new Vector2(17.5F, 9F);

		float headRotation = 0;
		float gunRotation  = 0;

		Stopwatch flamesTimer = new Stopwatch();
		Stopwatch bobTimer = new Stopwatch();

		// Initialize player animation data.
		int[] frames = new int[]
		{
			// Width, height.
			// Head.
			35,35,
			// Body.
			35,72,
			// Gun.
			35,18,
			// Flames.
			23,46
		};

		public Player(int inputLives, int inputVelocity, int inputDashVelocity, int inputIndex, Color inputTint, Texture2D[] inTex)
		{
			textures = inTex;
			tint = inputTint;
			runningVel = inputDashVelocity;
			walkingVel = inputVelocity;
			velocity = walkingVel;
			lives = inputLives;
			index = inputIndex;

			flamesTimer.Start();

			vectors = new Vector2[]
			{		
				// Vectors should be updated relative to the body.
				new Vector2(400 + 17.5F, 300 + 17.5F),
				new Vector2(400, 300),
				new Vector2(400 + 17.5F, 300 + 45),
				new Vector2(400 + 6, 300 + 54)
			};

			rectangles = new Rectangle[]
			{
				new Rectangle(0,0,35,35), // Head.
				new Rectangle(0,0,35,72), // Body.
				new Rectangle(0,0,35,18), // Gun.
				new Rectangle(0,0,23,46)  // Flames.
			};
		}

		public Vector2 Position()
		{
			return vectors[1];
		}

		public void Update(KeyboardState inputState, MouseState inputMouseState, GameTime inputGameTime)
		{
			currentKeyboardState = inputState;
			currentMouseState = inputMouseState;

			UpdateBodyAnimation(currentMouseState);
			UpdateGunAnimation(currentMouseState);
			UpdateHeadAnimation(currentMouseState);
			UpdateFlamesAnimation();

			// Mofidul: Here is the movement code I use.
			// Works absolutely fine for me, but I am moving a lot of things.
			// I have separate code (see backgrounds.cs) for passing x/y values.

			if (vectors[1].X > 0)
				if (currentKeyboardState.IsKeyDown(left))
					for (int i = 0; i < vectors.Length; i++)
						vectors[i].X -= velocity;

			if (vectors[1].Y > 0)
				if (currentKeyboardState.IsKeyDown(up))
					for (int i = 0; i < vectors.Length; i++)
						vectors[i].Y -= velocity;
			Vector2 blah = new Vector2(0, 0);
			if (vectors[1].X + 35 < xBound)
				if (currentKeyboardState.IsKeyDown(right))
					for (int i = 0; i < vectors.Length; i++)
						vectors[i].X += velocity;

			if (vectors[1].Y + textures[1].Height < yBound)
				if (currentKeyboardState.IsKeyDown(down))
					for (int i = 0; i < vectors.Length; i++)
						vectors[i].Y += velocity;

			lastMouseState = currentMouseState;
			lastKeyboardState = currentKeyboardState;
		}

		private void Move();

		public void Draw(SpriteBatch spriteBatch)
		{
			// Player Flames.
			spriteBatch.Draw(textures[3], vectors[3], rectangles[3], Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
			// Player body.
			spriteBatch.Draw(textures[1], vectors[1], rectangles[1], Color.White, 0, new Vector2(0, 0), 1, SpriteEffects.None, 0);
			// Player head.
			spriteBatch.Draw(textures[0], vectors[0], rectangles[0], Color.White, headRotation, headOffset, 1, SpriteEffects.None, 0);
			// Player weapon.
			spriteBatch.Draw(textures[2], vectors[2], rectangles[2], Color.White, gunRotation, gunOffset, 1, SpriteEffects.None, 0);
		}

		public void Bounds(int inputXBound, int inputYBound)
		{
			xBound = inputXBound;
			yBound = inputYBound;
		}

		public Rectangle Rect()
		{
			// Return a rectangle for the player based on the vector and texture. Used for collisions.
			// The precision loss is negligible (~1-2px).
			int x = (int)Math.Round(vectors[1].X, 0);
			int y = (int)Math.Round(vectors[1].Y, 0);
			return new Rectangle(x, y, textures[1].Width, textures[1].Height);
		}

		public void Rebind(Keys inputUp, Keys inputDown, Keys inputLeft, Keys inputRight)
		{
			up = inputUp;
			down = inputDown;
			left = inputLeft;
			right = inputRight;
		}

		public int RemainingLives()
		{
			return lives;
		}

		public void Lives(int inputLives)
		{
			if (inputLives > 0)
				lives += inputLives;
			if (inputLives < 0)
				lives -= inputLives;
		}

		private void Teleport()
		{
			// Blink, blonk.
			vectors[1].X = Rng(0, xBound - 50);
			vectors[1].Y = Rng(0, yBound - 50);
		}

		// Helpers.
		private bool Keypress(Keys inputKey)
		{
			if (currentKeyboardState.IsKeyUp(inputKey) && (lastKeyboardState.IsKeyDown(inputKey)))
				return true;

			return false;
		}

		public void ZeroLives()
		{
			// For debug.
			lives = 0;
		}

		private int Rng(int a, int b)
		{
			Random random = new Random();
			return random.Next(a, b);
		}

		private void UpdateHeadAnimation(MouseState mouse)
		{
			Vector2 mouseLoc = new Vector2(mouse.X, mouse.Y);

			Vector2 leftFacing = new Vector2(vectors[0].X - mouse.X, vectors[0].Y - mouse.Y);
			Vector2 rightFacing = new Vector2(mouse.X - vectors[0].X, mouse.Y - vectors[0].Y);

			float leftAngle = (float)(Math.Atan2(leftFacing.Y, leftFacing.X));
			float rightAngle = (float)(Math.Atan2(rightFacing.Y, rightFacing.X));

			if (mouse.X < vectors[0].X)
			{
				headRotation = leftAngle;
				rectangles[0] = new Rectangle(44, 0, frames[0], frames[1]);
			}
			if (mouse.X > vectors[0].X)
			{
				headRotation = rightAngle;
				rectangles[0] = new Rectangle(0, 0, frames[0], frames[1]);
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
			if (mouse.X < vectors[1].X)
			{
				rectangles[1] = new Rectangle(40, 0, frames[2], frames[3]);
			}
			if (mouse.X > vectors[1].X)
			{
				rectangles[1] = new Rectangle(0, 0, frames[2], frames[3]);
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
