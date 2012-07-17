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

		// Keyboard state. Used in the same manner as the main method:
		// 1. Capture the current keyboard sate.
		// 2. Do foo, bar.
		// 3. The current becomes the old.
		KeyboardState lastKeyboardState, currentKeyboardState;
		MouseState lastMouseState, currentMouseState;

		// Player lives.
		int lives;
		// Player's velocity.
		int velocity;
		// Is the player walking or running? 
		int walking;
		int running;

		// The width and height of the video game.
		int xBound;
		int yBound;

		int index;

		// Default keys are WSAD.
		Keys up = Keys.W;
		Keys down = Keys.S;
		Keys left = Keys.A;
		Keys right = Keys.D;
		Keys dash = Keys.Space;
		Keys teleport = Keys.F;

		// Dash ability.
		mTimer dashTimer;
		int dashCooldown = 3;
		int dashDuration = 3;

		// Teleport ability.
		mTimer teleportTimer;
		int teleportCooldown = 0;

		enum Boundaries { Warp, Wall };
		// Types. Linked to the above.
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
			// Player stats: Walking speed, running speed, lives, and index.
			running = inputDashVelocity;
			walking = inputVelocity;
			velocity = walking;
			lives = inputLives;
			index = inputIndex;

			// Shoehorning this in here since this method is called in the Initialize() method.
			dashTimer = new mTimer();
			teleportTimer = new mTimer();
		}

		public void Bounds(int inputXBound, int inputYBound)
		{
			// In case I need to switch up bounds later.
			// Thinking of making the game world a 2000x2000 arena.
			xBound = inputXBound;
			yBound = inputYBound;
		}

		public Rectangle Rect()
		{
			// Not entirely sold on the utility of this, but it sure is useful.
			// This returns a rectangle for the player based on the vector and texture.
			int x = (int)Math.Round(vector.X, 0);
			int y = (int)Math.Round(vector.Y, 0);
			return new Rectangle(x, y, texture.Width, texture.Height);
		}

		public void Rebind(Keys inputUp, Keys inputDown, Keys inputLeft, Keys inputRight)
		{
			// Optional, allows for rebinding of controls.
			// Mandatory for more than one player.
			up = inputUp;
			down = inputDown;
			left = inputLeft;
			right = inputRight;
			// A, B, Start...
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			// Draw the player.
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
			// teleportTimer.Update(inputGameTime.ElapsedGameTime);

			if (Keypress(dash))
				if (dashTimer.Elapsed(dashCooldown))
				{
					dashTimer.Reset();
					velocityType = Velocity.Dashing;
				}

			if (Keypress(teleport))
				if (teleportTimer.Elapsed(teleportCooldown))
				{
					teleportTimer.Reset();
					Teleport();
				}

			// Velocity type.
			if (velocityType == Velocity.Walking)
				PlayerWalking();
			if (velocityType == Velocity.Dashing)
				PlayerDashing();

			// Switch bounding based on boundType.
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

		// To explain the next two:
		// It's an escape ability on a long cooldown.
		// You have three seconds at being able to move much faster, and warp around the map.
		private void PlayerWalking()
		{
			velocity = walking;
			boundType = Boundaries.Wall;
		}

		private void PlayerDashing()
		{
			velocity = running;
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
			// If the key has been pressed and then released, true. Else, false.
			// Helper class.
			if (currentKeyboardState.IsKeyUp(inputKey) && (lastKeyboardState.IsKeyDown(inputKey)))
				return true;

			return false;
		}

		public int Rng(int a, int b)
		{
			// Butt-simple random number generator. Roll between a and b.
			// Normally I wouldn't methodize this, but there are things I may need to do here.
			// Wicked things.
			Random random = new Random();
			return random.Next(a, b);
		}

		// Debugging information.
		public void Debug(SpriteFont font, SpriteBatch spriteBatch, int x, int y)
		{
			List<string> debug = new List<string>()
			{
				// What messages are to be drawn.
				// Useful fact: If you are drawing more than one debug panel, put 170.X between them.
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
