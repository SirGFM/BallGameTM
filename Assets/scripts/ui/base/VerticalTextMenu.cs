using UiText = UnityEngine.UI.Text;

/**
 * VerticalTextMenu implements a vertical menu that controls a UI.
 *
 * The menu entries must be defined by overriding start(), and initializing
 * 'options' before calling base.start().
 *
 * To actually display the options, this component uses three text
 * elements:
 *
 *     - shadow: A slightly offset text to act as the shadow of the text.
 *     - unselected: All options, but more opaque/grayed out.
 *     - select: Displays only the currently selected option
 *
 * These three elements are managed by the component itself, and mustn't be
 * manually initialized!
 */

public class VerticalTextMenu : VerticalMenu {
    public UiText shadow;
    public UiText unselected;
    public UiText selected;

    protected string[] options;

    override protected int getNumberOfOptions() {
        return this.options.Length;
    }

    override protected void updateSelected() {
        string txt = "";

        for (int i = 0; i < this.options.Length; i++) {
            if (i == this.getCurrentOpt())
                txt += $"-- {this.options[i]} --\n";
            else
                txt += "\n";
        }

        selected.text = txt;
    }

    override protected void start() {
        string txt = "";

        foreach (string opt in this.options)
            txt += $"{opt}\n";

        shadow.text = txt;
        unselected.text = txt;

        base.start();
    }
}
