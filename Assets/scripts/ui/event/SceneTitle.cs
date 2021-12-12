using UiText = UnityEngine.UI.Text;

/**
 * SceneTitle sends an event to the Root object sending this object's
 * UiText. The Root object must then update the UiText's text to the
 * scene title.
 */

public class SceneTitle : BaseRemoteAction {

	/** The scene that this component is on. */
	public UiScene scene;

    void Start() {
		UiText txt = this.gameObject.GetComponent<UiText>();
		if (txt == null) {
			throw new System.Exception($"{this} requires a UiText component!");
		}

		this.StartCoroutine(this.setSceneTitle(txt));
    }

	/**
	 * Try to set this object's title every frame, until it succeeds.
	 */
	private System.Collections.IEnumerator setSceneTitle(UiText txt) {
		bool done = false;

		do {
			rootEvent<LoaderIface>(
					(x,y) => x.SetSceneTitle(out done, txt, this.scene));
			yield return null;
		} while (done);
	}
}
