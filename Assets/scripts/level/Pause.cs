
/**
 * Pause opens the pause menu if it detects the appropriate event.
 */

public class Pause : BaseRemoteAction {

	void Update() {
		if (Input.GetPauseJustPressed()) {
			rootEvent<LoaderIface>( (x,y) => x.ShowPause() );
		}
	}
}
