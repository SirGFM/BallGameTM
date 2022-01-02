using CoroutineRet = System.Collections.IEnumerator;
using DefInput = UnityEngine.Input;
using KeyCode = UnityEngine.KeyCode;
using GO = UnityEngine.GameObject;
using UEMath = UnityEngine.Mathf;
using Vec2 = UnityEngine.Vector2;

using Int32 = System.Int32;
using GroupCollection = System.Text.RegularExpressions.GroupCollection;
using MatchCollection = System.Text.RegularExpressions.MatchCollection;
using Regex = System.Text.RegularExpressions.Regex;
using CultureInfo = System.Globalization.CultureInfo;

/**
 * ========================================================================
 * ------------------------------------------------------------------------
 * |   CUSTOM INPUT MANAGER                                               |
 * ------------------------------------------------------------------------
 * ========================================================================
 *
 * === DISCLAIMER =========================================================
 *
 * This is a mess... and it's only partially my fault. I haven't checked
 * how, if at all, Unity's input manager has improved since Unity 2019...
 * Honestly, at this point I'm just sticking with my own personal
 * solution, because it works.
 *
 * === INTRODUCTION =======================================================
 *
 * The static class Input implements an input manager. This class has
 * accessors for every in-game action, as well as mechanisms to remap
 * inputs and to stabilize axis. This last point was originally implemented
 * for Mystery Tower, when someone had issues using a Playstation gamepad
 * because some of its axis rest at 0.5f and varies between [0.0f, 1.0f]
 * and Unity doesn't automatically account for that.
 *
 * Unity's input system is exclusively used to get inputs from gamepads, as
 * I couldn't find any way to poll the state of a specific gamepad. So, the
 * Unity's input manager must have a number of pre-configured gamepads
 * inputs as named buttons ('joystick <NUMBER> button <NUMBER>') and named
 * axis ('joystick <NUMBER> axis <NUMBER>'). Then, this custom input manager
 * simply retrieve the status of any given action from those names (e.g.,
 * calling UnityEngine.Input.GetAxis('joystick 0 axis 0')). An important
 * point is that 'joystick 0' is configured to received inputs from every
 * connected gamepad.
 *
 * The pre-configured gamepad inputs in Unity's input manager may be
 * generated with the Python script gen-input.py. This script has a
 * hard-coded number of gamepads, buttons per gamepad and axis per gamepad.
 * This Input system must match these same values in the constants
 * Input.gamepadNum, Input.gamepadAxisNum and Input.gamepadButtonNum.
 * These are mainly used to poll over the correct inputs when trying to
 * remap an input.
 *
 * Keyboard keys are polled using UnityEngine.KeyCode and the GetKey()
 * method from UnityEngine.Input.
 *
 * === GAME ACTIONS =======================================================
 *
 * TL;DR: To define the actions in your game, edit the Actions enumeration
 * and change the mapping to valid default inputs (axis0, axis1 and axis2).
 *
 * This input manager works by creating "actions". These are listed in the
 * public enumeration Input.Actions. These actions are mapped into input
 * through the private class Input.axis (which people using this input
 * manager most likely doesn't need to know about). Those two are combined
 * into multiple arrays of Input.axis, to have multiple keybinds (like
 * keyboard and gamepad) at the same time, where each index represent an
 * action, identified by the position in the array.
 *
 * To simplify converting an Input.Actions to the index of an action in an
 * array, ActionsMethods implements the method idx() for this enumeration.
 * Thus, the following is valid:
 *
 *     axis[] mapping = {
 *         new axis( SOME_VALUE ), // Input.Actions.Left
 *         new axis( SOME_OTHER_VALUE ), // Input.Actions.Right
 *         // ...
 *     };
 *
 *     axis left = mapping[Input.Actions.Left.idx()];
 *
 * All of that because C# can't use enumeration as integers... Which, for
 * someone who writes a lot of C code, is quite annoying.
 *
 * === REMAPPING INPUTS ===================================================
 *
 * NOTE: Unity is quite finicky with inputs. When a scene is loaded, every
 * input is zeroed-out. Therefore, trying to train an input in this state
 * will cause lots of issues. However, if the user moved every axis around
 * before interactively assigning each axis, this should work without any
 * problem.
 *
 * Before starting to query for an input, it's important to be sure that no
 * key is pressed. Input.TrainAxisStable() reports whether all detected
 * gamepad axis are stable and try to figure out the each axis resting
 * position, so it's important to call this function a few times before
 * starting to remap inputs. This function may end up causing input to be
 * misconfigured, if an analog is held in a direction when this function is
 * called, though! Additionally, Input.CheckAnyKeyDown() reports whether
 * any key or button is down. Therefore, before remaoping inputs, be sure
 * to call something like:
 *
 *     private System.Collections.IEnumerator WaitNoInput() {
 *         uint trainNum = 3;
 *         while (!Input.TrainAxisStable() || Input.CheckAnyKeyDown() ||
 *                     trainNum > 0) {
 *             yield return null;
 *             if (trainNum > 0) {
 *                 trainNum--;
 *             }
 *         }
 *         yield return null;
 *     }
 *
 * To reconfigure a single action, use Input.WaitInput(). This function
 * spawns a coroutine that waits for any input and overwrites the action
 * when it detects an input. The 'column' parameter determines which of the
 * multiple mappings will be edited and the 'caller' is the GameObject that
 * will execute the coroutine for remapping the input. This GameObject
 * needs a KeyLogger component, but Input.WaitInput() adds a new KeyLogger
 * component if the supplied object doesn't have one.
 *
 * This coroutine may be controlled from another object by calling either
 * Input.IsWaitingInput() or Input.CancelWaitInput(). The former returns
 * whether or not the coroutine has finished running and a new input was
 * mapped to the action, and the latter cancels the remapping, keeping the
 * action unchanged.
 *
 * To display the current mappings, use Input.AxisName() specifying the
 * desired mapping in the column parameter
 *
 * To clear an input, use Input.ClearAxis() specifying the desired mapping
 * in the column parameter. Also, to revert an input back to its original,
 * hard-coded mapping use Input.RevertMap() specifying the desired mapping
 * in the column parameter.
 *
 * === PERSISTENT REMAPPINGS ==============================================
 *
 * The Input class always starts with the hard-coded input mappings. To
 * change this and make custom mappings persistent, the mapping must be
 * serialized and saved to a file.
 *
 * Serialization and deserialization may be done by calling
 * Input.axisToJson() and Input.axisFromJson(), respectively. Both
 * functions take the input being saved/or loaded as the 'column'
 * parameter. Note that Input.axisFromJson() throws an exception on error!
 *
 * One built-in option to save these values is to use Unity's
 * UnityEngine.PlayerPrefs. The serialized mapping may be saved or loaded
 * as a string, calling UnityEngine.PlayerPrefs.SetString() and
 * UnityEngine.PlayerPrefs.GetString(), respectively.
 *
 */

public static class ActionsMethods {
	public static int idx(this Input.Actions a) {
		return (int)a;
	}
}

static public class Input {
	private const int gamepadNum = 9;
	private const int gamepadAxisNum = 10;
	private const int gamepadButtonNum = 20;

	public enum Actions {
		Left = 0,
		Right,
		Up,
		Down,
		Action,
		Reset,
		Pause,
		MouseCamera,
		CameraLeft,
		CameraRight,
		CameraUp,
		CameraDown,
		ResetCamera,
		NumActions,
	};

	private enum axisType {
		positiveAxis = 0,
		negativeAxis,
		none,
	};

	private class axis {
		string input;
		KeyCode key;
		axisType type;
		bool isKey;
		string name;
		float rest;

		private axis() {
			/* Empty constructor */
		}

		static private Regex jsonParser = null;
		static public axis fromJson(string json) {
			if (jsonParser == null)
				jsonParser = new Regex("\"input\": \"(.*)\",.*" +
						"\"key\": \"(.*)\",.*" +
						"\"type\": \"(.*)\",.*" +
						"\"isKey\": \"(.*)\",.*" +
						"\"name\": \"(.*)\",.*" +
						"\"rest\": \"(.*)\"");
			MatchCollection matches = jsonParser.Matches(json);
			GroupCollection groups = matches[0].Groups;

			string input = groups[1].Value;
			int ikey = Int32.Parse(groups[2].Value);
			KeyCode key = (KeyCode)ikey;
			int itype = Int32.Parse(groups[3].Value);
			axisType type = (axisType)itype;
			int iIsKey = Int32.Parse(groups[4].Value);
			bool isKey = (iIsKey != 0);
			string name = groups[5].Value;
			float rest = float.Parse(groups[6].Value, CultureInfo.InvariantCulture);

			axis a = new axis();
			a.input = input;
			a.key = key;
			a.type = type;
			a.isKey = isKey;
			a.name = name;
			a.rest = rest;
			return a;
		}

		public string toJson() {
			int ikey = (int)this.key;
			int itype = (int)this.type;
			int iIsKey = this.isKey ? 1 : 0;

			return $"\"input\": \"{this.input}\"," +
				$"\"key\": \"{ikey}\"," +
				$"\"type\": \"{itype}\"," +
				$"\"isKey\": \"{iIsKey}\"," +
				$"\"name\": \"{this.name}\","+
				$"\"rest\": \"{this.rest:0.###}\"";
		}

		private void initGamepad(string input, axisType type, float rest) {
			this.type = type;
			this.input = input;
			this.isKey = false;
			this.rest = rest;

			this.key = KeyCode.None;

			if (this.type == axisType.positiveAxis)
				this.name = $"{this.input} +";
			else if (this.type == axisType.negativeAxis)
				this.name = $"{this.input} -";
			else
				this.name = this.input;
		}

		public axis(string input, axisType type, float rest) {
			this.initGamepad(input, type, rest);
		}

		public axis(string input, axisType type) {
			this.initGamepad(input, type, 0.0f);
		}

		public axis(KeyCode key) {
			this.key = key;
			this.isKey = true;

			this.type = axisType.none;
			this.input = "";

			this.name = $"Key: {this.key}";
		}

		override public string ToString() {
			return this.name;
		}

		private float key2axis() {
			if (DefInput.GetKey(this.key))
				return 1.0f;
			else
				return 0.0f;
		}

		private float getAxisPerc(float val) {
			float diff = val - this.rest;

			if ((this.type == axisType.negativeAxis && diff > 0) ||
					(this.type == axisType.positiveAxis && diff < 0)) {
				return 0.0f;
			}

			if (val < this.rest)
				return UEMath.Abs(diff / (1.0f + rest));
			else if (val > this.rest)
				return diff / (1.0f - rest);
			else
				return 0.0f;
		}

		private float axisVal2axis(float val) {
			float perc = this.getAxisPerc(val);

			if ((val < this.rest && this.type != axisType.negativeAxis) ||
					(val > this.rest && this.type == axisType.negativeAxis) ||
					perc < 0.5f)
				return 0.0f;
			return perc;
		}

		public float GetAxis() {
			if (this.isKey)
				return this.key2axis();
			else
				return this.axisVal2axis(DefInput.GetAxis(this.input));
		}

		public float GetAxisRaw() {
			if (this.isKey)
				return this.key2axis();
			else
				return this.axisVal2axis(DefInput.GetAxisRaw(this.input));
		}

		public bool GetButton() {
			return this.GetAxisRaw() > 0.5f;
		}

		public bool GetButtonJustPressed() {
			if (this.isKey)
				return DefInput.GetKeyDown(this.key);
			else
				return DefInput.GetButtonDown(this.input);
		}
	}; /* private class axis */

	static private axis[] axis0 = {
		new axis(KeyCode.A) /* Left */,
		new axis(KeyCode.D) /* Right */,
		new axis(KeyCode.W) /* Up */,
		new axis(KeyCode.S) /* Down */,
		new axis(KeyCode.Space) /* Action */,
		new axis(KeyCode.R) /* Reset */,
		new axis(KeyCode.Escape) /* Pause */,
		new axis(KeyCode.Mouse1) /* MouseCamera */,
		new axis(KeyCode.H) /* CameraLeft */,
		new axis(KeyCode.K) /* CameraRight */,
		new axis(KeyCode.U) /* CameraUp */,
		new axis(KeyCode.J) /* CameraDown */,
		new axis(KeyCode.Mouse2) /* ResetCamera */,
	};

	static private axis[] axis1 = {
		new axis("joystick 0 axis 0", axisType.negativeAxis) /* Left */,
		new axis("joystick 0 axis 0", axisType.positiveAxis) /* Right */,
		new axis("joystick 0 axis 1", axisType.negativeAxis) /* Up */,
		new axis("joystick 0 axis 1", axisType.positiveAxis) /* Down */,
		new axis("joystick 0 button 0", axisType.none) /* Action */,
		new axis("joystick 0 button 3", axisType.none) /* Reset */,
		new axis("joystick 0 button 7", axisType.none) /* Pause */,
		null /* MouseCamera */,
		new axis("joystick 0 axis 3", axisType.negativeAxis) /* CameraLeft */,
		new axis("joystick 0 axis 3", axisType.positiveAxis) /* CameraRight */,
		new axis("joystick 0 axis 4", axisType.negativeAxis) /* CameraUp */,
		new axis("joystick 0 axis 4", axisType.positiveAxis) /* CameraDown */,
		new axis("joystick 0 button 5", axisType.none) /* ResetCamera */,
	};

	static private axis[] axis2 = {
		new axis("joystick 0 axis 6", axisType.negativeAxis) /* Left */,
		new axis("joystick 0 axis 6", axisType.positiveAxis) /* Right */,
		new axis("joystick 0 axis 7", axisType.positiveAxis) /* Up */,
		new axis("joystick 0 axis 7", axisType.negativeAxis) /* Down */,
		null /* Action */,
		null /* Reset */,
		null /* Pause */,
		null /* MouseCamera */,
		null /* CameraLeft */,
		null /* CameraRight */,
		null /* CameraUp */,
		null /* CameraDown */,
		null /* ResetCamera */,
	};

	/* =======================================================================
	 *   Remapper accessors
	 * =======================================================================*/

	static private float _combineAxis(axis[] arr, int pos, int neg) {
		if (pos >= arr.Length || neg >= arr.Length ||
				arr[pos] == null || arr[neg] == null)
			return 0.0f;
		return arr[pos].GetAxis() - arr[neg].GetAxis();
	}

	static private float combineAxis(Actions pos, Actions neg) {
		float val;
		val = _combineAxis(axis0, pos.idx(), neg.idx());
		if (val == 0.0f)
			val = _combineAxis(axis1, pos.idx(), neg.idx());
		if (val == 0.0f)
			val = _combineAxis(axis2, pos.idx(), neg.idx());
		return val;
	}

	static private float _combineAxisRaw(axis[] arr, int pos, int neg) {
		if (pos >= arr.Length || neg >= arr.Length ||
				arr[pos] == null || arr[neg] == null)
			return 0.0f;
		return arr[pos].GetAxisRaw() - arr[neg].GetAxisRaw();
	}

	static private float combineAxisRaw(Actions pos, Actions neg) {
		float val;
		val = _combineAxisRaw(axis0, pos.idx(), neg.idx());
		if (val == 0.0f)
			val = _combineAxisRaw(axis1, pos.idx(), neg.idx());
		if (val == 0.0f)
			val = _combineAxisRaw(axis2, pos.idx(), neg.idx());
		return val;
	}

	static private bool _getButton(axis[] arr, int bt) {
		if (bt >= arr.Length || arr[bt] == null)
			return false;
		return arr[bt].GetButton();
	}

	static private bool combineButton(Actions bt) {
		int idx = bt.idx();
		return _getButton(axis0, idx) || _getButton(axis1, idx) ||
			_getButton(axis2, idx);
	}

	static private bool _getButtonJP(axis[] arr, int bt) {
		if (bt >= arr.Length || arr[bt] == null)
			return false;
		return arr[bt].GetButtonJustPressed();
	}

	static private bool combineButtonJustPressed(Actions bt) {
		int idx = bt.idx();
		return _getButtonJP(axis0, idx) || _getButtonJP(axis1, idx) ||
			_getButtonJP(axis2, idx);
	}

	/* =======================================================================
	 *   Remapping helpers
	 * =======================================================================*/

	static private axis[] getArr(int column) {
		switch (column) {
		case 0:
			return axis0;
		case 1:
			return axis1;
		case 2:
			return axis2;
		default:
			throw new System.Exception($"Invalid input column ({column})");
		}
	}

	static private float[] axisRest = null;

	/**
	 * Reset the axis training, so it may be executed again from scratch.
	 */
	static public void ResetAxisTraining() {
		Input.axisRest = null;
	}

	/**
	 * Check every axis on every gamepad to ensure that everything is
	 * as stable as possible.
	 *
	 * @param trainDefaultGamepad: Whether the default gamepad (which may
	 *                             detect input from any gamepad) should
	 *                             also be trained.
	 *
	 * @return true if the axis are stable, false otherwise
	 */
	static public bool TrainAxisStable(bool trainDefaultGamepad = false) {
		bool stable = true;

		if (Input.axisRest == null) {
			Input.axisRest = new float[gamepadNum * gamepadAxisNum];
			for (int i = 0; i < gamepadNum * gamepadAxisNum; i++) {
				Input.axisRest[i] = 0.0f;
			}
		}

		for (int gpIdx = 0; gpIdx < gamepadNum; gpIdx++) {
			if (!trainDefaultGamepad && gpIdx == 0) {
				continue;
			}
			trainDefaultGamepad = false;

			for (int gpAxis = 0; gpAxis < gamepadAxisNum; gpAxis++) {
				int i = gpIdx * gamepadAxisNum + gpAxis;
				string name = $"joystick {gpIdx} axis {gpAxis}";
				float cur = DefInput.GetAxisRaw(name);
				float rest = Input.axisRest[i];
				float diff = UEMath.Abs(rest - cur);

				Input.axisRest[i] = cur * 0.75f + rest * 0.25f;
				stable = (stable && diff < 0.05f);
			}
		}

		return stable;
	}

	static public bool CheckAnyKeyDown() {
		return UnityEngine.Input.anyKey;
	}

	static public bool CheckAnyKeyJustPressed() {
		return UnityEngine.Input.anyKeyDown;
	}

	static private UnityEngine.Coroutine waitFunc = null;
	static private KeyLogger waitCaller = null;

	static private CoroutineRet _waitInput(axis[] arr, Actions action) {
		int idx = action.idx();
		bool done = false;

		while (!done) {
			/* Wait until the end of the next frame */
			yield return null;

			if (waitCaller.lastKey != KeyCode.None) {
				arr[idx] = new axis(waitCaller.lastKey);
				done = true;
				break;
			}
			else {
				/* Test every option in every gamepad :grimacing: */
				for (int gpIdx = 1; !done && gpIdx < gamepadNum; gpIdx++) {
					for (int gpAxis = 0; gpAxis < gamepadAxisNum; gpAxis++) {
						string name = $"joystick {gpIdx} axis {gpAxis}";
						int i = gpIdx * gamepadAxisNum + gpAxis;
						float rest = Input.axisRest[i];
						float val = DefInput.GetAxisRaw(name);
						float diff = val - rest;

						/* Check that the axis is 80% of the way pressed
						 * in the given direction */
						if (val > rest && diff > 0.25f &&
								diff / (1.0f - rest) >= 0.8f) {
							arr[idx] = new axis(name, axisType.positiveAxis, rest);
							done = true;
							break;
						}
						else if (val < rest && diff < -0.25f &&
								UEMath.Abs(diff / (1.0f + rest)) >= 0.8f) {
							arr[idx] = new axis(name, axisType.negativeAxis, rest);
							done = true;
							break;
						}
					}
					for (int gpBt = 0; gpBt < gamepadButtonNum; gpBt++) {
						string name = $"joystick {gpIdx} button {gpBt}";
						if (DefInput.GetButton(name)) {
							arr[idx] = new axis(name, axisType.none);
							done = true;
							break;
						}
					}
				}
			}
		}

		waitFunc = null;
		waitCaller.GetComponentInChildren<KeyLogger>().enabled = false;
		waitCaller = null;
	}

	static public void WaitInput(GO caller, int column, Actions action) {
		if (waitFunc != null || waitCaller != null)
			return;

		axis[] arr = getArr(column);

		KeyLogger kl = caller.GetComponentInChildren<KeyLogger>();
		if (kl == null)
			kl = caller.AddComponent<KeyLogger>();
		kl.enabled = true;

		waitCaller = kl;
		waitFunc = kl.StartCoroutine(_waitInput(arr, action));
	}

	static public void CancelWaitInput() {
		if (waitFunc == null || waitCaller == null)
			return;

		waitCaller.StopCoroutine(waitFunc);
		waitFunc = null;
		waitCaller.GetComponentInChildren<KeyLogger>().enabled = false;
		waitCaller = null;
	}

	static public bool IsWaitingInput() {
		return (waitFunc != null);
	}

	static public void ClearAxis(Actions action, int column) {
		axis[] arr = getArr(column);

		if (action.idx() < arr.Length && arr[action.idx()] != null)
			arr[action.idx()] = null;
	}

	static public string AxisName(Actions action, int column) {
		axis[] arr = getArr(column);

		if (action.idx() < arr.Length && arr[action.idx()] != null)
			return arr[action.idx()].ToString();
		return "";
	}

	static public void RevertMap(int column) {
		if (Input.axisRest == null) {
			Input.TrainAxisStable();
		}

		switch (column) {
		case 0:
			axis[] _axis0 = {
				new axis(KeyCode.A) /* Left */,
				new axis(KeyCode.D) /* Right */,
				new axis(KeyCode.W) /* Up */,
				new axis(KeyCode.S) /* Down */,
				new axis(KeyCode.Space) /* Action */,
				new axis(KeyCode.R) /* Reset */,
				new axis(KeyCode.Escape) /* Pause */,
				new axis(KeyCode.Mouse1) /* MouseCamera */,
				new axis(KeyCode.H) /* CameraLeft */,
				new axis(KeyCode.K) /* CameraRight */,
				new axis(KeyCode.U) /* CameraUp */,
				new axis(KeyCode.J) /* CameraDown */,
				new axis(KeyCode.Mouse2) /* ResetCamera */,
			};
			axis0 = _axis0;
			break;
		case 1:
			axis[] _axis1 = {
				new axis("joystick 0 axis 0", axisType.negativeAxis, Input.axisRest[0]) /* Left */,
				new axis("joystick 0 axis 0", axisType.positiveAxis, Input.axisRest[0]) /* Right */,
				new axis("joystick 0 axis 1", axisType.negativeAxis, Input.axisRest[1]) /* Up */,
				new axis("joystick 0 axis 1", axisType.positiveAxis, Input.axisRest[1]) /* Down */,
				new axis("joystick 0 button 0", axisType.none) /* Action */,
				new axis("joystick 0 button 3", axisType.none) /* Reset */,
				new axis("joystick 0 button 7", axisType.none) /* Pause */,
				null /* MouseCamera */,
				new axis("joystick 0 axis 3", axisType.negativeAxis, Input.axisRest[3]) /* CameraLeft */,
				new axis("joystick 0 axis 3", axisType.positiveAxis, Input.axisRest[3]) /* CameraRight */,
				new axis("joystick 0 axis 4", axisType.negativeAxis, Input.axisRest[4]) /* CameraUp */,
				new axis("joystick 0 axis 4", axisType.positiveAxis, Input.axisRest[4]) /* CameraDown */,
				new axis("joystick 0 button 5", axisType.none) /* ResetCamera */,
			};
			axis1 = _axis1;
			break;
		case 2:
			axis[] _axis2 = {
				new axis("joystick 0 axis 6", axisType.negativeAxis, Input.axisRest[6]) /* Left */,
				new axis("joystick 0 axis 6", axisType.positiveAxis, Input.axisRest[6]) /* Right */,
				new axis("joystick 0 axis 7", axisType.negativeAxis, Input.axisRest[7]) /* Up */,
				new axis("joystick 0 axis 7", axisType.positiveAxis, Input.axisRest[7]) /* Down */,
				null /* Action */,
				null /* Reset */,
				null /* Pause */,
				null /* MouseCamera */,
				null /* CameraLeft */,
				null /* CameraRight */,
				null /* CameraUp */,
				null /* CameraDown */,
				null /* ResetCamera */,
			};
			axis2 = _axis2;
			break;
		default:
			throw new System.Exception("Invalid input map");
		}
	}

	static private string _axisToJson(axis[] _axis) {
		string json = "[";
		for (int i = 0; i < _axis.Length; i++) {
			json += "{";
			if (_axis[i] != null)
				json += _axis[i].toJson();
			json += "},";
		}
		json += "]";

		return json;
	}

	static public string axisToJson(int column) {
		switch (column) {
		case 0:
			return _axisToJson(axis0);
		case 1:
			return _axisToJson(axis1);
		case 2:
			return _axisToJson(axis2);
		default:
			throw new System.Exception("Invalid input map");
		}
	}

	static private Regex jsonParser = null;
	static public void axisFromJson(int column, string json) {
		if (jsonParser == null)
			jsonParser = new Regex("{([^}]*)},+");
		MatchCollection matches = jsonParser.Matches(json);
		if (matches.Count != (int)Actions.NumActions)
			throw new System.Exception("Invalid number of actions in the configuration file");

		axis[] _axis = new axis[matches.Count];
		for (int i = 0; i < matches.Count; i++) {
			int len = matches[i].Value.Length - 3;
			string subJson = matches[i].Value.Substring(1, len);

			if (subJson.Length > 0)
				_axis[i] = axis.fromJson(subJson);
			else
				_axis[i] = null;
		}

		switch (column) {
		case 0:
			axis0 = _axis;
			break;
		case 1:
			axis1 = _axis;
			break;
		case 2:
			axis2 = _axis;
			break;
		default:
			throw new System.Exception("Invalid input map");
		}
	}

	/* =======================================================================
	 *   Controller getters
	 * =======================================================================*/

	static private Vec2 _getNormalizedMovement(Actions left, Actions right,
			Actions up, Actions down) {
		float x, y;

		x = combineAxis(right, left);
		y = combineAxis(up, down);

		Vec2 v = new Vec2(x, y);
		return v.normalized;
	}

	static public Vec2 GetMovement() {
		return Input._getNormalizedMovement(Actions.Left, Actions.Right,
				Actions.Up, Actions.Down);
	}

	static public float GetHorizontalAxis() {
		return combineAxis(Actions.Right, Actions.Left);
	}

	static public float GetVerticalAxis() {
		return combineAxis(Actions.Up, Actions.Down);
	}

	static public bool GetActionButton() {
		return combineButton(Actions.Action);
	}

	static public bool GetResetButton() {
		return combineButtonJustPressed(Actions.Reset);
	}

	static public bool GetPauseDown() {
		return combineButton(Actions.Pause);
	}

	static public bool GetPauseJustPressed() {
		return combineButtonJustPressed(Actions.Pause);
	}

	static public bool GetMouseCameraEnabled() {
		return combineButton(Actions.MouseCamera);
	}

	static public Vec2 GetCamera() {
		return Input._getNormalizedMovement(Actions.CameraLeft,
				Actions.CameraRight, Actions.CameraUp, Actions.CameraDown);
	}

	static public float GetCameraX() {
		return combineAxis(Actions.CameraRight, Actions.CameraLeft);
	}

	static public float GetCameraY() {
		return combineAxis(Actions.CameraUp, Actions.CameraDown);
	}

	static public UnityEngine.Vector3 GetMousePosition() {
		return UnityEngine.Input.mousePosition;
	}

	static public bool GetResetCameraDown() {
		return combineButton(Actions.ResetCamera);
	}

	static public bool GetResetCameraJustPressed() {
		return combineButtonJustPressed(Actions.ResetCamera);
	}

	/* =======================================================================
	 *   Menu getters
	 * =======================================================================*/

	static public bool MenuLeft() {
		return DefInput.GetAxisRaw("joystick 0 axis 0") < -0.7f ||
			DefInput.GetKey("left") ||
			Input.GetHorizontalAxis() < -0.7f;
	}

	static public bool MenuRight() {
		return DefInput.GetAxisRaw("joystick 0 axis 0") > 0.7f ||
			DefInput.GetKey("right") ||
			Input.GetHorizontalAxis() > 0.7f;
	}

	static public bool MenuUp() {
		return DefInput.GetAxisRaw("joystick 0 axis 1") < -0.7f ||
			DefInput.GetKey("up") ||
			Input.GetVerticalAxis() > 0.7f;
	}

	static public bool MenuDown() {
		return DefInput.GetAxisRaw("joystick 0 axis 1") > 0.7f ||
			DefInput.GetKey("down") ||
			Input.GetVerticalAxis() < -0.7f;
	}

	static public bool MenuSelect() {
		return DefInput.GetAxisRaw("joystick 0 button 0") > 0.7f ||
			DefInput.GetKey("return") ||
			Input.GetActionButton();
	}

	static public bool MenuCancel() {
		return DefInput.GetAxisRaw("joystick 0 button 1") > 0.7f ||
			DefInput.GetKey("escape");
	}
}
