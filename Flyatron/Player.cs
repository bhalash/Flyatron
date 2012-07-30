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
		enum Playerstate { Alive, Dead };
		Playerstate state;

		// Flames under the player.
		Stopwatch exhaust;

		// Default keys are WSAD, but are changable via Rebind().
		Keys up = Keys.W;
		Keys down = Keys.S;
		Keys left = Keys.A;
		Keys right = Keys.D;

		List<Texture2D> textures;

		Vector2[] vectors = new Vector2[]
		{		
			// Vectors should be updated relative to index 0.
			new Vector2(x + frameOffset[0], y + frameOffset[1]),
			new Vector2(x + frameOffset[2], y + frameOffset[3]),
			new Vector2(x + frameOffset[4], y + frameOffset[5]),
		};

		Rectangle[] rectangles = new Rectangle[]
		{
			new Rectangle(0,0,35,72), // Body.
			new Rectangle(0,0,35,35), // Head.
			new Rectangle(0,0,23,46)  // Flames.
		};

		float scale = 0.7F; 

		static int[] frames = new int[] { 35, 72, 35, 35, 23, 46 };
		static int[] frameOffset = new int[] { 0, 0, 13, 13, 4, 40 };

		static int x = 400;
		static int y = 300;

		// Player speed.
		int lives, currentLives, velocity, walkingVel;

		// These are the centre of the respective texture frames, used to correctly rotate them.
		Vector2 headOffset = new Vector2(17.5F, 17.5F);

		float headRotation = 0;

		public Player(int inputLives, int inputVelocity,Color inputTint, List<Texture2D> inTex)
		{
			textures = inTex;
			walkingVel = inputVelocity;
			velocity = walkingVel;
			lives = currentLives = inputLives;

			exhaust = new Stopwatch();
			exhaust.Start();
		}

		public Vector2 Position()
		{
			return vectors[1];
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
				spriteBatch.Draw(textures[2], vectors[2], rectangles[2], Color.White, 0, new Vector2(0, 0), scale, SpriteEffects.None, 0);
				// Player body.
				spriteBatch.Draw(textures[0], vectors[0], rectangles[0], Color.White, 0, new Vector2(0, 0), scale, SpriteEffects.None, 0);
				// Player head.
				spriteBatch.Draw(textures[1], vectors[1], rectangles[1], Color.White, headRotation, headOffset, scale, SpriteEffects.None, 0);
			}
		}

		private void Dead()
		{
			if (lives > 0)
			{
				Lives(-1);
				X(50);
				Y(Game.HEIGHT / 2 - textures[0].Height / 2);
			}

			state = Playerstate.Alive;
		}

		private void Living()
		{
			UpdateBodyAnimation(Game.currentMouseState);
			UpdateHeadAnimation(Game.currentMouseState);
			UpdateFlamesAnimation();

			if (Game.currentKeyboardState.IsKeyDown(left))
				if (vectors[0].X > 0)
					for (int i = 0; i < vectors.Length; i++)
						vectors[i].X -= velocity;

			if (Game.currentKeyboardState.IsKeyDown(up))
				if (vectors[0].Y > 0)
					for (int i = 0; i < vectors.Length; i++)
						vectors[i].Y -= velocity;

			if (Game.currentKeyboardState.IsKeyDown(right))
				if (vectors[0].X + 30 < Game.WIDTH)
					for (int i = 0; i < vectors.Length; i++)
						vectors[i].X += velocity;

			if (Game.currentKeyboardState.IsKeyDown(down))
				if (vectors[0].Y + 50 < Game.HEIGHT)
					for (int i = 0; i < vectors.Length; i++)
						vectors[i].Y += velocity;
		}

		public Rectangle Rectangle()
		{
			return new Rectangle((int)vectors[0].X, (int)vectors[0].Y, 25, 52);
		}

		public void X(int newX)
		{
			// Streamline.
			vectors[0].X = newX;
			vectors[1].X = newX + 13;
			vectors[2].X = newX + frameOffset[4];
		}

		public void Y(int newY)
		{
			vectors[0].Y = newY;
			vectors[1].Y = newY + 13;
			vectors[2].Y = newY + frameOffset[5];
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

		public void State(int newState)
		{
			if (newState == 1)
				state = Playerstate.Alive;
			if (newState == 2)
				state = Playerstate.Dead;
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
			if ((exhaust.ElapsedMilliseconds >= 0) && (exhaust.ElapsedMilliseconds < 300))
				rectangles[2] = new Rectangle(52, 0, frames[4], frames[5]);
			if ((exhaust.ElapsedMilliseconds >= 300) && (exhaust.ElapsedMilliseconds < 600))
				rectangles[2] = new Rectangle(25, 0, frames[4], frames[5]);
			if ((exhaust.ElapsedMilliseconds >= 600) && (exhaust.ElapsedMilliseconds < 900))
				rectangles[2] = new Rectangle(0, 0, frames[4], frames[5]);

			if (exhaust.ElapsedMilliseconds > 900)
				exhaust.Restart();
		}
	}
}
