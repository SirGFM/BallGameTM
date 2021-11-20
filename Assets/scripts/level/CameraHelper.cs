using AudioListener = UnityEngine.AudioListener;
using Camera = UnityEngine.Camera;
using EvSys = UnityEngine.EventSystems;
using GO = UnityEngine.GameObject;

/**
 * CameraHelper allows an object to register itself as the main camera
 * within the root object. This component should only be used on the root
 * gameObject, so CameraHelperIface events may be issued to the root
 * object.
 */

public interface CameraHelperIface : EvSys.IEventSystemHandler {
	/**
	 * Configure the main camera for a given object.
	 *
	 * @param camera: The main camera.
	 */
	void SetMainCamera(GO camera);

	/**
	 * Retrieve the previously configured the main camera.
	 *
	 * @param out camera: The main camera.
	 */
	void GetMainCamera(out GO camera);
}

public class CameraHelper : UnityEngine.MonoBehaviour, CameraHelperIface {
	/* This object's main camera. */
	private GO mainCamera = null;

	void Start() {
		/* Depending on how the stages are loaded into the final game, the
		 * stage itself may not have a camera. This should only affect
		 * running the game within the editor, in which case a camera must
		 * be manually added to the scene. */
		if (Camera.allCamerasCount == 0) {
			GO obj = new GO("TempCamera", typeof(Camera),
					typeof(CameraController), typeof(AudioListener));
			obj.tag = "MainCamera";

			Camera cam = obj.GetComponent<Camera>();
			cam.depth = -1;
		}
	}

	public void SetMainCamera(GO camera) {
		this.mainCamera = camera;
	}

	public void GetMainCamera(out GO camera) {
		camera = this.mainCamera;
	}
}
