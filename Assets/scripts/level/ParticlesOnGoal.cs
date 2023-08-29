using Particles = UnityEngine.ParticleSystem;

/**
 * ParticlesOnGoal plays the object's particles when it receives an OnGoal signal.
 */

public class ParticlesOnGoal : BaseRemoteAction, BasicGoalIface {

	/** The object's particle system. */
	private Particles particles;

	void Start() {
		this.particles = this.GetComponent<Particles>();
		if (this.particles == null) {
			throw new System.Exception($"{this} requires a particles system!");
		}

		this.particles.Stop(true);
	}

	public void OnGoal() {
		this.particles.Play(true);
	}
}
