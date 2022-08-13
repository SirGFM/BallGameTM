using AudioClip = UnityEngine.AudioClip;
using AudioSource = UnityEngine.AudioSource;
using CoroutineRet = System.Collections.IEnumerator;
using GO = UnityEngine.GameObject;
using Scene = UnityEngine.SceneManagement.Scene;
using SceneMng = UnityEngine.SceneManagement.SceneManager;
using SceneMode = UnityEngine.SceneManagement.LoadSceneMode;

/**
 * AudioLoader, quite literally, is a holder for all audio assets in the game.
 *
 * Additionally, since every game needs some global object to keep playing
 * songs through scenes, this object has a global Audio Source used to play
 * musics.
 *
 * Music control is tied to scene loading. Each song has a selector string
 * that, if contained in the path of the loaded scene, changes the currently
 * playing song.
 *
 * ---
 *
 * Simply adding an AudioClip to an object causes it to be added to the game.
 * Also, if its preloadAudioData field is set, then it will be loaded
 * automatically without any further intervention needed (when the scene is
 * loaded, I assume).
 *
 * The alternative to holding each audio clip in its on variable would be to
 * place the audios in a 'Resources' directory* and then load the audios
 * asynchronously in a coroutine. However, this would require keeping a list of
 * audios files, since there's no way to list the files in the 'Resources'
 * directory, and then figuring out some way to access these loaded files.
 *
 * If loading starts taking too long, this object could be moved to a separated
 * scene, which could then be loaded asynchronously with a progress bar in the
 * main scene... This is better then setting loadInBackground in the audios,
 * since there's no way to know the progress of the audio clips when using that.
 *
 *   * That's literally a directory named 'Resources' under 'Assets/'.
 */

public class AudioLoader : UnityEngine.MonoBehaviour {

	/* Path of levels shouldn't update the song in any way. */
	public string[] ignoredSelector = {
		"levels/Loader",
	};

	public AudioClip songChillBeginnings;

	public AudioClip songGrassyFields;

	/* Path of levels that use the grassy fields song. */
	public string grassyFieldsSelector = "levels/00-basic";

	public AudioClip sfxMoveMenu;
	public AudioClip sfxEnterMenu;
	public AudioClip sfxCancelMenu;
	public AudioClip sfxJump;
	public AudioClip sfxFall;
	public AudioClip sfxExplode;
	public AudioClip sfxVictory;
	public AudioClip sfxDefeatOpening;
	public AudioClip sfxDefeat;

	/** The main menu scene, loaded if there are no connected gamepad. */
	public string mainMenuScene = "MainMenu";

	/** The gamepad calibration scene, loaded if there are connected gamepads. */
	public string calibrationScene = "RecalibrateGamepad";

#if UNITY_EDITOR
	/** Mute the music while in the editor. */
	public bool muteMusic = true;
#endif

	/** The object used to play music through scenes. */
	private AudioSource musicPlayer;

	void Start() {
		Global.Sfx.setAudioLoader(this);

		this.musicPlayer = this.gameObject.AddComponent<AudioSource>();
		/* Makes this a "2D sound" (i.e., its position is ignored). */
		this.musicPlayer.spatialBlend = 0.0f;
		/* Initialize the volume to a safe default,
		 * later overriden by the configuration. */
		this.musicPlayer.volume = 0.25f;

#if UNITY_EDITOR
		this.musicPlayer.mute = this.muteMusic;
#endif

		SceneMng.activeSceneChanged += this.loadMusic;

		this.StartCoroutine(this.run());
		this.StartCoroutine(this.startSong());
		GO.DontDestroyOnLoad(this.gameObject);
	}

	/**
	 * Update the volume of the currently playing music.
	 *
	 * @param val: The new volume.
	 */
	public void setMusicVolume(float val) {
		this.musicPlayer.volume = val;
	}

	/**
	 * Start playing a song.
	 *
	 * @param clip: The song.
	 */
	private void playSong(AudioClip clip) {
		if (clip != null && this.musicPlayer.clip != clip) {
			this.musicPlayer.clip = clip;
			this.musicPlayer.Play();
			this.musicPlayer.loop = true;
		}
	}

	/**
	 * Event called whenever the active scene changes to play the scene's song.
	 *
	 * @param current: The current scene.
	 * @param next: The new active scene.
	 */
	private void loadMusic(Scene current, Scene next) {
		foreach (string ignored in this.ignoredSelector) {
			if (next.path.Contains(ignored)) {
				return;
			}
		}

		if (next.path.Contains(this.grassyFieldsSelector)) {
			this.playSong(this.songGrassyFields);
		}
		else {
			this.playSong(this.songChillBeginnings);
		}
	}

	/** Check that every audio was loaded and then change to the next scene. */
	private CoroutineRet run() {
		/* Check whether every AudioClip field in this class is loaded. */
		var fields = this.GetType().GetFields();

		for (int i = 0; i < fields.Length; i++) {
			var f = fields[i];

			AudioClip clip = f.GetValue(this) as AudioClip;
			if (!clip) {
				continue;
			}
			else if (clip.loadState == UnityEngine.AudioDataLoadState.Unloaded) {
				clip.LoadAudioData();
			}
			else if (clip.loadState == UnityEngine.AudioDataLoadState.Failed) {
				throw new System.Exception($"Couldn't load '{f.Name}'");
			}

			/* Bail checking the remaining clips if any isn't loaded yet. */
			if (clip.loadState != UnityEngine.AudioDataLoadState.Loaded) {
				i = -1;
				yield return null;
				continue;
			}
		}

		yield return null;

		/* This will only be reached if every AudioClip finished loading.
		 * Check if there's any named joystick,
		 * as that indicates that there's a connected joystick. */
		foreach (string name in UnityEngine.Input.GetJoystickNames()) {
			if (name.Length > 0) {
				SceneMng.LoadSceneAsync(this.calibrationScene, SceneMode.Single);
				yield break;
			}
		}
		SceneMng.LoadSceneAsync(this.mainMenuScene, SceneMode.Single);
	}

	/** Start playing the menu song as soon as it's loaded. */
	private CoroutineRet startSong() {
		/* Wait until the main menu song is loaded. */
		while (this.songChillBeginnings.loadState != UnityEngine.AudioDataLoadState.Loaded) {
			yield return null;
		}

		this.playSong(this.songChillBeginnings);
	}
}
