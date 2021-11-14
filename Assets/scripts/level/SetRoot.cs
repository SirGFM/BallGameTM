/**
 * SetRoot configures this game object as the scene's root game object,
 * which should be used to receive global events.
 */

public class SetRoot : UnityEngine.MonoBehaviour {

	void Start() {
		BaseRemoteAction.setRootTarget(this.gameObject);
	}
}
