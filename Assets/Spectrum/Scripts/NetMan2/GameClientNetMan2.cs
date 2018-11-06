using Mirror;
using Telepathy;

namespace Spectrum
{
	class GameClientNetMan2 : NetMan2
	{

		public GameNetworkManager GameNetMan;

		private void Start()
		{
			Spectrum.LogLevel = Spectrum.SpectrumLogLevel.Information;
			StartClient();
		}

		public override void RegisterClientHandlers()
		{
			base.RegisterClientHandlers();
			RegisterHandler(Spectrum.MsgTypes.IPAndPortOfGameServerForClient, ReceivedServerToConnect);
		}

		public override void OnConnected()
		{
			base.OnConnected();
			Spectrum.LogInformation("Connected to master received by GameClientNetMan2");
			ClientSendMsg(Spectrum.MsgTypes.SendGameServerIPToClient, new EmptyMessage());
		}

		private void ReceivedServerToConnect(NetworkMessage netMsg)
		{
			var c = netMsg.ReadMessage<StringMessage>();
			Spectrum.LogInformation("Received server info: " + c);
			var d = c.value.Split(')');
			var address = d[0];
			var port = int.Parse(d[1]);
			GameNetMan.networkAddress = address;
			GameNetMan.networkPort = port;
			GameNetMan.StartClient();
		}
	}
}
