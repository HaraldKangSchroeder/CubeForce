# CubeForce

CubeForce is a private project of mine and a realtime online multiplayer shooter game up to 6 players at the moment.

VIDEO

Its purpose was for me to get in touch with the game engine unity and to implement an udp based dedicated multiplayer server with the C# .NET framework from scratch. The game is still in progress and sometimes a bit buggy, but it is already playable.

## Setup

### Local network
One player (host) needs to run the server in addition to the game itself. To start the server, he needs to execute `./Cubeforce/Server/Server.exe`

Afterwards, all players can start the client application/game by executing `./Cubeforce/Game/Game.exe` and use the host ip address to connect to the server.
Be careful, sound might be a bit loud at the beginning. So keep your volume low.

### Global network
This requires further configurations. It works if the host does portforwarding (server listens on port 60002), but keep in mind that this opens security leaks.

## Gameplay

* Shoot - Left mouse click
* Move - WASD
* Sidestep - Space
* Shield - Shift

## Note
When you start the application, you will get a warning - smth like "unrecognized manufacturer" or similar. This is due to the fact that I am indeed just nobody,
so you have to trust me there.
