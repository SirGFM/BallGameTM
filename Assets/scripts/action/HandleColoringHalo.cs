using Color = UnityEngine.Color;
using EvSys = UnityEngine.EventSystems;
using GO = UnityEngine.GameObject;

/**
 * HandleColoringHalo brokes requests from a SetColoredHalo to a Halo,
 * acting as an intermediate so Halo may be on its own, more nested, hierarchy.
 */

public interface HandleColoringHaloIface : EvSys.IEventSystemHandler {
	/**
	 * Register another game object as this object's halo.
	 *
	 * @param gameObject: The self-reported halo  object.
	 */
	void RegisterHalo(GO gameObject);
}

public class HandleColoringHalo : BaseRemoteAction, SetColoredHaloIface, HandleColoringHaloIface {

	/**
	 * This object's halo.
	 */
	private GO halo;

	public void OnSetColoredHalo(Color color) {
		if (halo == null) {
			return;
		}

		issueEvent<SetColoredHaloIface>(
				(x,y) => x.OnSetColoredHalo(color),
				this.halo);
	}

	public void RegisterHalo(GO gameObject) {
		this.halo = gameObject;
	}
}
