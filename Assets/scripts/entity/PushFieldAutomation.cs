using Col = UnityEngine.BoxCollider;
using Particles = UnityEngine.ParticleSystem;
using UEMath = UnityEngine.Mathf;
using Vec3 = UnityEngine.Vector3;

/**
 * PushFieldAutomation configures every sub-object that composes a
 * "push field" object.
 */

public enum PushFieldDirection {
	Up,
	Down,
	Forward,
	Backward,
}

public class PushFieldAutomation : UnityEngine.MonoBehaviour {
	/** Block automatically updating the sub-objects, so they may be
	 * manually tweaked. */
	public bool BlockAutomation = false;

	/** Forcefully update the child component, since there's no way to
	 * automatically detect changes on the transform. */
	public bool ForceUpdate = false;

	/** The relative direction of the push field. */
	public PushFieldDirection Direction;

	/** Intensity to push entities. */
	public float Force;

	/** The dimensions of this push field. */
	public Vec3 Size;

	/** How long each fog particle should live. */
	public float FogDuration = 1.0f;
	/** How fast each fog particle should move. */
	public float FogSpeed = 1.0f;
	/** How many fog particles should exist at once. */
	public int FogMaxParticles = 1000;

	/** How long each arrow particle should live. */
	public float ArrowDuration = 1.0f;
	/** How fast each arrow particle should move. */
	public float ArrowSpeed = 1.0f;
	/** How many arrow particles should exist at once. */
	public int ArrowMaxParticles = 1000;

	/** Name of the game object with the fog emitter. */
	public string FogObjectName = "Fog";
	/** Name of the game object with the arrow emitter. */
	public string ArrowObjectName = "ArrowParticleEmitter";

	/**
	 * Configure the particle emitter to match this object's dimensions
	 */
	private void setParticle(Particles p, float dur, float speed, int max) {
		/* XXX: For some weird reason, although the ParticleSystem
		 * components appears to be fields, they cannot be accessed and
		 * modified directly... */
		var main = p.main;

		main.startLifetime = dur;
		main.startSpeed = speed;
		main.maxParticles = max;

		Vec3 ang = this.transform.eulerAngles;
		var shape = p.shape;

		switch (this.Direction) {
		case PushFieldDirection.Up:
			shape.position = new Vec3(0.0f, -this.Size.y * 0.5f, 0.0f);
			shape.rotation = new Vec3(-90.0f, 0.0f, 0.0f);
			shape.scale = new Vec3(this.Size.x, this.Size.z, 1.0f);
			break;
		case PushFieldDirection.Down:
			shape.position = new Vec3(0.0f, this.Size.y * 0.5f, 0.0f);
			shape.rotation = new Vec3(90.0f, 0.0f, 0.0f);
			shape.scale = new Vec3(this.Size.x, this.Size.z, 1.0f);
			break;
		case PushFieldDirection.Forward:
			shape.position = new Vec3(0.0f, 0.0f, -this.Size.z * 0.5f);
			shape.rotation = new Vec3(0.0f, 0.0f, 0.0f);
			shape.scale = new Vec3(this.Size.x, this.Size.y, 1.0f);
			break;
		case PushFieldDirection.Backward:
			shape.position = new Vec3(0.0f, 0.0f, this.Size.z * 0.5f);
			shape.rotation = new Vec3(0.0f, 180.0f, 0.0f);
			shape.scale = new Vec3(this.Size.x, this.Size.y, 1.0f);
			break;
		}
	}

	/**
	 * Configure the rotation of the particles emitted by the given
	 * particle mitter.
	 *
	 * @param p: The particle emitter.
	 * @param minX: The minimum rotation in the X axis.
	 * @param maxX: The maximum rotation in the X axis.
	 * @param minY: The minimum rotation in the Y axis.
	 * @param maxY: The maximum rotation in the Y axis.
	 * @param minZ: The minimum rotation in the Z axis.
	 * @param maxZ: The maximum rotation in the X axis.
	 */
	private void set3dRotation(Particles p, float minX, float maxX,
			float minY, float maxY, float minZ, float maxZ) {
		minX = minX * UEMath.PI / 180.0f;
		maxX = maxX * UEMath.PI / 180.0f;
		minY = minY * UEMath.PI / 180.0f;
		maxY = maxY * UEMath.PI / 180.0f;
		minZ = minZ * UEMath.PI / 180.0f;
		maxZ = maxZ * UEMath.PI / 180.0f;

		/* XXX: For some weird reason, although the main component of the
		 * ParticleSystem appears to be fields, they cannot be accessed and
		 * modified directly... */
		var main = p.main;

		main.startRotationX = new Particles.MinMaxCurve(minX, maxX);
		main.startRotationY = new Particles.MinMaxCurve(minY, maxY);
		main.startRotationZ = new Particles.MinMaxCurve(minZ, maxZ);
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
		Col c = this.gameObject.GetComponentInChildren<Col>();
		if (c == null) {
			throw new System.Exception($"{this} requires a child with a BoxCollider component!");
		}
		c.size = this.Size;

		/* Update the push component based on the direction of this object
		 * and on the direction of the force field. */
		Push p = this.gameObject.GetComponentInChildren<Push>();
		if (p == null) {
			throw new System.Exception($"{this} requires a child with a Push component!");
		}

		switch (this.Direction) {
		case PushFieldDirection.Up:
			p.Direction = this.transform.up.normalized;
			break;
		case PushFieldDirection.Down:
			p.Direction = -1.0f * this.transform.up.normalized;
			break;
		case PushFieldDirection.Forward:
			p.Direction = this.transform.forward.normalized;
			break;
		case PushFieldDirection.Backward:
			p.Direction = -1.0f * this.transform.forward.normalized;
			break;
		}
		p.Force = this.Force;

		/* Update the arrow and the fog particle emitter. */
		Particles[] ps = this.gameObject.GetComponentsInChildren<Particles>();
		if (ps == null || ps.Length != 2) {
			throw new System.Exception($"{this} requires exactly two children with a ParticleSystem component!");
		}

		Particles fog = null;
		Particles arrow = null;

		foreach (Particles tmp in ps) {
			if (tmp.gameObject.name == this.FogObjectName) {
				fog = tmp;
			}
			if (tmp.gameObject.name == this.ArrowObjectName) {
				arrow = tmp;
			}
		}
		if (fog == null || arrow == null) {
			throw new System.Exception($"{this} requires a child named {this.FogObjectName} and another child named {this.ArrowObjectName}, both with a ParticleSystem");
		}

		this.setParticle(fog, this.FogDuration, this.FogSpeed,
				this.FogMaxParticles);
		this.setParticle(arrow, this.ArrowDuration, this.ArrowSpeed,
				this.ArrowMaxParticles);

		switch (this.Direction) {
		case PushFieldDirection.Up:
			this.set3dRotation(arrow,
					0.0f, 0.0f,
					0.0f, 180.0f,
					0.0f, 0.0f);
			break;
		case PushFieldDirection.Down:
			this.set3dRotation(arrow,
					0.0f, 0.0f,
					0.0f, 180.0f,
					180.0f, 180.0f);
			break;
		case PushFieldDirection.Forward:
			this.set3dRotation(arrow,
					75.0f, 105.0f,
					0.0f, 0.0f,
					0.0f, 0.0f);
			break;
		case PushFieldDirection.Backward:
			this.set3dRotation(arrow,
					75.0f, 105.0f,
					180.0f, 180.0f,
					0.0f, 0.0f);
			break;
		}
	}
}
