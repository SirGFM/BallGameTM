using GO = UnityEngine.GameObject;

public class ExampleEventSender : BaseRemoteAction {

	/**
	 * The event's target. If unset, the event will target the sender
	 * itself. */
	public GO target;
	/** The message sent in the event. */
	public string msg;
	/**
	 * (Lazy method to) send an event. It's immediately consumed and set
	 * back to false. */
	public bool send = false;

	void Update() {
		if (send) {
			send = false;

			issueEvent<ExampleEventIface>(
					(x,y) => x.OnExampleEvent(this.gameObject, this.msg),
					this.target);
		}
	}
}
