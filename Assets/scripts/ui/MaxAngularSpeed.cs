using Rigidbody = UnityEngine.Rigidbody;
using UEMath = UnityEngine.Mathf;
using Vec3 = UnityEngine.Vector3;

public class MaxAngularSpeed : UnityEngine.MonoBehaviour {

	public enum RotationAxis {
		X = 0,
		Y,
		Z
	}

	private Rigidbody rb;

	public float maxAngularSpeed = 0.5f;

	public RotationAxis axis = RotationAxis.Y;

	void Start() {
		this.rb = this.GetComponent<Rigidbody>();
		if (this.rb == null) {
			throw new System.Exception($"{this} requires a Rigidbody component!");
		}
	}

	void Update() {
		if (UEMath.Abs(this.rb.angularVelocity[(int)axis]) > this.maxAngularSpeed) {
			Vec3 newSpeed = this.rb.angularVelocity;

			float curSpeed = newSpeed[(int)axis];
			/* Get the speed's sign. */
			curSpeed /= UEMath.Abs(curSpeed);
			curSpeed *= this.maxAngularSpeed;

			newSpeed[(int)axis] = curSpeed;
			this.rb.angularVelocity = newSpeed;
		}
	}
}
