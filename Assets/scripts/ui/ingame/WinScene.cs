
public class WinScene : WinLoseScene {
	protected override void onJustPressed() {
		this.rootEvent<GoalIface>( (x,y) => x.OnAdvanceLevel() );
	}

	protected override void playOpeningSfx() {
		Global.Sfx.playVictoryOpening();
	}

	protected override void playSfx() {
		Global.Sfx.playVictory();
	}
}
