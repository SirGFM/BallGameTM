using AsyncOp = UnityEngine.AsyncOperation;
using EvSys = UnityEngine.EventSystems;
using Scene = UnityEngine.SceneManagement.Scene;
using SceneMng = UnityEngine.SceneManagement.SceneManager;
using SceneMode = UnityEngine.SceneManagement.LoadSceneMode;
using UiText = UnityEngine.UI.Text;

/**
 * Loader loads levels in the background, managing the UI as necessary.
 * This component also assigns itself as the game's Root object, so it
 * may handle events from any loaded scene.
 *
 * While a stage is being loaded, the UI must send this object a
 * SetProgressBar event to signal which object has the UI, so the loader
 * may update it as the level is loaded. Similarly, the UI must send a
 * SetSceneTitle to set the name on the loading screen and within the
 * in-game UI. While in game, the UI may send SetTime events to update the
 * desired timer in the UI.
 *
 * This component also implementns OnReset, to reload the current stage
 * and OnGoal to load the next stage. The stage that will be loaded by
 * this component is stored in the global variable currentLevel, which
 * must be manually set to start a game from the first level, or to start
 * from the selected level.
 */

/** Identify UI scenes that may try to ask for the scene's name. */
public enum UiScene {
	LoadingScene,
	GameScene
}

/** Identify the timers tracked by the loader. */
public enum UiTimer {
	IgtTimer,
	LevelTimer,
	None, /* Used exclusively to hide the timer's descriptor. */
}

public interface LoaderIface : EvSys.IEventSystemHandler {
	/**
	 * SetProgressBar sends the scene's ProgressBar to another object.
	 *
	 * @param out done: Whether the event was handled.
	 * @param pb: The progress bar.
	 */
	void SetProgressBar(out bool done, ProgressBar pb);

	/**
	 * SetSceneTitle configures the element that shows the scene name. The
	 * name is slightly modified based on the scene type.
	 *
	 * @param out done: Whether the event was handled.
	 * @param txt: The text element to be updated with the scene name.
	 * @param ui: Which UI scene this element is on, so the name may be
	 *            customized accordingly.
	 */
	void SetSceneTitle(out bool done, UiText txt, UiScene ui);

	/**
	 * SetTimer configures the element that shows the requested timer.
	 *
	 * @param txt: The text element to be updated with the timer's value.
	 * @param timer: Which timer should be used.
	 */
	void SetTimer(UiText txt, UiTimer timer);

	/**
	 * Resets the current stage.
	 */
	void OnReset();

	/**
	 * Show the pause menu.
	 */
	void ShowPause();

	/**
	 * Hide the pause menu.
	 */
	void HidePause();
}

public class Loader : BaseRemoteAction, LoaderIface, GoalIface {
	/** XXX: The first scene in the game **must** be the mainmenu, while the
	 * second one is the first stage...
	 * To reset the game back to the first stage, this must be manually
	 * set back to 1. */
	static public int currentLevel = 1;

	/** Whether an end card is already playing, and thus other ones should be played. */
	private bool blockEndCard;

	/** The loader scene. */
	static private string loaderSceneName = "Loader";

	/** The scene shown on death. */
	private string loseSceneName = "OnLose";

	/** The scene shown when the player reaches a goal. */
	private string winSceneName = "OnWin";

	/** The scene shown when the pause is pressed. */
	private string pauseSceneName = "Pause";

	/** Name of the sub-scene used to display the loading progress. */
	public string uiScene = "LoadingUI";

	/** Name of the sub-scene used to display the game UI. */
	public string gameUiScene = "GameUI";

	/** UI progress bar. */
	private ProgressBar pb;

	/** The loader scene, used to reload it. */
	private int loaderScene;

	/** Whether a new scene is being loaded. */
	private bool loadingNew;

	/** Wehther resetting is blocked, as while the stage is being loaded. */
	private bool blockReset;

	/** Whether the pause scene has been loaded. */
	private bool isPauseLoaded;

	void Start() {
		this.blockEndCard = false;
		this.loadingNew = false;
		this.blockReset = true;
		this.isPauseLoaded = false;

		Global.rtaTimer.Start();

		this.loaderScene = SceneMng.GetActiveScene().buildIndex;

		BaseRemoteAction.setRootTarget(this.gameObject);
		this.StartCoroutine(this.load());
	}

	/**
	 * Reload the current scene. The actual stage that should be loaded
	 * depends on Loader.currentLevel's value.
	 */
	private void reloadScene() {
		if (this.loadingNew) {
			return;
		}

		Global.levelTimer.Stop();
		Global.igtTimer.Stop();
        SceneMng.LoadSceneAsync(this.loaderScene, SceneMode.Single);
		this.loadingNew = true;
	}

	public void OnGoal() {
		if (this.blockReset || this.loadingNew) {
			return;
		}

		/* Eventually, a game over scene which loops back to the main menu
		 * should be loaded. Therefore, currentLevel shouldn't ever
		 * overflow in a release build. However, this is useful when
		 * running in the editor to avoid issues. */
#if UNITY_EDITOR
		if (Loader.currentLevel >= SceneMng.sceneCountInBuildSettings) {
			UnityEngine.Debug.LogWarning("Tried to load a scene past the last index! Going back to the first level...");
			Loader.currentLevel = 1;
		}
#endif

		this.blockReset = true;
		this.startEndCard(this.winSceneName);
	}

	public void OnAdvanceLevel() {
		if (this.loadingNew) {
			return;
		}

		Loader.currentLevel++;
		this.reloadScene();
	}

	public void OnReset() {
		if (this.blockReset || this.loadingNew) {
			return;
		}

		this.reloadScene();
	}

	public void SetProgressBar(out bool done, ProgressBar pb) {
		this.pb = pb;
		done = true;
	}

	public void SetSceneTitle(out bool done, UiText txt, UiScene ui) {
		string separator;
		string levelName = LevelNameList.GetLevel(currentLevel);

		switch (ui) {
		case UiScene.LoadingScene:
			separator = "\n";
			break;
		case UiScene.GameScene:
			separator = ": ";
			break;
		default:
			throw new System.Exception($"Invalid UiScene ({ui})");
		}

		txt.text = $"Level {Loader.currentLevel}{separator}{levelName}";
		done = true;
	}

	public void SetTimer(UiText txt, UiTimer timer) {
		switch (timer) {
		case UiTimer.IgtTimer:
			txt.text = Global.igtTimer.ToString();
			break;
		case UiTimer.LevelTimer:
			txt.text = Global.levelTimer.ToString();
			break;
		default:
			throw new System.Exception($"Invalid UiTimer ({timer})");
		}
	}

	public void ShowPause() {
		if (!this.isPauseLoaded) {
			SceneMng.LoadSceneAsync(this.pauseSceneName, SceneMode.Additive);
			this.isPauseLoaded = true;
		}
	}

	public void HidePause() {
		if (this.isPauseLoaded) {
			SceneMng.UnloadSceneAsync(this.pauseSceneName);
			this.isPauseLoaded = false;
		}
	}

	/**
	 * Loads the current stage in background, updating the progress bar
	 * (if it was configured) as the stage is loaded.
	 */
	private System.Collections.IEnumerator load() {
		AsyncOp op;

		this.pb = null;
		op = SceneMng.LoadSceneAsync(this.uiScene, SceneMode.Additive);
		yield return op;

		string bg = LevelNameList.GetLevelBG(currentLevel);
		op = SceneMng.LoadSceneAsync(bg, SceneMode.Additive);
		if (op == null) {
			string lvl = LevelNameList.GetLevel(currentLevel);
			UnityEngine.Debug.LogWarning($"Couldn't load the background {bg} for level {lvl}!");
		}
		while (op != null && op.progress < 1.0f) {
			/* Update a progress bar */
			if (this.pb != null)
				this.pb.progress = op.progress * 0.1f;
			yield return new UnityEngine.WaitForFixedUpdate();
		}

		op = SceneMng.LoadSceneAsync(currentLevel, SceneMode.Additive);
		while (op.progress < 1.0f) {
			/* Update a progress bar */
			if (this.pb != null)
				this.pb.progress = 0.1f + op.progress * 0.85f;
			yield return new UnityEngine.WaitForFixedUpdate();
		}

		/* The active scene dictates where the lightning settings are read from.
		 * Thus, the level scene must be set as the active one!. */
		 while (true) {
			Scene curScene;

			curScene = SceneMng.GetSceneByBuildIndex(currentLevel);
			if (curScene != null && SceneMng.SetActiveScene(curScene)) {
				break;
			}
			yield return new UnityEngine.WaitForFixedUpdate();
		}

		Global.igtTimer.Start();
		Global.levelTimer.Reset();
		Global.levelTimer.Start();

		SceneMng.UnloadSceneAsync(this.uiScene);

		/* Load the game UI */
		op = SceneMng.LoadSceneAsync(this.gameUiScene, SceneMode.Additive);
		yield return op;

		this.blockReset = false;
	}

	/**
	 * Load a given level, configuring everything so level progression may
	 * work properly.
	 *
	 * @param idx: The index of the level to be loaded (should start at 1).
	 */
	static public void LoadLevel(int idx) {
		Loader.currentLevel = idx;
		Global.rtaTimer.Reset();
		Global.igtTimer.Reset();
		SceneMng.LoadSceneAsync(Loader.loaderSceneName, SceneMode.Single);
	}

	/**
	 * Load an end card scene.
	 *
	 * @param scene: The end card overlay scene.
	 */
	private void startEndCard(string scene) {
		if (!this.blockEndCard) {
			Global.igtTimer.Stop();
			Global.levelTimer.Stop();

			SceneMng.LoadSceneAsync(scene, SceneMode.Additive);
			this.blockEndCard = true;
		}
	}

	/** Load the scene shown on death. */
	public void OnRetryLevel() {
		this.startEndCard(this.loseSceneName);
	}
}
