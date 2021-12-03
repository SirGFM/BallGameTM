using AudioListener = UnityEngine.AudioListener;
using Camera = UnityEngine.Camera;
using EvSys = UnityEngine.EventSystems;
using GO = UnityEngine.GameObject;
using Sphere = UnityEngine.SphereCollider;
using Rigidbody = UnityEngine.Rigidbody;
using Vec3 = UnityEngine.Vector3;

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

	/** How many colliders the camera has, for zooming onto the player. */
	private int numColliders = 10;

	void Start() {
		/* Depending on how the stages are loaded into the final game, the
		 * stage itself may not have a camera. This should only affect
		 * running the game within the editor, in which case a camera must
		 * be manually added to the scene. */
		if (Camera.allCamerasCount == 0) {
			int camLayer = UnityEngine.LayerMask.NameToLayer("Player");

			GO obj = new GO("TempCamera", typeof(Camera),
					typeof(CameraController), typeof(AudioListener));
			obj.tag = "MainCamera";
			obj.layer = camLayer;

			Camera cam = obj.GetComponent<Camera>();
			cam.depth = -1;

			for (int i = 0; i < numColliders; i++) {
				this.createChild(obj.transform, i, camLayer);
			}
		}
	}

	/**
	 * Create a collider for controlling the camera's distance from its
	 * target.
	 *
	 * This assumes that colliders each collider should be 1 unit smaller
	 * than the next, and should the set camera's distance to its size. The
	 * colliders are also placed slightly above and behind the camera.
	 *
	 * @param parent: The camera, to which the spanwed child is attached.
	 * @param idx: Object's index between 0 and numColliders, used to define
	 *             its size and position.
	 * @param camLayer: The layer for camera-related objects.
	 */
	private void createChild(UnityEngine.Transform parent, int idx,
			int camLayer) {
		GO child = new GO("CameraCollision", typeof(Rigidbody),
				typeof(Sphere), typeof(SetCameraDistance));
		child.layer = camLayer;

		Rigidbody rb = child.GetComponent<Rigidbody>();
		rb.isKinematic = true;
		rb.useGravity = false;

		float size = ((float)numColliders) - 1.0f - (float)idx;
		float pos = 0.25f * (((float)numColliders) - size);

		Sphere sp = child.GetComponent<Sphere>();
		sp.radius = size * 0.5f;
		sp.center = new Vec3(0.0f, 1.0f, -1.0f);
		sp.center = sp.center.normalized * pos;
		sp.isTrigger = true;

		SetCameraDistance dist = child.GetComponent<SetCameraDistance>();
		dist.Distance = size;

		child.transform.parent = parent;
		child.transform.position = new Vec3();
	}

	public void SetMainCamera(GO camera) {
		this.mainCamera = camera;
	}

	public void GetMainCamera(out GO camera) {
		camera = this.mainCamera;
	}
}
