using Col = UnityEngine.Collider;
using EvSys = UnityEngine.EventSystems;
using GO = UnityEngine.GameObject;

/**
 * ResetOnTouch issues a Reset signal, requesting the object to go back to its initial position,
 * on touch.
 */
public class ResetOnTouch : BaseRemoteAction {
	void OnTriggerEnter(Col other) {
		GO tgt = other.gameObject;

		issueEvent<ResetToRestIface>(
				(x,y) => x.Reset(),
				tgt);
	}
}
