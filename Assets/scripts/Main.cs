
/**
 * Main is one of the first few scripts executed by the game, responsible
 * for parsing command line arguments, loading the configuration etc.
 */

public class Main : UnityEngine.MonoBehaviour {

	void Start() {
		try {
			foreach (string arg in System.Environment.GetCommandLineArgs()) {
				if (arg == "--reset-config") {
					Config.reset();
				}
			}
		} catch (System.Exception) {
		}

		Config.load();
	}
}
