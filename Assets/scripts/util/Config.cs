
static public class Config {
	/**
	 * Configure (and save) whether the camera should be inverted on the
	 * horizontal axis.
	 *
	 * @param v: Whether the camera should be inverted.
	 */
	static public void setHorCamInverted(bool v) {
		if (v) {
			Global.camX = -1.0f;
		}
		else {
			Global.camX = 1.0f;
		}
		/* TODO: Save */
	}

	/** Retrieve whether the camera is inverted on the horizontal axis. */
	static public bool getHorCamInverted() {
		return (Global.camX < 0.0f);
	}

	/**
	 * Configure (and save) whether the camera should be inverted on the
	 * vertical axis.
	 *
	 * @param v: Whether the camera should be inverted.
	 */
	static public void setVerCamInverted(bool v) {
		if (v) {
			Global.camY = -1.0f;
		}
		else {
			Global.camY = 1.0f;
		}
		/* TODO: Save */
	}

	/** Retrieve whether the camera is inverted on the vertical axis. */
	static public bool getVerCamInverted() {
		return (Global.camY < 0.0f);
	}
}
