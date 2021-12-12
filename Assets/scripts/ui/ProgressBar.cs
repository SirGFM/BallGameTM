using Axis = UnityEngine.RectTransform.Axis;
using UEMath = UnityEngine.Mathf;
using Rect = UnityEngine.Rect;
using Transform = UnityEngine.RectTransform;

/**
 * ProgressBar updates the scale of the object based on its progress.
 * Another script should get a reference to this component, by assigning
 * itself as the Root object and accepting SetProgressBar events, and
 * update the progress field manually.
 */

public class ProgressBar : BaseRemoteAction {
    /** Rectangle that contains the image */
    private Transform uit;
    /** Original width of the image's rectangle */
    private float width;

    /** Track whether the progress changed, and the image must be expanded */
    private float lastProgress = 0.0f;
    /** Current progress, in the [0.0f, 1.0f] range */
    public float progress = -1.0f;

    void Start() {
        this.uit = null;
        this.getSelf();

		this.StartCoroutine(this.setProgressBar());
    }

	/**
	 * Try to set this object as a ProgressBar every frame, until it succeeds.
	 */
	private System.Collections.IEnumerator setProgressBar() {
		bool done = false;

		do {
			rootEvent<LoaderIface>( (x,y) => x.SetProgressBar(out done, this));
			yield return null;
		} while (done);
	}

    private bool getSelf() {
        if (this.uit == null) {
            this.uit = this.GetComponent<Transform>();
            this.width = this.uit.rect.width;
            this.uit.SetSizeWithCurrentAnchors(Axis.Horizontal, 0);
        }
        return (this.uit != null);
    }

    void Update() {
        if (!this.getSelf())
            return;

        if (this.lastProgress != this.progress) {
            if (this.progress < 0.0f)
                this.progress = 0.0f;
            else if (this.progress > 1.0f)
                this.progress = 1.0f;

            int size = (int)UEMath.Floor(this.progress * this.width);
            this.uit.SetSizeWithCurrentAnchors(Axis.Horizontal, size);
            this.lastProgress = this.progress;
        }
    }
}
