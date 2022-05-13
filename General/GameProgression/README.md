
# Load and Save Mechanics 

For simplicity, this is a naive save machanics, Enemies and their stats are not being saved. That means if an enemy is 
injured but not killed, then after saving and reloading, this enemy will spawn in its original position with full health. 

## Save

* Take snapshot of the game 

* Convert into ProgressionInstance

* Parse ProgressionInstance into SerializableInstance to eliminate types that cannot be serializaed 

* Serialize into Json file 

* Save as a json 


