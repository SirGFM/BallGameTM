# Making Levels

## Basic scene organization

**NOTE**: This may change a bit depending on how level loading is implemented!

Every level must have at least the following elements:

* Light source: the level's light source.
* Root: A root object, so global event work properly.
* Ball: The player object.
* Goal: The level's objective.

![Basic level elements](/docs/imgs/basic-level.png)

Currently, every level uses the same directional light, but this could change. Honestly, as long as everything is visible and not too resource intensive, it should be fine...

For the other three objects, they should be the same prefabs in every level. At some point, these prefabs may change depending on loader scene, but using a prefab should cause these changes to be transparent to the existing levels.

On this note, the lack of a camera is intentional, as it should come from the loader level. This shall be useful in case the background is rendered to one camera, and the game is rendered to a different camera and on top of the result of the first one.

On the image above, every static element in within the Floor game object. This is so these elements may be marked as "Static" for Unity, which could be better... I guess? I mainly did it like so that was an option! This is completely optional, but I do find it helpful to have stuff organized into "containers".

## Components

### Triangles and Ramps

**Update:** Thanks to [GabrielSilva584](https://github.com/SirGFM/BallGameTM/pull/1), these primitives now use a Mesh Collider and thus can be scaled freely.

Unity does not provide any default sloped object. Ramps could be created by using rotated cubes, but that leaves some geometry dangling beneath the ramp. To solve that, and to allow the construction of more interesting geometry, three prefabs were created: Triangles, Ramps and "Inner Triangles":

![Half-sphere created from Ramps, Triangles and Cubes](/docs/imgs/half-sphere.png)

To create geometry like this half-sphere above, Ramps and Triangles are more than enough. However, the "Inner Triangle", a dented cube, was needed to allow "outward slopes", as exemplified on the image bellow:

![Geometry created from Ramps, Triangles, Inner Triangles and Cubes](/docs/imgs/sloped-object.png)

Note that since these objects use rotated box colliders, they have some restrictions on how they may be scaled!


### Moving platforms

Moving platforms are composed of two independent game objects: MovingPlatform and MovingPlatformController. The former is the actual platform that carries the player around. However, the platform itself does not define it's own trajectory. The latter must be placed and configured to define the trajectory of the moving platform:

![Animation of a platform moving in a loop](/docs/imgs/moving-platform.gif)

Depending on the speed of the platform, it may need to apply more or less force on anything being carried. To adjust that, simply change the Force Correction on the Moving Platform component.

The MovingPlatformController has more options:

![MovingPlatformController](/docs/imgs/moving-platform-controller.png)

* Direction: Defines the direction of the platform's movement. It's shown in the editor as a red line;
* Force: The force applied to the carried object in the specified direction. Note that the direction is properly normalized before being applied to the platform;
* Distance: Distance between the collider that stops the moving platform and the collider that makes the platform move;
* Seconds: How long the platform will stand in place before being moved.

For platforms moving back-and-forth, defining a distance between the collider that stops the platform and the one that moves the platform makes the platform decelerate a bit before coming to a halt. This way, the carried object wont get launched when the platform stops.

For vertically moving platforms, it's useful to edit the prefab to avoid some corner cases:

![Customized platform for vertical movement](/docs/imgs/vertical-moving-platform.png)

On the image above, the collider that interacts with the MovingPlatformController was grown, to ensure that the platform is able to hit the distanced colliders. Additionally to that, a small invisible border was added to make the player less likely from getting launched from the platform (in general... vertical moving platforms are finicky!)

These modifications were made to two objects: the PlayerCollider, which is the object that physically interacts with the player, and the Controller, which interacts with MovingPlatformController objects. Although MovingPlatform, the parent object, is also a physics object, it's only used for the platform's movement. This had to be separated from the PlayerCollider so the player wouldn't affect the platform (making it neither fall nor rotate). The MovingPlatform is also the object that sends its own moving force to the player, hence why it's on the PlayerController layer.
