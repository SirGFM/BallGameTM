using Col = UnityEngine.Collider;
using GO = UnityEngine.GameObject;
using Vec3 = UnityEngine.Vector3;

/**
 * Push pushes an entity as soon as it comes in contact with this component
 * with constant force. Differently from Push, it does so only on the first
 * frame that the entity comes in contact with this component, so it should
 * be used for springs and things like that.
 *
 * Both Push and PushOnTouch uses the same PushIface interface to push the
 * touching object.
 */

public class PushOnTouch : BaseRemoteAction {

	/** Direction and intensity to push entities. */
	public Vec3 Force;

	void OnTriggerEnter(Col other) {
		GO tgt = other.gameObject;

		issueEvent<PushIface>(
				(x,y) => x.OnPush(this.Force),
				tgt);
	}
}
