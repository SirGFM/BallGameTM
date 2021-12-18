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
					Vec3 v3 = new Vec3(0.0f, this.Jump, 0.0f);
					this.rb.AddForce(v3);
					this.tryJump = 0.0f;
					break;
				}

				yield return new UnityEngine.WaitForFixedUpdate();
			}

			/* Wait for the component to leave the floor for at least one
			 * frame. As a safe guard, if the component doesn't leave the
			 * ground for 1 second, assumes something went wrong and go
			 * back to the initial state.*/
			for (float t = 0.0f; t < 1.0f && this.onFloor;
					t += Time.fixedDeltaTime) {
				yield return new UnityEngine.WaitForFixedUpdate();
			}

			/* Avoid infinite loops (when !this.onFloor). */
			yield return new UnityEngine.WaitForFixedUpdate();
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
		this.onFloor = true;
	}

	public void OnNotTouchingAny() {
		this.onFloor = false;
	}
}
