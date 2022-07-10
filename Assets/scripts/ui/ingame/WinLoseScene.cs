using Color = UnityEngine.Color;
using RawImage = UnityEngine.UI.RawImage;
using RectT = UnityEngine.RectTransform;
using Time = UnityEngine.Time;
using UiText = UnityEngine.UI.Text;
using Vec2 = UnityEngine.Vector2;
using Vec3 = UnityEngine.Vector3;

/**
 * WinLoseScene fades a BG color in, dimming out the game and show a text
 * describing a win/lose situation. Then, it waits for any input before doing
 * an action (e.g., restarting the stage).
 *
 * This should be sub-classed so the following functions may be implemented:
 *   - onJustPressed
 *   - playOpeningSfx
 *   - playSfx
 */

public class WinLoseScene : BaseRemoteAction {

	/** A image covering the entire screen with a single color. */
	public RectT bgColor;
	/** The main title of the scene (e.g., "You Win"). */
	public RectT title;
	/** A repeat of the title, that flashes over the title. */
	public RectT titleFx;
	/** Flavor text that drops before allowing the scene to continue. */
	public RectT flavor;
	/** Text describing how to advance from this scene. */
	public RectT pressToPlay;

	/** How long the scaling of the title takes, in seconds. */
	public float scaleDelay = 3.0f;
	/** Initial position of the title (e.g., offscreen). */
	public float initialPos;
	/** Final position of the title (e.g., centered). */
	public float finalPos;
	/** How much should the title be up scaled. */
	public float titleScale = 2.0f;
	/** How long the entire effect takes (fade-in + fade-out), in seconds. */
	public float fxDelay = 0.3f;
	/** How long it takes to show the flavor text, in seconds. */
	public float txtDelay = 0.5f;
	/** Period of the text describing how to advance the scene flash effect. */
	public float inputDelay = 1.0f;

	/** Whether the scene is ready to receive events, enabling onJustPressed()
	 * to be called. */
	private bool waitingForInput;

	/** Called after playing the entire animation, when any input on any input
	 * device is detected. */
	protected virtual void onJustPressed() {
	}

	/** Play an SFX as soon as the animation starts. */
	protected virtual void playOpeningSfx() {
	}

	/** Play an SFX as soon as the main title has fully appeared. */
	protected virtual void playSfx() {
	}

	/** Executes the animation. */
	private System.Collections.IEnumerator run() {
		float dt;

		RawImage bgImg = this.bgColor.GetComponent<RawImage>();

		this.playOpeningSfx();

		/* Drop the "You Win" title and upscale it */
		for (dt = 0.0f; dt < this.scaleDelay; dt += Time.deltaTime) {
			float scale = this.titleScale * dt / scaleDelay;
			this.title.localScale = new Vec3(scale, scale, 1.0f);

			float pos = initialPos + (finalPos - initialPos) * dt / scaleDelay;
			this.title.anchoredPosition = new Vec2(0 ,pos);

			float alpha = 0.5f * dt / scaleDelay;
			Color c = bgImg.color;
			bgImg.color = new Color(c.r, c.g, c.b, alpha);

			yield return null;
		}

		/* Show and hide the effect over the "You Win" title */
		this.titleFx.gameObject.SetActive(true);
		UiText fx = this.titleFx.GetComponent<UiText>();

		this.playSfx();

		float _fxDelay = this.fxDelay / 2.0f;
		for (dt = 0.0f; dt < _fxDelay; dt += Time.deltaTime) {
			yield return null;

			fx.color = new Color(1.0f, 1.0f, 1.0f, 0.5f * dt / _fxDelay);
			float scale = this.titleScale + 0.25f * dt / _fxDelay;
			this.titleFx.localScale = new Vec3(scale, scale, 1.0f);
		}
		for (; dt > 0.0f; dt -= Time.deltaTime) {
			yield return null;

			fx.color = new Color(1.0f, 1.0f, 1.0f, 0.5f * dt / _fxDelay);
			float scale = this.titleScale + 0.25f * dt / _fxDelay;
			this.titleFx.localScale = new Vec3(scale, scale, 1.0f);
		}
		this.titleFx.gameObject.SetActive(false);

		if (this.flavor != null) {
			UiText txt = this.flavor.GetComponent<UiText>();

			/* Show the extra flavor text */
			for (dt = 0.0f; dt < this.txtDelay; dt += Time.deltaTime) {
				yield return null;

				txt.color = new Color(1.0f, 1.0f, 1.0f, dt / this.txtDelay);

				float scale = dt / this.txtDelay;
				this.flavor.localScale = new Vec3(1.0f, scale, 1.0f);
			}
			txt.color = Color.white;
		}

		/* Run indefinitely waiting for input */
		UiText[] txts = this.pressToPlay.GetComponentsInChildren<UiText>();
		this.waitingForInput = true;
		while (true) {
			float _inputDelay = this.inputDelay / 2.0f;
			for (dt = 0.0f; dt < _inputDelay; dt += Time.deltaTime) {
				yield return null;
				float alpha = dt / _inputDelay;
				foreach (UiText t in txts)
					t.color = new Color(1.0f, 1.0f, 1.0f, alpha);
			}
			for (; dt > 0.0f; dt -= Time.deltaTime) {
				yield return null;
				float alpha = dt / _inputDelay;
				foreach (UiText t in txts)
					t.color = new Color(1.0f, 1.0f, 1.0f, alpha);
			}
		}
	}

	void Start() {
		this.waitingForInput = false;
		this.StartCoroutine(this.run());
	}

	void Update() {
		if (this.waitingForInput && Input.CheckAnyKeyJustPressed()) {
			this.onJustPressed();
		}
	}
}
