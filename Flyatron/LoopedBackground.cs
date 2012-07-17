/*
 * This method should ideally be gracefully handle any number of backdrop layers. 
 * The elaborateness of any backdrop is limited by your ability to create background textures.
 */

using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Flyatron;

namespace Flyatron
{
	class Backdrop
	{
		Texture2D[] texture;
		Vector2[][]	vector;

		int a = 0;
		int b = 0;

		int xBounds;
		int yBounds;
		int layers;

		public Backdrop(Texture2D[] inputTexture, int inputXBounds, int inputYBounds, int inputLayers)
		{
			texture = inputTexture;
			xBounds = inputXBounds;
			yBounds = inputYBounds;
			layers  = inputLayers;
		}

		public void Initialize()
		{
			// TODO: Fix this. Vector arrays are not working.
			vector = new Vector2[layers][];
			texture = new Texture2D[layers];

			for (int i = 0; i < layers; i++)
				vector[i] = new Vector2[2];

			for (int i = 0; i < layers; i++)
			{
				vector[i][0] = new Vector2(0, 0);
				vector[i][1] = new Vector2(texture[i].Width);
			}

			vector[0][0] = new Vector2(0, 0);
		}

		public void Update()
		{
			for (int i = 0; i < layers; i++)
			{
				// This manages right-to-left scrolling.
				// If vector0's texture is completely off the screen, it is moved to the
				// right of vector1's texture.
				if (vector[i][0].X + texture[i].Width <= 0)
					vector[i][0].X = vector[i][1].X + texture[i].Width;
				if (vector[i][1].X + texture[i].Width <= 0)
					vector[i][1].X = vector[i][0].X + texture[i].Width;
			}

			for (int i = 0; i < layers; i++)
			{
				// As above, but works for left-to-right scrolling.
				// Combined, these two loops emulate one infinitely scrolling texture.
				if (vector[i][0].X > texture[i].Width)
					vector[i][0].X = vector[i][1].X - texture[i].Width;
				if (vector[i][1].X > texture[i].Width)
					vector[i][1].X = vector[i][0].X - texture[i].Width;
			}
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			// Examples.
			Vector2 vector1 = new Vector2(0, 0);
			Vector2[] vector3 = new Vector2[1];
			vector3[0] = new Vector2(0, 0);
			// spriteBatch.Draw(texture[0], vector[0][0], Color.White);
			// spriteBatch.Draw(texture[0], vector[0][1], Color.White);
			spriteBatch.Draw(texture[0], vector3[0], Color.White);
			spriteBatch.Draw(texture[0], new Vector2(0,100), Color.White);
		}
	}
}
