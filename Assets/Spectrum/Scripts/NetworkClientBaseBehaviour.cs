using UnityEngine;
using Mirror;

namespace Spectrum
{
	public class NetworkClientBaseBehaviour : MonoBehaviour
	{
		public static NetworkClientBaseBehaviour Instance { get; private set; }

		public NetworkClient MirrorClient;

		public bool AutomaticallyConnectToMasterServer = false;
		public string AutoMasterIP;
		public int AutoMasterPort;

		public int HostPortOverride = 0;

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
			if (AutomaticallyConnectToMasterServer)
			{
				Transport.layer = new TelepathyWebsocketsMultiplexTransport();
				DontDestroyOnLoad(gameObject);
				MirrorClient = new NetworkClient
				{
					hostPort = HostPortOverride
				};
				RegisterClientMessages();
				MirrorClient.Connect(AutoMasterIP, AutoMasterPort);
			}
		}

		public virtual void RegisterClientMessages()
		{
			MirrorClient.RegisterHandler(MsgType.Connect, OnClientConnectInternal);
			MirrorClient.RegisterHandler(MsgType.Disconnect, OnClientDisconnectInternal);
			MirrorClient.RegisterHandler(MsgType.NotReady, OnClientNotReadyMessageInternal);
			MirrorClient.RegisterHandler(MsgType.Error, OnClientErrorInternal);
			MirrorClient.RegisterHandler(MsgType.Scene, OnClientSceneInternal);
		}

		public virtual void OnClientSceneInternal(NetworkMessage netMsg)
		{
			var c = netMsg.reader.ReadString();
			ClientScene.Ready(MirrorClient.connection);
			Spectrum.LogInformation("Spectrum Client or Game Server client scene message: "+c);
		}

		public virtual void OnClientErrorInternal(NetworkMessage netMsg)
		{
			var c = netMsg.ReadMessage<ErrorMessage>();
			Spectrum.LogInformation("Spectrum Client or Game Server client error message: "+c.errorCode);
		}

		public virtual void OnClientNotReadyMessageInternal(NetworkMessage netMsg)
		{
			Spectrum.LogInformation("Spectrum Client or Game Server not ready message");
		}

		public virtual void OnClientDisconnectInternal(NetworkMessage netMsg)
		{
			Spectrum.LogInformation("Spectrum Client or Game Server disconnected from Master Server");
			MirrorClient.Disconnect();
			MirrorClient.Shutdown();
		}

		public virtual void OnClientConnectInternal(NetworkMessage netMsg)
		{
			Spectrum.LogInformation("Spectrum Client or Game Server connected to Master Server");
		}

		public virtual void OnApplicationQuit()
		{
			Transport.layer.Shutdown();
		}
	}
}