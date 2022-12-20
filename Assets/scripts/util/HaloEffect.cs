using Color = UnityEngine.Color;
using Material = UnityEngine.Material;
using ObjectRenderer = UnityEngine.MeshRenderer;
using Time = UnityEngine.Time;
using Transform = UnityEngine.Transform;
using Vec3 = UnityEngine.Vector3;

/**
 * HalloEffect manipulates the object and its children's texture
 * to show/hide a colored effect around the object.
 *
 * The show/hide effect is achieved by scaling the object up/down.
 * The object's texture is duplicated and its '_TintColor' is modified
 * to match any OnSetColoredHalo received.
 *
 * The effect's visuals are entirely based on the models in this
 * object's children.
 *
 * Lastly, this object sends a RegisterHalo on Start().
 */

public class HaloEffect : BaseRemoteAction, SetColoredHaloIface {

	/** The basic material used by every object. */
	public Material material;

	/** How long it takes to scale the effect up/down. */
	public float scaleTime = 0.5f;

	/** Size of the effect, when hidden. */
	public float hiddenScale = 0.125f;

	/** Size of the effect, when visible. */
	public float visibleScale = 1.0f;

	/** The object's transform. */
	private Transform self;

	/** Whether a color was set this frame. */
	private bool hasColor;

	/** Last color that was set. */
	private Color lastColor;

	void Start() {
		ObjectRenderer[] children;
		children = this.gameObject.GetComponentsInChildren<ObjectRenderer>();

		if (children.Length == 0) {
			throw new System.Exception($"{this} requires at least on child with a renderer.");
		}

		/* Set the material of the first child and then get its copy. */
		children[0].sharedMaterial = this.material;
		this.material = children[0].material;

		foreach (ObjectRenderer r in children) {
			r.sharedMaterial = this.material;
		}

		this.self = this.transform;
		this.setScale(this.hiddenScale);
		this.hasColor = false;

		/* Register this as the object's halo. */
		issueEvent<HandleColoringHaloIface>( (x,y) => x.RegisterHalo(this.gameObject) );
	}

	void OnDestroy() {
		/* Destroy the duplicated material. */
		Destroy(this.material);
	}

	/**
	 * Update this object's scale to the desired size.
	 *
	 * @param scale: The size;
	 */
	private void setScale(float scale) {
		this.self.localScale = new Vec3(scale, scale, scale);
	}

	void Update() {
		float delta = this.visibleScale - this.hiddenScale;
		delta = delta / this.scaleTime * Time.deltaTime;

		if (this.hasColor && this.self.localScale.x < this.visibleScale) {
			float scale = this.self.localScale.x + delta;

			if (scale > this.visibleScale) {
				scale = this.visibleScale;
			}
			this.setScale(scale);
		}
		else if (!this.hasColor && this.self.localScale.x > this.hiddenScale) {
			float scale = this.self.localScale.x - delta;

			if (scale < this.hiddenScale) {
				scale = this.hiddenScale;
			}
			this.setScale(scale);
		}
	}

	void LateUpdate() {
		this.hasColor = false;
	}

	public void OnSetColoredHalo(Color color) {
		this.hasColor = true;

		if (this.lastColor != color) {
			this.material.SetColor("_TintColor", color);
			this.lastColor = color;
		}
	}
}
