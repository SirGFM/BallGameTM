using Model = UnityEngine.Mesh;
using ObjectModel = UnityEngine.MeshFilter;

/**
 * LowResModel swaps the game object's model by a simpler, lower resolution
 * one, so the game may run better in lower-end machines.
 */

public class LowResModel : UnityEngine.MonoBehaviour {
	/** The low res model. */
	public UnityEngine.Mesh Model;

	void Start() {
		if (Config.getLowResModels()) {
			ObjectModel mesh = this.gameObject.GetComponent<ObjectModel>();
			mesh.sharedMesh = this.Model;
		}
	}
}
