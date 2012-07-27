using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using Flyatron;
using System.Diagnostics;

namespace Flyatron
{
	public class Game : Microsoft.Xna.Framework.Game
	{
		// Welcome to Flyatron!

		public static bool DEBUG = false;

		public static int WIDTH = 1024;
		public static int HEIGHT = 600;

		public enum   Screen { Menu, Play, New, Scores, About, End, Death };
		public static Screen SCREEN = Screen.Menu;

		bool fullScreen = false;
		bool showMouse = false;

		static double VERSION = 0.1;
		static string versionString = "Version " + VERSION;

		public static Game Instance;

		static SpriteBatch spriteBatch;
		GraphicsDeviceManager graphics;

		public static KeyboardState lastKeyboardState, currentKeyboardState;
		public static MouseState lastMouseState, currentMouseState;

		// Flyatron uses the font "Press Start 2P". The font was created by William Cody of Zone38.
		// The font is distributed under the permissive SIL Open Font License (OFL), and included in
		// this project under it, along with creator attribution.
		// Font download page:	 http://www.zone38.net/font/
		// OFL license homepage: http://scripts.sil.org/cms/scripts/page.php?site_id=nrsi&id=OFL
		static SpriteFont font10, font14, font25;	

		// Soundtrack instance.
		Muzak eightBitWeapon;

		// Player.
		Player a;
		List <Texture2D> playerTextures;

		// Fear, fire foes.
		Texture2D[] mineTextures;
		List <Mine> mines;
		int totalMines = 30;

		// Scoreboard.
		Scoreboard scores;
		string scoreFile = "scores.txt";

		// Backdrop.
		Texture2D[] alphaTextures;
		Backdrop cloudyBackdrop;

		// Event timer.
		Stopwatch deathScreenTimer;

		// Music sourced from the chiptunes group "8-Bit Weapon". Used with permission.
		// Homepage: http://www.8bitweapon.com
		List<string> playList = new List<string>()
		{ 
			"8 Bit Weapon/Chip On Your Shoulder", 
			"8 Bit Weapon/Closer (Bitpop Mix)", 
			"8 Bit Weapon/Micro Boogie", 
			"8 Bit Weapon/One Last Mission", 
			"8 Bit Weapon/Times Changing" 
		};

		// Texture for the menu. Declared here since I use it in several places.
		Texture2D menuBg;
		Vector2 menuVec;

		// Custom mouse texture.
		Texture2D mouse;
		Vector2 mousePos;
		MouseState mouseState;

		public Game()
		{
			Instance = this;
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = WIDTH;
			graphics.PreferredBackBufferHeight = HEIGHT;
			this.graphics.IsFullScreen = fullScreen;
			this.IsMouseVisible = showMouse;

			Content.RootDirectory = "Content";
		}

		protected override void LoadContent()
		{
			alphaTextures = new Texture2D[]
			{
				// Backdrop textures.
				Content.Load<Texture2D>("sky\\cloud0"),
				Content.Load<Texture2D>("sky\\cloud1"),
				Content.Load<Texture2D>("sky\\cloud2")
			};

			// Initialize backdrop.
			cloudyBackdrop = new Backdrop(alphaTextures);

			// Numerical value equates to pixel size.
			font10 = Content.Load<SpriteFont>("fonts\\PressStart2P_10");
			font14 = Content.Load<SpriteFont>("fonts\\PressStart2P_14");
			font25 = Content.Load<SpriteFont>("fonts\\PressStart2P_25");

			// Initialize menu backdrop.
			menuBg = Content.Load<Texture2D>("bg\\paused");
			menuVec = new Vector2(0, 0);

			// Initialize soundtrack audio.
			eightBitWeapon = new Muzak();
			eightBitWeapon.Play(Content.Load<Song>(playList[0]));

			if (DEBUG)
				eightBitWeapon.Pause();
		}

		protected override void Initialize()
		{
			playerTextures = new List<Texture2D>()
			{
				// Player textures.
				Content.Load<Texture2D>("player\\body"),
				Content.Load<Texture2D>("player\\head"),
				Content.Load<Texture2D>("player\\gun"),
				Content.Load<Texture2D>("player\\flames")
			};

			mineTextures = new Texture2D[]
			{
				Content.Load<Texture2D>("mine\\core"),
				Content.Load<Texture2D>("mine\\spikes")
			};

			// Initialize SpriteBatch.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// Load timers.
			deathScreenTimer = new Stopwatch();

			// Initialize foe mines.
			mines = new List<Mine>();

			for (int i = 0; i < totalMines; i ++)
				mines.Add(new Mine(mineTextures, 5));

			// Load player art/stats.
			a = new Player(5, 10, Color.White, playerTextures);

			// Load mouse texture.
			mouse = Content.Load<Texture2D>("pointer");

			// Import top scores.
			scores = new Scoreboard(scoreFile);

			base.Initialize();
		}

		protected override void UnloadContent()
		{
		}

		protected override void Update(GameTime gameTime)
		{
			currentKeyboardState = Keyboard.GetState();
			currentMouseState = Mouse.GetState();

			// Update mouse pointer.
			UpdateMouse(currentMouseState);
			// Update SCREEN selection.
			UpdateSwitch(SCREEN);

			if (Keypress(Keys.Escape))
			{
				// Toggle between menu and gameplay.
				if (SCREEN == Screen.Play)
					SCREEN = Screen.Menu;
				else if (SCREEN == Screen.Menu)
					SCREEN = Screen.Play;
			}

			base.Update(gameTime);

			lastMouseState = currentMouseState;
			lastKeyboardState = currentKeyboardState;
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin();
			// Draw the background.
			cloudyBackdrop.Draw(spriteBatch);
			// Update game state.
			Switch(SCREEN, gameTime, spriteBatch);
			spriteBatch.End();

			base.Draw(gameTime);
		}

		public static bool Keypress(Keys inputKey)
		{
			if (currentKeyboardState.IsKeyUp(inputKey) && (lastKeyboardState.IsKeyDown(inputKey)))
				return true;

			return false;
		}

		private void NewGame()
		{
			// NewGame() should run once, in one pass. 
			// Set up the sprites, reset the appropriate counters. 
			// Thereafter, switch to Screen.Play.

			scores.Reset();

			SCREEN = Screen.Play;
		}

		private void UpdateDeath()
		{
			SCREEN = Screen.Play;
		}

		private void DrawDeath()
		{
			for (int i = 0; i < mines.Count; i++)
				mines[i].Halt(6);

			a.X(100);
			a.Y(HEIGHT / 2 - a.Rectangle().Height / 2);
			a.Lives(-1);

			if (a.RemainingLives() <= 0)
				SCREEN = Screen.End;
		}

		private void UpdateMenu()
		{
			// Menu opts.
			if (Keypress(Keys.D1))
				SCREEN = Screen.Play;
			if (Keypress(Keys.D2))
				SCREEN = Screen.New;
			if (Keypress(Keys.D3))
				SCREEN = Screen.Scores;
			if (Keypress(Keys.D4))
				SCREEN = Screen.About;
			if (Keypress(Keys.D5))
			{
				this.Exit();
			}
		}

		private void DrawMenu()
		{
			eightBitWeapon.Volume(0.5F);
			int Y = 100;

			string title = "Flyatron";

			List<string> opt = new List<string>()
			{
				"Resume Game",
				"New Game",
				"High Scores",
				"About",
				"Exit"
			};

			cloudyBackdrop.Update(3);
			spriteBatch.Draw(menuBg, menuVec, Color.White);
			spriteBatch.DrawString(font25, title, new Vector2(50, 50), Color.White);
			// Draw the menu.
			for (int i = 0; i < opt.Count; i++)
			{
				spriteBatch.DrawString(font14, (i + 1) + ". " + opt[i], new Vector2(50, Y), Color.White);
				Y += 35;
			}

			if (DEBUG)
				spriteBatch.DrawString(
					font10,
					versionString,
					new Vector2(WIDTH - 30 - font10.MeasureString(versionString).Length(), HEIGHT - 30),
					Color.White
				);
		}

		private void UpdateAbout()
		{
			if (Keypress(Keys.Escape))
				SCREEN = Screen.Menu;
		}

		private void DrawAbout()
		{
			int x = 50;
			int y = 120;

			List<string> messages = new List<string>()
			{
				"Dedicated to Ciara and Garrett.",
				"",
				"Big thanks to:",
				"The 091 Labs Hackerspace",
				"8 Bit Weapon",
				"Alanna Kelly",
				"Domhall Walsh",
				"Duncan Thomas",
				"Jennifer Tidmore (just because :)",
			};

			string title = "About Flyatron";

			cloudyBackdrop.Update(3);
			spriteBatch.Draw(menuBg, menuVec, Color.White);
			spriteBatch.DrawString(font25, title, new Vector2(x, 50), Color.White);

			for (int i = 0; i < messages.Count; i++)
			{
				spriteBatch.DrawString(font10, messages[i], new Vector2(x, y), Color.White);
				y += 20;
			}
		}

		private void UpdateEnd()
		{
			if (Keypress(Keys.Escape))
				SCREEN = Screen.Scores;
		}

		private void DrawEnd()
		{
			if (!deathScreenTimer.IsRunning)
				deathScreenTimer.Start();

			string message = "GAME OVER";

			cloudyBackdrop.Update(3);

			Vector2 fontVector = new Vector2(WIDTH / 2 - font25.MeasureString(message).Length() / 2, HEIGHT / 2 - 25);
			Color color = Color.White;

			if ((deathScreenTimer.ElapsedMilliseconds > 1000) && (deathScreenTimer.ElapsedMilliseconds <= 2000))
			{
				spriteBatch.Draw(menuBg, menuVec, color * 0.25F);
				spriteBatch.DrawString(font25, message, fontVector, color * 0.25F);
			}
			if ((deathScreenTimer.ElapsedMilliseconds > 2000) && (deathScreenTimer.ElapsedMilliseconds <= 3000))
			{
				spriteBatch.Draw(menuBg, menuVec, color * 0.5F);
				spriteBatch.DrawString(font25, message, fontVector, color * 0.5F);
			}
			if ((deathScreenTimer.ElapsedMilliseconds > 3000) && (deathScreenTimer.ElapsedMilliseconds <= 4000))
			{
				spriteBatch.Draw(menuBg, menuVec, color * 0.75F);
				spriteBatch.DrawString(font25, message, fontVector, color * 0.75F);
			}
			if (deathScreenTimer.ElapsedMilliseconds > 4000)
			{
				spriteBatch.Draw(menuBg, menuVec, color * 1.0F);
				spriteBatch.DrawString(font25, message, fontVector, color * 1.0F);
			}
		}

		private void UpdateScores()
		{
			if (Keypress(Keys.Escape))
				SCREEN = Screen.Menu;
		}

		private void DrawScores()
		{
			string title = "Top Scores";

			cloudyBackdrop.Update(3);
			spriteBatch.Draw(menuBg, menuVec, Color.White);
			spriteBatch.DrawString(font25, title, new Vector2(50, 50), Color.White);
			scores.Report(font14, spriteBatch, 55, 100, Color.White);
		}

		private void UpdatePlay()
		{
			cloudyBackdrop.Update(5);

			// cloudyBackdrop.Update(currentKeyboardState, 6);
			scores.Increment();
			a.Update(new GameTime());

			for (int i = 0; i < mines.Count; i++)
				mines[i].Update(a.Position());

			for (int i = 0; i < mines.Count; i++)
				if (Helper.Circle(a.Rectangle(), mines[i].Rectangle()))
					SCREEN = Screen.Death;

			if (Keypress(Keys.N))
				eightBitWeapon.Play(Content.Load<Song>(playList[Helper.Rng(0, playList.Count - 1)]));
			if (Keypress(Keys.P))
				eightBitWeapon.Pause();
			if (Keypress(Keys.U))
				eightBitWeapon.Resume();
		}

		private void DrawPlay(GameTime gameTime)
		{
			eightBitWeapon.Volume(0.7F);

			DrawHud(gameTime);

			a.Draw(spriteBatch);

			for (int i = 0; i < mines.Count; i++)
				mines[i].Draw(spriteBatch);

			spriteBatch.Draw(mouse, mousePos, Color.White);
		}

		private void DrawHud(GameTime gameTime)
		{
			int y = 30;

			List<string> hud = new List<string>()
			{
				"Score: " + Convert.ToString(scores.Current()),
				"Lives: " + Convert.ToString(a.RemainingLives())
			};

			for (int i = 0; i < hud.Count; i++)
			{
				spriteBatch.DrawString(
					font14,
					hud[i],
					new Vector2(30, y),
					Color.Black
				);

				y += 30;
			}

			if (!DEBUG)
				spriteBatch.DrawString(
					font10,
					eightBitWeapon.NameTime(),
					new Vector2(GraphicsDevice.Viewport.X + 25, GraphicsDevice.Viewport.Height - 30),
					Color.Black
				);

			if (DEBUG)
				spriteBatch.DrawString(
					font10,
					Convert.ToString(gameTime.TotalGameTime),
					new Vector2(30, HEIGHT - 30),
					Color.Black
				);
		}

		private void UpdateMouse(MouseState inputMousestate)
		{
			mouseState = inputMousestate;
			mousePos.X = mouseState.X;
			mousePos.Y = mouseState.Y;

			// Restrict the Mouse so that it stays inside the current display
			if (mousePos.X < 0)
				mousePos.X = 0;
			if (mousePos.X > WIDTH)
				mousePos.X = WIDTH;
			if (mousePos.Y < 0)
				mousePos.Y = 0;
			if (mousePos.Y > HEIGHT)
				mousePos.Y = HEIGHT;
		}

		private void Switch(Screen screenState, GameTime gameTime, SpriteBatch spriteBatch)
		{
			switch (SCREEN)
			{
				case Screen.Menu:
					{
						DrawMenu();
						break;
					}
				case Screen.Play:
					{
						DrawPlay(gameTime);
						break;
					}
				case Screen.New:
					{
						NewGame();
						break;
					}
				case Screen.Scores:
					{
						DrawScores();
						break;
					}
				case Screen.About:
					{
						DrawAbout();
						break;
					}
				case Screen.End:
					{
						DrawEnd();
						break;
					}
				case Screen.Death:
					{
						DrawDeath();
						break;
					}
			}
		}

		private void UpdateSwitch(Screen screenState)
		{
			switch (SCREEN)
			{
				case Screen.Menu:
					{
						UpdateMenu();
						break;
					}
				case Screen.Play:
					{
						UpdatePlay();
						break;
					}
				case Screen.New:
					{
						NewGame();
						break;
					}
				case Screen.Scores:
					{
						UpdateScores();
						break;
					}
				case Screen.About:
					{
						UpdateAbout();
						break;
					}
				case Screen.End:
					{
						UpdateEnd();
						break;
					}
				case Screen.Death:
					{
						UpdateDeath();
						break;
					}
			}
		}
	}
}
