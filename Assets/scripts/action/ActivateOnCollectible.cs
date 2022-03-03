using EvSys = UnityEngine.EventSystems;
using GO = UnityEngine.GameObject;

/**
 * ActivateOnCollectible counts how much of a resource was obtained,
 * sending a SetActive event when the desired number of resources is
 * achieved.
 *
 * Collecting resources is reported by sending an Increase event to this
 * object.
 */

public interface GetCollectibleIface : EvSys.IEventSystemHandler {
	/**
	 * Signal that a collectible exists, increasing the total objects
	 * required to activate this object.
	 */
	void IncreaseTotal();

	/**
	 * Signal that a new collectible was obtained.
	 */
	void Increase();
}

public class ActivateOnCollectible : BaseRemoteAction, GetCollectibleIface {
	/** The current number of items collected. */
	private int count = 0;

	/** Number of collectibles required to activate this object. */
	private int required = 0;

	/** Whether the activation enables or disables the target. */
	public bool EnableOrDisableTarget = true;

	public void Increase() {
		this.count++;

		if (this.count == this.required) {
			bool handled = false;

			issueEvent<SetActiveIface>(
					(x,y) => x.SetActive(out handled, this.EnableOrDisableTarget));

			if (!handled) {
				throw new System.Exception($"No object able to handle SetActive in {this}");
			}
		}
	}

	public void IncreaseTotal() {
		this.required++;
	}
}
