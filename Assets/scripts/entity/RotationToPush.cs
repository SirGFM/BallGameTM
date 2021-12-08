/**
 * RotationToPush is a "meta-component" that converts the local rotation to
 * a push direction, to configure a child's Push component.
 */

public class RotationToPush : UnityEngine.MonoBehaviour {
	/** Forcefully update the child component, since there's no way to
	 * automatically detect changes on the transform. */
	public bool ForceUpdate = false;

	/** Intensity to push entities. */
	public float Force = 500.0f;

	void OnValidate() {
		if (this.ForceUpdate) {
			this.ForceUpdate = false;
		}

		Push p = this.gameObject.GetComponentInChildren<Push>();
		if (p == null) {
			throw new System.Exception($"{this} requires a child with a Push component!");
		}

		p.Force = this.Force;
		p.Direction = this.transform.up.normalized;
	}
}
