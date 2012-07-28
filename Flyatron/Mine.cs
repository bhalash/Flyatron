using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using Flyatron;

namespace Flyatron
{
	class Mine
	{
		Stopwatch rotation;
		Stopwatch halt;
		
		int haltDuration;

		enum Minestate { Halted, Mobile };
		Minestate state = Minestate.Mobile;

		float[] angle = new float[] {0,0};

		float velocity;

		Texture2D[] texture;
		Vector2 vector;
		Vector2 offset;
		Rectangle[] rectangle;

		float distance;

		public Mine(Texture2D[] inputTextures, float inputVelocity)
		{
			texture = inputTextures;
			velocity = inputVelocity;

			vector  = new Vector2(0 - texture[0].Width, Helper.Rng(0,Game.HEIGHT - texture[0].Height));
			offset  = new Vector2(20, 20);

			rectangle = new Rectangle[]
				{
					new Rectangle(0, 0, 40, 40),
					new Rectangle(0, 0, 40, 40),
				};

			rotation = new Stopwatch();
			rotation.Start();
			halt = new Stopwatch();
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			for (int i = 0; i < texture.Length; i++)
				spriteBatch.Draw(texture[i], vector, rectangle[i], Color.White, angle[i], offset, 1, SpriteEffects.None, 0);
		}

		public void Update()
		{
			switch (state)
			{
				case (Minestate.Halted):
					{
						MineHalted();
						break;
					}
				case (Minestate.Mobile):
					{
						MineMobile();
						break;
					}
			}
		}

		private void MineMobile()
		{
			UpdateAnimation();

			distance = Vector2.Distance(vector, reference);

			vector.X -= velocity;

			if ((vector.X + texture[0].Width < 0) || (distance < 35))
				Halt();
		}

		public void Halt(int seconds = 3)
		{
			vector.X = Game.WIDTH + texture[0].Width;
			vector.Y = Helper.Rng(0, Game.WIDTH - texture[0].Height);
			state = Minestate.Halted;
			haltDuration = Helper.Rng(0, seconds * 1000);
			halt.Restart();
		}

		private void MineHalted()
		{
			if (halt.ElapsedMilliseconds > haltDuration)
				state = Minestate.Mobile;
		}

		public Rectangle Rectangle()
		{
			return new Rectangle((int)vector.X - 20, (int)vector.Y - 20, 40, 40);
		}

		public Vector2 Position()
		{
			return vector;
		}

		private void UpdateAnimation()
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
	}
}
