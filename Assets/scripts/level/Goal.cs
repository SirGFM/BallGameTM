using Col = UnityEngine.Collider;
using EvSys = UnityEngine.EventSystems;

/**
 * Goal signals to the scene that an entity has reached the goal, by
 * sending a OnGoal event.
 */

public interface BasicGoalIface : EvSys.IEventSystemHandler {
	/**
	 * Signals an entity has reached the goal.
	 */
	void OnGoal();
}

public interface GoalIface : BasicGoalIface {
	/**
	 * Signals an entity that the game should go to the next level.
	 */
	void OnAdvanceLevel();

	/**
	 * Signals an entity that the game must restart the current level.
	 */
	void OnRetryLevel();
}

public class Goal : BaseRemoteAction {

	void OnTriggerEnter(Col other) {
		/** Trigger the goal on the game, advancing to the next stage. */
		rootEvent<GoalIface>( (x,y) => x.OnGoal() );

		/** Trigger any effect on the object itself. */
		issueEvent<BasicGoalIface>( (x,y) => x.OnGoal() );
	}
}
