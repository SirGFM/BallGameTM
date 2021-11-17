using Col = UnityEngine.Collider;
using GO = UnityEngine.GameObject;
using RB = UnityEngine.Rigidbody;
using Vec3 = UnityEngine.Vector3;

/**
 * MovingPlatform implements SetForceIface to move the component based on
 * markers placed on the level. This entity should also have an Interrupt
 * component so it might more easily move back and forth, or in a given
 * path, though living this component off may lead to fun and interesting
 * results.
 *
 * To carry components around, this entity pushes any touching component
 * with the same force currently being applied to this entity (technically
 * it applies slighly more force to the touching entity than to itself).
 *
 * XXX: For the best results, this should have the same drag as the
 * touching entity!
 */

public class MovingPlatform : BaseRemoteAction, SetForceIface {
	/** The component's rigidbody. */
	private RB rb;

	/** The force currently being applied to the entity. */
	private Vec3 force;

	/** Factor applied to the force applied on the touching entity. */
	public float ForceCorrection = 1.5f;

	void Start() {
		this.rb = this.GetComponent<RB>();
		if (this.rb == null) {
			throw new System.Exception($"{this} requires a Rigidbody component!");
		}

		Col c = this.GetComponent<Col>();
		if (c == null) {
			throw new System.Exception($"{this} requires any Collider!");
		}
		else if (!c.isTrigger) {
			throw new System.Exception($"{this}'s Collider must be a trigger!");
		}

		this.force = new Vec3();
	}

	void FixedUpdate() {
		this.rb.AddForce(this.force);
	}

	public void OnSetForce(Vec3 force) {
		this.force = force;
	}

	void OnTriggerStay(Col other) {
		bool blocked = false;

		issueEvent<BlockIface>( (x,y) => x.IsBlocked(out blocked));

		if (!blocked) {
			GO tgt = other.gameObject;

			/* XXX: Because of... reasons (most likely drag, but I honestly
			 * don't know), the platform must apply slightly more force to
			 * the object being carried. */
			issueEvent<PushIface>(
					(x,y) => x.OnPush(this.force * this.ForceCorrection),
					tgt);
		}
	}
}
