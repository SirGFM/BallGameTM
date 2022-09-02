using App = UnityEngine.Application;
using UiText = UnityEngine.UI.Text;

public class SetVersion : UnityEngine.MonoBehaviour {
	void Start() {
		UiText ui = this.GetComponentInChildren<UiText>();
		ui.text = $"Current Version: {App.version}";
	}
}
