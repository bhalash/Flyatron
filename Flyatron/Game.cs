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

		public static bool DEBUG = true;

		public static int WIDTH  = 1024;
		public static int HEIGHT = 600;
		static bool FULLSCREEN   = false;

		static double VERSION = 0.1;
		static string VERSIONSTRING = "Version " + VERSION;

		enum Gamestate { Menu, Play, New, Scores, About, End, Death };
		Gamestate state = Gamestate.Menu;

		public static KeyboardState PREV_KEYBOARD, CURRENT_KEYBOARD;
		public static MouseState PREV_MOUSE, CURR_MOUSE;

		public static Game Instance;

		Texture2D cross;
		Texture2D border;
		float bOpacity;

		static SpriteBatch spriteBatch;
		GraphicsDeviceManager graphics;

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
		Texture2D[] playerTextures;
		
		// Gun.
		Gun gun;
		Texture2D[] gunTex;

		// Fear, fire foes.
		Texture2D[] mineTex;
		List <Mine> mine;
		int totalMines = 35;

		// Bonuses.
		Bonus bonus;
		Texture2D[] bonusTex;

		// Scoreboard.
		Scoreboard scores;

		// Backdrop.
		Texture2D[] backdropTex;
		Backdrop cloudyBackdrop;

		// Mouse.
		Flymouse mouse;

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

		public Game()
		{
			Instance = this;
			graphics = new GraphicsDeviceManager(this);
			graphics.PreferredBackBufferWidth = WIDTH;
			graphics.PreferredBackBufferHeight = HEIGHT;
			this.graphics.IsFullScreen = FULLSCREEN;

			if (!DEBUG)
				this.IsMouseVisible = false;
			else if (DEBUG)
				this.IsMouseVisible = true;

			Content.RootDirectory = "Content";
		}

		protected override void LoadContent()
		{
			backdropTex = new Texture2D[]
			{
				// Backdrop textures.
				Content.Load<Texture2D>("sky\\cloud0"),
				Content.Load<Texture2D>("sky\\cloud1"),
				Content.Load<Texture2D>("sky\\cloud2")
			};

			// Initialize backdrop.
			cloudyBackdrop = new Backdrop(backdropTex);

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
			border = Content.Load<Texture2D>("border");
			cross = Content.Load<Texture2D>("zero");

			playerTextures = new Texture2D[]
			{
				// Player textures.
				Content.Load<Texture2D>("player\\body"),
				Content.Load<Texture2D>("player\\head"),
				Content.Load<Texture2D>("player\\flames")
			};

			mineTex = new Texture2D[]
			{
				Content.Load<Texture2D>("mine\\core"),
				Content.Load<Texture2D>("mine\\spikes"),
				Content.Load<Texture2D>("mine\\explosion")
			};

			bonusTex = new Texture2D[]
			{				
				Content.Load<Texture2D>("bonus\\1"),
				Content.Load<Texture2D>("bonus\\2"),
				Content.Load<Texture2D>("bonus\\3"),
				Content.Load<Texture2D>("bonus\\4"),
				Content.Load<Texture2D>("bonus\\5")
			};

			// Load player art/stats.
			first = new Player(5, 10, Color.White, playerTextures);

			// Gun.
			gunTex = new Texture2D[]
			{
				Content.Load<Texture2D>("gun\\gun"),
				Content.Load<Texture2D>("gun\\bullet"),
				Content.Load<Texture2D>("border")
			};

			gun = new Gun(gunTex);

			// Initialize bonus.
			bonus = new Bonus(bonusTex);

			// Initialize mouse.
			mouse = new Flymouse(Content.Load<Texture2D>("pointer"));

			// Initialize SpriteBatch.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// Load timers.
			deathScreenTimer = new Stopwatch();

			// Initialize foe mine.
			mine = new List<Mine>();

			// Debug. Border opacity.
			bOpacity = 0.3F;

			for (int i = 0; i < totalMines; i ++)
				mine.Add(new Mine(mineTex));

			// Import top scores.
			scores = new Scoreboard();

			base.Initialize();
		}

		protected override void UnloadContent()
		{
		}

		protected override void Update(GameTime gameTime)
		{
			CURRENT_KEYBOARD = Keyboard.GetState();
			CURR_MOUSE = Mouse.GetState();

			mouse.Update();

			// Update SCREEN selection.
			UpdateSwitch();

			if (Helper.Keypress(Keys.Escape))
			{
				// Toggle between menu and gameplay.
				if (state == Gamestate.Play)
					state = Gamestate.Menu;
				else if ((state == Gamestate.Menu) && (first.RemainingLives() > 0))
					state = Gamestate.Play;
			}

			base.Update(gameTime);

			PREV_MOUSE = CURR_MOUSE;
			PREV_KEYBOARD = CURRENT_KEYBOARD;
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			spriteBatch.Begin();
			// Draw the background.
			cloudyBackdrop.Draw(spriteBatch);
			// Update game state.
			Switch(gameTime, spriteBatch);
			mouse.Draw(spriteBatch);
			spriteBatch.End();

			base.Draw(gameTime);
		}

		private void NewGame()
		{
			scores.Reset();
			first.Lives(5);

			for (int i = 0; i < mine.Count; i++)
				mine[i].State(2);

			state = Gamestate.Play;
		}

		private void UpdateDeath()
		{
			// This function should be called if and when the player dies.

			for (int i = 0; i < mine.Count; i++)
				mine[i].State(3);

			first.X(100);
			first.Y(HEIGHT / 2 - first.Rectangle().Height / 2);
			first.Lives(-1);

			if (first.RemainingLives() <= 0)
				state = Gamestate.End;
			else
				state = Gamestate.Play;
		}

		private void DrawDeath()
		{
			// Nothing to draw here. Included for completeness.
		}

		private void UpdateMenu()
		{
			// Menu opts.
			if ((Helper.Keypress(Keys.D1)) && (first.RemainingLives() > 0))
				state = Gamestate.Play;
			if (Helper.Keypress(Keys.D2))
				state = Gamestate.New;
			if (Helper.Keypress(Keys.D3))
				state = Gamestate.Scores;
			if (Helper.Keypress(Keys.D4))
				state = Gamestate.About;
			if (Helper.Keypress(Keys.D5))
				this.Exit();
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
					VERSIONSTRING,
					new Vector2(WIDTH - 30 - font10.MeasureString(VERSIONSTRING).Length(), HEIGHT - 30),
					Color.White
				);
		}

		private void UpdateAbout()
		{
			if (Helper.Keypress(Keys.Escape))
				state = Gamestate.Menu;
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
			for (int i = 0; i < mine.Count; i++)
				mine[i].State(2);

			bonus.State(2);

			if ((Helper.Keypress(Keys.Escape)) || (Helper.LeftClick()))
			{
				state = Gamestate.Menu;
				deathScreenTimer.Reset();
			}
		}

		private void DrawEnd()
		{
			Color color = Color.White;

			if (!deathScreenTimer.IsRunning)
				deathScreenTimer.Start();

			string message = "GAME OVER";

			cloudyBackdrop.Update(3);

			Vector2 fontVector = new Vector2(WIDTH / 2 - font25.MeasureString(message).Length() / 2, HEIGHT / 2 - 25);

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
			if (Helper.Keypress(Keys.Escape))
				state = Gamestate.Menu;
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
			if (first.RemainingLives() <= 0)
				state = Gamestate.End;

			// Update backdrop.
			cloudyBackdrop.Update(5);
			// Tick scores.
			scores.Increment();

			bonus.Update(first.Rectangle());

			if (!DEBUG)
				for (int i = 0; i < mine.Count; i++)
					mine[i].Update(first.Rectangle());
				
			// Player/mine collision.
			for (int i = 0; i < mine.Count; i++)
				if (Helper.CircleCollision(first.Rectangle(), mine[i].Rectangle()))
					state = Gamestate.Death;

			// Player/bonus collision.
			if (Helper.CircleCollision(first.Rectangle(), bonus.Rectangle()))
				if (bonus.Type() == 1)
					first.Lives(1);
				else if (bonus.Type() == 2)
					for (int i = 0; i < mine.Count; i++)
						mine[i].State(3);

			// Update player + gun.
			first.Update(); 
			gun.Update(first.Position());

			// Audio controls: Pause, unpause, skip forward.
			if (Helper.Keypress(Keys.N))
				eightBitWeapon.Play(Content.Load<Song>(playList[Helper.Rng(playList.Count - 1)]));
			if (Helper.Keypress(Keys.P))
				eightBitWeapon.Pause();
			if (Helper.Keypress(Keys.U))
				eightBitWeapon.Resume();
		}

		private void DrawPlay(GameTime gameTime)
		{
			eightBitWeapon.Volume(0.7F);

			// Draw HUD.
			DrawHud(gameTime);
			// Draw player.
			first.Draw(spriteBatch);
			// Draw gun. Should always be drawn after the player!
			gun.Draw(spriteBatch);
			// Bonus.
			bonus.Draw(spriteBatch);

			// Draw mine.
			for (int i = 0; i < mine.Count; i++)
				mine[i].Draw(spriteBatch);

			if (DEBUG)
			{
				// Outline textures if debug is enabled.
				spriteBatch.Draw(border, first.Rectangle(), Color.White * bOpacity);
				spriteBatch.Draw(border, mouse.Rectangle(), Color.White * bOpacity);
				spriteBatch.Draw(border, bonus.Rectangle(), Color.White * bOpacity);
				spriteBatch.Draw(border, gun.Rectangle(), Color.White * bOpacity);

				for (int i = 0; i < mine.Count; i++)
					spriteBatch.Draw(border, mine[i].Rectangle(), Color.White * bOpacity);
			}
		}

		private void DrawHud(GameTime gameTime)
		{
			int y = 30;

			List<string> hud = new List<string>()
			{
				"Score: " + Convert.ToString(scores.Current()),
				"Lives: " + Convert.ToString(first.RemainingLives())
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

		private void Switch(GameTime gameTime, SpriteBatch spriteBatch)
		{
			switch (state)
			{
				case Gamestate.Menu:
					{
						DrawMenu();
						break;
					}
				case Gamestate.Play:
					{
						DrawPlay(gameTime);
						break;
					}
				case Gamestate.New:
					{
						NewGame();
						break;
					}
				case Gamestate.Scores:
					{
						DrawScores();
						break;
					}
				case Gamestate.About:
					{
						DrawAbout();
						break;
					}
				case Gamestate.End:
					{
						DrawEnd();
						break;
					}
				case Gamestate.Death:
					{
						DrawDeath();
						break;
					}
			}
		}

		private void UpdateSwitch()
		{
			switch (state)
			{
				case Gamestate.Menu:
					{
						UpdateMenu();
						break;
					}
				case Gamestate.Play:
					{
						UpdatePlay();
						break;
					}
				case Gamestate.New:
					{
						NewGame();
						break;
					}
				case Gamestate.Scores:
					{
						UpdateScores();
						break;
					}
				case Gamestate.About:
					{
						UpdateAbout();
						break;
					}
				case Gamestate.End:
					{
						UpdateEnd();
						break;
					}
				case Gamestate.Death:
					{
						UpdateDeath();
						break;
					}
			}
		}
	}
}
