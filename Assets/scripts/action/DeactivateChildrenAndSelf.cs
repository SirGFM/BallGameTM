/**
 * DeactivateChildrenAndSelf inherits from ChildrenSetActive, thus
 * forwarding SetActive events to every child inside this
 * gameObject, but also disables this game object after some time, when it
 * receives a SetActive event to disable this object.
 */

public class DeactivateChildrenAndSelf : ChildrenSetActive {

	/** Whether the object has already been disabled (and the deactive
	 * coroutine should be running). */
	private bool disabled = false;

	/** How long until this object is disabled. */
	public float timeToDeactivate = 1.0f;

	/** Disable this object after some time has elapsed. */
	private System.Collections.IEnumerator timedDeactivate() {
		yield return new UnityEngine.WaitForSeconds(this.timeToDeactivate);

		this.gameObject.SetActive(false);
	}

	override public void SetActive(out bool handled, bool enable) {
		handled = true;
		if (this.disabled) {
			return;
		}

		base.SetActive(out handled, enable);
		if (!enable) {
			this.StartCoroutine(this.timedDeactivate());

			this.disabled = true;
		}
	}
}
