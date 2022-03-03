using Col = UnityEngine.Collider;

/**
 * ActivateWhileInside detects when there are objects inside itself to
 * either enable or disable the object itself (by sending a SetActive
 * event).
 *
 * This object should usually simply start as disabled, although that must
 * be set manually.
 */

public class ActivateWhileInside : BaseRemoteAction {

	/** The number of objects inside this game object. */
	private int num_inside = 0;

	/**
	 * Set this object's state to either enabled or disabled.
	 *
	 * @param enable: Whether the object should be enabled or disabled.
	 */
	private void set_state(bool enable) {
		bool handled = false;

		issueEvent<SetActiveIface>(
				(x,y) => x.SetActive(out handled, enable));
	}

	void OnTriggerEnter(Col other) {
		if (!enabled) {
			return;
		}

		if (this.num_inside == 0) {
			this.set_state(true);
		}

		this.num_inside++;
	}

	void OnTriggerExit(Col other) {
		if (!enabled) {
			return;
		}

		this.num_inside--;

		if (this.num_inside == 0) {
			this.set_state(false);
		}
	}
}
