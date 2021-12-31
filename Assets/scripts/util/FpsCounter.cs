using EvSys = UnityEngine.EventSystems;
using Time = UnityEngine.Time;

/**
 * FpsCounter accumulates the number of physics updates (executed with a
 * fixed time step), logic updates (executed with a variable time step)
 * and drawn frames every second.
 *
 * It reports the accumulated value as a Report() event. An object must
 * implement the FpsCounterIface interface to handle it.
 */

public interface FpsCounterIface : EvSys.IEventSystemHandler {
	/**
	 * Report the current frames per second.
	 *
	 * @param fps: The number of fixed updates in the last second
	 * @param ups: The number of logic updates in the last second
	 * @param dps: The number of drawn frames in the last second
	 */
	void Report(int fps, int ups, int dps);
}

public class FpsCounter : BaseRemoteAction {
	/** Number of physics update since the last second. */
	private int fps;

	/** Number of updates since the last second. */
	private int ups;

	/** Number of drawn frames since the last second. */
	private int dps;

	void Start() {
		this.fps = 0;
		this.ups = 0;
		this.dps = 0;

		this.StartCoroutine(this.countFps());
		this.StartCoroutine(this.countDps());
		this.StartCoroutine(this.updateFps());
	}

	/** Count each fixed update into 'this.ups'. */
	private System.Collections.IEnumerator countFps() {
		while (true) {
			yield return new UnityEngine.WaitForFixedUpdate();
			this.fps++;
		}
	}

	/** Count each fixed frame into 'this.dps'. */
	private System.Collections.IEnumerator countDps() {
		while (true) {
			yield return new UnityEngine.WaitForEndOfFrame();
			this.dps++;
		}
	}

	/** Report the number of frames accumulated since the last second. */
	private System.Collections.IEnumerator updateFps() {
		while (true) {
			for (float t = 0; t < 1.0; t += Time.deltaTime) {
				yield return null;
				this.ups++;
			}

			this.issueEvent<FpsCounterIface>(
				(x,y) => x.Report(this.fps, this.ups, this.dps));
			this.fps = 0;
			this.ups = 0;
			this.dps = 0;
		}
	}
}
