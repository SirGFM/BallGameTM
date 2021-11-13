using Col = UnityEngine.Collider;
using EvSys = UnityEngine.EventSystems;
using GO = UnityEngine.GameObject;
using ListConSrc = System.Collections.Generic.List<UnityEngine.Animations.ConstraintSource>;
using ParentConstraint = UnityEngine.Animations.ParentConstraint;

/**
 * DetectCollider signals the parent objects whether its in contact with
 * other objects or not. To simplify the signaled object, this component
 * sends one event each frame signaling whether it's touching anything
 * (by sending an OnTouchingAny event) or not (by sending an
 * OnNotTouchingAny).
 *
 * This may mainly be used to detect when an object is grounded, so it may
 * control whether or not it can jump. For this reason, there's no event
 * for "touching in X direction".
 *
 * If this were used inside a spheric object, the collider would rotate
 * together with the object's Rigidbody. Therefore, one solution is to
 * keep this object either by itself or as a sibling of the main object and
 * use a ParentConstraint component to make this object follow its "logical
 * parent". Since BaseRemoteAction usually sends events to the object
 * itself, so it may bubble up to its parents, this object considers any
 * ParentConstraint as its parent and sends events to that object instead.
 */

public interface DetectColliderIface : EvSys.IEventSystemHandler {
	/**
	 * Signals that this entity is touching at least one other object.
	 */
	void OnTouchingAny();

	/**
	 * Signals that this entity is NOT touching any other object.
	 */
	void OnNotTouchingAny();
}

public class DetectCollider : BaseRemoteAction {
	/** The number of objects currently touching this. */
	private int touching;

	/** The parent of the game object... for wonky scenearious. */
	private GO parent;

	void Start() {
		this.touching = 0;

		ParentConstraint pc = this.GetComponent<ParentConstraint>();
		if (pc != null) {
			ListConSrc sources = new ListConSrc();

			pc.GetSources(sources);
			if (sources.Count > 1) {
				throw new System.Exception($"{this} has multiple parents! Bailing out...");
			}

			this.parent = sources[0].sourceTransform.gameObject;
		}
	}

	void FixedUpdate() {
		if (this.touching > 0) {
			issueEvent<DetectColliderIface>( (x,y) => x.OnTouchingAny(), this.parent );
		}
		else {
			issueEvent<DetectColliderIface>( (x,y) => x.OnNotTouchingAny(), this.parent );
		}
	}

	void OnTriggerEnter(Col other) {
		this.touching++;
	}

	void OnTriggerExit(Col other) {
		if (this.touching > 0) {
			this.touching--;
		}
	}
}
