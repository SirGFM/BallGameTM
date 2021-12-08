using RB = UnityEngine.Rigidbody;
using Vec3 = UnityEngine.Vector3;

/**
 * Pushable implements the PushIface interface, applying the received force
 * to the component's Rigidbody.
 */

public class Pushable : UnityEngine.MonoBehaviour, PushIface {
	/** This object's rigidbody. */
	private RB rb;

	void Start() {
		this.rb = this.GetComponent<RB>();
		if (this.rb == null) {
			throw new System.Exception($"{this} requires a Rigidbody component!");
		}
	}

	public void OnPush(Vec3 force) {
		this.rb.AddForce(force);
	}
}
