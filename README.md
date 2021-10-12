# CubeForce

CubeForce is a private project of mine and a realtime online multiplayer game up to 6 players at the moment.

VIDEO

Its purpose was for me to get in touch with the game enginge unity and to implement an udp based dedicated multiplayer server with the C# .net framework from scratch. The game is still in progress and far from finished, but it is already playable.

## Setup

### Local network
One player (host) needs to run the server in addition to the game itself. To start the server, he needs to execute TODO

Afterwards, all players can start the client application/game and use the host ip address to connect to the server.

### Global network
This requires further configurations. It works if the host does portforwarding (server listens on port 60002), but keep in mind that this opens security leaks.