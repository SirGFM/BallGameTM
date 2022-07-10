using Col = UnityEngine.Collider;

public class KillPlane : UnityEngine.MonoBehaviour {
	private bool done = false;

	void OnTriggerEnter(Col other) {
		if (this.done) {
			return;
		}

		Loader.StartLoseAnimation();
		this.gameObject.SetActive(false);

		this.done = true;
	}
}
