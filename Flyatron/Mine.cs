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
		enum Minestate { Halted, Traverse, Explosion, Shot };
		Minestate state;

		Vector2 minePosition, explosionPosition, rotationOffset;
		Rectangle core, spikes, explosion, reference;
		Texture2D coreTexture, spikeTexture, explosionTexture, borderTexture;
		Stopwatch rotation, halt, explosionTimer, shotTimer;

		SpriteEffects effects;

		float baseVelocity, velocity, velocityMod;
		float haltDuration, distance;
		float mineScale, angle, angleDiff;
		float explosionScale, explosionOpacity;

		int health;

		// Debug messages.
		string dString1, dString2;
		Vector2 dPosition1, dPosition2;

		Color color;

		public Mine(Texture2D[] inputTexture)
		{
			state = Minestate.Halted;
			color = Color.White;

			coreTexture		  = inputTexture[0];
			spikeTexture	  = inputTexture[1];
			explosionTexture = inputTexture[2];
			borderTexture    = inputTexture[3];

			core = new Rectangle(0, 0, 40, 40);
			spikes = new Rectangle(90, 0, 40, 40);
			explosion = new Rectangle(0, 0, 125, 125);

			angle = 0;
			angleDiff = 0.3F;

			mineScale = 1;
			health = 100;

			// Velocity is randomized within a small range to present more challenge to the player.
			baseVelocity = 3; velocityMod = 7;

			velocity = baseVelocity + Helper.Rng((int)velocityMod);

			minePosition = new Vector2(0 - coreTexture.Width, 0 - coreTexture.Height);
			rotationOffset = new Vector2(20, 20);

			// Animation timer.
			rotation = new Stopwatch(); rotation.Start();
			// Respawn timer.
			halt = new Stopwatch();
			// Explosion.
			explosionTimer = new Stopwatch();
			// Shot animation.
			shotTimer = new Stopwatch();

			effects = SpriteEffects.None;

			if (Game.DEBUG)
			{
				velocity = 3;
				dString1 = dString2 = "";
				dPosition1 = dPosition2 = new Vector2(0, 0);
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if (Game.DEBUG)
			{
				spriteBatch.DrawString(Game.FONT07, dString1, dPosition1, Color.Black);
				spriteBatch.DrawString(Game.FONT07, dString2, dPosition2, Color.Black);
			}

			// if was more compact than a switch, here.
			if ((state == Minestate.Traverse) || (state == Minestate.Shot))
			{
				spriteBatch.Draw(coreTexture,  minePosition, core,  color, angle, rotationOffset, mineScale, effects, 0);
				spriteBatch.Draw(spikeTexture, minePosition, spikes, color, 360 - angle, rotationOffset, mineScale, effects, 0);

				if (Game.DEBUG)
					spriteBatch.Draw(borderTexture, minePosition, core, color * 0.3F, angle, rotationOffset, mineScale, effects, 0);
			}

			if (state == Minestate.Explosion)			
				spriteBatch.Draw(explosionTexture, explosionPosition, explosion, color * explosionOpacity, 0, rotationOffset, explosionScale, effects, 0);
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
				case (Minestate.Shot):
					{
						Shot();
						break;
					}
			}
		}

		private void Shot()
		{
			Animate();

			if (!shotTimer.IsRunning)
			{
				shotTimer.Restart();
				// Health decreases inverse to speed: Slower mines take less damage. TODO: Tweak.
				health -= (int)((velocity * (velocity * 0.3F)) * 5);
			}

			// Proximity.
			distance = Vector2.Distance(minePosition, new Vector2(reference.X + (reference.Width / 2), reference.Y + (reference.Height / 2)));

			// Traverse right.
			minePosition.X += baseVelocity * 2;

			if (shotTimer.ElapsedMilliseconds > 180)
			{
				if (health <= 0)
					state = Minestate.Explosion;
				else 
					state = Minestate.Traverse;

				shotTimer.Stop();
			}
		}

		private void Traverse()
		{
			Animate();

			if (Game.DEBUG)
			{
				dString1 = "H: " + health + " " + "V: " + velocity;
				dString2 = "X: " + minePosition.X + " " + "Y: " + minePosition.Y;

				dPosition1 = new Vector2(minePosition.X + core.Width + 1, minePosition.Y - 14);
				dPosition2 = new Vector2(minePosition.X + core.Width + 1, minePosition.Y + 2);
			}

			// Proximity.
			distance = Vector2.Distance(minePosition, new Vector2(reference.X + (reference.Width / 2), reference.Y + (reference.Height / 2)));

			// Traverse left.
			minePosition.X -= velocity;

			// Check if it needs to be drawn.
			if (minePosition.X + core.Width < 0)
				state = Minestate.Halted;
		}

		private void Halt()
		{
			if (!halt.IsRunning)
				halt.Start();

			minePosition.X = Game.WIDTH + core.Width;
			minePosition.Y = Helper.Rng(Game.HEIGHT - spikes.Height);

			// Randomize speed for the next run.
			if (!Game.DEBUG)
				velocity = baseVelocity + Helper.Rng((int)velocityMod);
			if (Game.DEBUG)
				velocity = 3;

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
			float elapsed = explosionTimer.ElapsedMilliseconds * 0.002F;

			// Keep the explosion centered on the mine's position.
			explosionPosition.X = minePosition.X - spikes.Width  / 3 * explosionScale;
			explosionPosition.Y = minePosition.Y - spikes.Height / 3 * explosionScale;

			if (!explosionTimer.IsRunning)
				explosionTimer.Restart();

			explosionScale = elapsed;
			explosionOpacity = 1 - elapsed;

			if (explosionTimer.ElapsedMilliseconds >= 500)
			{
				explosionTimer.Stop();
				state = Minestate.Halted;
			}
		}

		private void Animate()
		{
			if (health < 66)
			{
				spikes.X = 90;
				angleDiff = 0.3F;
			}
			if ((health > 33) && (health <= 66))
			{
				spikes.X = 44;
				angleDiff = 0.5F;
			}
			if (health <= 33)
			{
				spikes.X = 0;
				angleDiff = 0.7F;
			}

			if (rotation.ElapsedMilliseconds >= 20F)
			{
				angle +=  angleDiff;
				rotation.Restart();
			}

			if (distance >= 200)
				core.X = 0;
			if ((distance < 200) && (distance >= 100))
				core.X = 40;
			if (distance < 100)
				core.X = 80;

			if (angle >= 360)
				angle = 0;
		}

		public Rectangle Rectangle()
		{
			return new Rectangle((int)minePosition.X - 20, (int)minePosition.Y - 20, spikes.Width, spikes.Height);
		}

		public Vector2 Position()
		{
			return minePosition;
		}

		public int ReportState()
		{
			if (state == Minestate.Traverse)
				return 1;
			if (state == Minestate.Explosion)
				return 2;
			if (state == Minestate.Shot)
				return 4;

			// If halted.
			else return 3;
		}

		public void State(int newState)
		{
			if (newState == 1)
				state = Minestate.Traverse;
			if (newState == 2)
				state = Minestate.Halted;
			if (newState == 3)
				state = Minestate.Explosion;
			if (newState == 4)
				state = Minestate.Shot;
		}

		public void X(int newX)
		{
			minePosition.X = newX;
		}

		public void Y(int newY)
		{
			minePosition.Y = newY;
		}
	}
}
