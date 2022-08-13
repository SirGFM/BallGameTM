using SceneMng = UnityEngine.SceneManagement.SceneManager;
using SceneMode = UnityEngine.SceneManagement.LoadSceneMode;

/**
 * AdvanceLevel detects an OnGoal signal and tries to advance to the next
 * scene. When it reaches the last scene, it simply goes back to the first
 * scene.
 */

public class AdvanceLevel : UnityEngine.MonoBehaviour, GoalIface {

	public void OnGoal() {
        int next;

        next = SceneMng.GetActiveScene().buildIndex + 1;
		if (next >= SceneMng.sceneCountInBuildSettings) {
			next = 0;
		}

        SceneMng.LoadSceneAsync(next, SceneMode.Single);
	}

	public void OnAdvanceLevel() {
		this.OnGoal();
	}

	public void OnRetryLevel() {
		/* Stub. Do nothing. */
	}
}
