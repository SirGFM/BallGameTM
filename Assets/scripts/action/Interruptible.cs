using RB = UnityEngine.Rigidbody;

/**
 * Interruptible implements the BlockIface interface, putting the entity's
 * Rigidbody to sleep while it receives OnBlock events.
 */

public class Interruptible : UnityEngine.MonoBehaviour, BlockIface {
	/** The component's rigibody, that is put to sleep. */
	private RB rb;

	/** Manually track whether the component is sleeping or awake. */
	private bool blocked;

	void Start() {
		this.rb = this.GetComponent<RB>();
		if (this.rb == null) {
			throw new System.Exception($"{this} requires a Rigidbody component!");
		}

		this.blocked = false;
	}

	public void OnBlock() {
		this.blocked = true;
		this.rb.Sleep();
	}

	public void OnUnblock() {
		this.rb.WakeUp();
		this.blocked = false;
	}

	public void IsBlocked(out bool blocked) {
		blocked = this.blocked;
	}
}
