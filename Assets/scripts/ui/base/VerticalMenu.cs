/**
 * VerticalMenu implements a generic vertical menu.
 *
 * UI scenes with vertical menus (say, the main menu) may implement custom
 * behaviour by overriding the function updateSelected(), called whenever
 * the menu moves to a new position. The number of entries must be defined
 * by overriding the function getNumberOfOptions().
 */

public class VerticalMenu : Menu {
    private int curOpt;

    virtual protected int getNumberOfOptions() {
        return 0;
    }

    protected int getCurrentOpt() {
        return this.curOpt;
    }

    virtual protected void updateSelected() {
    }

    override protected void onDown() {
        this.curOpt++;
        if (this.curOpt >= this.getNumberOfOptions())
            this.curOpt = 0;
        this.updateSelected();
    }

    override protected void onUp() {
        this.curOpt--;
        if (this.curOpt < 0)
            this.curOpt = this.getNumberOfOptions() - 1;
        this.updateSelected();
    }

    override protected void start() {
        this.curOpt = 0;
        this.updateSelected();
    }
}
