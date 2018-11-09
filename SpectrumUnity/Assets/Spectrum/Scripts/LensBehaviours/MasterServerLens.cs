using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mirror;

namespace Spectrum.Lens
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
			RegisterHandler((short)Spectrum.MsgTypes.AuthCode, ReceivedAuthCode);
		}

		private void ReceivedAuthCode(LensMessage netMsg)
		{
			var c = netMsg.ReadMessage<StringMessage>();
			if (c.value != Spectrum.AuthCode)
			{
				netMsg.conn.Disconnect();
			}
		}

		private void OnSpawnerReady(LensMessage netMsg)
		{
			Spectrum.LogInformation("Spawner connected");
			var c = netMsg.ReadMessage<IntegerMessage>();
			Spectrum.LogInformation(netMsg.conn.connectionId.ToString());
			Spawners.Add(netMsg.conn.connectionId, new ServerInfo { Address = netMsg.conn.address, Port = c.value });
		}

		private void OnGameServerReady(LensMessage netMsg)
		{
			var c = netMsg.ReadMessage<IntegerMessage>();
			GameServers.Add(netMsg.conn.connectionId, new ServerInfo { Address = netMsg.conn.address, Port = c.value });
			var d = Mathf.Max(4, WaitingClients.Count);
			for (int i = 0; i < d; i++)
			{
				SendConnectionMessage(WaitingClients[i], netMsg.conn.address, c.value);
			}
		}

		private void SendConnectionMessage(int ConnectionId, string Address, int Port)
		{
			var ServerMessage = new StringMessage(Address + ")" + Port);
			ServerSendMsg(ConnectionId, (short)Spectrum.MsgTypes.IPAndPortOfGameServerForClient, ServerMessage);
		}

		private void IncreasePlayerCount(LensMessage netMsg)
		{
			GameServers.FirstOrDefault(x => x.Key == netMsg.conn.connectionId).Value.JoinedClients += 1;
		}

		private void DecreasePlayerCount(LensMessage netMsg)
		{
			var MatchedServer = GameServers.Where(x => x.Key == netMsg.conn.connectionId).ToList();
			if (MatchedServer.Any())
			{
				MatchedServer.FirstOrDefault().Value.JoinedClients -= 1;
			}
		}

		private void SendGameServerIPToClient(LensMessage netMsg)
		{
			var c = GameServers.Where(x => x.Value.JoinedClients < x.Value.ClientCapacity).ToList();
			var ServerMessage = new StringMessage();
			if (c.Any())
			{
				var p = c[Random.Range(0, c.Count)];
				SendConnectionMessage(netMsg.conn.connectionId, p.Value.Address, p.Value.Port);
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

		public override void OnDisconnected(LensConnection conn)
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
