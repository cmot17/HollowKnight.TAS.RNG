## Overview

This tool gets the data from a modified version of Assembly-CSharp.dll, and then executes a Lua script in libTAS to display the data on the screen, minimizing the impact on the game and avoiding desync, but it will slow down the game slightly.

Supports Hollow Knight 1028/1221/1432/1432_Mod Linux versions
Requires at least libtas_1.4.1_amd64_hk_v9

## Assembly-CSharp.dll changes

* New field in GameManager `private static readonly long TasInfoMark = 1234567890123456789` to assist in finding the location in memory
* New field in GameManager `public static string TasInfo` used by the libTAS lua script to read and draw to the screen
* CameraController adds OnPreRender and OnPostRender which call the same methods in TasInfo for centering and scaling the camera, and for getting various data.
* Start() method in PlayMakerUnity2DProxy.Start() method to handle the newly created Object.

## Features

* Information about the knight, including position, speed, state, etc.
* Game time with the same logic as LiveSplit
* Enemy data with hitboxes
* Camera follow, zoom and disable screen shake
* Custom additional displayed data, which requires some knowledge of HK code
* Real-time switching of features by editing `HollowKnightTasInfo.config` file

## Instructions for use

1. Copy the `hollow_knight_Data/Manager/Assembly-CSharp.dll` file to the `game directory/hollow_knight_Data/Manager` directory and overwrite it. `original/Assembly-CSharp.dll` is the original unmodified file
2. In the libTAS menu `Video -> OSD -> Lua` should be checked
3. In the libTAS menu go to `Tools -> Lua -> Execute lua script` and select `HollowKnightTasInfo.lua` to display information after reading the script.
4. Edit the `Game directory/HollowKnightTasInfo.config` file to switch the functions on and off in real time and customize the data you want to display.

## Acknowledgements

* Shout out to Kilaye for the special HK version of [libTAS](https://github.com/clementgallet/libTAS/tree/hollowknight) and the added Lua API for drawing lines.
* Game time calculation from [LiveSplit.HollowKnight](https://github.com/ShootMe/LiveSplit.HollowKnight)
* Refer to [HollowKnight.Modding](https://github.com/HollowKnight-Modding/HollowKnight.Modding) for the use of MonoMod.
* Refer to [HollowKnight.HitboxDraw](https://github.com/seresharp/HollowKnight.HitboxDraw) for drawing hitboxes.
* Refer to [DebugMod](https://github.com/seresharp/DebugMod) for HP, camera follow, and zoom.
* View scenes directly in Unity with [HKWorldEdit2](https://github.com/nesrak1/HKWorldEdit2)
* View PlayMaker FSMs with [HollowKnightFSMView](https://github.com/nesrak1/HollowKnightFSMView)
* Testing done by cuber_kk, inoki, Zippy Rhys.