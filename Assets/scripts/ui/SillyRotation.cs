using ConstantForce = UnityEngine.ConstantForce;
using Transform = UnityEngine.Transform;
using UEMath = UnityEngine.Mathf;
using Vec3 = UnityEngine.Vector3;

public class SillyRotation : UnityEngine.MonoBehaviour {

	private Transform self;

	private ConstantForce cf;

	public float minRotation = -112.5f;
	public float maxRotation = 22.5f;

	void Start() {
		this.cf = this.GetComponent<ConstantForce>();
		if (this.cf == null) {
			throw new System.Exception($"{this} requires a ConstantForce component!");
		}

		this.self = this.transform;
	}

	/**
	 * Update the object's torque, rotating in the signal's direction.
	 *
	 * @param signal: Direction that the object should rotate toward.
	 */
	private void updateTorque(float signal) {
		Vec3 newTorque = this.cf.torque;
		newTorque.y = signal * UEMath.Abs(this.cf.torque.y);
		this.cf.torque = newTorque;
	}

	void Update() {
		float angle = this.self.eulerAngles.y;

		if (angle > 180.0f) {
			angle -= 360.0f;
		}

		if (angle < this.minRotation) {
			this.updateTorque(1.0f);
		}
		else if (angle > this.maxRotation) {
			this.updateTorque(-1.0f);
		}
	}
}
