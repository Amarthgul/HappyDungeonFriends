# Happy Dungeon Friends

<p align="center">
  <img src="https://github.com/Amarthgul/HappyDungeonFriends/blob/main/Icon.bmp">
</p>

This is an open source 2D top down game of a young girl happily explores a whimsical dungeon 
filled with rainbow, flowers, and unicorn. But do be cautious, it may contain contents that should
not be shown to kids. 

By the end it should have all codes, images (include PSDs), and musics available.

Note that this project has a strong personal flavour, I do try to keep the naming, comments, and implementation 
consistent, but it is not strictly bound to any design pattern or strategy. Like some words are used interchangeably; 
and the inner structure of a file, like what fileds goes first or how to sort the methods, are not the same; 
how brackets are indented may also vary slightly depending on whether or not I want a certain part to be more 
compact. 

Currently still work in progress. 

まだまだ...

--------------------------------------------------------

## Update 19th March 2022: 

Added new stopwatch. Detail at Dev log 0.11.    

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

Partially substituted the `stopwatch` class. there are now 2 types of stopwatches:

* `Stopwatch`, defined in this project under `Misc` folder. This stopwatch can be set to halt 
when the player opens the bag or pause the game. Most enemy and item use this stopwatch since
they are not supposed to update while player opens bag or paused the game. (the catch is that 
by toggling the `REAL_TIME_ACTION` in `Globals`, they *can*)

* `System.Diagnostics.Stopwatch` the C# stopwatch, which continues to count regardless of circumstances.  

### 0.10 

25 June 2021

Basic framework, the character can move, attack, pickup and use item, kill and be killed. 
Plus the user interface and game control.  

## Random comments 

* This game drew inspiration from some of my favorites, such as DotA 2, Diablo, Warcraft, Isaac, 
and some part of Gris and Disgaea. 

* Items count CD on its own and still counts when in the bag, it seems possible to quickly 
switch item in and out of slots and thus drastically increase the ability of the character.
I don't really want to change that, it feels fun. 

* Since `Stopwatch` will continue running once after started, it might happen that some enemy
or environments will have a surge of action after the change of game states. It is definitely 
possible to add a switch condition to all stopwatches (or write a new stopwatch class),
but considering the amount of work needed,
I probably won't be doing it. 
