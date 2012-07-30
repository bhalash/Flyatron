using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Flyatron
{
	class Bullet
	{
		Texture2D texture;
		Vector2 vector;

		enum Bulletstate { Idle, Firing };
		Bulletstate state = Bulletstate.Firing;

		public Bullet(Texture2D newTexture)
		{
			texture = newTexture;

			vector = new Vector2(100, 100);
		}

		public void Update()
		{
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if (state == Bulletstate.Firing)
				spriteBatch.Draw(texture, Rectangle(), Color.White);
		}

		public void Reload()
		{
		}

		public Rectangle Rectangle()
		{
			return new Rectangle((int)vector.X, (int)vector.Y, 16, 16);
		}

		public void State(int newState)
		{
			if (newState == 1)
				state = Bulletstate.Idle;
			if (newState == 2)
				state = Bulletstate.Firing;
		}
	}
}
