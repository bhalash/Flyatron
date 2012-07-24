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
		public static Game Instance;

		static double version = 0.1;
		static string versionString = "Version " + version;
		bool debug = true;

		static SpriteBatch spriteBatch;
		GraphicsDeviceManager graphics;

		// Width, height, full screen.
		// Laptop's native is 1366x768.
		// A decent working size for me is 1024x600.
		int gameWidth = 1366;
		int gameHeight = 768;
		bool fullScreen = true;
		bool showMouse = false;

		// Another gloabally used variable. 
		// This is the default movement velocity of any sprite.
		float velocity; 

		Mine mine;

		// Test if a key or button has been: 
		KeyboardState lastKeyboardState, currentKeyboardState;
		MouseState lastMouseState, currentMouseState;

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
		Texture2D[] playerTextures;

		// Mines.
		Texture2D[] mineTextures;

		// Score
		Scoreboard scores;
		string scoreFile = "scores.txt";

		// Menu state.
		enum ScreenState
		{ Menu, Play, New, ScoresScreen, AboutScreen, GameOver };

		// Backdrop.
		Texture2D[] alphaTextures;
		Backdrop alpha;

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

		// Flyatron should start at the game menu.
		ScreenState screen = ScreenState.Menu;
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
			graphics.PreferredBackBufferWidth = gameWidth;
			graphics.PreferredBackBufferHeight = gameHeight;
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
			alpha = new Backdrop(alphaTextures, gameHeight);

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

			if (debug)
				eightBitWeapon.Pause();
		}

		protected override void Initialize()
		{
			playerTextures = new Texture2D[]
			{
				// Player textures.
				Content.Load<Texture2D>("player\\head"),
				Content.Load<Texture2D>("player\\body"),
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

			// Initialize demonstration mine.
			mine = new Mine(mineTextures);

			// Load player art/stats.
			a = new Player(3, 8, 15, 0, Color.White, playerTextures);
			a.Bounds(gameWidth, gameHeight);

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
			// Update screen selection.
			UpdateSwitchScreen(screen);

			if (Keypress(Keys.Escape))
			{
				// Toggle between menu and gameplay.
				if (screen == ScreenState.Play)
					screen = ScreenState.Menu;
				else if (screen == ScreenState.Menu)
					screen = ScreenState.Play;
			}

			if ((a.RemainingLives() <= 0) && (screen == ScreenState.Play))
				screen = ScreenState.GameOver;

			base.Update(gameTime);

			lastMouseState = currentMouseState;
			lastKeyboardState = currentKeyboardState;
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin();
			// Draw the background.
			alpha.Draw(spriteBatch);
			// Update game state.
			SwitchScreen(screen, gameTime, spriteBatch);

			spriteBatch.End();

			base.Draw(gameTime);
		}

		private void Movement()
		{
		}

		private bool Keypress(Keys inputKey)
		{
			if (currentKeyboardState.IsKeyUp(inputKey) && (lastKeyboardState.IsKeyDown(inputKey)))
				return true;

			return false;
		}

		private int Rng(int a, int b)
		{
			Random random = new Random();
			return random.Next(a, b);
		}

		private void UpdateMenu()
		{
			// Menu opts.
			if (Keypress(Keys.D1))
				screen = ScreenState.Play;
			if (Keypress(Keys.D2))
				screen = ScreenState.New;
			if (Keypress(Keys.D3))
				screen = ScreenState.ScoresScreen;
			if (Keypress(Keys.D4))
				screen = ScreenState.AboutScreen;
			if (Keypress(Keys.D5))
			{
				this.Exit();
			}
		}

		private void UpdateAboutScreenScreen()
		{
			if (Keypress(Keys.Escape))
				screen = ScreenState.Menu;
		}

		private void UpdateGameOverScreen()
		{
			if (Keypress(Keys.Escape))
			{
				scores.Collate();
				screen = ScreenState.Menu;
			}
		}

		private void UpdateScoreScreen()
		{
			if (Keypress(Keys.Escape))
				screen = ScreenState.Menu;
		}

		private void UpdatePlayScreen()
		{
			mine.Update(currentKeyboardState);
			alpha.Update(currentKeyboardState, 6);
			scores.Increment();
			a.Update(currentKeyboardState, currentMouseState, new GameTime());

			if (Keypress(Keys.N))
				eightBitWeapon.Play(Content.Load<Song>(playList[Rng(0, playList.Count - 1)]));
			if (Keypress(Keys.P))
				eightBitWeapon.Pause();
			if (Keypress(Keys.U))
				eightBitWeapon.Resume();
		}

		private void Play(GameTime gameTime)
		{
			eightBitWeapon.Volume(0.7F);
			HUD(gameTime);
			a.Draw(spriteBatch);
			// Draw mine. Debug. 
			mine.Draw(spriteBatch);
			spriteBatch.Draw(mouse, mousePos, Color.White);
		}

		private void Menu()
		{
			eightBitWeapon.Volume(0.5F);
			int Y = 100;

			if (a.RemainingLives() == 0)
				for (int i = 0; i < 3; i++)
					a.Lives(3);

			string title = "Flyatron";

			List<string> opt = new List<string>()
			{
				"Resume Game",
				"New Game",
				"High Scores",
				"About",
				"Exit"
			};

			alpha.Demo(3);
			spriteBatch.Draw(menuBg, menuVec, Color.White);
			spriteBatch.DrawString(font25, title, new Vector2(50, 50), Color.White);
			// Draw the menu.
			for (int i = 0; i < opt.Count; i++)
			{
				spriteBatch.DrawString(font14, (i + 1) + ". " + opt[i], new Vector2(50, Y), Color.White);
				Y += 35;
			}

			if (debug)
				spriteBatch.DrawString(
					font10,
					versionString,
					new Vector2(gameWidth - 30 - font10.MeasureString(versionString).Length(), gameHeight - 30),
					Color.White
				);
		}

		private void GameOverScreen()
		{
			if (!deathScreenTimer.IsRunning)
				deathScreenTimer.Start();

			string message = "GAME OVER";

			alpha.Demo(3);

			Vector2 fontVector = new Vector2(gameWidth / 2 - font25.MeasureString(message).Length() / 2, gameHeight / 2 - 25);
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

			if (Keypress(Keys.Escape))
			{
				deathScreenTimer.Reset();
				screen = ScreenState.Menu;
			}
		}

		private void HUD(GameTime gameTime)
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

			if (!debug)
				spriteBatch.DrawString(
					font10,
					eightBitWeapon.NameTime(),
					new Vector2(GraphicsDevice.Viewport.X + 25, GraphicsDevice.Viewport.Height - 30),
					Color.Black
				);

			if (debug)
				spriteBatch.DrawString(
					font10,
					Convert.ToString(gameTime.TotalGameTime),
					new Vector2(30, gameHeight - 30),
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
			if (mousePos.X > gameWidth)
				mousePos.X = gameWidth;
			if (mousePos.Y < 0)
				mousePos.Y = 0;
			if (mousePos.Y > gameHeight)
				mousePos.Y = gameHeight;
		}

		private void NewGame()
		{
			// NewGame() should run once, in one pass. 
			// Set up the sprites, reset the appropriate counters. 
			// Thereafter, switch to ScreenState.Play.

			scores.Reset();

			screen = ScreenState.Play;
		}

		private void AboutScreen()
		{
			string title = "About Flyatron";

			alpha.Demo(3);
			spriteBatch.Draw(menuBg, menuVec, Color.White);
			spriteBatch.DrawString(font25, title, new Vector2(50, 50), Color.White);
			spriteBatch.DrawString(font10, "Dedicated to Ciara and Garrett.", new Vector2(50, 120), Color.White);
		}

		private void ScoresScreen()
		{
			string title = "Top Scores";

			alpha.Demo(3);
			spriteBatch.Draw(menuBg, menuVec, Color.White);
			spriteBatch.DrawString(font25, title, new Vector2(50, 50), Color.White);
			scores.Report(font14, spriteBatch, 55, 100, Color.White);
		}

		private void SwitchScreen(ScreenState screenState, GameTime gameTime, SpriteBatch spriteBatch)
		{
			switch (screen)
			{
				case ScreenState.Menu:
					{
						Menu();
						break;
					}
				case ScreenState.Play:
					{
						Play(gameTime);
						break;
					}
				case ScreenState.New:
					{
						NewGame();
						break;
					}
				case ScreenState.ScoresScreen:
					{
						ScoresScreen();
						break;
					}
				case ScreenState.AboutScreen:
					{
						AboutScreen();
						break;
					}
				case ScreenState.GameOver:
					{
						GameOverScreen();
						break;
					}
			}
		}

		private void UpdateSwitchScreen(ScreenState screenState)
		{
			switch (screen)
			{
				case ScreenState.Menu:
					{
						UpdateMenu();
						break;
					}
				case ScreenState.Play:
					{
						UpdatePlayScreen();
						break;
					}
				case ScreenState.New:
					{
						NewGame();
						break;
					}
				case ScreenState.ScoresScreen:
					{
						UpdateScoreScreen();
						break;
					}
				case ScreenState.AboutScreen:
					{
						UpdateAboutScreenScreen();
						break;
					}
				case ScreenState.GameOver:
					{
						UpdateGameOverScreen();
						break;
					}
			}
		}
	}
}
