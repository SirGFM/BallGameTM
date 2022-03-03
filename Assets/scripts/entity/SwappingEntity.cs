using ExecEv = UnityEngine.EventSystems.ExecuteEvents;
using GO = UnityEngine.GameObject;
using Time = UnityEngine.Time;

/**
 * Component to sequentially alternate between multiple gameObject in a
 * timely manner. Each object should implement one specific, for example
 * a force field pushing upward and another pushing downward. Empty
 * gameObjects are also allowed and simply cause that the activation of
 * the next gameObject to be delayed.
 *
 * The DefaultTime is used if a custom time isn't specified for the object.
 *
 * This component tries to send a SetActive event to the entities getting
 * enabled and disabled. If the event isn't handled, the gameObject itself
 * gets enabled/disabled. Otherwise, it's left as is.
 */

public class SwappingEntity : UnityEngine.MonoBehaviour {

	/** An entity that is enabled for some time. */
	[System.Serializable]
	public class TimedEntity {
		/** The gameObject. */
		public GO Object;
		/** For how long the gameObject should stay active. */
		public float Time;

		/** Activate this object and return its active time, or 0 if not set. */
		public float Activate() {
			if (this.Object != null) {
				bool handled = false;

				if (this.Object.activeSelf) {
					ExecEv.ExecuteHierarchy<SetActiveIface>(
							this.Object, null,
							(x,y) => x.SetActive(out handled, true));
				}

				if (!handled || !this.Object.activeSelf) {
					this.Object.SetActive(true);
				}
			}

			if (this.Time > 0.0f) {
				return this.Time;
			}
			return 0.0f;
		}

		/** Deactivate this object. */
		public void Deactivate() {
			if (this.Object != null) {
				bool handled = false;

				ExecEv.ExecuteHierarchy<SetActiveIface>(
						this.Object, null,
						(x,y) => x.SetActive(out handled, false));
				if (!handled) {
					this.Object.SetActive(false);
				}
			}
		}
	};

	/** Objects that are sequentially enabled by this object. */
	public TimedEntity[] Objects;

	/** For how long objects should be enabled, when not specified. */
	public float DefaultTime = 5.0f;

	/** Index of the current object. */
	private int idx;

	/** Time until the object must be disabled. */
	private float dt;

	void Start() {
		this.idx = 0;

		/* Disable every object but the first one. */
		for (int i = 0; i < this.Objects.Length; i++) {
			this.Objects[i].Deactivate();
		}

		if (this.Objects.Length > 0) {
			this.dt = this.Objects[0].Activate();
			if (this.dt == 0.0f) {
				this.dt = this.DefaultTime;
			}
		}
	}

	void Update() {
		this.dt -= Time.deltaTime;
		if (this.dt <= 0) {
			if (this.Objects.Length == 0) {
				return;
			}

			this.Objects[this.idx].Deactivate();

			this.idx++;
			if (this.idx >= this.Objects.Length) {
				this.idx = 0;
			}

			float newDt = this.Objects[this.idx].Activate();
			if (newDt == 0.0f) {
				newDt = this.DefaultTime;
			}

			this.dt += newDt;
		}
	}
}
