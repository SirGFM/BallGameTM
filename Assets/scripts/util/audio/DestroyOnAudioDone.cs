using AudioSource = UnityEngine.AudioSource;
using GO = UnityEngine.GameObject;

/**
 * DestroyOnAudioDone pools whether the audio is playing or not, destroing the
 * game object as soon as it detects that the audio has finished.
 */

public class DestroyOnAudioDone  : UnityEngine.MonoBehaviour {
	private AudioSource src;

	void Start() {
		src = this.gameObject.GetComponent<AudioSource>();
		if (src == null) {
			throw new System.Exception($"'{this}' must have an AudioSource!");
		}
	}

	void Update() {
		if (!src.isPlaying) {
			GO.Destroy(this.gameObject);
		}
	}
}
