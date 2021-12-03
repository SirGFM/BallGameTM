using Col = UnityEngine.Collider;
using GO = UnityEngine.GameObject;

/**
 * Signals the camera a SetCameraDistance event if this object is clipping
 * into any geometry.
 */

public class SetCameraDistance : BaseRemoteAction {
	/** The game's main camera. */
	private GO cam = null;

	/** Maximum distance for the camera. */
	public float Distance = 10.0f;

	void Start() {
		this.StartCoroutine(this.retryGetMainCamera());
	}

	/**
	 * Try to call getMainCamera() every frame, until it succeeds.
	 */
	private System.Collections.IEnumerator retryGetMainCamera() {
		while (!getMainCamera()) {
			yield return null;
		}
	}

	/**
	 * Try to retrieve and configure the main camera.
	 *
	 * @return Whether the camera was configured or not.
	 */
	private bool getMainCamera() {
		this.cam = null;
		this.rootEvent<CameraHelperIface>(
				(x,y) => x.GetMainCamera(out this.cam));
		if (this.cam == null) {
			return false;
		}

		return true;
	}

	void OnTriggerEnter(Col other) {
		this.issueEvent<CameraIface>(
				(x,y) => x.SetCameraDistance(this.Distance),
				this.cam);
	}

	void OnTriggerStay(Col other) {
		this.issueEvent<CameraIface>(
				(x,y) => x.SetCameraDistance(this.Distance),
				this.cam);
	}
}
