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

		Keys left = Keys.A;
		Keys right = Keys.D;
		Keys down = Keys.S;
		Keys up = Keys.W;
		
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

			// sep indicated the vectical separation from 0. For my specific cloud textures, a division of 
			// 100 pixels per layer is a solid start.

			int sep = 100;
			for (int i = 0; i < layers; i++)
			{
				vector[i][0] = new Vector2(0,sep);
				vector[i][1] = new Vector2(texture[i].Width, sep);
				sep += 100; 
			}
		}

		public void Rebind(Keys newUp, Keys newDown, Keys newRight, Keys newLeft)
		{
			up = newUp;
			down = newDown;
			left = newLeft;
			right = newRight;
		}

		public void Update(KeyboardState inputState, int speed)
		{
			currentState = inputState;
			// Scroll each of the textures. 
			LoopLayers();
			// Enforce Y-axis bounding to stop the layers scrolling off the screen.
			Bounding(yBounds);

			if (currentState.IsKeyDown(up))
				ScrollUp(speed);
			if (currentState.IsKeyDown(down))
				ScrollDown(speed);
			if (currentState.IsKeyDown(left))
				ScrollLeft(speed);
			if (currentState.IsKeyDown(right))
				ScrollRight(speed);
	
			previousState = currentState;
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			for (int i = 0; i < layers; i++)
				for (int j = 0; j < 2; j++)
					spriteBatch.Draw(texture[i], vector[i][j], Color.White);
		}

		public void ScrollLeft(int speed)
		{
			for (int i = 0; i < layers; i++)
			{
				for (int j = 0; j < 2; j++)
					vector[i][j].X -= speed;

				speed += speed / 4;
			}
		}

		public void ScrollRight(int speed)
		{
			for (int i = 0; i < layers; i++)
			{
				for (int j = 0; j < 2; j++)
					vector[i][j].X += speed;

				speed += speed / 4;
			}
		}

		public void ScrollUp(int speed)
		{
			for (int i = 0; i < layers; i++)
			{
				for (int j = 0; j < 2; j++)
					vector[i][j].Y -= speed;

				speed += speed / 4;
			}
		}

		public void ScrollDown(int speed)
		{
			for (int i = 0; i < layers; i++)
			{
				for (int j = 0; j < 2; j++)
					vector[i][j].Y += speed;

				speed += speed / 4;
			}
		}

		public void Bounding(int yBound)
		{
			// TODO.
		}

		public void Demo(int speed)
		{
			// Demo() is intended for use on menu screens. 
			for (int i = 0; i < layers; i++)
			{
				for (int j = 0; j < 2; j++)
					vector[i][j].X -= speed;

				speed++;
			}

			LoopLayers();
		}

		public void LoopLayers()
		{
			// This method silently manages the scrolling background. 
			// If either texture is completely off the screen to either side, it is 
			// moved to the far side of the on-screen texture, dependent on which
			// direction is being scrolled. 
			for (int i = 0; i < layers; i++)
			{
				if (vector[i][0].X + texture[i].Width <= 0)
					vector[i][0].X = vector[i][1].X + texture[i].Width;
				if (vector[i][1].X + texture[i].Width <= 0)
					vector[i][1].X = vector[i][0].X + texture[i].Width;
			}

			for (int i = 0; i < layers; i++)
			{
				if (vector[i][0].X > texture[i].Width)
					vector[i][0].X = vector[i][1].X - texture[i].Width;
				if (vector[i][1].X > texture[i].Width)
					vector[i][1].X = vector[i][0].X - texture[i].Width;
			}
		}
	}
}
