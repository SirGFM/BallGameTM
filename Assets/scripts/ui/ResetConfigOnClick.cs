using GO = UnityEngine.GameObject;

public class ResetConfigOnClick : UnityEngine.MonoBehaviour {
#if !UNITY_WEBGL
    void Start() {
        GO.Destroy(this.gameObject);
    }
#endif

    public void OnClick() {
        try {
            Config.reset();
            Config.load();
            Global.Sfx.playVictory();
        } catch (System.Exception) {
            Global.Sfx.playDefeat();
        }
    }
}
