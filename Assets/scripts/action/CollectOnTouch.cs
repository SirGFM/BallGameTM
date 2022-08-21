using Col = UnityEngine.Collider;

/**
 * CollectOnTouch sends a Increase event whenever an object touches it,
 * then it disables itself. Since objects with a CollectOnTouch component
 * must be on the same hierarchy as objects with a ActivateOnCollectible,
 * this components defaults to disabling itself by disabling its
 * GameObject. Optionally, it may send a SetActive event to itself, but
 * this will cause issues if the event isn't handled by the object itself!
 */

public class CollectOnTouch : BaseRemoteAction {

	/** Whether this object has already been collected. */
	private bool collected = false;

	/** Whether this object should send a SetActive event to disable itself
	 * (which, if not handled, may cause issues). */
	public bool DisableByEvent = false;

	void Start() {
		issueEvent<GetCollectibleIface>( (x,y) => x.IncreaseTotal());
	}

	/** Disable this object. */
	private void disableSelf() {
		bool handled = false;

		if (this.DisableByEvent) {
			issueEvent<SetActiveIface>(
					(x,y) => x.SetActive(out handled, false));
		}

		/* Disable this object manually if either the event wasn't handled
		 * or if the event wasn't even sent. */
		if (!handled) {
			this.gameObject.SetActive(false);
		}
	}

	void OnEnable() {
		/** Ensure that further "SetActive"s will be ignored. */
		if (this.collected) {
			this.disableSelf();
		}
	}

	void OnTriggerEnter(Col other) {
		if (this.collected) {
			/* Allow the object to be collected only once, in case
			 * SetActive takes a while to be fully handled (e.g., if
			 * playing an animation). */
			return;
		}

		issueEvent<GetCollectibleIface>( (x,y) => x.Increase());

		this.disableSelf();
		this.collected = true;
		Global.Sfx.playCollectible();
	}
}
