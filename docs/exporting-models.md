# Making models and prefabs

Since Unity has a few primitive objects,
it's possible to prototype objects by creating an object with a children for each primitive.
Although that is quite useful, it's also quite resource intensive.
Even if batching were enabled, instead of batching the primitives,
it seems like Unity ends up batching each object individually.

**Thus, these prototypes must eventually be modelled in an external tool!**

## Shading mode in Blender

To make Blender models more smooth in Unity,
be sure to select every face of the model in `Edit Mode` and hit `Ctrl+f`.

![Toolbar showing the 'Shade Smooth' setting](/docs/imgs/blender-shading.png)

This information is carried over when exporting the model!

## Exporting Blender models

To standardize everything, assume that 10cm (the default size of a cube) is 1 unit in Unity.

![A wire frame box showing the scale on top of spikes](/docs/imgs/model-blender-scale.png)

Unity disagrees with Blender's 3D space...
In Blender, the Y axis is the vertical axis and the Z axis is the depth axis.
However, in Unity the Y axis is the depth axis and the Z axis is the vertical axis.

To adjust the model so it works normally in Unity:

1. Enter `Edit Mode`
1. Rotate the object -90 degrees in the X axis (so -Y is up)

![A Blender model rotated as needed by Unity](/docs/imgs/model-blender-rotation.png)

The object should be exported as an FBX.
Also, be sure to select the desired object and
to check `Limit to Selected Objects` when exporting.

It seems like Unity simply ignores the other options,
so leave everything else as is.

![Blender's FBX export options](/docs/imgs/blender-fbx-export-options.png)

Finally, after importing the model in Unity (which is done automatically),
be sure to select the imported object and set the `Scale Factor` to 50.

![Setting the Scale Factor in Unity](/docs/imgs/unity-model-importing.png)
