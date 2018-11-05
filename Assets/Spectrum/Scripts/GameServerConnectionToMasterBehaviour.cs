using UnityEngine;
using Mirror;

namespace Spectrum
{
	public class GameServerConnectionToMasterBehaviour : MonoBehaviour
	{
		public static GameServerConnectionToMasterBehaviour Instance { get; private set; }

		public GameNetworkManager GameNetworkManager;

		public bool AutoStartServer = false;
		public string AutoStartMasterIP = "10.1.10.65";
		public int AutoStartMasterPort = 7777;

		private NetworkClient NClient;

		private void Start()
		{
			Spectrum.LogLevel = Spectrum.SpectrumLogLevel.Information;
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				Destroy(gameObject);
			}
			if (AutoStartServer || Spectrum.Args.StartGameServer)
			{
				Transport.layer = new TelepathyWebsocketsMultiplexTransport();
				DontDestroyOnLoad(gameObject);
				NClient = new NetworkClient
				{
					hostPort = 7778
				};
				RegisterClientMessages();
				NClient.Connect(AutoStartMasterIP, AutoStartMasterPort);
			}
		}

		private void RegisterClientMessages()
		{
			NClient.RegisterHandler(MsgType.Connect, OnClientConnectInternal);
			NClient.RegisterHandler(MsgType.Disconnect, OnClientDisconnectInternal);
			NClient.RegisterHandler(MsgType.NotReady, OnClientNotReadyMessageInternal);
			NClient.RegisterHandler(MsgType.Error, OnClientErrorInternal);
			NClient.RegisterHandler(MsgType.Scene, OnClientSceneInternal);
		}

		private void OnClientSceneInternal(NetworkMessage netMsg)
		{
			Debug.Log("clientscene");
		}

		private void OnClientErrorInternal(NetworkMessage netMsg)
		{
			Debug.Log("clienterror");
		}

		private void OnClientNotReadyMessageInternal(NetworkMessage netMsg)
		{
			Debug.Log("clientnotready");
		}

		private void OnClientDisconnectInternal(NetworkMessage netMsg)
		{
			Debug.Log("clientdisconnect");
		}

		private void OnClientConnectInternal(NetworkMessage netMsg)
		{
			Debug.Log("clientconnect");
			GameNetworkManager.OpenGameServer();
		}

		protected void OnApplicationQuit()
		{
			Transport.layer.Shutdown();
		}

		public void GameServerAvailable()
		{
			var c = new IntegerMessage(GameNetworkManager.networkPort);
			NClient.Send(Spectrum.MsgTypes.AddGameServerToList, c);
		}

		public void ClientConnectedToGameServer()
		{
			var c = new EmptyMessage();
			NClient.Send(Spectrum.MsgTypes.IncrementPlayerCountOfServer, c);
		}

		public void ClientDisconnectedFromGameServer()
		{
			var c = new EmptyMessage();
			NClient.Send(Spectrum.MsgTypes.DecrementPlayerCountOfServer, c);
		}
	}
}