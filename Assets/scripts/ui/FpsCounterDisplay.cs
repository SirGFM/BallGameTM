using UiText = UnityEngine.UI.Text;

/**
 * FpsCounterDisplay handles FpsCounterIface events by displaying the
 * current FPS to Text elements.
 */

public class FpsCounterDisplay : UnityEngine.MonoBehaviour, FpsCounterIface {

	/** Display the number of physics update since the last second. */
	public UiText fps;

	/** Display the number of updates since the last second. */
	public UiText ups;

	/** Display the number of drawn frames since the last second. */
	public UiText dps;

	public void Report(int fps, int ups, int dps) {
		if (this.fps != null) {
			this.fps.text = $"Fixed: {fps}/s";
		}
		if (this.ups != null) {
			this.ups.text = $"Update: {ups}/s";
		}
		if (this.dps != null) {
			this.dps.text = $"Draw: {dps}/s";
		}
	}
}
