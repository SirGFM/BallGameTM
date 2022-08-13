
public class LoseScene : WinLoseScene {
	protected override void onJustPressed() {
		this.rootEvent<LoaderIface>( (x,y) => x.OnReset() );
	}

	protected override void playOpeningSfx() {
		Global.Sfx.playDefeatOpening();
	}

	protected override void playSfx() {
		Global.Sfx.playDefeat();
	}
}
