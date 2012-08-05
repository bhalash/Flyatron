using System;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Flyatron
{
	class Muzak
	{
		Song[] playlist;
		Song currentSong;

		public Muzak(Song[] inputPlaylist)
		{
			playlist = inputPlaylist;
			currentSong = playlist[0];

			if (!Game.DEBUG)
				Play();
		}

		public void Update()
		{
			// Audio controls: Pause, unpause, skip forward.
			if (Helper.Keypress(Keys.N))
				currentSong = playlist[Helper.Rng(playlist.Length - 1)];
			if (Helper.Keypress(Keys.P))
				Pause();
			if (Helper.Keypress(Keys.U))
				Resume();
		}

		public void Pause()
		{
			if (MediaPlayer.State == MediaState.Playing)
				MediaPlayer.Pause();
		}

		public void Resume()
		{
			if (MediaPlayer.State == MediaState.Paused)
				MediaPlayer.Resume();
		}

		public void Play()
		{
			MediaPlayer.Play(currentSong);
		}

		public bool Playing()
		{
			// Is it playing?
			if (MediaPlayer.State == MediaState.Playing)
				return true;		
			return false;
		}

		public void Volume(float inputVolume)
		{
			MediaPlayer.Volume = inputVolume;
		}

		public string Parse(TimeSpan timeSpan)
		{
			// Parse code stolen from RB Whitaker.
			// Source URL: http://rbwhitaker.wikidot.com/playing-background-music
			int minutes = timeSpan.Minutes;
			int seconds = timeSpan.Seconds;

			if (seconds < 10)
				return minutes + ":0" + seconds;
			else
				return minutes + ":" + seconds;
		}

		public string Name()
		{
			// Song name.
			return currentSong.Name.Replace("\\", ": ");
		}

		public string Elapsed()
		{
			return Parse(MediaPlayer.PlayPosition);
		}

		public string Length()
		{
			return Parse(currentSong.Duration);
		}

		public string Time()
		{
			return Elapsed() + " / " + Length();
		}

		public string NameTime()
		{
			if (!Playing())
				return Name() + " [Paused]";
			return Name() + " " + Time();
		}
	}
}
