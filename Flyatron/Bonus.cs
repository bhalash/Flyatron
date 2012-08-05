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
		enum Bonusstate { Halted, Traverse }; Bonusstate state;
		enum Bonustype { Life, Nuke }; Bonustype type;

		float scale, velocity;
		Stopwatch scaleTimer, halt;
		int haltDuration;
		Texture2D bonusTexture;
		Vector2 bonusPosition, rotationOffset;
		Rectangle bonus;
		Color color;
		SpriteEffects effects;

		// Debug messages.
		string dString1, dString2;
		Vector2 dPosition1, dPosition2;

		public Bonus(Texture2D inputTexture)
		{
			state = Bonusstate.Traverse;

			bonusTexture = inputTexture;

			bonus = new Rectangle(0, 0, 49, 49);

			color = Color.White;
			effects = SpriteEffects.None;

			velocity = 4;
			scale = 1;
			
			bonusPosition = new Vector2(0 - bonus.Width, Helper.Rng(Game.HEIGHT - bonus.Height));
			rotationOffset = new Vector2(24.5F, 24.5F);

			// Animation timer.
			scaleTimer = new Stopwatch();
			scaleTimer.Start();
			// Respawn timer.
			halt = new Stopwatch();

			if (Game.DEBUG)
			{
				dString1 = dString2 = "";
				dPosition1 = dPosition2 = new Vector2(0, 0);
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if (state == Bonusstate.Traverse)
			{
				spriteBatch.Draw(bonusTexture, bonusPosition, bonus, color, 0, rotationOffset, scale, effects, 0);

				if (Game.DEBUG)
				{
					spriteBatch.DrawString(Game.FONT07, dString1, dPosition1, Color.Black);
					spriteBatch.DrawString(Game.FONT07, dString2, dPosition2, Color.Black);
				}
			}
		}

		public void Update()
		{
			switch (state)
			{
				case (Bonusstate.Halted):
					{
						Halt();
						break;
					}
				case (Bonusstate.Traverse):
					{
						Traverse();
						break;
					}
			}
		}

		private void Traverse()
		{
			Animate();

			if (Game.DEBUG)
			{
				dString1 = "Type: " + type;
				dString2 = "X: " + bonusPosition.X + " " + "Y: " + bonusPosition.Y;

				dPosition1 = new Vector2(bonusPosition.X + bonus.Width - 20, bonusPosition.Y - 22);
				dPosition2 = new Vector2(bonusPosition.X + bonus.Width - 20, bonusPosition.Y - 7);
			}

			// Traverse left.
			bonusPosition.X -= velocity;

			// Check if it needs to be drawn.
			if (bonusPosition.X + bonus.Width < 0)
				state = Bonusstate.Halted;
		}

		private void Halt()
		{
			if (!halt.IsRunning)
				halt.Start();

			if (Helper.Rng(31337) % 2 == 0)
				type = Bonustype.Nuke;
			else
				type = Bonustype.Life;

			bonusPosition.X = Game.WIDTH + bonus.Width;
			bonusPosition.Y = Helper.Rng(Game.HEIGHT - bonus.Height);

			haltDuration = Helper.Rng2(10000,20000);

			if (Game.DEBUG)
				haltDuration = 1;

			// Check if it needs to be drawn.
			if (halt.ElapsedMilliseconds > haltDuration)
			{
				state = Bonusstate.Traverse;
				halt.Reset();
			}
		}

		private void Animate()
		{
			float elapsed = scaleTimer.ElapsedMilliseconds;

			if (!scaleTimer.IsRunning)
				scaleTimer.Start();

			if (type == Bonustype.Life)
				bonus.X = 0;
			else
				bonus.X = 53;

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
			return new Rectangle((int)bonusPosition.X - bonus.Width / 2, (int)bonusPosition.Y - bonus.Height / 2, bonus.Width, bonus.Height);
		}

		public Vector2 Position()
		{
			return bonusPosition;
		}

		public void State(int newState)
		{
			if (newState == 1)
				state = Bonusstate.Traverse;
			if (newState == 2)
				state = Bonusstate.Halted;
		}

		public int ReportType()
		{
			if (type == Bonustype.Life)
				return 1;

			else return 2;
		}

		public int ReportState()
		{
			if (type == Bonustype.Life)
				return 1;

			else return 2;
		}

		public void X(int newX)
		{
			bonusPosition.X = newX;
		}

		public void Y(int newY)
		{
			bonusPosition.Y = newY;
		}
	}
}
