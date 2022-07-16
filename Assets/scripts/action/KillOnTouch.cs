using Col = UnityEngine.Collider;
using EvSys = UnityEngine.EventSystems;
using GO = UnityEngine.GameObject;
using Vec3 = UnityEngine.Vector3;

/**
 * KillOnTouch sends a OnKill event as soon as anything touches it.
 *
 * Indestructible objects should simply not implement that interface,
 * while objects that may be destroyed must handle its own destruction
 * (for example, spawning particles).
 *
 * Alongside that, an object that receives OnKill may send a KillAt
 * (from KillAtIface) so a parent object may explode in its instead.
 */

public interface KillIface : EvSys.IEventSystemHandler {
	/** Signals the object that it should die. */
	void OnKill();
}

public interface KillAtIface : EvSys.IEventSystemHandler {
	/**
	 * Signals the object that it should die, at the specific position.
	 * This is mainly intended to play a death animation at the given position.
	 */
	void KillAt(Vec3 pos);
}

public class KillOnTouch : BaseRemoteAction {
	void OnTriggerEnter(Col other) {
		GO target = other.gameObject;

		this.issueEvent<KillIface>( (x,y) => x.OnKill(), target);
	}
}
