
static public class Config {
	/** Multiplier for flipping the camera in the horizontal axis. */
	static private float camDirX = 1.0f;
	/** Multiplier for the camera speed in the horizontal axis. */
	static private float camSpeedX = 1.0f;
	/** Multiplier for flipping the camera in the vertical axis. */
	static private float camDirY = 1.0f;
	/** Multiplier for the camera speed in the vertical axis. */
	static private float camSpeedY = 1.0f;

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
}
