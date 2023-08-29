using EvSys = UnityEngine.EventSystems;
using GO = UnityEngine.GameObject;
using RB = UnityEngine.Rigidbody;
using Vec3 = UnityEngine.Vector3;

/**
 * ResetToRestIface defines the Reset signal, used to request the object to go back to its initial position.
 */
public interface ResetToRestIface : EvSys.IEventSystemHandler {
	/** Signal that the object should reset back to its resting position. */
	void Reset();
}

public class ResetToRest : BaseRemoteAction, ResetToRestIface {
	/** The component's rigibody. */
	private RB rb;

	/** The object's starting position. */
	private Vec3 startingPosition;

	void Start() {
		this.rb = this.GetComponent<RB>();

		this.startingPosition = this.transform.position;
	}

	public void Reset() {
		/* If this object is a non-kinematic rigid body,
		 * temporarily set it as kinematic to manually set its position. */
		if (this.rb != null) {
			this.rb.isKinematic = true;
		}

		this.transform.position = this.startingPosition;

		if (this.rb != null) {
			this.rb.isKinematic = false;
		}
	}
}
