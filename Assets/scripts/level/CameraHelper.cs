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

	public void SetMainCamera(GO camera) {
		this.mainCamera = camera;
	}

	public void GetMainCamera(out GO camera) {
		camera = this.mainCamera;
	}
}
