
static public class Global {
	/**
	 * Camera multiplier in the horizontal axis. May be used for camera
	 * sensitivity and inverting the camera.
	 */
    static public float camX = 1.0f;

	/**
	 * Camera multiplier in the vertical axis. May be used for camera
	 * sensitivity and inverting the camera.
	 */
    static public float camY = 1.0f;

	/**
	 * Implement a timer to track durations that may be temporarily paused.
	 *
	 * To start tracking time, call 'Start()'. The timer will start running
	 * and may be retrieved by calling 'ToString()'. Use 'Stop()' to
	 * temporarily pause the timer, keeping the accumulated elapsed time.
	 *
	 * To discard the currently accumulated timer, call 'Reset()'. This
	 * also stops the timer!
	 */
	public class Timer {
		/** The object used to measure elapsed time. */
		private System.Diagnostics.Stopwatch timer;

		/** The total accumulated time, regardless of when the timer was
		 * stopped. */
		private System.TimeSpan acc;

		public Timer() {
			this.timer = new System.Diagnostics.Stopwatch();
			this.acc = new System.TimeSpan();
		}

		/** Start the timer. */
		public void Start() {
			this.timer.Start();
		}

		/** Temporarily stop the timer. */
		public void Stop() {
			this.timer.Stop();
			this.acc = this.acc.Add(this.timer.Elapsed);
			this.timer.Reset();
		}

		/** Stop the timer and discard the accumulated timer. */
		public void Reset() {
			this.Stop();
			this.acc = new System.TimeSpan();
		}

		/**
		 * Convert the currently accumulated time to a string, even if the
		 * timer wasn't stopped.
		 *
		 * This conversion use only as many digits as needed, so (for
		 * example) it won't show the minutes digits until the timer has
		 * been running for at least one minute. However, the timer always
		 * shows the seconds digit (even before it's accumulated a single
		 * second).
		 *
		 * @return The time as a string.
		 */
		override public string ToString() {
			string ret = "";
			bool cont = false;

			System.TimeSpan cur = this.acc.Add(this.timer.Elapsed);

			if (cur.Days > 0) {
				ret += $"{cur.Days}d ";
				cont = true;
			}
			if (cont || cur.Hours > 0) {
				ret += $"{cur.Hours:00}:";
				cont = true;
			}
			if (cont || cur.Minutes > 0) {
				ret += $"{cur.Minutes:00}:";
				cont = true;
			}
			if (cont || cur.Seconds > 0) {
				ret += $"{cur.Seconds:00}.";
				cont = true;
			}

			if (!cont) {
				ret = "0.";
			}

			return ret + $"{cur.Milliseconds:000}";
		}
	}

	/** Whether the in-game time should be visibile. */
	static public bool showTimer = true;

	/**
	 * RTA ("real time attack") timer. Starts counting from new-game/level
	 * select, and continues until the game is over.
	 */
	static public Timer rtaTimer = new Timer();

	/**
	 * IGT ("in-game time") timer. Similar to RTA, but discounting load
	 * times.
	 */
	static public Timer igtTimer = new Timer();

	/** Track the duration of a single run in a single level. */
	static public Timer levelTimer = new Timer();
}
