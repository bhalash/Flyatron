using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace Flyatron
{
	class Missile
	{
		Texture2D[]	texture;
		Vector2[]   vector;
		Rectangle[] rectangle;
		int frameX, frameY, frameW, frameH;
		float rotation, scale;

		Color color;

		// Debug.
		Stopwatch kill;

		public Missile(Texture2D[] newTexture, Vector2 newVector)
		{
			color = Color.White;

			rotation = 0;
			scale = 1;

			frameX = 0;
			frameY = 59;
			frameW = 55;
			frameH = 11;

			texture = newTexture;

			vector = new Vector2[2]
			{
				newVector,
				new Vector2(0,0)
			};

			rectangle = new Rectangle[2]
			{
				// Movement frame.
				new Rectangle((int)vector[0].X, (int)vector[0].Y, frameW, frameH),
				// Animation frame.
				new Rectangle(frameX, frameY, frameW, frameH)
			};

			kill = new Stopwatch();
			kill.Start();
		}

		public void Update()
		{
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture[2], vector[0], rectangle[0], color, rotation, vector[1], scale, SpriteEffects.None,0F);
		}

		public bool Expired()
		{
			if (kill.ElapsedMilliseconds > 2000)
				return true;

			return false;
		}

		public Vector2 Position()
		{
			return vector[0];
		}
	}
}
