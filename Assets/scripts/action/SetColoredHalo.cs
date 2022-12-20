using Col = UnityEngine.Collider;
using Color = UnityEngine.Color;
using EvSys = UnityEngine.EventSystems;

/**
 * SetColoredHalo sends a color to the colliding entity, so this entity
 * may set its own halo (if any) to the desired color.
 */

public interface SetColoredHaloIface : EvSys.IEventSystemHandler {
	/**
	 * Set the color of the entity's halo.
	 *
	 * @param color: The color of the halo.
	 */
	void OnSetColoredHalo(Color color);
}

public class SetColoredHalo : BaseRemoteAction {

	/** The color set by this entities. */
	public Color color;

	void OnTriggerStay(Col other) {
		issueEvent<SetColoredHaloIface>(
				(x,y) => x.OnSetColoredHalo(this.color),
				other.gameObject);
	}
}
