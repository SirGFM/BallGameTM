using Particles = UnityEngine.ParticleSystem;
using Vec3 = UnityEngine.Vector3;

/**
 * ExplodeOnKill implements the KillAtIface,
 * launching particles when the object is killed.
 */

public class ExplodeOnKill : UnityEngine.MonoBehaviour, KillAtIface {

	/** The object's particle system. */
	private Particles particles;

	void Start() {
		this.particles = this.GetComponent<Particles>();
		if (this.particles == null) {
			throw new System.Exception($"{this} requires a particles system!");
		}

		this.particles.Stop(true);
	}

	public void KillAt(Vec3 pos) {
		var shape = this.particles.shape;
		shape.position = pos;
		this.particles.Play(true);
	}
}
