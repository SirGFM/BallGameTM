using Col = UnityEngine.Collider;
using EvSys = UnityEngine.EventSystems;
using GO = UnityEngine.GameObject;
using Time = UnityEngine.Time;
using Vec3 = UnityEngine.Vector3;

/**
 * BlockForTime blocks the touching entity for the specified amount of
 * time. Entities that want to be blocked must implement the BlockIface
 * interface.
 *
 * When an entity touches another entity with a BlockForTime component,
 * this component calls OnBlock on the touching entity on every frame.
 * After the desired time has elapsed,  this component calls OnUnblock
 * on the touching entity exactly once.
 *
 * Note that chaning a BlockForTime's Seconds affects currently active
 * events!
 *
 * Interruptible is the default implementation of a blocking component,
 * which simple forces the component's Rigidbody to sleep.
 */

public interface BlockIface : EvSys.IEventSystemHandler {
	/**
	 * OnBlock should halt the entity. It's called once per frame after
	 * an entity comes in contact with this component.
	 */
	void OnBlock();

	/**
	 * OnUnblock is called exactly once after the desired time has elapsed,
	 * signaling to the touching entity that it should move normally.
	 */
	void OnUnblock();

	/**
	 * Checks whether this entity is currently blocked
	 *
	 * @param out blocked: Returns whether the entity is blocked.
	 */
	void IsBlocked(out bool blocked);
}

public class BlockForTime : BaseRemoteAction {

	/** For how long touching entities should be blocked. */
	public float Seconds;

	private System.Collections.IEnumerator haltTarget(GO tgt) {
		for (float elapsed = 0.0f; elapsed < this.Seconds; elapsed += Time.deltaTime) {
			issueEvent<BlockIface>( (x,y) => x.OnBlock(), tgt);
			yield return null;
		}

		issueEvent<BlockIface>( (x,y) => x.OnUnblock(), tgt);
	}

	void OnTriggerEnter(Col other) {
		GO tgt = other.gameObject;

		this.StartCoroutine(this.haltTarget(tgt));
	}
}
