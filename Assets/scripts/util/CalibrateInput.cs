using SceneMng = UnityEngine.SceneManagement.SceneManager;
using SceneMode = UnityEngine.SceneManagement.LoadSceneMode;

/**
 * CalibrateInput forcefully reset every axis calibration and re-calibrate
 * the inputs, jumping to the defined scene afterwards.
 */

public class CalibrateInput : UnityEngine.MonoBehaviour {
	/** Minimum number of frames that the calibration runs. */
	public uint IterCount = 5;

	/** The scene to be loaded as soon as the calibration ends. */
	public string NextScene = "Loader";

	void Start() {
		Input.ResetAxisTraining();
		this.StartCoroutine(this.Calibrate());
	}

	private System.Collections.IEnumerator Calibrate() {
		uint trainNum = this.IterCount;

		while (!Input.TrainAxisStable() || Input.CheckAnyKeyDown() ||
				trainNum > 0) {
			yield return null;

			if (trainNum > 0) {
				trainNum--;
			}
		}

		yield return null;

        SceneMng.LoadSceneAsync("Loader", SceneMode.Single);
	}
}
