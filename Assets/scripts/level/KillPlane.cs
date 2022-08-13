using Col = UnityEngine.Collider;

public class KillPlane : BaseRemoteAction {

	void OnTriggerEnter(Col other) {
		rootEvent<GoalIface>( (x,y) => x.OnRetryLevel() );
		this.gameObject.SetActive(false);
	}
}
