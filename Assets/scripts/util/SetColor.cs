using Color = UnityEngine.Color;
using Renderer = UnityEngine.Renderer;

/**
 * SetColor overwrites the color of the object's material. The material
 * SHALL become a custom instance, and thus shouldn't affect other objects.
 *
 * This should mainly be used in development, to speed up prefab creation,
 * instead of having to create a custom material for each temporary object.
 */

public class SetColor : UnityEngine.MonoBehaviour {

	public Color color = Color.white;

	public void Start() {
		Renderer r = this.GetComponent<Renderer>();

		if (r == null) {
				throw new System.Exception($"{this} requires a render!");
		}
		r.material.color = this.color;
	}
}
