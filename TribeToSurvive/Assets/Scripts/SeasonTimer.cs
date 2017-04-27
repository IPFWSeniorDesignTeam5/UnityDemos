using System;

namespace Tribal
{
	public static class SeasonTimer
	{
		public enum SeasonType
		{
			Spring = 1,
			Summer = 2,
			Fall = 3,
			Winter = 0
		}

		static float seasonTime = 0, currentTime = 0, lastUpdate = 0;

		static bool paused;

		public static float SeasonLength {get; private set;}

		public delegate void SeasonEndEventListener( SeasonEndEventArgs e );

		public static event SeasonEndEventListener SeasonEndEvent;

		public static SeasonType CurrentSeason { get; private set; }

		public static float SeasonProgress {get{ return seasonTime / SeasonLength; }}

		static SeasonTimer()
		{
			CurrentSeason = SeasonType.Winter;
			seasonTime = 0f;
			currentTime = 0f;
		}

		public class SeasonEndEventArgs : EventArgs
		{
			public SeasonType end {get; private set;}
			public SeasonType start {get; private set;}

			public SeasonEndEventArgs( SeasonType last, SeasonType next )
			{
				end = last;
				start = next;
			}
		}

		public static void SetSeasonLength (float seasonLength )
		{
			SeasonLength = seasonLength;
		}

		public static void Pause()
		{
			paused = true;
		}

		public static void Resume()
		{
			lastUpdate = 0;
			paused = false;
		}

		public static void Update( float time)
		{
			if( !paused )
			{
				seasonTime += time - lastUpdate;
				currentTime += time - lastUpdate;

				lastUpdate = time;

				if( seasonTime >= SeasonLength )
				{
					SeasonEnd();
				}	
			}
		}

		private static void SeasonEnd()
		{
			SeasonType old, next;

			old = CurrentSeason;
			next = (SeasonType)((((int)CurrentSeason) + 1) % 4);

			seasonTime = 0f;

			CurrentSeason = next;

			if( null != SeasonEndEvent )
				SeasonEndEvent( new SeasonEndEventArgs( old, next ) );
		}
	}
}

