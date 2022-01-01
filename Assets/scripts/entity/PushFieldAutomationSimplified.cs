using Col = UnityEngine.BoxCollider;
using Particles = UnityEngine.ParticleSystem;
using UEMath = UnityEngine.Mathf;
using Vec3 = UnityEngine.Vector3;

/**
 * PushFieldAutomation configures every sub-object that composes a
 * "push field" object.
 */

public class PushFieldAutomationSimplified : UnityEngine.MonoBehaviour {

	/** Default value for the Force. */
	private static float defaultForce = 5f;

	/** Default value for Size. */
	private static Vec3 defaultSize = new Vec3(10f, 10f, 10f);

	/** Default value for the Particle Lifetime. */
	private static float defaultLifetime = 2f;

	/** Default value for the Particle Speed. */
	private static float defaultSpeed = 5f;

	/** Block automatically updating the sub-objects, so they may be
	 * manually tweaked. */
	public bool BlockAutomation = false;

	/** Forcefully update the child component, since there's no way to
	 * automatically detect changes on the transform. */
	public bool ForceUpdate = false;

	/** Intensity to push entities. */
	public float Force = defaultForce;

	/** The dimensions of this push field. */
	public Vec3 Size = defaultSize;

	/** Box Collider for the effect range. */
	public Col Collider;

	/** Particle System with the fog emitter. */
	public Particles FogObject;
	/** Particle System with the wind emitter. */
	public Particles WindObject;

	/** Base Fog Emission Rate. */
	public float FogEmission = 60f;

	/** Base Wind Emission Rate. */
	public float WindEmission = 4f;

	/**
	 * Configure the particle emitter to match this object's dimensions
	 */
	private void setParticle(Particles p, float baseEmission) {
		/* XXX: For some weird reason, although the ParticleSystem
		 * components appears to be fields, they cannot be accessed and
		 * modified directly... */
		var main = p.main;
		var emission = p.emission;
		var shape = p.shape;

		float scaleFactorX = this.Size.x/defaultSize.x;
		float scaleFactorY = this.Size.y/defaultSize.y;
		float scaleFactorZ = this.Size.z/defaultSize.z;
		float scaleFactorForce = this.Force/defaultForce;

		main.startLifetime = defaultLifetime*scaleFactorY/scaleFactorForce;
		main.startSpeed = defaultSpeed*scaleFactorForce;
		emission.rateOverTime = baseEmission*scaleFactorX*scaleFactorZ*scaleFactorForce;

		shape.position = new Vec3(0.0f, -this.Size.y * 0.5f, 0.0f);
		shape.rotation = new Vec3(-90.0f, 0.0f, 0.0f);
		shape.scale = new Vec3(this.Size.x, this.Size.z, 1.0f);
	}

	void OnValidate() {
		if (this.BlockAutomation) {
			return;
		}

		if (this.ForceUpdate) {
			this.ForceUpdate = false;
		}

		if (this.transform.localScale.x != 1.0f ||
				this.transform.localScale.y != 1.0f ||
				this.transform.localScale.z != 1.0f) {
			UnityEngine.Debug.LogWarning($"The scale in the transform is ignored by particle emitters in {this.gameObject.name}! To change the collider's dimensions, modify this component's 'Size' field. Reverting the scale back to the default...");
			this.transform.localScale = new Vec3(1.0f, 1.0f, 1.0f);
		}

		/* Update the collider, assuming that it's centered into the push
		 * field. */
		Collider.size = this.Size;

		/* Update the push component based on the direction of this object
		 * and on the direction of the force field. */
		Push p = this.gameObject.GetComponentInChildren<Push>();
		if (p == null) {
			throw new System.Exception($"{this} requires a child with a Push component!");
		}
		p.Direction = this.transform.up.normalized;
		p.Force = this.Force;

		this.setParticle(FogObject, FogEmission);
		this.setParticle(WindObject, WindEmission);
	}
}
