using EvSys = UnityEngine.EventSystems;
using UiText = UnityEngine.UI.Text;

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

public class Loader : UnityEngine.MonoBehaviour {
}
