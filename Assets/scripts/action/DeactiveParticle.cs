using Particles = UnityEngine.ParticleSystem;

/**
 * DeactiveParticle implements the SetActiveIface for a simple object with
 * a single particle system.
 */

public class DeactiveParticle : UnityEngine.MonoBehaviour, SetActiveIface {

	/** The object's particle system. */
	private Particles particles;

	void Start() {
		this.particles = this.GetComponentInChildren<Particles>();
		if (this.particles == null) {
			throw new System.Exception($"{this} requires a particles system!");
		}
	}

	public void SetActive(out bool handled, bool enable) {
		if (enable) {
			this.particles.Play(true);
		}
		else {
			this.particles.Stop(true);
		}

		handled = true;
	}
}
