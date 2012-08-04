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
		// Default keys are WSAD, but are changable via Rebind().
		Keys up;
		Keys down;
		Keys left;
		Keys right;

		enum Playerstate { Alive, Dead };
		Playerstate state;

		Stopwatch exhaust;
		Texture2D[] texture;
		Vector2[] vector;
		Rectangle[] rectangle;

		float scale, headRotation;
		int[] frames, frameOffset;
		int x, y;
		int lives, currentLives, velocity, walkingVel;
		Vector2 headOffset;

		public Player(int newLives, int newVelocity, Color newTint, Texture2D[] newText)
		{

			frames = new int[] { 35, 72, 35, 35, 23, 46 };
			frameOffset = new int[] { 0, 0, 13, 13, 4, 40 };

			x = 400;
			y = 300;

			headOffset = new Vector2(17.5F, 17.5F);
			
			// Default keys are WSAD, but are changable via Rebind().
			up = Keys.W;
			down = Keys.S;
			left = Keys.A;
			right = Keys.D;

			vector = new Vector2[]
			{		
				// Vectors should be updated relative to index 0.
				new Vector2(x + frameOffset[0], y + frameOffset[1]),
				new Vector2(x + frameOffset[2], y + frameOffset[3]),
				new Vector2(x + frameOffset[4], y + frameOffset[5])
			};

			rectangle = new Rectangle[]
			{
				new Rectangle(0,0,35,72), // Body.
				new Rectangle(0,0,35,35), // Head.
				new Rectangle(0,0,23,46)  // Flames.
			};

			scale = 0.7F; 

			texture = newText;
			walkingVel = newVelocity;
			velocity = walkingVel;
			lives = currentLives = newLives;

			exhaust = new Stopwatch();
			exhaust.Start();
		}

		public Vector2 Position()
		{
			return vector[1];
		}

		public void Update()
		{
			switch (state)
			{
				case Playerstate.Alive:
					{
						Living();
						break;
					}
				case Playerstate.Dead:
					{
						Dead();
						break;
					}
			};
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if (state == Playerstate.Alive)
			{
				// Player Flames.
				spriteBatch.Draw(texture[2], vector[2], rectangle[2], Color.White, 0, new Vector2(0, 0), scale, SpriteEffects.None, 0);
				// Player body.
				spriteBatch.Draw(texture[0], vector[0], rectangle[0], Color.White, 0, new Vector2(0, 0), scale, SpriteEffects.None, 0);
				// Player head.
				spriteBatch.Draw(texture[1], vector[1], rectangle[1], Color.White, headRotation, headOffset, scale, SpriteEffects.None, 0);
			}
		}

		private void Dead()
		{
			if (lives > 0)
			{
				Lives(-1);
				X(50);
				Y(Game.HEIGHT / 2 - texture[0].Height / 2);
			}

			state = Playerstate.Alive;
		}

		private void Living()
		{
			UpdateBodyAnimation(Game.MOUSE);
			UpdateHeadAnimation(Game.MOUSE);
			UpdateFlamesAnimation();

			if (Game.KEYBOARD.IsKeyDown(left))
				if (vector[0].X > 0)
					for (int i = 0; i < vector.Length; i++)
						vector[i].X -= velocity;

			if (Game.KEYBOARD.IsKeyDown(up))
				if (vector[0].Y > 0)
					for (int i = 0; i < vector.Length; i++)
						vector[i].Y -= velocity;

			if (Game.KEYBOARD.IsKeyDown(right))
				if (vector[0].X + 30 < Game.WIDTH)
					for (int i = 0; i < vector.Length; i++)
						vector[i].X += velocity;

			if (Game.KEYBOARD.IsKeyDown(down))
				if (vector[0].Y + 50 < Game.HEIGHT)
					for (int i = 0; i < vector.Length; i++)
						vector[i].Y += velocity;
		}

		public Rectangle Rectangle()
		{
			return new Rectangle((int)vector[0].X, (int)vector[0].Y, 25, 52);
		}

		public void X(int newX)
		{
			// Streamline.
			vector[0].X = newX;
			vector[1].X = newX + 13;
			vector[2].X = newX + frameOffset[4];
		}

		public void Y(int newY)
		{
			vector[0].Y = newY;
			vector[1].Y = newY + 13;
			vector[2].Y = newY + frameOffset[5];
		}

		public int RemainingLives()
		{
			return currentLives;
		}

		public void Lives(int inputLives)
		{
			currentLives = inputLives;
		}

		public void Plus()
		{
			currentLives++;
		}

		public void Minus()
		{
			currentLives--;
		}

		public void ResetLives()
		{
			currentLives = lives;
		}

		public void State(int newState)
		{
			if (newState == 1)
				state = Playerstate.Alive;
			if (newState == 2)
				state = Playerstate.Dead;
		}

		private void UpdateHeadAnimation(MouseState mouse)
		{
			Vector2 mouseLoc = new Vector2(Game.MOUSE.X, Game.MOUSE.Y);

			Vector2 leftFacing = new Vector2(vector[1].X - mouse.X, vector[1].Y - mouse.Y);
			Vector2 rightFacing = new Vector2(mouse.X - vector[1].X, mouse.Y - vector[1].Y);

			if (mouse.X < vector[1].X)
			{
				headRotation = (float)(Math.Atan2(leftFacing.Y, leftFacing.X));
				rectangle[1] = new Rectangle(44, 0, frames[2], frames[3]);
			}

			else if (mouse.X > vector[0].X)
			{
				headRotation = (float)(Math.Atan2(rightFacing.Y, rightFacing.X));
				rectangle[1] = new Rectangle(0, 0, frames[2], frames[3]);
			}
		}

		private void UpdateBodyAnimation(MouseState mouse)
		{
			if (mouse.X < vector[0].X)
			{
				rectangle[0] = new Rectangle(40, 0, frames[0], frames[1]);
			}
			if (mouse.X > vector[0].X)
			{
				rectangle[0] = new Rectangle(0, 0, frames[0], frames[1]);
			}
		}

		private void UpdateFlamesAnimation()
		{
			if ((exhaust.ElapsedMilliseconds >= 0) && (exhaust.ElapsedMilliseconds < 300))
				rectangle[2] = new Rectangle(52, 0, frames[4], frames[5]);
			if ((exhaust.ElapsedMilliseconds >= 300) && (exhaust.ElapsedMilliseconds < 600))
				rectangle[2] = new Rectangle(25, 0, frames[4], frames[5]);
			if ((exhaust.ElapsedMilliseconds >= 600) && (exhaust.ElapsedMilliseconds < 900))
				rectangle[2] = new Rectangle(0, 0, frames[4], frames[5]);

			if (exhaust.ElapsedMilliseconds > 900)
				exhaust.Restart();
		}
	}
}
