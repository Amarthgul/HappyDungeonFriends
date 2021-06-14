# Happy Dungeon Friends

<p align="center">
  <img src="https://github.com/Amarthgul/HappyDungeonFriends/blob/main/Icon.bmp">
</p>

This is an open source 2D top down game of a young girl happily explores a whimsical dungeon 
filled with rainbow, flowers, and unicorn.

By the end it should have all codes, images (include PSDs), and musics available.

Note that this project has a strong personal flavour, I do try to keep the naming, comments, and implementation 
consistent, but it is not strictly bound to any design pattern or strategy. Like some words are used interchangeably; 
and the inner structure of a file, like what fileds goes first or how to sort the methods, are not the same; 
how brackets are indented may also vary slightly depending on whether or not I want a certain part to be more 
compact. 

Currently still work in progress. 

まだまだ...

--------------------------------------------------------

Update 10 June 2021: 

STD enemy can now do different attack methods. 

<p align="center">
	<img src="https://github.com/Amarthgul/HappyDungeonFriends/blob/main/Content/Screencap/lye4f1D.png" width="512">
</p>

--------------------------------------------------------

Dev logs

* Items count CD on its own and still counts when in the bag, it seems possible to quickly 
switch item in and out of slots and thus drastically increase the ability of the character.
I don't really want to change that, it feels fun. 

* Since `Stopwatch` will continue running once after started, it might happen that some enemy
or environments will have a surge of action after the change of game states. It is definitely 
possible to add a switch condition to all stopwatches, but considering the amount of work needed,
I probably won't be doing it. 
