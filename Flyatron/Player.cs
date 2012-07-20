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
		// The player's texture, assigned vector, and tint (if any).
		Texture2D texture;
		Vector2 vector;
		Color tint;

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

		public Player(int inputLives, int inputVelocity, int inputDashVelocity, int inputIndex, Texture2D inputTexture, Color inputTint)
		{
			// Input art: Texture, vector, and tint.
			texture = inputTexture;
			tint = inputTint;

			// Player stats: Walking speed, runningVel speed, lives, and index.
			runningVel = inputDashVelocity;
			walkingVel = inputVelocity;
			velocity = walkingVel;
			lives = inputLives;
			index = inputIndex;
		}

		public void Update(KeyboardState inputState, MouseState inputMouseState, GameTime inputGameTime)
		{
			currentKeyboardState = inputState;
			currentMouseState = inputMouseState;

			if (vector.X > 0)
				if (currentKeyboardState.IsKeyDown(left))
					vector.X -= velocity;

			if (vector.Y > 0)
				if (currentKeyboardState.IsKeyDown(up))
					vector.Y -= velocity;

			if (vector.X + texture.Width < xBound)
				if (currentKeyboardState.IsKeyDown(right))
					vector.X += velocity;

			if (vector.Y + texture.Height < yBound)
				if (currentKeyboardState.IsKeyDown(down))
					vector.Y += velocity;

			lastMouseState = currentMouseState;
			lastKeyboardState = currentKeyboardState;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, vector, tint);
			// Draw player HUD.
			// TODO
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
			int x = (int)Math.Round(vector.X, 0);
			int y = (int)Math.Round(vector.Y, 0);
			return new Rectangle(x, y, texture.Width, texture.Height);
		}

		public void Rebind(Keys inputUp, Keys inputDown, Keys inputLeft, Keys inputRight)
		{
			up = inputUp;
			down = inputDown;
			left = inputLeft;
			right = inputRight;
		}

		private void Teleport()
		{
			// Blink, blonk.
			vector.X = Rng(0, xBound - 50);
			vector.Y = Rng(0, yBound - 50);
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

		public void ZeroLives()
		{
			lives = 0;
		}

		// Helpers.
		private bool Keypress(Keys inputKey)
		{
			if (currentKeyboardState.IsKeyUp(inputKey) && (lastKeyboardState.IsKeyDown(inputKey)))
				return true;

			return false;
		}

		private int Rng(int a, int b)
		{
			Random random = new Random();
			return random.Next(a, b);
		}

		// Debugging information.
		public void Debug(SpriteFont font, SpriteBatch spriteBatch)
		{
			int x = 30;
			int y = 30;

			List<string> debug = new List<string>()
			{
				// Put 170.X between duplicate debug panels.
				"Player #: "	+ index,
				"X: " + vector.X,
				"Y: " + vector.Y,
				"Width: " + texture.Width,
				"Height: " + texture.Height,
				"Lives: " + lives,
				"Velocity: " + velocity,
			};

			for (int i = 0; i < debug.Count; i++)
			{
				spriteBatch.DrawString(font, debug[i], new Vector2(x, y), Color.Black);
				y += 15;
			}
		}
	}
}
