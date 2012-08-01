using System;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Flyatron
{
	class Muzak
	{
		Song song;

		public Muzak()
		{
		}

		public void Play(Song inputSong)
		{
			song = inputSong;
			MediaPlayer.Play(song);
		}

		public bool Playing()
		{
			// Is it playing?
			if (MediaPlayer.State == MediaState.Playing)
				return true;		
			return false;
		}

		public void Pause()
		{
			// All-in-one switch.
			if (MediaPlayer.State == MediaState.Playing)
				MediaPlayer.Pause();
		}

		public void Resume()
		{
			if (MediaPlayer.State == MediaState.Paused)
				MediaPlayer.Resume();
		}

		public void Volume(float inputVolume)
		{
			MediaPlayer.Volume = inputVolume;
		}

		// Parse code stolen from RB Whitaker.
		// Source URL: http://rbwhitaker.wikidot.com/playing-background-music
		public string Parse(TimeSpan timeSpan)
		{
			int minutes = timeSpan.Minutes;
			int seconds = timeSpan.Seconds;

			if (seconds < 10)
				return minutes + ":0" + seconds;
			else
				return minutes + ":" + seconds;
		}

		// These functions return various information regarding the song.
		// Mostly a duplicate of existing functions, but I want the option of extensibility.
		public string Name()
		{
			// Song name.
			return song.Name.Replace("\\", ": ");
		}

		public string Elapsed()
		{
			return Parse(MediaPlayer.PlayPosition);
		}

		public string Length()
		{
			return Parse(song.Duration);
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
