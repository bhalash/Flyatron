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
		int gameWidth   = 1024;
		int gameHeight	 = 600;
		bool fullScreen = false;
		bool showMouse  = true;

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
		
		// Menu state.
		enum ScreenState
		{Menu, Play, New, ScoresScreen, AboutScreen, GameOver};

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
				Content.Load<Texture2D>("sky\\cloud0"),
				Content.Load<Texture2D>("sky\\cloud1"),
				Content.Load<Texture2D>("sky\\cloud2")
			};

			alpha = new Backdrop(alphaTextures, gameHeight);

			// Numerical value equates to pixel size.
			font10 = Content.Load<SpriteFont>("fonts\\PressStart2P_10");
			font14 = Content.Load<SpriteFont>("fonts\\PressStart2P_14");
			font25 = Content.Load<SpriteFont>("fonts\\PressStart2P_25");

			menuBg = Content.Load<Texture2D>("bg\\paused");
			menuVec = new Vector2(0, 0);

			eightBitWeapon = new Muzak();
			eightBitWeapon.Play(Content.Load<Song>(playList[0]));
		}

		protected override void Initialize()
		{
			spriteBatch = new SpriteBatch(GraphicsDevice);

			deathScreenTimer = new Stopwatch();

			a = new Player(3, 8, 15, 0, Content.Load<Texture2D>("player\\astronaut"), Color.White);
			a.Bounds(gameWidth, gameHeight);

			base.Initialize();
		}

		protected override void UnloadContent()
		{
		}

		protected override void Update(GameTime gameTime)
		{
			currentKeyboardState = Keyboard.GetState();

			if (Keypress(Keys.Escape))
			{
				if (screen == ScreenState.Play)
					screen = ScreenState.Menu;
				else if (screen == ScreenState.Menu)
					screen = ScreenState.Play;
			}

			if (Keypress(Keys.C))
				a.OneDown();
			if ((a.RemainingLives() <= 0) && (screen == ScreenState.Play))
				screen = ScreenState.GameOver;

			if (screen == ScreenState.Menu)
				UpdateMenu();
			if (screen == ScreenState.Play)
				UpdatePlayScreen();
			if (screen == ScreenState.New)
				NewGame();
			if (screen == ScreenState.ScoresScreen)
				UpdateAboutScreenScreen();
			if (screen == ScreenState.AboutScreen)
				UpdateScoreScreen();
			if (screen == ScreenState.GameOver)
				UpdateDeathScreen();
		
			base.Update(gameTime);

			lastKeyboardState = currentKeyboardState;
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin();

			alpha.Draw(spriteBatch);

			// Draw depending on state.
			if (screen == ScreenState.Menu)
				Menu();
			if (screen == ScreenState.Play)
				Play(gameTime);
			if (screen == ScreenState.New)
				NewGame();
			if (screen == ScreenState.ScoresScreen)
				ScoresScreen();
			if (screen == ScreenState.AboutScreen)
				AboutScreen();
			if (screen == ScreenState.GameOver)
				GameOverScreen();

			spriteBatch.End();

			base.Draw(gameTime);
		}

		private bool Keypress(Keys inputKey)
		{
			if (currentKeyboardState.IsKeyUp(inputKey) && (lastKeyboardState.IsKeyDown(inputKey)))
				return true;

			return false;
		}

		public int Rng(int a, int b)
		{
			Random random = new Random();
			return random.Next(a, b);
		}
		
		public void UpdateMenu()
		{
			// Menu opts.
			if (Keypress(Keys.D1))
				screen = ScreenState.Play;
			if (Keypress(Keys.D2))
				screen = ScreenState.New;
			if (Keypress(Keys.D3))
				screen = ScreenState.AboutScreen;
			if (Keypress(Keys.D4))
				screen = ScreenState.ScoresScreen;
			if (Keypress(Keys.D5))
				this.Exit();
		}

		public void UpdateAboutScreenScreen()
		{
			if (Keypress(Keys.Escape))
				screen = ScreenState.Menu;
		}

		public void UpdateDeathScreen()
		{
			if (Keypress(Keys.Escape))
				screen = ScreenState.Menu;
		}

		public void UpdateScoreScreen()
		{
			if (Keypress(Keys.Escape))
				screen = ScreenState.Menu;
		}

		public void UpdatePlayScreen()
		{
			alpha.Update(currentKeyboardState, 6);

			a.Update(currentKeyboardState, currentMouseState, new GameTime());

			if (Keypress(Keys.N))
				eightBitWeapon.Play(Content.Load<Song>(playList[Rng(0, playList.Count - 1)]));
			if (Keypress(Keys.P))
				eightBitWeapon.Pause();
			if (Keypress(Keys.U))
				eightBitWeapon.Resume();
		}

		public void Menu()
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

			alpha.Demo(3);
			spriteBatch.Draw(menuBg, menuVec, Color.White);
			spriteBatch.DrawString(font25, title, new Vector2(50, 50), Color.White);
			// Draw the menu.
			for (int i = 0; i < opt.Count; i++)
			{
				spriteBatch.DrawString(font14, (i + 1) + ". " + opt[i], new Vector2(50, Y), Color.White);
				Y += 35;
			}
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
			if ((deathScreenTimer.ElapsedMilliseconds > 4000) && (deathScreenTimer.ElapsedMilliseconds <= 5000))
			{
				spriteBatch.Draw(menuBg, menuVec, color * 1.0F);
				spriteBatch.DrawString(font25, message, fontVector, color * 1.0F);
			}

			if (deathScreenTimer.ElapsedMilliseconds > 5000)
			{
				deathScreenTimer.Stop();
				screen = ScreenState.ScoresScreen;
			}
		}

		private void Play(GameTime gameTime)
		{
			eightBitWeapon.Volume(0.7F);

			if (!debug)
				// Playing track.
				spriteBatch.DrawString(
					font10, 
					eightBitWeapon.NameTime(), 
					new Vector2(GraphicsDevice.Viewport.X + 25, GraphicsDevice.Viewport.Height - 30), 
					Color.Black
				);	

			if (debug)
			{
				a.Debug(font10, spriteBatch);

				spriteBatch.DrawString(
					font10, 
					Convert.ToString(gameTime.TotalGameTime), 
					new Vector2(30, gameHeight - 30), 
					Color.Black
				);

				spriteBatch.DrawString(
					font10, 
					versionString,
					new Vector2(gameWidth - 30 - font10.MeasureString(versionString).Length(), gameHeight - 30),
				Color.Black
			);
			}

			a.Draw(spriteBatch);
		}

		private void NewGame()
		{
			// TODO
			screen = ScreenState.Play;
		}

		private void AboutScreen()
		{
			string title = "About Flyatron";

			alpha.Demo(3);
			spriteBatch.Draw(menuBg, menuVec, Color.White);
			spriteBatch.DrawString(font25, title, new Vector2(50, 50), Color.White);
			spriteBatch.DrawString(font14, "TODO", new Vector2(50, 120), Color.White);
		}

		private void ScoresScreen()
		{
			string title = "Top Scores";

			alpha.Demo(3);
			spriteBatch.Draw(menuBg, menuVec, Color.White);
			spriteBatch.DrawString(font25, title, new Vector2(50, 50), Color.White);
			spriteBatch.DrawString(font14, "TODO", new Vector2(50, 120), Color.White);
		}
	}
}
