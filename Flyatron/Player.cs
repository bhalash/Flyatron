using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
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

		int lives;
		int velocity;
		int walkingVel;
		int runningVel;

		// Player bounds.
		int xBound;
		int yBound;

		// Player index. For multiplayer.
		int index;

		// Default keys are WSAD, but are changable via Rebind().
		Keys up = Keys.W;
		Keys down = Keys.S;
		Keys left = Keys.A;
		Keys right = Keys.D;
		Keys dash = Keys.Space;
		Keys teleport = Keys.F;

		// Dash ability.
		mTimer dashTimer;
		int dashCooldown = 0;
		int dashDuration = 5;

		// Teleport ability.
		mTimer teleportTimer;
		int teleportCooldown = 0;

		enum Boundaries { Warp, Wall };
		Boundaries boundType;

		enum Velocity { Walking, Dashing };
		Velocity velocityType;

		public Player()
		{
		}

		public void Art(Texture2D inputTexture, Vector2 inputVector, Color inputTint)
		{
			// Input art: Texture, vector, and tint.
			texture = inputTexture;
			vector = inputVector;
			tint = inputTint;
		}

		public void Stats(int inputLives, int inputVelocity, int inputDashVelocity, int inputIndex)
		{
			// Player stats: Walking speed, runningVel speed, lives, and index.
			runningVel = inputDashVelocity;
			walkingVel = inputVelocity;
			velocity = walkingVel;
			lives = inputLives;
			index = inputIndex;

			dashTimer = new mTimer();
			teleportTimer = new mTimer();
		}

		public void Bounds(int inputXBound, int inputYBound)
		{
			xBound = inputXBound;
			yBound = inputYBound;
		}

		public Rectangle Rect()
		{
			// Return a rectangle for the player based on the vector and texture. Used for collisions.
			// The precision loss is negigible (~1-2px).
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
			// A, B, Start...
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture, vector, tint);
			// Draw player HUD.
			// TODO
		}

		public void Update(KeyboardState inputState, MouseState inputMouseState, GameTime inputGameTime)
		{
			currentKeyboardState = inputState;
			currentMouseState = inputMouseState;

			// Update timers.
			dashTimer.Update(inputGameTime.ElapsedGameTime);
			teleportTimer.Update(inputGameTime.ElapsedGameTime);

			if (Keypress(dash))
				if (dashTimer.Elapsed(dashCooldown))
				{
					// Burst of speed and warp across boundaries.
					dashTimer.Reset();
					velocityType = Velocity.Dashing;
				}

			if (Keypress(teleport))
				if (teleportTimer.Elapsed(teleportCooldown))
				{
					// Teleport player to a random location. 
					// TODO: Make player explode (a la Asteroids).
					teleportTimer.Reset();
					Teleport();
				}

			if (velocityType == Velocity.Walking)
				PlayerWalking();
			if (velocityType == Velocity.Dashing)
				PlayerDashing();

			if (boundType == Boundaries.Wall)
				Walled();
			if (boundType == Boundaries.Warp)
				Warping();

			// Up, down, left, right.
			if (currentKeyboardState.IsKeyDown(up))
				vector.Y -= velocity;
			if (currentKeyboardState.IsKeyDown(down))
				vector.Y += velocity;
			if (currentKeyboardState.IsKeyDown(left))
				vector.X -= velocity;
			if (currentKeyboardState.IsKeyDown(right))
				vector.X += velocity;

			lastMouseState = currentMouseState;
			lastKeyboardState = currentKeyboardState;
		}

		private void Walled()
		{
			// If Walled, do not allow the player to go beyond screen bounds.
			if (vector.X < 0)
				vector.X = 0;
			if (vector.Y < 0)
				vector.Y = 0;
			if (vector.Y + texture.Height > yBound)
				vector.Y = yBound - texture.Height;
			if (vector.X + texture.Width > xBound)
				vector.X = xBound - texture.Width;
		}

		private void Warping()
		{
			// If Warp, warp the player to the opposite side of the screen.
			if (vector.X + texture.Width < 0)
				vector.X = xBound;
			if (vector.X > xBound)
				vector.X = 0 - texture.Width;
			if (vector.Y + texture.Height < 0)
				vector.Y = yBound;
			if (vector.Y > yBound)
				vector.Y = 0 - texture.Height;
		}

		private void PlayerWalking()
		{
			velocity = walkingVel;
			boundType = Boundaries.Wall;
		}

		private void PlayerDashing()
		{
			velocity = runningVel;
			boundType = Boundaries.Warp;

			if (dashTimer.Elapsed(dashDuration))
				velocityType = Velocity.Walking;
		}

		private void Teleport()
		{
			// Blink, blonk.
			vector.X = Rng(0, xBound - 50);
			vector.Y = Rng(0, yBound - 50);
		}

		// Helpers.
		private bool Keypress(Keys inputKey)
		{
			if (currentKeyboardState.IsKeyUp(inputKey) && (lastKeyboardState.IsKeyDown(inputKey)))
				return true;

			return false;
		}

		public int Rng(int a, int b)
		{
			Random random = new Random();
			return random.Next(a, b);
		}

		// Debugging information.
		public void Debug(SpriteFont font, SpriteBatch spriteBatch, int x, int y)
		{
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
				"Bounding: " + boundType,
				"Sprint: " + dashTimer.Elapsed(dashCooldown),
				"Teleport: " + teleportTimer.Elapsed(teleportCooldown),
			};

			for (int i = 0; i < debug.Count; i++)
			{
				spriteBatch.DrawString(font, debug[i], new Vector2(x, y), Color.Black);
				y += 15;
			}
		}
	}
}
