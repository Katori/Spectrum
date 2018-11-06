using Mirror;

namespace Spectrum
{
	public class GameClientConnectionToMasterBehaviour : NetworkClientBaseBehaviour
	{

		public GameNetworkManager GameNetworkManager;

		public void RequestMatch()
		{
			//string path = Application.streamingAssetsPath + "/" + Spectrum.SpectrumConnectionInfoFileName;
			//Debug.Log("path: " + path);
			//if (File.Exists(path))
			//{
			//	string data = File.ReadAllText(path);
			//	SpectrumConnectionInfo info = JsonUtility.FromJson<SpectrumConnectionInfo>(data);
			//	Spectrum.LogInformation("Client connecting to master at: "+info.MasterServerIP + " " + info.MasterServerPort);
			//	MirrorClient.Connect(info.MasterServerIP, info.MasterServerPort);

			//}
			//else
			//{
			//	Spectrum.LogError("No client connection info file at " + path + ", creating dummy file");
			//	SpectrumConnectionInfo info = new SpectrumConnectionInfo();
			//	File.WriteAllBytes(path, Encoding.UTF8.GetBytes(JsonUtility.ToJson(info)));
			//}
			MirrorClient.Send(Spectrum.MsgTypes.SendGameServerIPToClient, new EmptyMessage());
		}

		public override void RegisterClientMessages()
		{
			base.RegisterClientMessages();
			MirrorClient.RegisterHandler(Spectrum.MsgTypes.IPAndPortOfGameServerForClient, ReceivedGameServerToConnectTo);
		}

		private void ReceivedGameServerToConnectTo(NetworkMessage netMsg)
		{
			var c = netMsg.ReadMessage<StringMessage>();
			Spectrum.LogInformation("found game server to connect to: " + c.value);
			var d = c.value.Split(')');
			var address = d[0];
			var port = int.Parse(d[1]);
			MirrorClient.Disconnect();
			GameNetworkManager.networkAddress = address;
			GameNetworkManager.networkPort = port;
			GameNetworkManager.StartClient();
		}
	}
}