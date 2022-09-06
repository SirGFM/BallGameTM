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

### Materials

To allow customizing the models color palette,
we must first understand
how materials are exported from Blender and imported into Unity.

The material must have been sorted in the Blender model, but I couldn't find any way to check its order...
So better to always follow the steps bellow.

The only way that I could find to sort materials is to separate each material into its own model and then rejoin these object.
When joining two object, **the material of object selected last will come before the materials of the other object**.
So, for models with multiple materials, say mat1, mat2, mat3, and mat4, we must join them in sorta the reverse order:

1. First, join mat4 to mat3 (i.e., first select mat3 then mat4) as joined1
2. Then, join joined1 to mat2 as joined2
3. Finally, join joined2 to mat1

The final object will be guaranteed to have its materials sorted in the order mat1, mat2, mat3, mat4.

To separate each material into its own object, go into Edit mode, select every vertex (press `a`), press `p` and select `By Material`.

![Separate objects by material](/docs/imgs/separate-objects-by-material.png)

Then, exit Edit mode and join the objects as described above, pressing `Ctrl+j` to join two objects.
It may be easier to select the objects from the Outliner tab, since that list that materials in the object.

![Separated objects in the Outliner tab](/docs/imgs/separate-objects-in-outliner.png)

Then, after exporting the object, be sure to adjust the materials in Unity's inspector.
Don't mind the order of the materials in this view, as its not representative of the material order in the Mesh Renderer.

![Materials on Unity's inspector](/docs/imgs/materials-on-unitys-inspector.png)

Doing all that, the material order should be correct when the object is imported into a scene,
allowing for changing models while keeping the colors consistently configurable.

![Materials in object imported into an Unity scene](/docs/imgs/material-order-in-unity.png)

Lastly, to simplify finding the model in Unity, be sure to name the model something meaningful.
In the Outliner tab, this is how a renamed object that would appear as `basic-ball` on Unity looks like:

![Naming the model in Blender](/docs/imgs/blender-model-name.png)
