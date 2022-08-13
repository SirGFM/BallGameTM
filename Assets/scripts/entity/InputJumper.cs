using RB = UnityEngine.Rigidbody;
using Time = UnityEngine.Time;
using Vec3 = UnityEngine.Vector3;

public class InputJumper : UnityEngine.MonoBehaviour, DetectColliderIface {
	/* The object's Rigidbody. */
    private RB rb;
	/* Count down for when the component is trying to jump. */
	private float tryJump;
	/* Whether the component is currently on the floor. */
	private bool onFloor;

	/* Jump force. */
    public float Jump = 500.0f;
	/* How much time before the component touches the floor it accepts a
	 * jump input. */
    public float JumpBuffer = 0.3f;

    void Start() {
        this.rb = this.GetComponent<RB>();
        if (this.rb == null) {
            throw new System.Exception($"{this} requires a Rigidbody component!");
        }

		this.tryJump = 0.0f;
		this.onFloor = false;

		this.StartCoroutine(this.jumpStateMachine());
    }

	/**
	 * jumpStateMachine controls jumping.
	 *
	 * This component should update its onFloor property from OnTouchingAny
	 * and OnNotTouchingAny events, and the component shall only jump when
	 * it's on a floor.
	 *
	 * Whenever the jump input is pressed, a timer start so jumping may be
	 * briefly buffered.
	 */
	private System.Collections.IEnumerator jumpStateMachine() {
		while (true) {
			while (this.onFloor) {
				if (this.tryJump > 0.0f) {
					Global.Sfx.playJump();

					Vec3 v3 = new Vec3(0.0f, this.Jump, 0.0f);
					this.rb.AddForce(v3);
					this.tryJump = 0.0f;
					break;
				}

				yield return new UnityEngine.WaitForFixedUpdate();
			}

			/* XXX: Unity's physics are WEIRD and sometimes decides to
			 * delay issuing physics events because F*** you... Forcefully
			 * wait 10 frames before the loop bellow to avoid issues if the
			 * physics engine decides that none of the floor colliders are
			 * touching anything right after jumping (even though they
			 * visually ARE inside a geometry), but it later decides that
			 * the colliders are still inside a geometry (at which point,
			 * they visually aren't anymore).
			 *
			 * The reason to avoid this corner case is that it causes some
			 * jumps to be way higher than the others, usually in sloped
			 * surfaces. */
			float t = 0.0f;
			for (int i = 0; i < 10; i++) {
				t += Time.fixedDeltaTime;
				yield return new UnityEngine.WaitForFixedUpdate();
			}

			/* Wait for the component to leave the floor for at least one
			 * frame. As a safe guard, if the component doesn't leave the
			 * ground for 1 second, assumes something went wrong and go
			 * back to the initial state.*/
			for (; t < 1.0f && this.onFloor;
					t += Time.fixedDeltaTime) {
				yield return new UnityEngine.WaitForFixedUpdate();
			}

			/* Delay getting to the main loop until the component is back
			 * at the floor. Otherwise, the 10 frames loop will possibly
			 * delay some jumps. */
			while (!this.onFloor) {
				yield return new UnityEngine.WaitForFixedUpdate();
			}
		}
	}

    void FixedUpdate() {
		if (Input.GetActionButton()) {
			this.tryJump = this.JumpBuffer;
		}
		else if (this.tryJump > 0.0f) {
			this.tryJump -= Time.fixedDeltaTime;
		}
    }

	public void OnTouchingAny() {
		if (!this.onFloor) {
			Global.Sfx.playFall();
		}
		this.onFloor = true;
	}

	public void OnNotTouchingAny() {
		this.onFloor = false;
	}
}
