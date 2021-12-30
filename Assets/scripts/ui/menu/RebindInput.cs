using Color = UnityEngine.Color;
using CoroutineRet = System.Collections.IEnumerator;
using Lambda = System.Action;
using UiViewport = UnityEngine.UI.Image;
using UiText = UnityEngine.UI.Text;
using UiTransform = UnityEngine.RectTransform;
using Vec2 = UnityEngine.Vector2;

/**
 * RebindInput scene.
 *
 * 'inputMap' must be set before changing to this scene so it may configure
 * each of the input sets.
 */

public class RebindInput : VerticalTextMenu {
	static public int inputMap = 0;

	public UiTransform Content = null;
	public UiText valueShadow;
	public UiText valueUnselected;
	public UiText valueSelected;
	public UiText topics;
	public UiText description;
	public UiViewport viewport;

	private int _ignoreInputs;

	private interface Value {
		string getText();
		string getHeader();
		string getValue();
		string getDesc();
		void onSelect();
	};

	private Value[] vals;

	private class Header: Value {
		private string text;

		public Header(string text) {
			this.text = text;
		}

		public string getText() {
			return "";
		}

		public string getHeader() {
			return this.text;
		}

		public string getValue() {
			return "";
		}

		public string getDesc() {
			return "";
		}

		public void onSelect() {
		}
	}

	private class Command: Value {
		private string text;
		private Lambda _onSelect;
		private string desc;

		public Command(string text, Lambda _onSelect, string desc) {
			this.text = text;
			this._onSelect = _onSelect;
			this.desc = desc;
		}

		public string getText() {
			return this.text;
		}

		public string getHeader() {
			return "";
		}

		public string getValue() {
			return "";
		}

		public string getDesc() {
			return this.desc;
		}

		public void onSelect() {
			this._onSelect();
		}
	}

	private CoroutineRet WaitNoInput() {
		uint trainNum = 3;
		while (!Input.TrainAxisStable() || Input.CheckAnyKeyDown() || trainNum > 0) {
			yield return null;
			if (trainNum > 0) {
				trainNum--;
			}
		}
		yield return null;
	}

	private CoroutineRet WaitInput(Action act) {
		bool done = false;
		this._ignoreInputs++;

		/* Gotta wait until the 'Input.Actions' that started this event
		 * is over before the new press can be polled. */
		act.setTimeoutText($"Waiting input...");
		this.updateSelected();
		yield return this.WaitNoInput();
		Input.WaitInput(this.gameObject, RebindInput.inputMap, act.getAction());

		for (int time = 5; !done && time >= 0; time--) {
			act.setTimeoutText($"Waiting input... {time}...");
			this.updateSelected();

			for (int i = 0; !done && i < 80; i++) {
				yield return new UnityEngine.WaitForSeconds(0.01f);

				done = !Input.IsWaitingInput();
			}
		}

		if (!done) {
			Input.CancelWaitInput();
			Input.ClearAxis(act.getAction(), RebindInput.inputMap);
		}
		act.setTimeoutText("");
		this.updateSelected();

		this._ignoreInputs--;
	}

	private class Action : Value {
		private Input.Actions action;
		private RebindInput self;
		private string description;
		private string timeoutText;

		public Action(RebindInput self, Input.Actions action,
				string description) {
			this.action = action;
			this.self = self;
			this.description = description;
			this.timeoutText = "";
		}

		public string getText() {
			string act = this.action.ToString();
			if (act.Length < 8 || !act.Contains("Camera"))
				return act;
			else if (act.StartsWith("Camera"))
				return act.Substring(6);
			else if (act == "MouseCamera")
				return "Use Mouse";
			else
				return act.Substring(0, act.Length - 6)+"Cam.";
		}

		public string getHeader() {
			return "";
		}

		public string getValue() {
			if (this.timeoutText.Length == 0)
				return Input.AxisName(this.action, RebindInput.inputMap);
			else
				return this.timeoutText;
		}

		public string getDesc() {
			return this.description;
		}

		public void onSelect() {
			this.self.StartCoroutine(this.self.WaitInput(this));
		}

		public void setTimeoutText(string txt) {
			this.timeoutText = txt;
		}

		public Input.Actions getAction() {
			return this.action;
		}
	};

	private bool isValHeader(int idx) {
		return (this.vals[idx] is Header _);
	}

	override protected bool ignoreInputs() {
		return this._ignoreInputs > 0;
	}

	override protected void onDown() {
		base.onDown();
		if (this.isValHeader(this.getCurrentOpt()))
			this.onDown();
	}

	override protected void onUp() {
		base.onUp();
		if (this.isValHeader(this.getCurrentOpt()))
			this.onUp();
	}

	/** Called whenever an option is selected (i.e., accept is pressed) */
	override protected void onSelect() {
		this.vals[this.getCurrentOpt()].onSelect();
	}

	/** Update the viewport, so the currently select item is always visible */
	private void updateViewport() {
		UiTransform parent;
		parent = this.Content.parent.GetComponent<UiTransform>();

		const float labelHeight = 21f;
		int parentRows = (int)(parent.rect.height / labelHeight);
		int currentRow = (int)(this.Content.anchoredPosition.y / labelHeight);

		float y = labelHeight * this.getCurrentOpt();
		if (y < this.Content.anchoredPosition.y) {
			Vec2 pos;
			if (y == labelHeight)
				y = 0.0f;
			pos = new Vec2(Content.anchoredPosition.x, y);
			Content.anchoredPosition = pos;
		}
		else if (this.getCurrentOpt() - currentRow > parentRows - 1) {
			y = (this.getCurrentOpt() + 1 - parentRows) * labelHeight;
			Vec2 pos;
			pos = new Vec2(Content.anchoredPosition.x, y);
			Content.anchoredPosition = pos;
		}
	}

	/** Called whenever a new option is highlighted. */
	override protected void updateSelected() {
		base.updateSelected();

		string values = "";
		string selected = "";
		for (int i = 0; i < this.vals.Length; i++) {
			string val = this.vals[i].getValue();
			values += $"{val}\n";
			if (i == this.getCurrentOpt())
				selected  += $"{val}\n";
			else
				selected  += "\n";
		}

		this.valueShadow.text = values;
		this.valueUnselected.text = values;
		this.valueSelected.text = selected;
		this.description.text = this.vals[this.getCurrentOpt()].getDesc();

		this.updateViewport();
	}

	private CoroutineRet SetAll() {
		this._ignoreInputs++;

		int idx = this.getCurrentOpt();

		do {
			this.onDown();
			if (this.vals[this.getCurrentOpt()] is Action act) {
				yield return this.WaitInput(act);
				yield return this.WaitNoInput();
			}
		} while (idx != this.getCurrentOpt());

		this._ignoreInputs--;
	}

	private void saveInputs() {
		// TODO save the inputs
	}

	private void back() {
		this.saveInputs();
		// TODO Load the options menu
	}

	private CoroutineRet handleBack() {
		while (true) {
			while (this.ignoreInputs())
				yield return new UnityEngine.WaitForSeconds(0.3f);

			yield return null;
			if (Input.MenuCancel())
				this.back();
		}
	}

	/**
	 * Waits until Pause is pressed, and then released, to re-enable menu
	 * inputs.
	 */
	private CoroutineRet testInput() {
		while (!Input.GetPauseJustPressed()) {
			yield return null;
		}
		while (Input.GetPauseDown()) {
			yield return null;
		}

		this._ignoreInputs = 0;
		this.viewport.color = Color.white;
	}

	override protected void start() {
		Value[] _vals = {
			new Command("Set All",
					() => this.StartCoroutine(this.SetAll()),
					"Automatically configures every input"),
			new Header("-- Character --"),
			new Header("--  & Menu --"),
			new Action(this,
					Input.Actions.Left,
					"Press input to move the character left"),
			new Action(this,
					Input.Actions.Right,
					"Press input to move the character right"),
			new Action(this,
					Input.Actions.Up,
					"Press input to move the character up"),
			new Action(this,
					Input.Actions.Down,
					"Press input to move the character down"),
			new Action(this,
					Input.Actions.Action,
					"Press input to jump/accept"),
			new Header("-- Game --"),
			new Action(this,
					Input.Actions.Reset,
					"Press input to reset to the start of the level"),
			new Action(this,
					Input.Actions.Pause,
					"Press input to pause the game"),
			new Header("-- Camera --"),
			new Action(this,
					Input.Actions.MouseCamera,
					"Press input to enable mouse-based camera control"),
			new Action(this,
					Input.Actions.CameraLeft,
					"Press input to move the camera left"),
			new Action(this,
					Input.Actions.CameraRight,
					"Press input to move the camera right"),
			new Action(this,
					Input.Actions.CameraUp,
					"Press input to move the camera up"),
			new Action(this,
					Input.Actions.CameraDown,
					"Press input to move the camera down"),
			new Header("--"),
			new Command("Test",
					() => {
					this.viewport.color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
					this._ignoreInputs = 1;
					this.StartCoroutine(this.testInput());
					},
					"Test the inputs. Press PAUSE to quit."),
			new Header("--"),
			new Command("Revert",
					() => {
					Input.RevertMap(RebindInput.inputMap);
					this.saveInputs();
					this.updateSelected();
					},
					"Reverts this scheme to its default configuration"),
			new Command("Back",
					() => this.back(),
					"Go back to the Options menu"),
		};
		this.vals = _vals;

		/* Create the list of options (left view) */
		this.options = new string[this.vals.Length];
		string topicsTxt = "";
		for (int i = 0; i < this.vals.Length; i++) {
			Value val = this.vals[i];
			this.options[i] = val.getText();
			topicsTxt += $"{val.getHeader()}\n";
		}

		this._ignoreInputs = 0;

		this.CombinedLoadScene("scenes/bg-scene/TestInputs");
		base.start();
		this.topics.text = topicsTxt;

		this.StartCoroutine(this.handleBack());
	}
}
