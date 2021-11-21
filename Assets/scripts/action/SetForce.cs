using Col = UnityEngine.Collider;
using Color = UnityEngine.Color;
using EvSys = UnityEngine.EventSystems;
using GO = UnityEngine.GameObject;
using Vec3 = UnityEngine.Vector3;

/**
 * SetForce signals the touching entity to apply a given force on every
 * frame. This may be used to implement moving platforms, for example
 * having two objects with a SetForce component with opposing forces.
 * Then, a component that implements SetForceIface would be signaled
 * to move back and forth between those two SetForces.
 */

public interface SetForceIface : EvSys.IEventSystemHandler {
	/**
	 * Set the touching entity's force to the desired value.
	 *
	 * @param force: The desired force (direction+intensity).
	 */
	void OnSetForce(Vec3 force);
}

public class SetForce : BaseRemoteAction {
	public Vec3 Direction;
	public float Force;

	void OnTriggerStay(Col other) {
		GO tgt = other.gameObject;

		Vec3 force = this.Direction.normalized * this.Force;
		issueEvent<SetForceIface>(
				(x,y) => x.OnSetForce(force),
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
