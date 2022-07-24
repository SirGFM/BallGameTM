using GO = UnityEngine.GameObject;
using Scene = UnityEngine.SceneManagement.Scene;
using SceneMng = UnityEngine.SceneManagement.SceneManager;

/**
 * SetMenuRoot configures this game object as the scene's root game object,
 * which should be used to receive global events.
 *
 * This should only be used in menus, as those scenes are loaded manually.
 */

public class SetMenuRoot : UnityEngine.MonoBehaviour {

	void Start() {
		BaseRemoteAction.setRootTarget(this.gameObject);
	}
}
