Terrain Pressure Paint
======================


    © 2011-2015 Freakshow Studio AS
    All rights reserved


Introduction
------------

*Terrain Pressure Paint* is an editor extension that allows painting on terrain with a pressure sensitive input device, like a Wacom tablet and pen.

It allows you to use pressure control for:

 * Raise/Lower - Brush Size and Opacity
 * Paint Height - Brush Size and Opacity
 * Smooth Height - Brush Size and Opacity
 * Paint Texture - Brush Size, Opacity and Target Strength
 * Paint Details - Brush Size, Opacity and Target Strength
 * Place Trees - Brush Size and Tree Density

You can easily select which variables to control with pen pressure, and adjust minimum and maximum values.


Usage
-----

To use, you first need to open the *Terrain Pressure Paint* window, found in the menu under ```Window -> Terrain Pressure Paint```. This window needs to be open while you paint for pressure sensitivity to work, it is recommended to dock it somewhere in the editor -- for example below the terrain inspector.

Use the toggle button for each option to select if it should be controlled by pen pressure, and then paint on the terrain as you normally would. Keep in mind that some of the options control multiple paint modes, for example *Opacity* is shared between height painting and texture painting.

There is also a min/max slider for each value, that specifies the range of the pen pressure. The leftmost value will be used with light pressure and the rightmost with hard pressure. The window will also display the currently registered pressure.

As this extension works by overriding the values in the terrain inspector, the values that are controlled by pressure will not be updated correctly in the terrain inspector. This means that if you switch off pressure control for an option, you might need to set the correct value manually in the inspector before you continue painting without pressure.

The options control the following in the different paint modes:

### Raise/Lower, paint height and smooth height

*Size* controls brush size, *Opacity* controls opacity, the other options are unused.

### Paint Texture

*Size* controls brush size, *Opacity* controls opacity, *Texture Strength* controls target strength, the other options are unused.

### Place Trees

*Tree Brush Size* controls brush size, *Tree Spacing* controls tree density, the other options are unused.

### Paint Details

*Size* controls brush size, *Detail Opacity* controls opacity, *Detail Strength* controls target strength, the other options are unused.


Known Issues
------------

On Windows, the *Terrain Pressure Paint* window must be closed before quitting Unity. If you don't do this, then the Unity Editor will hang when attempting to quit, and must be terminated from the task manager.


Support
-------

Should you require assistance, please contact <support@freakshowstudio.com>
