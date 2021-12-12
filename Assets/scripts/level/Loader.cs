using EvSys = UnityEngine.EventSystems;

public interface LoaderIface : EvSys.IEventSystemHandler {
	/**
	 * Resets the current stage.
	 */
	void OnReset();
}
