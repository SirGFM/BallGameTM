
static public class Config {
	/** Multiplier for flipping the camera in the horizontal axis. */
	static private float camDirX = 1.0f;
	/** Multiplier for the camera speed in the horizontal axis. */
	static private float camSpeedX = 1.0f;
	/** Multiplier for flipping the camera in the vertical axis. */
	static private float camDirY = 1.0f;
	/** Multiplier for the camera speed in the vertical axis. */
	static private float camSpeedY = 1.0f;
	/** Whether the in-game timer is visible or not. */
	static private bool timer = true;

	/**
	 * Configure (and save) whether the camera should be inverted on the
	 * horizontal axis.
	 *
	 * @param v: Whether the camera should be inverted.
	 */
	static public void setHorCamInverted(bool v) {
		if (v) {
			Config.camDirX = -1.0f;
		}
		else {
			Config.camDirX = 1.0f;
		}

		Global.camX = Config.camDirX * Config.camSpeedX;
		/* TODO: Save */
	}

	/** Retrieve whether the camera is inverted on the horizontal axis. */
	static public bool getHorCamInverted() {
		return (Config.camDirX < 0.0f);
	}

	/**
	 * Configure (and save) the camera's speed on the horizontal axis.
	 *
	 * @param v: The camera's speed on the horizontal axis.
	 */
	static public void setHorCamSpeed(float v) {
		Config.camSpeedX = v;

		Global.camX = Config.camDirX * Config.camSpeedX;
		/* TODO: Save */
	}

	/* Retrieve the camera's speed on the horizontal axis. */
	static public float getHorCamSpeed() {
		return Config.camSpeedX;
	}

	/**
	 * Configure (and save) whether the camera should be inverted on the
	 * vertical axis.
	 *
	 * @param v: Whether the camera should be inverted.
	 */
	static public void setVerCamInverted(bool v) {
		if (v) {
			Config.camDirY = -1.0f;
		}
		else {
			Config.camDirY = 1.0f;
		}

		Global.camY = Config.camDirY * Config.camSpeedY;
		/* TODO: Save */
	}

	/** Retrieve whether the camera is inverted on the vertical axis. */
	static public bool getVerCamInverted() {
		return (Config.camDirY < 0.0f);
	}

	/**
	 * Configure (and save) the camera's speed on the horizontal axis.
	 *
	 * @param v: The camera's speed on the horizontal axis.
	 */
	static public void setVerCamSpeed(float v) {
		Config.camSpeedY = v;

		Global.camY = Config.camDirY * Config.camSpeedY;
		/* TODO: Save */
	}

	/* Retrieve the camera's speed on the horizontal axis. */
	static public float getVerCamSpeed() {
		return Config.camSpeedY;
	}

	/**
	 * Configure (and save) the in-game timer visibility.
	 *
	 * @param v: Whether the in-game timer is visible.
	 */
	static public void setInGameTimer(bool v) {
		Config.timer = v;

		Global.showTimer = v;
		/* TODO: Save */
	}

	/** Retrieve whether the in-game timer is visible. */
	static public bool getInGameTimer() {
		return Config.timer;
	}

	/**
	 * Configure (and save) how loud the game as a whole is.
	 *
	 * @param v: The game's new volume.
	 */
	static public void setGlobalVolume(float v) {
		Global.Sfx.setGlobalVolume(v);
		/* TODO: Save */
	}

	static public float getGlobalVolume() {
		return Global.Sfx.getGlobalVolume();
	}

	/**
	 * Configure (and save) the volume for the game's music.
	 *
	 * @param v: The volume of the music.
	 */
	static public void setMusicVolume(float v) {
		Global.Sfx.setMusicVolume(v);
		/* TODO: Save */
	}

	/** Retrieve the current music volume. */
	static public float getMusicVolume() {
		return Global.Sfx.getMusicVolume();
	}

	/**
	 * Configure (and save) the volume for new sound effects.
	 *
	 * @param v: The volume for new sound effects.
	 */
	static public void setSfxVolume(float v) {
		Global.Sfx.sfxVolume = v;
		/* TODO: Save */
	}

	/** Retrieve the current sound effects volume. */
	static public float getSfxVolume() {
		return Global.Sfx.sfxVolume;
	}
}
