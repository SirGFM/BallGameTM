using EvSys = UnityEngine.EventSystems;
using ExecEv = UnityEngine.EventSystems.ExecuteEvents;
using GO = UnityEngine.GameObject;
using Handler = UnityEngine.EventSystems.IEventSystemHandler;

/**
 * Helper class for sending events to other objects. See ExampleEvent
 * and ExampleEventSender for a demonstration of the event system.
 *
 * Events are defined by an interface that inherits from
 * UnityEngine.EventSystems.IEventSystemHandler. The functions defined
 * in the interface may then be called by specifying the interface when
 * calling 'issueEvent<>' or 'rootEvent<>', and using a lambda to call
 * the desired function. The object that want to receive this event must
 * inherit from this interface and implement all defined methods.
 *
 * For example, the interface ExampleEventIface defines a single function:
 * OnExampleEvent. In order to accept events of that kind, ExampleEvent
 * inherits from both UnityEngine.MonoBehaviour and ExampleEventIface, and
 * implements OnExampleEvent. To send this event, ExampleEventSender simply
 * issueEvent<ExampleEventIface> with the lambda
 * '(x,y) => x.OnExampleEvent(...)'. The first parameter for that lambda is
 * the component that received the event, while the second parameter is
 * simply ignored. The parameters sent to the event are captured into the
 * lambda.
 *
 * An important detail of this method is that events can only be propagated
 * upward. Care must be taken to place senders as far within nested objects
 * as necessary. However, this does simplify a lot of interactions. To
 * implement a damage system, instead of having to grab a specific component
 * from the colliding object, one could simply send a "Hurt(damage)" event.
 * The opposite, adding logic to the object hurt itself on collision, could
 * also be implemented by using 'out' parameters. In this alternative method,
 * the object that will be hurt would send an event "GetDamage(out damage)".
 * If no object handled this event, 'damage' would be left unchanged.
 *
 * The root game object is supposed to be a common game object that oversees
 * the game. It can be used to play music, advance to the next stage, track
 * the score and so on. Again, the advantage of using this is that the root
 * object could register itself and then other objects wouldn't need to know
 * which object implements those functionalities. Otherwise, the sending
 * game object would need to have a public Transform or GameObject, which
 * makes every component way more tightly coupled.
 */
public class BaseRemoteAction : UnityEngine.MonoBehaviour {
	static private GO root = null;

	/**
	 * Set the current root target, so objects may call this.rootEvent<...>(...)
	 * without having to know which is the scene's root target.
	 *
	 * @param target: The scene's root target
	 */
	static public void setRootTarget(GO target) {
		BaseRemoteAction.root = target;
	}

	/**
	 * Send an event upwards. If no target is specified, the event is sent to
	 * the object itself.
	 *
	 * @param cb: The event being sent
	 * @param customTarget: The event receiver, if any
	 */
	protected void issueEvent<T>(ExecEv.EventFunction<T> cb,
			GO customTarget = null) where T : Handler {
		if (customTarget != null)
			ExecEv.ExecuteHierarchy<T>(customTarget, null, cb);
		else
			ExecEv.ExecuteHierarchy<T>(this.gameObject, null, cb);
	}

	/**
	 * Send an event to the root game object (which must be manually set).
	 *
	 * @param cb: The event being sent
	 */
	protected void rootEvent<T>(ExecEv.EventFunction<T> cb) where T : Handler {
		ExecEv.ExecuteHierarchy<T>(root, null, cb);
	}
}
