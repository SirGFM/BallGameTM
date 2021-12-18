using SceneMng = UnityEngine.SceneManagement.SceneManager;
using SceneMode = UnityEngine.SceneManagement.LoadSceneMode;

/**
 * CalibrateInput forcefully reset every axis calibration and re-calibrate
 * the inputs, jumping to the defined scene afterwards.
 *
 * Note that as soon as a scene starts, the inputs will be in a weird
 * state. To work around that, this script waits until an input is pressed
 * and then released to properly train the axes.
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

		while (!Input.CheckAnyKeyDown()) {
			yield return null;
		}

		while (!Input.TrainAxisStable(true) || Input.CheckAnyKeyDown() ||
				trainNum > 0) {
			yield return null;

			if (trainNum > 0) {
				trainNum--;
			}
		}

		yield return null;

		Input.RevertMap(0);
		Input.RevertMap(1);
		Input.RevertMap(2);

		SceneMng.LoadSceneAsync(this.NextScene, SceneMode.Single);
	}
}
