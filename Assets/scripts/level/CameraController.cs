using EvSys = UnityEngine.EventSystems;
using GO = UnityEngine.GameObject;
using Transform = UnityEngine.Transform;
using Time = UnityEngine.Time;
using UEMath = UnityEngine.Mathf;
using Vec2 = UnityEngine.Vector2;
using Vec3 = UnityEngine.Vector3;

/**
 * CameraController moves the gameObject around a given target, keeping a
 * constant distance to the target, and keeping this gameObject facing the
 * target.
 *
 * The target is configured by a SetCameraTarget event. If no target has
 * been configured, the camera simply stands still.
 *
 * SetCameraDistance should be issued to control the camera, making it
 * approach or get farther from the target, to somewhat fix clipping into
 * geometry.
 */

public interface CameraIface : EvSys.IEventSystemHandler {
	/**
	 * Configure's the camera's main target.
	 *
	 * @param target: The camera's main target.
	 */
	void SetCameraTarget(GO target);

	/**
	 * Limit the current distance to a percentage of the maximum distance.
	 *
	 * @param perc: The percentage of the camera from the maximum distance.
	 */
	void SetCameraDistance(float perc);
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
	/** Dynamic distance, to avoid/minimize clipping. */
	private float curDist;

	/** The position of the mouse on the last frame. */
    private Vec3 lastMouse;

	void Start() {
		this.self = this.transform;
		this.lastMouse = Input.GetMousePosition();

		/* Try to configure this as the main camera, and on failure
		 * set a coroutine to try it again at a later time. */
		if (!configureMainCamera()) {
			this.StartCoroutine(this.retryConfigureMainCamera());
		}

		this.curDist = this.distance;
		this.StartCoroutine(this.zoomCamera());
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
		 *   - Make the camera inversible
		 *   - Mouse sensibility
		 *   - Gamepad sensibility
		 */
		if (Input.GetMouseCameraEnabled()) {
			Vec3 mouseDelta = Input.GetMousePosition() - this.lastMouse;

			this.horAng += mouseDelta.x;
			this.verAng += mouseDelta.y;
		}
		else {
			Vec2 cam = Input.GetCamera();

			this.horAng += cam.x * 5.0f;
			this.verAng += cam.y * 5.0f;
		}

		this.horAng = Math.NormalizeAngle(this.horAng);
		this.verAng = Math.NormalizeAngle(this.verAng);

		/* Simply rotate a backward vector of the length of the distance
		 * around the horizontal and vertical planes, to calculate the
		 * position of the camera relative to its target. */
		(float rx, float ry, float rz) = Math.RotateVec3(0, 0,
				-this.curDist, this.horAng, this.verAng);

		this.self.position = new Vec3(rx, ry, rz) + this.target.position;
		this.self.LookAt(this.target);

		/* Update the mouse's origin to calculate the movement on a later
		 * frame. */
		this.lastMouse = Input.GetMousePosition();
	}

	public void SetCameraTarget(GO target) {
		this.target = target.transform;
	}

	/** The camera zoom closes to the camera since the last frame. */
	private float lastDistPerc;

	/**
	 * Slowly adjust the camera over every frame, to zoom into or away
	 * from the target depending on whether anything is colliding with
	 * the camera.
	 */
	private System.Collections.IEnumerator zoomCamera() {
		while (true) {
			this.lastDistPerc = this.distance;
			yield return new UnityEngine.WaitForFixedUpdate();

			if (UEMath.Abs(this.lastDistPerc - this.curDist) > 0.01f) {
				this.curDist = this.curDist + (this.lastDistPerc - this.curDist) * Time.fixedDeltaTime;
			}
		}
	}

	public void SetCameraDistance(float perc) {
		if (perc < this.lastDistPerc) {
			this.lastDistPerc = perc;
		}
	}
}
