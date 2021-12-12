using GO = UnityEngine.GameObject;
using Scene = UnityEngine.SceneManagement.Scene;
using SceneMng = UnityEngine.SceneManagement.SceneManager;

/**
 * SetRoot configures this game object as the scene's root game object,
 * which should be used to receive global events.
 */

public class SetRoot : UnityEngine.MonoBehaviour {

	void Start() {
		/**
		 * In the release build, stages will be loaded by the Loader scene.
		 * However, if running in the editor, each level should have its
		 * own Root object, to simplify testing a stage by itself.
		 */
#if UNITY_EDITOR
		Scene scene = SceneMng.GetActiveScene();

		foreach (GO go in scene.GetRootGameObjects()) {
			if (go.GetComponent<Loader>() != null) {
				Destroy(this.gameObject);
				return;
			}
		}
		BaseRemoteAction.setRootTarget(this.gameObject);
#else
		Destroy(this.gameObject);
#endif
	}
}
