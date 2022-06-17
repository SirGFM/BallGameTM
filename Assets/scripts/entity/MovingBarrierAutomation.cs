using BoxCol = UnityEngine.BoxCollider;
using SphCol = UnityEngine.SphereCollider;
using GO = UnityEngine.GameObject;
using UEMath = UnityEngine.Mathf;
using Vec3 = UnityEngine.Vector3;

/**
 * MovingBarrierAutomation configures every sub-object that composes a
 * Barrier that moves when every child collectible is collected.
 */

public class MovingBarrierAutomation : UnityEngine.MonoBehaviour {

	/** Block automatically updating the sub-objects, so they may be
	 * manually tweaked. */
	public bool BlockAutomation = true;

	/** Forcefully update the child component, since there's no way to
	 * automatically detect changes on the transform. */
	public bool ForceUpdate = false;

	/** Use a collider to detect whether or not the player (or anything,
	 * really) is inside an specified area to enable or disable the particle
	 * effect in the collectibles. */
	public bool DeactivateParticleIfOutside = false;

	/** Resize the sphere collider, used to detect whether or not the
	 * particle of collectibles should be enabled, to contain every
	 * collectible.
	 * This is ignored unless DeactivateParticleIfOutside is true! */
	public bool RecalculateParticleDeactivationArea = false;

	/** Multiplier used to modify the particle deactivation area. */
	public float ParticleDeactivationAreaMultiplier = 2.5f;

	/** How big the barrier should be. */
	public Vec3 BarrierSize;

	/** How long should the movement take. */
	public float MovementDuration = 1.0f;

	/** The movement direction. */
	public MovementDirection Direction = MovementDirection.Down;

	private T getComponentInAllChildren<T>() {
		T[] components;

		components = this.GetComponentsInChildren<T>();
		if (components.Length != 1) {
			throw new System.Exception($"{this} should have exactly 1 child with a {typeof(T).Name}");
		}

		return components[0];
	}

	void OnValidate() {
		if (this.BlockAutomation) {
			return;
		}
		if (this.ForceUpdate) {
			this.ForceUpdate = false;
		}

		MoveOnDeactivate mod;
		mod = getComponentInAllChildren<MoveOnDeactivate>();
		this.updateBarrier(mod);

		ActivateWhileInside awi;
		awi = getComponentInAllChildren<ActivateWhileInside>();
		awi.enabled = this.DeactivateParticleIfOutside;
		if (awi.enabled && this.RecalculateParticleDeactivationArea) {
			this.updateParticleDeactivationArea(awi, mod.gameObject);
		}

		DeactivateChildrenAndSelf dcas;
		dcas = getComponentInAllChildren<DeactivateChildrenAndSelf>();
		dcas.timeToDeactivate = this.MovementDuration;
	}

	private void updateBarrier(MoveOnDeactivate mod) {
		GO obj = mod.gameObject;
		BoxCol box = obj.GetComponent<BoxCol>();
		if (box == null) {
			throw new System.Exception($"{obj} must have a box collider");
		}

		/* Update the box's scale, so the model may be scaled as well.
		 * Since the entire area is centered around the bottom of the barrier,
		 * the box may be moved upwards to align correctly. */
		float y = this.BarrierSize.y / 2;
		box.transform.localPosition = new Vec3(0.0f, y, 0.0f);
		box.transform.localScale = this.BarrierSize;
		box.size = new Vec3(1.0f, 1.0f, 1.0f);

		/* Update the actual movement when the object is deactivated. */
		Vec3 movement;
		switch (this.Direction) {
		case MovementDirection.Up:
			movement = new Vec3(0.0f, this.BarrierSize.y, 0.0f);
			break;
		case MovementDirection.Down:
			movement = new Vec3(0.0f, -this.BarrierSize.y, 0.0f);
			break;
		case MovementDirection.Right:
			movement = new Vec3(this.BarrierSize.x, 0.0f, 0.0f);
			break;
		case MovementDirection.Left:
			movement = new Vec3(-this.BarrierSize.x, 0.0f, 0.0f);
			break;
		case MovementDirection.Forward:
			movement = new Vec3(0.0f, 0.0f, this.BarrierSize.z);
			break;
		case MovementDirection.Back:
			movement = new Vec3(0.0f, 0.0f, -this.BarrierSize.z);
			break;
		default:
			throw new System.Exception($"Invalid MovementDirection {this.Direction}!");
		}
		mod.Movement = movement;
		mod.Duration = this.MovementDuration;
	}

	private void updateParticleDeactivationArea(ActivateWhileInside obj,
			GO barrier) {
		CollectOnTouch[] children;
		Vec3 min = new Vec3();
		Vec3 max = new Vec3();

		/* Calculate the box that encloses every collectible and the
		 * barrier. */
		for (int i = 0; i < 3; i++) {
			float val = barrier.transform.position[i];
			min[i] = val;
			max[i] = val;
		}

		children = obj.GetComponentsInChildren<CollectOnTouch>();
		foreach (CollectOnTouch child in children) {
			Vec3 pos = child.transform.position;

			for (int i = 0; i < 3; i++) {
				min[i] = UEMath.Min(min[i], pos[i]);
				max[i] = UEMath.Max(max[i], pos[i]);
			}
		}

		/* Use the longest axis as the sphere's diameter and
		 * calculate the local center of the sphere. */
		float diameter = 0.0f;
		Vec3 center = new Vec3();
		for (int i = 0; i < 3; i++) {
			diameter = UEMath.Max(diameter, max[i] - min[i]);
			center[i] = (max[i] + min[i]) * 0.5f;
		}
		center -= this.transform.position;
		diameter *= this.ParticleDeactivationAreaMultiplier;

		/* Adjust the sphere's attributes. */
		SphCol col = obj.GetComponent<SphCol>();
		if (col == null) {
			throw new System.Exception($"{obj} must have a circle collider");
		}
		col.center = center;
		col.radius = diameter * 0.5f;
	}
}

/** Identify all movement directions allowed for the
 * MovingBarrierAutomation automation. */
public enum MovementDirection {
	Up,
	Down,
	Right,
	Left,
	Forward,
	Back,
}
