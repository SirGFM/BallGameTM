using Collider = UnityEngine.Collider;
using EvSys = UnityEngine.EventSystems;
using Rigidbody = UnityEngine.Rigidbody;
using Time = UnityEngine.Time;
using Transform = UnityEngine.Transform;
using Vec3 = UnityEngine.Vector3;

/**
 * MoveOnDeactivate implementes SetActiveIface, moving the game object when
 * it's deactivated.
 */

public class MoveOnDeactivate : UnityEngine.MonoBehaviour, SetActiveIface {

	/** How long the movement takes. */
	public float Duration = 1.0f;

	/** The movement made by the object. */
	public Vec3 Movement;

	/** Whether the object has already started moving. */
	private bool isMoving = false;

	public void SetActive(out bool handled, bool enable) {
		handled = true;
		if (this.isMoving) {
			return;
		}

		if (!enable) {
			this.startMoving();
		}
		else {
			this.gameObject.SetActive(enable);
		}
	}

	/**
	 * Setup the object's movement and start moving it.
	 */
	private void startMoving() {
		this.isMoving = true;

		Rigidbody rb = this.GetComponent<Rigidbody>();

		if (rb) {
			/* Ensure that rigidbodies are kinect,
			 * so they may be moved around without issues. */
			rb.isKinematic = true;
		}
		else {
			/* If the object doesn't have a rigid body,
			 * it's supposed to be a static collider.
			 * In this case, disable the collider
			 * and move the  model manually. */
			foreach (Collider col in this.GetComponents<Collider>()) {
				col.enabled = false;
			}
		}

		Transform t = this.GetComponent<Transform>();
		this.StartCoroutine(this.moveObject(t));
	}

	/**
	 * Move the object as configured and then disable it.
	 *
	 * @param t: The transform to be moved around.
	 */
	private System.Collections.IEnumerator moveObject(Transform obj) {
		Vec3 speed = this.Movement / this.Duration;

		for (float t = 0; t < this.Duration; t += Time.fixedDeltaTime) {
			obj.position = obj.position + speed * Time.fixedDeltaTime;

			yield return new UnityEngine.WaitForFixedUpdate();
		}

		this.gameObject.SetActive(false);
	}
}
