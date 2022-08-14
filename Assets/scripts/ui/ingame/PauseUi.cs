using CoroutineRet = System.Collections.IEnumerator;
using RectT = UnityEngine.RectTransform;
using SceneMng = UnityEngine.SceneManagement.SceneManager;
using SceneMode = UnityEngine.SceneManagement.LoadSceneMode;
using UiText = UnityEngine.UI.Text;
using Vec3 = UnityEngine.Vector3;

public class PauseUi : BaseRemoteAction {

	/** The reset text, updated on load with the reset button. */
	public UiText quit;

	/** The reset text, updated on load with the reset button. */
	public UiText reset;

	/** The container for the quitting text. */
	public RectT quiting;

	/** The main menu scene. */
	public string mainMenuScene = "scenes/menu/MainMenu";

	/**
	 * Format the text, using richtext, as bold.
	 *
	 * @param str: The text to be formatted.
	 */
	private string bold(string str) {
		return $"<b>{str}</b>";
	}

	/**
	 * Color the text, using richtext, light blue.
	 *
	 * @param str: The text to be formatted.
	 */
	private string lightBlue(string str) {
		return $"<color=#cbdbfcff>{str}</color>";
	}

	/**
	 * Retrieve the name of every button accepted for the requested action.
	 *
	 * @param act: The action.
	 */
	private string getActionButtons(Input.Actions act) {
		string str = "";

		for (int i = 0; i < 3; i++) {
			string tmp = Input.AxisName(act, i);
			if (tmp.Length > 0)
				str += $" or {bold(lightBlue(tmp))}";
		}

		if (str.Length > 4)
			str = str.Substring(4);
		return str;
	}

	/**
	 * Monitor the state of the quit button, to decide on going back to the
	 * main menu or to close this view.
	 */
	private CoroutineRet waitAction() {
		string str = "Quiting in ";
		UiText text = this.quiting.GetComponentInChildren<UiText>();

		/* Reveal the quit warning by scaling it up vertically. */
		text.text = "";
		for (int i = 0; i < 11 && Input.GetPauseDown(); i++) {
			quiting.localScale = new Vec3(1.0f, 0.1f * (float)i, 1.0f);
			yield return new UnityEngine.WaitForSeconds(0.01f);
			continue;
		}

		/* Show the main text, one character at a time. */
		for (int i = 0; i < str.Length && Input.GetPauseDown(); i++) {
			text.text = str.Substring(0, i);
			yield return new UnityEngine.WaitForSeconds(0.025f);
			continue;
		}

		/* Update only the number counting down, until it reaches zero. */
		for (int j = 3; j >= 0 && Input.GetPauseDown(); j--) {
			for (int i = 0; i < 5 && Input.GetPauseDown(); i++) {
				switch (i) {
				case 0:
					text.text = str + $"{j}";
					break;
				case 1:
				case 2:
				case 3:
					text.text += ".";
					break;
				}
				yield return new UnityEngine.WaitForSeconds(0.15f);
				continue;
			}
		}

		/* If at any point the button was released, simply close this scene.
		 * Otherwise, go back to the main menu. */
		if (Input.GetPauseDown()) {
			SceneMng.LoadSceneAsync(this.mainMenuScene, SceneMode.Single);
		}
		else {
			rootEvent<LoaderIface>( (x,y) => x.HidePause() );
		}
	}

	void Update() {
		if (Input.GetPauseJustPressed()) {
			this.StartCoroutine(this.waitAction());
		}
	}

	void Start() {
		this.quiting.localScale = new Vec3(1.0f, 0, 1.0f);
		this.quit.text = $"Hold {this.getActionButtons(Input.Actions.Pause)} to quit to the Main Menu.\nTap to close this menu.";
		this.reset.text = $"Press {this.getActionButtons(Input.Actions.Reset)} to restart.";
	}
}
