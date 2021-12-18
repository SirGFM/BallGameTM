
/**
 * Reset reload the current level if it detects the appropriate event.
 */

public class Reset : BaseRemoteAction {

	void Update() {
		if (Input.GetResetButton()) {
			rootEvent<LoaderIface>(	(x,y) => x.OnReset() );
		}
	}
}
