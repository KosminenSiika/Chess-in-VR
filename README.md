This is the source code for a VR Game/Application made for the Mixed Reality and Metaverse course at the University of Turku. It can be downloaded from the releases section of this repository. 

The game is a VR implementation of chess where you interact with the pieces and turn clock in a natural way using your VR controllers. Chess is played against a computer opponent, which uses the open-source chess engine Fairy-Stockfish (available at https://fairy-stockfish.github.io/) to make its moves. The player can choose which pieces to play with, the time available for each colour as well as the difficulty of the computer opponent. Difficulty ranges from 1 to 8 emulating the difficulties available on Lichess (https://lichess.org/). The game is made with SEATED gameplay in mind, but can also be played standing up which might require you to recenter yourself through SteamVR / Oculus's menus / etc. The game was developed and tested using a Meta Quest 2 headset using Oculus as the OpenXR runtime, but it should also work using other OpenXR runtimes and headsets. 

The loose correspondence between the difficulty level and chess ELO:
- Difficulty 1 ~ under 400 elo
- Difficulty 2 ~ 500 elo
- Difficulty 3 ~ 800 elo
- Difficulty 4 ~ 1100 elo
- Difficulty 5 ~ 1500 elo
- Difficulty 6 ~ 1900 elo
- Difficulty 7 ~ 2300 elo
- Difficulty 8 ~ 2800+ elo

FAQ:
- Q: What are the controls?
- A: Interaction with UI components is done with the trigger buttons. Interaction with pieces, the chess clock as well as teleporting between the stage and the chair is done with the grip button. The left controller menu button opens the game menu.

- Q: How can I sit in the chair?
- A: Point at the chair, so that you can see a line appear from the your hand and press the grip button. To get out of the chair, point at the ground and do the same.

- Q: My camera is way too high / way too low?!?!
- A: Recenter your view through the runtime you're using (e.g. SteamVR / Oculus / etc.).

- Q: I'm changing the settings on Chessbot 5000, but nothing is happening...?
- A: Changing the setting does not affect the match that is currently being played. Remember to start a new match from the left controller's game menu and your new settings will be applied. 

- Q: I made my move, but the computer is not doing a move?!
- A: Remember to end your turn by pressing the chess clock. You will know the computer is thinking of a move when it wiggles its moustache.

- Q: Wait a minute, am I playing against Grumbot?
- A: Actually, you're playing against Chessbot 5000. But yes the design is heavily inspired by Grumbot, a minecraft build by Mumbo Jumbo and Grian on the Hermitcraft server. :)

