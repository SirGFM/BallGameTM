using App = UnityEngine.Application;
using Color = UnityEngine.Color;
using Image = UnityEngine.UI.Image;

public class MainMenu : VerticalTextMenu {
    private string[] _opts = {
        "New game",
        "Quit"
    };

    override protected void onSelect() {
        switch (this.getCurrentOpt()) {
        case 0:
            this.LoadLevel(1);
            break;
        case 1:
            App.Quit();
            break;
        }
    }

    override protected void start() {
        this.options = this._opts;
        base.start();
    }
}
