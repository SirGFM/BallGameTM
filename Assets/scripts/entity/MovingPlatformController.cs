using GO = UnityEngine.GameObject;
using Vec3 = UnityEngine.Vector3;

/**
 * MovingPlatformController is a "meta component" for controlling both the
 * BlockForTime and SetForce components of a platform controller's at once.
 *
 * If both components were in the same object, the Moving Platform would
 * suddenly come to a halt, launching any carried object away. This "meta
 * component" requires that BlockForTime and SetForce are on different
 * objects under this component so it may displace both components, causing
 * the Platform to first hit the SetForce component, decreasing its speed,
 * and then hitting the BlockForTime component.
 */

public class MovingPlatformController : UnityEngine.MonoBehaviour {
	/** The direction of the moving platform. */
	public Vec3 Direction;

	/** The force applied to the moving platform. */
	public float Force;

	/** The distance between the blocker and the SetForce objects. */
	public float Distance;

	/** For how long touching entities should be blocked. */
	public float Seconds = 3.5f;

	void OnValidate() {
		GO self = this.gameObject;

		BlockForTime b = self.GetComponentInChildren<BlockForTime>();
		SetForce f = self.GetComponentInChildren<SetForce>();

		if (b == null) {
			UnityEngine.Debug.LogWarning("This gameObject needs a BlockForTime component!");
			return;
		}
		if (f == null) {
			UnityEngine.Debug.LogWarning("This gameObject needs a SetForce component!");
			return;
		}
		if (b.gameObject == f.gameObject) {
			UnityEngine.Debug.LogWarning("BlockForTime and SetForce must be in different objects!");
			return;
		}

		/* Update the components. */
		b.Seconds = this.Seconds;
		f.Direction = this.Direction;
		f.Force = this.Force;

		/* Position both objects such that the SetForce is before the
		 * BlockForTime. Note that transform.position places the object in
		 * world space, so the components' position must be based on this
		 * object's position. */
		Vec3 origin = this.transform.position;
		Vec3 dist = this.Direction.normalized * this.Distance;
		if (self == b.gameObject) {
			f.transform.position = origin + dist;
		}
		else if (self == f.gameObject) {
			b.transform.position = origin - dist;
		}
		else {
			b.transform.position = origin;
			f.transform.position = origin + dist;
		}
	}
}
