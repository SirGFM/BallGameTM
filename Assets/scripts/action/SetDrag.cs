using Col = UnityEngine.Collider;
using EvSys = UnityEngine.EventSystems;
using GO = UnityEngine.GameObject;
using Vec3 = UnityEngine.Vector3;

/**
 * SetDrag signals the touching entity to set its own drag to a desired
 * value. This may be used to simulate ice and other surfaces.
 *
 * When an entity comes in contact with this component, and while it stays
 * in contact, this component sends OnSetDrag events with the desired drag.
 * Then, when the entity stops touching this component, it receives a
 * OnResetDrag event.
 *
 * Note: I couldn't figure out how to make Unity's physics material affect
 * an entity. Nothing but the 'bounce' field worked... so this is the hack
 * that I figured out. ¯\_(ツ)_/¯
 */

public interface SetDragIface : EvSys.IEventSystemHandler {
	/**
	 * Set the touching entity's drag to the desired value.
	 *
	 * @param force: The desired drag.
	 */
	void OnSetDrag(float drag);

	/**
	 * Signal the entity to revert back to its original drag.
	 */
	void OnResetDrag();
}

public class SetDrag : BaseRemoteAction {
	public float Drag;

	void setDrag(GO tgt) {
		issueEvent<SetDragIface>( (x,y) => x.OnSetDrag(this.Drag), tgt);
	}

	void OnTriggerEnter(Col other) {
		this.setDrag(other.gameObject);
	}

	void OnTriggerStay(Col other) {
		this.setDrag(other.gameObject);
	}

	void OnTriggerExit(Col other) {
		GO tgt = other.gameObject;

		issueEvent<SetDragIface>( (x,y) => x.OnResetDrag(), tgt);
	}
}
