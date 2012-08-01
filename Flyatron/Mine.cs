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
		Stopwatch expTimer;

		// Health.
		int health;

		// Current state of the mine.
		enum Minestate { Halted, Traverse, Explosion };
		Minestate state = Minestate.Traverse;

		// Mine traverse speed.
		float velocity, baseVelocity;
		int variableVelocity;

		// Animation: Texture, vector, rotation offset, and frame rectangle.
		Texture2D[] texture;
		Vector2 vector;
		Vector2 offset;
		Rectangle[] rectangle;
		// Rotation.
		float[] angle;

		// Explosion.
		float expOpacity;
		float expScale;
		Vector2 expVector;

		// Halt/loop timer.
		int haltDuration;

		// Reference vector (for animaiton/collision).
		Rectangle reference;
		// Distance to reference vector.
		float distance;

		public Mine(Texture2D[] inputTextures)
		{
			texture = inputTextures;
			angle = new float[] { 0, 0 };
			baseVelocity = 3;
			variableVelocity = 7;
			velocity = baseVelocity + Helper.Rng(variableVelocity);
			vector = new Vector2(0 - texture[0].Width, Helper.Rng(Game.HEIGHT - texture[0].Height));
			offset = new Vector2(20, 20);
			health = 100;

			rectangle = new Rectangle[]
				{
					new Rectangle(0, 0, 40, 40),
					new Rectangle(0, 0, 40, 40),
					new Rectangle(0, 0, 125, 125)
				};

			// Animation timer.
			rotation = new Stopwatch();
			rotation.Start();
			// Respawn timer.
			halt = new Stopwatch();
			// Explosion.
			expTimer = new Stopwatch();
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			switch (state)
			{
				case Minestate.Traverse:
					{
						for (int i = 0; i < texture.Length - 1; i++)
							spriteBatch.Draw(
								texture[i],
								vector,
								rectangle[i],
								Color.White,
								angle[i],
								offset,
								1,
								SpriteEffects.None,
								0
							);

						break;
					}

				case Minestate.Explosion:
					{
						if (state == Minestate.Explosion)
							spriteBatch.Draw(
								texture[2],
								expVector,
								rectangle[2],
								Color.White * expOpacity,
								0,
								offset,
								expScale,
								SpriteEffects.None,
								0
							);

						break;
					}
			}

		}

		public void Update(Rectangle newReference)
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

			// Proximity.
			distance = Vector2.Distance(vector, new Vector2(reference.X + (reference.Width / 2), reference.Y + (reference.Height / 2)));

			// Player mine collision.
			if (Helper.CircleCollision(Rectangle(), reference))
				State(3);

			// Traverse left.
			vector.X -= velocity;

			// Check if it needs to be drawn.
			if (vector.X + texture[0].Width < 0)
				state = Minestate.Halted;
		}

		private void Halt()
		{
			if (!halt.IsRunning)
				halt.Start();

			vector.X = Game.WIDTH + texture[0].Width;
			vector.Y = Helper.Rng(Game.HEIGHT - texture[0].Height);

			// Randomize speed for the next run.
			velocity = baseVelocity + Helper.Rng(variableVelocity);

			haltDuration = Helper.Rng(10000);

			// Check if it needs to be drawn.
			if (halt.ElapsedMilliseconds > haltDuration)
			{
				state = Minestate.Traverse;
				halt.Reset();
			}
		}

		private void Explosion()
		{
			float elapsed = expTimer.ElapsedMilliseconds * 0.002F;

			// Bump score 10 points.
			Scoreboard.Bump(10);

			expVector.X = vector.X - texture[2].Width / 3 * expScale;
			expVector.Y = vector.Y - texture[2].Height / 3 * expScale;

			if (!expTimer.IsRunning)
				expTimer.Restart();

			expScale = elapsed;
			expOpacity = 1 - elapsed;

			if (expTimer.ElapsedMilliseconds >= 500)
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
		public void State(int newState)
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
