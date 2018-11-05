using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

namespace Spectrum
{
	class MasterServerNetworkManager : NetworkManager
	{

		private Dictionary<int, SpawnServerInfo> Spawners = new Dictionary<int, SpawnServerInfo>();

		private Dictionary<int, GameServerInfo> GameServers = new Dictionary<int, GameServerInfo>();

		private readonly int MinPort = 7779;
		private readonly int MaxPort = 7789;
		
		public class SpawnServerInfo
		{
			public int Port;
			public int SpawnedGameServers = 0;
			public readonly int Capacity = 4;
		}

		public class GameServerInfo
		{
			public string Address;
			public int Port;
			public int JoinedPlayers = 0;
			public readonly int Capacity = 4;
		}

		private void Start()
		{
			Spectrum.LogLevel = Spectrum.SpectrumLogLevel.Information;
			if (Spectrum.Args.StartMaster)
			{
				string NetworkAddress = Spectrum.MasterServerIP;
				networkAddress = NetworkAddress;
				serverBindAddress = NetworkAddress;
				networkPort = Spectrum.MasterServerPort;
				serverBindToIP = true;
				StartServer();
			}
			else
			{
				StartServer();
			}
		}

		public override void OnServerConnect(NetworkConnection conn)
		{
			base.OnServerConnect(conn);
			NetworkServer.SetClientReady(conn);
		}

		public override void OnServerAddPlayer(NetworkConnection conn)
		{
			Spectrum.LogInformation("Master Server - Setting client ready of connectionId:" + conn.connectionId);
			NetworkServer.SetClientReady(conn);
		}

		public override void OnStartServer()
		{
			base.OnStartServer();
			Debug.Log("Master Server - Registering handlers");
			NetworkServer.RegisterHandler(Spectrum.MsgTypes.AddSpawnerToList, OnSpawnerReady);
			NetworkServer.RegisterHandler(Spectrum.MsgTypes.AddGameServerToList, OnGameServerReady);
			NetworkServer.RegisterHandler(Spectrum.MsgTypes.SendGameServerIPToClient, SendGameServerIPToClient);
			NetworkServer.RegisterHandler(Spectrum.MsgTypes.IncrementPlayerCountOfServer, IncreasePlayerCount);
			NetworkServer.RegisterHandler(Spectrum.MsgTypes.DecrementPlayerCountOfServer, DecreasePlayerCount);
		}

		public override void OnClientDisconnect(NetworkConnection conn)
		{
			base.OnClientDisconnect(conn);
			if (Spawners.ContainsKey(conn.connectionId))
			{
				Spawners.Remove(conn.connectionId);
			}
			if (GameServers.ContainsKey(conn.connectionId))
			{
				Spawners.ElementAt(0).Value.SpawnedGameServers -= 1;
				GameServers.Remove(conn.connectionId);
			}
		}



		private void DecreasePlayerCount(NetworkMessage netMsg)
		{
			Spectrum.LogInformation("Player "+netMsg.conn.connectionId+" disconnected");
			GameServers[netMsg.conn.connectionId].JoinedPlayers -= 1;
		}

		private void IncreasePlayerCount(NetworkMessage netMsg)
		{
			Spectrum.LogInformation("Player " + netMsg.conn.connectionId + " connected");
			GameServers[netMsg.conn.connectionId].JoinedPlayers += 1;
		}

		private void SendGameServerIPToClient(NetworkMessage netMsg)
		{
			Spectrum.LogInformation("Client requested a join, finding server");
			var c = GameServers.Where(x => x.Value.JoinedPlayers < x.Value.Capacity);
			if (c.Any())
			{
				Spectrum.LogInformation("Found a server, directing client to it");
				var ServerToJoin = c.ElementAt(Random.Range(0, c.Count()));
				var d = new StringMessage(ServerToJoin.Value.Address + ")" + ServerToJoin.Value.Port);
				netMsg.conn.Send(Spectrum.MsgTypes.IPAndPortOfGameServerForClient, d);
			}
			else
			{
				Spectrum.LogInformation("No server found, asking Spawner to create one");
				var l = new IntegerMessage(MinPort);
				if (Spawners.Any())
				{
					NetworkServer.connections[Spawners.ElementAt(0).Key].Send(Spectrum.MsgTypes.PortOfGameServerToOpen, l);
					Spawners.ElementAt(0).Value.SpawnedGameServers += 1;
				}
				else
				{
					Spectrum.LogError("No spawners available");
				}
			}
		}

		private void OnGameServerReady(NetworkMessage netMsg)
		{
			Spectrum.LogInformation("Received game server ready command");

			var portMessage = netMsg.ReadMessage<IntegerMessage>();
			var c = new GameServerInfo
			{
				Address = netMsg.conn.address,
				Port = portMessage.value
			};
			GameServers.Add(netMsg.conn.connectionId, c);
		}

		private void OnSpawnerReady(NetworkMessage netMsg)
		{
			Spectrum.LogInformation("Received spawner ready command");
			var b = new SpawnServerInfo
			{
				Port = 7777,
				SpawnedGameServers = 0
			};
			Spawners.Add(netMsg.conn.connectionId, b);
		}
	}
}
