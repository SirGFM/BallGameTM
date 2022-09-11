using PlayerPrefs = UnityEngine.PlayerPrefs;

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
	/** How many particles are emitted per second. */
	static private float particleQuantity = 1.0f;
	/** Whether the low res models should be used. */
	static private bool lowResModels = false;

	private const bool defaultInvertCamX = false;
	private const bool defaultInvertCamY = false;
	private const bool defaultShowTimer = true;
	private const float defaultCamXSpeed = 1.0f;
	private const float defaultCamYSpeed = 1.0f;
	private const float defaultGlobalVolume = 1.0f;
	private const float defaultMusicVolume = 0.6f;
	private const float defaultSfxVolume = 0.4f;
	private const float defaultParticleQuantity = 1.0f;
	private const bool defaultLowResModels = false;

	/** Reset the configurations back to the default values. */
	static public void reset() {
		Config.saveBool("InvertCamX", defaultInvertCamX);
		PlayerPrefs.SetFloat("CamXSpeed", defaultCamXSpeed);
		Config.saveBool("InvertCamY", defaultInvertCamY);
		PlayerPrefs.SetFloat("CamYSpeed", defaultCamYSpeed);
		Config.saveBool("ShowTimer", defaultShowTimer);
		PlayerPrefs.SetFloat("GlobalVolume", defaultGlobalVolume);
		PlayerPrefs.SetFloat("MusicVolume", defaultMusicVolume);
		PlayerPrefs.SetFloat("SfxVolume", defaultSfxVolume);
		PlayerPrefs.SetFloat("MinDeadzone", Input.minAxis);
		PlayerPrefs.SetFloat("MaxDeadzone", Input.maxAxis);
		PlayerPrefs.SetFloat("ParticleQuantity", defaultParticleQuantity);
		Config.saveBool("LowResModels", defaultLowResModels);

		Config.savePlayerModel();

		for (int set = 0; set < 3; set++) {
			Input.RevertMap(set);
			Config.saveInput(set);
		}

		/* Ensure the audio is properly set. */
		setGlobalVolume(PlayerPrefs.GetFloat("GlobalVolume", defaultGlobalVolume));
		setMusicVolume(PlayerPrefs.GetFloat("MusicVolume", defaultMusicVolume));
		setSfxVolume(PlayerPrefs.GetFloat("SfxVolume", defaultSfxVolume));
	}

	/** Load the player configuration. */
	static public void load() {
		setHorCamInverted(Config.loadBool("InvertCamX", defaultInvertCamX));
		setHorCamSpeed(PlayerPrefs.GetFloat("CamXSpeed", defaultCamXSpeed));
		setVerCamInverted(Config.loadBool("InvertCamY", defaultInvertCamY));
		setVerCamSpeed(PlayerPrefs.GetFloat("CamYSpeed", defaultCamYSpeed));
		setInGameTimer(Config.loadBool("ShowTimer", defaultShowTimer));
		setGlobalVolume(PlayerPrefs.GetFloat("GlobalVolume", defaultGlobalVolume));
		setMusicVolume(PlayerPrefs.GetFloat("MusicVolume", defaultMusicVolume));
		setSfxVolume(PlayerPrefs.GetFloat("SfxVolume", defaultSfxVolume));
		setMinDeadzone(PlayerPrefs.GetFloat("MinDeadzone", Input.minAxis));
		setMaxDeadzone(PlayerPrefs.GetFloat("MaxDeadzone", Input.maxAxis));
		setParticleQuantity(PlayerPrefs.GetFloat("ParticleQuantity", defaultParticleQuantity));
		setLowResModels(Config.loadBool("LowResModels", defaultLowResModels));

		PlayerModel.Model = PlayerPrefs.GetInt("Model", PlayerModel.Model);
		PlayerModel.BaseColor = PlayerPrefs.GetInt("BaseColor", PlayerModel.BaseColor);
		PlayerModel.MainDetailColor = PlayerPrefs.GetInt("MainDetailColor", PlayerModel.MainDetailColor);
		PlayerModel.SubDetailColor = PlayerPrefs.GetInt("SubDetailColor", PlayerModel.SubDetailColor);

		for (int set = 0; set < 3; set++) {
			Config.loadInput(set);
		}
	}

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
		Config.saveBool("InvertCamX", v);
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
		PlayerPrefs.SetFloat("CamXSpeed", v);
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
		Config.saveBool("InvertCamY", v);
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
		PlayerPrefs.SetFloat("CamYSpeed", v);
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
		Config.saveBool("ShowTimer", v);
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
		PlayerPrefs.SetFloat("GlobalVolume", v);
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
		PlayerPrefs.SetFloat("MusicVolume", v);
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
		PlayerPrefs.SetFloat("SfxVolume", v);
	}

	/** Retrieve the current sound effects volume. */
	static public float getSfxVolume() {
		return Global.Sfx.sfxVolume;
	}

	/**
	 * Save the requested input set.
	 *
	 * @param set: The input set being saved.
	 */
	static public void saveInput(int set) {
		string key = $"Input-{set}";

		try {
			string data = Input.axisToJson(set);
			PlayerPrefs.SetString(key, data);
		} catch (System.Exception) {
		}
	}

	/**
	 * Load the requested input set.
	 *
	 * @param set: The input set being loaded.
	 */
	static private void loadInput(int set) {
		string key = $"Input-{set}";

		if (!PlayerPrefs.HasKey(key)) {
			return;
		}
		string data = PlayerPrefs.GetString(key, "");

		try {
			Input.axisFromJson(set, data);
		} catch (System.Exception) {
			Input.RevertMap(set);
		}
	}

	/**
	 * Load a boolean value from the configuration file.
	 *
	 * @param key: The name of the value.
	 * @param def: The default value, if it doesn't exist in the file.
	 */
	static private bool loadBool(string key, bool def) {
		if (!PlayerPrefs.HasKey(key)) {
			return def;
		}

		return PlayerPrefs.GetInt(key) == 1;
	}

	/**
	 * Save a boolean value to the configuration file.
	 *
	 * @param key: The name of the value.
	 * @param v: The value.
	 */
	static private void saveBool(string key, bool v) {
		PlayerPrefs.SetInt(key, (v ? 1 : 0));
	}

	/** Save the current player model/palette to the configuration file. */
	static public void savePlayerModel() {
		PlayerPrefs.SetInt("Model", PlayerModel.Model);
		PlayerPrefs.SetInt("BaseColor", PlayerModel.BaseColor);
		PlayerPrefs.SetInt("MainDetailColor", PlayerModel.MainDetailColor);
		PlayerPrefs.SetInt("SubDetailColor", PlayerModel.SubDetailColor);
	}

	/* Retrieve the axis minimum deadzone. */
	static public float getMinDeadzone() {
		return Input.minAxis;
	}

	/**
	 * Configure (and save) the axis minimum deadzone.
	 *
	 * @param v: The deadzone
	 */
	static public void setMinDeadzone(float v) {
		Input.minAxis = v;
		PlayerPrefs.SetFloat("MinDeadzone", v);
	}

	/* Retrieve the axis maximum deadzone. */
	static public float getMaxDeadzone() {
		return Input.maxAxis;
	}

	/**
	 * Configure (and save) the axis maximum deadzone.
	 *
	 * @param v: The deadzone
	 */
	static public void setMaxDeadzone(float v) {
		Input.maxAxis = v;
		PlayerPrefs.SetFloat("MaxDeadzone", v);
	}

	/* Retrieve the particle emission rate. */
	static public float getParticleQuantity() {
		return Config.particleQuantity;
	}

	/**
	 * Configure (and save) the particle emission rate.
	 *
	 * @param v: The emission rate
	 */
	static public void setParticleQuantity(float v) {
		Config.particleQuantity = v;
		PlayerPrefs.SetFloat("ParticleQuantity", v);
	}

	/** Retrieve whether the low res models should be used. */
	static public bool getLowResModels() {
		return Config.lowResModels;
	}

	/**
	 * Configure (and save) whether the low res models should be used.
	 *
	 * @param v: The the value
	 */
	static public void setLowResModels(bool v) {
		Config.lowResModels = v;
		Config.saveBool("LowResModels", v);
	}
}
