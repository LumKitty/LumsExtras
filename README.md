# Misc. extensions for VNyan
A (hopefully) growing collection of small useful things that aren't worth putting in their own plugin. Please let me know if you have any ideas or requests.

## Current features:
- Disable resizing of the VNyan window (because it's easy to accidentally resize or maximise mid-stream and screw up where your VTuber is on your overlay)
- Reposition the VNyan window to specified co-ordinates
- Grab the exact camera position/rotation/FOV (e.g. for use with LIV, or Lunazera's additional camera props)

## Installation:
Menu -> Settings -> Misc -> Additional settings
- Allow third party plugins
- Open plugins folder
- Extract the contents of the ZIP file into this folder

## Triggers
```_lum_ext_setwindow``` - Change the window position and resizability  
num1 - X position of the window  
num2 - Y position of the window  
num3 - 1 = Disable window resizing, 2 = Enable window resizing  

Note: If you want to position the window at 0,0 you'll need to call it with X and Y set to -99999999 this is because it is impossible for a plugin to distinguish between 0 and a node simply not being hooked up

```_lum_ext_getcam``` -- Open a notepad window with the exact co-ordinates, rotation and FOV of the current main camera

As with all my plugins, this is free to use, but if you find it useful, consider sending a follow or a raid my way, and if you somehow make millions with it, consider sending me some :D

### https://twitch.tv/LumKitty
