# Happy Dungeon Friends

<p align="center">
  <img src="https://github.com/Amarthgul/HappyDungeonFriends/blob/main/Icon.bmp">
</p>

This is an open source 2D top down game of a young girl happily explores a whimsical dungeon 
filled with rainbow, flowers, and unicorn. But do be cautious, it may contain contents that should
not be shown to kids. 

Written in C# XNA, sprites designed and drawn by me, musics from online resouces(mostly bought on ZapSplat), 
by the end it should have all codes, images (include PSDs), and musics/SFX files available.

Currently still work in progress. 

まだまだ...

--------------------------------------------------------

## Latest Update

**Please note that these updates may not be reflected in release**

Updated 28th March 2022: 

Added a new UI display for save and load game progressions, still work in
progress. 


<p align="center">
	<img src="https://github.com/Amarthgul/HappyDungeonFriends/blob/main/Content/Screencap/W4hXXG4.png" width="512">
</p>

<p align="center">
	<img src="https://github.com/Amarthgul/HappyDungeonFriends/blob/main/Content/Screencap/lye4f1D.png" width="512">
</p>

--------------------------------------------------------

## How to control: 

Unfortunately there is no key-binding that the player can change in-game, but by toggling certain variables, 
the game can have different controls. By default the control is `Keyboard (RPG)` with `Mouse`.

### Keyboard (Traditional)

Traditional approach combines the arrow key with MOBA skill keys, when used with mouse, it
produces an experience somewhat similar to Dota or Starcraft. 

* Arrow keys for movement and option selection (some may not be fully implmented)
* `Q` `W` `E` `R` to use items 
* `Enter` to confirm
* `Alt` to display hotkeys
* `A` to attack
* `B` to open bag, you can also click on the bag icon
* `Tab` to display minimap, use LMB to pan it around 
* `Esc` to pause/quit 

### Keyboard (RPG)

A more concentrated approach, as most operational keys are gathered in left side of the keyboard.

* `Q` `W` `E` `R` to move 
* `1`, `2`, `3`, `4` to use items
* `Enter` to confirm
* `Alt` to display hotkeys
* `Space` to attack 
* `B` to open bag, you can also click on the bag icon
* `Tab` to display minimap, use LMB to pan it around 
* `Esc` to pause/quit 

### Mouse 
* RMB key to move 
* LMB to attack or select/open

(It is not sure if Mac keyboard can work, tbh Mac cannot run it at all)  

--------------------------------------------------------

## Dev logs

### Ver.0.91

19 March 2022

**Finally back to work after almost a year's hiatus!**

* Fixed the bug (or feature?) that the items cooldown contines to count even when the player has
the game paused or in bag view. Achieved by partially substituted the `stopwatch` class. there 
are now 2 types of stopwatches:

  * `Stopwatch`, defined in this project under `Misc` folder. This stopwatch can be set to halt 
when the player opens the bag or pause the game. Most enemy and item use this stopwatch since
they are not supposed to update while player opens bag or paused the game (the catch being that 
by toggling the `REAL_TIME_ACTION` in `Globals`, they still *can*). 

  * `System.Diagnostics.Stopwatch`, i.e. the C# stopwatch, which continues to count regardless
of circumstances.  

* Added dual index support to allow a position to have both unique block(the underlaying terrian sprite)
and an item/enemy spawn on top. Implementation detail at `General/IndexCoder`

### Ver.0.9

25 June 2021

* Basic framework, the character can move, attack, pickup and use item, kill and be killed. 
Plus the user interface and game control.  

## Random comments 

* This is a badly made game, no way to cover that, my lack of experience made too many shitty 
desicions, including but not limited to gameplay, variable names, UI, code structure, etc.

* Based on Legend of Zelda (gameplay-wise), this project was originally a group project
for one of my CSE classes. But I feel that project was largly flawed, so I decided to completely 
remake it and add things I wish I have added for the project. 

* Drew inspiration from some of my favorite games, Diablo, DotA 2, Warcraft, Isaac, Gris, Disgaea, 
to name a few. Also imbued heavily is the idea of Mahou Shoujo, Madoka Magica especially, that is, failure 
and sacrifice even with the earnest wish and best effort. 
