
public class PushFieldInstructions : UnityEngine.MonoBehaviour {
}

[UnityEditor.CustomEditor(typeof(PushFieldInstructions))]
public class PushFieldInstructionsEditor : UnityEditor.Editor {
	public override UnityEngine.UIElements.VisualElement CreateInspectorGUI() {
		return new UnityEngine.UIElements.Label(@"

!!! PushFieldInstructions !!!

Please, read these carefully!
--------------------------------------------------

A PushField has three components: a collider, fog
particles element and arrow particles. Each
component must be customized independently, since
scaling the main object wouldn't give the expected
result.

Collider:
  - The collider must be a trigger
  - Edit the 'size' field to modify its dimensions
  - Edit the 'Push' component to set the field's
    direction and force
  - **THE OBJECT'S ROTATION DOES NOT AFFECT THE
    FORCE DIRECTION**

Fog:
  - To make the particle go over a longer
    distance, change the 'Start Lifetime' in the
    'Fog' module
  - Set the color in the 'Fog' module
  - Set the direction in the 'Shape' module, by
    rotating it
  - Set the size of the area in the 'Shape'
    module, by setting its 'Scale'

Arrow particles:
  - To make the particle go over a longer
    distance, change the 'Start Lifetime' in the
    'ArrowParticleEmitter' module
  - Set the color by changing the 'Material' in
    the 'Renderer' module
  - Set the direction in the 'Shape' module, by
    setting its 'Rotation'
      - The arrows rotates automatically torwards
        its moving direction
      - The '3D Start Rotation' in the
        'ArrowParticleEmitter' module is
        configured in such a way that the arrows
        should rotate around it self, but pointing
        in the right direction
  - Set the size of the area in the 'Shape'
    module, by setting its 'Scale'
");
	}
}
