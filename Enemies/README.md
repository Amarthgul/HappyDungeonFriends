# Happy Dungeon Friends - Enemies 

Enemy is being divided into 2 part: physical representation and brain agent. 

* Physical representation is the enemy class, controlling what it could do. 
* Brain agent decides how the enemy behaves, i.e. how it would do. 

Turning, attacking, escaping or following the player are all decided by the brain agent. However, 
their timer (if have any) may loacte differently. For example, the turn timer is located
in brain agent, since an enemy can turn at any time and should be decided by the barin when
and which direction it turn to. Attack timer, on the other hand, is in the physical class, 
since an enemy cannot attack faster than its physical ability. 

Note that to add an enemy, not only will it need an index, this index also need to be added
in the generator's get enemy list. 

## Enemy Standard (`IEnemySTD`)

The body of an enemy unit.

### Attack logic 

* Can this enemy attack? `canAttack`
  * *Yes*.

    Is is ranged attack? `canRangedAttack`

    * *Yes*.

      How long will the projecile reach? `rangedAttackDistance`

    * *No*.

      In what range is the melee attack effective? `meleeAttackRange`

      Does the melee attack shows a specific sprite? `meleeShowProjectile`

      * *Yes*.

      * *No*.

    How long does this attack action last? `attackLastingTime`

    Does the enemy need to stop moving to commit an attack? `holdOnAttack`

     * *Yes*.

     * *No*.

  * *No*.

    Nothing to do here. 


# Agents 

## STD agent 

The brain of an enemy unit. 

### Movement 

* Does this enemy lock on player when the player is emitting light? 

  * *Yes*. 
    
    Is the player currently emitting light? 

    * *Yes*

      Lock onto the player. 

* Does this enemy actively seek the player? 

  * *Yes*

    * Is it not blind, photophobic, and the player happens to be in its sensing range with lights? 

      * *Yes*

        Try to escape from the player. 

    * Will it passionately seek the player and the player happens to be in the sensing
range? 

      * *Yes* 

        Try to chase the player. 

    * Default or when the player moves out of the range: back to normal. 

--------------------------------------------------------


## The stupid 

## The vampire 

## The dark archer 

## The moth 


## Blood Bead 

Blood bead is a low level slim-like creature. It does not actively do anything, but inflicts damage to
the character when being bumped onto. 


----------------------------------------------------