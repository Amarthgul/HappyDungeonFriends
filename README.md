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

## Update 25 June 2021: 

Despite still cannot die or win, most of the game feature frameworks has now been finsihed.   

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
* `Alt` to display hotkeys
* `B` to open bag, you can also click on the bag icon
* `Tab` to display minimap, use LMB to pan it around 
* `Esc` to pause/quit 

--------------------------------------------------------

## Dev logs and random comments 

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
