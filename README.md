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

<p align="center">
	<img src="https://github.com/Amarthgul/HappyDungeonFriends/blob/main/Content/Screencap/W4hXXG4.png" width="512">
</p>

<p align="center">
	<img src="https://github.com/Amarthgul/HappyDungeonFriends/blob/main/Content/Screencap/lye4f1D.png" width="512">
</p>

--------------------------------------------------------

## Latest Update

**Please note that the update in this section may not be reflected in the release**

Update 29th Dec 2022:

* Added a proper gold drop function after killing an enemy 

* Added item description for gold when mouse if hovering on top

* Fixed the problem where items are drawn in the wrong layer when dragged from bag to slots

* Fixed the problem where bag icon on HUD keeps showing on-hover effect even after quitting the bag view

--------------------------------------------------------

## How to control: 

There is no key-binding that the player can change in-game, but by toggling certain variables, 
the game can have different controls. By default the control is `Keyboard (RPG)` with `Mouse`.

### Keyboard (RPG)

An approach that resembles common RPG games, most operational keys are gathered in left side of the keyboard.

* `Q` `W` `E` `R` to move 
* `1` `2` `3` `4` to use items/skills
* `Enter` to confirm
* `Alt` to display hotkeys
* `Space` to attack 
* `B` to open bag, you can also click on the bag icon
* `Tab` to display minimap, use LMB to pan it around 
* `Esc` to pause/quit 

### Keyboard (Traditional)

Traditional approach combines the arrow key with MOBA skill keys, when used with mouse, it
produces an experience somewhat similar to Dota or Starcraft. 

* Arrow keys for movement and option selection
* `Q` `W` `E` `R` to use items/skills
* `Enter` to confirm
* `Alt` to display hotkeys
* `A` to attack
* `B` to open bag, you can also click on the bag icon
* `Tab` to display minimap, use LMB to pan it around 
* `Esc` to pause/quit 

### Mouse 
* RMB key to move 
* LMB to attack or select/open


--------------------------------------------------------

## Dev logs

### Ver.0.92

Released 19th May 2022

* The game can now save and load from saved. Note this is a naive save mechanism, it saves most of
the progression, but enemies are not fully recorded, that is, if an enemy is injured but not killed, 
then after load from saved, this enemy will spawn at its last position with full health and other stats
as originally spawned.

* Bag icon in HUD now come with on hover sound.

* Updated the bag graphics and character art.

* As suggested in issue, distance to pass through a door is widened.

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

--------------------------------------------------------

## Known issues 

* Item descriptions may differ or duplicate, due to them all invoking from `TextBridge` instance. 
For example, if `Note` items are dropped from enemies, they may have the same text description despite
the alogrithm was designed to make them different. 