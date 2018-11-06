using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Internal;
using static Spectrum.Adept.Spectrum;

namespace Adept
{
    public class Program
    {
		public static Dictionary<int, GameServerInfo> GameServers = new Dictionary<int, GameServerInfo>();
		private static readonly int serverFrequency = 60;

		public static void Main(string[] args)
        {
			//CreateWebHostBuilder(args).Build().Run();
			var servercontainer = new ServerContainer();
			Transport.layer = new TelepathyTransport();
			
			RegisterHandlers();
			Thread serverThread = new Thread(() =>
			{

				Transport.layer.ServerStart("10.1.10.65", 7654, 20);
				while (true)
				{
					while (Transport.layer.ServerGetNextMessage(out int connectionId, out TransportEvent transportEvent, out byte[] data))
					{
						switch (transportEvent)
						{
							case TransportEvent.Connected:
								//Debug.Log("NetworkServer loop: Connected");
								servercontainer.HandleConnect(connectionId, 0);
								break;
							case TransportEvent.Data:
								//Debug.Log("NetworkServer loop: clientId: " + message.connectionId + " Data: " + BitConverter.ToString(message.data));
								servercontainer.HandleData(connectionId, data, 0);
								break;
							case TransportEvent.Disconnected:
								//Debug.Log("NetworkServer loop: Disconnected");
								servercontainer.HandleDisconnect(connectionId, 0);
								break;
						}
					}
					Thread.Sleep(1000 / serverFrequency);
				}
			});
			serverThread.IsBackground = false;
			serverThread.Start();
			
			
		}

		private static void RegisterHandlers()
		{
			ServerContainer.RegisterHandler((short)MsgTypes.SendGameServerIPToClient, SendGameServerToClient);
			ServerContainer.RegisterHandler((short)MsgTypes.AddGameServerToList, NewGameServerActive);
			ServerContainer.RegisterHandler((short)MsgType.Disconnect, ConnectionDisconnected);
		}

		private static void ConnectionDisconnected(NetworkMessage netMsg)
		{
			if (GameServers.ContainsKey(netMsg.conn.connectionId))
			{
				Debug.WriteLine("removing game server from list");
				GameServers.Remove(netMsg.conn.connectionId);
			}
		}

		private static void NewGameServerActive(NetworkMessage netMsg)
		{
			Debug.WriteLine("adding game server to list");
			var c = netMsg.ReadMessage<IntegerMessage>();
			GameServers.Add(netMsg.conn.connectionId, new GameServerInfo { NetworkAddress = netMsg.conn.address, Port = c.value });
		}

		private static void SendGameServerToClient(NetworkMessage netMsg)
		{
			var AvailableGameServers = GameServers.Where(x => x.Value.JoinedPlayers < x.Value.Capacity);
			var AvailableGameServersAsList = AvailableGameServers.ToList();
			var r = new System.Random((int)System.DateTime.Now.Ticks);
			if (AvailableGameServersAsList.Any())
			{
				var RandomAvailableGameServer = AvailableGameServersAsList[r.Next(0, AvailableGameServersAsList.Count)];
				var ConnectionMessage = new StringMessage(RandomAvailableGameServer.Value.NetworkAddress + ")" + RandomAvailableGameServer.Value.Port);
				netMsg.conn.Send(5011, ConnectionMessage);
			}
			else
			{
				// no server found
			}
		}

		public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

		
	}
}
