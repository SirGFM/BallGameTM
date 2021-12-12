using SceneMng = UnityEngine.SceneManagement.SceneManager;
using SceneMode = UnityEngine.SceneManagement.LoadSceneMode;

/**
 * EditorReset reloads the current level if it detects the appropriate event.
 */

public class EditorReset : UnityEngine.MonoBehaviour, LoaderIface {

	public void OnReset() {
		int cur = SceneMng.GetActiveScene().buildIndex;

		SceneMng.LoadSceneAsync(cur, SceneMode.Single);
	}
}
