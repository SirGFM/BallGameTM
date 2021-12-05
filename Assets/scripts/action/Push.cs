using Col = UnityEngine.Collider;
using Color = UnityEngine.Color;
using EvSys = UnityEngine.EventSystems;
using GO = UnityEngine.GameObject;
using Vec3 = UnityEngine.Vector3;

/**
 * Push pushes the entity in contact with this component with constant
 * force. This may be used to implement treadmills and some basic
 * force-field like effects. Since this applies the force on every frame,
 * it's not ideal for force-fields, as the applied force isn't attenuated
 * based on the touching entity's distance. Also, since it's applied on
 * every frame, it's also not ideal for springs, as that should be a one
 * time event. For that, PushOnTouch should be used instead.
 *
 * Both Push and PushOnTouch uses the same PushIface interface to push the
 * touching object.
 */

public interface PushIface : EvSys.IEventSystemHandler {
	/**
	 * Push the entity with the given force.
	 *
	 * @param force: The force to be applied (direction+intensity).
	 */
	void OnPush(Vec3 force);
}

public class Push : BaseRemoteAction {

	/** Direction to push entities. */
	public Vec3 Direction;

	/** Intensity to push entities. */
	public float Force;

	void OnTriggerStay(Col other) {
		GO tgt = other.gameObject;

		Vec3 force = this.Direction.normalized * this.Force;
		issueEvent<PushIface>(
				(x,y) => x.OnPush(force),
				tgt);
	}

	void OnDrawGizmos() {
		UnityEngine.Vector3 to;
		UnityEngine.Vector3 pos;
		UnityEngine.Transform t = this.transform;

		pos = t.position;
		to = pos + this.Direction.normalized * this.Force;

		UnityEngine.Gizmos.color = Color.red;
		UnityEngine.Gizmos.DrawLine(pos, to);
	}
}
