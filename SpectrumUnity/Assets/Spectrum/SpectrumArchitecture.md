# Spectrum Architecture

## Master Server

NetworkManager:

- Serves
- Keeps list of Spawners and Game Servers
- OnMsg 5000:
  - Add Spawner to List
- OnMsg 5005:
  - IntegerMessage - port of GameServer
  - Add GameServer to List
- OnMsg 5010:
  - Send open GameServer IP to Client
- OnMsg 5020:
  - Increment GameServer JoinedPlayers by one
- OnMsg 5021:
  - Decrement GameServer JoinedPlayers by one
- SendMsg 5001:
  - IntegerMessage - port of gameserver to open
  - sent when client requests server and there are no open servers
- SendMsg 5011:
  - StringMessage - IP and port of GameServer for client, separated by ")"

## Spawner Server

- Reads master IP and port from args
- Connects to Master
- On connection, sends 5000
- OnMsg 5001:
  - IntegerMessage - port of gameserver to open
  - Open GameServer on port

## Game Server

- Reads master IP and port from args
- Connects to Master
- On connection, sends 5005 and loads game scene

## Client

NetworkManager:

- Connect to Master Server
- SendMsg 5010:
  - Ready to connect to game
- OnMsg 5011:
  - StringMessage - splitat ")", [0] is IP and [1] is port
  - Close Master Server connection
  - Connect to Game Server