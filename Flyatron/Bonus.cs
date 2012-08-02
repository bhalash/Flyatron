using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;

namespace Flyatron
{
	class Bonus
	{
		// Animate.
		Stopwatch scaleTimer;
		// Respawn.
		Stopwatch halt;
		// Explosion.
		Stopwatch expTimer;

		int frameW, frameH;

		// Current state of the mine.
		enum BonusState { Halted, Traverse };
		BonusState state = BonusState.Traverse;
		// Type.
		enum BonusType { Life, Nuke };
		BonusType type;

		// Mine traverse speed.
		float velocity;

		// Animation: Texture, vector, rotation offset, and frame rectangle.
		Texture2D[] texture;
		Rectangle[] rectangle;
		Vector2[] vector;

		// Rotation.
		float angle;
		float scale;

		// Halt/loop timer.
		int haltDuration;

		// Reference vector (for animaiton/collision).
		Rectangle reference;

		public Bonus(Texture2D[] inputTexture)
		{
			texture = inputTexture;
			velocity = 4;

			frameW = 49;
			frameH = 49;

			vector = new Vector2[]
			{
				new Vector2(0 - frameW, Helper.Rng(Game.HEIGHT - frameH)),
				new Vector2(24.5F, 24.5F)
			};

			angle = 0;
			scale = 1;

			rectangle = new Rectangle[]
			{
				new Rectangle(0, 0,  49, 49),
				new Rectangle(0, 0, 125, 125)
			};

			// Animation timer.
			scaleTimer = new Stopwatch();
			scaleTimer.Start();
			// Respawn timer.
			halt = new Stopwatch();
			// Explosion.
			expTimer = new Stopwatch();
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if (state == BonusState.Traverse)
				switch (type)
				{
					case BonusType.Life:
						{
							for (int i = 0; i < 2; i++)
								spriteBatch.Draw(
									texture[i],
									vector[0],
									rectangle[0],
									Color.White,
									angle,
									vector[1],
									scale,
									SpriteEffects.None,
									0
								);

							break;
						}
					case BonusType.Nuke:
						{
							for (int i = 2; i < 4; i++)
								spriteBatch.Draw(
									texture[i],
									vector[0],
									rectangle[0],
									Color.White,
									angle,
									vector[1],
									scale,
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
				case (BonusState.Halted):
					{
						Halt();
						break;
					}
				case (BonusState.Traverse):
					{
						Traverse();
						break;
					}
			}
		}

		private void Traverse()
		{
			Animate();

			// Traverse left.
			vector[0].X -= velocity;

			// Player/bonus collision.
			if (Helper.CircleCollision(Rectangle(), reference))
				State(2);

			// Check if it needs to be drawn.
			if (vector[0].X + frameW < 0)
				state = BonusState.Halted;
		}

		private void Halt()
		{
			if (!halt.IsRunning)
				halt.Start();

			if (Helper.Rng(31337) % 2 == 0)
				type = BonusType.Nuke;
			else
				type = BonusType.Life;

			vector[0].X = Game.WIDTH + frameW;
			vector[0].Y = Helper.Rng(Game.HEIGHT - frameH);

			haltDuration = Helper.Rng2(10000,20000);

			// Check if it needs to be drawn.
			if (halt.ElapsedMilliseconds > haltDuration)
			{
				state = BonusState.Traverse;
				halt.Reset();
			}
		}

		private void Animate()
		{
			float elapsed = scaleTimer.ElapsedMilliseconds;

			if (!scaleTimer.IsRunning)
				scaleTimer.Start();

			if ((elapsed >= 0) && (elapsed <= 300))
				scale = 1 - elapsed * 0.001F;

			if (elapsed > 300)
			{
				scaleTimer.Reset();
				scale = 1;
			}				
		}

		public Rectangle Rectangle()
		{
			return new Rectangle((int)vector[0].X - frameW / 2, (int)vector[0].Y - frameH / 2, frameW, frameH);
		}

		public Vector2 Position()
		{
			return vector[0];
		}

		public void State(int newState)
		{
			if (newState == 1)
				state = BonusState.Traverse;
			if (newState == 2)
				state = BonusState.Halted;
		}

		public int Type()
		{
			if (type == BonusType.Life)
				return 1;

			else return 2;
		}

		public void X(int newX)
		{
			vector[0].X = newX;
		}

		public void Y(int newY)
		{
			vector[0].Y = newY;
		}
	}
}
