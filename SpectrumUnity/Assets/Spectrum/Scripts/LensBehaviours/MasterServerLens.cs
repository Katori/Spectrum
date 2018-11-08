using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

namespace Spectrum
{
	class MasterServerLens : LensBehaviour
	{
		private Dictionary<int, ServerInfo> Spawners = new Dictionary<int, ServerInfo>();

		private Dictionary<int, ServerInfo> GameServers = new Dictionary<int, ServerInfo>();

		private List<int> WaitingClients;

		private readonly int MinGameServerPort = 7779;
		private readonly int MaxGameServerPort = 7789;

		public class ServerInfo
		{
			public string Address;
			public int Port;
			public int JoinedClients = 0;
			public readonly int ClientCapacity = 4;
		}

		private void Start()
		{
			Spectrum.LogLevel = Spectrum.SpectrumLogLevel.Information;
			Telepathy.Logger.LogMethod = Debug.Log;
			Telepathy.Logger.LogWarningMethod = Debug.LogWarning;
			Telepathy.Logger.LogErrorMethod = Debug.LogError;
			StartServer();
		}


		public override void RegisterServerHandlers()
		{
			base.RegisterServerHandlers();
			RegisterHandler((short)Spectrum.MsgTypes.SendGameServerIPToClient, SendGameServerIPToClient);
			RegisterHandler((short)Spectrum.MsgTypes.AddSpawnerToList, OnSpawnerReady);
			RegisterHandler((short)Spectrum.MsgTypes.AddGameServerToList, OnGameServerReady);
			RegisterHandler((short)Spectrum.MsgTypes.IncrementPlayerCountOfServer, IncreasePlayerCount);
			RegisterHandler((short)Spectrum.MsgTypes.DecrementPlayerCountOfServer, DecreasePlayerCount);
		}

		private void OnSpawnerReady(NetworkMessage netMsg)
		{
			var c = netMsg.ReadMessage<IntegerMessage>();
			Spawners.Add(netMsg.conn.connectionId, new ServerInfo { Address = netMsg.conn.address, Port = c.value });
		}

		private void OnGameServerReady(NetworkMessage netMsg)
		{
			var c = netMsg.ReadMessage<IntegerMessage>();
			GameServers.Add(netMsg.conn.connectionId, new ServerInfo { Address = netMsg.conn.address, Port = c.value });
			for (int i = 0; i < 4; i++)
			{
				if (WaitingClients.ElementAtOrDefault(i) != 0)
				{
					var ServerMessage = new StringMessage(netMsg.conn.address+")"+c.value);
					ServerSendMsg(WaitingClients[i], (short)Spectrum.MsgTypes.IPAndPortOfGameServerForClient, ServerMessage);
				}
			}
		}

		private void IncreasePlayerCount(NetworkMessage netMsg)
		{
			GameServers.FirstOrDefault(x => x.Key == netMsg.conn.connectionId).Value.JoinedClients += 1;
		}

		private void DecreasePlayerCount(NetworkMessage netMsg)
		{
			GameServers.FirstOrDefault(x => x.Key == netMsg.conn.connectionId).Value.JoinedClients -= 1;
		}

		private void SendGameServerIPToClient(NetworkMessage netMsg)
		{
			var c = GameServers.Where(x => x.Value.JoinedClients < x.Value.ClientCapacity).ToList();
			var ServerMessage = new StringMessage();
			if (c.Any())
			{
				var p = c[Random.Range(0, c.Count)];
				ServerMessage.value = p.Value.Address + ")" + p.Value.Port;
				ServerSendMsg(netMsg.conn.connectionId, (short)Spectrum.MsgTypes.IPAndPortOfGameServerForClient, ServerMessage);
			}
			else
			{
				var AvailableSpawners = Spawners.Where(x => x.Value.JoinedClients < x.Value.ClientCapacity).ToList();
				var SelectedSpawner = AvailableSpawners[Random.Range(0, AvailableSpawners.Count)];
				ServerSendMsg(SelectedSpawner.Key, (short)Spectrum.MsgTypes.PortOfGameServerToOpen, new EmptyMessage());
				ServerSendMsg(netMsg.conn.connectionId, (short)Spectrum.MsgTypes.ClientWaitForSpawnedServer, new EmptyMessage());
				WaitingClients.Add(netMsg.conn.connectionId);
			}
		}

		public override void OnDisconnected(NetworkConnection conn)
		{
			base.OnDisconnected(conn);
			if (Spawners.ContainsKey(conn.connectionId))
			{
				Spawners.Remove(conn.connectionId);
			}
			else if (GameServers.ContainsKey(conn.connectionId))
			{
				GameServers.Remove(conn.connectionId);
			}
		}
	}
}
