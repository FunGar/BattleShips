Hello Happy team! 
# Short rules
Those rules were used by me and my friends when we were playing battleships in school, but any other rules can be implemented after.
1. Map is 10x10
2. Each player have the same fleet of:
   + 1 x Carrier, size = 4
   + 2 x Battleship, size = 3
   + 3 x Destroyer, size = 2
   + 4 x Patrol Boat, size = 1
3. Ship cannot be positioned next to other ship in any direction: horizontal, vertical and diagonal
4. Player gets 1 shot per turn
5. If the player hits an enemy ship, he gets an extra turn.
Other rules are like in any other Batleships game.
# Implementation notes
1. Players are bots and they can play the game in fractions of a second, so i put a 1 second delay after each turn just to see what is happening on board.
2. Bots are placing ships randomly, according to the rules
3. Bots are using this tactic for shooting:
   + Shoots randomly until it hits an enemy
   + After the first hit, randomly shoots in 4 adjacent squares until it hits an enemy
   + After the second hit, shoots in next square in current direction until the ship is destroyed or until it misses or next square is border
   + If misses or hits the border it will change direction to the opposite and shoot until the ship is destroyed
   + After it destroys a ship in any of those steps it will mark all adjacent squares as water and return to step 1
