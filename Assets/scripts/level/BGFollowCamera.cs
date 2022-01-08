using Camera = UnityEngine.Camera;
using Transform = UnityEngine.Transform;
using GO = UnityEngine.GameObject;
using Vec3 = UnityEngine.Vector3;

/**
 * BGFollowCamera controls a background camera by copying the rotation of
 * the main camera into this camera, and by changing the FoV and height of
 * this camera based on the distance of the main camera from the scene's
 * origin.
 */

public class BGFollowCamera : BaseRemoteAction {
	/** The main camera. */
	private Transform mainCamera = null;

	/** The object's own transform. */
	private Transform self = null;

	/** The object's own camera. */
	private Camera subCamera = null;

	/** The base FoV, changed based on the main camera's position relative
	 * to the origin. */
	public float baseFoV = 65.0f;

	void Start() {
		this.self = this.transform;
		this.subCamera = this.GetComponent<Camera>();
		if (this.subCamera == null) {
			throw new System.Exception($"{this} requires a Camera component!");
		}

		this.StartCoroutine(this.retryGetMainCamera());
	}

	/**
	 * Try to retrieve the main camera every frame, until it succeeds.
	 */
	private System.Collections.IEnumerator retryGetMainCamera() {
		while (this.mainCamera == null) {
			GO cam = null;

			this.rootEvent<CameraHelperIface>(
					(x,y) => x.GetMainCamera(out cam));
			if (cam != null) {
				this.mainCamera = cam.transform;
			}

			yield return null;
		}
	}

	void FixedUpdate() {
		if (this.mainCamera != null) {
			this.self.rotation = this.mainCamera.rotation;

			Vec3 camPos = this.mainCamera.position;
			Vec3 pos = this.self.localPosition;
			pos.y = camPos.y * 0.01f;
			this.self.localPosition = pos;

			float m = camPos.magnitude;
			this.subCamera.fieldOfView = this.baseFoV + m * 0.05f;
		}
	}
}
