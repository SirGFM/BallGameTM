using GO = UnityEngine.GameObject;
using UEMath = UnityEngine.Mathf;
using RB = UnityEngine.Rigidbody;
using Time = UnityEngine.Time;
using Transform = UnityEngine.Transform;
using Vec3 = UnityEngine.Vector3;

/**
 * Ball is the main component of the player-controlled ball entity. It
 * handles movement and respond to action from various events that the
 * entity.
 *
 * This entity is expected to receive PushIface events, in case it enters
 * a conveyor belt or a force field, and SetDragIface events, in case it
 * enters ice or a rouch terrain.
 *
 * Otherwise, it "simply" applies force to the entity based on the input.
 * The amount of force applied does increase if the entity continues
 * moving, causing a nice acceleration.
 */

public class Ball : BaseRemoteAction, PushIface, SetDragIface {
	/** The main camera, retrieved from a GetMainCamera event. */
    private Transform cam;
	/** This object's rigidbody. */
    private RB rb;
	/** This object's default drag, so it may recover it on a OnResetDrag
	 * event. */
    private float drag;
	/** Current force being applied to the object. */
    private float speed;
	/** How long has elapsed since the entity started to move. */
    private float speedDT;

	/** Force applied when the object starts moving. */
    public float MinSpeed = 5.0f;
	/** How long it takes, in seconds, for the maximum force to be
	 * applied. */
    public float TimeToMaxSpeed = 30.0f;
	/** Maximum force applied to object, after TimeToMaxSpeed seconds have
	 * elapsed. */
    public float MaxSpeed = 25.0f;
	/** Deadzone for increasing the movement. */
    public float InputThreshold = 0.33f;

    void Start() {
        this.rb = this.GetComponent<RB>();
        if (this.rb == null) {
            throw new System.Exception($"{this} requires a Rigidbody component!");
        }

        this.drag = this.rb.drag;

		/* Try to retrieve and configure the main camera, and on failure
		 * set a coroutine to try it again at a later time. */
		if (!configureMainCamera()) {
			this.StartCoroutine(this.retryConfigureMainCamera());
		}

		this.updateSpeed(0.0f, 0.0f);
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

		cam = null;
		this.rootEvent<CameraHelperIface>(
				(x,y) => x.GetMainCamera(out cam));
		if (cam == null) {
			return false;
		}

        this.cam = cam.transform;

		this.issueEvent<CameraIface>(
				(x,y) => x.SetCameraTarget(this.gameObject),
				cam);
		return true;
	}

	/**
	 * Update the component's current speed based on for how long the
	 * axis have been outside a deadzone.
	 *
	 * @param x: The horizontal movement.
	 * @param y: The vertcal movement.
	 */
	private void updateSpeed(float x, float y) {
		if (UEMath.Sqrt(x * x + y * y) < this.InputThreshold) {
			this.speed = this.MinSpeed;
			this.speedDT = 0.0f;
		}
		else if (this.speedDT < this.TimeToMaxSpeed) {
			float perc, delta;

			this.speedDT += Time.fixedDeltaTime;

			perc = this.speedDT / this.TimeToMaxSpeed;
			delta = this.MaxSpeed - this.MinSpeed;
			this.speed = this.MinSpeed + delta * perc;
		}
		else {
			this.speed = this.MaxSpeed;
		}
	}

    void FixedUpdate() {
		/* TODO: Update the input method */
        float x = UnityEngine.Input.GetAxis("Horizontal");
        float y = UnityEngine.Input.GetAxis("Vertical");

		this.updateSpeed(x, y);

		float ang = 0.0f;
		if (this.cam != null) {
			ang = this.cam.eulerAngles.y;
		}

        (x, y) = Math.RotateVec2(x, y, ang);
        Vec3 v3 = new Vec3(x, 0.0f, y);
        v3 *= this.speed;
        this.rb.AddForce(v3);
    }

    public void OnPush(Vec3 force) {
        this.rb.AddForce(force);
    }

    public void OnSetDrag(float drag) {
        this.rb.drag = drag;
    }

    public void OnResetDrag() {
        this.rb.drag = this.drag;
    }
}
