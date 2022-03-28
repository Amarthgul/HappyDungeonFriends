# Happy Dungeon Friends

<p align="center">
  <img src="https://github.com/Amarthgul/HappyDungeonFriends/blob/main/Icon.bmp">
</p>

This is an open source 2D top down game of a young girl happily explores a whimsical dungeon 
filled with rainbow, flowers, and unicorn. But do be cautious, it may contain contents that should
not be shown to kids. 

By the end it should have all codes, images (include PSDs), and musics available.

Currently still work in progress. 

まだまだ...

--------------------------------------------------------

## Latest Update

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

(currently accepts keybord and mouse input, gamepad support is not added)

* Arrow keys for movement and option selection (some may not be fully implmented)
* `Q` `W` `E` `R` to use item 
* `Enter` to confirm
* `Alt` to display hotkeys
* `B` to open bag, you can also click on the bag icon
* `Tab` to display minimap, use LMB to pan it around 
* `Esc` to pause/quit 

(It is not sure if Mac keyboard can work, tbh Mac cannot run it at all)  

--------------------------------------------------------

## Dev logs

### 0.11  

19 March 2022

**Finally back to work after almost a year's hiatus!**

* Fixed the bug (or feature?) that the items cooldown contines to count even when the player has
the game paused or in bag view. Achieved by partially substituted the `stopwatch` class. there 
are now 2 types of stopwatches:

  * `Stopwatch`, defined in this project under `Misc` folder. This stopwatch can be set to halt 
when the player opens the bag or pause the game. Most enemy and item use this stopwatch since
they are not supposed to update while player opens bag or paused the game. (the catch is that 
by toggling the `REAL_TIME_ACTION` in `Globals`, they *can*)

  * `System.Diagnostics.Stopwatch`, i.e. the C# stopwatch, which continues to count regardless
of circumstances.  

* Added dual index support to allow a position to have both unique block(the underlaying terrian sprite)
and an item/enemy spawn on top. Implementation detail at `General/IndexCoder`

### 0.10 

25 June 2021

* Basic framework, the character can move, attack, pickup and use item, kill and be killed. 
Plus the user interface and game control.  

## Random comments 

* This game is apprently based on Legend of Zelda (gameplay-wise), originally a group project
for one of my CSE classes. But I feel that project was largly flawed, so I decided to completely 
remake it and add things I wish I have added for the project. 

* Drew inspiration from some of my favorite games, Diablo, DotA 2, Warcraft, Isaac, Gris, Disgaea, 
to name a few. Also imbued heavily is the idea of Mahou Shoujo, Madoka Magica especially, that is, failure 
and sacrifice even with the earnest wish and best effort. 
