using App = UnityEngine.Application;
using Color = UnityEngine.Color;
using CoroutineRet = System.Collections.IEnumerator;
using Image = UnityEngine.UI.Image;

public class MainMenu : VerticalTextMenu {
	/** The error text container, to be enabled if loading failed. */
	public UnityEngine.GameObject ErrorText;

	private string[] _opts = {
		"New game",
		"Level Select",
		"Customize PLayer",
		"Options",
		"Quit"
	};

	override protected void onSelect() {
		switch (this.getCurrentOpt()) {
		case 0:
			this.LoadLevel(1);
			break;
		case 1:
			this.LoadScene("scenes/menu/LevelSelect");
			break;
		case 2:
			this.LoadScene("scenes/menu/CustomizePlayer");
			break;
		case 3:
			this.LoadScene("scenes/menu/Options");
			break;
		case 4:
			App.Quit();
			break;
		}
	}

	override protected void start() {
		this.options = this._opts;
		this.CombinedLoadScene("scenes/bg-scene/MainMenuBg");
		base.start();

		if (Loader.FailedToLoad() && this.ErrorText != null) {
			this.ErrorText.SetActive(true);
		}

		/* Do not hide the mouse in the WebGL build
		 * so the player may press the 'reset config' button. */
#if !UNITY_WEBGL
		this.StartCoroutine(this.hideMouse());
#endif
	}

	override protected void onCancel() {
		UnityEngine.Cursor.lockState = UnityEngine.CursorLockMode.None;
		base.onCancel();
	}

	/** Hide the mouse cursor on press. */
	private CoroutineRet hideMouse() {
		while (true) {
			if (UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Mouse0)) {
				UnityEngine.Cursor.lockState = UnityEngine.CursorLockMode.Locked;
			}
			yield return null;
		}
	}
}
