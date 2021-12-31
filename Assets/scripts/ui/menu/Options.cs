using QualitySettings = UnityEngine.QualitySettings;
using ResMode = UnityEngine.Resolution;
using Screen = UnityEngine.Screen;
using UiText = UnityEngine.UI.Text;
using UiTransform = UnityEngine.RectTransform;
using Vec2 = UnityEngine.Vector2;
using Lambda = System.Action<int>;

public class Options : VerticalTextMenu {
	public UiTransform Content = null;

	public UiText titles;
	public UiText help;
	public UiText valueShadow;
	public UiText valueUnselected;
	public UiText valueSelected;

	/**
	 * List of values that a given option (say, " volume") has.
	 *
	 * When a new value is select, by calling `onLeft()` and `onRight()`,
	 * it may automatically take effect by configuring the `onMove` lambda.
	 */
	private class Values {
		private string[] values;
		private int idx;
		private Lambda onMove;
		private Lambda onSelect;

		/** Initialize a horizontal list from a vararg-like argument */
		public Values(Lambda onMove, params string[] list) {
			this.values = new string[list.Length];
			for (int i = 0; i < list.Length; i++)
				this.values[i] = list[i];
			this.idx = 0;
			this.onMove = onMove;
		}

		public Values setAt(int initialIdx) {
			this.idx = initialIdx;
			return this;
		}

		public string getText() {
			if (this.idx == 0 && this.values.Length > 1)
				return $"{this.values[this.idx]} >";
			else if (this.idx > 0 && this.idx == this.values.Length - 1)
				return $"< {this.values[this.idx]}";
			else if (this.idx < this.values.Length)
				return $"< {this.values[this.idx]} >";
			else
				return "???";
		}

		public void onLeft() {
			this.idx--;
			if (this.idx < 0)
				this.idx = this.values.Length - 1;
			if (this.onMove != null)
				this.onMove(this.idx);
		}

		public void onRight() {
			this.idx++;
			if (this.idx >= this.values.Length)
				this.idx = 0;
			if (this.onMove != null)
				this.onMove(this.idx);
		}

		public int getIdx() {
			return this.idx;
		}
	};

	/**
	 * An option in the menu, that may or may not have a list of
	 * selectable values.
	 *
	 * Creating an option from `SectionHeader()` instances a custom
	 * option that doesn't have an associated description and is marked
	 * as a "header" (checked from its `isHeader()`).
	 */
	private class Option {
		protected bool _isHeader;
		private string title;
		private string desc;
		private Values val;

		public Option(string title, string desc, Values v) {
			this._isHeader = false;
			this.title = title;
			this.desc = desc;
			this.val = v;
		}

		static public Option SectionHeader(string title) {
			Option o = new Option(title, "", null);
			o._isHeader = true;
			return o;
		}

		public bool isHeader() {
			return this._isHeader;
		}

		public string getHeader() {
			if (this._isHeader)
				return this.title;
			else
				return "";
		}

		public string getTitle() {
			return this.title;
		}

		public void setOptions(Values val) {
			this.val = val;
		}

		public string getOption() {
			if (this.val != null)
				return this.val.getText();
			else
				return "";
		}

		public string getDescription() {
			return this.desc;
		}

		public int getValue() {
			if (this.val != null)
				return this.val.getIdx();
			else
				return -1;
		}

		public void onLeft() {
			if (this.val != null)
				this.val.onLeft();
		}

		public void onRight() {
			if (this.val != null)
				this.val.onRight();
		}
	}

	/** Whether the game is in fullscreen mode. Set from a lambda
	 * (configured in `start()`) */
	private bool isFull = false;

	/** The resolution of the game. Set from a lambda
	 * (configured in `start()`) */
	private int resMode = 0;

	/** List of options in the menu */
	private Option[] opts;

	/** List of resolutions */
	private ResMode[] resolutions;

	/** The graphical quality of the game. Set from a lambda (configured in
	 * `start()`). Note that the list of quality options is retrieved
	 * directly from Unity. */
	private int qualityMode = 0;

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

	/** Called whenever a new option is selected.
	 * Overriden so headers may be skipped. */
	override protected void updateSelected() {
		if (this.opts[this.getCurrentOpt()].isHeader()) {
			if (this.wasDown)
				this.onDown();
			else
				this.onUp();
		}
		else {
			base.updateSelected();
			this.updateSelectedValue();
			this.updateViewport();
		}
	}

	private bool wasDown;
	override protected void onDown() {
		this.wasDown = true;
		base.onDown();
	}

	override protected void onUp() {
		this.wasDown = false;
		base.onUp();
	}

	/** Configure the list of values (the right view) */
	private void updateValues() {
		string txt = "";

		for (int i = 0; i < this.opts.Length; i++) {
			Option o = this.opts[i];
			txt += $"{o.getOption()}\n";
		}

		this.valueShadow.text = txt;
		this.valueUnselected.text = txt;

		this.updateSelectedValue();
	}

	/* Update the currently highlighted value (whithin the option's list) */
	private void updateSelectedValue() {
		string selected = "";

		for (int i = 0; i < this.opts.Length; i++) {
			Option o = this.opts[i];
			if (i == this.getCurrentOpt())
				selected += $"{o.getOption()}\n";
			else
				selected += "\n";
		}

		this.valueSelected.text = selected;

		Option cur = this.opts[this.getCurrentOpt()];
		this.help.text = cur.getDescription();
	}

	override protected void onLeft() {
		this.opts[this.getCurrentOpt()].onLeft();
		this.updateValues();
	}

	override protected void onRight() {
		this.opts[this.getCurrentOpt()].onRight();
		this.updateValues();
	}

	/** Update the game's graphical mode */
	private void updateGraphics() {
		ResMode res = this.resolutions[this.resMode];
		Screen.SetResolution(res.width, res.height, this.isFull, res.refreshRate);
		QualitySettings.SetQualityLevel(this.qualityMode, true);
	}

	private void back() {
		this.LoadScene("scenes/menu/MainMenu");
	}

	/** Called whenever an option is selected */
	override protected void onSelect() {
		string cur = this.opts[this.getCurrentOpt()].getTitle();

		if (cur == "Apply")
			this.updateGraphics();
		else if (cur == "Back")
			this.back();
		else if (cur == "Reset") {
			Input.RevertMap(0);
			Input.RevertMap(1);
			Input.RevertMap(2);
		}
		else if (cur.StartsWith("Input")) {
			switch (cur[cur.Length - 1]) {
				case 'A':
					RebindInput.inputMap = 0;
					break;
				case 'B':
					RebindInput.inputMap = 1;
					break;
				case 'C':
					RebindInput.inputMap = 2;
					break;
				default:
					throw new System.Exception($"Invalid rebindable input '{cur}'");
			}
			this.LoadScene("scenes/menu/RebindInputs");
		}
	}

	override protected void start() {
		/* Create a value with the list of resolution modes. First, check
		 * how many 16x9 resolutions there are and then create the actual
		 * list. */
		ResMode[] tmp = new ResMode[Screen.resolutions.Length];
		int _16x9Count = 0;
		for (int i = 0; i < Screen.resolutions.Length; i++) {
			ResMode mode = Screen.resolutions[i];

			if (mode.height % 9 == 0 && mode.width % 16 == 0 &&
					mode.height / 9 == mode.width / 16) {
				tmp[_16x9Count] = mode;
				_16x9Count++;
			}
		}

		this.resolutions = new ResMode[_16x9Count];
		string[] resList = new string[_16x9Count];
		this.resMode = 0;
		for (int i = 0; i < _16x9Count; i++) {
			ResMode mode = tmp[i];

			/* Load the current resolution. */
			if (Screen.height == mode.height && Screen.width == mode.width) {
				this.resMode = i;
			}

			this.resolutions[i] = mode;
			resList[i] = $"{mode.width}x{mode.height}@{mode.refreshRate}";
		}

		/* Load the current quality settings */
		this.qualityMode = QualitySettings.GetQualityLevel();

		this.isFull = Screen.fullScreen;

		string[] audioModes = new string[11];
		for (int i = 1; i < audioModes.Length; i++) {
			audioModes[i] = $"{i*10}%";
		}
		audioModes[0] = "Off";
		float audioRatio = (float)audioModes.Length - 1.0f;

		Option[] _opts = {
			Option.SectionHeader("-- Graphics --"),
			new Option("Quality",
					"Set the game's overall graphical quality.\n"+
					"Only takes effect on \"Apply\"!",
					(new Values(idx => this.qualityMode = idx,
								QualitySettings.names)).setAt(this.qualityMode)),
			new Option("Resolution",
					"Set the game's resolution.\n"+
					"Only takes effect on \"Apply\"!",
					(new Values(idx => this.resMode = idx,
								resList)).setAt(this.resMode)),
			new Option("Fullscreen",
					"Choose windowed or fullscreen mode.\n"+
					"Only takes effect on \"Apply\"!",
					(new Values(idx => this.isFull = (idx == 1),
								"Windowed",
								"Fullscreen")).setAt(this.isFull ? 1 : 0)),
			new Option("Apply",
					"Apply the selected resolution and\n"+
					"windowed mode.",
					null),
			Option.SectionHeader("-- General --"),
			new Option("Camera X",
					"Configure horizontal camera movement.\n"+
					"Try it out!",
					(new Values(idx => Config.setHorCamInverted(idx == 1),
								"Normal",
								"Inverted")).setAt((Config.getHorCamInverted()) ? 1 : 0)),
			new Option("Camera Y",
					"Configure vertical camera movement.\n"+
					"Try it out!",
					(new Values(idx => Config.setVerCamInverted(idx == 1),
								"Normal",
								"Inverted")).setAt((Config.getVerCamInverted()) ? 1 : 0)),
			Option.SectionHeader("-- Rebind --"),
			new Option("Reset",
					"Reset input bindings to their initial configurations.",
					null),
			new Option("Input A",
					"Configure 1 of 3 simultaneous control schemes. (Default: Keyboard)",
					null),
			new Option("Input B",
					"Configure 1 of 3 simultaneous control schemes. (Default: Gamepad)",
					null),
			new Option("Input C",
					"Configure 1 of 3 simultaneous control schemes. (Default: Empty)",
					null),

			Option.SectionHeader("--"),
			new Option("Back",
					"Go back to the previous menu.",
					null),
		};
		this.opts = _opts;

		/* Create the list of options (left view) */
		string highlighText = "";
		this.options = new string[this.opts.Length];

		for (int i = 0; i < this.opts.Length; i++) {
			Option o = this.opts[i];
			highlighText += $"{o.getHeader()}\n";
			this.options[i] = o.getTitle();
		}

		this.titles.text = highlighText;

		this.CombinedLoadScene("scenes/bg-scene/TestPerformance");
		base.start();
		/* Forcefully starts from index 1 */
		this.onDown();

		this.updateValues();
	}

	void Update() {
		if (Input.MenuCancel()) {
			this.back();
		}
	}
}
