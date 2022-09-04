using Color = UnityEngine.Color;
using GO = UnityEngine.GameObject;
using Lambda = System.Action;
using Lambda2P = System.Action<string, string>;
using UiText = UnityEngine.UI.Text;
using UiTextAnchor = UnityEngine.TextAnchor;
using UiTransform = UnityEngine.RectTransform;
using UiFont = UnityEngine.Font;
using Vec2 = UnityEngine.Vector2;
using Vec3 = UnityEngine.Vector3;

using String = System.String;

public class LevelSelect : Menu {
	/** The view containing and moving the names of the worlds. */
	public UiTransform ContentTitle = null;
	/** The view containing and moving the names of the levels. */
	public UiTransform Content = null;
	/** Font used by every text element. */
	public UiFont Font = null;
	/** Font size for the world names. */
	public int TitleSize = 24;
	/** Font size for the level names. */
	public int LevelSize = 16;
	/** Width for each world column. */
	public float ColumnWidth = 150.0f;
	/** Distance between each world column. This may be overwritten if
	 * there are too few columns to fill the entire screen. */
	public float ColumnDistance = 4.0f;
	/** Color used to highlight the selected world/level. */
	public Color SelectedColor = Color.white;
	/** Color of unselected worlds/levels. */
	public Color ShadowColor = new Color(50f/256f, 60f/256f, 67f/256f, 1.0f);
	/** Magic number used to convert from font size to text height... */
	public float FontSizeToHeight = 0.875f;

	/** Content for each world. */
	private struct worldData {
		/** World's title. */
		public string name;
		/** Name for each level. */
		public string[] levels;
		/** Text element used to display this world's name when selected. */
		public UiText uiName;
		/** Text element used to display a selected level in this world. */
		public UiText uiLevels;
		/** Index of the first scene for this world. */
		public int firstLevelIdx;
	}

	/** List of worlds in the game. */
	private worldData[] worlds;

	/** Index of the currently selected world. */
	private int selectedWorld;
	/** Index of the currently selected level. */
	private int selectedLevel;

	/** Calculated width of the view containing every text element. */
	private float contentWidth;
	/** Calculated height of the view containing every level text element. */
	private float contentHeight;
	/** Width of the viewport limiting the visibile text elements. */
	private float viewWidth;
	/** Height of the viewport limiting the visibile level text elements. */
	private float viewHeight;

	/**
	 * Update the view after any movement, highlighting and centering
	 * the currently selected world/level.
	 *
	 * @param lastWorld: The previously selected world (or -1, to ignore).
	 * @param newWorld: The newly selected world.
	 * @param newLevel: The newly selected level.
	 */
	private void updateView(int lastWorld, int newWorld, int newLevel) {
		if (lastWorld != -1 && lastWorld != newWorld) {
			this.worlds[lastWorld].uiName.text = "";
			this.worlds[lastWorld].uiLevels.text = "";
		}

		/* Align the texts horizontally. */
		Vec2 pos = new Vec2();
		pos.x = Content.anchoredPosition.x;
		if (lastWorld != newWorld) {
			this.worlds[newWorld].uiName.text = this.worlds[newWorld].name;

			pos.x = newWorld * (this.ColumnWidth + this.ColumnDistance);
			if (pos.x + this.ColumnWidth >= this.contentWidth) {
				/* Align it to the right edge of the screen. */
				pos.x = this.contentWidth - this.viewWidth;
			}
			else if (pos.x > 0.0f && this.contentWidth > this.viewWidth) {
				/* Align it at the middle of the screen. */
				pos.x = pos.x - this.viewWidth / 2 + this.ColumnWidth / 2;
			}
			pos.x *= -1.0f;

			pos.y = ContentTitle.anchoredPosition.y;
			ContentTitle.anchoredPosition = pos;
		}

		string level = new string('\n', newLevel) + "-- "+this.worlds[newWorld].levels[newLevel]+" --";
		this.worlds[newWorld].uiLevels.text = level;

		/* Align the texts vertically. */
		float entryHeight = this.LevelSize * this.FontSizeToHeight;
		pos.y = entryHeight * newLevel;
		if (pos.y < this.viewHeight / 2) {
			pos.y = 0.0f;
		}
		else if (pos.y + entryHeight >= this.contentHeight - this.viewHeight / 2) {
			/* Align it to the bottom of the screen. */
			pos.y = this.contentHeight - this.viewHeight;
		}
		else {
			/* Align it at the middle of the screen. */
			pos.y = pos.y - this.viewHeight / 2 + entryHeight / 2;
		}
		Content.anchoredPosition = pos;
		Content.anchoredPosition = pos;
	}

	/**
	 * Create a new text element within the given parent.
	 *
	 * @param parent: The parent element.
	 * @param pivot: The position of the text's pivot.
	 * @param size: The text's font size.
	 * @param color: The text's color.
	 * @param anchor: Position of the text within its rect.
	 * @param posX: Position of the text relative to the parent top-left corner.
	 * @param posY: Position of the text relative to the parent top-left corner.
	 * @param text: The element's content.
	 */
	private UiText createText(UiTransform parent, Vec2 pivot, int size, Color color,
			UiTextAnchor anchor, float posX, float posY, string text) {
		GO obj = new GO();

		UiText uiText = obj.AddComponent<UiText>();

		uiText.font = this.Font;
		uiText.fontSize = size;
		uiText.text = text;
		uiText.alignment = anchor;
		uiText.color = color;

		UiTransform rect = uiText.rectTransform;
		rect.SetParent(parent);
		rect.anchorMin = new Vec2(0.0f, 0.0f);
		rect.anchorMax = new Vec2(0.0f, 1.0f);
		rect.pivot =  pivot;
		rect.localScale = new Vec3(1.0f, 1.0f, 1.0f);

		rect.offsetMin = new Vec2(posX, posY);
		rect.offsetMax = new Vec2(posX + this.ColumnWidth, posY);

		return uiText;
	}

	/**
	 * Iterate over every level name in the game and execute a lambda on it.
	 *
	 * @param fn: A lambda receive the world name and the level name.
	 */
	private void foreachLevel(Lambda2P fn) {
		for (int i = 1; i <= LevelNameList.GetLevelCount(); i++) {
			string level = LevelNameList.GetLevel(i);
			if (level == "Gameover") {
				return;
			}

			string world = LevelNameList.GetLevelWorld(i);
			fn(world, level);
		}
	}

	override protected void start() {
		string tmp = null;
		int num = 0;

		this.CombinedLoadScene("scenes/bg-scene/LevelSelectBg");

		/* Find out how many worlds there are in the game. */
		this.foreachLevel((world, level) => {
			if (tmp == null || world != tmp) {
				num++;
			}
			tmp = world;
		});
		this.worlds = new worldData[num];

		/* Find out how many levels there are in each world. */
		tmp = null;
		num = 0;
		int idx = 0;
		this.foreachLevel((world, level) => {
			if (tmp == null || world == tmp) {
				num++;
			}
			else {
				this.worlds[idx].levels = new string[num];
				idx++;
				num = 1;
			}
			tmp = world;
		});
		this.worlds[idx].levels = new string[num];

		/* Store every level and world name. */
		tmp = null;
		num = 0;
		idx = 0;
		this.foreachLevel((world, level) => {
			if (tmp != null && world != tmp) {
				num++;
				idx = 0;
			}
			tmp = world;

			this.worlds[num].name = world;
			this.worlds[num].levels[idx] = level;
			idx++;
		});

		/* Calculate the distance between column, to evenly divide if there aren't
		 * enough columns to fill the view, or to expand it as necessary.
		 * Also calcultate and update the view's height as necessary. */
		UiTransform parent = this.Content.parent.GetComponent<UiTransform>();
		this.viewWidth = parent.rect.width;
		this.viewHeight = parent.rect.height;

		this.contentHeight = 0.0f;
		for (int i = 0; i < this.worlds.Length; i++) {
			float height = this.LevelSize * this.FontSizeToHeight * this.worlds[i].levels.Length;
			if (height > this.contentHeight) {
				this.contentHeight = height;
			}
		}

		float dist = this.ColumnDistance;
		this.contentWidth = (this.ColumnWidth + dist) * this.worlds.Length - dist;
		if (this.contentWidth < this.viewWidth) {
			dist = this.viewWidth - this.ColumnWidth * this.worlds.Length;
			dist /= this.worlds.Length - 1.0f;
			this.contentWidth = this.viewWidth;
			this.ColumnDistance = dist;
		}
		this.Content.offsetMin = new Vec2(0.0f, 0.0f);
		this.Content.offsetMax = new Vec2(this.contentWidth, this.contentHeight);
		this.Content.anchoredPosition = new Vec2(0.0f, 0.0f);

		float minTitleHeight = this.ContentTitle.offsetMin.y;
		float maxTitleHeight = this.ContentTitle.offsetMax.y;
		this.ContentTitle.offsetMin = new Vec2(0.0f, minTitleHeight);
		this.ContentTitle.offsetMax = new Vec2(this.contentWidth, maxTitleHeight);

		/* Create the UI text for the world names. */
		for (int i = 0; i < this.worlds.Length; i++) {
			string world = this.worlds[i].name;

			float posX = (this.ColumnWidth + dist) * i;
			Vec2 pivot = new Vec2(0.0f, 0.5f);
			this.createText(this.ContentTitle, pivot, this.TitleSize, this.ShadowColor,
					UiTextAnchor.MiddleCenter, posX - 1.0f, -1.0f, world);
			UiText text = this.createText(this.ContentTitle, pivot, this.TitleSize,
					this.SelectedColor, UiTextAnchor.MiddleCenter, posX, 0.0f, "");

			this.worlds[i].uiName = text;
		}

		/* Create the UI text for the level names. */
		for (int i = 0; i < this.worlds.Length; i++) {
			string levelsText = String.Join("\n", this.worlds[i].levels);

			float posX = (this.ColumnWidth + dist) * i;
			Vec2 pivot = new Vec2(0.0f, 0.0f);
			this.createText(this.Content, pivot, this.LevelSize, this.ShadowColor,
					UiTextAnchor.UpperCenter, posX, 0.0f, levelsText);
			UiText text = this.createText(this.Content, pivot, this.LevelSize,
					this.SelectedColor, UiTextAnchor.UpperCenter, posX - 1.0f, -1.0f, "");

			this.worlds[i].uiLevels = text;
		}

		/* Calculate the index of the first scene in each world. */
		this.worlds[0].firstLevelIdx = 1;
		for (int i = 1; i < this.worlds.Length; i++) {
			num = this.worlds[i-1].levels.Length;
			this.worlds[i].firstLevelIdx = this.worlds[i - 1].firstLevelIdx + num;
		}

		this.updateView(-1, 0, 0);
	}

	/**
	 * Handle adjusting the view after a movement. The movement itself must
	 * be implemented within the supplied lambda.
	 *
	 * @param move: The lambda used to select the next level/world.
	 */
	private void onMovement(Lambda move) {
		int lastWorld = this.selectedWorld;

		move();

		/* Adjust the currently selected level, ensuring that it's
		 * a valid index for the current world. */
		int max = this.worlds[this.selectedWorld].levels.Length;
		if (this.selectedLevel >= max) {
			this.selectedLevel = max - 1;
		}

		this.updateView(lastWorld, this.selectedWorld, this.selectedLevel);
	}

	override protected void onLeft() {
		this.onMovement(() => {
			this.selectedWorld--;
			if (this.selectedWorld < 0) {
				this.selectedWorld = this.worlds.Length - 1;
			}
		});
	}

	override protected void onRight() {
		this.onMovement(() => {
			this.selectedWorld++;
			if (this.selectedWorld >= this.worlds.Length) {
				this.selectedWorld = 0;
			}
		});
	}

	override protected void onUp() {
		this.onMovement(() => {
			this.selectedLevel--;
			if (this.selectedLevel < 0) {
				this.selectedLevel = this.worlds[this.selectedWorld].levels.Length - 1;
			}
		});
	}

	override protected void onDown() {
		this.onMovement(() => {
			this.selectedLevel++;
			if (this.selectedLevel >= this.worlds[this.selectedWorld].levels.Length) {
				this.selectedLevel = 0;
			}
		});
	}

	override protected void onSelect() {
		int level = this.worlds[this.selectedWorld].firstLevelIdx;
		level += this.selectedLevel;
		this.LoadLevel(level);
	}

	override protected void onCancel() {
		this.LoadScene("scenes/menu/MainMenu");
	}
}
