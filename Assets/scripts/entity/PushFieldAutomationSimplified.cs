using Col = UnityEngine.BoxCollider;
using GO = UnityEngine.GameObject;
using Particles = UnityEngine.ParticleSystem;
using UEMath = UnityEngine.Mathf;
using Vec3 = UnityEngine.Vector3;

/**
 * PushFieldAutomation configures every sub-object that composes a
 * "push field" object.
 */

public class PushFieldAutomationSimplified : UnityEngine.MonoBehaviour, SetActiveIface {

	/** The object that pushes other objects in a given direction. */
	private GO forceObject;

	/** Default value for the Force. */
	private static float defaultForce = 5f;

	/** Default value for Size. */
	private static Vec3 defaultSize = new Vec3(10f, 10f, 10f);

	/** Default value for the Particle Lifetime. */
	private static float defaultLifetime = 1.0f;

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

		float lifeTime = defaultLifetime;

		/* Calculate the emitter offset on the Y axis, to shrink it so
		 * particles should barely live over the end of the area. */
		float offY = defaultSpeed * scaleFactorForce * lifeTime;
		float height = this.Size.y;

		/* Also, decrease the lifeTime if the offset were too big. */
		if (offY >= height) {
			lifeTime = defaultLifetime / (scaleFactorForce * 0.5f);
			offY = defaultSpeed * scaleFactorForce * lifeTime;
		}

		main.startLifetime = lifeTime;
		main.startSpeed = defaultSpeed*scaleFactorForce;
		emission.rateOverTime = baseEmission*scaleFactorX*scaleFactorZ*scaleFactorForce;

		if (offY < height) {
			height -= offY;
		}
		else {
			height = 0.1f;
		}

		shape.position = new Vec3(0.0f, -offY * 0.5f, 0.0f);
		shape.rotation = new Vec3(-90.0f, 0.0f, 0.0f);
		shape.scale = new Vec3(this.Size.x, this.Size.z, height);
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

	/**
	 * Retrive the push object, in case this object receives an event
	 * before it's even initialized.
	 */
	private void getPushObject() {
		if (this.forceObject != null) {
			return;
		}

		Push p = this.gameObject.GetComponentInChildren<Push>();
		if (p == null) {
			throw new System.Exception($"{this} requires a child with a Push component!");
		}

		this.forceObject = p.gameObject;
	}

	public void SetActive(out bool handled, bool enable) {
		this.getPushObject();

		if (enable) {
			this.FogObject.Play(true);
			this.WindObject.Play(true);
			this.forceObject.SetActive(true);
		}
		else {
			this.FogObject.Stop(true);
			this.WindObject.Stop(true);
			this.forceObject.SetActive(false);
		}
		handled = true;
	}

	void Start() {
		float perc = Config.getParticleQuantity();
		float fogPerc = perc;

		if (fogPerc < 0.25f) {
			fogPerc *= 0.1f;
		}
		else if (fogPerc < 0.5f) {
			fogPerc *= 0.25f;
		}
		else if (fogPerc < 1.0f) {
			fogPerc *= 0.5f;
		}

		this.setParticle(FogObject, FogEmission * fogPerc);
		this.setParticle(WindObject, WindEmission * perc);
	}
}
