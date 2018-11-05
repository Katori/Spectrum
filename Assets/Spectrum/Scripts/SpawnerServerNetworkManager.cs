using Mirror;

namespace Spectrum
{
	class SpawnerServerNetworkManager : NetworkManager
	{

		private void Start()
		{
			Spectrum.LogLevel = Spectrum.SpectrumLogLevel.Information;
			if (Spectrum.Args.StartSpawner)
			{
				networkAddress = Spectrum.Args.MasterIp;
				networkPort = Spectrum.Args.MasterPort;
				client.RegisterHandler(Spectrum.MsgTypes.PortOfGameServerToOpen, OpenGameServerOnPort);
				StartClient();
			}
		}

		private void OpenGameServerOnPort(NetworkMessage netMsg)
		{
			var c = netMsg.ReadMessage<IntegerMessage>();
			Spectrum.LogInformation("Ready to start new game server on port " + c.value);
		}

		public override void OnClientConnect(NetworkConnection conn)
		{
			base.OnClientConnect(conn);
			var b = new EmptyMessage();
			NetworkClient.allClients[0].Send(Spectrum.MsgTypes.AddSpawnerToList, b);	
		}
	}
}
