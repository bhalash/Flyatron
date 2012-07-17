using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
// My classes. Nar-de-nar-nar.
using Flyatron;
using System.Collections.Generic;

namespace Flyatron
{
	public class Game : Microsoft.Xna.Framework.Game
	{
		public static Game Instance;

		static double version = 0.1;
		static string versionString = "Version " + version;
		bool debugging = true;

		// BOW BEFORE YOUR ENDING!
		static SpriteBatch spriteBatch;
		GraphicsDeviceManager graphics;

		// Looped background layers.
		BackgroundLayer cloud1, cloud2, cloud3;

		// Width, height, full screen.
		// Laptop's native is 1366x768.
		int gameWidth   = 1366;
		int gameHeight	 = 768;
		bool fullScreen = true;
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
		Player first;
		
		// Menu state.
		enum ScreenState
		{Menu, Play, New, Scores, About};

		// Music sourced from the chiptunes group "8-Bit Weapon". Used with permission in exchange for attribution.
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
		// Texture for the menu, since I use it in several places.
		Texture2D menuBg; Vector2 menuVec;

		public Game()
		{
			Instance = this;

			// Display opts.
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = gameWidth;
			graphics.PreferredBackBufferHeight = gameHeight;
			this.graphics.IsFullScreen = fullScreen;
			this.IsMouseVisible = showMouse;

			Content.RootDirectory = "Content";
		}

		protected override void LoadContent()
		{
			font10 = Content.Load<SpriteFont>("fonts\\PressStart2P_10");
			font14 = Content.Load<SpriteFont>("fonts\\PressStart2P_14");
			font25 = Content.Load<SpriteFont>("fonts\\PressStart2P_25");

			// Menu BG.
			menuBg = Content.Load<Texture2D>("bg\\paused");
			menuVec = new Vector2(0, 0);

			eightBitWeapon = new Muzak();
			eightBitWeapon.Play(Content.Load<Song>(playList[0]));

			first.Art(Content.Load<Texture2D>("player\\astronaut"), new Vector2(100, 300), Color.White);
		}

		protected override void Initialize()
		{
			// Background textures.
			cloud1 = new BackgroundLayer(Content.Load<Texture2D>("sky\\cloud2"), new Vector2(0, (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2 - 200)));
			cloud2 = new BackgroundLayer(Content.Load<Texture2D>("sky\\cloud1"), new Vector2(0, (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2 - 100)));
			cloud3 = new BackgroundLayer(Content.Load<Texture2D>("sky\\cloud0"), new Vector2(0, (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 2)));

			spriteBatch = new SpriteBatch(GraphicsDevice);

			first = new Player();
			// Player information.	
			first.Bounds(gameWidth, gameHeight);
			first.Stats(3, 8, 15, 0);
			base.Initialize();
		}

		protected override void UnloadContent()
		{
		}

		protected override void Update(GameTime gameTime)
		{
			// Capture current keyboard state.
			currentKeyboardState = Keyboard.GetState();

			if (screen == ScreenState.Menu)
				UpdateMenu();
			if (screen == ScreenState.Play)
				UpdatePlay();
			if (screen == ScreenState.New)
				New();
			if (screen == ScreenState.Scores)
				UpdateAbout();
			if (screen == ScreenState.About)
				UpdateScores();
		
			base.Update(gameTime);

			// Current keyboard state becomes the old. 
			// Repeat x60 per second.
			lastKeyboardState = currentKeyboardState;
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			// The SpriteBatch is god. 
			// The SpriteBatch is devil.
			// The SpriteBatch is the alpha and the omega.
			spriteBatch.Begin();
				// Draw background. Sprites are drawn in the order in which they are evoked.
				// I therefore want to make sure these remain in the background by evoking
				// them before anything else.
				cloud1.DrawLoop(spriteBatch);
				cloud2.DrawLoop(spriteBatch);
				cloud3.DrawLoop(spriteBatch);

				if (screen == ScreenState.Menu)
					Menu();
				if (screen == ScreenState.Play)
					Play();
				if (screen == ScreenState.New)
					New();
				if (screen == ScreenState.Scores)
					About();
				if (screen == ScreenState.About)
					Scores();

			spriteBatch.End();

			base.Draw(gameTime);
		}

		private bool Keypress(Keys inputKey)
		{
			// If the key has been pressed and lastKeyboardState released, true. Else, false.
			if (currentKeyboardState.IsKeyUp(inputKey) && (lastKeyboardState.IsKeyDown(inputKey)))
				return true;

			return false;
		}

		public int Rng(int a, int b)
		{
			// Butt-simple random number generator. Roll between a and b.
			// Normally I wouldn't methodize this, but there are things I may need to do here.
			// Wicked things.
			Random random = new Random();
			return random.Next(a, b);
		}

		public void UpdateMenu()
		{
			if (Keypress(Keys.Escape))
				screen = ScreenState.Play;
			// Menu opts.
			if (Keypress(Keys.D1))
				screen = ScreenState.Play;
			if (Keypress(Keys.D2))
				screen = ScreenState.New;
			if (Keypress(Keys.D3))
				screen = ScreenState.About;
			if (Keypress(Keys.D4))
				screen = ScreenState.Scores;
			if (Keypress(Keys.D5))
				this.Exit();
		}

		public void UpdateAbout()
		{
			if (Keypress(Keys.Escape))
				screen = ScreenState.Menu;
		}

		public void UpdateScores()
		{
			if (Keypress(Keys.Escape))
				screen = ScreenState.Menu;
		}

		public void UpdatePlay()
		{
			if (Keyboard.GetState().IsKeyDown(Keys.W))
			{
				// Move background up.
				cloud1.Update(0, 5);
				cloud2.Update(0, 6);
				cloud3.Update(0, 7);
			}
			if (Keyboard.GetState().IsKeyDown(Keys.S))
			{
				// Move background down.
				cloud1.Update(0, -5);
				cloud2.Update(0, -6);
				cloud3.Update(0, -7);
			}

			// Move player.
			first.Update(currentKeyboardState, currentMouseState, new GameTime());

			// if (Keyboard.GetState().IsKeyDown(Keys.A))
			// if (Keyboard.GetState().IsKeyDown(Keys.D))

			// Muzak controls.
			if (Keypress(Keys.N))
				eightBitWeapon.Play(Content.Load<Song>(playList[Rng(0, playList.Count - 1)]));
			if (Keypress(Keys.P))
				eightBitWeapon.Pause();
			if (Keypress(Keys.U))
				eightBitWeapon.Resume();

			// Toggle between the menu and gameplay.
			if (Keypress(Keys.Escape))
				screen = ScreenState.Menu;
		}

		public void Menu()
		{
			if (Keypress(Keys.Escape))
				screen = ScreenState.Play;
			// Quieter music.
			eightBitWeapon.Volume(0.2F);

			// Scroll background left...slowly-er.
			cloud1.Update(-1, 0);
			cloud2.Update(-2, 0);
			cloud3.Update(-3, 0);

			int Y = 100;

			string title = "Flyatron";

			List<string> opt = new List<string>()
			{
				"Resume Game",
				"New Game",
				"High Scores",
				"About Flyatron",
				"Exit"
			};

			spriteBatch.Draw(menuBg, menuVec, Color.White);
			spriteBatch.DrawString(font25, title, new Vector2(50, 50), Color.White);
			// Draw the menu.
			for (int i = 0; i < opt.Count; i++)
			{
				spriteBatch.DrawString(font14, (i + 1) + ". " + opt[i], new Vector2(50, Y), Color.White);
				Y += 35;
			}
		}

		private void Play()
		{
			if (!debugging)
				spriteBatch.DrawString(font10, eightBitWeapon.NameTime(), new Vector2(GraphicsDevice.Viewport.X + 25, GraphicsDevice.Viewport.Height - 30), Color.Black);

			// Draw player.
			if (debugging)
			{
				first.Debug(font10, spriteBatch, 30, 30);
				spriteBatch.DrawString(font10, versionString, new Vector2(30, gameHeight - 30), Color.Black);
			}

			first.Draw(spriteBatch);

			// Louder music.
			eightBitWeapon.Volume(0.5F);

			// Scroll background left.
			cloud1.Update(-5, 0);
			cloud2.Update(-6, 0);
			cloud3.Update(-7, 0);
		}

		private void New()
		{
			// Load a new game.
			screen = ScreenState.Play;
		}

		private void About()
		{
			string title = "About Flyatron";

			cloud1.Update(-1, 0);
			cloud2.Update(-2, 0);
			cloud3.Update(-3, 0);

			spriteBatch.Draw(menuBg, menuVec, Color.White);
			spriteBatch.DrawString(font25, title, new Vector2(50, 50), Color.White);
			spriteBatch.DrawString(font14, "TODO", new Vector2(50, 120), Color.White);
		}

		private void Scores()
		{
			string title = "Top Scores";

			cloud1.Update(-1, 0);
			cloud2.Update(-2, 0);
			cloud3.Update(-3, 0);

			spriteBatch.Draw(menuBg, menuVec, Color.White);
			spriteBatch.DrawString(font25, title, new Vector2(50, 50), Color.White);
			spriteBatch.DrawString(font14, "TODO", new Vector2(50, 120), Color.White);
		}
	}
}
