using GO = UnityEngine.GameObject;
using UiText = UnityEngine.UI.Text;

/**
 * SceneTimer sends an event to the Root object sending this object's
 * UiText every frame, so the timer may be updated.
 */

public class SceneTimer : BaseRemoteAction {

	/** The timer that this component shows. */
	public UiTimer timer;

	/** The component's text element. */
	private UiText txt;

    void Start() {
		if (!Global.showTimer) {
			GO.Destroy(this.gameObject);
			return;
		}
		else if (timer == UiTimer.None) {
			this.enabled = false;
			return;
		}

		this.txt = this.gameObject.GetComponent<UiText>();
		if (this.txt == null) {
			throw new System.Exception($"{this} requires a UiText component!");
		}
    }

	void Update() {
		rootEvent<LoaderIface>(
				(x,y) => x.SetTimer(this.txt, this.timer));
	}
}
