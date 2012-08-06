using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using Flyatron;

namespace Flyatron
{
	class Player
	{
		Keys up, down, left, right;
		Vector2 headPosition, bodyPosition, firePosition;
		Texture2D headTexture, bodyTexture, fireTexture;
		Vector2 headOffset;
		Rectangle body, head, fire;
		int lives, currentLives, headXOff, headYOff, fireXOff, fireYOff;
		float scale, headRotation, velocity;
		Stopwatch exhaust;
		Color color;

		enum Playerstate { Alive, Dead };
		Playerstate state;

		// Debug messages.
		string dString1, dString2;
		Vector2 dPosition1, dPosition2;

		public Player(Texture2D[] newTexture)
		{
			velocity = 8;
			lives = currentLives = 0;
			color = Color.White;
			lives = 5;
			scale = 0.7F; 

			// Default keys are WSAD, but are changable via Rebind().
			up = Keys.W; down = Keys.S; left = Keys.A; right = Keys.D;

			// Fire and gun offset relative to the body.
			headXOff = headYOff = 13;
			fireXOff = 04; fireYOff = 40;

			// Animation rectangle.
			body = new Rectangle(0, 0, 35, 72);
			head = new Rectangle(0, 0, 35, 35);
			fire = new Rectangle(0, 0, 23, 46);

			// Fire and head are both positioned relative to the body.
			bodyPosition = new Vector2(100, 200);
			headPosition = new Vector2(100 + headXOff, 200 + headYOff);
			firePosition = new Vector2(100 + fireXOff, 200 + fireYOff);

			bodyTexture = newTexture[0];
			headTexture = newTexture[1];
			fireTexture = newTexture[2];

			// Rotational offset. 
			headOffset = new Vector2(17.5F, 17.5F);

			// Fire animation timer.
			exhaust = new Stopwatch();
			exhaust.Start();

			if (Game.DEBUG)
			{
				dString1 = dString2 = "";
				dPosition1 = dPosition2 = new Vector2(0, 0);
			}
		}

		public Vector2 Position()
		{
			return bodyPosition;
		}

		public void Update()
		{
			switch (state)
			{
				case Playerstate.Alive:
					{
						Living();
						break;
					}
				case Playerstate.Dead:
					{
						Dead();
						break;
					}
			};
		}

		public void Draw(SpriteBatch spriteBatch)
		{
			if (Game.DEBUG)
			{
				spriteBatch.DrawString(Game.FONT07, dString1, dPosition1, Color.Black);
				spriteBatch.DrawString(Game.FONT07, dString2, dPosition2, Color.Black);
			}

			if (state == Playerstate.Alive)
			{
				// Fire.
				spriteBatch.Draw(fireTexture, firePosition, fire, Color.White, 0, default(Vector2), scale, SpriteEffects.None, 0);
				// Body.
				spriteBatch.Draw(bodyTexture, bodyPosition, body, Color.White, 0, default(Vector2), scale, SpriteEffects.None, 0);
				// Head.
				spriteBatch.Draw(headTexture, headPosition, head, Color.White, headRotation, headOffset, scale, SpriteEffects.None, 0);
			}
		}

		private void Dead()
		{
			if (lives > 0)
			{
				Lives(-1);
				X(25);
				Y(Game.HEIGHT / 2 - bodyTexture.Height / 2);
			}

			state = Playerstate.Alive;
		}

		private void Living()
		{
			UpdateBodyAnimation(Game.MOUSE);
			UpdateHeadAnimation(Game.MOUSE);
			UpdateFlamesAnimation();

			if (Game.DEBUG)
			{
				dString1 = "L: " + lives + " " + "V: " + velocity;
				dString2 = "X: " + bodyPosition.X + " " + "Y: " + bodyPosition.Y;

				dPosition1 = new Vector2(bodyPosition.X + body.Width + 1, bodyPosition.Y + 2);
				dPosition2 = new Vector2(bodyPosition.X + body.Width + 1, bodyPosition.Y + 17);
			}

			if (Game.KEYBOARD.IsKeyDown(left))
				if (bodyPosition.X > 0)
				{
					bodyPosition.X -= velocity;
					headPosition.X -= velocity;
					firePosition.X -= velocity;
				}

			if (Game.KEYBOARD.IsKeyDown(up))
				if (bodyPosition.Y > 0)
				{
					bodyPosition.Y -= velocity;
					headPosition.Y -= velocity;
					firePosition.Y -= velocity;
				}

			if (Game.KEYBOARD.IsKeyDown(right))
				if (bodyPosition.X + 30 < Game.WIDTH)
				{
					bodyPosition.X += velocity;
					headPosition.X += velocity;
					firePosition.X += velocity;
				}

			if (Game.KEYBOARD.IsKeyDown(down))
				if (bodyPosition.Y + 50 < Game.HEIGHT)
				{
					bodyPosition.Y += velocity;
					headPosition.Y += velocity;
					firePosition.Y += velocity;
				}
		}
		
		public Rectangle Rectangle()
		{
			return new Rectangle((int)bodyPosition.X, (int)bodyPosition.Y, 25, 52);
		}

		public void X(int newX)
		{
			bodyPosition.X = newX;
			headPosition.X = bodyPosition.X + headXOff;
			firePosition.X = bodyPosition.X + fireXOff;
		}

		public void Y(int newY)
		{
			bodyPosition.Y = newY;
			headPosition.Y = bodyPosition.Y + headYOff;
			firePosition.Y = bodyPosition.Y + fireYOff;
		}

		public int RemainingLives()
		{
			return currentLives;
		}

		public void Lives(int inputLives)
		{
			currentLives = inputLives;
		}

		public void Plus()
		{
			currentLives++;
		}

		public void Minus()
		{
			currentLives--;
		}

		public void ResetLives()
		{
			currentLives = lives;
		}

		public void State(int newState)
		{
			if (newState == 1)
				state = Playerstate.Alive;
			if (newState == 2)
				state = Playerstate.Dead;
		}

		private void UpdateHeadAnimation(MouseState mouse)
		{
			Vector2 mouseLoc = new Vector2(Game.MOUSE.X, Game.MOUSE.Y);

			Vector2 leftFacing = new Vector2(headPosition.X - mouse.X, headPosition.Y - mouse.Y);
			Vector2 rightFacing = new Vector2(mouse.X - headPosition.X, mouse.Y - headPosition.Y);

			if (mouse.X < headPosition.X + head.Width / 2)
			{
				headRotation = (float)(Math.Atan2(leftFacing.Y, leftFacing.X));
				head.X = 44;
			}

			else if (mouse.X >= headPosition.X + head.Width / 2)
			{
				headRotation = (float)(Math.Atan2(rightFacing.Y, rightFacing.X));
				head.X = 0;
			}
		}

		private void UpdateBodyAnimation(MouseState mouse)
		{
			if (mouse.X < bodyPosition.X + body.Width / 2)
				body.X = 40;
			if (mouse.X >= bodyPosition.X + body.Width / 2)
				body.X = 0;
		}

		private void UpdateFlamesAnimation()
		{
			if ((exhaust.ElapsedMilliseconds >= 0) && (exhaust.ElapsedMilliseconds < 300))
				fire.X = 52;
			if ((exhaust.ElapsedMilliseconds >= 300) && (exhaust.ElapsedMilliseconds < 600))
				fire.X = 25;
			if ((exhaust.ElapsedMilliseconds >= 600) && (exhaust.ElapsedMilliseconds < 900))
				fire.X = 0;

			if (exhaust.ElapsedMilliseconds > 900)
				exhaust.Restart();
		}
	}
}
