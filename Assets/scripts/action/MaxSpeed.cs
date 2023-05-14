using RB = UnityEngine.Rigidbody;
using Vec3 = UnityEngine.Vector3;

public class MaxSpeed : UnityEngine.MonoBehaviour {
	/** The component's rigibody. */
	private RB rb;

	/** The component's last known velocity, bellow the max speed. */
	private Vec3 lastVelocity;

	/** The object's max absolute speed. */
	public float Max = 0.0f;

	void Start() {
		this.rb = this.GetComponent<RB>();
		if (this.rb == null) {
			throw new System.Exception($"{this} requires a Rigidbody component!");
		}

		/* Disable this script if the max speed isn't set. */
		this.enabled = (this.Max > 0.0f);
		this.Max *= this.Max;

		this.lastVelocity = Vec3.zero;
	}

	void FixedUpdate() {
		if (this.rb.velocity.sqrMagnitude > this.Max) {
			this.rb.velocity = this.lastVelocity;
		}
		else {
			this.lastVelocity = this.rb.velocity;
		}
	}
}
