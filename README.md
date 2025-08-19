# Misc. extensions for VNyan
A (hopefully) growing collection of small useful things that aren't worth putting in their own plugin. Please let me know if you have any ideas or requests.

## Current features:
- Disable resizing of the VNyan window (because it's easy to accidentally resize or maximise mid-stream and screw up where your VTuber is on your overlay)
- Reposition the VNyan window to specified co-ordinates
- Get current desktop size, main monitor size and number of monitors (e.g. to reposition different based on docked vs undocked laptop etc.)
- Get current VNyan window size
- Grab the exact camera position/rotation/FOV (e.g. for use with LIV, or Lunazera's additional camera props)

## Installation:
1) Menu -> Settings -> Misc -> Additional settings
2) Allow third party plugins
3) Open plugins folder
4) Extract the contents of the ZIP file into this folder
5) (Optional) Import the example nodegraph to see how to use this

## Triggers
```_lum_ext_setwindow``` - Change the window position and resizability  
num1 - X position of the window  
num2 - Y position of the window  
num3 - 1 = Disable window resizing, 2 = Enable window resizing  

Note: If you want to position the window at 0,0 you'll need to call it with X and Y set to -99999999 this is because it is impossible for a plugin to distinguish between 0 and a node simply not being hooked up

  
```_lum_ext_getwindow``` - Get the size and position of the VNyan window
Sets four variables: 
- ```_lum_ext_winX```, ```_lum_ext_winY```, ```_lum_ext_winW```, ```_lum_ext_winX```: X, Y, Width Height

If you are using this information as part of an Ordered Execution node, ensure that the trigger is set to run "Now" and not the default "Next Frame" otherwise your query will run before you get the data

  
```_lum_ext_getdesktop``` - Get the size of your full desktop, size of your main monitor and number of monitors
Sets five variables:
- ```_lum_ext_desktopX```, ```_lum_ext_desktopY```: The total size of your entire desktop with all monitors taken into consideration
- ```_lum_ext_monitorX```, ```_lum_ext_monitorY```: The size of your main monitor only
- ```_lum_ext_monitors```: The total number of active monitors attached to your computer (disabled monitors are not included)

If you are using this information as part of an Ordered Execution node, ensure that the trigger is set to run "Now" and not the default "Next Frame" otherwise your query will run before you get the data
  
```_lum_ext_getcam``` -- Open a notepad window with the exact co-ordinates, rotation and FOV of the current main camera

```_lum_ext_setcam``` -- Make adjustments to the VNyan camera  
num1: Affects how your view is scaled when the window is sized to non-16:9 aspect ratios. 1 = VNyan becomes hor+ - 2 = VNyan becomes vert- (default)  
num2: Enable or disable the "physical camera" distortion effect that simulates lens curvature etc. 1 = disabled - 2 = enabled (default)  

As with all my plugins, this is free to use, but if you find it useful, consider sending a follow or a raid my way, and if you somehow make millions with it, consider sending me some :D

### https://twitch.tv/LumKitty
