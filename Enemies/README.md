# Happy Dungeon Friends - Enemies 

Enemy is being divided into 2 part: physical representation and brain agent. 

* Physical representation is the enemy class, controlling what it could do. 
* Brain agent decides how the enemy behaves, i.e. how it would do. 

Turning, attacking, escaping or following are all decided by the brain agent. However, 
their timer (if have any) may loacte differently. For example, the turn timer is located
in brain agent, since an enemy can turn at any time and should be decided by the barin when
and which direction it turn to. Attack timer, on the other hand, is in the physical class, 
since an enemy cannot attack faster than its physical ability. 

## Enemy Standard 

Standard enemy is the meta/template for other minion classes. Standard has most of the features needed
for other classes, such as segmented movement, burrow, etc. Can be enabled or disabled with bool fields. 

## Blood Bead 

Blood bead is a low level slim-like creature. It does not actively do anything, but inflicts damage to
the character when being bumped onto. 


----------------------------------------------------

# Agents 

## The stupid 

## The vampire 

## The dark archer 

## The moth 