
public class LoseScene : WinLoseScene {
	protected override void onJustPressed() {
		this.rootEvent<LoaderIface>( (x,y) => x.OnReset() );
	}

	protected override void playOpeningSfx() {
		// TODO
	}

	protected override void playSfx() {
		// TODO
	}
}
