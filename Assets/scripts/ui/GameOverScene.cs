using Color = UnityEngine.Color;
using Image = UnityEngine.UI.RawImage;
using SceneMng = UnityEngine.SceneManagement.SceneManager;
using SceneMode = UnityEngine.SceneManagement.LoadSceneMode;
using Time = UnityEngine.Time;
using UiText = UnityEngine.UI.Text;

/**
 * GameOverScene controls the game over scene, eventually going back to the main
 * menu.
 */

public class GameOverScene : UnityEngine.MonoBehaviour, LoaderIface {

	/** Object used to fade everything in and out. */
	public Image fade;

	/** Text flashed when waiting for further inputs. */
	public UiText pressToContinue;

	/** How long the fade animation takes. */
	public float fadeDuration = 2.0f;

	/** How long the fade animation for requesting input takes. */
	public float inputFadeDuration = 2.0f;

	/** The scene loaded with the background objects. */
	public string backgroundScene = "scenes/bg-scene/MainMenuBg";

	/** The main menu scene. */
	public string mainMenuScene = "MainMenu";

	void Start() {
		Global.rtaTimer.Stop();
		BaseRemoteAction.setRootTarget(this.gameObject);

		this.pressToContinue.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
		SceneMng.LoadSceneAsync(this.backgroundScene, SceneMode.Additive);
		this.StartCoroutine(this.run());
	}

	/** Wait for any input. */
	private System.Collections.IEnumerator waitInput() {
		while (true) {
			float dt;
			float _inputDelay = this.inputFadeDuration / 2.0f;

			/* Fade the text in. */
			for (dt = 0.0f; dt < _inputDelay; dt += Time.deltaTime) {
				if (Input.CheckAnyKeyJustPressed()) {
					yield break;
				}

				yield return null;
				float alpha = dt / _inputDelay;
				this.pressToContinue.color = new Color(1.0f, 1.0f, 1.0f, alpha);
			}

			/* Fade the text out. */
			for (; dt > 0.0f; dt -= Time.deltaTime) {
				if (Input.CheckAnyKeyJustPressed()) {
					yield break;
				}

				yield return null;
				float alpha = dt / _inputDelay;
				this.pressToContinue.color = new Color(1.0f, 1.0f, 1.0f, alpha);
			}
		}
	}

	/** Executes the animation. */
	private System.Collections.IEnumerator run() {
		float dt;
		Color c = this.fade.color;

		/* Fade the scene in. */
		for (dt = 0.0f; dt < this.fadeDuration; dt += Time.deltaTime) {
			float alpha = (this.fadeDuration - dt) / this.fadeDuration;
			this.fade.color = new Color(c.r, c.b, c.g, alpha);
			yield return null;
		}
		this.fade.color = new Color(c.r, c.b, c.g, 0.0f);

		/* Run indefinitely waiting for input, flashing a message. */
		yield return this.waitInput();

		/* Fade the scene out. */
		for (dt = 0.0f; dt < this.fadeDuration; dt += Time.deltaTime) {
			float alpha = dt / this.fadeDuration;
			this.fade.color = new Color(c.r, c.b, c.g, alpha);
			yield return null;
		}
		this.fade.color = new Color(c.r, c.b, c.g, 1.0f);

		SceneMng.LoadSceneAsync(this.mainMenuScene, SceneMode.Single);
	}

	public void SetTimer(UiText txt, UiTimer timer) {
		switch (timer) {
		case UiTimer.IgtTimer:
			txt.text = Global.igtTimer.ToString();
			break;
		case UiTimer.LevelTimer:
			txt.text = Global.levelTimer.ToString();
			break;
		case UiTimer.RtaTimer:
			txt.text = Global.rtaTimer.ToString();
			break;
		default:
			throw new System.Exception($"Invalid UiTimer ({timer})");
		}
	}

	public void SetProgressBar(out bool done, ProgressBar pb) {
		/** Dummy. Does nothing. */
		done = false;
	}

	public void SetSceneTitle(out bool done, UiText txt, UiScene ui) {
		/** Dummy. Does nothing. */
		done = false;
	}

	public void OnReset() {
		/** Dummy. Does nothing. */
	}

	public void ShowPause() {
		/** Dummy. Does nothing. */
	}

	public void HidePause() {
		/** Dummy. Does nothing. */
	}
}
