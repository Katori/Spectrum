using Mirror;

namespace Spectrum
{
	public class GameServerConnectionToMasterBehaviour : NetworkClientBaseBehaviour
	{

		public GameNetworkManager GameNetworkManager;

		public bool AutoStartServer = false;

		public override void OnClientConnectInternal(NetworkMessage netMsg)
		{
			base.OnClientConnectInternal(netMsg);
			if(AutoStartServer || Spectrum.Args.StartGameServer)
			{
				GameNetworkManager.OpenGameServer();
			}
		}

		public void GameServerAvailable()
		{
			var c = new IntegerMessage(GameNetworkManager.networkPort);
			MirrorClient.Send(Spectrum.MsgTypes.AddGameServerToList, c);
		}

		public void ClientConnectedToGameServer()
		{
			var c = new EmptyMessage();
			MirrorClient.Send(Spectrum.MsgTypes.IncrementPlayerCountOfServer, c);
		}

		public void ClientDisconnectedFromGameServer()
		{
			var c = new EmptyMessage();
			MirrorClient.Send(Spectrum.MsgTypes.DecrementPlayerCountOfServer, c);
		}
	}
}