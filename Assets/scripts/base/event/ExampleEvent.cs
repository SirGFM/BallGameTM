using EvSys = UnityEngine.EventSystems;
using GO = UnityEngine.GameObject;

public interface ExampleEventIface : EvSys.IEventSystemHandler {
	/**
	 * A dummy interface for events.
	 *
	 * @param sender: The event sender
	 * @param msg: The sender's message
	 */
	void OnExampleEvent(GO sender, string msg);
}

public class ExampleEvent : UnityEngine.MonoBehaviour, ExampleEventIface {
	/**
	 * Simply log the received messsage.
	 *
	 * @param sender: The event sender
	 * @param msg: The sender's message
	 */
	public void OnExampleEvent(GO sender, string msg) {
		UnityEngine.Debug.Log($"Received message {msg} from {sender}");
	}
}
