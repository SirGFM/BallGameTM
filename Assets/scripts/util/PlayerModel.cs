using Material = UnityEngine.Material;
using Model = UnityEngine.Mesh;
using ObjectModel = UnityEngine.MeshFilter;
using ObjectRenderer = UnityEngine.MeshRenderer;

/**
 * PlayerModel automatically replace the gameObject's model and its
 * materials by one of the available options.
 *
 * The model and color options must be set directly into this class.
 */

public class PlayerModel : UnityEngine.MonoBehaviour {
	/** The selected model. */
	static public int Model = 0;

	/** The selected base/body color. */
	static public int BaseColor = 3;

	/** The selected color for the main details. */
	static public int MainDetailColor = 27;

	/** The selected color for the secondary details. */
	static public int SubDetailColor = 13;

	/** List of possible models */
	public UnityEngine.Mesh[] Models;

	/** List of possible models */
	public UnityEngine.Material[] Materials;

	/** The currently active model. */
	private int _model = -1;

	/** The selected base/body color. */
	private int _baseColor = -1;

	/** The selected color for the main details. */
	private int _mainDetailColor = -1;

	/** The selected color for the secondary details. */
	private int _subDetailColor = -1;

	/** The actual, rendered mesh. */
	private ObjectModel mesh = null;

	/** The object with the materials used to render the mesh. */
	private ObjectRenderer paletteSelector = null;

	void Start() {
		this.mesh = this.gameObject.GetComponent<ObjectModel>();
		this.paletteSelector = this.gameObject.GetComponent<ObjectRenderer>();
	}

	void Update() {
		if (this.mesh && PlayerModel.Model != this._model) {
			if (PlayerModel.Model < this.Models.Length) {
				this.mesh.sharedMesh = this.Models[PlayerModel.Model];
			}
			this._model = PlayerModel.Model;
		}

		if (this.paletteSelector && (
				PlayerModel.BaseColor != this._baseColor ||
				PlayerModel.MainDetailColor != this._mainDetailColor ||
				PlayerModel.SubDetailColor != this._subDetailColor)) {
			Material[] materials = this.paletteSelector.sharedMaterials;

			if (materials.Length >= 1 && PlayerModel.BaseColor < this.Materials.Length) {
				materials[0] = this.Materials[PlayerModel.BaseColor];
			}
			if (materials.Length >= 2 && PlayerModel.MainDetailColor < this.Materials.Length) {
				materials[1] = this.Materials[PlayerModel.MainDetailColor];
			}
			if (materials.Length >= 3 && PlayerModel.SubDetailColor < this.Materials.Length) {
				materials[2] = this.Materials[PlayerModel.SubDetailColor];
			}

			this.paletteSelector.sharedMaterials = materials;

			this._baseColor = PlayerModel.BaseColor;
			this._mainDetailColor = PlayerModel.MainDetailColor;
			this._subDetailColor = PlayerModel.SubDetailColor;
		}
	}
}
