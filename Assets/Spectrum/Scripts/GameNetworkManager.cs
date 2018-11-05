using System.Text;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

namespace Spectrum
{
	public class GameNetworkManager : NetworkManager {

		public static GameNetworkManager Instance { get; private set; }

		private readonly string clientConnectioninfoFileName = "ClientConnectionInfo.json";

		private bool DoLoadGameScene = false;

		public bool UseSpawnCamera;
		public string SpawnCameraName = "SpawnCamera";

		public void Start()
		{
			if (Instance == null)
			{
				Instance = this;
			}
			else
			{
				Destroy(gameObject);
			}
		}

		public void ConnectToMasterAsClient()
		{
			Spectrum.LogLevel = Spectrum.SpectrumLogLevel.Information;
			string path = Application.streamingAssetsPath+"/"+clientConnectioninfoFileName;
			Debug.Log("path: " + path);
			if (File.Exists(path))
			{
				string data = File.ReadAllText(path);
				ClientConnectionInfo info = JsonUtility.FromJson<ClientConnectionInfo>(data);
				Debug.Log(info.MasterServerIP + " " + info.MasterServerPort);
				networkAddress = info.MasterServerIP;
				networkPort = info.MasterServerPort;
				StartClient();
				client.RegisterHandler(Spectrum.MsgTypes.IPAndPortOfGameServerForClient, ReceivedGameServerToConnectTo);
			}
			else
			{
				Spectrum.LogError("No client connection info file at "+path+", creating dummy file");
				ClientConnectionInfo info = new ClientConnectionInfo();
				File.WriteAllBytes(path, Encoding.UTF8.GetBytes(JsonUtility.ToJson(info)));
			}
		}

		public void OpenGameServer()
		{
			if (GameServerConnectionToMasterBehaviour.Instance.AutoStartServer)
			{
				Spectrum.LogInformation("Using Auto Server (Inspector Configured)");
				networkAddress = GameServerConnectionToMasterBehaviour.Instance.AutoStartMasterIP;
				serverBindAddress = GameServerConnectionToMasterBehaviour.Instance.AutoStartMasterIP;
			}
			else
			{
				networkAddress = Spectrum.MachineIP;
				serverBindAddress = Spectrum.MachineIP;
			}
			
			if (Spectrum.Args.StartGameServer)
			{
				networkPort = Spectrum.Args.AssignedPort;
			}
			else
			{
				networkPort = 7778;
			}
			serverBindToIP = true;
			StartServer();
		}

		[System.Serializable]
		public class ClientConnectionInfo
		{
			public string MasterServerIP;
			public int MasterServerPort;
		}

		public override void OnStartClient(NetworkClient client)
		{
			base.OnStartClient(client);
		}

		public override void OnClientConnect(NetworkConnection conn)
		{
			Debug.LogWarning("onclientconnectreached");
			if (DoLoadGameScene)
			{
				SceneManager.sceneLoaded += GameSceneLoaded;
				SceneManager.LoadScene(0);
				ClientScene.Ready(conn);
				if (autoCreatePlayer)
				{
					Spectrum.LogInformation("Adding player");
					ClientScene.AddPlayer();
				}
			}
			else
			{
				ClientScene.Ready(conn);
				var c = new EmptyMessage();
				Debug.LogWarning("sending game server request to master");
				conn.Send(Spectrum.MsgTypes.SendGameServerIPToClient, c);
			}
		}

		public override void OnServerAddPlayer(NetworkConnection conn)
		{
			base.OnServerAddPlayer(conn);
			Spectrum.LogInformation("Server added player: " + conn.connectionId);
		}

		private void GameSceneLoaded(Scene arg0, LoadSceneMode arg1)
		{
			if (UseSpawnCamera)
			{
				GameObject.Find(SpawnCameraName).SetActive(false);
			}
			SceneManager.sceneLoaded -= GameSceneLoaded;
		}

		private void ReceivedGameServerToConnectTo(NetworkMessage netMsg)
		{	
			var c = netMsg.ReadMessage<StringMessage>();
			Spectrum.LogInformation("found game server to connect to: " + c.value);
			var d = c.value.Split(')');
			var address = d[0];
			var port = int.Parse(d[1]);
			StopClient();
			DoLoadGameScene = true;
			networkAddress = address;
			networkPort = port;
			StartClient();
		}

		public override void OnStartServer()
		{
			base.OnStartServer();
			GameServerConnectionToMasterBehaviour.Instance.GameServerAvailable();
		}

		public override void OnServerConnect(NetworkConnection conn)
		{
			base.OnServerConnect(conn);
			GameServerConnectionToMasterBehaviour.Instance.ClientConnectedToGameServer();
		}

		public override void OnServerDisconnect(NetworkConnection conn)
		{
			base.OnServerDisconnect(conn);
			GameServerConnectionToMasterBehaviour.Instance.ClientDisconnectedFromGameServer();
		}
	}
}