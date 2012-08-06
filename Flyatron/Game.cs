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

		public static int WIDTH  = 1366;
		public static int HEIGHT = 768;
		static bool FULLSCREEN   = true;

		public static Rectangle BOUNDS = new Rectangle(0, 0, WIDTH, HEIGHT);

		static double VERSION = 1.1;
		static string VERSIONSTRING = "Version " + VERSION;

		enum Gamestate { Menu, Play, New, Scores, About, End, Death };
		Gamestate state = Gamestate.Menu;

		public static KeyboardState PREV_KEYBOARD, KEYBOARD;
		public static MouseState PREV_MOUSE, MOUSE;

		public static Game Instance;

		Texture2D border;
		float borderOpacity;

		static SpriteBatch spriteBatch;
		GraphicsDeviceManager graphics;

		// Player.
		Player PLAYER;
		Texture2D[] playerTextures;
		
		// Gun.
		Gun gun;
		Texture2D[] gunTextures;

		// Fear, fire foes.
		Texture2D[] mineTextures;
		List<Mine> mines;
		int totalMines;

		// Bonuses.
		Bonus bonus;

		// Scoreboard.
		Scoreboard scores;
		bool collated;

		// Backdrop.
		Texture2D[] backdropTex;
		Backdrop cloudyBackdrop;

		// Mouse.
		Flymouse mouse;

		// Event timer.
		Stopwatch deathScreenTimer;

		// Texture for the menu. Declared here since I use it in several places.
		Texture2D menuBg;
		Vector2 menuVec;

		// Player-held bombs.
		int playerNukes;

		// Music sourced from the chiptunes group "8-Bit Weapon". Used with permission.
		// Homepage: http://www.8bitweapon.com
		Song[] eightBitWeapon;
		Muzak soundtrack;

		// Flyatron uses the font "Press Start 2P". The font was created by William Cody of Zone38.
		// The font is distributed under the permissive SIL Open Font License (OFL), and included in
		// this project under it, along with creator attribution.
		// Font download page:	 http://www.zone38.net/font/
		// OFL license homepage: http://scripts.sil.org/cms/scripts/page.php?site_id=nrsi&id=OFL
		public static SpriteFont FONT07, FONT10, FONT14, FONT25;

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
			eightBitWeapon = new Song[]
			{ 
				Content.Load<Song>("8 Bit Weapon/Chip On Your Shoulder"), 
				Content.Load<Song>("8 Bit Weapon/Closer (Bitpop Mix)"),
				Content.Load<Song>("8 Bit Weapon/Micro Boogie"),
				Content.Load<Song>("8 Bit Weapon/One Last Mission"), 
				Content.Load<Song>("8 Bit Weapon/Times Changing")
			};

			// Initialize soundtrack audio.
			soundtrack = new Muzak(eightBitWeapon);

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
			FONT07 = Content.Load<SpriteFont>("fonts\\PressStart2P_07");
			FONT10 = Content.Load<SpriteFont>("fonts\\PressStart2P_10");
			FONT14 = Content.Load<SpriteFont>("fonts\\PressStart2P_14");
			FONT25 = Content.Load<SpriteFont>("fonts\\PressStart2P_25");

			// Initialize menu backdrop.
			menuBg = Content.Load<Texture2D>("bg\\paused");
			menuVec = new Vector2(0, 0);
		}

		protected override void Initialize()
		{
			// Border to outline content in debug.
			border = Content.Load<Texture2D>("border");

			playerTextures = new Texture2D[]
			{
				// Player textures.
				Content.Load<Texture2D>("player\\body"),
				Content.Load<Texture2D>("player\\head"),
				Content.Load<Texture2D>("player\\fire")
			};

			mineTextures = new Texture2D[]
			{
				Content.Load<Texture2D>("mine\\core"),
				Content.Load<Texture2D>("mine\\spikes"),
				Content.Load<Texture2D>("mine\\explosion"),
				Content.Load<Texture2D>("border")
			};

			// Gun.
			gunTextures = new Texture2D[]
			{
				Content.Load<Texture2D>("gun\\gun"),
				Content.Load<Texture2D>("gun\\bullet"),
				Content.Load<Texture2D>("border")
			};

			// Load player art/stats.
			PLAYER = new Player(playerTextures);

			gun = new Gun(gunTextures);

			// Initialize bonus.
			bonus = new Bonus(Content.Load<Texture2D>("bonus\\bonus"));

			// Initialize mouse.
			mouse = new Flymouse(Content.Load<Texture2D>("pointer"));

			// Initialize SpriteBatch.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			// Load timers.
			deathScreenTimer = new Stopwatch();

			// Initialize foe mine.
			mines = new List<Mine>();

			// Debug. Border opacity.
			borderOpacity = 0.3F;

			// Default 1 bomb.
			playerNukes = 1;

			// Total mines. Fewer for debug so I can study behaviour
			totalMines = 35;

			if (DEBUG)
				totalMines = 9;

			for (int i = 0; i < totalMines; i ++)
				mines.Add(new Mine(mineTextures));

			// Import top scores.
			scores = new Scoreboard();
			collated = false;

			base.Initialize();
		}

		protected override void UnloadContent()
		{
		}

		protected override void Update(GameTime gameTime)
		{
			KEYBOARD = Keyboard.GetState();
			MOUSE = Mouse.GetState();

			mouse.Update();
			// Update SCREEN selection.
			UpdateSwitch();

			if (Helper.Keypress(Keys.Escape))
			{
				// Toggle between menu and gameplay.
				if (state == Gamestate.Play)
					state = Gamestate.Menu;
				else if ((state == Gamestate.Menu) && (PLAYER.RemainingLives() > 0))
					state = Gamestate.Play;
			}

			base.Update(gameTime);

			PREV_MOUSE = MOUSE;
			PREV_KEYBOARD = KEYBOARD;
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
			collated = false;
			scores.Reset();
			PLAYER.Lives(5);
			playerNukes = 1;

			for (int i = 0; i < mines.Count; i++)
				mines[i].State(2);

			state = Gamestate.Play;
		}

		private void UpdateDeath()
		{
			if (!collated)
			{
				scores.Collate();
				collated = true;
			}

			for (int i = 0; i < mines.Count; i++)
				mines[i].State(3);

			PLAYER.X(100);
			PLAYER.Y(HEIGHT / 2 - PLAYER.Rectangle().Height / 2);
			PLAYER.Minus();

			if (PLAYER.RemainingLives() <= 0)
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
			if ((Helper.Keypress(Keys.D1)) && (PLAYER.RemainingLives() > 0))
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
			soundtrack.Volume(0.5F);
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
			spriteBatch.DrawString(FONT25, title, new Vector2(50, 50), Color.White);
			// Draw the menu.
			for (int i = 0; i < opt.Count; i++)
			{
				spriteBatch.DrawString(FONT14, (i + 1) + ". " + opt[i], new Vector2(50, Y), Color.White);
				Y += 35;
			}

			if (DEBUG)
				spriteBatch.DrawString(
					FONT10,
					VERSIONSTRING,
					new Vector2(WIDTH - 30 - FONT10.MeasureString(VERSIONSTRING).Length(), HEIGHT - 30),
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
				"",
				"091 Labs:",
				"Alanna Kelly",
				"Domhall Walsh",
				"Duncan Thomas",
				"",
				"IRC (##xna):",
				"NullSoldier",
				"tert13",
				"",
				"Also:",
				"8-Bit Weapon",
				"Jennifer Tidmore (just because :)",
			};

			string title = "About Flyatron";

			cloudyBackdrop.Update(3);
			spriteBatch.Draw(menuBg, menuVec, Color.White);
			spriteBatch.DrawString(FONT25, title, new Vector2(x, 50), Color.White);

			for (int i = 0; i < messages.Count; i++)
			{
				spriteBatch.DrawString(FONT10, messages[i], new Vector2(x, y), Color.White);
				y += 20;
			}
		}

		private void UpdateEnd()
		{
			for (int i = 0; i < mines.Count; i++)
				mines[i].State(2);

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

			Vector2 fontVector = new Vector2(WIDTH / 2 - FONT25.MeasureString(message).Length() / 2, HEIGHT / 2 - 25);

			if ((deathScreenTimer.ElapsedMilliseconds > 1000) && (deathScreenTimer.ElapsedMilliseconds <= 2000))
			{
				spriteBatch.Draw(menuBg, menuVec, color * 0.25F);
				spriteBatch.DrawString(FONT25, message, fontVector, color * 0.25F);
			}
			if ((deathScreenTimer.ElapsedMilliseconds > 2000) && (deathScreenTimer.ElapsedMilliseconds <= 3000))
			{
				spriteBatch.Draw(menuBg, menuVec, color * 0.5F);
				spriteBatch.DrawString(FONT25, message, fontVector, color * 0.5F);
			}
			if ((deathScreenTimer.ElapsedMilliseconds > 3000) && (deathScreenTimer.ElapsedMilliseconds <= 4000))
			{
				spriteBatch.Draw(menuBg, menuVec, color * 0.75F);
				spriteBatch.DrawString(FONT25, message, fontVector, color * 0.75F);
			}
			if (deathScreenTimer.ElapsedMilliseconds > 4000)
			{
				spriteBatch.Draw(menuBg, menuVec, color * 1.0F);
				spriteBatch.DrawString(FONT25, message, fontVector, color * 1.0F);
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
			spriteBatch.DrawString(FONT25, title, new Vector2(50, 50), Color.White);
			scores.Report(FONT14, spriteBatch, 55, 100, Color.White);
		}

		private void UpdatePlay()
		{
			// Update player + gun position..
			PLAYER.Update();
			gun.Update(PLAYER.Position());
			// Increment mine position.
			for (int i = 0; i < mines.Count; i++)
				mines[i].Update(PLAYER.Rectangle());
			// Music.
			soundtrack.Update();
			// Calculate any collisions.
			Collisions();
			// Update backdrop.
			cloudyBackdrop.Update(5);
			// Tick scores.
			scores.Update();
			// Increment bonus position.
			bonus.Update();
			// Goodnight, sweet prince.
			if (PLAYER.RemainingLives() <= 0)
			{
				state = Gamestate.End;
			}
		}

		private void Collisions()
		{
			// TODO: Put somewhere better.
			if (Helper.RightClick())
				if (playerNukes > 0)
				{
					playerNukes--;

					for (int i = 0; i < mines.Count; i++)
					{
						mines[i].State(3);
						scores.Bump(10);
					}
				}

			// Player/mine + mine/bullet collisions.
			for (int i = 0; i < mines.Count; i++)
			{
				if (!DEBUG)
				{
					// Player/mine. Condition to not interact if it is animating an explosion.
					if (Helper.CircleCollision(PLAYER.Rectangle(), mines[i].Rectangle()))
						if (mines[i].ReportState() == 1)
							state = Gamestate.Death;
				}

				// Bullet/mine.
				for (int j = 0; j < Gun.BULLETS.Count; j++)
					if ((Helper.CircleCollision(mines[i].Rectangle(), Gun.BULLETS[j].Rectangle()) && (mines[i].ReportState() != 4)))
					{
						Gun.BULLETS[j].State(3);
						mines[i].State(4, Gun.BULLETS[j].Position());
						scores.Bump(10);
					}
			}

			// Player/bonus collision.
			if (Helper.CircleCollision(PLAYER.Rectangle(), bonus.Rectangle()))
			{
				if (bonus.ReportType() == 1)
					bonus.State(2);
					PLAYER.Lives(1);
				if (bonus.ReportType() == 2)
					playerNukes++;

				bonus.State(2);
			}
		}

		private void DrawPlay(GameTime gameTime)
		{
			soundtrack.Volume(0.7F);

			// Draw HUD.
			DrawHud(gameTime);
			// Draw player.
			PLAYER.Draw(spriteBatch);
			// Draw gun. Should always be drawn after the player!
			gun.Draw(spriteBatch);
			// Bonus.
			bonus.Draw(spriteBatch);

			// Draw mine.
			for (int i = 0; i < mines.Count; i++)
				mines[i].Draw(spriteBatch);

			if (DEBUG)
			{
				// Outline textures if debug is enabled.
				spriteBatch.Draw(border, PLAYER.Rectangle(), Color.White * borderOpacity);
				spriteBatch.Draw(border, mouse.Rectangle(),  Color.White * borderOpacity);
				spriteBatch.Draw(border, bonus.Rectangle(),  Color.White * borderOpacity);
			}
		}

		private void DrawHud(GameTime gameTime)
		{
			int y = 30;

			List<string> hud = new List<string>()
			{
				"Score: " + Convert.ToString(scores.Current()),
				"Lives: " + Convert.ToString(PLAYER.RemainingLives()),
				"Nukes: " + Convert.ToString(playerNukes)
			};

			for (int i = 0; i < hud.Count; i++)
			{
				spriteBatch.DrawString(
					FONT14,
					hud[i],
					new Vector2(30, y),
					Color.Black
				);

				y += 30;
			}

			if (!DEBUG)
				spriteBatch.DrawString(
					FONT10,
					soundtrack.NameTime(),
					new Vector2(GraphicsDevice.Viewport.X + 25, GraphicsDevice.Viewport.Height - 30),
					Color.Black
				);

			if (DEBUG)
				spriteBatch.DrawString(
					FONT07,
					Convert.ToString(gameTime.TotalGameTime),
					new Vector2(30, HEIGHT - 20),
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
