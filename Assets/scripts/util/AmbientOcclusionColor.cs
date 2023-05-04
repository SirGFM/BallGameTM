using VolumeList = System.Collections.Generic.List<UnityEngine.Rendering.PostProcessing.PostProcessVolume>;
using AmbientOcclusion = UnityEngine.Rendering.PostProcessing.AmbientOcclusion;
using PostProcessLayer = UnityEngine.Rendering.PostProcessing.PostProcessLayer;
using PostProcessManager = UnityEngine.Rendering.PostProcessing.PostProcessManager;
using GO = UnityEngine.GameObject;

/**
 * AmbientOcclusionColor temporarily overrides the color of the ambient occlusion.
 * This set the value once and, as soon as it's done, it halts.
 */

public class AmbientOcclusionColor : BaseRemoteAction {
	public UnityEngine.Color color;

	void Update() {
		GO camera = null;

		/* Find the PostProcessLayer in the main camera. */
		this.rootEvent<CameraHelperIface>( (x,y) => x.GetMainCamera(out camera) );
		var layer = camera?.GetComponent<PostProcessLayer>();
		if (layer == null) {
			return;
		}

		/* Get the list of volumes (i.e., where effects are stored) in this camera. */
		VolumeList volumes = new VolumeList();
		PostProcessManager.instance?.GetActiveVolumes(layer, volumes);

		/* Find an AmbientOcclusion in the retrieved volumes and set its color. */
		foreach (var volume in volumes) {
			var ambientOcclusion = volume.profile.GetSetting<AmbientOcclusion>();
			if (ambientOcclusion != null) {
				ambientOcclusion.color.Override(this.color);

				this.enabled = false;
				return;
			}
		}
	}
}
