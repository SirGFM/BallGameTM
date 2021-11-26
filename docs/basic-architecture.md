# Basic Architecture

This is the foundation over which the entire game is built! It's not exactly essential for level editing, but it should be quite helpful to creating custom objects.

## Physics and events

BallGame™\* is a physics based ball game. Although a lot of the interactions are directly controlled by Unity's physics system, components communicate with each other through events (similarly to [MysteryTower](https://github.com/SirGFM/FallingBlocks/blob/master/docs/system/events.md)).

\* ™ actually stands for "Top Mystery". ;)

The main advantage of doing this is that a component may try to communicate with another component in a manner agnostic to this other component. Using Mystery Tower as an example:

> In Mystery Tower, the player may push multiple boxes at once. However, some special boxes cannot be pushed. Similarly, some special game objects (that don't appear in the released game) cannot be pushed as well. So, when the player wants to push a box it must first check if there are multiple adjacent boxes, and if there's any immovable object in this line.
>
> Instead of using ray tracing or iterating over every object in the scene, the player sends an event trying to push the desired box. When a box receives this event, it checks if there's anything nearby and sends another event to this nearby object, which repeats this procedure. If a box doesn't have a subsequent box, it simply gets pushed, and reports that it was pushed successfully. If it has an immovable object nearby, this object won't respond to the push event, which causes the box to report to the event that sent the push event that it couldn't be moved.

One example of this event system in BallGame™ is moving platforms. As far as I could tell, regular Rigidbody and colliders doesn't have friction. As such, if an object is on top of another object and the lower object get pushed, the top object will stand still and end up falling from the lower object. To solve this, the MovingPlatform component sends an OnPush event to touching objects. This way, although the top object is getting moved by an independent force (and not by the platform itself), it appears that the object is being carried by the platform.

## Collision layers

Instead of manually filtering which component interacts with which object (e.g., by retrieving the tag of the collided object), components usually act on any object they collide against. This could lead to undesired interactions, such as a wind field for the player pushing moving platforms away.

Those undesired interactions are filtered out through user-defined layers in Unity's collision system, which allows specifying which layers may collide with each other.

![Collision matrix for BallGame™](/docs/imgs/collision-layers.png)

The built-in layers (Default, TransfparentFX, Ignore Raycast, Water and UI) were left as is, and the following layers were created:

* LevelController: Layer to detect collision against LevelComponent in components that send events.
* LevelComponent: Layer to detect collision against LevelController in components that receive events.
* NoCollision: Layer that does not collide with anything else. Objects in this layer shouldn't even have a collider, but this exists here just in case...
* PlayerController: Layer to detect collision against PlayerComponent in components that send events.
* PlayerComponent: Layer to detect collision against PlayerController in components that receive events.
* Player: This layer simply collides with all default layers. This could be useful to avoid self-collision in case the player object were divided into multiple, non-nested objects.

LevelController and LevelComponent server the same purpose as PlayerController and PlayerComponent, but the former is for interaction between level elements and the latter is for interactions between the player and level elements. Objects that send events should usually be placed on the "Controller" layer, while objects that receive events should be on the "Component" layer.

Physical barriers (e.g. the floor or walls) should be on the Default layer.

## Event handling and object organization

To exemplify how events are handled in this game, let's imagine how a force field component that interacts only with player could be created.

To send events exclusively to the player, this game object must be on the PlayerController layer. Something like:

![Dummy force field gameObject](/docs/imgs/dummy-force-field.png)

This game object's sphere collider would only collide against the objects on the PlayerComponent layer. Then, the SetForce component would detect collisions and send a OnSetForce event to colliding objects on every frame.

In the player prefab, only the Affector object is on the PlayerComponent, which looks like:

![The main event receiver in the player prefab](/docs/imgs/player-event-receiver.png)

This object must have a kinematic Rigidbody, because of how Unity's physics work. However, it doesn't have any component that respond to event.

When the force field detect a collision with any object, it sends an event to the colliding object (calling [`UnityEngine.EventSystems.ExecuteEvents.ExecuteHierarchy<T>`](https://docs.unity3d.com/2017.4/Documentation/ScriptReference/EventSystems.ExecuteEvents.ExecuteHierarchy.html)). If the object doesn't know how to handle the received event, it sends the event to its parent, repeating this process until an object handles the event or there are no more objects in the hierarchy, in which case the event is simply ignored.

In the player's case, these events are handled by Ball component:

![The player main game object, which handle events](/docs/imgs/player-controller.png)

So, although the ForceField would collide against the Ball's Affector, the event would end up getting handled by the BallController.
