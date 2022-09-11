using Coroutine = UnityEngine.Coroutine;
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

	/**
	 * Set the horizontal angle that the camera should go back to, from a
	 * 2D vector in the X/Z axis.
	 *
	 * @param x: The X axis of 2D vector defining the angle.
	 * @param y: The Z axis of 2D vector defining the angle.
	 */
	void SetResetAngle(float x, float z);
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
	private float verAng = 30.0f;
	/** Dynamic distance, to avoid/minimize clipping. */
	private float curDist;

	/** The position of the mouse on the last frame. */
    private Vec3 lastMouse;

	/** Coroutine for resetting the camera. */
	private Coroutine resetCor;
	/** The X axis of 2D vector defining the resting angle. */
	private float resetX = 0.0f;
	/** The Z axis of 2D vector defining the resting angle. */
	private float resetZ = 0.0f;

	/** Maximum angle when rotating the camera upward. */
	private const float maxTopAngle = 88.0f;
	/** Maximum angle when rotating the camera downward. */
	private const float maxBottomAngle = 272.0f;

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
		this.resetCor = null;
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

	/**
	 * Return a modifier for whether a rotation from an angle to another
	 * should be done clock or counter-clockwise.
	 *
	 * @param src: The source angle (in degrees).
	 * @param dst: The destination angle (in degrees).
	 * @param diff: The absolute difference between the angles (in degrees).
	 */
	private float getDeltaAngle(float src, float dst, float diff) {
		float posTgt = Math.NormalizeAngle(src + diff);
		float negTgt = Math.NormalizeAngle(src - diff);

		float dp = UEMath.Abs(dst - posTgt);
		float dn = UEMath.Abs(dst - negTgt);

		if (dp < 0.01 || dp >= 360.0f && dp - 360.0f < 0.01) {
			return 1.0f;
		}
		else if (dn < 0.01 || dn >= 360.0f && dn - 360.0f < 0.01) {
			return -1.0f;
		}

		/* This fails in some situations (e.g., if src == 270 && dst == 0),
		 * so the conditional above manually checks whether the rotation
		 * is clockwise or counter-clockwise (accounting for when the
		 * target is past, or equal to, 360 degrees).
		 * This was left as is as a failsafe, in case the above fails. */
		if (dst > src && dst < src + 180.0f) {
			return 1.0f;
		}
		return -1.0f;
	}

	/**
	 * Rotate the camera back to a given position.
	 *
	 * @param hor: The horizontal angle.
	 * @param ver: The vertical angle.
	 */
	private System.Collections.IEnumerator resetCamera(float hor, float ver) {
		float dx = 0.0f;
		float dy = 0.0f;

		while (hor != Math.NormalizeAngle(hor)) {
			hor = Math.NormalizeAngle(hor);
		}
		while (ver != Math.NormalizeAngle(ver)) {
			ver = Math.NormalizeAngle(ver);
		}

		dx = Math.DiffAngle(this.horAng, hor);
		dx *= 5.0f * this.getDeltaAngle(this.horAng, hor, dx);
		dy = Math.DiffAngle(this.verAng, ver);
		dy *= 5.0f * this.getDeltaAngle(this.verAng, ver, dy);

		float maxTime = 0.0f;
		while ((this.horAng != hor || this.verAng != ver) &&
				maxTime < 0.75f) {
			int count = 10;
			float dt = Time.deltaTime / (float)count;

			/* To avoid precision errors, improve the integration by using
			 * a smaller delta time and running this a few times each
			 * frame. Otherwise, the camera could end up skipping over the
			 * point where it's less than 1 degree from the destination. */
			for (; count > 0; count--) {
				if (Math.DiffAngle(this.horAng, hor) < 1.0f) {
					this.horAng = hor;
					dx = 0.0f;
				}
				if (Math.DiffAngle(this.verAng, ver) < 1.0f) {
					this.verAng = ver;
					dy = 0.0f;
				}

				this.horAng += dx * dt;
				this.verAng += dy * dt;
			}

			maxTime += Time.deltaTime;
			yield return null;
		}
		/* If the loop above timed out, set the correct angle. */
		this.horAng = hor;
		this.verAng = ver;

		this.resetCor = null;
	}

	void Update() {
		float dVerAng = 0.0f;

		if (this.target == null) {
			return;
		}

		if (this.resetCor != null) {
			/* Don't do anything until the camera has reset. */
		}
		else if (Input.GetResetCameraJustPressed()) {
			float ang;

			if (this.resetZ != 0.0f || this.resetX != 0.0f) {
				ang = UEMath.Atan2(this.resetZ, this.resetX);
				ang = UEMath.Rad2Deg * ang;
			}
			else {
				ang = 0.0f;
			}

			this.resetCor = this.StartCoroutine(this.resetCamera(ang, 30.0f));
		}
		else if (Input.GetMouseCameraEnabled()) {
			Vec3 mouseDelta = Input.GetMousePosition() - this.lastMouse;

			this.horAng += mouseDelta.x * Global.camX;
			dVerAng = mouseDelta.y * Global.camY;
		}
		else {
			Vec2 cam = Input.GetCamera();

			this.horAng += cam.x * 2.5f * Global.camX;
			dVerAng = cam.y * 2.5f * Global.camY;
		}

		/* Limit the camera from going both over and under the player, as those
		 * situations cause the rotation axis to invert. */
		if (this.verAng <= maxTopAngle && this.verAng + dVerAng >= maxTopAngle) {
			this.verAng = maxTopAngle;
		}
		else if (this.verAng >= maxBottomAngle && this.verAng + dVerAng <= maxBottomAngle) {
			this.verAng = maxBottomAngle;
		}
		else if (dVerAng != 0.0f) {
			this.verAng += dVerAng;
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

	public void SetResetAngle(float x, float z) {
		this.resetX = x;
		this.resetZ = z;
	}
}
