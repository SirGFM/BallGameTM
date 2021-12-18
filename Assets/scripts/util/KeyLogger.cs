using Event = UnityEngine.Event;
using KeyCode = UnityEngine.KeyCode;

/**
 * KeyLogged keep monitoring if any input was pressed since the last
 * OnGUI() event and assign the last key, if any, to 'lastKey'.
 *
 * This component must be manually disabled to stop monitoring keys.
 * Ideally, a component that needs to retrieve the last pressed key should
 * enable this component to wait for a key an disable it as soon as a key
 * was pressed.
 */

public class KeyLogger : UnityEngine.MonoBehaviour {
	public KeyCode lastKey;

	void OnGUI() {
		Event e = Event.current;

		if (e.isKey)
			this.lastKey = e.keyCode;
		else if (e.isMouse)
			switch (e.button) {
			case 0:
				this.lastKey = KeyCode.Mouse0;
				break;
			case 1:
				this.lastKey = KeyCode.Mouse1;
				break;
			case 2:
				this.lastKey = KeyCode.Mouse2;
				break;
			case 3:
				this.lastKey = KeyCode.Mouse3;
				break;
			case 4:
				this.lastKey = KeyCode.Mouse4;
				break;
			case 5:
				this.lastKey = KeyCode.Mouse5;
				break;
			case 6:
				this.lastKey = KeyCode.Mouse6;
				break;
			default:
				this.lastKey = KeyCode.None;
				break;
			}
		else
			this.lastKey = KeyCode.None;
	}
}
