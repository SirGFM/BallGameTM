using EvSys = UnityEngine.EventSystems;

/**
 * SetActiveIface defines a standardize interface to disable and enable
 * objects. The main advantage is to have objects handle the specifics of
 * how they are disabled. For example, it allows gracefully stopping a
 * particle emitter instead of immediately stop rendering the particle
 * emitter, causing every particle to vanish at the same time, when the
 * emitter's GameObject were disabled.
 *
 * SimpleDeactivate is the default implementation of the SetActiveIface
 * interface, which simply disables the Game Object. It may work for simple
 * cases, but it may cause issues when the object has a particle emitter
 * (i.e., the particles will all vanish at the same time). Also, after a
 * object with this component is deactivated, it won't receive another
 * SetActive event afterward!
 */

public interface SetActiveIface : EvSys.IEventSystemHandler {
	/**
	 * Signal the entity that it should either get enabled or disabled.
	 *
	 * @param out handled: Whether the event was handled.
	 * @param enable: Whether the entity is being enabled or disabled.
	 */
	void SetActive(out bool handled, bool enable);
}

public class SimpleDeactivate : UnityEngine.MonoBehaviour, SetActiveIface {

	public void SetActive(out bool handled, bool enable) {
		handled = true;

		this.gameObject.SetActive(enable);
	}
}
