
/**
 * Reset reload the current level if it detects the appropriate event.
 */

public class Reset : BaseRemoteAction {

	void Update() {
		/* TODO: Update this after implementing the input system. */
		if (UnityEngine.Input.GetButtonDown("Reset")) {
			rootEvent<LoaderIface>(	(x,y) => x.OnReset() );
		}
	}
}
