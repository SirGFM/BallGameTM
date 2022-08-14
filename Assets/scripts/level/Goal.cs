using Col = UnityEngine.Collider;
using EvSys = UnityEngine.EventSystems;

/**
 * Goal signals to the scene that an entity has reached the goal, by
 * sending a OnGoal event.
 */

public interface GoalIface : EvSys.IEventSystemHandler {
	/**
	 * Signals an entity has reached the goal.
	 */
	void OnGoal();

	/**
	 * Signals an entity that the game must restart the current level.
	 */
	void OnRetryLevel();
}

public class Goal : BaseRemoteAction {

	void OnTriggerEnter(Col other) {
		rootEvent<GoalIface>( (x,y) => x.OnGoal() );
	}
}
