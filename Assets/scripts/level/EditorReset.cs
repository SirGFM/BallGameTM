using SceneMng = UnityEngine.SceneManagement.SceneManager;
using SceneMode = UnityEngine.SceneManagement.LoadSceneMode;
using UiText = UnityEngine.UI.Text;

/**
 * EditorReset reloads the current level if it detects the appropriate event.
 */

public class EditorReset : UnityEngine.MonoBehaviour, LoaderIface {

	public void OnReset() {
		int cur = SceneMng.GetActiveScene().buildIndex;

		SceneMng.LoadSceneAsync(cur, SceneMode.Single);
	}

	public void SetProgressBar(out bool done, ProgressBar pb) {
		done = false;
	}

	public void SetSceneTitle(out bool done, UiText txt, UiScene ui) {
		done = false;
	}

	public void SetTimer(UiText txt, UiTimer timer) {
	}
}
