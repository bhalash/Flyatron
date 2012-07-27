/*
 * Author: Mark Grealish (mark@bhalash.com)
 * 
 * This method should ideally be gracefully handle any number of backdrop layers. 
 * The elaborateness of any backdrop is limited by your ability to create background textures.
 * Scrolling is either tied to: 
 * 
 *		1. The Update(KeyboardState, int speed) method. The background will move in lockstep with the character, per WSAD.
 *		2. The Demo() method. It will gradually scroll toward the left. Intended for use in game menus.
 *		
 * You call this class pretty simply: Create an array of textures, 0-whatever and pass it to this class
 * along with the vertical size of the screen. 
 *  
 *		Backdrop alpha = new Backdrop(textures, 768);
 *		
 * There is an optional Debug() method. It's just here to track Y positions. Scrolling is infinite in either
 * horizontal direction and seamlessly limited in the vertical based on the *front-most* layer's position.
 *  
 * Enjoy!
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Flyatron
{
	class Backdrop
	{
		Texture2D[] texture;
		Vector2[][]	vector;

		int layers;

		public Backdrop(Texture2D[] inputTexture)
		{
			texture = inputTexture;
			layers  = inputTexture.Length;

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

		public void Update(int speed)
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

				speed += 2;
			}
		}

		public void ScrollRight(int speed)
		{
			for (int i = 0; i < layers; i++)
			{
				for (int j = 0; j < 2; j++)
					vector[i][j].X += speed;

				speed += 2;
			}
		}

		public void ScrollUp(int speed)
		{
			for (int i = 0; i < layers; i++)
			{
				for (int j = 0; j < 2; j++)
					if (vector[2][j].Y > 0)
						vector[i][j].Y -= speed;

				speed += 2;
			}
		}

		public void ScrollDown(int speed)
		{
			// *.Y bounding is handled implicitly, bassed upon the position of
			//  the frontmost (read: last in array) layer. 
			// Vertical moves only occur if it would not move the front layer
			// off of the screen in either direction.
			for (int i = 0; i < layers; i++)
			{
				for (int j = 0; j < 2; j++)
					if (vector[2][j].Y < Game.HEIGHT)
						vector[i][j].Y += speed;

				speed += 2;
			}
		}

		public void Debug(SpriteFont font, SpriteBatch spriteBatch, int x, int y)
		{
			List<string> debug = new List<string>()
			{
				// Put 170.X between duplicate debug panels.
				"vector[0][0].Y: " + vector[0][0].Y,
				"vector[1][0].Y: " + vector[1][0].Y,
				"vector[2][0].Y: " + vector[2][0].Y,
			};

			for (int i = 0; i < debug.Count; i++)
			{
				spriteBatch.DrawString(font, debug[i], new Vector2(x, y), Color.Black);
				y += 15;
			}
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
