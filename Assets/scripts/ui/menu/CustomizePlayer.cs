using Color = UnityEngine.Color;
using GO = UnityEngine.GameObject;
using Lambda = System.Action;
using Material = UnityEngine.Material;
using UiText = UnityEngine.UI.Text;
using UiTransform = UnityEngine.RectTransform;
using Image = UnityEngine.UI.Image;
using RawImage = UnityEngine.UI.RawImage;
using Vec2 = UnityEngine.Vector2;
using Vec3 = UnityEngine.Vector3;

public class CustomizePlayer : Menu {
	/** The player prefab, with every available model and material/color. */
	public GO Ball;

	/** UI container where each selectable color is placed. */
	public UiTransform Palette;

	/** Hack for having a 2D array in Unity Editor. */
	[System.Serializable]
	public struct MaterialList {
		public Material[] Colors;
	}

	/** A 2D list of colors, with each entry being a row of colors. */
	public MaterialList[] SortedColors;

	/** Maps each index in this list of colors to the index the PlayerModel. */
	private int[] idxToColor;

	/** The dimension of a single color square in the palette. */
	public float colorDimension = 10.0f;

	/** The shadow of the menu entries. */
	public UiText shadow;

	/** The dimmed version of the menu entries, for when it's not selected. */
	public UiText unselected;

	/** The highlighted, currently selected menu entry. */
	public UiText selected;

	/** The actual component with the player models and materials/colors. */
	private PlayerModel model;

	/** The list of menu options. */
	private enum Options {
		Model = 0,
		BaseColor,
		DetailColor,
		SubDetailColor,
		Back,
		NumOpts,
	}

	/** The currently selected menu option. */
	private Options curOpt;

	/** Whether the editor is in "color selection mode" (true) or
	 * "menu navigation mode" (false). */
	private bool onPalette;

	/** The currently selected player model. */
	private int playerModel;

	/** The horizontal index of the currently selected color. */
	private int colorX;

	/** The vertical index of the currently selected color. */
	private int colorY;

	/** The initial color when "color selection mode" was enabled. */
	private int originalColor;

	/** The UI component used to show the currently selected color. */
	private UiTransform selector;

	/** @return The displayed name for a given menu options. */
	private string optionToText(Options opt) {
		switch (opt) {
		case Options.Model:
			return "Change Model";
		case Options.BaseColor:
			return "Base Color";
		case Options.DetailColor:
			return "Main Details";
		case Options.SubDetailColor:
			return "Minor Details";
		case Options.Back:
			return "Back";
		}
		return "";
	}

	/**
	 * Configure a UI text element with every menu option.
	 *
	 * @param obj: The element to be filled.
	 */
	private void fillText(UiText obj) {
		string text = "";

		for (Options i = 0; i < Options.NumOpts; i++) {
			text += this.optionToText(i) + "\n";
		}

		obj.text = text;
	}

	/** Highlight the currently selected menu entry. */
	private void updateSelect() {
		string prefix = "";
		string suffix = "";
		string selectedText = new string('\n', (int)this.curOpt);

		if (this.curOpt == Options.Model) {
			prefix = "< ";
			suffix = " >";
		}
		else if (!this.onPalette) {
			prefix = "-- ";
			suffix = " --";
		}
		selectedText += prefix + optionToText(this.curOpt) + suffix;
		this.selected.text = selectedText;
	}

	/**
	 * Convert a color index to its actual pixel position.
	 *
	 * @param x: The horizontal index of the color.
	 * @param y: The vertical index of the color.
	 * @return The 2D, top-left pixel position of the color.
	 */
	private Vec2 Idx2dToPos(int x, int y) {
		int numCols = this.SortedColors.Length - 1;
		float posX = x * this.colorDimension;
		float posY = (numCols - y) * this.colorDimension;

		return new Vec2(posX, posY);
	}

	/**
	 * Create a new color tile.
	 *
	 * @param parent: The parent container for the tile.
	 * @param color: The color of the tile.
	 * @param pos: The top-left position of the tile within the parent.
	 * @param size: The dimensions of the tile.
	 * @return The color's UI rectangle.
	 */
	private UiTransform newColorTile(UiTransform parent, Color color, Vec2 pos, float size) {
		GO obj = new GO();

		Image tile = obj.AddComponent<Image>();
		tile.color = color;

		UiTransform rect = tile.rectTransform;
		rect.SetParent(parent);
		rect.anchorMin = new Vec2(0.0f, 0.0f);
		rect.anchorMax = new Vec2(0.0f, 0.0f);
		rect.pivot =  new Vec2(0.0f, 0.0f);
		rect.localScale = new Vec3(1.0f, 1.0f, 1.0f);

		rect.offsetMin = pos;

		pos.x += size;
		pos.y += size;
		rect.offsetMax = pos;

		return rect;
	}

	override protected void start() {
		this.model = this.Ball.GetComponentInChildren<PlayerModel>();
		this.playerModel = PlayerModel.Model;

		this.fillText(this.shadow);
		this.fillText(this.unselected);
		this.updateSelect();

		/* Populate the palette UI,
		 * mapping each color to its index in the player model array. */
		int i = 0;
		this.idxToColor = new int[this.model.Materials.Length];
		for (int y = 0; y < this.SortedColors.Length; y++) {
			Material[] line = this.SortedColors[y].Colors;

			for (int x = 0; x < line.Length; x++) {
				Material want = line[x];
				int playerIdx = 0;

				foreach (Material got in this.model.Materials) {
					if (want == got) {
						break;
					}

					playerIdx++;
				}
				if (playerIdx >= this.model.Materials.Length) {
					throw new System.Exception($"Invalid material ({want})");
				}

				this.idxToColor[i++] = playerIdx;

				this.newColorTile(this.Palette, want.color, Idx2dToPos(x, y), this.colorDimension);
			}
		}

		/* Create the color selector element. */
		Color color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
		float size = this.colorDimension * 0.5f;
		Vec2 pos = new Vec2(size * 0.5f, size * 0.5f);
		this.selector = this.newColorTile(this.Palette, color, pos, size);
		this.selector.gameObject.SetActive(false);

		size *= 0.5f;
		pos -= pos * 0.5f;
		this.newColorTile(this.selector, Color.black, pos, size);
	}

	/**
	 * Handle adjusting the view after a movement. The movement itself must
	 * be implemented within the supplied lambda.
	 *
	 * @param move: The lambda used to select the next menu option.
	 */
	private void onMovement(Lambda move) {
		move();

		if (this.curOpt < 0) {
			this.curOpt = Options.NumOpts - 1;
		}
		else if (this.curOpt == Options.NumOpts) {
			this.curOpt = 0;
		}
		this.updateSelect();

		if (this.playerModel != PlayerModel.Model) {
			if (this.playerModel < 0) {
				this.playerModel = this.model.Models.Length - 1;
			}
			else if (this.playerModel >= this.model.Models.Length) {
				this.playerModel = 0;
			}

			PlayerModel.Model = this.playerModel;
		}
	}

	/** Position the color selector at the currently selected color. */
	private void setSelectorPosition() {
		float height = this.Palette.rect.height;
		float size = this.selector.offsetMax.x - this.selector.offsetMin.x;
		Vec2 pos = new Vec2(size * 0.5f, height);

		pos.x += this.colorX * this.colorDimension;
		pos.y -= (0.75f + this.colorY) * this.colorDimension;
		this.selector.offsetMin = pos;

		pos.x += size;
		pos.y += size;
		this.selector.offsetMax = pos;
	}

	/** Setup "color selection mode". */
	private void startColorSelection() {
		/* Find the position of the color within this component. */
		int matIdx = 0;
		switch (this.curOpt) {
		case Options.BaseColor:
			matIdx = PlayerModel.BaseColor;
			break;
		case Options.DetailColor:
			matIdx = PlayerModel.MainDetailColor;
			break;
		case Options.SubDetailColor:
			matIdx = PlayerModel.SubDetailColor;
			break;
		}
		this.originalColor = matIdx;

		int colorIdx = 0;
		while (colorIdx < this.idxToColor.Length && this.idxToColor[colorIdx] != matIdx) {
			colorIdx++;
		}

		/* Find the position of this index in the palette. */
		this.colorX = 0;
		int tmp = 0;
		for (this.colorY = 0; tmp + this.SortedColors[this.colorY].Colors.Length < colorIdx; this.colorY++) {
			tmp += this.SortedColors[this.colorY].Colors.Length;
		}
		this.colorX = colorIdx - tmp;

		/* Position the selector on top of the color. */
		this.selector.gameObject.SetActive(true);
		this.setSelectorPosition();

		/* Start changing the color. */
		this.onPalette = true;
		this.updateSelect();
	}

	/**
	 * Disable "color selection mode".
	 *
	 * @param revert: Whether the color should be reverted or kept.
	 */
	private void stopColorSelection(bool revert) {
		/* Revert to the original color. */
		if (revert) {
			switch (this.curOpt) {
			case Options.BaseColor:
				PlayerModel.BaseColor = this.originalColor;
				break;
			case Options.DetailColor:
				PlayerModel.MainDetailColor = this.originalColor;
				break;
			case Options.SubDetailColor:
				PlayerModel.SubDetailColor = this.originalColor;
				break;
			}
		}

		this.selector.gameObject.SetActive(false);

		this.onPalette = false;
		this.updateSelect();
	}

	/**
	 * Handle adjusting the palette view after a movement. The movement itself
	 * must be implemented within the supplied lambda.
	 *
	 * @param move: The lambda used to select the next color.
	 */
	private void paletteMovement(Lambda move) {
		move();

		/* Ensure the position is valid. */
		if (this.colorY < 0) {
			this.colorY = this.SortedColors.Length - 1;
		}
		else if (this.colorY >= this.SortedColors.Length) {
			this.colorY = 0;
		}

		Material[] list = this.SortedColors[this.colorY].Colors;
		if (this.colorX < 0) {
			this.colorX = list.Length - 1;
		}
		else if (this.colorX >= list.Length) {
			this.colorX = 0;
		}

		/* Update the selector's position. */
		this.setSelectorPosition();

		/* Figure out the index in the player's list of materials. */
		int colorIdx = this.colorX;
		for (int i = 0; i < this.colorY; i++) {
			colorIdx += this.SortedColors[i].Colors.Length;
		}
		int matIdx = this.idxToColor[colorIdx];

		/* Update the player's material. */
		switch (this.curOpt) {
		case Options.BaseColor:
			PlayerModel.BaseColor = matIdx;
			break;
		case Options.DetailColor:
			PlayerModel.MainDetailColor = matIdx;
			break;
		case Options.SubDetailColor:
			PlayerModel.SubDetailColor = matIdx;
			break;
		default:
			/* Do nothing. */
			break;
		}
	}

	override protected void onLeft() {
		if (this.onPalette) {
			this.paletteMovement(() => {
				this.colorX--;
			});
		}
		else if (this.curOpt == Options.Model) {
			this.onMovement(() => {
				this.playerModel--;
			});
		}
	}

	override protected void onRight() {
		if (this.onPalette) {
			this.paletteMovement(() => {
				this.colorX++;
			});
		}
		else if (this.curOpt == Options.Model) {
			this.onMovement(() => {
				this.playerModel++;
			});
		}
	}

	override protected void onUp() {
		if (!this.onPalette) {
			this.onMovement(() => {
				this.curOpt--;
			});
		}
		else {
			this.paletteMovement(() => {
				this.colorY--;
			});
		}
	}

	override protected void onDown() {
		if (!this.onPalette) {
			this.onMovement(() => {
				this.curOpt++;
			});
		}
		else {
			this.paletteMovement(() => {
				this.colorY++;
			});
		}
	}

	override protected void onSelect() {
		switch (this.curOpt) {
		case Options.BaseColor:
		case Options.DetailColor:
		case Options.SubDetailColor:
			if (!this.onPalette) {
				this.startColorSelection();
			}
			else {
				this.stopColorSelection(false);
			}
			break;
		case Options.Back:
			this.onCancel();
			break;
		default:
			/* Do Nothing. */
			break;
		}
	}

	override protected void onCancel() {
		if (!this.onPalette) {
			Config.savePlayerModel();
			this.LoadScene("scenes/menu/MainMenu");
		}
		else {
			this.stopColorSelection(true);
		}
	}
}
