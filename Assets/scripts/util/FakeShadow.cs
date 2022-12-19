using GO = UnityEngine.GameObject;
using QualitySettings = UnityEngine.QualitySettings;
using Transform = UnityEngine.Transform;
using Vec3 = UnityEngine.Vector3;

public class FakeShadow : UnityEngine.MonoBehaviour {

	/** Layer of objects that may be hit by this shadow. */
	public UnityEngine.LayerMask targetLayers;

	/** The actual object from which the shadow is cast.
	 * If not set, it's set to the transform's parent. */
	public Transform parent;

	/** The shadow object. */
	private Transform self;

	/** The vertical position of the shadow, in world space. */
	private float posY;

	/** The list of objects that were hit by the raycast. */
	private UnityEngine.RaycastHit[] results;

	/** How many objects may be hit by the raycast at most. */
	public int numTargets = 20;

	/** Maximum distance for this soft/fake shadow. */
	public float maxDist = 100.0f;

	/** Distance from the hit target, to avoid Z-fighting. */
	public float targetOffset = 0.05f;

	/** The size of the shadow at the farthest distance. */
	public float minShadowScale = 0.01f;

	/** The size of the shadow at the closest distance. */
	public float maxShadowScale = 1.5f;

	/** The last normal hit, to avoid recalculating the object's angle. */
	private Vec3 lastNormal;

	public void Start() {
		/* Shadow is enable from Medium (2) onward.
		 * So, if only enable this if the quality is bellow that. */
		int qualityMode = QualitySettings.GetQualityLevel();
		if (qualityMode >= 2) {
			GO.Destroy(this.gameObject);
			return;
		}

		this.self = this.transform;
		this.results = new UnityEngine.RaycastHit[this.numTargets];
		if (this.parent == null) {
			this.parent = this.self.parent;
		}
	}

	/** Update this object's position to the parent's
	 * using the vertical position from the raycast. */
	public void FixedUpdate() {
		Vec3 pos = this.parent.position;

		/* Use the absolute position of the collision. */
		pos.y = this.posY + this.targetOffset;
		this.self.position = pos;
	}

	/** Update the vertical position to the closest object
	 * (but less often than the object is moved in the X/Y plane. */
	public void Update() {
		/* Initialize minDist with something greater than the max distance
		 * so we may know whether or not anything was hit. */
		float minDist = this.maxDist + 1.0f;
		Vec3 normal = Vec3.zero;

		int hits = UnityEngine.Physics.RaycastNonAlloc(
				this.parent.position,
				-this.parent.up,
				this.results,
				this.maxDist,
				this.targetLayers,
				UnityEngine.QueryTriggerInteraction.Ignore);

		/** Find the closest collision point. */
		for (int i = 0; i < hits; i++) {
			if (this.results[i].distance < minDist) {
				minDist = this.results[i].distance;
				this.posY = this.results[i].point.y;
				normal = this.results[i].normal;
			}
		}

		if (minDist <= this.maxDist) {
			/* Re-scale the shadow. */
			float alpha = minDist / this.maxDist;
			float delta = this.maxShadowScale - this.minShadowScale;

			float scale = this.maxShadowScale - alpha * delta;
			this.self.localScale = new Vec3(scale, 1.0f, scale);

			/* Rotate it to follow the object that was hit. */
			if (this.lastNormal != normal) {
				this.self.rotation = UnityEngine.Quaternion.FromToRotation(Vec3.up, normal);
				this.lastNormal = normal;
			}
		}
		else if (this.self.localScale.x > 0.0f) {
			/* Nothing was hit, just hide the shadow. */
			this.self.localScale = new Vec3(0.0f, 1.0f, 0.0f);
		}
	}
}
