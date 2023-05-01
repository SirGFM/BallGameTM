using Vec2 = UnityEngine.Vector2;

/**
 * UnlitDisplacementScroll controls the Unlit/displacement shader
 * (for example, used on the Lava material).
 *
 * This shader uses two texture: a main one, that gives the details/colors of the model,
 * and a gray-scale one that displaces specific pixels on the main texture.
 * White pixels on this secondary texture moves the pixels in the main one to the top right,
 * and black pixels moves them to the bottom left. Gray (128, or 0.5, in every component) pixels
 * keeps them on their original position.
 *
 * This scripts configures and scrolls both textures as desired.
 * This may be used to generate a ripple/wave effect over the main texture.
 *
 * This effect was based on "How scrolling textures gave Super Mario Galaxy 2 its charm"
 * (https://www.youtube.com/watch?v=8rCRsOLiO7k)..
 */

public class UnlitDisplacementScroll : UnityEngine.MonoBehaviour {
	/** How many times the main texture is repeated in both directions. */
	public Vec2 TextureTiling;

	/**
	 * Main texture's scrolling speed in revolutions/s.
	 * Setting this to 1 causes the texture to do 1 full revolution over that dimension per second.
	 */
	public Vec2 TextureScrollSpeed;

	/** How many times the displacement texture is repeated in both directions. */
	public Vec2 DisplacementTiling;

	/**
	 * Displacement texture's scrolling speed in revolutions/s.
	 * Setting this to 1 causes the texture to do 1 full revolution over that dimension per second.
	 */
	public Vec2 DisplacementScrollSpeed;

	/**
	 * How much each pixel is displaced. The greater the value,
	 * the noisier the image becomes.
	 */
	public float Factor;

	/**
	 * Name of the main texture property in the shader.
	 */
	public string TextureProperty = "_MainTex";

	/**
	 * Name of the displacement texture property in the shader.
	 */
	public string DisplacementProperty = "_DisplacementTex";

	/**
	 * Name of the displacement factor property in the shader.
	 */
	public string FactorProperty = "_DisplacementFactor";

	/**
	 * The model's texture (which must use a Unlit/displacement shader).
	 */
	private UnityEngine.Material Material;

	/**
	 * The ID of the main texture, in the shader.
	 */
	private int TextureID;

	/**
	 * The ID of the displacement texture, in the shader.
	 */
	private int DisplacementID;

	/**
	 * The ID of the main texture, in the shader.
	 */
	private int FactorID;

	/**
	 * Current offset of the main texture (in the range [0.0, TextureTiling]).
	 */
	private Vec2 TextureOffset;

	/**
	 * Current offset of the displacement texture (in the range [0.0, DisplacementTiling]).
	 */
	private Vec2 DisplacementOffset;

	void Start() {
		this.TextureOffset = new Vec2();
		this.DisplacementOffset = new Vec2();

		this.Material = this.gameObject.GetComponent<UnityEngine.Renderer>().material;

		/* Cache the ID of properties. */
		this.TextureID = -1;
		this.DisplacementID = -1;
		this.FactorID = -1;

		var names = this.Material.GetTexturePropertyNames();
		var ids = this.Material.GetTexturePropertyNameIDs();

		for (int i = 0; i < names.Length; i++) {
			if (names[i] == this.TextureProperty) {
				this.TextureID = ids[i];
			}
			else if (names[i] == this.DisplacementProperty) {
				this.DisplacementID = ids[i];
			}
			else if (names[i] == this.FactorProperty) {
				this.FactorID = ids[i];
			}
		}

		if (this.TextureID == -1) {
			throw new System.Exception($"Couldn't find texture property ('{this.TextureProperty}')");
		}
		else if (this.DisplacementID == -1) {
			throw new System.Exception($"Couldn't find displacement property ('{this.DisplacementProperty}')");
		}
		else if (this.FactorID == -1) {
			//throw new System.Exception($"Couldn't find factor property ('{this.FactorProperty}')");

			// XXX: For some reason, the factor isn't getting exported for the Material,
			// even though it's assignable by name...
			//
			// Set it at initialization and call it a day. :/
			this.Material.SetFloat(this.FactorProperty, this.Factor);
		}
	}

	/**
	 * clamp ensures that value is in the range [0.0, max].
	 *
	 * @param value: The current value.
	 * @param max: The maximum allowed value.
	 * @param min: The minimum allowed value.
	 * @return The clamped value.
	 */
	private float clamp(float value, float max, float min) {
		while (value > max) {
			value -= max;
		}
		while (value < min) {
			value += max;
		}
		return value;
	}

	/**
	 * Integrate (using Euler integration) the position,
	 * ensuring its in the range [0.0, maxPosition].
	 *
	 * @param maxPosition: The maximum allowed position.
	 * @param position: The current position.
	 * @param velocity: The moving speed.
	 * @param dt: The elapsed time.
	 * @return The integrated position.
	 */
	private Vec2 integrate(Vec2 maxPosition, Vec2 position, Vec2 velocity, float dt) {
		position += velocity * dt;
		position.x = clamp(position.x, maxPosition.x, 0.0f);
		position.y = clamp(position.y, maxPosition.y, 0.0f);
		return position;
	}

	void Update() {
		float dt = UnityEngine.Time.deltaTime;

		this.TextureOffset = integrate(this.TextureTiling, this.TextureOffset, this.TextureScrollSpeed, dt);
		this.DisplacementOffset = integrate(this.DisplacementTiling, this.DisplacementOffset, this.DisplacementScrollSpeed, dt);

		this.updateMaterial();
	}

	/**
	 * Update the properties in the material.
	 */
	private void updateMaterial() {
		this.Material.SetTextureOffset(this.TextureID, this.TextureOffset);
		this.Material.SetTextureScale(this.TextureID, this.TextureTiling);
		this.Material.SetTextureOffset(this.DisplacementID, this.DisplacementOffset);
		this.Material.SetTextureScale(this.DisplacementID, this.DisplacementTiling);
		if (this.FactorID != -1) {
			this.Material.SetFloat(this.FactorID, this.Factor);
		}
	}
}
