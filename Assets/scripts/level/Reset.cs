using SceneMng = UnityEngine.SceneManagement.SceneManager;
using SceneMode = UnityEngine.SceneManagement.LoadSceneMode;

/**
 * Reset reload the current level if it detects the appropriate event.
 */

public class Reset : UnityEngine.MonoBehaviour {

	void Update() {
		/* TODO: Update this after implementing the loader. */
		if (UnityEngine.Input.GetButtonDown("Reset")) {
			int cur = SceneMng.GetActiveScene().buildIndex;

			SceneMng.LoadSceneAsync(cur, SceneMode.Single);
		}
	}
}
