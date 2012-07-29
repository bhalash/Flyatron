using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;

namespace Flyatron
{
	class Mine
	{
		// Animate.
		Stopwatch rotation;
		// Respawn.
		Stopwatch halt;
		// Explosion.
		Stopwatch explosion;

		// Current state of the mine.
		enum Minestate { Halted, Traverse, Explosion };
		Minestate state = Minestate.Traverse;

		// Mine traverse speed.
		float velocity = 25;

		// Animation: Texture, vector, rotation offset, and frame rectangle.
		Texture2D[] texture;
		Vector2 vector;
		Vector2 offset;
		Rectangle[] rectangle;
		// Rotation.
		float[] angle = new float[] { 0, 0 };

		// Explosion.
		float expOpacity;
		float expScale;
		Vector2 expVector;

		// Halt/loop timer.
		int haltDuration;
		int seconds = 1;

		// Reference vector (for animaiton/collision).
		Vector2 reference;
		// Distance to reference vector.
		float distance;

		public Mine(Texture2D[] inputTextures)
		{
			texture = inputTextures;

			vector = new Vector2(0 - texture[0].Width, Helper.Rng(0, Game.HEIGHT - texture[0].Height));
			offset = new Vector2(20, 20);

			rectangle = new Rectangle[]
				{
					new Rectangle(0, 0, 40, 40),
					new Rectangle(0, 0, 40, 40),
					new Rectangle(0, 0, 125, 125)
				};

			rotation = new Stopwatch();
			rotation.Start();
			halt = new Stopwatch();
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if (state == Minestate.Traverse)
				for (int i = 0; i < texture.Length - 1; i++)
					spriteBatch.Draw(texture[i], vector, rectangle[i], Color.White, angle[i], offset, 1, SpriteEffects.None, 0);

			if (state == Minestate.Explosion)
				spriteBatch.Draw(texture[2], expVector, rectangle[2], Color.White * expOpacity, 0, offset, expScale, SpriteEffects.None, 0);
		}

		public void Update(Vector2 newReference)
		{
			reference = newReference;

			switch (state)
			{
				case (Minestate.Halted):
					{
						Halt();
						break;
					}
				case (Minestate.Traverse):
					{
						Traverse();
						break;
					}
				case (Minestate.Explosion):
					{
						Explosion();
						break;
					}
			}
		}

		private void Traverse()
		{
			Animate();

			distance = Vector2.Distance(vector, reference);

			// Traverse left.
			vector.X -= velocity;

			// Check if it needs to be drawn.
			if (vector.X + texture[0].Width < 0)
			{
				state = Minestate.Halted;
				halt.Start();
			}
		}

		private void Halt()
		{
			vector.X = Game.WIDTH + texture[0].Width;
			vector.Y = Helper.Rng(0, Game.WIDTH - texture[0].Height);

			haltDuration = Helper.Rng(0, seconds * 1000);

			// Check if it needs to be drawn.
			if (halt.ElapsedMilliseconds > haltDuration)
			{
				state = Minestate.Traverse;
				halt.Reset();
			}
		}

		private void Explosion()
		{
			float elapsed = expTimer.ElapsedMilliseconds * 0.001F;

			expVector.X = vector.X - texture[2].Width / 3 * expScale;
			expVector.Y = vector.Y - texture[2].Height / 3 * expScale;

			if (!expTimer.IsRunning)
				expTimer.Restart();

			expScale = elapsed;
			expOpacity = 1 - elapsed;

			if (expTimer.ElapsedMilliseconds >= 1000)
			{
				expTimer.Stop();
				state = Minestate.Halted;
			}
		}

		private void Animate()
		{
			if (rotation.ElapsedMilliseconds >= 20F)
			{
				angle[0] += 0.3F;
				angle[1] -= 1;
				rotation.Restart();
			}

			if (distance >= 200)
				rectangle[0].X = 0;
			if ((distance < 200) && (distance >= 100))
				rectangle[0].X = 40;
			if (distance < 100)
				rectangle[0].X = 80;

			if (angle[0] >= 360)
				angle[0] = 0;
			if (angle[1] >= 360)
				angle[1] = 0;
		}

		public Rectangle Rectangle()
		{
			// Rectangles for the explosion animation are coming back all fucked. 
			// I'm not going to worry about it too much because the player has already died at that point.
			return new Rectangle((int)vector.X - 20, (int)vector.Y - 20, rectangle[0].Width, rectangle[0].Height);
		}

		public Vector2 Position()
		{
			return vector;
		}

		// DEBUG
		public void Switch(int newState)
		{
			if (newState == 1)
				state = Minestate.Traverse;
			if (newState == 2)
				state = Minestate.Halted;
			if (newState == 3)
				state = Minestate.Explosion;
		}

		public void X(int newX)
		{
			vector.X = newX;
		}

		public void Y(int newY)
		{
			vector.Y = newY;
		}
	}
}
