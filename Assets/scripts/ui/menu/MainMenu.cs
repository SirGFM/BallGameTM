using App = UnityEngine.Application;
using Color = UnityEngine.Color;
using Image = UnityEngine.UI.Image;

public class MainMenu : VerticalTextMenu {
	private string[] _opts = {
		"New game",
		"Options",
		"Quit"
	};

	override protected void onSelect() {
		switch (this.getCurrentOpt()) {
		case 0:
			this.LoadLevel(1);
			break;
		case 1:
			this.LoadScene("scenes/menu/Options");
			break;
		case 2:
			App.Quit();
			break;
		}
	}

	override protected void start() {
		this.options = this._opts;
		this.CombinedLoadScene("scenes/bg-scene/MainMenuBg");
		base.start();
	}
}
