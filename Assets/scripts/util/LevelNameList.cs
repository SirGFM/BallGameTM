using CultureInfo = System.Globalization.CultureInfo;
using Scene = UnityEngine.SceneManagement.Scene;
using SceneMng = UnityEngine.SceneManagement.SceneManager;
using SceneUtil = UnityEngine.SceneManagement.SceneUtility;
using TextInfo = System.Globalization.TextInfo;

/**
 * LevelNameList caches every scene name on the build. It can either be
 * accessed directly, by calling GetLevel(), or by retrieving the array of
 * name, by calling Get().
 *
 * Since Unity uses the full path to the scene file as its name, this class
 * removes the directories from the level name, as well as the .unity
 * extension. Also, the class assumes that a level name always start with
 * a digit.
 *
 * If the name has any spaces, no extra conversion is done. Otherwise,
 * every word is capitalized, dashes ('-') are converted into spaces and
 * double dashes are converted into single dashes.
 *
 * So, a level like 'Assets/scenes/00-basic/00-first-steps.unity' is
 * converted into something like 'First Steps'. Meanwhile, a level named
 * something like 'Assets/scenes/00-basic/00-first steps.unity' wouldl be
 * converted into 'first steps'.
 */

static public class LevelNameList {
	/** Cache the list of level names. */
	static private string[] _list = null;

	/** Cache the list of bg scenes per level. */
	static private string[] _bgList = null;

	/**
	 * Convert the input name, a scene filename, into a level name.
	 *
	 * Everything until the first character is removed. Then, single
	 * dashes ('-') are converted into spaces and double dashes are
	 * converted into singles dashes. Lastly, every word is capitalized.
	 *
	 * However, if the filename already has spaces, no extra conversion is
	 * done and the filename is used as is after removing the initial
	 * number and the extension.
	 *
	 * @param baseName: The scene name, as retrieved from Unity.
	 * @return The processed name.
	 */
	static private string ProcessName(string baseName) {
		int start = baseName.LastIndexOf("/");
		int end = baseName.LastIndexOf(".");

		/* Extract the filename. */
		baseName = baseName.Substring(start + 1, end - start - 1);

		/* Remove dashes, converting it into spaces. */
		bool convert = (baseName.IndexOf(" ") == -1);
		if (convert) {
			baseName = baseName.Replace("--", "/");
			baseName = baseName.Replace("-", " ");
			baseName = baseName.Replace("/", "-");
		}

		/* Remove the initial number. */
		for (int i = 0; i < baseName.Length; i++) {
			if ((baseName[i] >= 'A' && baseName[i] <= 'Z') ||
					(baseName[i] >= 'a' && baseName[i] <= 'z')) {
				baseName = baseName.Substring(i);
				break;
			}
		}

		/* Capitalize every word. */
		if (convert) {
			TextInfo ti = new CultureInfo("en-US",false).TextInfo;
			baseName = ti.ToTitleCase(baseName);
		}

		return baseName;
	}

	/**
	 * Iterate over every filename, generating the internal list of level
	 * names.
	 */
	static private void UpdateList() {
		int i;
		int max = SceneMng.sceneCountInBuildSettings;

		for (i = 1; i < max; i++) {
			string name = SceneUtil.GetScenePathByBuildIndex(i);
			int pos = name.LastIndexOf("/");
			char first = name[pos + 1];

			/* Every level start with a number, so use this to find the
			 * number of levels */
			if (first < '0' && first > '9')
				break;
		}

		max = i;
		LevelNameList._list = new string[max];
		LevelNameList._bgList = new string[max];
		for (i = 1; i < max; i++) {
			string scene = SceneUtil.GetScenePathByBuildIndex(i);
			int bgEnd = scene.LastIndexOf("/");
			int bgStart = scene.Substring(0, bgEnd - 1).LastIndexOf("/");

			LevelNameList._list[i] = ProcessName(scene);

			/* The BG is simply named after the directory where the scene
			 * is located. */
			LevelNameList._bgList[i] = scene.Substring(bgStart + 1, bgEnd - bgStart - 1);
		}
	}

	/**
	 * Retrieve the list of level names.
	 *
	 * @return The list of level names.
	 */
	static public string[] Get() {
		if (LevelNameList._list == null)
			UpdateList();
		return LevelNameList._list;
	}

	/**
	 * Retrieve the name of a given level (starting at 1).
	 *
	 * @return The i-th level.
	 */
	static public string GetLevel(int i) {
		if (LevelNameList._list == null)
			UpdateList();
		if (i < LevelNameList._list.Length)
			return LevelNameList._list[i];
		return "Unknown";
	}

	/**
	 * Retrieve the name of the BG scene for a given level (starting at 1).
	 *
	 * @return The i-th level.
	 */
	static public string GetLevelBG(int i) {
		if (LevelNameList._bgList == null)
			UpdateList();
		if (i < LevelNameList._bgList.Length)
			return LevelNameList._bgList[i];
		return "Unknown";
	}
}
