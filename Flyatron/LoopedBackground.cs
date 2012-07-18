/*
 * This method should ideally be gracefully handle any number of backdrop layers. 
 * The elaborateness of any backdrop is limited by your ability to create background textures.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BetterBackgrounds
{
	class Backdrop
	{
		KeyboardState currentState, previousState;

		Texture2D[] texture;
		Vector2[][]	vector;
		
		int xBounds;
		int yBounds;
		int layers;

		public Backdrop(Texture2D[] inputTexture, int inputXBounds, int inputYBounds, int inputLayers)
		{
			texture = inputTexture;
			xBounds = inputXBounds;
			yBounds = inputYBounds;
			layers  = inputLayers;

			// I need to explicitly initialize all of the vector arrays here, or the content
			// is not correctly detected by the other dependent methods.
			vector = new Vector2[layers][];

			for (int i = 0; i < layers; i++)
				vector[i] = new Vector2[2];

			for (int i = 0; i < layers; i++)
			{
				vector[i][0] = new Vector2(0,0);
				vector[i][1] = new Vector2(texture[i].Width, 0);
			}
		}

		public void Update(KeyboardState inputKeyboardState)
		{
			currentState = inputKeyboardState;

			// Testing.
			if (Keyboard.GetState().IsKeyDown(Keys.A))
			{
				vector[0][0].X -= 10;
				vector[0][1].X -= 10;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.D))
			{
				vector[0][0].X += 10;
				vector[0][1].X += 10;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.W))
			{
				vector[0][0].Y -= 10;
				vector[0][1].Y -= 10;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.S))
			{
				vector[0][0].Y += 10;
				vector[0][1].Y += 10;
			}

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

			previousState = currentState;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(texture[0], vector[0][0], Color.White);
			spriteBatch.Draw(texture[0], vector[0][1], Color.White);
		}
	}
}
