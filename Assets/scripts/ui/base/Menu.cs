using CoroutineRet = System.Collections.IEnumerator;
using SceneMng = UnityEngine.SceneManagement.SceneManager;
using SceneMode = UnityEngine.SceneManagement.LoadSceneMode;
using Time = UnityEngine.Time;

/**
 * Base class for menus.
 *
 * Each UI scene must implement its behaviour in a sub-class of this class.
 * The methods that should be overriden to specialize this class are:
 *
 *    - start(): Called from this component's Start() event.
 *    - ignoreInputs(): Returns whether or not the menu should handle
 *                      inputs. Can be used to ignore inputs while
 *                      rebinding the controls.
 *    - onLeft(): Called whenever "left" is pressed.
 *    - onRight(): Called whenever "right" is pressed.
 *    - onUp(): Called whenever "up" is pressed.
 *    - onDown(): Called whenever "down" is pressed.
 *    - onSelect(): Called whenever "select" is pressed.
 *    - onCancel(): Called whenever "cancel" is pressed.
 *
 * The behaviour for each of these functions depends on the actual scene.
 * In an options menu, onLeft() and onRight()  may be used to alter values,
 * while onUp() and onDown() may be used to select which option is being
 * modified.
 *
 * Only the Start() event is implemented by the class, so sub-classes may
 * freely implement handlers for any other events.
 */

public class Menu : BaseRemoteAction {
	/** How long should it take for a pressed down input to start
	 * repeating. */
	public float waitRepeat = 0.5f;

	/** After a pressed down input starts to repeat, how long should it
	 * take for each consecutive repetition. */
	public float holdRepeat = 0.15f;

	virtual protected bool ignoreInputs() {
		return false;
	}

	virtual protected void start() {
	}

	virtual protected void onLeft() {
	}

	virtual protected void onRight() {
	}

	virtual protected void onUp() {
	}

	virtual protected void onDown() {
	}

	virtual protected void onSelect() {
	}

	virtual protected void onCancel() {
	}

	private bool anyDirDown() {
		return Input.MenuLeft() || Input.MenuRight() || Input.MenuUp() ||
				Input.MenuDown();
	}

	private CoroutineRet handleInputs() {
		float delay = this.waitRepeat;

		while (true) {
			if (this.ignoreInputs()) {
				yield return null;
				continue;
			}

			if (Input.MenuSelect()) {
				/* TODO SFX */
				this.onSelect();
				while (Input.MenuSelect())
					yield return null;
			}
			else if (Input.MenuCancel()) {
				/* TODO SFX */
				this.onCancel();
				while (Input.MenuCancel())
					yield return null;
			}
			else if (anyDirDown()) {
				bool playSound = true;

				if (Input.MenuLeft())
					this.onLeft();
				else if (Input.MenuRight())
					this.onRight();
				else if (Input.MenuUp())
					this.onUp();
				else if (Input.MenuDown())
					this.onDown();
				else
					playSound = false;

				/* TODO SFX */

				for (float t = 0; t < delay && this.anyDirDown();
						t += Time.deltaTime) {
					/* Do nothing until timeout or the key is released */
					yield return null;
				}
				delay = this.holdRepeat;
			}
			else {
				/* No key pressed:
				 *   1. Reset the delay between repeated presses
				 *   2. Try-again next frame
				 */
				delay = this.waitRepeat;
				yield return null;
			}
		}
	}

	void Start() {
		this.start();
		this.StartCoroutine(this.handleInputs());
	}

	/** Whether any other scene is currently being loaded. */
	private bool isLoading = false;

	/**
	 * Load a given level.
	 *
	 * @param idx: The index of the level to be loaded (should start at 1).
	 */
	protected void LoadLevel(int idx) {
		if (this.isLoading)
			return;

		this.isLoading = true;
		Loader.LoadLevel(idx);
	}

	/**
	 * Start loading a scene alongside the current scene and waits until
	 * it finishes loading, so the loading flag may be cleared.
	 *
	 * @param scene: The scene to be loaded.
	 */
	private System.Collections.IEnumerator loadCombined(string scene) {
		yield return SceneMng.LoadSceneAsync(scene, SceneMode.Additive);
		this.isLoading = false;
	}

	/**
	 * Load a scene on top/alongside the current scene.
	 *
	 * @param scene: The scene to be loaded.
	 */
	protected void CombinedLoadScene(string scene) {
		if (this.isLoading)
			return;

		this.isLoading = true;
		this.StartCoroutine(this.loadCombined(scene));
	}

	/**
	 * Load a scene replacing the current scene.
	 *
	 * @param scene: The scene to be loaded.
	 */
	protected void LoadScene(string scene) {
		if (this.isLoading)
			return;

		this.isLoading = true;
		SceneMng.LoadSceneAsync(scene, SceneMode.Single);
	}
}
