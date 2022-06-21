using GO = UnityEngine.GameObject;
using Transform = UnityEngine.Transform;

/**
 * ChildrenSetActive forward SetActive events to every child inside this
 * gameObject. By default, the children game objects are modified manually
 * (by calling SetActive on the game object). Optionally, this component
 * may send a SetActive event to every child, but this may cause issues if
 * the event isn't handled by a child object!
 */

public class ChildrenSetActive : BaseRemoteAction, SetActiveIface {

	/** This object's transform. */
	private Transform self = null;

	/** Whether this object should send a SetActive event to modify its
	 * children (which, if not handled, may cause issues). */
	public bool SetActiveByEvent = false;

	/** Fail-safe to avoid receiving a SetActive event while handling
	 * another SetActive event. */
	private bool avoidRecusion = false;

	void Start() {
		this.self = this.gameObject.transform;
	}

	virtual public void SetActive(out bool handled, bool enable) {
		if (this.self.childCount == 0 || this.avoidRecusion) {
			handled = false;
			return;
		}
		this.avoidRecusion = true;

		handled = true;
		for (int i = 0; i < this.self.childCount; i++) {
			GO child;

			child = this.self.GetChild(i).gameObject;

			if (this.SetActiveByEvent) {
				bool tmp = false;

				/* Since this.avoidRecusion is set before sending this
				 * event, even if the child doesn't handle it, this object
				 * will gracefully cause the event to fail (instead of
				 * causing an infinite recursion into this event). */
				issueEvent<SetActiveIface>(
						(x,y) => x.SetActive(out tmp, enable),
						child);
				/* Ensure that the event was handled by at least a child. */
				handled = handled || tmp;
			}
			else {
				child.SetActive(enable);
			}
		}
		this.avoidRecusion = false;
	}
}
