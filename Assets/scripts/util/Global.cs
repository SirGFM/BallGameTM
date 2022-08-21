using AudioClip = UnityEngine.AudioClip;
using AudioSource = UnityEngine.AudioSource;
using GO = UnityEngine.GameObject;

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

	/** Sfx holds every method used to play sound effects. */
	static public class Sfx {
		/** The global object playing the music and storing all the loaded
		 * audio clips. */
		static private AudioLoader audioLoader;

		/**
		 * Configure the Sfx's audio loader.
		 *
		 * @param audioLoader: The audio loader.
		 */
		static public void setAudioLoader(AudioLoader audioLoader) {
			Global.Sfx.audioLoader = audioLoader;
		}

		/** The game's overall volume. This shouldn't be set directly, as the audio
		 * listener must be updated as well. */
		static private float globalVolume = 1.0f;

		/**
		 * Update the game's overall volume.
		 *
		 * @param val: The new volume.
		 */
		static public void setGlobalVolume(float val) {
			Global.Sfx.globalVolume = val;

			if (Global.Sfx.audioLoader != null) {
				Global.Sfx.audioLoader.setGlobalVolume(val);
			}
		}

		/** Retrieve the game's overall volume. */
		static public float getGlobalVolume() {
			return Global.Sfx.globalVolume;
		}

		/** The music's volume. This shouldn't be set directly, as the music
		 * source must be updated as well. */
		static private float musicVolume = 0.6f;

		/**
		 * Update the volume of the currently playing music.
		 *
		 * @param val: The new volume.
		 */
		static public void setMusicVolume(float val) {
			Global.Sfx.musicVolume = val;

			if (Global.Sfx.audioLoader != null) {
				Global.Sfx.audioLoader.setMusicVolume(val);
			}
		}

		/** Retrieve the volume of the currently playing music. */
		static public float getMusicVolume() {
			return Global.Sfx.musicVolume;
		}

		/** The volume for newly played sound effects. */
		static public float sfxVolume = 0.4f;

		/**
		 * Play a sound effect.
		 *
		 * @param clip: The audio clip to be played.
		 * @param throughTransition: Make the audio persist through scene transition.
		 */
		static private void playAudio(AudioClip clip, bool throughTransition=false) {
			if (clip == null || Global.Sfx.sfxVolume <= 0.0f) {
				return;
			}

			/* Create a new object, since every sound effect needs its own
			 * audio source. */
			GO obj = new GO();

			if (throughTransition) {
				GO.DontDestroyOnLoad(obj);
			}

			AudioSource player = obj.AddComponent<AudioSource>();
			player.clip = clip;
			/* Makes this a "2D sound" (i.e., its position is ignored). */
			player.spatialBlend = 0.0f;
			player.volume = Global.Sfx.sfxVolume;
			player.Play();
			obj.AddComponent<DestroyOnAudioDone>();
		}

		static public void playMoveMenu() {
			Global.Sfx.playAudio(Global.Sfx.audioLoader?.sfxMoveMenu);
		}

		static public void playEnterMenu() {
			Global.Sfx.playAudio(Global.Sfx.audioLoader?.sfxEnterMenu, true);
		}

		static public void playCancelMenu() {
			Global.Sfx.playAudio(Global.Sfx.audioLoader?.sfxCancelMenu, true);
		}

		static public void playJump() {
			Global.Sfx.playAudio(Global.Sfx.audioLoader?.sfxJump);
		}

		static public void playFall() {
			Global.Sfx.playAudio(Global.Sfx.audioLoader?.sfxFall);
		}

		static public void playExplode() {
			Global.Sfx.playAudio(Global.Sfx.audioLoader?.sfxExplode);
		}

		static public void playVictoryOpening() {
			Global.Sfx.playAudio(Global.Sfx.audioLoader?.sfxVictoryOpening);
		}

		static public void playVictory() {
			Global.Sfx.playAudio(Global.Sfx.audioLoader?.sfxVictory);
		}

		static public void playDefeatOpening() {
			Global.Sfx.playAudio(Global.Sfx.audioLoader?.sfxDefeatOpening);
		}

		static public void playDefeat() {
			Global.Sfx.playAudio(Global.Sfx.audioLoader?.sfxDefeat);
		}

		static public void playCollectible() {
			Global.Sfx.playAudio(Global.Sfx.audioLoader?.sfxCollectible);
		}

		static public void playOpenDoor() {
			Global.Sfx.playAudio(Global.Sfx.audioLoader?.sfxOpenDoor);
		}
	}
}
