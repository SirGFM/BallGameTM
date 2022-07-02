using EvSys = UnityEngine.EventSystems;
using GO = UnityEngine.GameObject;
using Time = UnityEngine.Time;

/**
 * TimedActivate sends an event to either activate or deactivate the object
 * after the requested time.
 */

public class TimedActivate : BaseRemoteAction {

	/** How long until the object is deactivated. */
	public float ActivateTime = 1.0f;

	/** Whether the event is an activate (true) or deactivate (false). */
	public bool SendActivateOrDeactivate = false;

	void Update() {
		if (this.ActivateTime > 0) {
			this.ActivateTime -= Time.deltaTime;
		}
		else {
			bool handled;

			GO self = this.gameObject;
			bool flag = this.SendActivateOrDeactivate;

			issueEvent<SetActiveIface>(
					(x,y) => x.SetActive(out handled, flag), self);

			self.SetActive(false);
		}
	}
}
