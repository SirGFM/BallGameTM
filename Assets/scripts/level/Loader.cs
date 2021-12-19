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
 * in-game UI.
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
	 * Resets the current stage.
	 */
	void OnReset();
}

public class Loader : BaseRemoteAction, LoaderIface, GoalIface {
	/** XXX: The first scene in the game **must** be the mainmenu, while the
	 * second one is the first stage...
	 * To reset the game back to the first stage, this must be manually
	 * set back to 1. */
	static public int currentLevel = 1;

	/** The loader scene. */
	static private string loaderSceneName = "Loader";

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

	void Start() {
		this.loadingNew = false;
		this.blockReset = true;

		this.loaderScene = SceneMng.GetActiveScene().buildIndex;

		BaseRemoteAction.setRootTarget(this.gameObject);
		this.StartCoroutine(this.load());
	}

	/**
	 * Reload the current scene. The actual stage that should be loaded
	 * depends on Loader.currentLevel's value.
	 */
	private void reloadScene() {
        SceneMng.LoadSceneAsync(this.loaderScene, SceneMode.Single);
		this.loadingNew = true;
	}

	public void OnGoal() {
		if (this.loadingNew) {
			return;
		}

		Loader.currentLevel++;

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

	/**
	 * Loads the current stage in background, updating the progress bar
	 * (if it was configured) as the stage is loaded.
	 */
	private System.Collections.IEnumerator load() {
		AsyncOp op;

		this.pb = null;
		op = SceneMng.LoadSceneAsync(this.uiScene, SceneMode.Additive);
		yield return op;

		op = SceneMng.LoadSceneAsync(currentLevel, SceneMode.Additive);
		while (op.progress < 1.0f) {
			/* Update a progress bar */
			if (this.pb != null)
				this.pb.progress = op.progress * 0.95f;
			yield return new UnityEngine.WaitForFixedUpdate();
		}

		SceneMng.UnloadSceneAsync(this.uiScene);

		/* TODO: Load the game UI */
#if false
		op = SceneMng.LoadSceneAsync(this.gameUiScene, SceneMode.Additive);
		yield return op;
#endif

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
		SceneMng.LoadSceneAsync(Loader.loaderSceneName, SceneMode.Single);
	}
}
