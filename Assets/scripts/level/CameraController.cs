using EvSys = UnityEngine.EventSystems;
using GO = UnityEngine.GameObject;
using Transform = UnityEngine.Transform;
using Vec3 = UnityEngine.Vector3;

/**
 * CameraController moves the gameObject around a given target, keeping a
 * constant distance to the target, and keeping this gameObject facing the
 * target.
 *
 * The target is configured by a SetCameraTarget event. If no target has
 * been configured, the camera simply stands still.
 */

public interface CameraIface : EvSys.IEventSystemHandler {
	/**
	 * Configure's the camera's main target.
	 *
	 * @param target: The camera's main target.
	 */
	void SetCameraTarget(GO target);
}

public class CameraController : BaseRemoteAction, CameraIface {
	/** Cached reference to this component's transform. */
	private Transform self;
	/** The component being followed by the camera. */
	private Transform target = null;
	/** Distance between the camera and the target. */
	public float distance = 10.0f;

	/** The camera angle in the horizontal plane. */
	private float horAng = 0.0f;
	/** The camera angle in the vertical plane. */
	public float verAng = 30.0f;

	/** The position of the mouse on the last frame. */
    private Vec3 lastMouse;

	void Start() {
		this.self = this.transform;
		this.lastMouse = UnityEngine.Input.mousePosition;

		/* Try to configure this as the main camera, and on failure
		 * set a coroutine to try it again at a later time. */
		if (!configureMainCamera()) {
			this.StartCoroutine(this.retryConfigureMainCamera());
		}
	}

	/**
	 * Try to call configureMainCamera() every frame, until it succeeds.
	 */
	private System.Collections.IEnumerator retryConfigureMainCamera() {
		while (!configureMainCamera()) {
			yield return null;
		}
	}

	/**
	 * Try to retrieve and configure the main camera.
	 *
	 * @return Whether the camera was configured or not.
	 */
	private bool configureMainCamera() {
		GO cam;

		this.rootEvent<CameraHelperIface>(
				(x,y) => x.SetMainCamera(this.gameObject));

		cam = null;
		this.rootEvent<CameraHelperIface>(
				(x,y) => x.GetMainCamera(out cam));
		return (cam != null);
	}

	void Update() {
		if (this.target == null) {
			return;
		}

		/* TODO:
		 *   - Add gamepad/keys input
		 *   - Make the camera inversible
		 *   - Mouse sensibility
		 */
		if (UnityEngine.Input.GetAxis("Fire1") > 0) {
			Vec3 mouseDelta = UnityEngine.Input.mousePosition - this.lastMouse;

			this.horAng += mouseDelta.x;
			this.verAng += mouseDelta.y;
		}
		else {
			float x = UnityEngine.Input.GetAxis("HorizontalR");
			float y = UnityEngine.Input.GetAxis("VerticalR");

			this.horAng += x * 5.0f;
			this.verAng += y * 5.0f;
		}

		this.horAng = Math.NormalizeAngle(this.horAng);
		this.verAng = Math.NormalizeAngle(this.verAng);

		/* Simply rotate a backward vector of the length of the distance
		 * around the horizontal and vertical planes, to calculate the
		 * position of the camera relative to its target. */
		(float rx, float ry, float rz) = Math.RotateVec3(0, 0,
				-this.distance, this.horAng, this.verAng);

		this.self.position = new Vec3(rx, ry, rz) + this.target.position;
		this.self.LookAt(this.target);

		/* Update the mouse's origin to calculate the movement on a later
		 * frame. */
		this.lastMouse = UnityEngine.Input.mousePosition;
	}

	public void SetCameraTarget(GO target) {
		this.target = target.transform;
	}
}
